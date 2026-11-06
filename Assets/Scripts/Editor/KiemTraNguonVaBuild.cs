// Cong cu Editor kiem source r92 va full build; khong gan len GameObject.
// Cho Editor het import/compile 5 giay truoc OpenScene: workaround crash batch da ghi o ho so r92.
// Khong vao Play Mode; report/output bat buoc truyen bang -trial-report va -trial-output.
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

// Helper kiểm/build r92 đã đưa về source chính; không vào Play Mode hay sửa luật chơi.
public static class KiemTraNguonVaBuild
{
    const string GameScene = "Assets/Scenes/JourneyPresentation_v48_r92.unity";
    public static void BuildDelayed()
    {
        double began=EditorApplication.timeSinceStartup,idle=began;
        Debug.Log("R92_BUILD_DELAY_SCHEDULED; all existing scene checks retained");
        EditorApplication.CallbackFunction step=null;
        step=()=>{
            double now=EditorApplication.timeSinceStartup;
            if(now-began>180){EditorApplication.update-=step;File.WriteAllText(Arg("-trial-report"),new JObject{["result"]="IDLE_GATE_TIMEOUT",["buildPlayerInvoked"]=false}.ToString());EditorApplication.Exit(2);return;}
            if(EditorApplication.isUpdating||EditorApplication.isCompiling){idle=now;return;}
            if(now-idle<5)return;
            EditorApplication.update-=step;
            Debug.Log("R92_BUILD_DELAY_READY seconds="+(now-began)+" isUpdating="+EditorApplication.isUpdating+" isCompiling="+EditorApplication.isCompiling);
            Build();
        };
        EditorApplication.update+=step;
    }
    // r92 targeted isolation: defer explicit scene-open until Editor has serviced
    // startup callbacks and remained out of import/compile for five seconds.
    // Chỉ ở Editor: lưu và mở lại scene, kiểm reference; không chạy gameplay hoặc player.
    public static void ProbeOpenDelayed()
    {
        double began = EditorApplication.timeSinceStartup, idle = began;
        Debug.Log("R92_PROBE_SCHEDULED scene=" + GameScene);
        EditorApplication.CallbackFunction step = null;
        step = () => {
            double now = EditorApplication.timeSinceStartup;
            if (now - began > 180) {
                EditorApplication.update -= step;
                File.WriteAllText(Arg("-trial-report"), new JObject { ["result"]="IDLE_GATE_TIMEOUT", ["isUpdating"]=EditorApplication.isUpdating, ["isCompiling"]=EditorApplication.isCompiling }.ToString());
                EditorApplication.Exit(2); return;
            }
            if (EditorApplication.isUpdating || EditorApplication.isCompiling) { idle = now; return; }
            if (now - idle < 5) return;
            EditorApplication.update -= step;
            var report = new JObject { ["unity"]=Application.unityVersion, ["graphics"]=SystemInfo.graphicsDeviceType.ToString(), ["scene"]=GameScene, ["startupWaitSeconds"]=now-began, ["playModeEntered"]=false, ["gameplayModified"]=false };
            int exit=1;
            try {
                Debug.Log("R92_PROBE_OPEN_BEGIN isUpdating="+EditorApplication.isUpdating+" isCompiling="+EditorApplication.isCompiling);
                report["firstOpen"]=InspectScene(GameScene);
                report["sceneSaved"]=Array.IndexOf(Environment.GetCommandLineArgs(), "-trial-read-only") < 0 && EditorSceneManager.SaveScene(SceneManager.GetActiveScene());
                EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
                report["secondOpen"]=InspectScene(GameScene);
                // Kiểm checkpoint bị đổi kiểu và prefab; không chạy ma trận gameplay lịch sử.
                report["retainedScenes"]=new JArray(new[]{"Assets/Scenes/JourneyPresentation_v48_r90.unity","Assets/Scenes/JourneyPresentation_v48_r91.unity"}.Select(InspectScene));
                foreach(var audit in (JArray)report["retainedScenes"])
                    if(((JArray)audit["missingScripts"]).Count>0||((JArray)audit["brokenObjectReferences"]).Count>0)throw new Exception("Checkpoint references broken");
                report["prefabs"]=new JArray(AssetDatabase.FindAssets("t:Prefab",new[]{"Assets"}).Select(AssetDatabase.GUIDToAssetPath).Select(path=>{
                    var go=AssetDatabase.LoadAssetAtPath<GameObject>(path);
                    int missing=go?go.GetComponentsInChildren<Transform>(true).Sum(x=>GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(x.gameObject)):-1;
                    if(missing!=0)throw new Exception("Prefab missing script: "+path);
                    return new JObject{["path"]=path,["missingScripts"]=missing};
                }));
                InspectScene(GameScene);

                report["dependencies"]=new JArray(AssetDatabase.GetDependencies(GameScene,true));
                report["result"]="OPEN_REOPEN_COMPLETED_NOT_PLAYER_TESTED"; exit=0;
                Debug.Log("R92_PROBE_OPEN_REOPEN_COMPLETED");
            } catch(Exception e) { report["result"]="FAILED"; report["exception"]=e.ToString(); }
            File.WriteAllText(Arg("-trial-report"),report.ToString()); EditorApplication.Exit(exit);
        };
        EditorApplication.update += step;
    }
    static string Arg(string name)
    {
        var a = Environment.GetCommandLineArgs(); int i = Array.IndexOf(a, name);
        if (i < 0 || i + 1 >= a.Length) throw new Exception("Missing " + name);
        return Path.GetFullPath(a[i + 1]);
    }
    static JObject InspectScene(string path)
    {
        var scene = EditorSceneManager.OpenScene(path, OpenSceneMode.Single);
        var all = scene.GetRootGameObjects().SelectMany(g => g.GetComponentsInChildren<Transform>(true)).ToArray();
        var missing = new JArray(); var references = new JArray();
        foreach (var t in all)
        {
            int n = GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject);
            if (n != 0) missing.Add(new JObject { ["object"] = t.name, ["count"] = n });
            foreach (var c in t.GetComponents<Component>().Where(c => c))
            {
                var so = new SerializedObject(c); var it = so.GetIterator();
                while (it.NextVisible(true))
                    if (it.propertyType == SerializedPropertyType.ObjectReference &&
                        !it.objectReferenceValue && !it.objectReferenceEntityIdValue.Equals(default(EntityId)))
                        references.Add(new JObject { ["object"] = t.name, ["component"] = c.GetType().FullName, ["property"] = it.propertyPath });
            }
        }
        return new JObject { ["path"] = path, ["roots"] = scene.rootCount, ["objects"] = all.Length,
            ["missingScripts"] = missing, ["brokenObjectReferences"] = references,
            ["story"] = all.Count(t => t.GetComponent<DieuPhoiHanhTrinh>()),
            ["uiArbiters"] = all.Count(t => t.GetComponent<DieuPhoiGiaoDienR42>()),
            ["puppetControllers"] = all.Count(t => t.GetComponent<DieuKhienConRoi>()),
            ["canvases"] = all.Count(t => t.GetComponent<Canvas>()),
            ["playerEntry"] = all.Count(t => t.GetComponent<MenuVaoGameV47>()),
            ["preparation"] = all.Count(t => t.GetComponent<ChuanBiGoiHangR88>()),
            ["approaches"] = all.Sum(t => t.GetComponents<TiepCanCoChuDichR89>().Length),
            ["uiImplementation"] = "PlayerEntryV47 + JourneyUiR42 OnGUI; Canvas is not required",
            ["eventSystems"] = all.Count(t => t.GetComponent<EventSystem>()),
            ["inputModules"] = all.Sum(t => t.GetComponents<BaseInputModule>().Length) };
    }
    public static void Validate()
    {
        var report = new JObject { ["unity"] = Application.unityVersion, ["project"] = Directory.GetParent(Application.dataPath).FullName,
            ["editorCompileReachedEntry"] = true, ["playModeEntered"] = false, ["sceneSaved"] = false };
        int exit = 1;
        try
        {
            report["packageList"] = new JArray(UnityEditor.PackageManager.PackageInfo.GetAllRegisteredPackages().Select(p => new JObject { ["name"] = p.name, ["version"] = p.version, ["source"] = p.source.ToString() }));
            report["appUIConfigStillPresent"] = EditorBuildSettings.TryGetConfigObject<UnityEngine.Object>("com.unity.dt.app-ui", out var unused);
            var active = InspectScene(GameScene); report["gameScene"] = active;
            if (((JArray)active["missingScripts"]).Count != 0 || ((JArray)active["brokenObjectReferences"]).Count != 0)
                throw new Exception("Required game scene has missing scripts or broken serialized references");
            if ((int)active["story"] != 1 || (int)active["uiArbiters"] != 1 || (int)active["playerEntry"] != 1 || (int)active["preparation"] != 1 || (int)active["approaches"] != 2 || (int)active["puppetControllers"] < 1 || (int)active["eventSystems"] != 1 || (int)active["inputModules"] != 1)
                throw new Exception("Game scene bootstrap/UI/puppet/input owners differ");
            var binary = new JObject(); report["historicalBinaryAssets"] = binary;
            try { binary["recoveryScene"] = InspectScene("Assets/_Recovery/0 (2).unity"); }
            catch (Exception e) { binary["recoverySceneError"] = e.ToString(); }
            try
            {
                string p = "Assets/Art/Terrain/New Terrain.asset";
                var objects = AssetDatabase.LoadAllAssetsAtPath(p);
                binary["terrainObjects"] = new JArray(objects.Select(o => o ? o.GetType().FullName : "NULL"));
                binary["terrainLoaded"] = objects.Any(o => o is TerrainData);
            }
            catch (Exception e) { binary["terrainError"] = e.ToString(); }
            report["gameSceneAfterReopen"] = InspectScene(GameScene);
            report["gameDependencies"] = new JArray(AssetDatabase.GetDependencies(GameScene, true));
            report["prefabs"] = new JArray(AssetDatabase.FindAssets("t:Prefab", new[] { "Assets" }).Select(AssetDatabase.GUIDToAssetPath).Where(p => p.EndsWith(".prefab", StringComparison.OrdinalIgnoreCase)).Select(p => {
                var go = AssetDatabase.LoadAssetAtPath<GameObject>(p);
                return new JObject { ["path"] = p, ["loaded"] = go != null,
                    ["missingScripts"] = go ? go.GetComponentsInChildren<Transform>(true).Sum(t => GameObjectUtility.GetMonoBehavioursWithMissingScriptCount(t.gameObject)) : -1 };
            }));
            report["buildScenes"] = new JArray(EditorBuildSettings.scenes.Select(s => new JObject { ["path"] = s.path, ["enabled"] = s.enabled }));
            report["result"] = "EDITOR_VALIDATED_NOT_PLAYER_TESTED";
            exit = 0;
        }
        catch (Exception e) { report["result"] = "FAILED"; report["exception"] = e.ToString(); }
        File.WriteAllText(Arg("-trial-report"), report.ToString());
        EditorApplication.Exit(exit);
    }
    public static void Build()
    {
        var result = new JObject { ["scene"] = GameScene, ["unity"] = Application.unityVersion,
            ["origin"] = "Full BuildPipeline.BuildPlayer from D:/ConRoi source; retained active Library; new output, no old player files" };
        int exit = 1;
        try
        {
            string output = Arg("-trial-output");
            if (Directory.Exists(output) || File.Exists(output)) throw new Exception("Refuse existing output");
            var sceneAudit = InspectScene(GameScene);
            result["editorSceneAudit"] = sceneAudit;
            if (((JArray)sceneAudit["missingScripts"]).Count != 0 || ((JArray)sceneAudit["brokenObjectReferences"]).Count != 0
                || (int)sceneAudit["story"] != 1 || (int)sceneAudit["playerEntry"] != 1 || (int)sceneAudit["approaches"] != 2)
                throw new Exception("r92 scene bootstrap/reference validation failed");
            if (EditorBuildSettings.scenes.Count(x => x.enabled) != 1 || !EditorBuildSettings.scenes.Any(x => x.enabled && x.path == GameScene))
                throw new Exception("r92 is not the sole enabled build scene");
            result["sceneGuid"] = AssetDatabase.AssetPathToGUID(GameScene);
            result["dependencies"] = new JArray(AssetDatabase.GetDependencies(GameScene,true));
            Debug.Log("R92_BUILDPLAYER_BEGIN");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions { scenes = new[] { GameScene },
                locationPathName = Path.Combine(output, "ConRoi.exe"), target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.CleanBuildCache | BuildOptions.DetailedBuildReport });
            result["result"] = report.summary.result.ToString(); result["errors"] = report.summary.totalErrors;
            result["warnings"] = report.summary.totalWarnings; result["bytes"] = report.summary.totalSize;
            result["reviewDefine"] = "NONE: ordinary player";
            exit = report.summary.result == BuildResult.Succeeded ? 0 : 1;
        }
        catch (Exception e) { result["result"] = "FAILED"; result["exception"] = e.ToString(); }
        File.WriteAllText(Arg("-trial-report"), result.ToString());
        EditorApplication.Exit(exit);
    }
}
