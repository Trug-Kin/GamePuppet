from pathlib import Path
R=Path('D:/ConRoi/Assets/Scripts/InvestigationV36R103')
p=R/'InvestigationSession.cs';s=p.read_text(encoding='utf-8');a=s.index('            string detail=');b=s.index('\n            TrialReport=',a)
s=s[:a]+'''            string detail=TrialId=="release"?"Người rời mép rồi quay lại khi chốt chưa bắt. Hãy so với trạng thái trước sự cố trong clip.":TrialId=="help"?"Dây đã thả nhưng Tư vẫn giữ lực; cánh chưa hồi qua người. So với khoảng cánh đóng trước sự cố.":TrialId=="hold"?"Quan sát thời điểm giữ dây và người mất chỗ tựa. Chưa biết người kéo hoặc ý định.":TrialId=="hook"?"Móc giữ dây thay người. So bề mặt móc tại xưởng.":TrialId=="wedge"?"Vật chèn giữ cánh. So khe cánh tại xưởng.":"Người tới sau có thể để dấu. Diễn biến này chưa giải thích cú ngã trước đó.";'''+s[b:]
p.write_text(s,encoding='utf-8')
p=R/'InvestigationSessionUI.cs';s=p.read_text(encoding='utf-8')
s=s.replace('GUILayout.Space(14);Small("Save hướng điều tra riêng; không migration hoặc đọc save hành trình cũ.");','GUILayout.Space(14);Small("Bản dựng thử");')
s=s.replace('GUILayout.Space(18);Small(Message);','if(diagnostics){GUILayout.Space(18);Small(data.namespaceId);Small(Message);}')
s=s.replace('Lưu hồ sơ riêng','Lưu hồ sơ').replace('Tải hồ sơ riêng','Tải hồ sơ')
s=s.replace('GUI.Label(new Rect(267,766,905,28),','if(diagnostics)GUI.Label(new Rect(267,766,905,28),')
s=s.replace('if(Button("Đặt lại góc xem")){viewYaw=25;viewPitch=10;viewDistance=2;UpdateViewer();}', '''if(Button("Nhìn mặt trên")){viewYaw=0;viewPitch=-75;viewDistance=1.7f;UpdateViewer();}
            if(Button("Nhìn mặt dưới")){viewYaw=0;viewPitch=75;viewDistance=1.7f;UpdateViewer();}
            if(inspectId=="mechanism"&&Button("Cận chốt / toàn dây")){viewDistance=viewDistance<1?2:.55f;UpdateViewer();}''')
s=s.replace('Text("Quan sát bạn muốn ghi:");','Text("Điều bạn trực tiếp nhìn thấy:");')
s=s.replace('GUILayout.Button("Xóa",','GUILayout.Button("×",')
s=s.replace('Small("Hình nộm minh họa có kiểm soát; không phải camera sự thật hoặc mô phỏng vật lý tự do.");','Small("Bản dựng thử • theo dõi người, dây và cánh.");')
a=s.index('            Small("t="');b=s.index('\n            Text(TrialReport)',a)
s=s[:a]+'''            Text(MechanismPresentation.StateWords(modelGate.state));
            if(diagnostics)Small("t="+TrialTime.ToString("F2")+" / L="+modelGate.state.angle.ToString("F1")+" / latched="+modelGate.state.latched);'''+s[b:]
s=s.replace('Small("Có thể thử lại. Móc/vật chèn hoặc người còn giữ lực phải được đối chiếu dấu hiện trường; không tự loại vì tên giả thuyết.");','Small("Có thể thử lại, rồi so điều bạn thấy với clip và vật tại xưởng.");')
s=s.replace('Small("MINH HỌA PROTOTYPE • 9 giây liên tục • không âm thanh • không thấy ĐK/người kéo. Chưa là footage final.");','Small("Bản dựng thử từ buổi quay sửa R-12 • không âm thanh");')
s=s.replace('GUILayout.HorizontalSlider(ClipTime,0,9)','GUILayout.HorizontalSlider(ClipTime,0,MechanismPresentation.ClipLength)').replace(' / 9.00 s',' / 12.00 s')
s=s.replace('Text("Tiếp tục để quay lại đúng hoạt động. Save/load dùng hồ sơ thử riêng.");','Text("Tiếp tục để quay lại đúng hoạt động.");')
s=s.replace('Small("Chọn chưa đủ hoặc tự ghi nhận định với căn cứ. Đây chưa là ending/định tội; chưa có toàn bộ chứng cứ vụ cũ hoặc nguồn riêng nhận diện người gửi.");','Small("Ghi nhận định, căn cứ và điều còn thiếu. Bạn có thể chọn chưa đủ chứng minh.");')
p.write_text(s,encoding='utf-8')
# Definitions describe visible traits; ordering/identity/timing stays in free comparison notes.
p=R/'InvestigationEvidence.cs';s=p.read_text(encoding='utf-8')
s=s.replace('Dấu giày chồng lên vệt trượt, có người tới mép sau khi vệt hình thành.','Các khối vân đen nằm trên hai dải bụi kéo dài, phần giao không bị bụi phủ lại.')
s=s.replace('Dấu giày chồng vệt','Dấu đế tại mép gác')
s=s.replace('Gót khuyết lệch và hàng vân mòn tương ứng vùng dấu đã quan sát.','Gót có một góc khuyết; một khối vân giữa nhỏ hơn các khối bên cạnh.')
s=s.replace('Bản minh họa 9 giây liên tục','Bản dựng thử 12 giây liên tục')
s=s.replace('Clip sửa rối có sẵn, không cần phục hồi tệp.','Tư đặt điện thoại trên giá để quay cách thay dây tay phải R-12 cho người học nghề. Clip có sẵn ngay lúc khám, không cần phục hồi tệp.')
s=s.replace('new EvidenceDefinition("ha",','new EvidenceDefinition("shoecontrol","Giày mẫu B","Bàn mẫu đối chiếu của xưởng","Mẫu cùng màu và kiểu đế. Hãy xem góc gót và từng hàng vân; tên mẫu không xác định người mang.","Góc gót khuyết ngắn hơn; khối vân giữa còn nguyên."),\n            new EvidenceDefinition("ha",')
p.write_text(s,encoding='utf-8')
