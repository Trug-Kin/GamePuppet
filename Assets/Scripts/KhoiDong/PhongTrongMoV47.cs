// Đoạn mơ ngắn trong phòng quen thuộc; không thêm mục tiêu hoặc điểm nhận thức.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, brokenStrings, repairedStrings.
using System;
using UnityEngine;

// GDD 4.6: a brief dream version of the familiar room during falling asleep.
// No extra interaction, objective, awareness reward or alternative region order.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "DreamRoomV47")]
public sealed class PhongTrongMoV47:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 public Renderer[] brokenStrings,repairedStrings;
 [Serializable] sealed class RoomState{public bool dreaming;public float elapsed;}
 RoomState state=new RoomState();
 public bool DreamHub=>state.dreaming;
 public float Elapsed=>state.elapsed;
 public void Begin(){state=new RoomState{dreaming=true};}
 public bool Advance(){state.elapsed+=Time.deltaTime;return state.elapsed>=3;}
 public void End(){state=new RoomState();}
 public string Capture()=>JsonUtility.ToJson(state);
 public static bool HasDream(string json)=>!string.IsNullOrEmpty(json)&&JsonUtility.FromJson<RoomState>(json).dreaming;
 public static bool Valid(string json)
 {
  if(string.IsNullOrEmpty(json))return true;
  try{var s=JsonUtility.FromJson<RoomState>(json);return s!=null&&!float.IsNaN(s.elapsed)&&!float.IsInfinity(s.elapsed)&&s.elapsed>=0&&s.elapsed<=3.5f;}
  catch(ArgumentException){return false;}
 }
 public void Restore(string json){state=string.IsNullOrEmpty(json)?new RoomState():JsonUtility.FromJson<RoomState>(json);}
 public void Apply()
 {
  foreach(var r in brokenStrings)r.enabled=!DreamHub;
  foreach(var r in repairedStrings)r.enabled=DreamHub;
 }
}
