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
8. **`On StateChannel (Restart)` 노드의 `Assign State to …`는 반드시 블랙보드 `State` 변수에 링크한다** (링크 아이콘을 눌러 `State` 선택). 드롭다운에 `Jump` 같은 값이 보이면 링크가 안 된 상수라서 메시지의 값이 블랙보드에 들어가지 않고, `Switch`가 항상 처음 값의 가지만 실행한다. 저장된 에셋에서는 `LinkedVariable: rid: -2`(링크 없음)로 보인다. PDF 스크린샷에서는 이 칸에 `State`라는 변수 이름이 표시된다.
9. C#이 `_phase`처럼 자기 상태를 갖는 모듈(`LungeModule`)은 **그 값을 실제로 세팅하는지**를 확인한다. 값을 읽는 프로퍼티(`IsFlying` 등)만 만들고 세팅을 빠뜨리면 항상 `Idle`로 보여서 모듈이 아무것도 하지 않는다.
10. 노드 story의 단어가 블랙보드 변수 이름과 같으면 그 변수가 자동 연결된다. 새 변수를 원하면 다른 이름을 쓴다 (예: 블랙보드에 `State`가 있으니 노드 필드는 `NewState`).
11. `AnimationChannel`로 보내는 애니메이션 `Send`는 **`Repeat` 안에 두지 않는다.** `Repeat` 안의 노드는 매 프레임(또는 스킬이 끝날 때마다) 다시 실행되므로, `Combat` 카드에 `Send "Play Idle"`을 넣었을 때 스킬이 보낸 `SPIN`이 같은 순간 덮여서 보이지 않았다. 상태에 들어갈 때 한 번 재생할 애니메이션은 `Sequence [Send 애니메이션, Repeat …]`처럼 `Repeat` 위에 둔다. 보내는 해시마다 그 오너의 Animator에 같은 이름의 상태가 있어야 한다 (없으면 "Animator.GotoState: State could not be found"만 찍히고 재생되지 않는다).

---

## 8. 마이그레이션 체크리스트 (현재 구조 → 목표 구조)

**진행 상태**: 코드와 그래프 재구성은 끝났고(코드 리뷰와 저장된 그래프 값 확인 완료), 스폰 → 낙하 → 스킬 사용(투사체 발사)까지 실행 확인됨. 남은 것은 아래의 미완료 항목(런지/패링/사망/풀 재사용 실행 검증)이다. 옛 구조는 `Fish.ChangeState`가 C#에서 상태를 바꾸는 방식이었다.

- [x] **`FishStateEnum` / 채널**: 그대로 사용 (`Return`은 목표 구조에서도 사용)
- [x] **`Fish.cs`**: `HasStartedFalling`, `BindStateChannel`/`HandleStateChanged`/`SendState`, `Dead()`는 `SendState(Dead)`, `ChangeState` 삭제, `ResetItem` 초기화, `OnDestroy` 구독 해제
- [x] **`LungeModule.cs`**: 자체 비행 단계(`Idle/Approach/Return`), `_fish.ChangeState` 제거, `IsFlying/IsApproaching/IsReturning`, `HandleDeath`는 `_phase != Idle`일 때만 `EndFlight(true)`
- [x] **`FishFacingModule.cs`**: `_fish.IsDead`
- [x] **노드**: `FishFallingCondition`, `StartLungeAction`(접근 동안 `Running`), `WaitLungeEndAction`, 상태 전이는 기본 `Send Event Message`
- [x] **그래프** (`Fish BT.asset`): 시작점 2개(`On Start → Send Jump`, `On StateChannel (Restart)` → `Repeat → Switch`), `Assign State to State` 링크됨, 옛 `Pass If [State == Dead]` + Abort 구조 삭제됨
- [x] **`Repeat` 위치**: `Repeat`는 `Jump`와 `Combat` 가지 안에만 있다 (저장된 그래프로 확인: `Switch`의 자식이 `Repeat → Sequence`, `Repeat → Selector`). `Lunge`/`Return`/`Dead`는 한 번만 실행된다. `Switch` 위에 `Repeat`를 다시 두지 않는다.
- [x] **`Dead` 가지**: 지금은 `Log Message` → `Release Fish` 카드. 죽음 연출/드롭 같은 실제 죽음 로직은 이 카드의 `Release Fish` **앞**에 추가한다.
- [x] **검증** (사용자가 실행해서 확인): 스폰 → Jump → 낙하 시 Combat → 스킬 발사, 패링 성공 시 정상 복귀, 런지 도중 사망 처리, 풀에서 재사용해도 Jump부터 정상 동작
- [x] `CLAUDE.md`의 "Behavior graph" / "Object pooling" / "Fish" 항목을 새 구조로 갱신

**물고기 그래프 마이그레이션 완료.** 남은 후속 작업: 실제 죽음 연출을 `Dead` 카드에 추가, 재사용 시 스킬 쿨타임 초기화(필요할 때). 보스는 같은 패턴으로 옮겨졌다(§11).

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

**보스**: 같은 패턴으로 만들어져 있다. 구조와 규칙은 §11을 볼 것.

---

## 10. 확인하지 못한 것

- PDF 06·07에는 **피격/사망 상태를 채널로 보내는 방법**과 **애니메이션 채널 서브그래프의 상세**가 나오지 않는다(`Agent.OnHit/OnDeath`만 선언). 이 문서의 "외부 사건은 C#이 채널로 보낸다"는 부분은 PDF가 아니라 이 프로젝트에서의 설계 판단이다.
- (확인됨) 한 그래프에 시작점이 두 개(`On Start`, `On StateChannel`) 있는 구성은 저장된 `Fish BT.asset`에서 두 시작점이 **최상위 `ParallelAll` 아래로 묶여** 저장되는 것으로 확인했다. 즉 에디터가 자동으로 병렬 실행하도록 컴파일한다. 다만 `On Start`가 보낸 첫 `Jump` 메시지를 `On StateChannel`이 받는 것은 실행해서 확인해야 한다.
- 에디터의 노드/옵션 표기(`Pass If`, `Fail If`, Abort 옵션 이름 등)는 소스와 스크린샷 기준이라 실제 화면과 조금 다를 수 있다.

---

## 11. 보스 그래프 (`BossSystem`, `CHG/03.GameModule/Data/Boss/Boss BT.asset`)

물고기와 같은 패턴이다. 보스는 런지/점프/바다가 없고, 등장 → 일정 간격 패턴 공격 → (그로기) → 사망 흐름만 있다. 사용자가 플레이 모드에서 등장, 스킬 사용, 그로기(스킬 중단 포함), 패턴 파훼, 사망까지 동작을 확인했다(2026-09-25).

### 11.1 블랙보드

| 변수 | 타입 | 누가 채우나 |
|---|---|---|
| `Boss` | `Boss` | `Boss.OnSpawn` |
| `Target` | `GameObject` | `Boss.OnSpawn` |
| `State` | `BossStateEnum` (`Appear, Combat, Groggy, Dead`) | 루트 노드가 메시지로 대입 |
| `StateChannel` | `BossStateChannel : EventChannel<BossStateEnum>` | **에셋 할당하지 않음** (§3-5) |
| `AnimationChannel` | `AnimationChannel : EventChannel<HashDataSO>` | **에셋 할당하지 않음** (§3-5). `EnemyRenderer.BindChannel`이 `Boss.OnSpawn`에서 구독하고, 그래프 노드와 `AbstractEnemySkill`(경고 → 실행 애니메이션)이 보낸다. 정상 종료 후 `IDLE` 복귀는 Animator의 Exit Time 전환이 맡고, `StopSkill()`로 끊겼을 때만 코드가 `PlayIdle()`을 보낸다. 이 idle은 다음 상태 가지의 `Send`(예: 그로기 애니메이션)가 한 업데이트 뒤에 덮어쓴다. 물고기 그래프도 같은 이름·타입으로 가지고 있다 |
| `AppearDuration`, `GroggyDuration` | `float` | `Boss.OnSpawn`이 `BossDataSO` 값을 복사 (`Wait` 노드가 링크해서 씀) |

`Self`는 에디터 기본 변수(`GameObject`)다. **`SetVariableValue("Self", boss)`는 타입이 달라 조용히 무시되니 `"Boss"`를 쓴다** (실제로 이 실수로 `UseSkillAction`이 계속 `Failure`였다).

### 11.2 그래프

```
[On Start]
 └ Send "Appear" change on StateChannel

[On StateChannel (Restart)]   Assign State to → 블랙보드 State (링크)
 └ Switch [State]
     ├ Appear → ( Wait [AppearDuration] → Log → Send "Combat" )
     ├ Combat → Repeat → UseSkillAction ([Agent] = Boss, [Target] = Target)
     ├ Groggy → ( Wait [GroggyDuration] → [Boss] reset groggy → Log → Send "Combat" )
     └ Dead   → ( Log "보스 사망" )          ← 죽음 연출은 여기에 추가
```

- **`On StateChannel`의 Mode는 반드시 `Restart`.** `Default`면 자식이 실행 중일 때 온 메시지를 버린다. `Appear` 카드 안의 `Send "Combat"`이 무시되고 `Switch`가 끝난 뒤 영원히 대기하는 증상이 실제로 났다.
- `Repeat`는 `Combat` 가지에만 있다. 나머지는 한 번 실행 후 `Send`하거나(`Appear`, `Groggy`) 그대로 머문다(`Dead`).
- `UseSkillAction`의 빨간 X는 쿨타임 중이면 정상이다(`Repeat`가 매 프레임 재시도).

### 11.3 C#과 BT의 책임 분담

| 전이 | 누가 보내나 | 이유 |
|---|---|---|
| (시작) → `Appear` | 그래프 `On Start` | 초기 상태 |
| `Appear` → `Combat`, `Groggy` → `Combat` | 그래프 (`Wait` 뒤 `Send`) | 그래프 안의 흐름 |
| `Combat` → `Groggy` | **C#** `GroggyModule.AddGroggy` (게이지가 찼을 때 `Boss.SendState`) | 데미지/파훼라는 **외부 사건**. 그래프에서 `Pass If`로 검사하면 `UseSkillAction`이 `Running`인 동안 검사되지 않아 스킬 도중에 그로기가 걸리지 않는다 |
| → `Dead` | **C#** `Boss.Dead()` → `SendState(Dead)` | 외부 사건 |

- `Boss.State`는 `Fish.State`처럼 채널을 구독해서 따라가는 읽기용 복사본이다(`BindStateChannel`/`HandleStateChanged`, `OnStateChanged` 이벤트도 여기서 발생). C#은 블랙보드 `State`를 직접 쓰지 않는다.
- `Boss.OnSpawn`: `Boss`/`Target`/두 대기 시간 설정 → `BindStateChannel()` → `BTAgent.Restart()`. 스폰 전에 그래프가 한 번 돌며(`AppearDuration` 기본값 0) `Appear`를 즉시 통과하는 가짜 실행이 있지만, `Restart()`가 처음부터 다시 시작하므로 실제 등장 대기는 정상이다.
- 메시지가 오면 루트가 실행 중인 가지를 끝내므로, `Groggy`/`Dead` 진입 시 `UseSkillAction.OnEnd`가 `StopSkill()`을 부른다. 이미 발사된 투사체는 회수하지 않는다.

### 11.4 그로기 (`GroggyModule`)

- 게이지는 `OnDamaged`에서 `Damage × damageToGroggy`만큼, 그리고 외부에서 `AddGroggy(amount)`로 쌓인다.
- `State == Combat`이고 아직 차지 않았을 때만 쌓인다(등장/그로기/사망 중 무시). 보스를 죽인 공격은 `Dead()`가 먼저 실행되어 쌓이지 않는다.
- 가득 차면 `SendState(Groggy)`. 게이지는 그래프의 `ResetGroggyAction`(`[Boss] reset groggy`)이 그로기 끝에 비운다. **`Send "Combat"`은 카드의 마지막 줄**에 둔다.
- `BossDataSO`: `GroggyMaxGauge`, `GroggyDuration`.

### 11.5 패턴 파훼 (`PatternBreakModule`)

- 파훼 가능한 스킬은 `BossDataSO.BreakableSkills` (`BreakableSkillEntry { Skill(HashDataSO), BreakDamage, GroggyAmount }`)에 둔다. Fish와 공유하는 `EnemySkillDataSO`에는 넣지 않는다.
- 보스가 목록의 스킬을 쓰는 동안(`EnemySkillModule.CurrentSkill`, **경고 시간 포함**) 받은 데미지를 누적한다. 기준 이상이면 `CurrentSkill.StopSkill()` → `OnPatternBroken` → `GroggyModule.AddGroggy(GroggyAmount)` 순서로 처리한다(`StopSkill`을 먼저 해야 그로기 전이와 섞이지 않는다).
- 파훼는 상태 전이가 아니다. `Combat`에 머물고, `UseSkillAction`은 `CurrentSkill == null`을 보고 `Success`로 끝나 다음 스킬로 넘어간다. 파훼된 스킬도 쿨타임은 정상적으로 돈다.
- 누적은 `EnemySkillModule.OnSkillEnd`(정상/강제 종료 모두)에서 초기화된다. UI용으로 `OnBreakProgress`(0~1)를 낸다.
- 데미지 종류는 구분하지 않는다. "파훼 스킬로 준 데미지만 인정"이 필요해지면 공용 `DamageData`를 건드리지 않는 방식으로 따로 설계한다.

### 11.6 남은 작업

- [ ] `Boss.OnSpawn`의 `BTAgent.SetVariableValue("State", BossStateEnum.Appear)` 줄 삭제 (§11.3: C#은 `State`를 쓰지 않는다)
- [ ] `UseSkillAction.OnStart`의 디버그 로그(`UseSkill 실패: …`) 삭제
- [ ] 테스트용으로 바꾼 값(`GroggyDuration`, 스킬 `WarningTime`, `BreakDamage`) 정리
- [ ] (선택) 스폰 전 가짜 실행 제거: `BehaviorGraphAgent`를 꺼 두고 `OnSpawn`에서 `enabled = true` 후 `Restart()` — 실행 검증 필요
- [ ] `Dead` 가지의 실제 죽음 연출, 그로기 연출/애니메이션
- [ ] 이후 기능: 페이즈(우선 `HPBelowCondition` + `Priority`로), 오라 트랙, 약점 타격 인터럽트
