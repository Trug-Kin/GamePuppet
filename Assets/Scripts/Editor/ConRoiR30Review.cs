using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
[InitializeOnLoad] public static class ConRoiR30Review {
 static ConRoiR30Review(){EditorApplication.playModeStateChanged+=p=>{if(p==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r30.walk",false)){SessionState.SetBool("r30.walk",false);Object.FindAnyObjectByType<DieuPhoiHanhTrinh>().gameObject.AddComponent<R30WalkReview>();}};}
 public static void Begin(){
  if(!Application.isBatchMode){
   var asm=typeof(EditorWindow).Assembly;var sizesType=asm.GetType("UnityEditor.GameViewSizes");
   var singleton=typeof(ScriptableSingleton<>).MakeGenericType(sizesType);var sizes=singleton.GetProperty("instance").GetValue(null);
   var group=sizesType.GetMethod("GetGroup").Invoke(sizes,new[]{System.Enum.Parse(asm.GetType("UnityEditor.GameViewSizeGroupType"),"Standalone")});
   var sizeType=asm.GetType("UnityEditor.GameViewSize");var fixedType=asm.GetType("UnityEditor.GameViewSizeType");
   var size=System.Activator.CreateInstance(sizeType,System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.Public|System.Reflection.BindingFlags.NonPublic,null,new object[]{System.Enum.Parse(fixedType,"FixedResolution"),1280,720,"r30 review 1280x720"},null);
   group.GetType().GetMethod("AddCustomSize").Invoke(group,new[]{size});
   var view=EditorWindow.GetWindow(asm.GetType("UnityEditor.GameView"));view.GetType().GetProperty("selectedSizeIndex").SetValue(view,(int)group.GetType().GetMethod("GetTotalCount").Invoke(group,null)-1);
  }
  EditorSceneManager.OpenScene(ConRoiR30.Scene);SessionState.SetBool("r30.walk",true);EditorApplication.EnterPlaymode();
 }
}
