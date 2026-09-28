using CHG._02.Script.BossSystem;
using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

//보스를 Depth만큼 아래에서 시작해 Duration 동안 제자리(스폰된 위치)까지 떠오르게 한다. Appear 가지에서 쓴다
[Serializable, GeneratePropertyBag]
[NodeDescription(name: "BossRiseAction", story: "[Boss] rises [Depth] over [Duration] seconds", category: "Action", id: "dd0a6a005d6d4b40830f113abff90ddd")]
public partial class BossRiseAction : Action
{
    [SerializeReference] public BlackboardVariable<Boss> Boss;
    [SerializeReference] public BlackboardVariable<float> Depth = new(5f); //얼마나 아래에서 올라오는가
    [SerializeReference] public BlackboardVariable<float> Duration; 

    private Transform _body;
    private Vector3 _start, _end;
    private float _elapsed;

    protected override Status OnStart()
    {
        if (Boss.Value == null) return Status.Failure;

        _body = Boss.Value.transform;
        _end = _body.position; //스폰된 자리가 최종 위치
        _start = _end + Vector3.down * Depth.Value;
        _body.position = _start;
        _elapsed = 0f;
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        _elapsed += Time.deltaTime;
        float t = Duration.Value > 0f ? Mathf.Clamp01(_elapsed / Duration.Value) : 1f;
        float eased = 1f - (1f - t) * (1f - t); //빠르게 솟다가 천천히 멈춘다
        _body.position = Vector3.LerpUnclamped(_start, _end, eased);
        return t >= 1f ? Status.Success : Status.Running;
    }

    protected override void OnEnd()
    {
        //도중에 끊겨도(그래프 재시작, 사망 등) 최종 위치로 맞춘다. 안 그러면 다음 OnStart가 낮은 위치를 최종 위치로 삼는다
        if (_body != null) _body.position = _end;
        _body = null;
    }
}
