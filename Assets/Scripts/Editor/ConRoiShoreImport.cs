using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

public static class ConRoiShoreImport
{
    const string Folder="Assets/Art/Environment/LakeShoreV31";
    public static void ApplyContextPalette()
    {
        const string directory="Assets/Art/Environment/LakeGateMarket";
        var data=JObject.Parse(File.ReadAllText(directory+"/context.json"));
        var decisions=new JArray();
        foreach(var item in data["meshes"])
        {
            string source=(string)item["source_material"], key=source.ToLowerInvariant();
            Color? color=null;float smoothness=.12f;string role=null;
            if(key.StartsWith("roadsurface_"))
            {
                bool pedestrian=key.Contains("footway")||key.Contains("steps");
                color=pedestrian?new Color(.38f,.36f,.32f):new Color(.16f,.17f,.18f);
                role=pedestrian?"weathered_concrete":"asphalt";
            }
            else if(key.StartsWith("terrain_")){color=new Color(.28f,.30f,.22f);role="muted_earth_ground";}
            else if(key.StartsWith("island_estimated_green")){color=new Color(.15f,.24f,.09f);role="island_vegetation";}
            else if(key=="lake_flat"){color=new Color(.12f,.23f,.22f);smoothness=.8f;role="lake_water";}
            if(!color.HasValue)continue;
            var mat=AssetDatabase.LoadAssetAtPath<Material>(directory+"/"+(string)item["name"]+".mat");
            if(!mat)throw new Exception("Missing context material "+source);
            mat.color=color.Value;mat.SetFloat("_Smoothness",smoothness);EditorUtility.SetDirty(mat);
            decisions.Add(new JObject{["material"]=source,["role"]=role,["color_rgb"]=new JArray(color.Value.r,color.Value.g,color.Value.b)});
        }
        AssetDatabase.SaveAssets();
        File.WriteAllText("Docs/Reviews/lake_gate_market_current/unity_surface_palette.json",new JObject{["utc"]=DateTime.UtcNow.ToString("o"),["interpretation"]="Unity surface colors for source placeholder materials; original Blender materials and geometry preserved; no claim of surveyed finishes",["changed_materials"]=decisions}.ToString());
    }
    public static void ImportContext()
    {
        const string directory="Assets/Art/Environment/LakeGateMarket";
        string path=directory+"/LakeGateMarketContext.fbx";
        AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
        var importer=(ModelImporter)AssetImporter.GetAtPath(path);
        importer.globalScale=1;importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.None;
        importer.SaveAndReimport();
        var root=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        try
        {
            var data=JObject.Parse(File.ReadAllText(directory+"/context.json"));
            float maximum=0;int count=0;var checks=new JArray();
            foreach(var mf in root.GetComponentsInChildren<MeshFilter>(true))
            {
                var item=data["meshes"].Single(m=>(string)m["name"]==mf.name);
                Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
                var lo=P(item["unity_bounds"][0]);var hi=P(item["unity_bounds"][1]);
                var vv=mf.sharedMesh.vertices.Select(v=>mf.transform.TransformPoint(v)).ToArray();
                var bounds=new Bounds(vv[0],Vector3.zero);foreach(var v in vv)bounds.Encapsulate(v);
                float error=Mathf.Max((bounds.min-lo).magnitude,(bounds.max-hi).magnitude);maximum=Mathf.Max(maximum,error);
                mf.gameObject.layer=0;
                var color=item["color"];string matPath=directory+"/"+mf.name+".mat";
                var mat=AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Lit"));AssetDatabase.CreateAsset(mat,matPath);}
                mat.color=new Color((float)color[0],(float)color[1],(float)color[2],1);
                mat.SetFloat("_Smoothness",.12f);
                // Water is a Unity surface adaptation; Blender-only nodes are not portable.
                if(((string)item["source_material"]).ToLowerInvariant().Contains("water") || item["sources"].Any(s=>((string)s).Contains("Lake_XuanHuong")))
                {mat.color=new Color(.12f,.23f,.22f,1);mat.SetFloat("_Smoothness",.8f);}
                mf.GetComponent<Renderer>().sharedMaterial=mat;EditorUtility.SetDirty(mat);count++;
                checks.Add(new JObject{["mesh"]=mf.name,["bounds_error_m"]=error});
            }
            if(maximum>.003f)throw new Exception("Regional context coordinate mismatch "+maximum);
            if(root.GetComponentsInChildren<Collider>(true).Length!=0)throw new Exception("Visual context must not add collision");
            PrefabUtility.SaveAsPrefabAsset(root,directory+"/LakeGateMarketContext.prefab");AssetDatabase.SaveAssets();
            File.WriteAllText("Docs/Reviews/lake_gate_market_current/context_import.json",new JObject{["status"]="IMPORTED_VISUAL_CONTEXT_NO_ROUTE_VERDICT",["utc"]=DateTime.UtcNow.ToString("o"),["meshes"]=count,["max_bounds_error_m"]=maximum,["colliders"]=0,["checks"]=checks}.ToString());
            ApplyContextPalette();
        }
        finally{UnityEngine.Object.DestroyImmediate(root);}
    }
    public static void Import()
    {
        string path=Folder+"/LakeShoreV31.fbx";
        AssetDatabase.ImportAsset(path,ImportAssetOptions.ForceSynchronousImport);
        var importer=(ModelImporter)AssetImporter.GetAtPath(path);
        importer.globalScale=1;importer.addCollider=false;importer.materialImportMode=ModelImporterMaterialImportMode.None;
        importer.SaveAndReimport();
        var root=(GameObject)PrefabUtility.InstantiatePrefab(AssetDatabase.LoadAssetAtPath<GameObject>(path));
        try
        {
            var data=JObject.Parse(File.ReadAllText(Folder+"/landscape.json"));
            var checks=new JArray();float maximum=0;
            foreach(var mf in root.GetComponentsInChildren<MeshFilter>(true))
            {
                var item=data["meshes"].Single(m=>(string)m["name"]==mf.name);
                Vector3 P(JToken p)=>new Vector3((float)p[0],(float)p[1],(float)p[2]);
                var expected=item["vertices"].Select(P).ToArray();var actual=mf.sharedMesh.vertices.Select(v=>mf.transform.TransformPoint(v)).ToArray();
                var a=new Bounds(actual[0],Vector3.zero);var e=new Bounds(expected[0],Vector3.zero);
                foreach(var v in actual)a.Encapsulate(v);foreach(var v in expected)e.Encapsulate(v);
                float error=Mathf.Max((a.min-e.min).magnitude,(a.max-e.max).magnitude);maximum=Mathf.Max(maximum,error);
                checks.Add(new JObject{["mesh"]=mf.name,["bounds_error_m"]=error});
                mf.gameObject.layer=3;
                if((bool)item["collision"])mf.gameObject.AddComponent<MeshCollider>().sharedMesh=mf.sharedMesh;
                var color=item["color"];var materialName=string.Join("_",color.Take(3).Select(v=>Mathf.RoundToInt((float)v*255)));
                string matPath=Folder+"/Surface_"+materialName+".mat";
                var mat=AssetDatabase.LoadAssetAtPath<Material>(matPath);
                if(!mat){mat=new Material(Shader.Find("Universal Render Pipeline/Lit")){color=new Color((float)color[0],(float)color[1],(float)color[2])};mat.SetFloat("_Smoothness",.12f);AssetDatabase.CreateAsset(mat,matPath);}
                mf.GetComponent<Renderer>().sharedMaterial=mat;
            }
            if(maximum>.002f)throw new Exception("FBX world-coordinate mismatch: "+maximum);
            PrefabUtility.SaveAsPrefabAsset(root,Folder+"/LakeShoreV31.prefab");AssetDatabase.SaveAssets();
            File.WriteAllText("Docs/Reviews/lake_gate_market_current/unity_import.json",new JObject{["utc"]=DateTime.UtcNow.ToString("o"),["prefab"]=Folder+"/LakeShoreV31.prefab",["mesh_count"]=checks.Count,["maximum_bounds_error_m"]=maximum,["checks"]=checks,["status"]="IMPORTED_AND_PREFAB_SAVED_NOT_TRAVERSAL_VERDICT"}.ToString());
        }
        finally{UnityEngine.Object.DestroyImmediate(root);}
    }
}
