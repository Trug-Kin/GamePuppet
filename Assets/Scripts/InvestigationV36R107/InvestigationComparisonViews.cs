using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace ConRoi.Investigation.V36.R107
{
    public enum ComparisonSide { Left, Right }
    public enum ComparisonAvailability { Empty, Ready, MissingVisual, Paused, WrongContext }
    public readonly struct ComparisonViewState
    {
        public readonly string EvidenceId, Transcript, Diagnostic;
        public readonly EvidenceViewPose Pose;
        public readonly Texture Preview;
        public readonly ComparisonAvailability Availability;
        public bool CanControl => Availability == ComparisonAvailability.Ready;
        internal ComparisonViewState(string id, string transcript, string diagnostic, EvidenceViewPose pose, Texture preview, ComparisonAvailability availability)
        { EvidenceId = id; Transcript = transcript; Diagnostic = diagnostic; Pose = pose; Preview = preview; Availability = availability; }
    }
    // A visual-only snapshot: scripts, physics, cameras and audio from the source are never instantiated.
    internal sealed class ComparisonPreview : IDisposable
    {
        static int sequence;
        GameObject root; Transform model; Camera camera; RenderTexture texture;
        readonly List<Mesh> ownedMeshes = new List<Mesh>();
        readonly Vector3 anchor;
        internal EvidenceViewPose Pose { get; private set; }
        internal Texture Texture => texture;
        internal ComparisonPreview(Transform source, EvidenceViewPose pose)
        {
            anchor = new Vector3(9000 + (sequence++ % 10000) * 30, 1000, 0);
            root = new GameObject("Comparison visual snapshot") { hideFlags = HideFlags.DontSave };
            root.SetActive(false); root.transform.position = anchor;
            try
            {
                model = Snapshot(source, root.transform, true);
                var renderers = model.GetComponentsInChildren<Renderer>(true).Where(r => r.enabled).ToArray();
                if (renderers.Length == 0) throw new InvalidOperationException("No supported visible geometry");
                root.SetActive(true);
                var bounds = renderers[0].bounds; foreach (var r in renderers) bounds.Encapsulate(r.bounds);
                model.localScale *= 1.2f / Mathf.Max(.05f, bounds.size.magnitude);
                bounds = renderers[0].bounds; foreach (var r in renderers) bounds.Encapsulate(r.bounds);
                model.position += anchor - bounds.center;
                var cameraObject = new GameObject("Comparison snapshot camera"); cameraObject.transform.SetParent(root.transform, false);
                camera = cameraObject.AddComponent<Camera>(); camera.cullingMask = 1 << 30; camera.fieldOfView = 35;
                // Normalized models stay within 1.2 units; never render the next slot 30 units away.
                camera.nearClipPlane = .01f; camera.farClipPlane = 10;
                camera.clearFlags = CameraClearFlags.SolidColor; camera.backgroundColor = new Color(.11f, .14f, .15f);
                texture = new RenderTexture(600, 600, 24); texture.Create(); camera.targetTexture = texture;
                var prior = RenderTexture.active;
                try { RenderTexture.active = texture; GL.Clear(true, true, camera.backgroundColor); }
                finally { RenderTexture.active = prior; }
                var light = cameraObject.AddComponent<Light>(); light.type = LightType.Point; light.range = 12; light.intensity = 3; light.cullingMask = 1 << 30;
                Apply(pose);
            }
            catch { Dispose(); throw; }
        }
        Transform Snapshot(Transform source, Transform parent, bool top)
        {
            var go = new GameObject(source.name) { layer = 30 }; var target = go.transform; target.SetParent(parent, false);
            target.localPosition = top ? Vector3.zero : source.localPosition;
            target.localRotation = top ? Quaternion.identity : source.localRotation;
            target.localScale = top ? Vector3.one : source.localScale;
            var skin = source.GetComponent<SkinnedMeshRenderer>();
            var filter = source.GetComponent<MeshFilter>(); var meshRenderer = source.GetComponent<MeshRenderer>();
            Renderer copied = null;
            if (skin && skin.sharedMesh)
            {
                var baked = new Mesh(); ownedMeshes.Add(baked); skin.BakeMesh(baked);
                go.AddComponent<MeshFilter>().sharedMesh = baked;
                copied = go.AddComponent<MeshRenderer>(); copied.sharedMaterials = skin.sharedMaterials; copied.enabled = skin.enabled;
                CopyProperties(skin, copied);
            }
            else if (filter && filter.sharedMesh && meshRenderer)
            {
                go.AddComponent<MeshFilter>().sharedMesh = filter.sharedMesh;
                copied = go.AddComponent<MeshRenderer>(); copied.sharedMaterials = meshRenderer.sharedMaterials; copied.enabled = meshRenderer.enabled;
                CopyProperties(meshRenderer, copied);
            }
            var line = source.GetComponent<LineRenderer>();
            if (line)
            {
                var clone = go.AddComponent<LineRenderer>(); clone.useWorldSpace = false; clone.positionCount = line.positionCount;
                var points = new Vector3[line.positionCount]; line.GetPositions(points);
                if (line.useWorldSpace) for (int i = 0; i < points.Length; i++) points[i] = source.InverseTransformPoint(points[i]);
                clone.SetPositions(points); clone.widthMultiplier = line.widthMultiplier; clone.widthCurve = line.widthCurve;
                clone.colorGradient = line.colorGradient; clone.loop = line.loop; clone.alignment = line.alignment;
                clone.textureMode = line.textureMode; clone.sharedMaterials = line.sharedMaterials; clone.enabled = line.enabled;
                CopyProperties(line, clone);
            }
            foreach (Transform child in source) Snapshot(child, target, false);
            if (!top) go.SetActive(source.gameObject.activeSelf);
            return target;
        }
        static void CopyProperties(Renderer source, Renderer target)
        {
            target.shadowCastingMode = ShadowCastingMode.Off; target.receiveShadows = false;
            var block = new MaterialPropertyBlock(); source.GetPropertyBlock(block); target.SetPropertyBlock(block);
        }
        internal void Apply(EvidenceViewPose pose)
        {
            Pose = pose; camera.transform.position = anchor + Quaternion.Euler(pose.Pitch, pose.Yaw, 0) * new Vector3(0, 0, pose.Distance);
            camera.transform.LookAt(anchor);
        }
        internal void SetActive(bool active) { if (root) root.SetActive(active); }
        public void Dispose()
        {
            if (root) { root.SetActive(false); UnityEngine.Object.Destroy(root); root = null; }
            if (texture) { texture.Release(); UnityEngine.Object.Destroy(texture); texture = null; }
            foreach (var mesh in ownedMeshes) if (mesh) UnityEngine.Object.Destroy(mesh);
            ownedMeshes.Clear();
        }
    }
    public sealed partial class InvestigationSession
    {
        sealed class ComparisonSlot { internal string Id = "", Diagnostic = ""; internal ComparisonPreview Preview; }
        readonly ComparisonSlot[] comparisonSlots = { new ComparisonSlot(), new ComparisonSlot() };
        bool comparisonNotifying;
        public event Action<ComparisonSide, ComparisonViewState> ComparisonViewChanged;
        static bool ValidSide(ComparisonSide side) => side == ComparisonSide.Left || side == ComparisonSide.Right;
        public ComparisonViewState GetComparisonView(ComparisonSide side)
        {
            if (!ValidSide(side)) return new ComparisonViewState("", "", "Bên đối chiếu không hợp lệ.", EvidenceViewMath.Default(""), null, ComparisonAvailability.Empty);
            var slot = comparisonSlots[(int)side];
            var availability = Page != "compare" ? ComparisonAvailability.WrongContext : slot.Id == "" ? ComparisonAvailability.Empty :
                Paused ? ComparisonAvailability.Paused : slot.Preview == null ? ComparisonAvailability.MissingVisual : ComparisonAvailability.Ready;
            return new ComparisonViewState(slot.Id, Page == "compare" && slot.Id != "" ? EvidenceTranscript(slot.Id) : "", slot.Diagnostic,
                slot.Preview != null ? slot.Preview.Pose : EvidenceViewMath.Default(slot.Id), Page == "compare" ? slot.Preview?.Texture : null, availability);
        }
        bool ComparisonCommand(ComparisonSide side, out string error)
        {
            if (!ValidSide(side)) { error = "Bên đối chiếu không hợp lệ."; return false; }
            if (Page != "compare") { error = "Hãy mở trang đối chiếu trước."; return false; }
            if (Paused) { error = "Hãy tiếp tục trước khi đổi mục hoặc góc đối chiếu."; return false; }
            error = ""; return true;
        }
        public bool TrySelectComparison(ComparisonSide side, string id, out string error) => TrySelectComparison(side, id, null, out error);
        public bool TrySelectComparison(ComparisonSide side, string id, Transform source, out string error)
        {
            if (!ComparisonCommand(side, out error)) return false;
            if (string.IsNullOrEmpty(id) || InvestigationContent.Get(id) == null || !Has(id)) { error = "Vật chứng chưa có trong hồ sơ."; return false; }
            var slot = comparisonSlots[(int)side]; if (slot.Id == id) return true;
            if (!source)
            {
                var item = FindObjectsByType<InvestigationInteractable>(FindObjectsInactive.Exclude, FindObjectsSortMode.None).FirstOrDefault(x => x.evidenceId == id);
                if (item) source = item.inspectionPrefab ? item.inspectionPrefab.transform : item.transform;
                if (id == "mechanism" && gate) source = gate.transform;
            }
            ComparisonPreview preview = null; string diagnostic = "";
            if (source)
                try { preview = new ComparisonPreview(source, EvidenceViewMath.Default(id)); }
                catch (Exception e) { diagnostic = "Chưa dựng được hình xem; phần mô tả vẫn dùng được."; Debug.LogWarning("Comparison visual unavailable: " + e.GetType().Name); }
            else diagnostic = "Chưa có mô hình; dùng mô tả để đối chiếu.";
            slot.Preview?.Dispose(); slot.Id = id; slot.Preview = preview; slot.Diagnostic = diagnostic;
            PublishComparison(side); return true;
        }
        public bool TryAdjustComparison(ComparisonSide side, float yawDegrees, float pitchDegrees, float distanceDelta, out string error)
        {
            if (!ComparisonCommand(side, out error)) return false;
            var slot = comparisonSlots[(int)side];
            if (slot.Preview == null) { error = slot.Id == "" ? "Chưa chọn vật chứng ở bên này." : slot.Diagnostic; return false; }
            if (!EvidenceViewMath.Offset(slot.Preview.Pose, yawDegrees, pitchDegrees, distanceDelta, out var next)) { error = "Góc và khoảng cách phải là số hữu hạn."; return false; }
            if (!EvidenceViewMath.Same(next, slot.Preview.Pose)) { slot.Preview.Apply(next); PublishComparison(side); }
            return true;
        }
        public bool TryResetComparison(ComparisonSide side, out string error)
        {
            if (!ComparisonCommand(side, out error)) return false;
            var slot = comparisonSlots[(int)side];
            if (slot.Preview == null) { error = slot.Id == "" ? "Chưa chọn vật chứng ở bên này." : slot.Diagnostic; return false; }
            var pose = EvidenceViewMath.Default(slot.Id);
            if (!EvidenceViewMath.Same(pose, slot.Preview.Pose)) { slot.Preview.Apply(pose); PublishComparison(side); }
            return true;
        }
        public bool TryClearComparison(ComparisonSide side, out string error)
        {
            if (!ValidSide(side)) { error = "Bên đối chiếu không hợp lệ."; return false; }
            error = ""; var slot = comparisonSlots[(int)side];
            if (slot.Id == "" && slot.Preview == null) return true;
            slot.Preview?.Dispose(); slot.Preview = null; slot.Id = ""; slot.Diagnostic = ""; PublishComparison(side); return true;
        }
        void ClearAllComparison() { TryClearComparison(ComparisonSide.Left, out _); TryClearComparison(ComparisonSide.Right, out _); }
        void SyncComparisonContext()
        {
            if (Page != "compare") { ClearAllComparison(); return; }
            foreach (var slot in comparisonSlots) slot.Preview?.SetActive(!Paused);
            PublishComparison(ComparisonSide.Left); PublishComparison(ComparisonSide.Right);
        }
        void PublishComparison(ComparisonSide side)
        {
            if (comparisonNotifying || ComparisonViewChanged == null) return;
            comparisonNotifying = true;
            try
            {
                var state = GetComparisonView(side);
                foreach (Action<ComparisonSide, ComparisonViewState> listener in ComparisonViewChanged.GetInvocationList())
                    try { listener(side, state); } catch (Exception e) { Debug.LogWarning("Comparison listener failed: " + e.GetType().Name); }
            }
            finally { comparisonNotifying = false; }
        }
    }
}
