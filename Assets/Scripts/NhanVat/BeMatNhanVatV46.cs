// Cấu hình lớp bề mặt nhân vật hiện hữu.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): acting, frame, head, torso, upper, lower, hands, thighs, shins, cloth, skin, trousers, mesh, root.
// Code tự tìm/tạo: MeshFilter, MeshRenderer.
// Thứ tự chạy khai báo: 26000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Profiling;

// Render-only deformation. The saved V45 rig remains the sole owner of contacts and poses.
[DefaultExecutionOrder(26000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "HumanSurfaceV46")]
public sealed class BeMatNhanVatV46 : MonoBehaviour
{
 public DienXuatHanhTrinhV45 acting;
 [Serializable] public class Body
 {
  public string role; public Transform frame,head,torso;
  public Transform[] upper,lower,hands,thighs,shins;
  public Material cloth,skin,trousers;
  public float shoulder,width,hem;
  [NonSerialized] public Surface jacket,neck; [NonSerialized] public Surface[] arms,legs;
  [NonSerialized] public Quaternion torsoRest;
  [NonSerialized] public float[] ringY,ringX;
 }
 public Body[] bodies;
 public sealed class Surface
 {
  public Mesh mesh; public Transform root; public Vector3[] vertices; public int rows,sides;
  public Surface(Transform parent,string name,Material material,int count,int segments=20)
  {
   rows=count;sides=segments;
   var g=new GameObject(name);g.layer=3;g.transform.SetParent(parent,false);root=g.transform;
   mesh=new Mesh{name=name};mesh.MarkDynamic();vertices=new Vector3[rows*sides];
   var indices=new List<int>();
   for(int r=0;r<rows-1;r++)for(int n=0;n<sides;n++){int a=r*sides+n,b=r*sides+(n+1)%sides;indices.AddRange(new[]{a,b,b+sides,a,b+sides,a+sides});}
   // Close at the end rings; no separate joint balls.
   for(int n=1;n<sides-1;n++){indices.AddRange(new[]{0,n+1,n,(rows-1)*sides,(rows-1)*sides+n,(rows-1)*sides+n+1});}
   mesh.vertices=vertices;mesh.SetTriangles(indices,0);g.AddComponent<MeshFilter>().sharedMesh=mesh;g.AddComponent<MeshRenderer>().sharedMaterial=material;
  }
  public void Ring(int row,Vector3 center,Vector3 right,Vector3 forward,float width,float depth)
  {for(int n=0;n<sides;n++){float a=n*Mathf.PI*2/sides;vertices[row*sides+n]=root.InverseTransformPoint(center+right*(Mathf.Sin(a)*width)+forward*(Mathf.Cos(a)*depth));}}
  public void Upload(){mesh.vertices=vertices;mesh.RecalculateNormals();mesh.RecalculateBounds();}
 }
 static readonly ProfilerMarker marker=new ProfilerMarker("V46.HumanSurface.Deform");
 void Start()
 {
  foreach(var b in bodies)
  {
   b.torsoRest=b.torso?b.torso.localRotation:Quaternion.identity;
   float h=b.shoulder,w=b.width;
   b.ringY=new[]{b.hem,b.hem+.045f,Mathf.Lerp(b.hem+.05f,h-.22f,.45f),Mathf.Lerp(b.hem+.05f,h-.22f,.8f),h-.22f,h-.12f,h-.06f,h-.015f,h+.025f,h+.035f};
   b.ringX=new[]{w*.96f,w,w*.94f,w*.83f,w*.96f,w,w*.99f,w*.85f,.084f,.075f};
   b.jacket=new Surface(b.frame,"V46_"+b.role+"_ContinuousGarment",b.cloth,10,32);
   b.neck=new Surface(b.frame,"V46_"+b.role+"_NeckTrapezius",b.skin,8,24);
   b.arms=new Surface[2];for(int i=0;i<2;i++)b.arms[i]=new Surface(b.frame,"V46_"+b.role+"_ContinuousSleeve_"+i,b.cloth,17);
   if(b.thighs!=null&&b.thighs.Length==2){b.legs=new Surface[2];for(int i=0;i<2;i++)b.legs[i]=new Surface(b.frame,"V46_"+b.role+"_TrouserDeformation_"+i,b.trousers,17);}
  }
  Refresh();
 }
 public static Vector3 End(Transform t,bool end)=>t.position+t.up*(t.lossyScale.y*(end?1:-1));
 // A rounded bend distributed across four rings on each side of the elbow/knee.
 // Endpoint rings stay on the original contact axes, including immediately after Load.
 static void Tube(Surface s,Vector3 a,Vector3 bend,Vector3 c,Vector3 forward,float width,float cuff,bool leg=false)
 {
  Vector3 inAxis=(bend-a).normalized,outAxis=(c-bend).normalized;
  float rounding=Mathf.Min(Vector3.Distance(a,bend),Vector3.Distance(bend,c))*.27f;
  var p=bend-inAxis*rounding;var q=bend+outAxis*rounding;
  for(int r=0;r<s.rows;r++)
  {
   float t=r/(float)(s.rows-1);Vector3 center,tangent;
   if(t<.3f){center=Vector3.Lerp(a,p,t/.3f);tangent=inAxis;}
   else if(t>.7f){center=Vector3.Lerp(q,c,(t-.7f)/.3f);tangent=outAxis;}
   else{float u=(t-.3f)/.4f;center=(1-u)*(1-u)*p+2*u*(1-u)*bend+u*u*q;tangent=Vector3.Lerp(inAxis,outAxis,u).normalized;}
   var right=Vector3.Cross(tangent,forward).normalized;var depth=Vector3.Cross(right,tangent).normalized;
   float radius=Mathf.Lerp(width,cuff,t);radius*=1+.035f*Mathf.Sin(t*Mathf.PI*6)*(leg?.4f:1);
   s.Ring(r,center,right,depth,radius,radius*(leg?1.08f:.92f));
  }
  s.Upload();
 }
 void Refresh()
 {
  foreach(var b in bodies)
  {
   if(!b.frame.gameObject.activeInHierarchy)continue;
   var f=b.frame;float h=b.shoulder,w=b.width;
   // Hem/hip/waist/ribcage/shoulder/collar: one silhouette with distinct age and build.
   var y=b.ringY;var x=b.ringX;
   var sway=b.torso?b.torso.localRotation*Quaternion.Inverse(b.torsoRest):Quaternion.identity;
   for(int r=0;r<10;r++)
   {
    var p=new Vector3(0,y[r],.012f);var pivot=new Vector3(0,1.02f,0);var center=f.TransformPoint(pivot+sway*(p-pivot));
    b.jacket.Ring(r,center,f.TransformDirection(sway*Vector3.right),f.TransformDirection(sway*Vector3.forward),x[r],r>=8?.073f: .13f+(b.role=="Ha"?.025f:0));
   }
   b.jacket.Upload();
   var top=b.head.position-b.head.up*.143f;
   for(int r=0;r<8;r++){float t=r/7f;float radius=Mathf.Lerp(.115f,.048f,Mathf.SmoothStep(0,1,Mathf.Min(1,t*2)));b.neck.Ring(r,Vector3.Lerp(f.TransformPoint(new Vector3(0,h-.025f,.008f)),top,t),f.right,f.forward,radius,radius*.88f);}
   b.neck.Upload();
   for(int i=0;i<2;i++)
   {
    var a=End(b.upper[i],false);var bend=End(b.upper[i],true);var c=b.hands[i].position;
    // Extend the sleeve under the shoulder fabric; keep the wrist on the saved palm anchor.
    var shoulder=f.TransformPoint(new Vector3(i==0?-w*.77f:w*.77f,h-.07f,.012f));
    Tube(b.arms[i],shoulder,bend,c,f.forward,b.role=="Tung"?.088f:b.role=="Ha"?.080f:b.role=="Vy"?.068f:.077f,.043f);
    if(b.legs!=null)Tube(b.legs[i],End(b.thighs[i],false),End(b.thighs[i],true),End(b.shins[i],true),f.forward,.071f,.049f,true);
   }
  }
 }
 void LateUpdate(){if(acting&&acting.Ready)using(marker.Auto())Refresh();}
 void OnDestroy(){if(bodies==null)return;foreach(var b in bodies){if(b.jacket!=null)Destroy(b.jacket.mesh);if(b.neck!=null)Destroy(b.neck.mesh);if(b.arms!=null)foreach(var s in b.arms)Destroy(s.mesh);if(b.legs!=null)foreach(var s in b.legs)Destroy(s.mesh);}}
}
