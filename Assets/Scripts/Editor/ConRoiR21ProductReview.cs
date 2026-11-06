using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

// Product work only: no OS input, activation, launcher changes or frozen-build targets.
[InitializeOnLoad]
public static class ConRoiR21ProductReview
{
    public const string Source = "Assets/Scenes/JourneyRepair_v48_r21_recoveryprep.unity";
    public const string Scene = "Assets/Scenes/JourneyRepair_v48_r21_playtest.unity";
    public const string Out = "Docs/Reviews/journey_v48/r21/product-review";
    const string Art = "Assets/Art/Environment/JourneyV48/PlaytestR21";
    const string Flower = "Docs/Reviews/journey_v48/r21/flower-refinement";
    const string BuildPath = "Builds/Windows_v48_r21_playtest/ConRoi.exe";
    static IEnumerator routine;
    static double deadline;
    static readonly JArray checks = new JArray();
    static readonly JArray runtimeErrors = new JArray();
    static ConRoiR21ProductReview()
    {
        EditorApplication.playModeStateChanged += state =>
        {
            if (state == PlayModeStateChange.EnteredPlayMode && SessionState.GetBool("r21.product.play", false))
            {
                routine = PlayChecks(); deadline = EditorApplication.timeSinceStartup + 180;
                Application.logMessageReceived += RuntimeLog;
                EditorApplication.update += Tick;
            }
        };
    }
    static void RuntimeLog(string text, string trace, LogType type)
    {
        if (type == LogType.Error || type == LogType.Exception || type == LogType.Assert)
            runtimeErrors.Add(new JObject { ["type"] = type.ToString(), ["message"] = text, ["trace"] = trace });
    }
    static string Hash(string text)
    {
        using (var h = SHA256.Create()) return BitConverter.ToString(h.ComputeHash(System.Text.Encoding.UTF8.GetBytes(text))).Replace("-", "").ToLowerInvariant();
    }
    static JArray V(Vector3 v) => new JArray(v.x, v.y, v.z);
    static string PathOf(Transform t) => (t.parent ? PathOf(t.parent) + "/" : "") + t.name + "[" + t.GetSiblingIndex() + "]";
    static T[] All<T>() where T : UnityEngine.Object => UnityEngine.Object.FindObjectsByType<T>(FindObjectsInactive.Include);
    static JObject ColliderSnapshot()
    {
        var result = new JObject();
        foreach (var c in All<Collider>().OrderBy(c => PathOf(c.transform)))
            result[PathOf(c.transform) + ":" + c.GetType().Name] = new JObject
            {
                ["serialized"] = EditorJsonUtility.ToJson(c),
                ["matrix"] = new JArray(Enumerable.Range(0, 16).Select(i => c.transform.localToWorldMatrix[i])),
                ["mesh"] = c is MeshCollider m && m.sharedMesh ? AssetDatabase.GetAssetPath(m.sharedMesh) : ""
            };
        return result;
    }
    static DieuPhoiHanhTrinh Open(string path)
    {
        EditorSceneManager.OpenScene(path);
        return UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
    }
    static void Visible(DieuPhoiHanhTrinh s)
    {
        s.hub.SetActive(false); s.lake.SetActive(true); s.market.SetActive(true);
        foreach (var g in s.GetComponent<CanhQuanChoV48>().scenery) if (g) g.SetActive(true);
        Physics.SyncTransforms();
    }
    static void Require(bool ok, string message) { if (!ok) throw new Exception(message); }
    public static void Prepare()
    {
        Directory.CreateDirectory(Out);
        Require(!File.Exists(Scene), "Preserve existing derivative scene");
        Directory.CreateDirectory(Art);
        foreach (var name in new[] { "FlowerModule_Refined.fbx", "After_2_BaseColorSmoothness.png", "After_Yellow_BaseColorSmoothness.png" })
            File.Copy(Flower + "/" + name, Art + "/" + name, false);
        AssetDatabase.Refresh();
        var importer = (ModelImporter)AssetImporter.GetAtPath(Art + "/FlowerModule_Refined.fbx");
        importer.importAnimation = false; importer.addCollider = false; importer.globalScale = 1; importer.useFileScale = true;
        importer.SaveAndReimport();
        foreach (var name in new[] { "After_2_BaseColorSmoothness.png", "After_Yellow_BaseColorSmoothness.png" })
        {
            var ti = (TextureImporter)AssetImporter.GetAtPath(Art + "/" + name);
            ti.sRGBTexture = true; ti.alphaSource = TextureImporterAlphaSource.FromInput;
            ti.alphaIsTransparency = false; ti.mipmapEnabled = true; ti.wrapMode = TextureWrapMode.Clamp;
            ti.maxTextureSize = 512; ti.SaveAndReimport();
        }
        var s = Open(Source);
        var root = All<Transform>().Single(t => t.name == "R21_RealPlanterFlowerSample");
        var fs = root.GetComponentsInChildren<MeshFilter>(true).OrderBy(f => f.name).ToArray();
        Require(fs.Length == 102, "Expected 102 existing flower modules");
        var beforeMesh = fs[0].sharedMesh;
        var placements = new JArray(fs.Select(f => new JObject { ["name"] = f.name, ["position"] = V(f.transform.position), ["rotation"] = V(f.transform.eulerAngles), ["scale"] = V(f.transform.localScale) }));
        var oldMaterials = fs[0].GetComponent<Renderer>().sharedMaterials;
        var imported = UnityEngine.Object.Instantiate(AssetDatabase.LoadAssetAtPath<GameObject>(Art + "/FlowerModule_Refined.fbx"));
        var im = imported.GetComponentInChildren<MeshFilter>(); var matrix = im.transform.localToWorldMatrix;
        var mesh = UnityEngine.Object.Instantiate(im.sharedMesh); mesh.name = "R21_RefinedFlower_Metres";
        mesh.vertices = mesh.vertices.Select(matrix.MultiplyPoint3x4).ToArray();
        mesh.normals = mesh.normals.Select(n => matrix.inverse.transpose.MultiplyVector(n).normalized).ToArray(); mesh.RecalculateBounds();
        Require(Vector3.Distance(beforeMesh.bounds.min, mesh.bounds.min) < .00001f && Vector3.Distance(beforeMesh.bounds.max, mesh.bounds.max) < .00001f, "Flower bounds changed");
        var sourceNames = im.GetComponent<Renderer>().sharedMaterials.Select(m => m.name).ToArray();
        UnityEngine.Object.DestroyImmediate(imported);
        AssetDatabase.CreateAsset(mesh, Art + "/RefinedFlowerMetres.asset");
        var foliage = new Material(oldMaterials.Single(m => m.name == "Foliage")); foliage.name = "Foliage_Review"; AssetDatabase.CreateAsset(foliage, Art + "/Foliage.mat");
        var centre = new Material(oldMaterials.Single(m => m.name == "Centre")); centre.name = "Centre_Review"; AssetDatabase.CreateAsset(centre, Art + "/Centre.mat");
        Material Petal(string name, string map)
        {
            var mat = new Material(Shader.Find("Universal Render Pipeline/Lit")) { name = name, enableInstancing = true };
            mat.SetTexture("_BaseMap", AssetDatabase.LoadAssetAtPath<Texture2D>(Art + "/" + map)); mat.SetColor("_BaseColor", Color.white);
            mat.SetFloat("_Smoothness", 1); mat.SetFloat("_SmoothnessTextureChannel", 1); mat.EnableKeyword("_SMOOTHNESS_TEXTURE_ALBEDO_CHANNEL_A");
            mat.SetFloat("_Surface", 0); mat.SetFloat("_AlphaClip", 0); AssetDatabase.CreateAsset(mat, Art + "/" + name + ".mat"); return mat;
        }
        var red = Petal("PetalRed_Review", "After_2_BaseColorSmoothness.png");
        var yellow = Petal("PetalYellow_Review", "After_Yellow_BaseColorSmoothness.png");
        AssetDatabase.SaveAssets();
        // Real scene before/after uses identical cameras and lights. No temporary render state saved.
        RenderViews(s, "before", true);
        s = Open(Source);
        Require(AssetDatabase.CopyAsset(Source, Scene), "Scene copy failed"); s = Open(Scene);
        root = All<Transform>().Single(t => t.name == "R21_RealPlanterFlowerSample");
        foreach (var f in root.GetComponentsInChildren<MeshFilter>(true))
        {
            var r = f.GetComponent<Renderer>(); bool gold = r.sharedMaterials.Any(m => m && m.name == "PetalYellow");
            f.sharedMesh = mesh;
            r.sharedMaterials = sourceNames.Select(n => n.StartsWith("After_0") ? foliage : n.StartsWith("After_1") ? centre : gold ? yellow : red).ToArray();
        }
        var profile = s.gameObject.AddComponent<HoSoChoiThuR21>(); profile.story = s;
        // Instance IDs vary when reopening; compare collider references through stable asset data below.
        var stableBefore = StablePhysics(Source); var stableAfter = StablePhysics(Scene, false);
        Require(Equivalent(stableBefore, stableAfter), "Physics or player settings changed");
        EditorSceneManager.SaveScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene()); AssetDatabase.SaveAssets();
        File.WriteAllText(Out + "/integration.json", new JObject {
            ["source"] = Source, ["scene"] = Scene, ["modules"] = fs.Length, ["triangles_per_module"] = mesh.triangles.Length / 3,
            ["source_material_order"] = new JArray(sourceNames), ["placements"] = placements,
            ["before_bounds_min"] = V(beforeMesh.bounds.min), ["after_bounds_min"] = V(mesh.bounds.min),
            ["before_bounds_max"] = V(beforeMesh.bounds.max), ["after_bounds_max"] = V(mesh.bounds.max),
            ["physics_equal"] = true, ["physics_signature"] = Hash(stableAfter.ToString()), ["physics"] = stableAfter,
            ["scope"] = "Only one 102-module bed mesh/material replacement plus derivative-only save profile. No controller, NPC, water, fog or boundary edits."
        }.ToString());
        RenderViews(UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>(), "after", true);
        Open(Scene);
    }
    static bool Equivalent(JToken a,JToken b)
    {
        if(a is JObject ao && b is JObject bo)return ao.Properties().Select(p=>p.Name).OrderBy(n=>n).SequenceEqual(bo.Properties().Select(p=>p.Name).OrderBy(n=>n))&&ao.Properties().All(p=>Equivalent(p.Value,bo[p.Name]));
        if(a is JArray aa && b is JArray ba)return aa.Count==ba.Count&&aa.Zip(ba,Equivalent).All(x=>x);
        if((a.Type==JTokenType.Float||a.Type==JTokenType.Integer)&&(b.Type==JTokenType.Float||b.Type==JTokenType.Integer))return Math.Abs((double)a-(double)b)<.0001;
        return JToken.DeepEquals(a,b);
    }
    public static void VerifyIntegrated()
    {
        var before=StablePhysics(Source);var after=StablePhysics(Scene);
        File.WriteAllText(Out+"/physics-before.json",before.ToString());File.WriteAllText(Out+"/physics-after.json",after.ToString());
        Require(Equivalent(before,after),"Physics differs beyond 0.1mm serialization tolerance");
        var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
        var fs=All<Transform>().Single(t=>t.name=="R21_RealPlanterFlowerSample").GetComponentsInChildren<MeshFilter>(true);
        Require(fs.Length==102&&fs.All(f=>AssetDatabase.GetAssetPath(f.sharedMesh)==Art+"/RefinedFlowerMetres.asset"),"Integrated module count/binding");
        File.WriteAllText(Out+"/integration.json",new JObject{["source"]=Source,["scene"]=Scene,["modules"]=fs.Length,["triangles_per_module"]=fs[0].sharedMesh.triangles.Length/3,["physics_equivalent"]=true,["numeric_tolerance"]=.0001,["method"]="Full collider path, matrix, binding, enabled, layer, trigger and controller parameter comparison. Source scene/assets retained; only one bed and derivative save profile changed."}.ToString());
        RenderViews(s,"after",true);s=Open(Scene);Visible(s);
        Directory.CreateDirectory(Out+"/preflight");
        var flags=System.Reflection.BindingFlags.Static|System.Reflection.BindingFlags.NonPublic;
        typeof(ConRoiR21Preflight).GetField("Out",flags).SetValue(null,Out+"/preflight");
        typeof(ConRoiR21Preflight).GetMethod("RouteRegression",flags).Invoke(null,null);
        var routes=JObject.Parse(File.ReadAllText(Out+"/preflight/route-regression.json"));
        Require(routes["walks"].All(w=>(int)w["reached"]==(int)w["targets"]),"Route regression");
        Open(Scene);
    }
    static JObject StablePhysics(string path, bool open = true)
    {
        // Preserve unsaved integrated scene when comparing the baseline.
        var current = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (open && current.path == Scene) EditorSceneManager.SaveScene(current);
        var s = open ? Open(path) : UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
        if (!open && UnityEngine.SceneManagement.SceneManager.GetActiveScene().path != path) s = Open(path);
        var rows = new JArray(All<Collider>().OrderBy(c => PathOf(c.transform)).Select(c => new JObject {
            ["path"] = PathOf(c.transform), ["type"] = c.GetType().Name, ["enabled"] = c.enabled, ["activeSelf"] = c.gameObject.activeSelf,
            ["layer"] = c.gameObject.layer, ["trigger"] = c.isTrigger,
            ["matrix"] = new JArray(Enumerable.Range(0,16).Select(i => c.transform.localToWorldMatrix[i])),
            ["mesh"] = c is MeshCollider mc && mc.sharedMesh ? AssetDatabase.GetAssetPath(mc.sharedMesh) : "",
            ["convex"] = c is MeshCollider m && m.convex
        }));
        return new JObject { ["colliders"] = rows, ["controller"] = new JObject {
            ["height"] = s.player.controller.height, ["radius"] = s.player.controller.radius, ["step"] = s.player.controller.stepOffset,
            ["skin"] = s.player.controller.skinWidth, ["center"] = V(s.player.controller.center), ["minMove"] = s.player.controller.minMoveDistance,
            ["slope"] = s.player.controller.slopeLimit, ["speed"] = s.player.speed, ["jump"] = s.player.jumpHeight, ["gravity"] = s.player.gravity } };
    }
    static readonly (string name, Vector3 eye, Vector3 target)[] Views = {
        ("flower-player", new Vector3(642.5465f,2.9413f,545.6624f),new Vector3(641.0465f,1.72f,547.6624f)),
        ("flower-close",new Vector3(640.8f,2.24f,546.6f),new Vector3(641.1f,1.74f,547.4f)),
        ("planter-bridge",new Vector3(619,1.7f,554),new Vector3(638,1.05f,542)),
        ("water-shore",new Vector3(649,3.7f,500),new Vector3(654,.3f,506)),
        ("boundary-west",new Vector3(692,0,586),new Vector3(700,-3,590)),
        ("bridge-return",new Vector3(686,2.1f,494),new Vector3(644,1.6f,533))
    };
    static void Capture(Camera eye, string path)
    {
        var rt = RenderTexture.GetTemporary(1400,900,24); var previous = RenderTexture.active;
        var tex = new Texture2D(1400,900,TextureFormat.RGB24,false);
        try {
            for (int i=0;i<3;i++) RenderPipeline.SubmitRenderRequest(eye,new UniversalRenderPipeline.SingleCameraRequest { destination=rt });
            RenderTexture.active=rt;tex.ReadPixels(new Rect(0,0,1400,900),0,0);tex.Apply(); File.WriteAllBytes(path,tex.EncodeToPNG());
        } finally { RenderTexture.active=previous;RenderTexture.ReleaseTemporary(rt);UnityEngine.Object.DestroyImmediate(tex); }
    }
    static void RenderViews(DieuPhoiHanhTrinh s, string stage, bool clear)
    {
        Visible(s); Directory.CreateDirectory(Out + "/" + stage);
        var eye=UnityEngine.Object.Instantiate(s.player.GetComponentInChildren<Camera>());eye.transform.SetParent(null);eye.gameObject.SetActive(true);eye.enabled=false;eye.fieldOfView=60;eye.aspect=1400f/900;eye.farClipPlane=1000;
        RenderSettings.fog=!clear;var rows=new JArray();
        foreach(var view in Views) { eye.transform.SetPositionAndRotation(view.eye,Quaternion.LookRotation(view.target-view.eye));Capture(eye,Out+"/"+stage+"/"+view.name+".png"); rows.Add(new JObject{["name"]=view.name,["eye"]=V(view.eye),["target"]=V(view.target),["fov"]=eye.fieldOfView}); }
        File.WriteAllText(Out+"/"+stage+"/method.json",new JObject{["method"]="Editor URP render request, real scene, same cameras/lights, fog disabled for geometry. No runtime NPC pose or Windows input claim.",["views"]=rows}.ToString());
        UnityEngine.Object.DestroyImmediate(eye.gameObject);
    }
    public static void RunPlay()
    {
        Open(Scene); SessionState.SetBool("r21.product.play",true); EditorApplication.EnterPlaymode();
    }
    static void Tick()
    {
        try {
            EditorApplication.QueuePlayerLoopUpdate();
            if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Play Mode verification deadline");
            if(routine.MoveNext())return;
            Require(runtimeErrors.Count==0,"Runtime errors recorded");
            Finish(0,null);
        } catch(Exception e) { Finish(1,e.ToString()); }
    }
    static void Finish(int code,string error)
    {
        File.WriteAllText(Out+"/playmode.json",new JObject{["status"]=code==0?"PASS_AUTOMATED_PLAY_MODE":"FAIL_AUTOMATED_PLAY_MODE",["method"]="Editor Play Mode, direct story API and render requests. No OS input, no native acceptance.",["checks"]=checks,["runtime_errors"]=runtimeErrors,["error"]=error}.ToString());
        SessionState.SetBool("r21.product.play",false);EditorApplication.update-=Tick;Application.logMessageReceived-=RuntimeLog;
        EditorApplication.Exit(code);
    }
    static void Check(string name,bool ok,JToken details=null)
    {
        checks.Add(new JObject{["name"]=name,["pass"]=ok,["details"]=details});Require(ok,name);
    }
    static IEnumerator Delay(double seconds) { double until=EditorApplication.timeSinceStartup+seconds;while(EditorApplication.timeSinceStartup<until)yield return null; }
    static IEnumerator PlayChecks()
    {
        var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
        var profile=s.GetComponent<HoSoChoiThuR21>();
        // A headless Editor has no EndOfFrame callback. Keep this workaround in
        // the test fixture: the shipped loading gate is unchanged and unverified.
        UnityEngine.InputSystem.InputSystem.settings.updateMode=UnityEngine.InputSystem.InputSettings.UpdateMode.ProcessEventsManually;
        Application.runInBackground=true;
        Check("derivative_profile_active_before_commands",profile&&s.SavePathOverride==profile.SavePath&&profile.SavePath.Contains("r21-playtest"),new JValue(s.SavePathOverride));
        // Keep automated artifacts separate even from the user's eventual playtest profile.
        s.SavePathOverride=System.IO.Path.GetFullPath(Out+"/playmode-save.json");
        s.SetMenu(false);s.Restart();
        var wait=Delay(1);while(wait.MoveNext())yield return null;
        var loader=s.GetComponent<ManHinhTaiV44>();
        loader.StopAllCoroutines();
        Directory.CreateDirectory(Out+"/play");
        Capture(s.player.GetComponentInChildren<Camera>(),Out+"/play/room-first-frame.png");
        s.SetLoading(false);
        Check("batch_fixture_explicit_first_frame",true,new JValue("Synchronous URP render/readback, then release Loading only in test. WaitForEndOfFrame gate NOT certified; OS input processing disabled in this fixture."));
        s.Sleep();while(s.Loading||s.data.region!=0)yield return null;
        wait=Delay(3);while(wait.MoveNext())yield return null;
        Check("normal_story_transition_to_lake",s.data.region==0&&!s.MenuOpen);
        var bridge=s.GetComponent<TuTheBaoBenCauV48>();
        var eye=s.player.GetComponentInChildren<Camera>();
        var mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();
        s.player.enabled=false;if(mouse)mouse.enabled=false;
        foreach(var canvas in All<Canvas>())canvas.enabled=false;
        var mist=s.GetComponent<SuongDaLatV48>();
        Directory.CreateDirectory(Out+"/play");
        foreach(bool clear in new[]{true,false}) {
            mist.clearGeometryReview=clear;
            wait=Delay(3);while(wait.MoveNext())yield return null;
            foreach(var view in Views) {
                eye.transform.SetPositionAndRotation(view.eye,Quaternion.LookRotation(view.target-view.eye));
                yield return null;
                Capture(eye,Out+"/play/"+view.name+(clear?"-clear":"-mist")+".png");
            }
        }
        mist.clearGeometryReview=false;
        var contact=BaoContactProbeV48.Sample(bridge);
        Check("bao_leaning_without_fishing_props",bridge.leaningOnly&&(!bridge.rod||!bridge.rod.gameObject.activeSelf)&&(!bridge.line||!bridge.line.gameObject.activeSelf)&&(!bridge.bag||!bridge.bag.gameObject.activeSelf));
        Check("bao_rendered_idle_contact",ContactOK(contact),contact);
        var baoEye=new Vector3(655.66f,2.54f,513.57f);var baoTarget=new Vector3(657.26f,2.0f,512.21f);
        eye.transform.SetPositionAndRotation(baoEye,Quaternion.LookRotation(baoTarget-baoEye));yield return null;Capture(eye,Out+"/play/bao-idle.png");
        var wideEye=bridge.actor.position+new Vector3(-2.5f,1.45f,2f);
        eye.transform.SetPositionAndRotation(wideEye,Quaternion.LookRotation(bridge.actor.position+Vector3.up*.8f-wideEye));yield return null;Capture(eye,Out+"/play/bao-side-full.png");
        eye.transform.SetPositionAndRotation(baoEye,Quaternion.LookRotation(baoTarget-baoEye));
        var line=s.completion.NextTalk(0);s.Say(line);
        wait=Delay(.4);while(wait.MoveNext())yield return null;
        contact=BaoContactProbeV48.Sample(bridge);Check("bao_rendered_dialogue_contact",ContactOK(contact),contact);Capture(eye,Out+"/play/bao-dialogue.png");
        var recovery=s.GetComponent<PhucHoiTaiBienR21>();
        s.Place(recovery.verifiedFallback,Quaternion.Euler(0,31,0));
        wait=Delay(1.5);while(wait.MoveNext())yield return null;
        s.Save();Check("private_game_save_written",File.Exists(s.SavePathOverride));
        var expected=JsonUtility.FromJson<DieuPhoiHanhTrinh.State>(File.ReadAllText(s.SavePathOverride));int loads=s.SuccessfulLoads;
        s.Place(recovery.verifiedFallback+Vector3.right,Quaternion.Euler(0,80,0));s.Load();
        Check("private_save_load_restores_pose_progress",s.SuccessfulLoads==loads+1&&Vector3.Distance(s.player.transform.position,expected.position)<.01f&&Quaternion.Angle(s.player.transform.rotation,expected.rotation)<.1f&&s.data.baoTalks==expected.baoTalks);
        s.player.enabled=false;if(mouse)mouse.enabled=false;
        wait=Delay(.5);while(wait.MoveNext())yield return null;
        contact=BaoContactProbeV48.Sample(bridge);Check("bao_after_load_contact",ContactOK(contact),contact);
        eye.transform.SetPositionAndRotation(baoEye,Quaternion.LookRotation(baoTarget-baoEye));Capture(eye,Out+"/play/bao-after-load.png");
        Check("reflection_bound_to_active_water",UnityEngine.Object.FindAnyObjectByType<PhanChieuMatHo>().water.gameObject.activeInHierarchy);
        Check("fog_profile_preserved",RenderSettings.fog&&RenderSettings.fogMode==FogMode.ExponentialSquared&&RenderSettings.fogDensity>0);
    }
    static bool ContactOK(JObject contact)
    {
        return contact!=null&&contact["shoes"].Any()&&contact["sleeves"].Any()&&contact["shoes"].All(s=>s["gap"].Type!=JTokenType.Null&&Mathf.Abs((float)s["gap"])<.025f)&&contact["sleeves"].All(s=>(float)s["triangleToTopRailSurfaceMin"]>=-.003f&&(float)s["triangleToTopRailSurfaceMin"]<.025f);
    }
    public static void Build()
    {
        Require((string)JObject.Parse(File.ReadAllText(Out+"/playmode.json"))["status"]=="PASS_AUTOMATED_PLAY_MODE","Play Mode must pass before build");
        Require(!Directory.Exists(System.IO.Path.GetDirectoryName(BuildPath)),"Never replace a retained build");Open(Scene);
        var report=BuildPipeline.BuildPlayer(new[]{Scene},BuildPath,BuildTarget.StandaloneWindows64,BuildOptions.Development);
        var messages=new JArray();foreach(var step in report.steps)foreach(var m in step.messages)if(m.type==LogType.Warning||m.type==LogType.Error||m.type==LogType.Exception)messages.Add(new JObject{["step"]=step.name,["type"]=m.type.ToString(),["message"]=m.content});
        File.WriteAllText(Out+"/build.json",new JObject{["result"]=report.summary.result.ToString(),["errors"]=report.summary.totalErrors,["warnings"]=report.summary.totalWarnings,["messages"]=messages,["scene"]=Scene,["binary"]=BuildPath,["native_input"]="NOT_RUN; recoveryprep ledger unchanged"}.ToString());
        Require(report.summary.result==UnityEditor.Build.Reporting.BuildResult.Succeeded,"Playtest build failed");
    }
}
