using UnityEngine;

// Local SE junction: the approach road's authored outer grade is 1.105 m.
// Preserve the route footprint, blending the old path grade over 5 m outside
// the immediate bridge/planter junction. This is not a regional terrain lift.
public static class ConRoiV48R21JunctionGrade
{
 public static float Weight(Vector3 p)=>1-Mathf.SmoothStep(0,1,Mathf.InverseLerp(14,19,Vector2.Distance(new Vector2(p.x,p.z),new Vector2(635,548))));
 public static Vector3 Paving(Vector3 p){p.y=Mathf.Lerp(p.y,1.105f,Weight(p));return p;}
 public static bool InBridgeVoid(Vector3 p)
 {
  var nw=new Vector3(685.9787f,0,496.6172f);var axis=new Vector3(-.7636f,0,.6457f).normalized;var side=Vector3.Cross(Vector3.up,axis);var d=p-nw;d.y=0;
  return Vector3.Dot(d,axis)>0&&Vector3.Dot(d,axis)<57.27f&&Mathf.Abs(Vector3.Dot(d,side))<7;
 }
}
