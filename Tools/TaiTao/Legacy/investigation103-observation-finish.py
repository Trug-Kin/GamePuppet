from pathlib import Path
p=Path('Assets/Scripts/InvestigationV36R103/InvestigationEvidence.cs');s=p.read_text(encoding='utf-8')
changes={
'Vải/dầu không tự chứng minh thao tác xảy ra đúng lúc ngã.':'Các sợi chỉ còn bám ở mép rách; giữa mảnh có các đường dệt song song.',
'Sự khớp vật thể hỗ trợ người liên quan; không tự xác định giờ kéo hoặc ý định.':'Mép thiếu bất đối xứng; các mũi chỉ dừng lại ở cạnh rách.',
'Giày hỗ trợ hiện diện và thứ tự dấu, không một mình chứng minh gây ngã.':'Hai dãy vân lệch nhau; phần thiếu ở gót nằm lệch một bên.',
'Điểm điều khiển tách khỏi vị trí sửa; người đứng đó không đồng thời đẩy cánh bằng tay.':'Tay nắm dây đặt bên dưới gác, tách xa cánh và vị trí sửa.'}
for old,new in changes.items():
 assert old in s,old
 s=s.replace(old,new)
p.write_text(s,encoding='utf-8')
