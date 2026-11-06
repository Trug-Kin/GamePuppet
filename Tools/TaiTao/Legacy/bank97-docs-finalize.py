from pathlib import Path
R=Path('D:/ConRoi')
for f in ['Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md']:
 p=R/f;s=p.read_text(encoding='utf-8');s=s.replace('đi64điểm','đi68điểm')
 if 'PROJECT_GUIDE' in f:
  s=s.replace('Dải16..24m chỉ sửa winding các mặt nhỏ có bằng chứng backface, không đổi cao độ.','Dải16..24m ban đầu sửa winding các mặt nhỏ. Kiểm cận cuối xác nhận thêm Surface142 backface tại đầu nối về cầu: sửa winding của28193 mặt near-horizontal hướng xuống trong envelope X640..700/Z411..492 trên riêng mesh này, không đổi một vertex/cao độ nào.')
  s=s.replace('xem log/video cuối và filmstrip','xem log/video cuối và filmstrip')
  s=s.replace('một fixture đầu, input keyboard/mouse','một fixture đầu,68/68 waypoint (bao gồm dải biên X678.5/Z483→X679/Z486), input keyboard/mouse')
  s=s.replace('Chuyển phòng→load chợ và mở lại scene kiểm lại binding riêng','Chuyển phòng→load chợ giữ19 collider enabled nguồn r97, một root13 child; mở lại scene kiểm binding riêng')
  marker='**Bộ xem:**'
  extra='''**Bổ sung chốt biên:** `boundary-rays-before.txt` xác nhận X679/676/675/673 tại Z486 chỉ hit khi backface=true; `boundary-rays.txt` sau sửa hit bình thường. Ảnh cận `JourneyPresentation_v48_r97-endpoint-north.png` và `-upper-join.png` đã xem lại, lỗ cull lớn biến mất. Còn vài vệt vật liệu nhỏ trên terrain cũ và sai khác context xa; không mở đợt shading/I2. `scene-delta.json` xác nhận không đổi transform hoặc MonoBehaviour hiện hữu (Bảo/điểm tương tác giữ nguyên); không thay kiểm pose toàn bộ Bảo bằng kiểm tĩnh này.

Video tuyến cuối dùng ánh sáng review sun1.5/fog-clear ở lượt đi, giữ sky gameplay; từ waypoint32 trả sun/fog gameplay cho toàn lượt về. Clip bổ sung riêng [bậc trong gameplay](../Test/BangChung/journey_v48/bank_r97/bank-gameplay-stairs-controller.mp4) dài16.5s/485frame≈29.394fps, fixture tại lối thấp Z450,15/15 điểm lên rồi xuống, không ghép vào tuyến cầu. Cả hai capture cửa sổ Game1600×984, nguồn30fps VFR, không âm thanh; input mô phỏng không phải chứng nhận phần cứng. Video/filmstrip trước sửa terrain và trước sửa winding giữ riêng làm lịch sử; đọc metadata cuối để lấy thời lượng thực.

'''
  s=s.replace(marker,extra+marker,1)
 else:
  s=s.replace('cleanup → `bank97-final-render-export.py`','cleanup → `bank97-boundary-finish.py` → `bank97-final-render-export.py`')
  s=s.replace('Update mesh asset phải','Sửa winding cuối dùng `bank97-boundary-finish.py`: chỉ M95_022, X640..700/Z411..492, normalZ<−0.7, assert toàn bộ tọa độ vertex không đổi; sau lưu/thoát/mở lại export, chạy lại selective integration cùng script. `bank97-update-final-route.py` thêm dải biên vào tuyến68điểm và trả gameplay light từ waypoint32 (chạy một lần sau generator route). Update mesh asset phải')
 p.write_text(s,encoding='utf-8')
print('Finalized two existing docs')
