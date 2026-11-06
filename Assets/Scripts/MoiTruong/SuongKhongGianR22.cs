// Mật độ sương neo thế giới với corridor trong tọa độ map.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Thứ tự chạy khai báo: 27000; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;

// World-anchored density; only present in the scenery/experience derivative.
[DefaultExecutionOrder(27000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "SpatialMistR22")]
public sealed class SuongKhongGianR22 : MonoBehaviour
{
    public DieuPhoiHanhTrinh story;
    public bool clearReview;
    public bool debugDepth;
    public Vector4[] corridors = new Vector4[0]; // xz endpoints, fixed in map space
    public float clearHalfWidth=14, transitionWidth=35;
    public float coreDensity=.0017f, outerDensity=.029f;
    public static SuongKhongGianR22 Active {get;private set;}
    public bool Applies => enabled && story && story.data.region>=0 && story.data.region<2 && !clearReview;
    void OnEnable(){Active=this;}
    void OnDisable(){if(Active==this)Active=null;}
    void LateUpdate()
    {
        var region=story.data.region;
        RenderSettings.fog=region>=2&&!clearReview;
        if(region>=2){RenderSettings.fogMode=FogMode.ExponentialSquared;RenderSettings.fogDensity=region==2?.012f:.013f;}
    }
    public void Configure(Material material)
    {
        bool night=story.data.region==1;
        material.SetVectorArray("_MistCorridors",corridors.Length>0?corridors:new[]{Vector4.zero});
        material.SetInt("_MistCorridorCount",Mathf.Min(corridors.Length,24));
        material.SetInt("_MistDebug",debugDepth?1:0);
        material.SetVector("_MistProfile",new Vector4(clearHalfWidth,transitionWidth,coreDensity,outerDensity));
        material.SetColor("_MistColor",night?new Color(.055f,.073f,.095f):new Color(.31f,.385f,.41f));
    }
}
