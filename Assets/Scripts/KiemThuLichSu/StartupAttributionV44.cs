using System;
using System.IO;
using UnityEngine;
using UnityEngine.Profiling;

// Opt-in diagnostic trace, never required for the release telemetry.
public sealed class StartupAttributionV44:MonoBehaviour
{
 [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
 static void Begin()
 {
  var a=Environment.GetCommandLineArgs();if(Array.IndexOf(a,"--startup-profile")<0)return;
  int i=Array.IndexOf(a,"--verification-output");Directory.CreateDirectory(a[i+1]);
  Profiler.logFile=Path.Combine(a[i+1],"startup.raw");Profiler.enableBinaryLog=true;Profiler.enabled=true;
  DontDestroyOnLoad(new GameObject("StartupAttributionV44",typeof(StartupAttributionV44)));
 }
 void Update(){if(Time.frameCount<15)return;Profiler.enabled=false;Profiler.enableBinaryLog=false;Destroy(gameObject);}
}
