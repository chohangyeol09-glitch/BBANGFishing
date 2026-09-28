using DevLib.ObjectPool.Runtime;
using UnityEngine;

namespace CHG._02.Script.FishSystem
{
    /// <summary>
    /// 물고기가 죽은 자리에서 나와 flyDuration 동안 목적지로 날아간 뒤 풀로 돌아가는 오브젝트.
    /// DeathDropModule이 Pop해서 Fly를 부른다.
    /// </summary>
    public class FishDrop : PoolableMono
    {
        [Tooltip("시간(0~1)에 따른 진행도. 기본은 천천히 출발해 천천히 도착")]
        [SerializeField] private AnimationCurve moveCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
        [Tooltip("이 진행도부터 작아지기 시작해 도착할 때 사라진다. 1이면 줄어들지 않는다")]
        [SerializeField, Range(0f, 1f)] private float shrinkFrom = 0.8f;

        private PoolManagerSO _poolManager;
        private Transform _destination;
        private Vector3 _start, _lastEnd, _baseScale;
        private float _duration, _arcHeight, _elapsed;
        private bool _released = true;
        private FishDataSO _fishData;

        private void Awake() => _baseScale = transform.localScale;

        public void Fly(Vector3 start, Transform destination, float duration, float arcHeight, PoolManagerSO poolManager, FishDataSO fishData)
        {
            _poolManager = poolManager;
            _destination = destination;
            _start = start;
            _lastEnd = destination != null ? destination.position : start;
            _duration = Mathf.Max(0.01f, duration);
            _arcHeight = arcHeight;
            _elapsed = 0f;
            _released = false;
            _fishData = fishData;

            transform.position = start;
            transform.localScale = _baseScale;
        }

        private void Update()
        {
            if (_released) return;

            _elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(_elapsed / _duration);

            //목적지가 움직이면 따라가고, 도중에 사라지면 마지막 위치로 간다
            if (_destination != null) _lastEnd = _destination.position;

            float progress = moveCurve.Evaluate(t);
            Vector3 position = Vector3.LerpUnclamped(_start, _lastEnd, progress);
            position += Vector3.up * (_arcHeight * 4f * t * (1f - t)); //가운데서 가장 높은 포물선
            transform.position = position;

            if (shrinkFrom < 1f && t > shrinkFrom)
                transform.localScale = _baseScale * (1f - Mathf.InverseLerp(shrinkFrom, 1f, t));

            if (t >= 1f) Arrive();
        }
        
        private void Arrive()
        {
            if (_released) return;

            if (_fishData != null && !FishInventoryManager.Instance.AddFish(_fishData))
                Debug.LogWarning($"인벤토리에 {_fishData.FishName}을(를) 넣지 못했습니다 (가득 참).", this);

            Release();
        }

        private void Release()
        {
            if (_released) return;
            _released = true;
            

            if (_poolManager != null) _poolManager.Push(this);
            else Destroy(gameObject);
        }

        public override void ResetItem()
        {
            _released = true; //Fly가 불리기 전까지는 움직이지 않는다
            transform.localScale = _baseScale;
        }
    }
}
