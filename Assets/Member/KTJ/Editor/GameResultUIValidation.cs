using System;
using System.Collections;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class GameResultUIValidation
{
    static GameResultUIValidation() { EditorApplication.update += Check; }
    private static void Check()
    {
        const string request = "Temp/GameResultUIValidation.request";
        if (!File.Exists(request) || EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;
        File.Delete(request);
        try { Validate(); File.WriteAllText("Temp/GameResultUIValidation.result", "PASS: scene bindings, 4/5 second holds, penalty order, insufficient money, full-health revive, sound reset, title build entry. Edit Mode only."); }
        catch (Exception e) { File.WriteAllText("Temp/GameResultUIValidation.result", e.ToString()); Debug.LogException(e); }
    }

    public static void Validate()
    {
        var scene = EditorSceneManager.OpenPreviewScene("Assets/Member/KU/01.Scenes/FirstScene_Final.unity");
        try
        {
            var ui = scene.GetRootGameObjects().SelectMany(root => root.GetComponentsInChildren<GameResultUI>(true)).Single();
            var so = new SerializedObject(ui);
            foreach (string property in new[] { "player", "moneyManager", "gameOverUI", "gameClearUI" })
                Require(so.FindProperty(property).objectReferenceValue != null, property + " unassigned");
            Require(so.FindProperty("gameOverHold").floatValue == 4f, "game over hold");
            Require(so.FindProperty("gameClearHold").floatValue == 5f, "game clear hold");
            Require(so.FindProperty("deathPenalty").intValue == 1000, "penalty");
            string title = so.FindProperty("titleScene").stringValue;
            Require(EditorBuildSettings.scenes.Any(s => s.enabled && s.path == title), "title build entry");
            var money = (MoneyManager)so.FindProperty("moneyManager").objectReferenceValue;
            var player = (NKT.Player.Player)so.FindProperty("player").objectReferenceValue;
            var over = (CanvasGroup)so.FindProperty("gameOverUI").objectReferenceValue;
            so.FindProperty("fadeDuration").floatValue = 0f;
            so.ApplyModifiedPropertiesWithoutUndo();
            foreach (int initialMoney in new[] { 3500, 500, 0 })
            {
                money.SetMoney(initialMoney);
                typeof(NKT.Player.Player).GetField("_deathSoundPlayed", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(player, true);
                var routine = (IEnumerator)typeof(GameResultUI).GetMethod("GameOverRoutine", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ui, null);
                Require(routine.MoveNext(), "fade-in step");
                Drain((IEnumerator)routine.Current);
                Require(over.alpha == 1f && over.blocksRaycasts, "visible overlay");
                Require(routine.MoveNext() && routine.Current is WaitForSecondsRealtime wait && wait.waitTime == 4f, "four-second wait");
                Require(money.Money == initialMoney, "penalty occurred too early");
                Require(routine.MoveNext(), "fade-out step");
                Require(money.Money == Mathf.Max(0, initialMoney - 1000), "penalty amount");
                Drain((IEnumerator)routine.Current);
                Require(!routine.MoveNext(), "unexpected extra steps");
                Require(over.alpha == 0f && !over.blocksRaycasts, "hidden overlay");
                Require(player.CurrentHealth == player.MaxHealth && !player.IsDead, "revive health");
                Require(!(bool)typeof(NKT.Player.Player).GetField("_deathSoundPlayed", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(player), "death audio reset");
            }
            // Verify clear UI and its hold, stopping before the actual scene transition.
            var clear = (CanvasGroup)so.FindProperty("gameClearUI").objectReferenceValue;
            var clearRoutine = (IEnumerator)typeof(GameResultUI).GetMethod("GameClearRoutine", BindingFlags.Instance | BindingFlags.NonPublic).Invoke(ui, null);
            Require(clearRoutine.MoveNext(), "clear fade");
            Drain((IEnumerator)clearRoutine.Current);
            Require(clear.alpha == 1f, "clear overlay");
            Require(clearRoutine.MoveNext() && clearRoutine.Current is WaitForSecondsRealtime clearWait && clearWait.waitTime == 5f, "five-second wait");
            Debug.Log("GAME_RESULT_UI_VALIDATION_PASSED (Edit Mode; no gameplay or scene transition executed)");
        }
        finally { EditorSceneManager.ClosePreviewScene(scene); }
    }
    private static void Drain(IEnumerator routine) { while (routine.MoveNext()) { } }
    private static void Require(bool condition, string message) { if (!condition) throw new Exception(message); }
}
