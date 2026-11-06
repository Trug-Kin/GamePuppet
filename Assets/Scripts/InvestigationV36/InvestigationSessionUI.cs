using System.Linq;
using UnityEngine;

namespace ConRoi.Investigation.V36
{
    public sealed partial class InvestigationSession
    {
        GUIStyle titleStyle,bodyStyle,smallStyle,cardStyle,buttonStyle;
        Texture2D white;
        bool showLookup,confirmConclusion;
        readonly string[] trialNames={"Mở trước → thả → người di chuyển","Khải giúp mở; Tư còn đè giữ","Giữ D khi Tư đang dựa","Dùng móc M giữ dây","Dùng vật chèn","Chỉ tới sau cú ngã"};
        readonly string[] trialIds={"release","help","hold","hook","wedge","after"};
        readonly string[] conclusionTitles={"Người gửi thư và yêu cầu","Có mặt và dàn dựng","Vai trò trong cú ngã","Động cơ"};
        int conclusionIndex;
        Vector2 conclusionScroll;
        void Styles()
        {
            if(titleStyle!=null)return;white=Texture2D.whiteTexture;
            var font=Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            titleStyle=new GUIStyle(GUI.skin.label){font=font,fontSize=27,fontStyle=FontStyle.Bold,wordWrap=true,normal={textColor=new Color(.94f,.83f,.60f)}};
            bodyStyle=new GUIStyle(GUI.skin.label){font=font,fontSize=18,wordWrap=true,richText=false,normal={textColor=new Color(.9f,.91f,.88f)}};
            smallStyle=new GUIStyle(bodyStyle){fontSize=15};
            cardStyle=new GUIStyle(GUI.skin.box){padding=new RectOffset(18,18,14,14)};
            buttonStyle=new GUIStyle(GUI.skin.button){font=font,fontSize=17,wordWrap=true,padding=new RectOffset(12,12,9,9)};
            GUI.skin.textField.font=font;GUI.skin.textField.fontSize=18;GUI.skin.toggle.font=font;GUI.skin.toggle.fontSize=17;
        }
        void Fill(Rect rect,Color color){var old=GUI.color;GUI.color=color;GUI.DrawTexture(rect,white);GUI.color=old;}
        bool Button(string s,params GUILayoutOption[] options)=>GUILayout.Button(s,buttonStyle,options);
        void Text(string s)=>GUILayout.Label(s,bodyStyle);
        void Small(string s)=>GUILayout.Label(s,smallStyle);
        void Heading(string s)=>GUILayout.Label(s,titleStyle);
        void OnGUI()
        {
            Styles();var old=GUI.matrix;GUI.matrix=Matrix4x4.Scale(new Vector3(Screen.width/1200f,Screen.height/800f,1));
            if(!Modal)
            {
                Fill(new Rect(18,18,530,100),new Color(.055f,.09f,.10f,.92f));GUI.Label(new Rect(34,27,500,30),"CON RỐI  /  HỒ SƠ XƯỞNG",titleStyle);
                GUI.Label(new Rect(34,65,490,48),data.visitedHa?"Quay lại bàn dựng ở xưởng; ghi điều đã quan sát và phần còn thiếu.":"Khám xưởng → hỏi Hà tại quầy → quay lại xưởng.",smallStyle);
                GUI.Label(new Rect(590,388,30,30),"+",bodyStyle);
                if(focused){Fill(new Rect(350,594,500,48),new Color(.06f,.12f,.13f,.95f));GUI.Label(new Rect(365,603,480,35),"E  •  "+focused.verb+" "+(InvestigationContent.Get(focused.evidenceId)?.title??focused.name),bodyStyle);}
                Fill(new Rect(18,690,1164,92),new Color(.055f,.09f,.10f,.92f));GUI.Label(new Rect(32,700,1130,44),Message,bodyStyle);
                GUI.Label(new Rect(32,749,1120,25),"WASD / chuột • Space nhảy • E tương tác • P điện thoại/hồ sơ • Esc tạm dừng • F5 lưu riêng / F6 tải riêng",smallStyle);
            }
            else
            {
                Fill(new Rect(0,0,1200,800),new Color(.04f,.065f,.075f,.98f));
                GUI.Label(new Rect(28,20,1100,45),Paused?"TẠM DỪNG":"CON RỐI  /  ĐIỀU TRA",titleStyle);
                GUILayout.BeginArea(new Rect(22,84,218,660));
                if(Button("Trở lại  [R / Esc]")){if(Paused)SetPaused(false);else ClosePage();}
                if(Button("Điện thoại / hồ sơ")){SetPaused(false);OpenPage("dossier");}
                if(Has("letter")&&Button("Dải mật thư Hà")){SetPaused(false);OpenPage("cipher");}
                if(Button("Đối chiếu hai mục")){SetPaused(false);OpenPage("compare");}
                if(Has("phone")&&Button("Xem đoạn quay R-12")){SetPaused(false);Acquire("clip");OpenPage("clip");ClipTime=0;ClipPlaying=true;UpdateClip();}
                GUILayout.Space(18);if(Button("Lưu hồ sơ riêng  [F5]"))Save();if(Button("Tải hồ sơ riêng  [F6]"))Load();
                GUILayout.Space(14);Small("Save hướng điều tra riêng; không migration hoặc đọc save hành trình cũ.");
                GUILayout.Space(18);Small(Message);
                GUILayout.EndArea();
                GUILayout.BeginArea(new Rect(260,84,916,674));
                if(Paused){Heading("Mô phỏng và di chuyển đã dừng");Text("Tiếp tục để quay lại đúng hoạt động. Save/load dùng hồ sơ thử riêng.");}
                else if(Page=="inspect")InspectUI();
                else if(Page=="cipher")CipherUI();
                else if(Page=="compare")CompareUI();
                else if(Page=="reconstruction")ReconstructionUI();
                else if(Page=="clip")ClipUI();
                else DossierUI();
                GUILayout.EndArea();
                GUI.Label(new Rect(267,766,905,28),"Quan sát ≠ lời kể ≠ suy luận. Ghi liên hệ không tự biến nó thành sự thật.",smallStyle);
            }
            GUI.matrix=old;
        }
        void InspectUI()
        {
            var e=InvestigationContent.Get(inspectId);if(e==null)return;
            Heading(e.title);Small(e.source);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical(GUILayout.Width(360));
            if(viewerTexture&&viewer)GUILayout.Label(viewerTexture,GUILayout.Width(352),GUILayout.Height(352));else Small("Mục bản chép / ghi chú; không có vật 3D riêng.");
            Small("Giữ chuột trái để xoay • cuộn để phóng • R trả/cất. Vật gốc luôn ở vị trí hợp lệ.");
            if(Button("Đặt lại góc xem")){viewYaw=25;viewPitch=10;viewDistance=2;UpdateViewer();}
            if(inspectId=="phone"&&Button("Phát đoạn quay")){Acquire("clip");OpenPage("clip");ClipPlaying=true;ClipTime=0;UpdateClip();}
            if(inspectId=="letter"&&Button("Mở bảng gán ký tự"))OpenPage("cipher");
            if(inspectId=="henri"&&Button("Xem bảng tập")){showLookup=true;OpenPage("cipher");}
            GUILayout.EndVertical();
            GUILayout.BeginVertical(GUILayout.Width(530));scroll=GUILayout.BeginScrollView(scroll);
            Text("BẢN CHÉP / MÔ TẢ");Text(e.transcript);GUILayout.Space(12);
            Text("Quan sát bạn muốn ghi:");
            if(e.id=="clip"&&(!data.clipBeforeSeen||!data.clipEventSeen)){Text("Chưa xem đủ hai phần trước/trong sự cố. Quay lại clip để quan sát.");}
            else for(int i=0;i<e.observations.Length;i++){
                if(Observation(e.id,i))Small("Đã ghi • "+e.observations[i]);
                else if(Button("Ghi: "+e.observations[i]))Observe(e.id,i);
            }
            if(e.id=="coat"||e.id=="fabric")Small("Đối chiếu mép rách và đường may; không chỉ màu. Tạo hình mẫu vải đang là bản prototype.");
            GUILayout.EndScrollView();GUILayout.EndVertical();GUILayout.EndHorizontal();
        }
        void DossierUI()
        {
            if(Page=="conclusions"){ConclusionUI();return;}
            Heading("Điện thoại và hồ sơ");
            if(Has("khai")&&!data.samplesAllowed&&Button("Liên hệ Phương • xin xem mẫu áo và giày")){
                data.samplesAllowed=true;Acquire("samples");OpenEvidence("samples");return;
            }
            if(Button("Mở bản ghi kết luận của bạn")){OpenPage("conclusions");return;}
            scroll=GUILayout.BeginScrollView(scroll);
            foreach(var record in data.evidence){var e=InvestigationContent.Get(record.id);GUILayout.BeginVertical(cardStyle);if(Button(e.title+"  •  mở xem"))OpenEvidence(e.id);Small(e.source);foreach(int i in record.observations)if(i<e.observations.Length)Text("Quan sát đã ghi: "+e.observations[i]);GUILayout.EndVertical();}
            if(data.evidence.Count==0)Text("Chưa mở vật nào. Bắt đầu bằng điện thoại, dấu trên sàn và sổ Henri trong xưởng.");
            Heading("Liên hệ do bạn ghi");foreach(var rel in data.relations)Text(InvestigationContent.Get(rel.first).title+" ↔ "+InvestigationContent.Get(rel.second).title+": "+rel.note);
            GUILayout.EndScrollView();
        }
        void CompareUI()
        {
            Heading("Hai vật / hai ghi chú");Small("Chọn hai mục đã mở; không có đèn đúng/sai cho liên hệ.");
            scroll=GUILayout.BeginScrollView(scroll);GUILayout.BeginHorizontal();
            foreach(int side in new[]{0,1}){
                GUILayout.BeginVertical(cardStyle,GUILayout.Width(430));
                string selected=side==0?selectedA:selectedB;
                foreach(var record in data.evidence)if(Button((selected==record.id?"● ":"")+InvestigationContent.Get(record.id).title)){
                    if(side==0)selectedA=record.id;else selectedB=record.id;
                }
                if(!string.IsNullOrEmpty(selected)){var e=InvestigationContent.Get(selected);Text(e.transcript);foreach(int i in Record(selected).observations)Text(e.observations[i]);if(Button("Xem gần vật này"))OpenEvidence(selected);}
                GUILayout.EndVertical();
            }
            GUILayout.EndHorizontal();Text("Quan hệ / điều còn thiếu:");relationNote=GUILayout.TextField(relationNote,GUILayout.Height(55));
            if(Button("Ghi liên hệ"))AddRelation(selectedA,selectedB,relationNote);Text(feedback);GUILayout.EndScrollView();
        }
        void CipherUI()
        {
            Heading("Dải tư thế gửi Hà");Small("Mỗi hình = một chữ A–Z • chấm đặc = ngắt từ. Gán chữ tùy ý, xem bản đọc thử, sửa được.");
            scroll=GUILayout.BeginScrollView(scroll);
            if(Has("letter")){
                var area=GUILayoutUtility.GetRect(870,220);int col=0,row=0;
                foreach(char c in InvestigationContent.HaPlain){if(col>=21){row++;col=0;}var rect=new Rect(area.x+col*40,area.y+row*70,38,58);if(c==' ')GUI.Label(rect,"•",titleStyle);else DrawPose(rect,InvestigationContent.Alphabet.IndexOf(c));col++;}
                Text("Bản đọc thử: "+ReadCipher());
                GUILayout.BeginHorizontal();foreach(char c in InvestigationContent.Alphabet){GUILayout.BeginVertical(GUILayout.Width(51));var rect=GUILayoutUtility.GetRect(48,62);DrawPose(rect,InvestigationContent.Alphabet.IndexOf(c));string prior=data.mapping.FirstOrDefault(x=>x.pose==c.ToString())?.letter??"";string value=GUILayout.TextField(prior,1,GUILayout.Width(44));if(value!=prior)Assign(c.ToString(),value);if(GUILayout.Button("Xóa",buttonStyle,GUILayout.Width(44)))Assign(c.ToString(),"");GUILayout.EndVertical();}GUILayout.EndHorizontal();
                if(Button("Ghi cách đọc đã thử"))RecordReading();Text(feedback);
            }else Text("Chưa nhận thư. Bảng tập ở đây để tra trước.");
            if(Has("henri")){showLookup=GUILayout.Toggle(showLookup,"Mở bảng Henri (tùy chọn, không mất điểm)");if(showLookup){GUILayout.BeginHorizontal();foreach(char c in InvestigationContent.Alphabet){GUILayout.BeginVertical(GUILayout.Width(51));var rect=GUILayoutUtility.GetRect(48,62);DrawPose(rect,InvestigationContent.Alphabet.IndexOf(c));Text(c.ToString());GUILayout.EndVertical();}GUILayout.EndHorizontal();}}
            Text("Chú giải luôn tra được: STAND BY = chờ lệnh; WHERE = ở đâu; DID = dấu hỏi việc đã xảy ra; THE = từ xác định; THREE = ba; CRATES = thùng; GO = đi; SEND WORD = gửi lời lại.");
            if(Button("Tự mở gợi ý tiếp theo"))hintLevel=Mathf.Min(3,hintLevel+1);
            if(hintLevel>=1)Text("1. So bảy hình mở đầu với khẩu lệnh trong sổ nghề.");if(hintLevel>=2)Text("2. Một tư thế lặp giữ cùng chữ; dấu chấm không phải chữ.");if(hintLevel>=3)Text("3. Tra các hình trong THREE rồi kiểm cụm tiếp theo bằng bảng Henri; bạn vẫn tự gán chữ.");
            GUILayout.EndScrollView();
        }
        void Line(Vector2 a,Vector2 b,float width)
        {
            var m=GUI.matrix;var d=b-a;GUIUtility.RotateAroundPivot(Mathf.Atan2(d.y,d.x)*Mathf.Rad2Deg,a);GUI.DrawTexture(new Rect(a.x,a.y-width/2,d.magnitude,width),white);GUI.matrix=m;
        }
        void DrawPose(Rect r,int id)
        {
            if(poseIcons!=null&&id>=0&&id<poseIcons.Length&&poseIcons[id]){GUI.DrawTexture(r,poseIcons[id],ScaleMode.ScaleToFit,true);if(Event.current.type==EventType.MouseDown&&r.Contains(Event.current.mousePosition)&&cipherPuppet){cipherPuppet.pose=id;Event.current.Use();}return;}
            var old=GUI.color;GUI.color=new Color(.92f,.77f,.48f);float cx=r.center.x,y=r.y+23;GUI.DrawTexture(new Rect(cx-4,r.y+4,8,9),white);Line(new Vector2(cx,r.y+14),new Vector2(cx,r.y+37),3);
            float la=(-100+(id%5)*48)*Mathf.Deg2Rad,ra=(-85+(id/5)*75)*Mathf.Deg2Rad;
            var l=new Vector2(cx-3,y)+new Vector2(-Mathf.Cos(la),Mathf.Sin(la))*15;var rr=new Vector2(cx+3,y)+new Vector2(Mathf.Cos(ra),Mathf.Sin(ra))*15;
            Line(new Vector2(cx-3,y),l,3);Line(new Vector2(cx+3,y),rr,3);Line(l,l+new Vector2(-4,(id%2==0?-7:7)),2);Line(rr,rr+new Vector2(4,(id%3==0?-7:7)),2);
            Line(new Vector2(cx,r.y+36),new Vector2(cx-7,r.y+54),3);Line(new Vector2(cx,r.y+36),new Vector2(cx+7,r.y+54),3);GUI.color=old;
        }
        void ReconstructionUI()
        {
            Heading("Bàn dựng lại • bản sao cơ cấu");Small("Hình nộm minh họa có kiểm soát; không phải camera sự thật hoặc mô phỏng vật lý tự do.");
            GUILayout.BeginHorizontal();GUILayout.BeginVertical(GUILayout.Width(395));
            for(int i=0;i<trialIds.Length;i++)if(Button(trialNames[i]))StartTrial(trialIds[i]);
            if(Button("Dừng / đặt lại mô hình"))ResetTrial();
            GUILayout.EndVertical();GUILayout.BeginVertical(GUILayout.Width(500));
            if(stageTexture)GUILayout.Label(stageTexture,GUILayout.Width(490),GUILayout.Height(300));
            Small("t="+TrialTime.ToString("F2")+"s • L="+modelGate.state.angle.ToString("F1")+"° • D="+(modelGate.state.rope||modelGate.state.hook?"căng":"chùng")+" • C="+(modelGate.state.latched?"cài":"nhả"));
            Text(TrialReport);GUILayout.EndVertical();GUILayout.EndHorizontal();
            Small("Có thể thử lại. Móc/vật chèn hoặc người còn giữ lực phải được đối chiếu dấu hiện trường; không tự loại vì tên giả thuyết.");
        }
        void ClipUI()
        {
            Heading("Điện thoại • sửa R-12");Small("MINH HỌA PROTOTYPE • 9 giây liên tục • không âm thanh • không thấy ĐK/người kéo. Chưa là footage final.");
            if(clipTexture)GUILayout.Label(clipTexture,GUILayout.Width(780),GUILayout.Height(380));
            GUILayout.BeginHorizontal();if(Button(ClipPlaying?"Tạm dừng clip":"Phát clip"))ClipPlaying=!ClipPlaying;if(Button("Xem lại từ đầu")){ClipTime=0;ClipPlaying=true;UpdateClip();}GUILayout.EndHorizontal();
            float next=GUILayout.HorizontalSlider(ClipTime,0,9);if(Mathf.Abs(next-ClipTime)>.04f){ClipTime=next;ClipPlaying=false;UpdateClip();}Small(ClipTime.ToString("F2")+" / 9.00 s");
            if(Button("Ghi quan sát sau khi xem")){OpenEvidence("clip");}
        }
        public RenderTexture clipTexture;
        void ConclusionUI()
        {
            conclusionScroll=GUILayout.BeginScrollView(conclusionScroll);
            Heading("Bản ghi kết luận • phạm vi prototype");
            Small("Chọn chưa đủ hoặc tự ghi nhận định với căn cứ. Đây chưa là ending/định tội; chưa có toàn bộ chứng cứ vụ cũ hoặc nguồn riêng nhận diện người gửi.");
            conclusionIndex=GUILayout.SelectionGrid(conclusionIndex,conclusionTitles,2,buttonStyle);
            var c=data.conclusions[conclusionIndex];c.choice=GUILayout.SelectionGrid(c.choice,new[]{"Chưa đủ chứng minh","Khải có vai trò đã ghi","Tư tự thực hiện","Chưa xác định người"},2,buttonStyle);
            Text("Nhận định và giới hạn của bạn:");c.claim=GUILayout.TextField(c.claim??"",GUILayout.Height(45));
            scroll=GUILayout.BeginScrollView(scroll,GUILayout.Height(300));
            foreach(var record in data.evidence.Where(e=>e.observations.Count>0)){
                bool value=c.grounds.Contains(record.id);bool next=GUILayout.Toggle(value,InvestigationContent.Get(record.id).title);
                if(next&&!value)c.grounds.Add(record.id);if(!next&&value)c.grounds.Remove(record.id);
            }
            GUILayout.EndScrollView();Small("Căn cứ đã gắn: "+c.grounds.Count+". Số lượng không xác định đủ chứng minh.");
            confirmConclusion=GUILayout.Toggle(confirmConclusion,"Tôi muốn lưu lựa chọn hiện tại, kể cả phần chưa đủ; chưa có chấm đáp án.");
            if(confirmConclusion&&Button("Lưu bản ghi kết luận")){data.finalized=true;Save();feedback="Đã lưu nhận định và căn cứ của bạn. Không suy ý định gây chết từ thao tác cơ cấu.";}
            Text(feedback);
            GUILayout.EndScrollView();
        }
    }
}
