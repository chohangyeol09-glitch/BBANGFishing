using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class SkyboxRecoveryValidation
{
    static SkyboxRecoveryValidation() { EditorApplication.update += Run; }
    static void Run()
    {
        const string request = "Temp/SkyboxRecoveryValidation.request";
        if (!File.Exists(request) || EditorApplication.isCompiling || EditorApplication.isUpdating) return;
        File.Delete(request);
        var lines = new List<string>();
        try
        {
            AssetDatabase.Refresh(ImportAssetOptions.ForceSynchronousImport);
            var paths = Directory.GetFiles("Assets/LaFinca/Midgard Skybox Prime/Skyboxs", "*.mat", SearchOption.AllDirectories);
            Material sample = null;
            int valid = 0;
            foreach (var raw in paths)
            {
                string path = raw.Replace('\\','/');
                var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (!mat || !mat.shader || !mat.shader.isSupported || !mat.HasProperty("_Tex"))
                { lines.Add("FAIL material/shader: " + path); continue; }
                var cube = mat.GetTexture("_Tex") as Cubemap;
                if (!cube || cube.width == 0)
                { lines.Add("FAIL cubemap: " + path); continue; }
                if (ShaderUtil.ShaderHasError(mat.shader)) { lines.Add("FAIL shader compile: " + path); continue; }
                valid++;
                if (!sample) sample = mat;
            }
            if (sample) RenderSample(sample);
            lines.Insert(0, $"Materials={paths.Length}; LoadedShaderAndCubemap={valid}; Failed={paths.Length-valid}");
            File.WriteAllLines("Logs/SkyboxRecoveryValidation.txt", lines);
        }
        catch (Exception e) { File.WriteAllText("Logs/SkyboxRecoveryValidation.txt", "FAIL: " + e); Debug.LogException(e); }
    }

    static void RenderSample(Material material)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        var previous = RenderTexture.active;
        bool asyncCompile = ShaderUtil.allowAsyncCompilation;
        var target = new RenderTexture(800,450,24);
        var image = new Texture2D(800,450,TextureFormat.RGB24,false);
        Camera camera = null;
        try
        {
            ShaderUtil.allowAsyncCompilation = false;
            var go = new GameObject("Skybox recovery verification",typeof(Camera),typeof(Skybox));
            SceneManager.MoveGameObjectToScene(go,scene);
            camera = go.GetComponent<Camera>();
            camera.scene = scene;
            camera.clearFlags = CameraClearFlags.Skybox;
            go.GetComponent<Skybox>().material = material;
            camera.targetTexture = target;
            camera.Render();
            RenderTexture.active = target;
            image.ReadPixels(new Rect(0,0,800,450),0,0);
            image.Apply();
            File.WriteAllBytes("Logs/SkyboxRecovery-preview.png",image.EncodeToPNG());
        }
        finally
        {
            if (camera) camera.targetTexture = null;
            RenderTexture.active=previous;
            ShaderUtil.allowAsyncCompilation=asyncCompile;
            UnityEngine.Object.DestroyImmediate(image);
            UnityEngine.Object.DestroyImmediate(target);
            EditorSceneManager.ClosePreviewScene(scene);
        }
    }
}
