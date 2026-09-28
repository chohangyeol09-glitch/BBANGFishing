using System;
using System.IO;
using System.Linq;
using BBANGFishing.Audio;
using DevLib.EventChannelSystem;
using DevLib.ObjectPool.Runtime;
using DevLib.SoundSystem;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

[InitializeOnLoad]
public static class GameplayAudioSetup
{
    private const string Root = "Assets/Member/KTJ/Resources/GameplayAudio";
    private const string Request = "Temp/GameplayAudioSetup.request";
    static GameplayAudioSetup() { EditorApplication.update += CheckRequest; }

    private static void CheckRequest()
    {
        if (!File.Exists(Request) || EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;
        File.Delete(Request);
        try { Build(); File.WriteAllText("Temp/GameplayAudioSetup.result", "PASS: 9 clips, mixer routing, pool and manager references validated."); }
        catch (Exception e) { File.WriteAllText("Temp/GameplayAudioSetup.result", e.ToString()); Debug.LogException(e); }
    }

    [MenuItem("Tools/BBANGFishing/Setup Gameplay Audio")]
    public static void Build()
    {
        Directory.CreateDirectory(Root);
        AssetDatabase.Refresh();
        var library = Asset<GameplaySoundLibrary>("Library");
        var channel = Asset<EventChannelSO>("SoundChannel");
        var pool = Asset<PoolManagerSO>("SoundPool");
        var item = Asset<PoolItemSO>("SoundPoolItem");
        string[] filenames = {
            "드롭이 날아가서 획득되는 효과음.mp3", "물고기 피격 효과음.mp3", "물고기의 첨벙 효과음.mp3",
            "미니게임 good 효과음.wav", "미니게임 miss 효과음.wav", "미니게임 perfect 효과음.wav",
            "찌가 물에 떨어지는 효과음.wav", "패링 성공 효과음.mp3", "플레이어 사망 효과음.mp3"
        };
        library.clips = new SoundClipSo[filenames.Length];
        for (int i = 0; i < filenames.Length; i++)
        {
            var sound = Asset<SoundClipSo>(((GameplaySound)i).ToString());
            sound.clip = AssetDatabase.LoadAssetAtPath<AudioClip>("Assets/Member/KTJ/Assets/빵피싱 추가 사운드모음/" + filenames[i]);
            if (sound.clip == null) throw new Exception("Missing clip: " + filenames[i]);
            sound.audioType = AudioTypes.Sfx;
            sound.loop = false;
            sound.volume = 0.8f;
            sound.pitch = 1f;
            EditorUtility.SetDirty(sound);
            library.clips[i] = sound;
        }
        var mixer = AssetDatabase.LoadAssetAtPath<AudioMixer>("Assets/Member/JJK/05. Data/AudioMixer.mixer");
        var sfx = mixer.FindMatchingGroups("SFX").Where(group => group.name == "SFX").ToArray();
        var music = mixer.FindMatchingGroups("BGM").Where(group => group.name == "BGM").ToArray();
        if (sfx.Length != 1 || music.Length != 1) throw new Exception("Mixer groups missing or ambiguous.");
        var go = new GameObject("Gameplay Sound Player");
        try
        {
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            source.spatialBlend = 0f;
            source.outputAudioMixerGroup = sfx[0];
            var player = go.AddComponent<SoundPlayer>();
            player.PoolItem = item;
            Set(player, "sfxGroup", sfx[0]);
            Set(player, "musicGroup", music[0]);
            item.prefab = PrefabUtility.SaveAsPrefabAsset(go, Root + "/SoundPlayer.prefab");
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
        item.poolingName = "Gameplay SFX";
        item.initCount = 12;
        pool.itemList.Clear();
        pool.itemList.Add(item);
        go = new GameObject("Gameplay Sound Manager");
        try
        {
            var manager = go.AddComponent<SoundManager>();
            Set(manager, "poolManager", pool);
            Set(manager, "soundItem", item);
            Set(manager, "<SoundChannel>k__BackingField", channel);
            library.managerPrefab = PrefabUtility.SaveAsPrefabAsset(go, Root + "/SoundManager.prefab").GetComponent<SoundManager>();
        }
        finally { UnityEngine.Object.DestroyImmediate(go); }
        library.soundPool = pool;
        EditorUtility.SetDirty(item);
        EditorUtility.SetDirty(pool);
        EditorUtility.SetDirty(library);
        AssetDatabase.SaveAssets();
        if (library.managerPrefab.SoundChannel != channel || item.prefab.GetComponent<SoundPlayer>().PoolItem != item)
            throw new Exception("Invalid sound wiring.");
        Debug.Log("GAMEPLAY_AUDIO_SETUP_PASSED: 9 clips, existing SoundManager, SFX mixer, pooled playback.");
    }

    private static T Asset<T>(string name) where T : ScriptableObject
    {
        string path = Root + "/" + name + ".asset";
        var asset = AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset != null) return asset;
        asset = ScriptableObject.CreateInstance<T>();
        AssetDatabase.CreateAsset(asset, path);
        return asset;
    }

    private static void Set(UnityEngine.Object target, string property, UnityEngine.Object value)
    {
        var serialized = new SerializedObject(target);
        serialized.FindProperty(property).objectReferenceValue = value;
        serialized.ApplyModifiedPropertiesWithoutUndo();
    }
}
