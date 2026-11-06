using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Review-only renders. Never saves a checkpoint or modifies authoring assets.
public static class ConRoiReviewV42
{
 public static void PostClearance()
 {
  EditorSceneManager.OpenScene(ConRoiExperienceV42.ScenePath);var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();story.market.SetActive(true);Physics.SyncTransforms();
  var route=new[]{new Vector3(785.04f,6.6f,271.86f),new Vector3(783.6f,6.6f,272),new Vector3(783.6f,6.6f,274.1815f),new Vector3(785.1028f,6.6f,274.1815f)};
  var rows=new Newtonsoft.Json.Linq.JArray();
  for(int k=1;k<route.Length;k++)for(int n=0;n<=24;n++)
  {
   var p=Vector3.Lerp(route[k-1],route[k],n/24f);
   var floors=Physics.RaycastAll(p+Vector3.up*.2f,Vector3.down,2,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.8f).OrderByDescending(h=>h.point.y).ToArray();
   if(floors.Length==0){rows.Add(new Newtonsoft.Json.Linq.JObject{["point"]=p.ToString(),["error"]="no ground"});continue;}
   float floor=floors[0].point.y;var hits=Physics.OverlapCapsule(new Vector3(p.x,floor+.42f,p.z),new Vector3(p.x,floor+1.52f,p.z),.30f,~0,QueryTriggerInteraction.Ignore);
   if(hits.Length>0)rows.Add(new Newtonsoft.Json.Linq.JObject{["point"]=p.ToString(),["hits"]=new Newtonsoft.Json.Linq.JArray(hits.Select(c=>c.name))});
  }
  File.WriteAllText(ConRoiExperienceV42.Report+"/post-bypass-clearance.json",rows.ToString());EditorSceneManager.OpenScene(ConRoiExperienceV42.ScenePath);
 }
 public static void Compare()
 {
  foreach(bool after in new[]{false,true})
  {
   EditorSceneManager.OpenScene(after?ConRoiExperienceV42.ScenePath:ConRoiCompletionV41.ScenePath);
   var story=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var camera=story.player.GetComponentInChildren<Camera>();
   var regions=new[]{story.schoolChapter.environment,story.slopeChapter.environment,story.hub};
   var active=regions.Select(r=>r.activeSelf).ToArray();
   var eyePositions=new[]{new Vector3(-95,1.7f,-52),new Vector3(-180,4,-36),new Vector3(-36.8f,1.6f,-35.8f),new Vector3(-178,7.7f,6)};
   var targets=new[]{new Vector3(-80,3,-35),new Vector3(-180,7,-9),new Vector3(-33.8f,.9f,-33.3f),new Vector3(-164,11.3f,19)};
   var regionIndex=new[]{0,1,2,1};
   var rt=RenderTexture.GetTemporary(1280,720,24);var tex=new Texture2D(1280,720,TextureFormat.RGB24,false);
   for(int n=0;n<4;n++)
   {
    for(int k=0;k<3;k++)regions[k].SetActive(k==regionIndex[n]);story.lake.SetActive(false);story.market.SetActive(false);
    camera.transform.SetPositionAndRotation(eyePositions[n],Quaternion.LookRotation(targets[n]-eyePositions[n]));
    RenderPipeline.SubmitRenderRequest(camera,new UniversalRenderPipeline.SingleCameraRequest{destination=rt});
    var previous=RenderTexture.active;RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1280,720),0,0);tex.Apply();RenderTexture.active=previous;
    File.WriteAllBytes(ConRoiExperienceV42.Report+"/comparison/"+new[]{"school_garden","slope","hub_room","slope_detail"}[n]+(after?"_after.png":"_before.png"),tex.EncodeToPNG());
   }
   RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex);
  }
  // Discard all transient scene/camera edits, returning to the saved final scene.
  EditorSceneManager.OpenScene(ConRoiExperienceV42.ScenePath);
 }
}
