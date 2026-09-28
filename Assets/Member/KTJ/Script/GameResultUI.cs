using System.Collections;
using CHG._02.Script.BossSystem;
using Member.JJK._02._Scripts.Settings;
using NKT.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(10000)]
public sealed class GameResultUI : MonoBehaviour
{
    [SerializeField] private Player player;
    [SerializeField] private MoneyManager moneyManager;
    [SerializeField] private CanvasGroup gameOverUI;
    [SerializeField] private CanvasGroup gameClearUI;
    [SerializeField] private SettingsMenuUI settingsMenu;
    [SerializeField, Min(0f)] private float fadeDuration = 0.5f;
    [SerializeField, Min(0f)] private float gameOverHold = 4f;
    [SerializeField, Min(0f)] private float gameClearHold = 5f;
    [SerializeField, Min(0)] private int deathPenalty = 1000;
    [SerializeField] private string titleScene = "Assets/Member/KTJ/Scene/TitleScene.unity";

    private bool _presenting;
    private bool _clearPending;
    private bool _leavingScene;
    private bool _settingsWasEnabled;
    private float _previousTimeScale;

    private void Awake()
    {
        Hide(gameOverUI);
        Hide(gameClearUI);
        if (settingsMenu == null)
            settingsMenu = FindFirstObjectByType<SettingsMenuUI>(FindObjectsInactive.Include);
    }

    private void OnEnable()
    {
        if (player != null) player.OnDeath += HandlePlayerDeath;
        Boss.OnBossDefeated += HandleFinalBossDeath;
    }

    private void HandlePlayerDeath()
    {
        if (_presenting || _leavingScene) return;
        BeginPresentation();
        StartCoroutine(GameOverRoutine());
    }

    private void HandleFinalBossDeath(Boss boss)
    {
        if (_leavingScene || boss.gameObject.scene != gameObject.scene) return;
        if (_presenting)
        {
            _clearPending = true;
            return;
        }
        BeginPresentation();
        StartCoroutine(GameClearRoutine());
    }

    private void BeginPresentation()
    {
        _presenting = true;
        // A hit-stop may already have temporarily set the time scale to zero.
        _previousTimeScale = SettingsMenuUI.IsOpen ? Time.timeScale : (Time.timeScale > 0f ? Time.timeScale : 1f);
        if (settingsMenu != null)
        {
            _settingsWasEnabled = settingsMenu.enabled;
            settingsMenu.enabled = false;
        }
        player.LockLook();
        Time.timeScale = 0f;
    }

    private void LateUpdate()
    {
        // HitStopManager restores time scale in Update; keep the result screen paused.
        if (_presenting) Time.timeScale = 0f;
    }

    private IEnumerator GameOverRoutine()
    {
        Prepare(gameOverUI);
        yield return Fade(gameOverUI, 1f);
        yield return new WaitForSecondsRealtime(gameOverHold);
        moneyManager.SetMoney(moneyManager.Money - deathPenalty);
        yield return Fade(gameOverUI, 0f);
        Hide(gameOverUI);
        player.Revive();
        if (_clearPending)
        {
            _clearPending = false;
            yield return GameClearRoutine();
            yield break;
        }
        EndPresentation();
    }

    private IEnumerator GameClearRoutine()
    {
        Prepare(gameClearUI);
        yield return Fade(gameClearUI, 1f);
        yield return new WaitForSecondsRealtime(gameClearHold);
        if (!Application.CanStreamedLevelBeLoaded(titleScene))
        {
            Debug.LogError($"Title scene is missing from Build Settings: {titleScene}", this);
            Hide(gameClearUI);
            EndPresentation();
            yield break;
        }
        _leavingScene = true;
        EndPresentation();
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        SceneManager.LoadSceneAsync(titleScene);
    }

    private IEnumerator Fade(CanvasGroup group, float target)
    {
        float start = group.alpha;
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, Mathf.Clamp01(elapsed / fadeDuration));
            yield return null;
        }
        group.alpha = target;
    }

    private static void Prepare(CanvasGroup group)
    {
        group.gameObject.SetActive(true);
        group.transform.SetAsLastSibling();
        group.blocksRaycasts = true;
        group.interactable = false;
    }

    private static void Hide(CanvasGroup group)
    {
        if (group == null) return;
        group.alpha = 0f;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

    private void EndPresentation()
    {
        if (!_presenting) return;
        _presenting = false;
        Time.timeScale = _previousTimeScale;
        if (player != null) player.UnlockLook();
        if (settingsMenu != null) settingsMenu.enabled = _settingsWasEnabled;
    }

    private void OnDisable()
    {
        if (player != null) player.OnDeath -= HandlePlayerDeath;
        Boss.OnBossDefeated -= HandleFinalBossDeath;
        StopAllCoroutines();
        Hide(gameOverUI);
        Hide(gameClearUI);
        _clearPending = false;
        EndPresentation();
    }
}
