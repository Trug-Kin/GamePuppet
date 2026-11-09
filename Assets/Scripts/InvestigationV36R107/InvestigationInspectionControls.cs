using System;
using UnityEngine;

namespace ConRoi.Investigation.V36.R107
{
    public readonly struct EvidenceViewPose
    {
        public readonly float Yaw, Pitch, Distance;
        internal EvidenceViewPose(float yaw, float pitch, float distance) { Yaw = yaw; Pitch = pitch; Distance = distance; }
    }
    internal static class EvidenceViewMath
    {
        internal static EvidenceViewPose Default(string id) => new EvidenceViewPose(25,
            id == "shoe" || id == "shoecontrol" ? 65 : id == "slide" || id == "shoeprint" || id == "fabric" || id == "coat" ? -65 : 10,
            id == "slide" || id == "shoeprint" ? 1.65f : 2);
        internal static bool Offset(EvidenceViewPose pose, float yaw, float pitch, float distance, out EvidenceViewPose next)
        {
            next = pose;
            if (float.IsNaN(yaw) || float.IsInfinity(yaw) || float.IsNaN(pitch) || float.IsInfinity(pitch) || float.IsNaN(distance) || float.IsInfinity(distance)) return false;
            double angle = ((double)pose.Yaw + yaw) % 360; if (angle < 0) angle += 360;
            next = new EvidenceViewPose((float)angle, Mathf.Clamp(pose.Pitch + pitch, -80, 80), Mathf.Clamp(pose.Distance + distance, .5f, 4));
            return true;
        }
        internal static bool Same(EvidenceViewPose a, EvidenceViewPose b) => a.Yaw == b.Yaw && a.Pitch == b.Pitch && a.Distance == b.Distance;
    }
    public enum InspectionAvailability { Ready, WrongContext, NoEvidence, MissingVisual, Paused }
    public readonly struct InspectionViewState
    {
        public readonly string EvidenceId;
        public readonly EvidenceViewPose Pose;
        public readonly InspectionAvailability Availability;
        public readonly Texture Preview;
        public bool CanControl => Availability == InspectionAvailability.Ready;
        internal InspectionViewState(string id, EvidenceViewPose pose, InspectionAvailability availability, Texture preview)
        { EvidenceId = id; Pose = pose; Availability = availability; Preview = preview; }
    }
    public sealed partial class InvestigationSession
    {
        public event Action<InspectionViewState> InspectionViewChanged;
        bool inspectionNotifying;
        public InspectionViewState InspectionView
        {
            get
            {
                var availability = Page != "inspect" ? InspectionAvailability.WrongContext : !Has(inspectId) ? InspectionAvailability.NoEvidence :
                    !viewer || !viewerCamera ? InspectionAvailability.MissingVisual : Paused ? InspectionAvailability.Paused : InspectionAvailability.Ready;
                return new InspectionViewState(inspectId, new EvidenceViewPose(viewYaw, viewPitch, viewDistance), availability,
                    Page == "inspect" && viewer && viewerCamera ? viewerTexture : null);
            }
        }
        bool CanChangeInspection(out string error)
        {
            switch (InspectionView.Availability)
            {
                case InspectionAvailability.Ready: error = ""; return true;
                case InspectionAvailability.Paused: error = "Hãy tiếp tục trước khi đổi góc xem."; return false;
                case InspectionAvailability.MissingVisual: error = "Chưa có mô hình để xem; phần mô tả vẫn dùng được."; return false;
                case InspectionAvailability.NoEvidence: error = "Chưa mở vật chứng hợp lệ."; return false;
                default: error = "Hãy mở trang xem vật trước khi đổi góc."; return false;
            }
        }
        public bool TryAdjustInspection(float yawDegrees, float pitchDegrees, float distanceDelta, out string error)
        {
            if (!CanChangeInspection(out error)) return false;
            if (!EvidenceViewMath.Offset(InspectionView.Pose, yawDegrees, pitchDegrees, distanceDelta, out var next))
            { error = "Góc và khoảng cách phải là số hữu hạn."; return false; }
            if (EvidenceViewMath.Same(next, InspectionView.Pose)) return true;
            viewYaw = next.Yaw; viewPitch = next.Pitch; viewDistance = next.Distance;
            UpdateViewer(); PublishInspectionView(); return true;
        }
        public bool TryResetInspection(out string error)
        {
            if (!CanChangeInspection(out error)) return false;
            if (!EvidenceViewMath.Same(InspectionView.Pose, EvidenceViewMath.Default(inspectId))) ResetInspectionPose();
            return true;
        }
        void ResetInspectionPose()
        {
            var pose = EvidenceViewMath.Default(inspectId); viewYaw = pose.Yaw; viewPitch = pose.Pitch; viewDistance = pose.Distance;
            UpdateViewer(); PublishInspectionView();
        }
        void ClearInspectionVisual()
        {
            if (viewer) Destroy(viewer.gameObject); viewer = null;
        }
        void PublishInspectionView()
        {
            if (inspectionNotifying || InspectionViewChanged == null) return;
            inspectionNotifying = true;
            try
            {
                var state = InspectionView;
                foreach (Action<InspectionViewState> listener in InspectionViewChanged.GetInvocationList())
                    try { listener(state); } catch (Exception e) { Debug.LogWarning("Inspection listener failed: " + e.GetType().Name); }
            }
            finally { inspectionNotifying = false; }
        }
    }
}
