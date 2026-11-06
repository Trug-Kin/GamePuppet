using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
[InitializeOnLoad] public static class ConRoiR33Chair {
 public const string Scene="Assets/Scenes/JourneyRepair_v48_r33.unity";
 static ConRoiR33Chair(){EditorApplication.playModeStateChanged+=p=>{if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r33.chair",false)){SessionState.SetBool("r33.chair",false);UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>().gameObject.AddComponent<R33ChairReview>().output=SessionState.GetString("r33.output","");}};}
 public static void BeginBaseline(){Begin(ConRoiR32.Scene,"Docs/Reviews/journey_v48/r33-chair/editor-baseline-route3");}
 public static void AuthorAndBaseline(){
  if(File.Exists(Scene))throw new Exception("Refuse to overwrite r33 derivative");
  if(!AssetDatabase.CopyAsset(ConRoiR32.Scene,Scene))throw new Exception("Scene copy failed");
  EditorSceneManager.OpenScene(Scene);var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
  s.GetComponent<TraiNghiemTruongR28>().restoreChairTraceR33=true;
  UnityEngine.Object.DestroyImmediate(s.GetComponent<HoSoSceneR32>());s.gameObject.AddComponent<HoSoSceneR33>().story=s;
  EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene());AssetDatabase.SaveAssets();BeginBaseline();
 }
 public static void BeginFixed(){Begin(Scene,"Docs/Reviews/journey_v48/r33-chair/editor-fixed");}
 static void Begin(string scene,string output){EditorSceneManager.OpenScene(scene);SessionState.SetString("r33.output",output);SessionState.SetBool("r33.chair",true);EditorApplication.EnterPlaymode();}
}
