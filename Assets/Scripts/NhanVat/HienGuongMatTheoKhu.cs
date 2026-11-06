// Hiện/crossfade gương mặt theo khu; R91 dùng portrait jaw-only gắn với ghostHead/NoseTip.
// Component trong scene r92: AnhTung_Cousin, CoMai_Former_Teacher, CR_Bao, CR_Ha.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): ghostHead, minhFace, minhMaterial, minhHeadSource, fittedPortraitR91, portraitMaterialsR91.
// Code tự tìm/tạo: MeshFilter, MeshRenderer.
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "RegionalFaceProjection")]
public sealed class HienGuongMatTheoKhu:MonoBehaviour
{
 public Transform ghostHead;
 public Mesh minhFace;
 public Material minhMaterial;
 public Transform minhHeadSource;
 public bool opaqueRestR45;
 // r91: a jaw-only, centered Blender portrait; older checkpoints retain their source clone.
 public Mesh fittedPortraitR91;
 public Material[] portraitMaterialsR91;
 Renderer[] original;
 GameObject projection;
 Renderer face;
 Renderer[] projected;
 bool built;
 public bool Revealed {get;private set;}
 public float LastFadeSeconds {get;private set;}
 public int FadeSamples {get;private set;}
 public bool SawIntermediateAlpha {get;private set;}
 public int RevealCount {get;private set;}
 void Build()
 {
  if(built)return;built=true;original=ghostHead.GetComponentsInChildren<Renderer>().Where(r=>r.enabled).ToArray();
  var bounds=original[0].bounds;foreach(var renderer in original)bounds.Encapsulate(renderer.bounds);
  if(fittedPortraitR91)
  {
   projection=new GameObject("Minh_Face_Projection");projection.transform.SetParent(ghostHead,false);
   projection.AddComponent<MeshFilter>().sharedMesh=fittedPortraitR91;
   face=projection.AddComponent<MeshRenderer>();face.sharedMaterials=portraitMaterialsR91.Select(m=>new Material(m)).ToArray();projected=new[]{face};
   // Fit to the actual jaw, excluding hat/hair and without inheriting the source actor's world gaze.
   var jaw=original.FirstOrDefault(r=>r.name.EndsWith("_Jaw_Cheek_Forehead"))??original[0];
   // Sculpt roots have their own export rotation. Derive facing from the real
   // nose, rather than assuming the gaze pivot uses the sculpt's forward axis.
   var nose=original.FirstOrDefault(r=>r.name.EndsWith("_NoseTip"));
   var forward=nose?ghostHead.InverseTransformDirection(nose.bounds.center-jaw.bounds.center):Vector3.back;
   forward.y=0;
   projection.transform.localRotation=Quaternion.LookRotation(forward.sqrMagnitude>.000001f?forward.normalized:Vector3.back,Vector3.up);
   var mf=jaw.GetComponent<MeshFilter>();var points=mf.sharedMesh.vertices.Select(v=>ghostHead.InverseTransformPoint(mf.transform.TransformPoint(v))).ToArray();
   var target=new Bounds(points[0],Vector3.zero);foreach(var p in points)target.Encapsulate(p);
   float scale=target.size.y/Mathf.Max(.001f,fittedPortraitR91.bounds.size.y);
   projection.transform.localScale=Vector3.one*scale;
   projection.transform.localPosition=target.center-projection.transform.localRotation*(fittedPortraitR91.bounds.center*scale);
  }
  else if(minhHeadSource)
  {
   projection=Instantiate(minhHeadSource.gameObject);projection.name="Minh_Face_Projection";projection.SetActive(true);
   foreach(var col in projection.GetComponentsInChildren<Collider>(true))Destroy(col);
   projection.transform.SetParent(ghostHead,true);projection.transform.rotation=minhHeadSource.rotation;
   projected=projection.GetComponentsInChildren<Renderer>(true);var b=projected[0].bounds;foreach(var r in projected)b.Encapsulate(r.bounds);
   projection.transform.localScale*=.32f/Mathf.Max(.001f,b.size.y);b=projected[0].bounds;foreach(var r in projected)b.Encapsulate(r.bounds);projection.transform.position+=bounds.center-b.center;
   foreach(var renderer in projected)renderer.materials=renderer.sharedMaterials.Select(m=>new Material(m)).ToArray();face=projected[0];
  }
  else
  {
   projection=new GameObject("Minh_Face_Projection");projection.transform.SetParent(ghostHead,false);projection.AddComponent<MeshFilter>().sharedMesh=minhFace;face=projection.AddComponent<MeshRenderer>();face.sharedMaterial=new Material(minhMaterial);
   var size=minhFace.bounds.size;var parentScale=ghostHead.lossyScale;projection.transform.localScale=new Vector3(.25f/Mathf.Max(.00001f,size.x)/Mathf.Abs(parentScale.x),.32f/Mathf.Max(.00001f,size.y)/Mathf.Abs(parentScale.y),.25f/Mathf.Max(.00001f,size.z)/Mathf.Abs(parentScale.z));projection.transform.position=bounds.center-projection.transform.TransformVector(minhFace.bounds.center);projected=new[]{face};
  }
  foreach(var renderer in original)renderer.materials=renderer.sharedMaterials.Select(m=>new Material(m)).ToArray();
  foreach(var renderer in original.Concat(projected))foreach(var material in renderer.sharedMaterials)
  {material.SetFloat("_Surface",1);material.SetFloat("_SrcBlend",(float)BlendMode.SrcAlpha);material.SetFloat("_DstBlend",(float)BlendMode.OneMinusSrcAlpha);material.SetFloat("_ZWrite",0);material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");material.renderQueue=3000;}
  if(opaqueRestR45)Surface(false);
 }
 void Surface(bool transparent)
 {
  foreach(var renderer in original.Concat(projected))foreach(var material in renderer.sharedMaterials)
  {
   material.SetFloat("_Surface",transparent?1:0);
   material.SetFloat("_SrcBlend",(float)(transparent?BlendMode.SrcAlpha:BlendMode.One));
   material.SetFloat("_DstBlend",(float)(transparent?BlendMode.OneMinusSrcAlpha:BlendMode.Zero));
   material.SetFloat("_ZWrite",transparent?0:1);
   if(transparent)material.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");else material.DisableKeyword("_SURFACE_TYPE_TRANSPARENT");
   material.SetOverrideTag("RenderType",transparent?"Transparent":"Opaque");material.renderQueue=transparent?3000:2000;
  }
 }
 void Alpha(Renderer renderer,float value){foreach(var material in renderer.sharedMaterials){var c=material.GetColor("_BaseColor");c.a=value;material.SetColor("_BaseColor",c);}}
 public void Restore(bool revealed){Build();StopAllCoroutines();Revealed=revealed;projection.SetActive(revealed);foreach(var r in projected)Alpha(r,1);foreach(var renderer in original){renderer.enabled=!revealed;Alpha(renderer,1);}if(opaqueRestR45)Surface(false);}
 public void Reveal(){Build();RevealCount++;StartCoroutine(Fade());}
 IEnumerator Fade(){float started=Time.time;FadeSamples=0;SawIntermediateAlpha=false;if(opaqueRestR45)Surface(true);projection.SetActive(true);for(float t=0;t<.08f;t+=Time.deltaTime){float a=t/.08f;FadeSamples++;SawIntermediateAlpha|=a>0&&a<1;foreach(var r in projected)Alpha(r,a);foreach(var renderer in original)Alpha(renderer,1-a);yield return null;}LastFadeSeconds=Time.time-started;Restore(true);}
 void OnDestroy(){if(!built)return;foreach(var renderer in original.Concat(projected))if(renderer)foreach(var material in renderer.sharedMaterials)Destroy(material);}
}
