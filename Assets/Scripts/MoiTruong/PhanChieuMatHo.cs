// Camera phản chiếu mặt hồ trong URP; giữ nhánh VP/flip và occlusion đã kiểm ở R39.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): playerCamera, water, notebook.
// Code tự tìm/tạo: Camera, DieuPhoiHanhTrinh.
// Thứ tự chạy khai báo: 28000; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
// Capture after the lake/book/actor late poses so the reflected state matches this frame.
[DefaultExecutionOrder(28000)] [UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "LakePlanarReflection")]
public sealed class PhanChieuMatHo:MonoBehaviour
{
 public Camera playerCamera;
 public Renderer water;
 public bool urpCaptureR38;
 // URP SingleCameraRequest's resolved destination has already been oriented by
 // its final blit. Applying render-into-texture Y again mirrors the sampling.
 // Opt in on the r39 derivative; historical checkpoint configuration is retained.
 public bool resolvedRequestUvR39;
 // Opt-in: the old fixed 768x432 target visibly undersamples a 1080p lake.
 // Retained scenes keep their original capture policy and target size.
 public bool viewportSamplingR49;
 public TuongTacSoTayHoR37 notebook;
 int revision=-1;
 Camera reflection;
 RenderTexture texture;
 Material material;
 public bool HasRendered {get;private set;}
 void Start()
 {
  var go=new GameObject("Lake_Reflection_Camera");go.transform.SetParent(transform);reflection=go.AddComponent<Camera>();reflection.enabled=false;
  EnsureTarget();material=new Material(water.sharedMaterial);water.material=material;
 }
 bool EnsureTarget()
 {
  int width=768,height=432;
  if(viewportSamplingR49&&playerCamera){
   int w=Mathf.Max(1,playerCamera.pixelWidth),h=Mathf.Max(1,playerCamera.pixelHeight);
   // The params overload allocates a float[] on every size check.
   float scale=Mathf.Min(1f,Mathf.Min(1920f/w,1080f/h));
   width=Mathf.Max(1,Mathf.RoundToInt(w*scale));height=Mathf.Max(1,Mathf.RoundToInt(h*scale));
  }
  if(texture&&texture.width==width&&texture.height==height)return false;
  if(texture){if(reflection)reflection.targetTexture=null;texture.Release();Destroy(texture);}
  texture=new RenderTexture(width,height,24){name=viewportSamplingR49?"LakeReflection_R49_Viewport":"LakeReflection_Legacy",filterMode=FilterMode.Bilinear,wrapMode=TextureWrapMode.Clamp};
  return true;
 }
 static readonly Unity.Profiling.ProfilerMarker taskMarkerV46 = new Unity.Profiling.ProfilerMarker("V46.Lake.Reflection");
 static readonly Unity.Profiling.ProfilerMarker lookupMarker = new Unity.Profiling.ProfilerMarker("V47.Reflection.StoryLookup");
 static readonly Unity.Profiling.ProfilerMarker prepareMarker = new Unity.Profiling.ProfilerMarker("V47.Reflection.Prepare");
 static readonly Unity.Profiling.ProfilerMarker renderMarker = new Unity.Profiling.ProfilerMarker("V47.Reflection.Render");
 static readonly Unity.Profiling.ProfilerMarker bindMarker = new Unity.Profiling.ProfilerMarker("V47.Reflection.Bind");
    void LateUpdate(){using(taskMarkerV46.Auto()) LateUpdateMeasuredV46();}
    void LateUpdateMeasuredV46()
 {
  bool dynamicBook=urpCaptureR38&&notebook&&(notebook.Held||notebook.HandMotion||revision!=notebook.VisualRevision);
  if(!reflection||!water||!water.enabled||(!viewportSamplingR49&&!dynamicBook&&Time.frameCount%3!=0))return;
  DieuPhoiHanhTrinh story;using(lookupMarker.Auto())story=FindAnyObjectByType<DieuPhoiHanhTrinh>();if(story&&story.data.region!=0)return;
  bool targetChanged=viewportSamplingR49&&EnsureTarget();
  if(viewportSamplingR49&&!dynamicBook&&!targetChanged&&Time.frameCount%3!=0)return;
  using(prepareMarker.Auto()){
  float y=water.bounds.center.y;
  Vector4 plane=new Vector4(0,1,0,-y);var matrix=Matrix4x4.identity;matrix.m11=-1;matrix.m13=2*y;
  reflection.CopyFrom(playerCamera);reflection.enabled=false;reflection.targetTexture=texture;reflection.useOcclusionCulling=false;
  if(!urpCaptureR38)reflection.worldToCameraMatrix=playerCamera.worldToCameraMatrix*matrix;
  var p=playerCamera.transform.position;p.y=2*y-p.y;reflection.transform.position=p;
  if(urpCaptureR38)reflection.transform.rotation=Quaternion.LookRotation(Vector3.Reflect(playerCamera.transform.forward,Vector3.up),Vector3.Reflect(playerCamera.transform.up,Vector3.up));
  if(urpCaptureR38)reflection.worldToCameraMatrix=playerCamera.worldToCameraMatrix*matrix;
  var normal=reflection.worldToCameraMatrix.MultiplyVector(Vector3.up);var point=reflection.worldToCameraMatrix.MultiplyPoint(new Vector3(0,y+.025f,0));var clip=new Vector4(normal.x,normal.y,normal.z,-Vector3.Dot(point,normal));reflection.projectionMatrix=reflection.CalculateObliqueMatrix(clip);
  }
  bool old=GL.invertCulling;bool visible=water.enabled;
  try{water.enabled=false;GL.invertCulling=!old;using(renderMarker.Auto()){
   if(urpCaptureR38)RenderPipeline.SubmitRenderRequest(reflection,new UniversalRenderPipeline.SingleCameraRequest{destination=texture});
   else reflection.Render();
  }}
  finally{GL.invertCulling=old;water.enabled=visible;}
  using(bindMarker.Auto()){material.SetTexture("_LakeReflection",texture);material.SetMatrix("_LakeReflectionVP",GL.GetGPUProjectionMatrix(reflection.projectionMatrix,!resolvedRequestUvR39)*reflection.worldToCameraMatrix);material.SetFloat("_LakeReflectionReady",1);}
  HasRendered=true;
  if(notebook)revision=notebook.VisualRevision;
 }
 void OnDestroy(){if(reflection)Destroy(reflection.gameObject);if(texture){texture.Release();Destroy(texture);}if(material)Destroy(material);}
}
