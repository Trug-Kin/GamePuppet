#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEngine.Rendering;

// Dedicated offline-prepared scene only. No source-map, controller-setting or game-scene writes.
[ExecuteAlways, DefaultExecutionOrder(-20000)]
public sealed class C3RouteFixture : MonoBehaviour
{
    public TextAsset fixture;
    public TextAsset landscape;
    public GameObject landscapePrefab;
    public GameObject regionalContextPrefab;
    public bool useLandscapeCases;
    public DieuKhienDiChuyen player;
    public int selectedCase;
    public bool manualWalkFromW1;
    public string output = "Docs/Reviews/W_route_Unity_v30_20261016/runtime";
    GameObject generated;
    readonly List<UnityEngine.Object> owned = new List<UnityEngine.Object>();
    StreamWriter frames;
    JObject data;
    bool prepared;
    static readonly CultureInfo Inv = CultureInfo.InvariantCulture;

    void OnEnable()
    {
        // Never allow this fixture to run if dragged into a game scene.
        if (gameObject.scene.path != "Assets/WRouteC3Test_v30_20261016/DaLatMovementTest.unity") return;
        Build();
        if (Application.isPlaying)
        {
            if (manualWalkFromW1)
            {
                PrepareManualSpawn();
                if (!GetComponent<C3ManualWalk>()) gameObject.AddComponent<C3ManualWalk>();
            }
            else BeginCase();
        }
    }
    [ContextMenu("Rebuild this isolated fixture")]
    public void Build()
    {
        if (gameObject.scene.path != "Assets/WRouteC3Test_v30_20261016/DaLatMovementTest.unity")
            throw new InvalidOperationException("C3 fixture supports only its own test scene");
        ValidateProfile();
        Cleanup(); data = JObject.Parse(fixture.text);
        generated = new GameObject("V30_EXPORT_ONLY_NoOldC3Collider") {hideFlags = HideFlags.DontSave};
        generated.transform.SetParent(transform, false);
        var shader = Shader.Find("Universal Render Pipeline/Lit") ?? Shader.Find("Standard");
        var meshes=data["meshes"].AsEnumerable();
        if(useLandscapeCases)
        {
            var extension=JObject.Parse(landscape.text);
            meshes=meshes.Concat(extension["meshes"].Where(m=>!landscapePrefab || ((string)m["name"]).StartsWith("SUP_V31_")));
            data["cases"]=extension["cases"].DeepClone();
        }
        foreach (var item in meshes)
        {
            var mesh = new Mesh { name = (string)item["name"], indexFormat = IndexFormat.UInt32, hideFlags = HideFlags.DontSave };
            mesh.vertices = item["vertices"].Select(Point).ToArray();
            mesh.triangles = item["triangles"].Select(x => (int)x).ToArray();
            mesh.RecalculateNormals(); mesh.RecalculateBounds(); owned.Add(mesh);
            var go = new GameObject(mesh.name) {hideFlags = HideFlags.DontSave}; go.transform.SetParent(generated.transform, false); go.layer = 3;
            go.AddComponent<MeshFilter>().sharedMesh = mesh;
            var mat = new Material(shader) {hideFlags = HideFlags.DontSave}; var col = item["color"];
            mat.color = new Color((float)col[0], (float)col[1], (float)col[2], (float)col[3]);
            if (mat.HasProperty("_Smoothness")) mat.SetFloat("_Smoothness", .18f);
            owned.Add(mat); go.AddComponent<MeshRenderer>().sharedMaterial = mat;
            if ((string)item["role"] != "text_visual_only")
            {
                var collider = go.AddComponent<MeshCollider>();
                collider.sharedMesh = mesh; collider.convex = false; collider.isTrigger = false;
            }
        }
        if(useLandscapeCases && landscapePrefab)
        {
            var instance=Instantiate(landscapePrefab,generated.transform,false);
            instance.name="LakeShoreV31_ACTUAL_IMPORTED_PREFAB";
            instance.hideFlags=HideFlags.DontSave;
        }
        if(useLandscapeCases && regionalContextPrefab)
        {
            var context=Instantiate(regionalContextPrefab,generated.transform,false);
            context.name="LakeGateMarket_PRESERVED_IMPORTED_CONTEXT";context.hideFlags=HideFlags.DontSave;
            foreach(var renderer in generated.GetComponentsInChildren<MeshRenderer>())
                if(renderer.name.StartsWith("SUP_V31_"))renderer.enabled=false;
        }
        var sun = new GameObject("Local test daylight") {hideFlags = HideFlags.DontSave}; sun.transform.SetParent(generated.transform, false);
        sun.transform.rotation = Quaternion.Euler(48, -30, 0);
        var light = sun.AddComponent<Light>(); light.type = LightType.Directional; light.intensity = 1;
        light.shadows = LightShadows.Soft;
        Physics.SyncTransforms(); prepared = true;
    }
    public void ValidateProfile()
    {
        if (fixture == null || player == null) throw new InvalidOperationException("Missing C3 fixture/player");
        var c = player.controller;
        if (c.height != 1.8f || c.radius != .3f || c.center != Vector3.zero || c.skinWidth != .03f
            || c.stepOffset != .3f || c.slopeLimit != 45 || c.minMoveDistance != .001f
            || player.transform.lossyScale != Vector3.one || player.speed != 12 || player.gravity != -19.62f
            || player.jumpHeight != 3 || player.groundDistance != .15f || player.groundMask.value != 8
            || player.groundCheck.localPosition != new Vector3(0, -.8f, 0) || !player.useImmediateMovementInput)
            throw new InvalidOperationException("Frozen 180cm controller profile differs; refusing to retune");
    }
    static Vector3 Point(JToken x) => new Vector3((float)x[0], (float)x[1], (float)x[2]);
    public void PrepareManualSpawn()
    {
        if (!manualWalkFromW1) throw new InvalidOperationException("Manual mode is required");
        ValidateProfile(); player.DisableTestInputHook();
        if (GetComponent<C3ActualPlayRun>()) throw new InvalidOperationException("Remove automatic runner before manual preparation");
        var fr = data["frame"]; var F=Point(fr["F"]);var u=Point(fr["u"]);var n=Point(fr["n"]);
        var source=F+u*(-7f)+n*3.4f;var point=new Vector3(-source.x,0,-source.y);
        Physics.SyncTransforms();float z=float.NegativeInfinity;
        foreach(float x in new[]{-.3f,0,.3f})foreach(float v in new[]{-.3f,0,.3f})
        {
            var hits=Physics.RaycastAll(point+new Vector3(x,1000,v),Vector3.down,2000,8,QueryTriggerInteraction.Ignore).Where(h=>h.collider.name.StartsWith("SUP_"));
            foreach(var h in hits)z=Mathf.Max(z,h.point.y);
        }
        if(float.IsNegativeInfinity(z))throw new InvalidOperationException("No support at W1 spawn");
        var cc=player.controller;bool wasEnabled=cc.enabled;cc.enabled=false;
        player.transform.position=new Vector3(point.x,z+.96f,point.z);
        player.transform.rotation=Quaternion.LookRotation(new Vector3(-u.x,0,-u.y));
        typeof(DieuKhienDiChuyen).GetField("velocity",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(player,Vector3.zero);
        // Persist playable controls independently of the test recorder.
        cc.enabled=true;
        player.enabled=true;
        var look=player.GetComponent<DieuKhienGocNhin>();
        if(look)look.enabled=true;
        Physics.SyncTransforms();
    }
    [ContextMenu("Begin selected case in Play mode")]
    public void BeginCase()
    {
        if (!Application.isPlaying || !prepared) return;
        if (manualWalkFromW1) throw new InvalidOperationException("Automatic case spawn disabled in manual mode");
        ValidateProfile(); frames?.Dispose(); frames = null;
        var cases = (JArray)data["cases"]; selectedCase = Mathf.Clamp(selectedCase, 0, cases.Count - 1);
        var item = cases[selectedCase]; var p = Point(item["points"][0]);
        var cc = player.controller; cc.enabled = false;
        // Spawn placement only, never alter capsule or controller settings.
        float support = p.y;
        foreach (float x in new[] { -.3f, 0, .3f }) foreach (float z in new[] { -.3f, 0, .3f })
            if (Physics.Raycast(p + new Vector3(x, 2, z), Vector3.down, out var h, 4, 8, QueryTriggerInteraction.Ignore)) support = Mathf.Max(support, h.point.y);
        player.transform.position = new Vector3(p.x, support + .9f + .06f, p.z);
        Vector3 heading = item["points"].Count() > 1 ? Point(item["points"][1]) - p : Point(data["cases"][0]["points"][1]) - Point(data["cases"][0]["points"][0]);
        heading.y = 0; player.transform.rotation = Quaternion.LookRotation(heading);
        typeof(DieuKhienDiChuyen).GetField("velocity", BindingFlags.NonPublic | BindingFlags.Instance).SetValue(player, Vector3.zero);
        // Source scene has game/Compliance/other recorders disabled already.
        cc.enabled = true; player.enabled = true; player.DisableTestInputHook(); Physics.SyncTransforms();
        Directory.CreateDirectory(output); string stamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss_fffffff");
        frames = new StreamWriter(Path.Combine(output, (string)item["id"] + "_" + stamp + "_frames.csv")) { AutoFlush = true };
        frames.WriteLine("time,x,y,z,controller_grounded,ground_check,input_x,input_y,cc_vx,cc_vy,cc_vz");
        File.WriteAllText(Path.Combine(output, "session_" + stamp + ".json"),
            new JObject { ["case"] = item["id"], ["status"] = "PLAY_MODE_STARTED_NO_AUTOMATIC_VERDICT", ["scene"] = gameObject.scene.path }.ToString());
        Debug.Log("C3 case " + item["id"] + ": WASD to traverse; F6 next case. Check actual telemetry and annotate observations; no automatic PASS.");
    }
    void Update()
    {
        if (!Application.isPlaying || !prepared) return;
        if (manualWalkFromW1) return;
        if (Input.GetKeyDown(KeyCode.F6)) { selectedCase = (selectedCase + 1) % data["cases"].Count(); BeginCase(); }
    }
    void LateUpdate()
    {
        if (!Application.isPlaying || frames == null) return;
        var p = player.transform.position; var v = player.controller.velocity; var input = player.ReadMovementInput();
        bool ground = Physics.CheckSphere(player.groundCheck.position, player.groundDistance, player.groundMask);
        frames.WriteLine(string.Join(",", Time.realtimeSinceStartup.ToString("R", Inv), p.x.ToString("R", Inv), p.y.ToString("R", Inv), p.z.ToString("R", Inv), player.controller.isGrounded, ground, input.x.ToString("R", Inv), input.y.ToString("R", Inv), v.x.ToString("R", Inv), v.y.ToString("R", Inv), v.z.ToString("R", Inv)));
    }
    void OnDrawGizmos()
    {
        if (data == null) return; Gizmos.color = Color.cyan;
        foreach (var c in data["cases"])
        {
            var pts = c["points"].Select(Point).ToArray();
            foreach (var p in pts) Gizmos.DrawWireSphere(p + Vector3.up * .05f, .10f);
            for (int i = 1; i < pts.Length; i++) Gizmos.DrawLine(pts[i - 1] + Vector3.up * .05f, pts[i] + Vector3.up * .05f);
        }
    }
    void OnDisable() { frames?.Dispose(); frames = null; Cleanup(); }
    void Cleanup()
    {
        prepared = false;
        if (generated) { generated.SetActive(false); if (Application.isPlaying) Destroy(generated); else DestroyImmediate(generated); }
        foreach (var o in owned) if (o) { if (Application.isPlaying) Destroy(o); else DestroyImmediate(o); }
        owned.Clear(); generated = null;
    }
}

#endif
