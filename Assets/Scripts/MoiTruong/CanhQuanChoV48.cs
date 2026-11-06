// Hiện/ẩn cảnh quan chợ theo trạng thái vùng.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, scenery.
using UnityEngine;

// Shared city scenery stays present while walking between lake and market.
// Chapter actors, traces, exits and their progression remain region-specific.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "MarketSceneryV48")]
public sealed class CanhQuanChoV48:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;public GameObject[] scenery;
 int previous=-99;
 void LateUpdate()
 {
  int region=story.data.region;if(previous==region)return;previous=region;
  foreach(var g in scenery)if(g)g.SetActive(region==0||region==1);
 }
}
