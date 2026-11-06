using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEngine;

[DefaultExecutionOrder(-10000)]
public class DaLatRayMissProbe : MonoBehaviour
{
    public DieuKhienDiChuyen player;
    public string outputFolder;
    public bool runOnPlay;
    readonly HashSet<string> contacts=new HashSet<string>();
    readonly List<string> contactDetails=new List<string>();
    readonly List<Triangle> triangles=new List<Triangle>();
    struct Triangle {public Vector3 a,b,c;public string owner;}
    bool oldBackground,oldMouse,finished;
    DieuKhienGocNhin mouse;
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.normal.y>.1f && ((1<<hit.collider.gameObject.layer)&player.groundMask.value)!=0)
        {
            contacts.Add(hit.collider.name);
            if(contactDetails.Count<12)contactDetails.Add(hit.collider.name+" point="+hit.point.ToString("F7")+" normal="+hit.normal.ToString("F7"));
        }
    }
    IEnumerator Start()
    {
        if(!runOnPlay){enabled=false;yield break;}
        oldBackground=Application.runInBackground;Application.runInBackground=true;
        mouse=player.GetComponent<DieuKhienGocNhin>();oldMouse=mouse.enabled;mouse.enabled=false;
        var report=new JObject();int called=0;
        player.movementInputForTesting=()=>{called++;return new Vector2(.1234f,.5678f);};
        player.enableAutomatedTestInput=false;player.ReadMovementInput();
        report["hook_disabled_did_not_call_provider"]=called==0;
        player.enableAutomatedTestInput=true;Vector2 injected=player.ReadMovementInput();
        report["hook_enabled_test_scene_called_provider"]=called==1&&injected==new Vector2(.1234f,.5678f);
        var other=new GameObject("Non-test-scene hook guard fixture");
        var scene=UnityEngine.SceneManagement.SceneManager.CreateScene("HookGuardOutsideTest");
        UnityEngine.SceneManagement.SceneManager.MoveGameObjectToScene(other,scene);
        var outside=other.AddComponent<DieuKhienDiChuyen>();outside.enabled=false;
        outside.enableAutomatedTestInput=true;outside.movementInputForTesting=()=>{called++;return Vector2.one;};
        outside.ReadMovementInput();report["hook_outside_test_scene_ignored"]=called==1;
        Destroy(other);UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
        player.movementInputForTesting=()=>Vector2.zero;player.enableAutomatedTestInput=true;
        var colliders=FindObjectsByType<MeshCollider>(FindObjectsSortMode.None).Where(c=>c.gameObject.scene==gameObject.scene).ToArray();
        foreach(var col in colliders)
        {
            var vertices=col.sharedMesh.vertices.Select(v=>col.transform.TransformPoint(v)).ToArray();
            var indices=col.sharedMesh.triangles;
            for(int i=0;i<indices.Length;i+=3)triangles.Add(new Triangle{a=vertices[indices[i]],b=vertices[indices[i+1]],c=vertices[indices[i+2]],owner=col.name});
        }
        report["query"]=new JObject{["direction"]=new JArray(0,-1,0),["length_m"]=20,["origin_above_foot_m"]=5,["layer_mask"]=8,["trigger_interaction"]="Ignore",["queries_hit_backfaces"]=Physics.queriesHitBackfaces};
        var inputs=JObject.Parse(File.ReadAllText(outputFolder+"/miss_samples.json"));
        var results=new JArray();
        foreach(var sample in inputs["samples"])
        {
            Vector3 position=new Vector3((float)sample["position"]["x"],(float)sample["position"]["y"],(float)sample["position"]["z"]);
            float foot=(float)sample["foot_y"];Vector3 origin=new Vector3(position.x,foot+5,position.z);
            var ray=new Ray(origin,Vector3.down);var detail=new JObject{["position"]=JArray.FromObject(new[]{position.x,position.y,position.z}),["origin"]=JArray.FromObject(new[]{origin.x,origin.y,origin.z}),["cases"]=sample["cases"].DeepClone(),["recorded_miss_frames"]=sample["frame_count"]};
            bool baseline=Physics.Raycast(ray,out var floor,20,8,QueryTriggerInteraction.Ignore);
            detail["baseline_hit"]=baseline;detail["baseline_collider"]=baseline?floor.collider.name:null;
            detail["per_collider_ray_hits"]=new JArray(colliders.Where(c=>c.Raycast(ray,out var h,20)).Select(c=>c.name));
            detail["overlap_ground_check_supports"]=new JArray(Physics.OverlapSphere(new Vector3(position.x,foot+.1f,position.z),.4f,8,QueryTriggerInteraction.Ignore).Select(c=>c.name).Distinct());
            var sphereHits=Physics.SphereCastAll(new Vector3(position.x,foot+.6f+5,position.z),.6f,Vector3.down,10,8,QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance).ToArray();
            detail["capsule_foot_spherecast_supports"]=new JArray(sphereHits.Select(h=>new JObject{["collider"]=h.collider.name,["distance_m"]=h.distance,["point"]=JArray.FromObject(new[]{h.point.x,h.point.y,h.point.z})}));
            detail["geometric_covering_owners"]=new JArray(Coverage(position.x,position.z));
            double nearest=double.MaxValue;double nx=0,nz=0;string nearOwner="";
            var ownerDistances=new Dictionary<string,double>();
            foreach(var t in triangles)
            foreach(var edge in new[]{(t.a,t.b),(t.b,t.c),(t.c,t.a)})
            {
                double ax=edge.Item1.x,az=edge.Item1.z,dx=edge.Item2.x-ax,dz=edge.Item2.z-az;
                double u=(dx*dx+dz*dz)>0?Math.Max(0,Math.Min(1,((position.x-ax)*dx+(position.z-az)*dz)/(dx*dx+dz*dz))):0;
                double ex=ax+u*dx,ez=az+u*dz;double d=Math.Sqrt(Math.Pow(position.x-ex,2)+Math.Pow(position.z-ez,2));
                if(!ownerDistances.ContainsKey(t.owner)||d<ownerDistances[t.owner])ownerDistances[t.owner]=d;
                if(d<nearest){nearest=d;nx=position.x-ex;nz=position.z-ez;nearOwner=t.owner;}
            }
            detail["nearest_mesh_edge_distance_m"]=nearest;detail["nearest_owner"]=nearOwner;
            detail["nearest_owner_edges"]=new JArray(ownerDistances.OrderBy(k=>k.Value).Take(4).Select(k=>new JObject{["collider"]=k.Key,["distance_m"]=k.Value}));
            if(nearest>0){nx/=nearest;nz/=nearest;}else {nx=1;nz=0;}
            double? left=Boundary(position.x,position.z,-nx,-nz),right=Boundary(position.x,position.z,nx,nz);
            detail["gap_cross_section_direction_xz"]=new JArray(nx,nz);
            detail["mesh_gap_cross_section_width_m"]=left.HasValue&&right.HasValue?left.Value+right.Value:(double?)null;
            detail["negative_side_covered_within_2cm"]=left.HasValue;detail["positive_side_covered_within_2cm"]=right.HasValue;
            var offsets=new JArray();
            foreach(float delta in new[]{-.01f,-.001f,-.0005f,-.0002f,-.0001f,0,.0001f,.0002f,.0005f,.001f,.01f})
            {
                var o=origin+new Vector3((float)(nx*delta),0,(float)(nz*delta));
                bool hit=Physics.Raycast(o,Vector3.down,out var shifted,20,8,QueryTriggerInteraction.Ignore);
                offsets.Add(new JObject{["requested_offset_m"]=delta,["actual_origin"]=JArray.FromObject(new[]{o.x,o.y,o.z}),["hit"]=hit,["collider"]=hit?shifted.collider.name:null});
            }
            detail["offset_rays"]=offsets;
            detail["classification"]=!baseline&&Coverage(position.x,position.z).Count==0?"Geometric projected gap/boundary; original ray mask/direction/length are valid":"Inspect query/cooking/precision if mesh covers but ray misses";
            contacts.Clear();contactDetails.Clear();player.controller.enabled=false;player.transform.position=position;player.transform.rotation=Quaternion.identity;
            typeof(DieuKhienDiChuyen).GetField("velocity",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).SetValue(player,Vector3.zero);
            player.controller.enabled=true;Physics.SyncTransforms();yield return new WaitForSeconds(.5f);
            detail["actual_capsule_ground_contacts"]=new JArray(contacts.OrderBy(s=>s));detail["contact_details"]=new JArray(contactDetails);
            detail["replay_grounded"]=player.controller.isGrounded;detail["replay_displacement_m"]=Vector3.Distance(player.transform.position,position);
            results.Add(detail);
        }
        report["samples"]=results;report["status"]="TARGETED_PLAY_MODE_PROBE_COMPLETE";
        Directory.CreateDirectory(outputFolder);File.WriteAllText(outputFolder+"/ray_probe_results.json",report.ToString());
        finished=true;CleanUp();Debug.Log("DALAT_RAY_MISS_PROBE_COMPLETED");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#endif
    }
    List<string> Coverage(double x,double z)
    {
        var owners=new HashSet<string>();
        foreach(var t in triangles)
        {
            double denom=(t.b.z-t.c.z)*(t.a.x-t.c.x)+(t.c.x-t.b.x)*(t.a.z-t.c.z);
            if(Math.Abs(denom)<1e-15)continue;
            double a=((t.b.z-t.c.z)*(x-t.c.x)+(t.c.x-t.b.x)*(z-t.c.z))/denom;
            double b=((t.c.z-t.a.z)*(x-t.c.x)+(t.a.x-t.c.x)*(z-t.c.z))/denom;
            if(a>=-1e-12&&b>=-1e-12&&1-a-b>=-1e-12)owners.Add(t.owner);
        }
        return owners.ToList();
    }
    double? Boundary(double x,double z,double dx,double dz)
    {
        if(Coverage(x,z).Count>0)return 0;
        double hi=0;
        for(int i=1;i<=200;i++){hi=i*.0001;if(Coverage(x+dx*hi,z+dz*hi).Count>0)break;if(i==200)return null;}
        double lo=hi-.0001;
        for(int i=0;i<18;i++){double mid=(lo+hi)/2;if(Coverage(x+dx*mid,z+dz*mid).Count>0)hi=mid;else lo=mid;}
        return hi;
    }
    void CleanUp(){if(player){player.movementInputForTesting=null;player.enableAutomatedTestInput=false;}if(mouse)mouse.enabled=oldMouse;Application.runInBackground=oldBackground;}
    void OnDisable(){if(runOnPlay)CleanUp();}
}
