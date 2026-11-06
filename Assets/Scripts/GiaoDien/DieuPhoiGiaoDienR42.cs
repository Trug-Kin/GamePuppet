// Phân xử giao diện hành trình; giữ thứ tự với menu, pause và các màn đọc.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: DieuPhoiGiaoDienR42, ChuanBiHanhLyR41, TuongTacNguoiChoi, CanvasGroup, Image, ContentSizeFitter.
// Thứ tự chạy khai báo: 29000; giữ quan hệ khởi tạo/pose với các owner khác.
using UnityEngine;
using UnityEngine.UI;

// One presentation arbiter; never restores a cached target or changes saved state.
// Producers retain their own visibility. CanvasGroups suppress lower priority content.
[DefaultExecutionOrder(29000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyUiR42")]
public sealed class DieuPhoiGiaoDienR42 : MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 // Opt-in on r45 only. HUD is composited after world cameras and atmosphere;
 // it must not participate in world depth or be copied into lake reflections.
 public bool presentationR45;
 public static bool OverlayFor(DieuPhoiHanhTrinh owner)=>owner&&owner.GetComponent<DieuPhoiGiaoDienR42>()&&owner.GetComponent<DieuPhoiGiaoDienR42>().presentationR45;
 public static void ConfigureCanvas(DieuPhoiHanhTrinh owner,Canvas canvas)
 {
  if(!OverlayFor(owner))return;
  canvas.renderMode=RenderMode.ScreenSpaceOverlay;
  canvas.worldCamera=null;
  // The legacy workaround writes near depth. Overlay UI uses the UI shader
  // after the world passes, including planar reflection requests and mist.
  foreach(var graphic in canvas.GetComponentsInChildren<Graphic>(true))
   if(graphic.material&&graphic.material.shader&&graphic.material.shader.name=="ConRoi/R23/ReadableUI")graphic.material=null;
 }
 public string Mode {get;private set;}
 bool wasReading;
 void LateUpdate()
 {
  var p=story.polish;var a=GetComponent<ChuanBiHanhLyR41>();
  bool ending=!string.IsNullOrEmpty(story.data.ending),memory=a&&a.MemoryOpen;
  bool blocked=story.MenuOpen||story.Loading||story.Paused;
  bool reading=p.IsReading||story.SupplementInspection;
  var interaction=story.player.GetComponent<TuongTacNguoiChoi>();
  bool prompt=interaction.enabled&&interaction.Target!=null;
  Mode=story.Loading?"transition":story.MenuOpen?"menu":story.Paused?"pause":memory?"memory":ending?"ending":reading?"inspection":story.PhoneOpen?"phone":p.DialogueVisible?"dialogue":"gameplay";
  if(wasReading!=p.IsReading){wasReading=p.IsReading;story.RefreshControls();}
  bool gameplay=!blocked&&!ending&&!memory&&!reading&&!story.PhoneOpen&&!story.DreamHub;
  foreach(var c in GetComponentsInChildren<Canvas>(true))
  {
   ConfigureCanvas(story,c);
   foreach(Transform child in c.transform)
   {
    bool show=true;string n=child.name;
    if(c.name=="V40_Presentation")
    {
     show=!blocked;
     if(n=="Controls")show=false; // detailed help remains in the pause menu
     if(n=="Chapter")show=gameplay&&!p.DialogueVisible&&!prompt;
     if(n=="Dialogue")show=!blocked&&!memory&&!reading&&!story.PhoneOpen;
     if(n=="Document")show=!blocked&&p.IsReading;
     if(n=="Phone")show=!blocked&&!ending&&!reading&&story.PhoneOpen;
     if(n=="Wake_Fade")show=!story.MenuOpen&&!story.Paused;
     if(n=="Phone")Area(child,.51f,.08f,.97f,.94f);
     if(n=="Document")Area(child,.10f,.12f,.90f,.88f);
     if(n=="Dialogue")Area(child,ending?.10f:.12f,ending?.10f:.04f,ending?.90f:.88f,ending?.48f:.25f);
    }
    else if(c.name=="R23_ContextHint"||c.name=="V48_Objective"||c.name=="R22_ContextHint")
    {show=gameplay&&!p.DialogueVisible&&!prompt&&!story.data.finalStarted;Area(child,.02f,.05f,.70f,.23f);}
    else if(c.name=="R35_ParcelContext")
    {show=gameplay&&!p.DialogueVisible&&!prompt;Area(child,.02f,.04f,.70f,.19f);}
    else if(c.name=="R37_NotebookContext")show=gameplay&&!p.DialogueVisible&&!prompt;
    else if(c.name=="R41_PreparationContext")
    {
     show=!blocked&&(ending?memory||n=="Held_context":gameplay&&!p.DialogueVisible&&!prompt);
     if(n=="Remembered_drawings")Area(child,.03f,.08f,.48f,.90f);
     if(n=="Existing_phone_readonly")Area(child,.52f,.08f,.97f,.90f);
     if(n=="Held_context")Area(child,.20f,.02f,.80f,.08f);
    }
    else if(c.name=="R32_Phone_And_Inspection")
     show=!blocked&&!ending&&(reading||story.PhoneOpen||gameplay&&!p.DialogueVisible&&!prompt);
    var group=child.GetComponent<CanvasGroup>();if(!group)group=child.gameObject.AddComponent<CanvasGroup>();
    if(presentationR45&&n!="Wake_Fade")
    {
     var background=child.GetComponent<Image>();
     if(background&&background.color.a>.1f){var color=background.color;color.a=1; background.color=color;}
    }
    group.alpha=show?1:0;group.interactable=show;group.blocksRaycasts=show;
    foreach(var t in child.GetComponentsInChildren<Text>(true))
    {if(t.GetComponent<ContentSizeFitter>())continue;t.resizeTextForBestFit=true;t.resizeTextMinSize=16;t.resizeTextMaxSize=Mathf.Max(18,t.fontSize);t.verticalOverflow=VerticalWrapMode.Truncate;}
   }
  }
 }
 static void Area(Transform t,float left,float bottom,float right,float top)
 {var r=t as RectTransform;if(!r)return;r.anchorMin=new Vector2(left,bottom);r.anchorMax=new Vector2(right,top);r.pivot=new Vector2(.5f,.5f);r.offsetMin=r.offsetMax=Vector2.zero;}
}
