using DevLib.ObjectPool.Runtime;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace CHG._02.Script.CombatSystem.HitFeedback
{
    public class DamageText : PoolableMono
    {
        [SerializeField] private PoolManagerSO poolManager;
        [SerializeField] private TextMeshPro text;
        [SerializeField] private float popScale = 1.6f;
        [SerializeField] private float riseHeight = 1.2f;
        [SerializeField] private float sideRange = 0.5f;

        private Sequence _seq;
        private Camera _cam;
        private bool _released;

        public void Show(float damage, Vector3 position)
        {
            _cam ??= Camera.main;
            _released = false;
            transform.position = position;
            transform.localScale = Vector3.zero;
            text.text = Mathf.RoundToInt(damage).ToString();
            text.alpha = 1f;

            Vector3 end = position + new Vector3(Random.Range(-sideRange, sideRange), riseHeight, 0f);
            
            _seq = DOTween.Sequence().SetUpdate(true) 
                .Append(transform.DOScale(popScale, 0.12f).SetEase(Ease.OutBack))   
                .Append(transform.DOScale(1f, 0.1f))
                .Join(transform.DOJump(end, 0.4f, 1, 0.6f))
                .Insert(0.45f, text.DOFade(0f, 0.3f))
                .OnComplete(Release);
        }
        
        private void LateUpdate()
        {
            if (_cam != null) transform.rotation = _cam.transform.rotation;
        }

        private void Release()
        {
            if (_released) return;
            _released = true;
            poolManager.Push(this);
        }

        public override void ResetItem() => _seq?.Kill();
        
    }
}