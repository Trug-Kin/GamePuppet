from pathlib import Path
R=Path('D:/ConRoi/Assets/Scripts/InvestigationV36R107')
p=R/'InvestigationEvidence.cs';s=p.read_text(encoding='utf-8')
s=s.replace('public bool samplesAllowed,returnedToWorkshop,visitedHa;','public bool samplesAllowed,returnedToWorkshop,visitedHa;\n        public bool introAccepted,tutorialSkipped,firstFloorRecorded,firstNotesReviewed,tableDiscovered;\n        public int tutorialWaypoint;')
s=s.replace('Vệt kéo dài tới mép cánh. Hiện trường được giữ nguyên; các lần thử dùng mô hình riêng.','Hai dải bụi kéo dài trên sàn. Ở chỗ giao, các khối vân đế giày nằm trên dải bụi; có khoảng khuyết ở phần gót. Hãy xem hình và ghi đặc điểm nhìn thấy.')
s=s.replace('Vệt trượt đi từ chỗ sửa tới mép cánh.','Hai dải bụi kéo dài tới mép cánh.').replace('Dấu đế giày nằm trên một đoạn vệt trượt; thứ tự chồng dấu cần được đối chiếu.','Các khối vân đế giày nằm trên một đoạn dải bụi; phần giao không bị bụi phủ lại.')
p.write_text(s,encoding='utf-8')
p=R/'InvestigationSession.cs';s=p.read_text(encoding='utf-8')
s=s.replace('public string Message {get;private set;}="Khám xưởng, xem điện thoại và cơ cấu; sau đó tìm Hà ở quầy ngoài cửa.";','string transientMessage=""; float messageUntil;\n        public string Message {get=>Time.unscaledTime<messageUntil?transientMessage:"";private set{transientMessage=value;messageUntil=Time.unscaledTime+6;}}')
s=s.replace('void Start(){if(modelGate)modelGate.enabled=false;SetModalState();}','void Start(){if(modelGate)modelGate.enabled=false;if(!data.introAccepted)OpenPage("intro");else SetModalState();}')
s=s.replace('if(!Modal && !(k!=null', 'UpdateOnboarding();\n            if(!Modal && !(k!=null')
s=s.replace('if(target.action=="table"){OpenPage("reconstruction");return;}','if(target.action=="table"){data.tableDiscovered=true;OpenPage("reconstruction");return;}')
s=s.replace('public void OpenPage(string page){Page=page;','public void OpenPage(string page){if(page=="dossier"&&data.firstFloorRecorded)data.firstNotesReviewed=true;Page=page;')
s=s.replace('"clip","conclusions"}.Contains(data.page)','"clip","conclusions","intro"}.Contains(data.page)')
s=s.replace('viewDistance=2;UpdateViewer();','viewDistance=(inspectId=="slide"||inspectId=="shoeprint")?1.65f:2;UpdateViewer();')
p.write_text(s,encoding='utf-8')
p=R/'InvestigationSessionUI.cs';s=p.read_text(encoding='utf-8')
a=s.index('                Fill(new Rect(18,18,530,100)');b=s.index('\n            }\n            else',a)
s=s[:a]+'''                Fill(new Rect(18,18,720,74),new Color(.055f,.09f,.10f,.90f));
                GUI.Label(new Rect(30,24,696,60),CurrentObjective,bodyStyle);
                GUI.Label(new Rect(590,388,30,30),"+",smallStyle);
                DrawObjectiveMarker();
                if(focused){Fill(new Rect(315,604,570,48),new Color(.06f,.12f,.13f,.95f));string prompt=focused.evidenceId=="slide"?"E · Kiểm tra dấu trên sàn":focused.action=="table"?"E · Dùng bàn dựng lại":"E · "+focused.verb+" "+(focused.action=="rope"?"tay nắm dây":focused.action=="gateforce"?"cánh lan can":InvestigationContent.Get(focused.evidenceId)?.title??"vật đang nhìn");GUI.Label(new Rect(330,612,540,36),prompt,bodyStyle);}
                if(Message!=""){Fill(new Rect(240,678,720,58),new Color(.055f,.09f,.10f,.92f));GUI.Label(new Rect(254,685,690,48),Message,bodyStyle);}
                GUI.Label(new Rect(22,763,600,25),"P · Ghi chép     Esc · Tạm dừng     Bản dựng thử",smallStyle);''' +s[b:]
s=s.replace('bool focusedReview=!Paused', 'if(!Paused&&Page=="intro"){GUILayout.BeginArea(new Rect(210,160,780,510));Heading("Hiện trường xưởng rối");Text("Ông Tư được phát hiện tử vong tại xưởng rối. Hãy kiểm tra hiện trường để tìm hiểu chuyện gì đã xảy ra.");GUILayout.Space(24);Text("Vào xưởng, lên gác và kiểm tra dấu trên sàn gần lan can.");GUILayout.Space(20);Small("WASD để đi · Chuột để nhìn · E khi có lời nhắc");GUILayout.Space(20);if(Button("Bắt đầu"))BeginIntroduction(false);if(Button("Bỏ qua hướng dẫn"))BeginIntroduction(true);Small("Bản dựng thử");GUILayout.EndArea();GUI.matrix=old;return;}\n                bool focusedReview=!Paused')
a=s.index('                if(Button("Điện thoại / hồ sơ"))');b=s.index('\n                GUILayout.EndArea();',a)
s=s[:a]+'''                if(Paused||Page=="dossier"||Page=="conclusions"){
                    if(Page!="dossier"&&Button("Ghi chép")){SetPaused(false);OpenPage("dossier");}
                    if(!Paused&&Has("letter")&&Button("Đọc thư Hà"))OpenPage("cipher");
                    if(!Paused&&data.evidence.Count>=2&&Button("Đối chiếu hai mục"))OpenPage("compare");
                    if(!Paused&&Has("phone")&&Button("Xem đoạn quay R-12")){Acquire("clip");OpenPage("clip");ClipTime=0;ClipPlaying=true;UpdateClip();}
                    if(!Paused&&data.tableDiscovered&&Has("mechanism")&&Button("Bàn dựng lại"))OpenPage("reconstruction");
                    GUILayout.Space(18);if(Button("Lưu  [F5]"))Save();if(Button("Tải  [F6]"))Load();
                    if(!data.tutorialSkipped&&Button("Bỏ qua hướng dẫn")){data.tutorialSkipped=true;data.introAccepted=true;}
                }
                GUILayout.Space(14);Small("Bản dựng thử");
                if(diagnostics){GUILayout.Space(18);Small(data.namespaceId);Small(Message);}''' +s[b:]
s=s.replace('Small("Giữ chuột trái để xoay • cuộn để phóng • R trả/cất. Vật gốc luôn ở vị trí hợp lệ.");','Small("Giữ chuột để xoay · Cuộn để phóng");\n            if(Button("Trở lại [R]"))ClosePage();')
# Closing a page defers inspector cleanup to the next GUI event.
s=s.replace('if(Button("Trở lại [R]"))ClosePage();','if(Button("Trở lại [R]")){ClosePage();GUILayout.EndVertical();GUILayout.EndHorizontal();return;}')
a=s.index('            Text("Điều bạn trực tiếp nhìn thấy:");');b=s.index('\n            if(e.id=="coat"',a)
s=s[:a]+'''            Text("Điều bạn trực tiếp nhìn thấy:");
            if(e.id=="clip"&&(!data.clipBeforeSeen||!data.clipEventSeen)){Text("Quay lại đoạn quay để quan sát trước khi ghi.");}
            else {
                if(Button("Ghi quan sát"))RecordVisibleObservations(e.id);
                var rec=Record(e.id);
                if(rec!=null)foreach(int i in rec.observations)if(i<e.observations.Length)Small("Đã ghi · "+e.observations[i]);
            }''' +s[b:]
s=s.replace('Heading("Điện thoại và hồ sơ");','Heading("Ghi chép");Text(CurrentObjective);\n            if(data.evidence.Count>=2)Small("Đối chiếu đặt hai chi tiết cạnh nhau để bạn tự xem và ghi liên hệ.");')
s=s.replace('Chưa mở vật nào. Bắt đầu bằng điện thoại, dấu trên sàn và sổ Henri trong xưởng.','Chưa có ghi chép. Vào xưởng, lên gác và kiểm tra dấu trên sàn gần lan can.')
p.write_text(s,encoding='utf-8')
print('Updated isolated R107 session, save flags, contextual UI. Mechanism unchanged.')
