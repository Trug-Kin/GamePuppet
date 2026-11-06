using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class ConRoiR22Screen
{
    public static void Survey()
    {
        EditorSceneManager.OpenScene(ConRoiR22Scenery.Scene);var rows=new JArray();
        foreach(var f in Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include)){
            if(!f.sharedMesh||f.transform.root.name=="R22_Market_Front_Detail")continue;
            var r=f.GetComponent<Renderer>();if(!r||!r.enabled)continue;
            var mesh=f.sharedMesh;var v=mesh.vertices.Select(f.transform.TransformPoint).ToArray();var tris=new JArray();
            for(int sub=0;sub<mesh.subMeshCount;sub++){var tt=mesh.GetTriangles(sub);for(int i=0;i<tt.Length;i+=3){var c=(v[tt[i]]+v[tt[i+1]]+v[tt[i+2]])/3;if(c.x<777||c.x>804||c.y<17||c.y>22.5||c.z<140||c.z>160)continue;tris.Add(new JObject{["sub"]=sub,["indices"]=new JArray(tt[i],tt[i+1],tt[i+2]),["vertices"]=new JArray(Enumerable.Range(0,3).Select(j=>new JArray(v[tt[i+j]].x,v[tt[i+j]].y,v[tt[i+j]].z)))});}}
            if(tris.Count>0)rows.Add(new JObject{["name"]=f.transform.root.name+"/"+f.name,["mesh"]=AssetDatabase.GetAssetPath(mesh),["materials"]=new JArray(r.sharedMaterials.Select(m=>m.name)),["triangles"]=tris});
        }
        File.WriteAllText(ConRoiR22Scenery.Out+"/screen-survey.json",rows.ToString());
    }
}
