// Migrate cao độ một lần sau load thành công; không chỉnh grounded/controller mỗi frame.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, previousSupports.
using UnityEngine;

// Presentation-only terrain migration. Runs once per successful load, never while walking.
// Original save bytes and all progression fields remain untouched.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "RouteGradeR73")]
public sealed class KhoiPhucCaoDoSauTaiR73 : MonoBehaviour
{
    public DieuPhoiHanhTrinh story;
    public Mesh[] previousSupports;
    public Matrix4x4[] previousWorldMatrices;
    // Optional parallel binding in the raised-dam derivative. Match the old
    // support before selecting its replacement, so an under-bridge surface
    // cannot win solely because it is nearer to the old deck elevation.
    public Collider[] preferredCurrentSupports;
    // Defaults retain r73's measured footprint. Later scene derivatives serialize
    // their own envelope and previous surfaces; no per-frame grounding changes.
    public Vector2 affectedMinXZ = new Vector2(699.99f, 297.49f);
    public Vector2 affectedMaxXZ = new Vector2(777.74f, 420.01f);
    // Load-only search span; old scenes retain the original two metres below.
    public float loadProbeBelow = 2f;
    public float loadProbeAbove = 2f;
    public int Adjustments { get; private set; }
    public string LastResult { get; private set; } = "not_loaded";
    public void ReseatAfterLoad()
    {
        if (!story) return;
        if (story.data.region < 0 || story.data.region > 1) return;
        var p = story.player.transform.position;
        // Coarse existing triangles can carry a vertex edit beyond the vertex mask.
        // This is the measured envelope of changed terrain faces, not a larger grading operation.
        if (p.z < affectedMinXZ.y || p.z > affectedMaxXZ.y || p.x < affectedMinXZ.x || p.x > affectedMaxXZ.x) return;
        var cc = story.player.controller;
        float foot = p.y + story.player.transform.TransformVector(cc.center).y - cc.height * .5f;
        bool oldSupport = false;
        Collider preferred = null; float preferredOldDistance=float.PositiveInfinity;
        bool paired=preferredCurrentSupports!=null&&preferredCurrentSupports.Length==previousSupports.Length;
        for (int m = 0; m < previousSupports.Length && (paired||!oldSupport); m++)
        {
            if (!previousSupports[m]) continue;
            var vs = previousSupports[m].vertices; var ts = previousSupports[m].triangles;
            var matrix = previousWorldMatrices[m];
            for (int i = 0; i < ts.Length; i += 3)
            {
                var a = matrix.MultiplyPoint3x4(vs[ts[i]]);
                var b = matrix.MultiplyPoint3x4(vs[ts[i + 1]]);
                var c = matrix.MultiplyPoint3x4(vs[ts[i + 2]]);
                float den = (b.z - c.z) * (a.x - c.x) + (c.x - b.x) * (a.z - c.z);
                if (Mathf.Abs(den) < 1e-7f) continue;
                float u = ((b.z - c.z) * (p.x - c.x) + (c.x - b.x) * (p.z - c.z)) / den;
                float v = ((c.z - a.z) * (p.x - c.x) + (a.x - c.x) * (p.z - c.z)) / den;
                if (u < -.0001f || v < -.0001f || u + v > 1.0001f) continue;
                float oldDistance=Mathf.Abs(foot - (u * a.y + v * b.y + (1 - u - v) * c.y));
                if (oldDistance < .25f)
                { oldSupport = true;if(paired&&preferredCurrentSupports[m]&&oldDistance<preferredOldDistance){preferred=preferredCurrentSupports[m];preferredOldDistance=oldDistance;}break; }
            }
        }
        if (!oldSupport) { LastResult = "not_near_previous_ground"; return; }
        RaycastHit best = default; float nearest = float.PositiveInfinity;
        float above = Mathf.Max(2, loadProbeAbove);
        if(preferred&&preferred.enabled&&preferred.gameObject.activeInHierarchy&&preferred.Raycast(new Ray(new Vector3(p.x,foot+above,p.z),Vector3.down),out var matched,above+Mathf.Max(2,loadProbeBelow))&&matched.normal.y>=Mathf.Cos(cc.slopeLimit*Mathf.Deg2Rad))best=matched;
        foreach (var h in Physics.RaycastAll(new Vector3(p.x, foot + above, p.z), Vector3.down, above + Mathf.Max(2, loadProbeBelow), 8, QueryTriggerInteraction.Ignore))
        {
            if(preferred&&best.collider==preferred)break;
            if (h.normal.y < Mathf.Cos(cc.slopeLimit * Mathf.Deg2Rad)) continue;
            float distance = Mathf.Abs(h.point.y - foot);
            if (distance < nearest) { best = h; nearest = distance; }
        }
        if (!best.collider) { LastResult = "no_current_support"; return; }
        float shift = best.point.y + cc.skinWidth * .25f - foot;
        if (Mathf.Abs(shift) < .04f) { LastResult = "already_on_current_ground"; return; }
        var target = p + Vector3.up * shift;
        var centre = target + story.player.transform.TransformVector(cc.center);
        var lower = centre + Vector3.down * (cc.height * .5f - cc.radius) + Vector3.up * .1f;
        var upper = centre + Vector3.up * (cc.height * .5f - cc.radius);
        foreach (var other in Physics.OverlapCapsule(lower, upper, cc.radius * .95f, 8, QueryTriggerInteraction.Ignore))
            if (!other.transform.IsChildOf(story.player.transform)) { LastResult = "placement_obstructed"; return; }
        story.Place(target, story.player.transform.rotation);
        story.data.position = target;
        Adjustments++; LastResult = "old_elevation_reseated";
    }
}
