using System;
using CHG._02.Script.CoreSystem;
using UnityEngine;

namespace NKT.Fishing.MiniGame
{
    public abstract class ReelMiniGameBase : MonoBehaviour
    {
        public event Action<bool> OnFinished;

        public void Show(Grade grade)
        {
            gameObject.SetActive(true);
            StartGame(grade);
        }

        public void Hide()
        {
            StopGame();
            gameObject.SetActive(false);
        }
        
        protected abstract void StartGame(Grade grade);
        protected abstract void StopGame();
        
        protected void Finish(bool success) => OnFinished?.Invoke(success);
    }
}