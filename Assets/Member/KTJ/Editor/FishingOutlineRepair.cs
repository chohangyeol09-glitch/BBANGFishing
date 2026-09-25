using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FishingOutlineRepair
{
    const string Folder = "Assets/URP GanzSe Fantasy Low Poly Fishing Props/Prefabs";
    const string Output = "Assets/Member/KTJ/Materials/FishingOutlineMeshes";
    static FishingOutlineRepair() { EditorApplication.update += Run; }
    static void Run()
    {
        const string request = "Temp/FishingOutlineRepair.request";
        if (!File.Exists(request) || EditorApplication.isCompiling || EditorApplication.isUpdating || EditorApplication.isPlayingOrWillChangePlaymode) return;
        File.Delete(request);
        try
        {
            var original = AssetDatabase.LoadAssetAtPath<Material>("Assets/Member/KTJ/Materials/FishingPropsSoftToon.mat");
            if (!original) throw new Exception("Fishing material missing");
            Directory.CreateDirectory(Output);
            AssetDatabase.Refresh();
            var normal = MakeMaterial(original, "FishingPropsSmoothOutline", 20);
            var small = MakeMaterial(original, "FishingPropsSmallOutline", 2);
            var meshes = new Dictionary<Mesh, Mesh>();
            int count = 0, smallCount = 0;
            foreach (var path in Directory.GetFiles(Folder,"*.prefab",SearchOption.AllDirectories))
            {
                var root = PrefabUtility.LoadPrefabContents(path);
                try
                {
                    var renderers = root.GetComponentsInChildren<MeshRenderer>(true);
                    if (renderers.Length == 0) continue;
                    var bounds = renderers[0].bounds;
                    foreach (var r in renderers) bounds.Encapsulate(r.bounds);
                    bool isSmall = Mathf.Max(bounds.size.x, Mathf.Max(bounds.size.y,bounds.size.z)) < 0.5f;
                    foreach (var r in renderers)
                    {
                        var filter = r.GetComponent<MeshFilter>();
                        if (!filter || !filter.sharedMesh) continue;
                        var source = filter.sharedMesh;
                        if (!meshes.TryGetValue(source,out var baked))
                        {
                            baked = UnityEngine.Object.Instantiate(source);
                            baked.name = source.name + "_OutlineNormals";
                            var vertices = baked.vertices;
                            var normals = baked.normals;
                            if (vertices.Length != normals.Length) throw new Exception("Missing normals: " + source.name);
                            var sums = new Dictionary<Vector3,Vector3>();
                            for (int i=0;i<vertices.Length;i++)
                            {
                                sums.TryGetValue(vertices[i],out var sum);
                                sums[vertices[i]]=sum+normals[i];
                            }
                            var smooth = new List<Vector3>(vertices.Length);
                            for (int i=0;i<vertices.Length;i++)
                            {
                                var sum=sums[vertices[i]];
                                smooth.Add(sum.sqrMagnitude>0.000001f ? sum.normalized : normals[i]);
                            }
                            baked.SetUVs(7,smooth);
                            string assetPath=AssetDatabase.GenerateUniqueAssetPath(Output+"/"+baked.name.Replace('/','_')+".asset");
                            AssetDatabase.CreateAsset(baked,assetPath);
                            meshes.Add(source,baked);
                        }
                        filter.sharedMesh=baked;
                        var materials=r.sharedMaterials;
                        for(int i=0;i<materials.Length;i++) if(materials[i]==original) materials[i]=isSmall?small:normal;
                        r.sharedMaterials=materials;
                    }
                    PrefabUtility.SaveAsPrefabAsset(root,path);
                    count++;
                    if(isSmall) smallCount++;
                }
                finally { PrefabUtility.UnloadPrefabContents(root); }
            }
            AssetDatabase.SaveAssets();
            File.WriteAllText("Logs/FishingOutlineRepair.txt",$"PASS: prefabs={count}; small(width2)={smallCount}; smoothMeshes={meshes.Count}. Original surface normals, meshes and colliders preserved.");
        }
        catch(Exception e) { File.WriteAllText("Logs/FishingOutlineRepair.txt",e.ToString()); Debug.LogException(e); }
    }
    static Material MakeMaterial(Material source,string name,float width)
    {
        string path="Assets/Member/KTJ/Materials/"+name+".mat";
        var material=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(!material){material=new Material(source);AssetDatabase.CreateAsset(material,path);}
        MK.Toon.Properties.outlineData.SetValue(material,MK.Toon.OutlineData.Baked);
        MK.Toon.Properties.outlineSize.SetValue(material,width);
        EditorUtility.SetDirty(material);
        return material;
    }
}
