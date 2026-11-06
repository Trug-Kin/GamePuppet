using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;

// Source-only build entry: never reads a previous player or writes r42 review reports.
public static class ConRoiR43
{
    const string Scene = "Assets/Scenes/JourneyRepair_v48_r42.unity";
    const string Report = "Docs/Reviews/journey_v48/r43-build-source/source-build.json";
    public static void BuildSource() { Build(false); }
    public static void BuildReview() { Build(true); }
    static void Build(bool review)
    {
        var result = new JObject { ["scene"] = Scene, ["unity"] = Application.unityVersion,
            ["origin"] = "BuildPipeline.BuildPlayer from current source; no prior player files",
            ["cleanBuildCache"] = true, ["reviewDefine"] = review };
        int exit = 1;
        try
        {
            var args = Environment.GetCommandLineArgs();
            int at = Array.IndexOf(args, "-r43-build-output");
            if (at < 0 || at + 1 >= args.Length) throw new Exception("Specify a fresh -r43-build-output directory");
            string directory = Path.GetFullPath(args[at + 1]);
            if (Directory.Exists(directory) || File.Exists(directory)) throw new Exception("Refuse existing output: " + directory);
            string symbols = PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Standalone);
            if (symbols.Split(';').Contains("CON_ROI_BUILD_REVIEW")) throw new Exception("Review define must be explicit per build, not global");
            result["globalDefines"] = symbols;
            result["output"] = directory;
            var scene = EditorSceneManager.OpenScene(Scene, OpenSceneMode.Single);
            var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
            if (all.Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)) != 0) throw new Exception("Missing Script in r42 scene");
            if (all.Any(t => t.GetComponent<R42JourneyReview>() || t.GetComponent<R42AudioCapture>())) throw new Exception("Serialized review component");
            if (all.Count(t => t.GetComponent<DieuPhoiGiaoDienR42>()) != 1 || all.Count(t => t.GetComponent<DieuPhoiHanhTrinh>()) != 1) throw new Exception("Unexpected bootstrap/UI owners");
            var options = BuildOptions.CleanBuildCache | BuildOptions.DetailedBuildReport;
            if (review) options |= BuildOptions.Development;
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions {
                scenes = new[] { Scene }, locationPathName = Path.Combine(directory, "ConRoi.exe"),
                target = BuildTarget.StandaloneWindows64, options = options,
                extraScriptingDefines = review ? new[] { "CON_ROI_BUILD_REVIEW" } : Array.Empty<string>()
            });
            result["result"] = report.summary.result.ToString();
            result["errors"] = report.summary.totalErrors;
            result["warnings"] = report.summary.totalWarnings;
            result["bytes"] = report.summary.totalSize;
            exit = report.summary.result == BuildResult.Succeeded ? 0 : 1;
        }
        catch (Exception e) { result["result"] = "Failed"; result["exception"] = e.ToString(); }
        File.WriteAllText(Report, result.ToString());
        EditorApplication.Exit(exit);
    }
}
