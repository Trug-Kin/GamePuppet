// Quản lý hai bài giấy lớp 203, cầm/đặt/đối chiếu và memory; không cộng hai trục ending.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Code tự tìm/tạo: TraiNghiemHanhTrinhR23, TuongTacNguoiChoi, MeshFilter, MeshRenderer, RawImage.
// Memory giữ tên field/schema; restore phối hợp owner State và bộ đếm SuccessfulLoads.
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

// Opt-in derivative. Reuses the Blender r32 meshes without editing vertex data.
public sealed partial class TuongTacGiayLop203R32 {
 InputAction takeR34,compareR34;
 bool quickCompare;
 GameObject placementGhost;
 Material ghostMaterial;
 public bool PlacementVisible=>placementGhost&&placementGhost.activeSelf;
 public string JourneyObjectiveR44=>story.player.transform.position.y<2.8f?"Theo cầu thang trái tới lớp 203. Có thể hỏi Mai về lớp học cũ.":"Có thể hỏi Mai cạnh bảng về lớp học cũ. Biển TỈNH DẬY ở cửa lớp dẫn về phòng; nhìn gần và E khi muốn về.";
 public string ResponseR34=>GetComponent<TraiNghiemHanhTrinhR23>() is TraiNghiemHanhTrinhR23 journey&&journey.journeyR44?journey.SchoolReplyR44:memory.choice==1?
  "Cô Mai: Bản này theo đúng dàn ý. Câu em muốn thử một công việc khác không còn ở đây.":memory.choice==2?
  "Cô Mai: Em vẫn giữ đoạn muốn thử một tháng. Trong khay chưa có câu trả lời ấy.":memory.choice==3?
  "Cô Mai: Hôm nay em để bài lại đây. Dòng cô gạch vẫn còn, chưa cần viết tiếp lúc này.":
  "Cô Mai: Cô để lại bài có tên em. Có một chỗ cô đã gạch; em có thể xem lại.";
 void InitR34(){takeR34=Key("Take paper","<Keyboard>/e");compareR34=Key("Compare papers","<Keyboard>/c");}
 void DisposeR34(){takeR34?.Dispose();compareR34?.Dispose();if(placementGhost)Destroy(placementGhost);if(ghostMaterial)Destroy(ghostMaterial);}
 void ResolveR34(){
  memory.choice=memory.places[1]==2?1:memory.places[0]==3||memory.choice==2&&memory.places[0]==1?2:memory.choice==3?3:0;
 }
 public void LeaveR34(){
  ResolveR34();if(memory.choice==0)memory.choice=3;
  for(int i=0;i<2;i++)if(memory.places[i]==1)memory.places[i]=i==0&&memory.choice==2?3:0;
  memory.abandoned=memory.choice==3;selected=-1;Close();
 }
 public string MaiTalkR34(){
  ResolveR34();
  if(memory.choice==0&&memory.viewed==0&&selected<0)return ResponseR34;
  if(memory.choice==0)memory.choice=memory.places[0]==1?2:3;
  Capture();return MaiResponse;
 }
 void UpdateR34(){
  if(!Active||story.Paused||story.PhoneOpen)return;
  if(Inspecting){
   if(compareR34.WasPressedThisFrame()&&memory.viewed==3)quickCompare=!quickCompare;
   if(!quickCompare&&takeR34.WasPressedThisFrame()&&Time.unscaledTime-opened>.18f)TakeR34(view);
  }else{
   if(returnItem.WasPressedThisFrame())ReturnR34();

  }
 }
 public void TakeR34(int id){
  if(!Active||story.Paused||story.PhoneOpen||story.MenuOpen||story.Loading||id<0||id>1)return;
  memory.places[id]=1;selected=id;if(Inspecting)Close();ApplyPapers();Capture();
  story.PlayEffectAt(story.polish.pageTurn,home[id]);story.NotifyInteraction();
 }
 void ReturnR34(){
  int id=Inspecting?view:selected;if(id<0)return;
  memory.places[id]=0;if(Inspecting)Close();selected=memory.places[1-id]==1?1-id:-1;
  ApplyPapers();Capture();story.PlayEffectAt(story.polish.pageTurn,home[id]);
 }
 void PlacementPreviewR34(){
  bool show=Active&&!Inspecting&&!story.Paused&&!story.PhoneOpen&&!story.MenuOpen&&!story.Loading&&selected>=0&&memory.places[selected]==1;
  var target=show?story.player.GetComponent<TuongTacNguoiChoi>().Target as DichTuongTacGiayR32:null;
  show=show&&target&&target.activity==this&&target.paper<0&&(!target.inTray||selected==1);
  if(!placementGhost&&show){
   placementGhost=new GameObject("R34_PlacementPreview_ExistingBlenderMesh");
   placementGhost.AddComponent<MeshFilter>();placementGhost.AddComponent<MeshRenderer>();
   ghostMaterial=new Material(Shader.Find("Universal Render Pipeline/Unlit"));
   ghostMaterial.SetFloat("_Surface",1);ghostMaterial.SetFloat("_ZWrite",0);
   ghostMaterial.SetFloat("_SrcBlend",(float)UnityEngine.Rendering.BlendMode.SrcAlpha);
   ghostMaterial.SetFloat("_DstBlend",(float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
   ghostMaterial.EnableKeyword("_SURFACE_TYPE_TRANSPARENT");ghostMaterial.renderQueue=3000;
   ghostMaterial.SetColor("_BaseColor",new Color(.68f,.83f,.76f,.38f));
   placementGhost.GetComponent<MeshRenderer>().sharedMaterial=ghostMaterial;
  }
  if(!placementGhost)return;placementGhost.SetActive(show);if(!show)return;
  placementGhost.GetComponent<MeshFilter>().sharedMesh=papers[selected].GetComponent<MeshFilter>().sharedMesh;
  placementGhost.transform.SetPositionAndRotation((target.inTray?tray.position:outside.position+Vector3.right*(selected*.24f))+Vector3.up*.002f,rotations[selected]);
  placementGhost.transform.localScale=scales[selected];
 }
 void ComparePanelR34(){
  Panel("Compare",20,90,1240,610);Label("HAI BÀI ĐÃ XEM",55,110,1130,45,28);
  Label(Contents[0],55,170,540,370,21);Label(Contents[1],665,170,540,370,21);
  Button(memory.related?"Gỡ đánh dấu liên quan":"Đánh dấu: Có vẻ liên quan",55,595,540,48,Relate);
  Button("Trở lại bài đang xem [C / Tab]",665,595,540,48,()=>quickCompare=false);
 }
 void HeldPanelR34(){
  if(!Active||selected<0||memory.places[selected]!=1)return;
  Panel("Held paper",995,365,260,260);
  var picture=Box("Held actual page",1010,378,85,120).AddComponent<RawImage>();picture.texture=pages[selected];picture.raycastTarget=false;
  Label(Names[selected],1104,385,140,90,19);
  Label("Đang cầm"+(memory.places[0]==1&&memory.places[1]==1?" · Q đổi giấy":"")+"\nTab · Xem lại\nR · Đặt lại bàn",1010,515,230,100,18);
 }
}
