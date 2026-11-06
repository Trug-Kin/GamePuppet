using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
[InitializeOnLoad] public static class ConRoiR32Review {
 static ConRoiR32Review(){EditorApplication.playModeStateChanged+=p=>{if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r32.walk",false)){SessionState.SetBool("r32.walk",false);Object.FindAnyObjectByType<DieuPhoiHanhTrinh>().gameObject.AddComponent<R32WalkReview>().output="Docs/Reviews/journey_v48/r32/editor-final";}};}
 public static void Begin(){EditorSceneManager.OpenScene(ConRoiR32.Scene);SessionState.SetBool("r32.walk",true);EditorApplication.EnterPlaymode();}
}
