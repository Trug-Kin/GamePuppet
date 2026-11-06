// Quản lý hai bài giấy lớp 203, cầm/đặt/đối chiếu và memory; không cộng hai trục ending.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Code tự tìm/tạo: Canvas, CanvasScaler, TraiNghiemHanhTrinhR23, ChuanBiHanhLyR41, RawImage, HuongDanHanhTrinhV48, RectMask2D, Image.
// Memory giữ tên field/schema; restore phối hợp owner State và bộ đếm SuccessfulLoads.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public sealed partial class TuongTacGiayLop203R32 {
 Canvas paperUI;Transform uiRoot;string uiSignature="";
 void RefreshUI(){
  if(!paperUI){var g=new GameObject("R32_Phone_And_Inspection",typeof(Canvas),typeof(CanvasScaler),typeof(GraphicRaycaster));g.transform.SetParent(transform,false);paperUI=g.GetComponent<Canvas>();paperUI.renderMode=RenderMode.ScreenSpaceCamera;paperUI.worldCamera=eye;paperUI.planeDistance=.31f;paperUI.sortingOrder=240;var scaler=g.GetComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;scaler.referenceResolution=new Vector2(1280,720);uiRoot=g.transform;}
  DieuPhoiGiaoDienR42.ConfigureCanvas(story,paperUI);
  paperUI.enabled=!story.MenuOpen&&!story.Loading&&!story.Paused&&string.IsNullOrEmpty(story.data.ending);
  var journey=GetComponent<TraiNghiemHanhTrinhR23>();bool connected=journey&&journey.journeyR44;
  string sig=$"{view}|{quickCompare}|{story.PhoneOpen}|{phoneNotes}|{compare}|{memory.viewed}|{memory.related}|{Active}|{selected}|{memory.choice}|{memory.places[0]}|{memory.places[1]}|{story.data.schoolTrace}|{memory.puppetSeen}";
  if(connected&&story.PhoneOpen){var known=GetComponent<ChuanBiHanhLyR41>();sig+="|"+story.SuccessfulLoads+"|"+journey.Objective+"|"+journey.HeldR44+"|"+journey.LocalThreadR44+"|"+(phoneNotes&&known?known.RememberedNotes:"");}
  if(sig==uiSignature)return;uiSignature=sig;
  foreach(Transform child in uiRoot)Destroy(child.gameObject);
  if(completeR34&&Inspecting&&quickCompare){ComparePanelR34();}
  else if(Inspecting){
   Panel("Inspect",20,90,1240,610);
   var image=Box("Paper preview",40,130,390,488);image.AddComponent<RawImage>().texture=previewImage;
   Label(Names[view],465,113,730,45,28);Label(Contents[view],465,173,720,370,22);
   Label("Kéo chuột trái · Xoay | Cuộn · Phóng nhẹ",45,completeR34?660:635,1170,40,18);
   if(completeR34){
    Button("Cầm bài / về lớp [E]",465,555,345,45,()=>TakeR34(view));
    Button("Đóng xem [Tab]",835,555,345,45,Close);
    Button("Đặt lại bàn [R]",465,608,345,43,Return);
    if(memory.viewed==3)Button("Đối chiếu hai bài [C]",835,608,345,43,()=>quickCompare=true);
   }else{Button("Đặt lại bàn [R]",480,570,320,48,Return);Button("Cất / trở lại lớp [Tab]",825,570,320,48,Close);}
  }else if(story.PhoneOpen){
   Panel("Phone notebook",30,95,580,610);
   Button(phoneNotes?"Việc trước mắt / vật đang giữ [N]":"Ghi nhớ / đối chiếu [N]",50,112,540,42,()=>phoneNotes=!phoneNotes);
   if(compare&&memory.viewed==3){
    Panel("Compare",35,90,1210,610);Label("HAI GHI CHÚ ĐÃ XEM",65,115,1100,45,26);
    Label(Contents[0],65,180,535,385,21);Label(Contents[1],665,180,535,385,21);
    Button(memory.related?"Gỡ đánh dấu liên quan":"Đánh dấu: Có vẻ liên quan",65,605,535,50,Relate);
    Button("Trở lại sổ",665,605,535,50,()=>compare=false);
   }else if(!phoneNotes){
    Label("VIỆC TRƯỚC MẮT\n"+(connected?journey.Objective:Active?Objective:story.GetComponent<HuongDanHanhTrinhV48>().Objective),52,173,530,connected?145:165);
    Label("VẬT ĐANG GIỮ\n"+(connected?journey.HeldR44:Active?HeldNames:"Không giữ vật trong mơ"),52,connected?330:350,530,connected?90:95);
    Label(connected?journey.LocalThreadR44:Active?ChoiceText:"N · Ghi nhớ / đối chiếu",52,connected?430:455,530,connected?115:80,18);
    if(Active)for(int i=0;i<2;i++){int id=i;if(memory.places[id]==1)Button("Xem lại · "+Names[id],52,552+i*50,530,42,()=>{story.ClosePhone();Inspect(id);});}
    if(connected)Label("P · Cất điện thoại để thao tác vật",52,658,530,30,18);
   }else{
    var notes=GetComponent<ChuanBiHanhLyR41>();ScrollNotes("ĐIỀU ĐÃ THẤY\n"+(notes?notes.RememberedNotes:"")+(memory.puppetSeen?"\nTôi đã thấy rối đẩy tờ giấy của nó vào khay.":""));
    if(memory.viewed==3)Button("Xem hai ghi chú cạnh nhau",52,500,530,48,()=>compare=true);
    Label("N · Đổi trang | P · Cất điện thoại",52,637,530,45,18);
   }
  }else{
   compare=false;
   if(completeR34)HeldPanelR34();
   else if(Active&&selected>=0){Panel("Held",20,650,1230,48);Label("Đang giữ: "+HeldNames+" · Tab xem · Q đổi giấy · E tại khay / mép bàn để đặt",30,656,1200,38,18);}
  }
 }
 void ScrollNotes(string value){
  var viewport=Box("Memory scroll",52,173,530,320);viewport.AddComponent<RectMask2D>();var image=viewport.AddComponent<Image>();image.color=Color.clear;
  var scroll=viewport.AddComponent<ScrollRect>();scroll.horizontal=false;scroll.movementType=ScrollRect.MovementType.Clamped;scroll.scrollSensitivity=30;
  var g=new GameObject("Known notes",typeof(RectTransform),typeof(Text),typeof(ContentSizeFitter));g.transform.SetParent(viewport.transform,false);
  var r=g.GetComponent<RectTransform>();r.anchorMin=new Vector2(0,1);r.anchorMax=Vector2.one;r.pivot=new Vector2(0,1);r.anchoredPosition=Vector2.zero;r.sizeDelta=Vector2.zero;
  var t=g.GetComponent<Text>();t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=20;t.color=new Color(.96f,.94f,.85f);t.text=value;t.raycastTarget=false;
  g.GetComponent<ContentSizeFitter>().verticalFit=ContentSizeFitter.FitMode.PreferredSize;scroll.viewport=viewport.GetComponent<RectTransform>();scroll.content=r;
 }
 GameObject Box(string n,float x,float y,float w,float h){var g=new GameObject(n,typeof(RectTransform));g.transform.SetParent(uiRoot,false);var r=g.GetComponent<RectTransform>();r.anchorMin=r.anchorMax=r.pivot=new Vector2(0,1);r.anchoredPosition=new Vector2(x,-y);r.sizeDelta=new Vector2(w,h);return g;}
 void Panel(string n,float x,float y,float w,float h){var i=Box(n,x,y,w,h).AddComponent<Image>();i.color=new Color(.025f,.035f,.032f,1);i.raycastTarget=true;}
 Text Label(string value,float x,float y,float w,float h,int size=21){var t=Box("Text",x,y,w,h).AddComponent<Text>();t.text=value;t.font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");t.fontSize=size;t.color=new Color(.96f,.94f,.85f);t.raycastTarget=false;return t;}
 void Button(string value,float x,float y,float w,float h,UnityAction action){var g=Box(value,x,y,w,h);var im=g.AddComponent<Image>();im.color=new Color(.18f,.24f,.22f);var b=g.AddComponent<Button>();b.targetGraphic=im;b.onClick.AddListener(action);var t=Label(value,x+12,y+8,w-24,h-10,20);t.alignment=TextAnchor.MiddleCenter;}
}
