using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// Scene-local fixture. Source map meshes are clipped copies, never modified.
[DefaultExecutionOrder(-20000)]
public sealed class CanopyAccessFixture : MonoBehaviour
{
    public TextAsset fixture;
    public DieuKhienDiChuyen player;
    public string output="Docs/Reviews/ntmk_canopy_20261014_access_measurement/controller_180cm_runs";
    void Awake()
    {
        foreach(var script in player.GetComponentsInChildren<MonoBehaviour>(true))if(!(script is DieuKhienDiChuyen))script.enabled=false;
        var data=JObject.Parse(fixture.text);
        var shader=Shader.Find("Universal Render Pipeline/Lit");var mat=new Material(shader);mat.color=new Color(.55f,.58f,.55f);
        foreach(var item in data["meshes"])
        {
            var mesh=new Mesh{name=(string)item["name"],indexFormat=IndexFormat.UInt32};
            mesh.vertices=item["vertices"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();
            mesh.triangles=item["triangles"].Select(v=>(int)v).ToArray();mesh.RecalculateNormals();mesh.RecalculateBounds();
            var go=new GameObject(mesh.name);go.transform.SetParent(transform,false);go.layer=3;
            go.AddComponent<MeshFilter>().sharedMesh=mesh;go.AddComponent<MeshRenderer>().sharedMaterial=mat;
            go.AddComponent<MeshCollider>().sharedMesh=mesh;
        }
        var light=new GameObject("Fixture daylight").AddComponent<Light>();light.type=LightType.Directional;light.transform.rotation=Quaternion.Euler(45,-30,0);
        var run=gameObject.AddComponent<CanopyAccessRun>();run.player=player;run.output=Path.GetFullPath(output);
        run.points=data["route"].Select(v=>new Vector3((float)v[0],(float)v[1],(float)v[2])).ToArray();
        // Each capsule bottom starts 4 cm above the maximum support height
        // in its enclosing 0.60 m square, including the local floor slope.
        run.spawnPositions=new[] {
            new Vector3(-.273864746f,1.7155992f,.961761475f),
            new Vector3(4.38140869f,2.37433955f,-15.3883972f)
        };
        player.enabled=true;
        Physics.SyncTransforms();
    }
}
