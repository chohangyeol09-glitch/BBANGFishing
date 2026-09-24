# Fish / Boss Behavior Graph 설계 가이드 (강의 PDF 06·07 패턴)

이 문서는 사용자가 원하는 BT 사용 방식을 정리한 것이다. 출처는 강의 자료 **`06. 적 BT 제작하기_1 적의 기본 이동과 애니메이션 루트모션 확장`**, **`07. 적 BT 제작하기_2 센서와 공격`** (본문 텍스트와 그래프 스크린샷을 모두 확인함). 물고기와 보스의 Behavior 그래프를 만들거나 고칠 때는 **이 방식을 따른다.** `CLAUDE.md`의 "Behavior graph" 항목과 같이 읽을 것.

> 마이그레이션 상태는 §8 체크리스트에 있다. 그래프가 이 구조로 완전히 바뀌기 전까지는 `CLAUDE.md`에 적힌 "현재 구조"와 이 문서의 "목표 구조"가 다를 수 있다.

---

## 1. 한 줄 요약

> **상태(State)는 열거형 + 이벤트 채널로 관리하고, 상태 전이는 그래프 안의 노드가 채널에 메시지를 보내서 한다.**
> 루트 노드가 채널을 듣다가(`On StateChannel (Restart)`) 메시지가 오면 `Switch(State)`를 처음부터 다시 시작해서 새 상태의 가지로 넘어간다.

C# 코드가 `ChangeState(...)`로 상태를 바꾸고 BT는 읽기만 하는 방식이 **아니다.**

---

## 2. PDF의 구조

### 2.1 블랙보드 변수

| 변수 | 타입 | 용도 |
|---|---|---|
| `Enemy` | `AbstractEnemy` | 노드가 모듈에 접근하는 통로 (`Enemy.NavMovement`, `.Sensor`, `.SkillModule`, `.AgentRenderer`) |
| `State` | `EnemyState` (`[BlackboardEnum]`: IDLE, MOVE, COMBAT, HIT, DEATH) | 현재 상태. `Switch`가 이 값으로 분기 |
| `TargetGameObject` | `GameObject` | 감지한 대상 |
| `StateChannel` | `EventChannel<EnemyState>` (`[State] change`) | 상태 전이 메시지를 주고받는 채널 |
| `AnimationChannel` | `AnimParamSO`를 실어 나르는 채널 | 상태에 들어갈 때 애니메이션을 재생하라는 메시지 |

`[BlackboardEnum]` 속성이 있어야 열거형을 블랙보드 변수로 쓸 수 있다.

### 2.2 그래프에는 시작점이 두 개

```
[On Start] (Repeat 꺼짐)
 └ Send "IDLE" change on StateChannel          ← 초기 상태를 이벤트로 보낸다

[On StateChannel (Restart)]   "Assign State to State"
 └ Switch [State]
     ├ DEATH
     ├ HIT
     ├ COMBAT
     ├ MOVE
     └ IDLE
```

- `On StateChannel (Restart)`는 **`Start On Event Message`** 노드이고 Mode가 **Restart**이다. 메시지에 실려 온 `State` 값을 블랙보드 `State`에 대입한다(`Assign State to State`).
- 메시지가 올 때마다 자식(`Switch`)을 **끝내고 처음부터 다시 시작**하므로, 현재 실행 중이던 가지는 즉시 중단되고 새 상태의 가지가 시작된다.

### 2.3 상태 전이는 가지 안의 노드가 한다

각 상태 가지는 **카드(한 상자 안에 여러 줄 = 순서대로 실행)** 로 만들고, **마지막 줄에 다음 상태를 `Send`** 한다.

| 상태 | 가지 내용 (PDF) |
|---|---|
| IDLE | `Send IDLE anim` → `Wait between 2 and 3 seconds` → **`Send "MOVE" change`** |
| MOVE | `Send WALK anim` → `Enemy Move to point` → **`Send "IDLE" change`** |
| (감지 루프) | `Repeat` → `Try In Order` [ (`Enemy find TargetGameObject` → **`Send "COMBAT" change`**), `Wait 0.5` ] |
| COMBAT | `Send BATTLE_READY anim` → `Enemy rotate to Target` → `Repeat` → `Try In Order` [ `Fail If [Target in stopdistance]` → (`Send RUN anim` → `chase to Target`), (`Send BATTLE_READY anim` → `Branch On [Can use skill N to Target]`: False=`Wait 0.3` / True=`UseSkill`) ] |

핵심 규칙:
- 카드의 줄 중 하나라도 `Failure`면 카드 전체가 `Failure`이고, `Try In Order`는 다음 자식으로 넘어간다.
- 전이를 보내는 `Send`는 **카드의 마지막 줄**이다. 보낸 순간 루트가 이 가지를 끝내고 새 가지를 시작하기 때문이다.
- 상태 가지는 **(a) `Repeat`로 계속 돌거나 (b) 마지막에 다음 상태를 `Send`** 해야 한다. 둘 다 없으면 그 상태에서 멈춘다(§3의 4번).

### 2.4 C# 쪽 (PDF 범위)

- `AbstractEnemy : Agent`가 `BTAgent`(`BehaviorGraphAgent`)와 모듈 프로퍼티를 들고 있고, `Start()`에서 `BTAgent.SetVariableValue("Enemy", this)`를 한 번 한다.
- 노드는 `[Enemy]`를 받아 그 프로퍼티(`NavMovement`, `Sensor`, `SkillModule`, `AgentRenderer`)로 일한다. **노드 안에 게임 로직을 두지 않고 모듈을 호출만 한다.**
- 노드는 에디터의 **New Action**(문장을 쓰면 `[Enemy]` 같은 변수가 자동으로 생김)으로 만든다.
- `UseSkillAction`: `OnCurrentSkillEnd`를 구독해 끝까지 `Running`, `OnEnd`에서 구독 해제와 `StopSkillIfNotFinished`. `CanUseSkillCondition`은 `SkillModule.CanUseSkill`을 그대로 노출.
- `FindTargetAction`은 **이미 타겟이 있으면 `Failure`** 를 반환하도록 해서, 감지 루프가 계속 돌아도 안전하다.
- `Agent`에 `IsDead`, `OnHit`, `OnDeath`가 있고 "이벤트 발행은 나중에"라고만 나온다. **피격/사망 상태를 채널에 어떻게 보내는지는 06·07 범위 밖이다.**
- 채널 에셋의 인스펙터 값을 채운 뒤에 코드의 네임스페이스나 클래스 이름을 바꾸면 에러가 난다(PDF 명시).

---

## 3. 왜 이렇게 동작하는가 (패키지 소스로 확인한 사실, `com.unity.behavior@1.0.16`)

1. **`Switch`는 시작할 때 한 번만 가지를 고른다.** 값이 바뀌어도 이미 실행 중인 가지를 중단하지 않는다. 그래서 상태 전환을 즉시 반영하려면 `Start On Event Message`(Restart)로 `Switch`를 다시 시작해야 한다.
2. **Restart 모드**: 메시지가 오면 `EndNode(Child); StartNode(Child);`를 한다. 실행 중이던 노드는 `OnEnd`가 호출되므로 거기서 정리(스킬 중단 등)를 한다.
3. **`Send Event Message` 노드**는 `OnStart`에서 `Running`을 반환하고 **다음 `OnUpdate`에서** 전송한 뒤 `Success`를 반환한다(1틱 뒤 전송). 노드 안에서 자기 그래프를 재시작시키는 이벤트를 보내는 것은 소스 주석에도 지원하는 사용법으로 적혀 있다(`StartOnEvent<State> → Switch<State> → State 'A' → TriggerEvent 'To State B'`).
4. **`Start On Event Message`는 자식이 끝나면 다음 메시지를 기다린다.** 그래서 마지막에 `Send`가 없는 가지는 그 상태에 영원히 머문다.
5. **이벤트 채널 변수에 에셋을 할당하지 않으면 그래프 인스턴스마다 채널이 따로 생성된다**(`InitializeDefaultEventChannels`). 물고기 여러 마리가 서로의 상태 메시지를 받지 않는다. 에셋을 할당하면 전부가 공유하므로 **할당하지 않는다.**
6. `BehaviorGraphAgent`는 `OnEnable/OnDisable`을 처리하지 않는다. 풀에서 재사용할 때는 `BTAgent.Restart()`를 직접 불러야 한다.
7. 제네릭 채널 `EventChannel<T>`는 C#에서도 `Event`(`delegate void (T)`)로 구독하고 `SendEventMessage(T)`로 보낼 수 있다.

---

## 4. 우리 프로젝트와의 대응

| PDF | 우리 프로젝트 |
|---|---|
| `AbstractEnemy`, 블랙보드 `Enemy` | `Fish` (`Agent` 상속), 블랙보드 `Fish` (보스는 `Boss`) |
| `EnemyState` | `FishStateEnum` (`Jump, Combat, Lunge, Return, Dead`, `[BlackboardEnum]`), 보스는 `BossStateEnum` |
| `StateChannel` | `CHG._02.Script.CombatSystem.BT.Channel.StateChannel : EventChannel<FishStateEnum>` |
| `TargetGameObject` | 블랙보드 `Target` (`Fish.OnSpawn`이 채움) |
| `UseSkillAction` / `CanUseSkillCondition` | `UseSkillFromDataAction` (`FishDataSO.Skills` 기반), `CanUseFishSkill` 계열 |
| `FindTargetAction` + 센서 | 물고기는 타겟이 스포너에서 주어지므로 불필요 (보스가 필요하면 추가) |
| IDLE/MOVE 상태 | 물고기에는 없음. 대신 `Jump`(점프) 상태 |
| COMBAT의 `Branch On` | `Try In Order` + `Pass If [...]` (조건이 참인 가지 선택) |

---

## 5. 물고기 그래프 목표 구조

```
[On Start]
 └ Send "Jump" change on StateChannel

[On StateChannel (Restart)]  Assign State to State
 └ Switch [State]
     ├ Jump
     │   └ Repeat → Try In Order
     │        ├ Pass If [Fish started falling] → (Send "Combat" change)
     │        └ Wait 0.05 seconds
     ├ Combat
     │   └ Repeat → Try In Order
     │        ├ Pass If [Escaped to sea] → (Release Fish)
     │        ├ Pass If [Can lunge]      → (Send "Lunge" change)
     │        └ Pass If [Not in sea]     → UseSkillFromData
     ├ Lunge
     │   └ Try In Order
     │        ├ (Start Lunge → Send "Return" change)     ← 접근이 끝나거나 패링되면 Return
     │        └ (Send "Combat" change)                   ← StartLunge가 실패한 경우의 안전장치
     ├ Return
     │   └ (Wait Lunge End → Send "Combat" change)
     └ Dead
         └ (죽음 로직 → Release Fish)
```

`( … → … )`는 한 카드(순차 실행)이다.

---

## 6. C#과 BT의 책임 분담

| | 하는 일 |
|---|---|
| **BT** | 다음 상태를 **결정**하고 `Send`한다. 행동(스킬/런지)을 시작하고 끝까지 기다린다. |
| **C# (`Fish`, 모듈)** | 판단에 필요한 **사실(fact)** 을 제공한다: `IsInSea`, `IsDead`, `HasStartedFalling`, `LungeModule.IsFlying/IsApproaching/IsReturning`. 행동의 **실행**(물리, 비행 보간, 스킬 코루틴). |
| **C#이 채널로 직접 보내도 되는 경우** | **그래프 밖에서 생긴 외부 사건**(체력 0 → `Dead`, 나중에 피격/그로기)만. 그래프 안의 흐름 전이는 반드시 BT가 한다. |

`Fish.State`(C# 프로퍼티)는 넉백 게이트(`State == Combat`) 같은 C# 쪽 읽기용으로 남긴다. **단일 진실은 채널**이고, `Fish`는 채널을 구독해서 `State`를 따라간다(누가 보냈든 항상 일치).

```csharp
// Fish.cs — 채널을 구독해서 State를 미러링
private StateChannel _stateChannel;

private void BindStateChannel() // OnSpawn에서, BTAgent.Restart() 전에 호출
{
    if (_stateChannel == null)
    {
        if (!BTAgent.GetVariable("StateChannel", out BlackboardVariable<StateChannel> v) || v.Value == null)
        {
            Debug.LogError("StateChannel 변수를 찾을 수 없습니다.");
            return;
        }
        _stateChannel = v.Value;
    }
    _stateChannel.Event -= HandleStateChanged;
    _stateChannel.Event += HandleStateChanged;
}

private void HandleStateChanged(FishStateEnum newState) => State = newState;

public void SendState(FishStateEnum newState) // 외부 사건(사망 등)을 채널로 보낼 때만
{
    if (_stateChannel != null) _stateChannel.SendEventMessage(newState);
}

public override void Dead()
{
    base.Dead();
    SendState(FishStateEnum.Dead);
}
```

- `BTAgent.SetVariableValue("State", ...)`는 **C#에서 더 이상 하지 않는다.** 루트의 `Assign State to State`가 메시지로 대입한다.
- `OnDestroy`에서 `_stateChannel.Event -= HandleStateChanged;`.

---

## 7. 규칙과 함정

1. 상태 가지는 `Repeat`로 계속 돌거나, 마지막에 다음 상태를 `Send`하거나 둘 중 하나여야 한다 (§3-4).
2. 카드의 마지막 줄이 아닌 곳에서 상태를 `Send`하면 그 아래 줄은 실행되지 않는다(가지가 즉시 끝나므로).
3. 행동 노드(`StartLungeAction`, `UseSkillFromDataAction` 등)는 **`OnEnd`에서 정리**한다. 사망 같은 외부 전이가 가지를 강제로 끝내기 때문이다 (`UseSkillFromDataAction`은 `StopSkill()`).
4. `OnUpdate`는 `OnStart`가 `Running`을 반환했을 때만 호출된다. 즉시 끝나는 노드는 `OnStart`에서 `Success/Failure`를 반환한다.
5. 채널 변수(`StateChannel`)에 에셋을 할당하지 않는다 (§3-5).
6. 노드는 에디터에서 만들거나, 직접 만들 때는 **고유한 `id`(GUID)** 를 준다. 이미 그래프에 넣은 노드의 클래스 이름/네임스페이스를 바꾸지 않는다.
7. 풀링되는 물고기는 스폰마다 `BTAgent.Restart()`를 부른다 (§3-6). 초기 상태는 그래프의 `On Start → Send "Jump"`가 다시 보내준다.
8. 노드 story의 단어가 블랙보드 변수 이름과 같으면 그 변수가 자동 연결된다. 새 변수를 원하면 다른 이름을 쓴다 (예: 블랙보드에 `State`가 있으니 노드 필드는 `NewState`).

---

## 8. 마이그레이션 체크리스트 (현재 구조 → 목표 구조)

**현재 구조 (마이그레이션 전)**: 시작점 하나. `Try In Order` → [`Pass If [State == Dead]`(Abort Lower Priority), `Repeat → Switch(State)`]. 상태는 `Fish.ChangeState`가 C#에서 바꾸고(점프 종료, 런지 시작/복귀, 사망), 그 안에서 채널로도 보낸다(듣는 노드는 없음).

- [ ] **`FishStateEnum` / 채널**: 그대로 사용 (`Return`은 목표 구조에서도 사용)
- [ ] **`Fish.cs`**
  - [ ] `HasStartedFalling` 추가, `FixedUpdate`는 사실만 갱신 (`ChangeState` 호출 제거)
  - [ ] §6의 `BindStateChannel` / `HandleStateChanged` / `SendState` 추가, `Dead()`는 `SendState(Dead)`
  - [ ] `ChangeState` 삭제, `OnSpawn`의 `SetVariableValue("State", …)` 삭제, `ResetItem`에 `State = Jump; HasStartedFalling = false;`
- [ ] **`LungeModule.cs`**: 모듈 자체의 비행 단계(`Idle/Approach/Return`)를 두고 `_fish.ChangeState` 호출 제거. `IsFlying`, `IsApproaching`, `IsReturning` 공개. `IsParryable`은 `Approach` 단계로 판단. 비행이 끝나면 반드시 `Idle`로 되돌린다(예전 "영원히 얼어붙는" 버그의 원인이 이 값을 안 되돌린 것).
- [ ] **`FishFacingModule.cs`**: `State == Dead` 대신 `_fish.IsDead`
- [ ] **노드**
  - [ ] `FishFallingCondition` 신규 (`Fish.HasStartedFalling`)
  - [ ] `StartLungeAction`: 접근 단계가 끝날 때까지 `Running` (상태는 안 바꿈)
  - [ ] `WaitLungeEndAction` 신규: 비행이 끝날 때까지 `Running`
  - [ ] 상태 전이는 기본 제공 **`Send Event Message`** 사용
- [ ] **그래프** (`Fish BT.asset`): §5 구조로 재구성, 기존 `Pass If [State == Dead]` + Abort 구조 삭제
- [ ] **검증**: 스폰 → Jump → 낙하 시 Combat(넉백 켜짐) → 바다 접촉 시 Lunge → Return → Combat → 바다에서 반납, 그리고 런지/스킬 도중 사망 시 즉시 Dead 가지로
- [ ] `CLAUDE.md`의 "Behavior graph" 항목을 새 구조로 갱신하고 이 체크리스트를 정리

---

## 9. 새 상태나 노드를 추가할 때

**새 상태**
1. `FishStateEnum`(보스면 `BossStateEnum`)에 값 추가 → `Switch`에 포트가 생긴다.
2. 그 포트에 가지를 만든다. (a) `Repeat`로 계속 돌거나 (b) 마지막에 다음 상태를 `Send`.
3. 그 상태로 오는 전이 지점(다른 가지의 마지막 `Send`, 또는 C#의 외부 사건)을 추가한다.

**새 노드**
1. 에디터 **New Action / New Condition**에서 `[Fish] …` 문장으로 만든다.
2. 노드는 `Fish`(또는 `Boss`)를 받아 **모듈을 호출만** 한다. 지속되는 행동은 `Running`, 끝나면 `Success`, 시작 못 하면 `Failure`.
3. `OnEnd`에서 진행 중인 것을 정리한다.

**보스**: 같은 패턴을 쓴다. `BossStateEnum`(`Appear/Combat/Groggy/Dead` …)을 `[BlackboardEnum]`으로 두고 `BossStateChannel`을 만든다. 현재 `Boss.cs`/`GroggyModule.cs`의 C# `ChangeState`는 이 문서의 원칙(내부 흐름 전이는 BT, 외부 사건만 C#)에 맞게 옮겨야 한다. 단, 그로기 진입 같은 **외부 사건(게이지 누적)** 은 C#이 채널로 보내도 된다.

---

## 10. 확인하지 못한 것

- PDF 06·07에는 **피격/사망 상태를 채널로 보내는 방법**과 **애니메이션 채널 서브그래프의 상세**가 나오지 않는다(`Agent.OnHit/OnDeath`만 선언). 이 문서의 "외부 사건은 C#이 채널로 보낸다"는 부분은 PDF가 아니라 이 프로젝트에서의 설계 판단이다.
- 한 그래프에 시작점이 두 개(`On Start`, `On StateChannel`) 있는 구성은 PDF 스크린샷으로 확인했다. 런타임에서는 시작점마다 별도 그래프 모듈로 병렬 실행되는 것으로 보이나(`BehaviorGraphAgent`가 `m_Graph.Graphs`를 순회함) 우리 프로젝트에서 직접 실행해 보고 확인해야 한다.
- 에디터의 노드/옵션 표기(`Pass If`, `Fail If`, Abort 옵션 이름 등)는 소스와 스크린샷 기준이라 실제 화면과 조금 다를 수 있다.
