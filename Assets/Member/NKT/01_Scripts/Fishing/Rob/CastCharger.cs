using System;
using System.Collections;
using UnityEngine;

namespace NKT.Fishing.Rob
{
    public class CastCharger : MonoBehaviour
    {
        [SerializeField] private float speed = 1.5f;

        public event Action OnChargeStarted;        //차지를 시작했을때, ui 보여줄때 구독
        public event Action<float> OnValueChanged;  //차지 바 움직이는 값 전달용
        public event Action<float> OnCharged;       //끝났을때 최종 값 전달용이자 ui 사라지게
        
        private Coroutine _coroutine;
        private float _power;
        private bool _isCoroutine => _coroutine != null;
        
        //todo: 이걸 조작에 구독해서 End가 되면 차지한 값을 낚시대 던지는곳으로 보낸다
        [ContextMenu("Start")]
        public void ProgressStart()
        {
            if (_isCoroutine) return;
            
            _coroutine = StartCoroutine(ProgressCoroutine());
            OnChargeStarted?.Invoke();
        }

        [ContextMenu("End")]
        public void ProgressEnd()
        {
            if (!_isCoroutine) return;

            StopCoroutine(_coroutine);
            _coroutine = null;
            OnCharged?.Invoke(_power);
        }

        private IEnumerator ProgressCoroutine()
        {
            float timer = 0;

            while (true)
            {
                timer += Time.deltaTime * speed;
                _power = (1f - Mathf.Cos(timer * Mathf.PI)) * 0.5f; // cos은 -1~1이니까 0~2로하고 반으로 나눠서 0~1
                
                OnValueChanged?.Invoke(_power);
                
                yield return null;
            }
        }
    }
}