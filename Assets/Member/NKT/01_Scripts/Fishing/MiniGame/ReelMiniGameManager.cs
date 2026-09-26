using NKT.Player.Modules;
using UnityEngine;

namespace NKT.Fishing.MiniGame
{
    public class ReelMiniGameManager : MonoBehaviour
    {
        [SerializeField] private FishingModule fishingModule;
        [SerializeField] private ReelMiniGameBase[] miniGames;
    
        private ReelMiniGameBase _current;
        private int _lastIndex = -1;
    
        private void Awake()
        {
            fishingModule.OnStateChanged += OnStateChanged;
    
            foreach (ReelMiniGameBase game in miniGames)
            {
                game.OnFinished += OnGameFinished;
                game.gameObject.SetActive(false);
            }
        }
    
        private void OnDestroy()
        {
            if (fishingModule != null)
            {
                fishingModule.OnStateChanged -= OnStateChanged;
            }
    
            foreach (ReelMiniGameBase game in miniGames)
                if (game != null) game.OnFinished -= OnGameFinished;
        }
    
        private void OnStateChanged(FishingState state)
        {
            if (state == FishingState.Reeling) Begin();
            else End();
        }
    
        private void Begin()
        {
            _current = PickGame();
    
            if (_current == null)
            {
                fishingModule.ReportReelFinished(false);
                return;
            }
    
            _current.Show(fishingModule.Grade);
        }
    
        private void End()
        {
            if (_current == null) return;
    
            _current.Hide();
            _current = null;
        }
    
        private void OnGameFinished(bool success)
        {
            End();
            fishingModule.ReportReelFinished(success);
        }
    
        private ReelMiniGameBase PickGame()
        {
            if (miniGames == null || miniGames.Length == 0) return null;
            if (miniGames.Length == 1) return miniGames[0];
    
            int index;
            do { index = Random.Range(0, miniGames.Length); }
            while (index == _lastIndex);
    
            _lastIndex = index;
            return miniGames[index];
        }
    }
}