// Menu vào game/tiếp tục/chơi mới; NewGame gọi Restart, Continue chỉ đóng menu sau Load thành công.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: HuongDanHanhTrinhV48.
// Thứ tự chạy khai báo: 22000; giữ quan hệ khởi tạo/pose với các owner khác.
using System;
using UnityEngine;
using UnityEngine.InputSystem;

// Player entry only; verification explicitly bypasses the menu.
[DefaultExecutionOrder(22000)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PlayerEntryV47")]
public sealed class MenuVaoGameV47:MonoBehaviour
{
 public DieuPhoiHanhTrinh story;
 bool help,confirm;string message="";
 void Start(){if(Array.IndexOf(Environment.GetCommandLineArgs(),"--verify-suite")<0)story.SetMenu(true);}
 public void NewGame(){story.SetMenu(false);story.Restart();help=confirm=false;message="";}
 public void Continue(){int count=story.SuccessfulLoads;story.Load();if(story.SuccessfulLoads>count){story.SetMenu(false);help=confirm=false;message="";}else message="Chưa có bản lưu hợp lệ. Chọn New Game để bắt đầu.";}
 void Update()
 {
  if(!story.MenuOpen&&!story.Paused)return;
  var k=Keyboard.current;if(k==null)return;
  if(k.hKey.wasPressedThisFrame)help=!help;
  if(!story.MenuOpen)return;
  if(k.enterKey.wasPressedThisFrame)NewGame();
  if(k.lKey.wasPressedThisFrame)Continue();
 }
 void OnGUI()
 {
  if(!story.MenuOpen&&!story.Paused)return;
  int depth=GUI.depth;GUI.depth=-100;
  var scale=new Vector3(Screen.width/1280f,Screen.height/720f,1);var old=GUI.matrix;GUI.matrix=Matrix4x4.TRS(Vector3.zero,Quaternion.identity,scale);
  GUI.Box(new Rect(285,130,710,470),"");
  var style=new GUIStyle(GUI.skin.label){fontSize=24,wordWrap=true};
  GUI.Label(new Rect(330,155,620,55),story.MenuOpen?(story.GetComponent<HuongDanHanhTrinhV48>()?"CON RỐI · v48":"CON RỐI · v47 RC3"):"TẠM DỪNG",style);
  if(GUI.Button(new Rect(330,225,290,45),story.MenuOpen?"New Game · Chơi mới [Enter]":"New Game · Chơi mới")){if(story.MenuOpen)NewGame();else confirm=true;}
  if(GUI.Button(new Rect(650,225,290,45),story.MenuOpen?"Load Game · Tải bản lưu [L]":"Load Game · Tải bản lưu [F6]"))Continue();
  if(GUI.Button(new Rect(330,285,290,45),"Hướng dẫn điều khiển [H]"))help=!help;
  if(!story.MenuOpen&&GUI.Button(new Rect(650,285,290,45),"Lưu trò chơi [F5]")){story.Save();message="Đã lưu.";}
  GUI.Label(new Rect(330,350,620,190),confirm?"Chơi mới sẽ ghi đè bản lưu hiện tại.\n":help?"WASD: đi · Chuột: nhìn · Space: nhảy\nE: tương tác / đọc tiếp · R: trả vật\nTab: cất trang · P: điện thoại · 1/2/3: chọn\nEsc: pause / tiếp tục · F5/F6: lưu / tải\nNgủ ở giường; tìm lối về ở cuối khu.":"Khám phá bốn khu theo thứ tự. Tương tác và dấu vết là lựa chọn của bạn.\n\n"+(story.Paused?"Esc · Tiếp tục\n":"")+message,style);
  if(confirm){if(GUI.Button(new Rect(330,485,290,45),"Xác nhận chơi mới"))NewGame();if(GUI.Button(new Rect(650,485,290,45),"Hủy"))confirm=false;}
  if(GUI.Button(new Rect(650,545,290,35),"Thoát trò chơi"))Application.Quit();
  GUI.matrix=old;GUI.depth=depth;
 }
}
