using System;
using DevLib.ModuleSystem;
using NKT.Fishing.Bait;
using UnityEngine;

namespace NKT.Player.Modules
{
    public class BaitModule : MonoBehaviour, IModule
    {
        [SerializeField] private BaitSO defaultBait;
        [SerializeField] private BaitSO currentBait;
        
        public event Action<BaitSO, int> OnBaitChange;//ui 용

        public BaitSO CurrentBait => currentBait != null ? currentBait : defaultBait;

        public int RemainingUses => currentBait != null ? _remainingUses : -1;

        private int _remainingUses;
        
        public void Initialize(ModuleOwner owner) { }

        public void Equip(BaitSO bait)
        {
            currentBait = bait;
            _remainingUses = bait.maxUses;
            
            OnBaitChange?.Invoke(bait, _remainingUses);
        }

        public void Consume()
        {
            if (currentBait == null || currentBait.isInfinite) return;

            _remainingUses--;
            
            if(_remainingUses <= 0)
                currentBait = null;
            
            OnBaitChange?.Invoke(CurrentBait, RemainingUses);
        }
    }
}