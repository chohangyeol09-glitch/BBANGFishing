using System.Linq;
using CHG._02.Script.Agents;
using DevLib.EventChannelSystem;
using DevLib.ModuleSystem;
using DevLib.ObjectPool.Runtime;
using DG.Tweening;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.HitFeedback
{
    public class HitFeedbackModule : Module, IAfterInitModule
{
    [Header("Flash")]
    [SerializeField] private Material flashMaterial;
    [SerializeField] private float flashDuration = 0.1f;

    [Header("Scale")]
    [SerializeField] private Transform visualRoot;   
    [SerializeField] private float punchAmount = 0.1f;
    [SerializeField] private float punchDuration = 0.15f;

    [Header("Spawn")]
    [SerializeField] private PoolManagerSO poolManager;
    [SerializeField] private PoolItemSO hitParticleItem;
    [SerializeField] private PoolItemSO damageTextItem;
    [SerializeField] private Vector3 textOffset = new(0f, 0.5f, 0f);

    [Header("Hit Stop")]
    [SerializeField] private EventChannelSO feedbackChannel;
    [SerializeField] private float hitStopDuration = 0.08f;
    [SerializeField] private float killHitStopDuration = 0.5f;

    private Agent _agent;
    private Renderer[] _renderers;
    private Material[][] _baseMats, _flashMats;
    private Material _flashInstance;
    private Vector3 _baseScale;
    private Tween _flashTween, _scaleTween;
    private readonly HitStopEvent _hitStopEvt = new();
    
    public override void Initialize(ModuleOwner owner)
    {
        base.Initialize(owner);
        _agent = owner as Agent;
        _baseScale = visualRoot.localScale;

        _flashInstance = new Material(flashMaterial);
        _renderers = visualRoot.GetComponentsInChildren<Renderer>();
        _baseMats = new Material[_renderers.Length][];
        _flashMats = new Material[_renderers.Length][];
        for (int i = 0; i < _renderers.Length; i++)
        {
            _baseMats[i] = _renderers[i].sharedMaterials;
            _flashMats[i] = _baseMats[i].Append(_flashInstance).ToArray();
        }
    }

    public void AfterInit() => _agent.OnDamaged += HandleDamaged;

    private void HandleDamaged(DamageData data)
    {
        Flash();
        PunchScale();
        SpawnParticle(data);
        SpawnText(data);

        _hitStopEvt.Duration = _agent.IsDead ? killHitStopDuration : hitStopDuration;
        feedbackChannel.RaiseEvent(_hitStopEvt);
    }

    private void Flash()
    {
        _flashTween?.Kill();
        SetFlashSlot(true);
        _flashInstance.SetFloat("_FlashAmount", 1f);
        _flashTween = _flashInstance.DOFloat(0f, "_FlashAmount", flashDuration)
            .SetUpdate(true)
            .OnComplete(() => SetFlashSlot(false));
    }

    private void PunchScale()
    {
        _scaleTween?.Kill();
        visualRoot.localScale = _baseScale;
        _scaleTween = visualRoot.DOPunchScale(_baseScale * punchAmount, punchDuration, 1, 0f)
            .SetUpdate(true);
    }

    private void SpawnParticle(DamageData data)
    {
        var blood = poolManager.Pop<PoolParticle>(hitParticleItem);
        blood.transform.SetPositionAndRotation(data.HitPoint,
            Quaternion.LookRotation(data.HitDirection.sqrMagnitude > 0f ? data.HitDirection : Vector3.up));
    }

    private void SpawnText(DamageData data)
    {
        var text = poolManager.Pop<DamageText>(damageTextItem);
        text.Show(data.Damage, data.HitPoint + textOffset);
    }

    private void SetFlashSlot(bool on)
    {
        for (int i = 0; i < _renderers.Length; i++)
            _renderers[i].sharedMaterials = on ? _flashMats[i] : _baseMats[i];
    }

    public void ResetFeedback()
    {
        _flashTween?.Kill();
        _scaleTween?.Kill();
        SetFlashSlot(false);
        visualRoot.localScale = _baseScale;
    }

    private void OnDestroy()
    {
        if (_agent != null) _agent.OnDamaged -= HandleDamaged;
        _flashTween?.Kill();
        _scaleTween?.Kill();
        if (_flashInstance != null) Destroy(_flashInstance);
    }
}
}