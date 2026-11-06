#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

// Separate diagnostic launch only; never enabled in timing comparisons.
public sealed class EntryProfileV47:MonoBehaviour
{
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
 static void Begin()
 {
  var a=Environment.GetCommandLineArgs();bool route=Array.IndexOf(a,"--route-profile")>=0;if(!route&&Array.IndexOf(a,"--entry-profile")<0)return;
  int n=Array.IndexOf(a,"--verification-output");Directory.CreateDirectory(a[n+1]);
  var go=new GameObject("EntryProfileV47",typeof(EntryProfileV47));var p=go.GetComponent<EntryProfileV47>();p.routeMode=route;p.directory=a[n+1];
  if(!route){Profiler.logFile=Path.Combine(a[n+1],"entry.raw");Profiler.enableBinaryLog=true;Profiler.enabled=true;}
  DontDestroyOnLoad(go);
 }
 double began=-1;bool routeMode;string directory;DieuPhoiHanhTrinh story;
 void Start(){story=FindAnyObjectByType<DieuPhoiHanhTrinh>();QualitySettings.vSyncCount=0;Application.targetFrameRate=120;if(!routeMode)began=Time.realtimeSinceStartupAsDouble;}
 void Update()
 {
  if(routeMode&&began<0){var pos=story.player.transform.position;if(story.data.region!=0||pos.x<580||pos.z<537)return;began=Time.realtimeSinceStartupAsDouble;Profiler.logFile=Path.Combine(directory,"lake-moving.raw");Profiler.enableBinaryLog=true;Profiler.enabled=true;}
  if(Time.realtimeSinceStartupAsDouble-began<(routeMode?8:4))return;Profiler.enabled=false;Profiler.enableBinaryLog=false;
  File.WriteAllText(Path.Combine(directory,"profile-complete.json"),"{\"purpose\":\"Partial CPU trace capture only; intentionally stopped after diagnostic window; not a completed experience suite\"}");
  Application.Quit();
 }
}

#endif
