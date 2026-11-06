using System.Linq;
using UnityEngine;
public static class ConRoiV48R21TerrainGround
{
 public static MeshCollider[] CreateProbes()
 {
  return GameObject.Find("R20_Whole_Source_Export").GetComponentsInChildren<MeshFilter>(true).Where(f=>f.name=="Surface_142"||f.name=="Surface_176").Select(f=>{
   var g=new GameObject("Temporary_Actual_Terrain_Probe");g.hideFlags=HideFlags.HideAndDontSave;g.transform.SetPositionAndRotation(f.transform.position,f.transform.rotation);g.transform.localScale=f.transform.lossyScale;var c=g.AddComponent<MeshCollider>();c.sharedMesh=f.sharedMesh;return c;
  }).ToArray();
 }
 public static bool Sample(MeshCollider[] probes,Vector2 p,out float y)
 {
  y=float.NegativeInfinity;bool found=false;
  foreach(var c in probes)if(c.Raycast(new Ray(new Vector3(p.x,60,p.y),Vector3.down),out var h,120)&&h.normal.y>.05f){found=true;y=Mathf.Max(y,h.point.y);}
  return found;
 }
 public static void Dispose(MeshCollider[] probes){foreach(var p in probes)Object.DestroyImmediate(p.gameObject);}
}
