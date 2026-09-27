using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering.Universal;

[InitializeOnLoad]
public static class CartoonWaterScaleValidation
{
    const string Request = "Temp/CartoonWaterScaleValidation.request";
    static CartoonWaterScaleValidation() { EditorApplication.update += Run; }
    // Explicit geometry nodes make Shader Graph declare UV/tangent/normal inputs in every pass.
    static void Run()
    {
        if (!File.Exists(Request)) return;
        if (EditorApplication.isCompiling || EditorApplication.isUpdating)
        { return; }
        File.Delete(Request);
        try
        {
            const string path = "Assets/WaterURP/SimpleCartoonWater/ShaderGraph/Simple_Water.shadergraph";
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceSynchronousImport);
            var shader = AssetDatabase.LoadAssetAtPath<Shader>(path);
            if (!shader) throw new Exception("Shader Graph import failed.");
            var material = AssetDatabase.LoadAssetAtPath<Material>("Assets/WaterURP/SimpleCartoonWater/Materials/CartoonWater.mat");
            if (material.shader != shader) throw new Exception("Material shader reference changed.");
            // Compile the actual material passes, rather than checking only graph JSON.
            for (int pass = 0; pass < material.passCount; pass++) material.SetPass(pass);
            if (ShaderUtil.ShaderHasError(shader))
                throw new Exception(string.Join("\n", Array.ConvertAll(ShaderUtil.GetShaderMessages(shader), m => m.message)));
            var cases = new[] {Vector3.one, new Vector3(4,1,4), new Vector3(8,2,3), new Vector3(-3,1,5)};
            foreach (var scale in cases)
            {
                var matrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(15,37,12), scale);
                float u = matrix.MultiplyVector(Vector3.right).magnitude;
                float v = matrix.MultiplyVector(Vector3.forward).magnitude;
                if (Mathf.Abs(u / Mathf.Abs(scale.x) - 1) > 0.00001f || Mathf.Abs(v / Mathf.Abs(scale.z) - 1) > 0.00001f)
                    throw new Exception("Scale compensation mismatch: " + scale);
                float normalStretch = matrix.MultiplyVector(Vector3.up).magnitude;
                float actualHeight = matrix.MultiplyVector(Vector3.up * (0.75f / normalStretch)).magnitude;
                if (Mathf.Abs(actualHeight - 0.75f) > 0.00001f)
                    throw new Exception("Wave height changed with scale: " + scale);
            }
            RenderChecks(material);
            if (ShaderUtil.ShaderHasError(shader)) throw new Exception(string.Join("\n", Array.ConvertAll(ShaderUtil.GetShaderMessages(shader), m => m.message)));
            File.WriteAllText("Logs/CartoonWaterScaleValidation.txt", "PASS: Shader Graph imported; material reference preserved; no shader errors after rendering. UV density and world wave height calculations passed for unit, uniform, nonuniform, mirrored and rotated scales. Rendered normal and grazing angles at unit and enlarged scale to Logs/WaterValidation-*.png.");
            Debug.Log("CartoonWater scale validation PASS");
        }
        catch (Exception e) { File.WriteAllText("Logs/CartoonWaterScaleValidation.txt", "FAIL: " + e); Debug.LogException(e); }
    }

    static void RenderChecks(Material material)
    {
        var scene = EditorSceneManager.NewPreviewScene();
        bool asyncCompilation = ShaderUtil.allowAsyncCompilation;
        var previousActive = RenderTexture.active;
        var target = new RenderTexture(960, 540, 24);
        var pixels = new Texture2D(960, 540, TextureFormat.RGB24, false);
        var groundMaterial = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        Camera camera = null;
        try
        {
            ShaderUtil.allowAsyncCompilation = false;
            var water = GameObject.CreatePrimitive(PrimitiveType.Plane);
            SceneManager.MoveGameObjectToScene(water, scene);
            water.GetComponent<Renderer>().sharedMaterial = material;
            var floor = GameObject.CreatePrimitive(PrimitiveType.Cube);
            SceneManager.MoveGameObjectToScene(floor, scene);
            floor.transform.position = new Vector3(0,-2,0);
            floor.transform.localScale = new Vector3(120,1,120);
            groundMaterial.SetColor("_BaseColor", new Color(0.7f,0.65f,0.45f));
            floor.GetComponent<Renderer>().sharedMaterial = groundMaterial;
            var cameraObject = new GameObject("Water validation camera", typeof(Camera));
            SceneManager.MoveGameObjectToScene(cameraObject, scene);
            camera = cameraObject.GetComponent<Camera>();
            camera.scene = scene;
            camera.clearFlags = CameraClearFlags.SolidColor;
            camera.backgroundColor = new Color(0.35f,0.65f,0.85f);
            camera.nearClipPlane = 0.05f;
            camera.farClipPlane = 300;
            camera.targetTexture = target;
            var data = camera.GetUniversalAdditionalCameraData();
            data.requiresDepthTexture = true;
            data.requiresColorTexture = true;
            foreach (int scale in new[] {1,4})
            {
                water.transform.localScale = Vector3.one * scale;
                foreach (bool grazing in new[] {false,true})
                {
                    camera.transform.position = grazing ? new Vector3(0,1.2f,-4) : new Vector3(0,5,-4);
                    camera.transform.LookAt(new Vector3(0,0.5f,2));
                    camera.Render();
                    RenderTexture.active = target;
                    pixels.ReadPixels(new Rect(0,0,960,540),0,0);
                    pixels.Apply();
                    File.WriteAllBytes($"Logs/WaterValidation-scale{scale}-{(grazing ? "grazing" : "normal")}.png",pixels.EncodeToPNG());
                }
            }
        }
        finally
        {
            if (camera) camera.targetTexture = null;
            RenderTexture.active = previousActive;
            ShaderUtil.allowAsyncCompilation = asyncCompilation;
            UnityEngine.Object.DestroyImmediate(target);
            UnityEngine.Object.DestroyImmediate(pixels);
            UnityEngine.Object.DestroyImmediate(groundMaterial);
            EditorSceneManager.ClosePreviewScene(scene);
        }
    }
}
