// Cuộn chỉ trong trải nghiệm dốc R24.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): chapter.
using UnityEngine;

// Scenery adaptation only: keep the retained rolling prop on the new gutter profile.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R24SlopeSpool")]
public sealed class CuonChiDocR24:MonoBehaviour
{
 public ChuongDocV39 chapter;float xOffset,yOffset;
 static float Curve(float z)=>Mathf.Clamp01((z-18)/12)*2;
 static float Height(float z)=>Mathf.Clamp(z,0,30)*.2f;
 void Start(){var p=chapter.rollingWood.position-new Vector3(-180,0,-60);xOffset=p.x-Curve(p.z);yOffset=p.y-Height(p.z);}
 void LateUpdate(){if(chapter.story.data.region!=3)return;var p=chapter.rollingWood.position-new Vector3(-180,0,-60);p.x=Curve(p.z)+xOffset;p.y=Height(p.z)+yOffset;chapter.rollingWood.position=p+new Vector3(-180,0,-60);}
}
