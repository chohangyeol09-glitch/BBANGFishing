using System.Collections;
using CHG._02.Script.CoreSystem;
using UnityEngine;

namespace NKT.Fishing.MiniGame
{
    public abstract class MultiStepMiniGame : ReelMiniGameBase
    {
        [SerializeField] private Vector2Int requireRange = new Vector2Int(3, 6);
        [SerializeField] private float finishDelay = 0.6f;
        [SerializeField] private int allowedMiss = 2;
        
        protected int RequiredSuccess { get; private set; }
        protected bool IsDone => _done;
        public int Success => _success;
        public int MissCount => _miss;

        private int _success;
        private int _miss;
        private bool _done;
        
        protected override void StartGame(Grade grade)
        {
            float t = (int)grade / 3f;
            
            VariableReset(t);

            OnGameStart(grade, t);
            NextStep();
        }

        private void VariableReset(float t)
        {
            RequiredSuccess = Mathf.RoundToInt(Mathf.Lerp(requireRange.x, requireRange.y, t));
            _success = 0;
            _miss = 0;
            _done = false;
        }

        protected override void StopGame()
        {
            _done = true;
            OnGameStop();
        }

        protected void ReportStep(bool ok)
        {
            if (_done) return;
            
            if (ok) _success++;
            else _miss++;

            if (_miss > allowedMiss)
            {
                _done = true;
                StartCoroutine(FinishAfterDelay(false));
                return;
            }

            if (_success >= RequiredSuccess)
            {
                _done = true;
                StartCoroutine(FinishAfterDelay(true));
                return;
            }
            
            NextStep();
        }
        private IEnumerator FinishAfterDelay(bool success)
        {
            yield return new WaitForSeconds(finishDelay);

            Finish(success);
        }
        
        
        protected abstract void OnGameStart(Grade grade, float f);
        protected abstract void NextStep();
        private void OnGameStop() { }
    }
}