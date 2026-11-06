using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public static class CanopyAccessSetup
{
    const string Scene="Assets/CanopyAccessTest_20261014/DaLatMovementTest.unity";
    const string Output="Docs/Reviews/ntmk_canopy_20261014_access_measurement/controller_180cm_runs";
    public static void Run()
    {
        Directory.CreateDirectory(Output);
        EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);
        var fixture=UnityEngine.Object.FindFirstObjectByType<CanopyAccessFixture>();
        if(!fixture)throw new Exception("Missing isolated canopy fixture");
        var player=fixture.player;var cc=player.controller;
        if(cc.height!=1.8f||cc.radius!=.3f||cc.center!=Vector3.zero||player.transform.lossyScale!=Vector3.one
            ||cc.skinWidth!=.03f||cc.stepOffset!=.3f||cc.slopeLimit!=45f
            ||player.groundCheck.localPosition!=new Vector3(0,-.8f,0)||player.groundDistance!=.15f
            ||Camera.main.transform.localPosition!=new Vector3(0,.75f,0))
            throw new Exception("Unexpected 180cm test controller profile; refusing to resize");
        File.WriteAllText(Output+"/scene_prepared.txt",Scene+"\nExisting Player copied from prior test scene; only canopy and clipped local support copies, no map rebuild or FBX.\nheight="+cc.height+" radius="+cc.radius+" scale="+player.transform.lossyScale+" step="+cc.stepOffset+" slope="+cc.slopeLimit+" skin="+cc.skinWidth+" speed="+player.speed+" gravity="+player.gravity+"\n");
        // A previous run must never make an interrupted new run look complete.
        string resultPath=Path.Combine(Output,"controller_result.json");
        if(File.Exists(resultPath))File.Move(resultPath,Path.Combine(Output,"controller_result_previous_"+DateTime.UtcNow.ToString("yyyyMMddHHmmssfffffff")+".json"));
        EditorApplication.playModeStateChanged+=state=> {
            if(state==PlayModeStateChange.EnteredEditMode&&Application.isBatchMode)
                EditorApplication.delayCall+=()=>EditorApplication.Exit(File.Exists(Output+"/controller_result.json")?0:2);
        };
        EditorApplication.isPlaying=true;
    }
}
