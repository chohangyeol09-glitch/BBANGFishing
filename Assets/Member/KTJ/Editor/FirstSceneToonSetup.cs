using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using P = MK.Toon.Properties;

// Editor-only: generated materials and renderer overrides belong to this scene.
[InitializeOnLoad]
public static class FirstSceneToonSetup
{
    const string ScenePath = "Assets/Member/KTJ/Scene/FirstScene_copy.unity";
    const string Output = "Assets/Member/KTJ/Materials/FirstSceneToon";
    const string Request = "Temp/FirstSceneToon.request";
    const string Report = "Logs/FirstSceneToon.txt";

    static FirstSceneToonSetup() { EditorApplication.delayCall += ProcessRequest; }

    static void ProcessRequest()
    {
        if (!File.Exists(Request)) return;
        if (EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode)
        { EditorApplication.delayCall += ProcessRequest; return; }
        File.Delete(Request);
        try { Apply(); }
        catch (Exception e) { Directory.CreateDirectory("Logs"); File.WriteAllText(Report, e.ToString()); Debug.LogException(e); }
    }

    [MenuItem("Tools/BBANG Fishing/Apply FirstScene Soft Toon")]
    public static void Apply()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) throw new InvalidOperationException("Exit Play Mode first.");
        var shader = Shader.Find("MK/Toon/URP/Standard/Simple");
        var outlineShader = Shader.Find("MK/Toon/URP/Standard/Simple + Outline");
        if (!shader || !outlineShader) throw new InvalidOperationException("MK Toon URP shaders missing.");
        var guiType = AppDomain.CurrentDomain.GetAssemblies().Select(a => a.GetType("MK.Toon.Editor.URP.StandardSimpleEditor"))
            .First(t => t != null);
        var gui = (ShaderGUI)Activator.CreateInstance(guiType, true);
        var scene = SceneManager.GetSceneByPath(ScenePath);
        bool opened = !scene.IsValid() || !scene.isLoaded;
        if (opened) scene = EditorSceneManager.OpenScene(ScenePath, OpenSceneMode.Additive);
        Directory.CreateDirectory("Logs");
        if (!File.Exists("Logs/FirstScene_copy.before-toon.unity")) File.Copy(ScenePath, "Logs/FirstScene_copy.before-toon.unity");
        Directory.CreateDirectory(Output);
        AssetDatabase.Refresh();
        var cache = new Dictionary<string, Material>();
        var lines = new List<string>();
        int changed = 0, slots = 0, outlined = 0;
        foreach (var renderer in scene.GetRootGameObjects().SelectMany(o => o.GetComponentsInChildren<Renderer>(true)))
        {
            if (!(renderer is MeshRenderer) && !(renderer is SkinnedMeshRenderer)) continue;
            if (renderer.GetComponentInParent<Canvas>() || renderer.GetComponent<TMPro.TMP_Text>()) continue;
            var hierarchy = new List<string>();
            bool important = renderer is SkinnedMeshRenderer;
            for (var t = renderer.transform; t; t = t.parent)
            {
                hierarchy.Add(t.name);
                important |= t.GetComponents<MonoBehaviour>().Any(c => c && (c.GetType().Name == "InteractionTarget" || c.GetType().Name == "Player"));
            }
            var objectPath = string.Join("/", hierarchy.AsEnumerable().Reverse());
            important = true; // Scene style: outlines on all converted objects.
            if (hierarchy.Any(n => n.IndexOf("water", StringComparison.OrdinalIgnoreCase) >= 0)) continue;
            var materials = renderer.sharedMaterials;
            bool modified = false;
            for (int i = 0; i < materials.Length; i++)
            {
                var src = materials[i];
                if (!src) continue;
                var srcPath = AssetDatabase.GetAssetPath(src);
                if (srcPath.StartsWith(Output + "/", StringComparison.Ordinal)) continue;
                string sn = src.shader.name;
                bool supported = sn == "Universal Render Pipeline/Lit" || sn == "Universal Render Pipeline/Simple Lit" ||
                    sn == "Universal Render Pipeline/Unlit" || sn == "Standard" || sn.StartsWith("MK/Toon/URP/Standard/") ||
                    sn == "BluBlu/SG_Lit" || sn == "BluBlu/SG_Foliage" || sn == "BluBlu/VertexColor" || sn == "BluBlu/RockMoss";
                bool transparent = (src.HasProperty("_Surface") && src.GetFloat("_Surface") > 0) || src.renderQueue >= 3000;
                if (!supported || transparent || src.name.IndexOf("water", StringComparison.OrdinalIgnoreCase) >= 0)
                { lines.Add("KEPT " + objectPath + " | " + src.name + " | " + sn); continue; }
                AssetDatabase.TryGetGUIDAndLocalFileIdentifier(src, out string guid, out long localId);
                string key = guid + "_" + localId + (important ? "_Outline" : "_Simple");
                if (!cache.TryGetValue(key, out var dst))
                {
                    dst = new Material(src) { name = src.name + (important ? "_SoftToon_Outline" : "_SoftToon") };
                    gui.AssignNewShaderToMaterial(dst, src.shader, important ? outlineShader : shader);
                    string colorProperty = src.HasProperty("_BaseColor") ? "_BaseColor" : src.HasProperty("_AlbedoColor") ? "_AlbedoColor" : "_Color";
                    if (src.HasProperty(colorProperty)) P.albedoColor.SetValue(dst, src.GetColor(colorProperty));
                    string map = src.HasProperty("_BaseMap") ? "_BaseMap" : src.HasProperty("_AlbedoMap") ? "_AlbedoMap" : "_MainTex";
                    if (src.HasProperty(map))
                    {
                        P.albedoMap.SetValue(dst, src.GetTexture(map));
                        dst.SetTextureScale("_AlbedoMap", src.GetTextureScale(map));
                        dst.SetTextureOffset("_AlbedoMap", src.GetTextureOffset(map));
                    }
                    if (sn.StartsWith("BluBlu/"))
                    {
                        P.emissionColor.SetValue(dst, Color.black);
                        P.emissionMap.SetValue(dst, null);
                        if (sn == "BluBlu/SG_Lit") P.albedoColor.SetValue(dst, Color.white);
                        if (sn == "BluBlu/SG_Foliage")
                        {
                            P.albedoMap.SetValue(dst, src.GetTexture("Texture2D_CAD82441"));
                            Color tint = src.GetColor("Color_369F793F"); tint.a = 1;
                            P.albedoColor.SetValue(dst, tint);
                            P.alphaClipping.SetValue(dst, true);
                            P.alphaCutoff.SetValue(dst, 0.5f);
                            dst.SetInt("_RenderFace", 0);
                        }
                        if (sn == "BluBlu/VertexColor" || sn == "BluBlu/RockMoss")
                        {
                            P.albedoColor.SetValue(dst, Color.white);
                            P.albedoMap.SetValue(dst, null);
                        }
                    }
                    P.light.SetValue(dst, MK.Toon.Light.Cel);
                    P.lightThreshold.SetValue(dst, 0.45f);
                    P.diffuseSmoothness.SetValue(dst, 0.3f);
                    P.goochBrightColor.SetValue(dst, new Color(1f, 0.97f, 0.90f, 1));
                    P.goochDarkColor.SetValue(dst, new Color(0.32f, 0.39f, 0.48f, 1));
                    P.specular.SetValue(dst, MK.Toon.Specular.Off);
                    P.rim.SetValue(dst, MK.Toon.Rim.Off);
                    if (important)
                    {
                        P.outline.SetValue(dst, MK.Toon.Outline.HullClip);
                        P.outlineSize.SetValue(dst, 20f);
                        P.outlineColor.SetValue(dst, new Color(0.12f, 0.16f, 0.18f, 1));
                    }
                    gui.ValidateMaterial(dst);
                    var safeName = string.Concat(src.name.Select(c => char.IsLetterOrDigit(c) || c == '_' ? c : '_'));
                    string path = Output + "/" + safeName + "_" + key + ".mat";
                    if (AssetDatabase.LoadAssetAtPath<Material>(path)) throw new InvalidOperationException("Output already exists: " + path);
                    AssetDatabase.CreateAsset(dst, path);
                    cache.Add(key, dst);
                    lines.Add("MATERIAL " + srcPath + " -> " + path);
                }
                if (sn == "BluBlu/VertexColor" || sn == "BluBlu/RockMoss") BakeMeshColors(renderer, src, sn);
                materials[i] = dst;
                modified = true;
                slots++;
                if (important) outlined++;
            }
            if (!modified) continue;
            Undo.RecordObject(renderer, "Apply scene soft toon");
            renderer.sharedMaterials = materials;
            PrefabUtility.RecordPrefabInstancePropertyModifications(renderer);
            EditorUtility.SetDirty(renderer);
            changed++;
        }
        AssetDatabase.SaveAssets();
        foreach (var mat in cache.Values)
        {
            if (ShaderUtil.ShaderHasError(mat.shader)) throw new InvalidOperationException("Shader error: " + mat.shader.name);
            if (Mathf.Abs(mat.GetFloat("_LightThreshold") - 0.45f) > 0.001f) throw new InvalidOperationException("Threshold mismatch");
        }
        EditorSceneManager.MarkSceneDirty(scene);
        if (!EditorSceneManager.SaveScene(scene)) throw new IOException("Scene save failed.");
        lines.Insert(0, $"SUCCESS: renderers={changed}, slots={slots}, materials={cache.Count}, outlinedSlots={outlined}");
        File.WriteAllLines(Report, lines);
        Debug.Log(lines[0]);
        try
        {
            var camera = scene.GetRootGameObjects().SelectMany(o => o.GetComponentsInChildren<Camera>(true)).FirstOrDefault(c => c.enabled);
            if (camera)
            {
                var previousTarget = camera.targetTexture;
                var previousActive = RenderTexture.active;
                var target = new RenderTexture(1280, 720, 24);
                var image = new Texture2D(1280, 720, TextureFormat.RGB24, false);
                try
                {
                    camera.targetTexture = target;
                    camera.Render();
                    RenderTexture.active = target;
                    image.ReadPixels(new Rect(0, 0, 1280, 720), 0, 0);
                    image.Apply();
                    File.WriteAllBytes("Logs/FirstSceneToon-preview.png", image.EncodeToPNG());
                }
                finally
                {
                    camera.targetTexture = previousTarget;
                    RenderTexture.active = previousActive;
                    UnityEngine.Object.DestroyImmediate(image);
                    UnityEngine.Object.DestroyImmediate(target);
                }
            }
        }
        catch (Exception e) { File.AppendAllText(Report, "\nPreview unavailable: " + e.Message); }
        SceneView.RepaintAll();
        if (opened) EditorSceneManager.CloseScene(scene, true);
    }

    // Preserve the mountain palette and static rock/moss colors in scene-local meshes.
    static void BakeMeshColors(Renderer renderer, Material src, string shaderName)
    {
        var filter = renderer.GetComponent<MeshFilter>();
        if (!filter || !filter.sharedMesh) throw new InvalidOperationException("Missing mesh: " + renderer.name);
        var mesh = UnityEngine.Object.Instantiate(filter.sharedMesh);
        mesh.name = filter.sharedMesh.name + "_SoftToonColors";
        var oldColors = mesh.colors;
        var normals = mesh.normals;
        var colors = new Color[mesh.vertexCount];
        for (int i = 0; i < colors.Length; i++)
        {
            if (shaderName == "BluBlu/VertexColor")
            {
                var c = oldColors.Length == colors.Length ? oldColors[i] : Color.white;
                colors[i] = c.r * src.GetColor("_VertexColorRed") + c.g * src.GetColor("_VertexColorGreen") + c.b * src.GetColor("_VertexColorBlue");
            }
            else
            {
                float up = renderer.transform.localToWorldMatrix.inverse.transpose.MultiplyVector(normals[i]).normalized.y;
                float mask = Mathf.Clamp01((up + 1f) * 0.5f * (src.GetFloat("_CoverStr") * 2f + 25f) - 25f);
                colors[i] = Color.Lerp(src.GetColor("_BaseColor"), src.GetColor("_CoverTint"), mask);
            }
            colors[i].a = 1;
        }
        mesh.colors = colors;
        string path = AssetDatabase.GenerateUniqueAssetPath(Output + "/" + mesh.name + ".asset");
        AssetDatabase.CreateAsset(mesh, path);
        Undo.RecordObject(filter, "Preserve scene toon colors");
        filter.sharedMesh = mesh;
        PrefabUtility.RecordPrefabInstancePropertyModifications(filter);
        EditorUtility.SetDirty(filter);
    }
}
