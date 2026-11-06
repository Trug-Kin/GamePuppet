using System;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEditor;
public static class ConRoiR29Finalize {
 public static void Run(){
  string checkpoint="Docs/Checkpoints/r29-school/EditorBuildSettings.asset.before";
  if(!File.Exists(checkpoint))File.Copy("ProjectSettings/EditorBuildSettings.asset",checkpoint);
  EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ConRoiR29School.Scene,true)};
  File.WriteAllText(ConRoiR29School.Out+"/build-input.json",new JObject{["enabled_scene"]=Path.GetFullPath(ConRoiR29School.Scene),["scene_guid"]=AssetDatabase.AssetPathToGUID(ConRoiR29School.Scene),["build_settings_scenes"]=new JArray(ConRoiR29School.Scene),["loading"]="Integrated scene; LakeMarketStory switches existing region roots through Sleep/Wake/Apply. JourneyLoadingV44 retained.",["editor_playmode_startup_loading"]="NOT_RUN: batch review bypasses startup loading; Sleep/Wake and region/save-load exercised",["windows_binary"]="NOT_BUILT / NOT_RUN",["whole_game_acceptance"]=false}.ToString());
  ConRoiR29Inventory.Run();
 }
}
