using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class ConRoiR22Build
{
    public static void Build()
    {
        string output=ConRoiR22Scenery.Out;
        var review=JObject.Parse(File.ReadAllText(output+"/after-playmode.json"));
        if(!(bool)review["complete"]||review["errors"].Any())throw new Exception("Final Play Mode review must complete first");
        const string binary="Builds/Windows_v48_r22_playtest/ConRoi.exe";
        if(File.Exists("Builds/Windows_v48_r22_playtest.zip"))throw new Exception("Never overwrite a retained package");
        // Before first handoff only, allow incremental revision of this exact
        // new draft. Retained r21/recoveryprep paths are never build targets.
        if(Directory.Exists(Path.GetDirectoryName(binary))&&(!File.Exists(binary)||!File.Exists(output+"/build.json")||(string)JObject.Parse(File.ReadAllText(output+"/build.json"))["binary"]!=binary))throw new Exception("Existing directory is not the current r22 draft");
        EditorSceneManager.OpenScene(ConRoiR22Scenery.Scene);
        var report=BuildPipeline.BuildPlayer(new[]{ConRoiR22Scenery.Scene},binary,BuildTarget.StandaloneWindows64,BuildOptions.Development|BuildOptions.DetailedBuildReport);
        var messages=new JArray();
        foreach(var step in report.steps)foreach(var m in step.messages)
            if(m.type==LogType.Warning||m.type==LogType.Error||m.type==LogType.Exception)
                messages.Add(new JObject{["step"]=step.name,["type"]=m.type.ToString(),["message"]=m.content});
        var packed=report.packedAssets.SelectMany(p=>p.contents).Select(p=>p.sourceAssetPath).Where(p=>!string.IsNullOrEmpty(p)).Distinct().OrderBy(p=>p).ToArray();
        var required=new[]{"Flowers_Red_0.asset","Flowers_Red_1.asset","Flowers_Red_2.asset","Flowers_Yellow_0.asset","Flowers_Yellow_1.asset","Flowers_Yellow_2.asset","MarketAwning.fbx","WorldMist.shader","Pipeline.asset","Renderer.asset","IslandDeckCollider.asset"};
        string art="Assets/Art/Environment/JourneyV48/SceneryR22/";
        var presence=new JObject();foreach(var name in required)presence[name]=packed.Contains(art+name);
        bool present=presence.Properties().All(p=>(bool)p.Value);
        File.WriteAllText(output+"/build-packed-assets.json",new JArray(packed).ToString());
        File.WriteAllText(output+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["messages"]=messages,["scene"]=ConRoiR22Scenery.Scene,["binary"]=binary,["required_assets_present"]=present,["required_asset_evidence"]=presence,["method"]="Unity Windows64 Development build; DetailedBuildReport packed asset paths plus scene renderer/material audit and Play Mode images. Binary has not been driven by Windows input.",["native_input"]="NOT_RUN; launcher diagnosis PAUSED; existing INTERRUPTED/NOT_RUN unchanged"}.ToString());
        if(report.summary.result!=BuildResult.Succeeded||!present)throw new Exception("Build failed or required packed assets missing");
    }
}
