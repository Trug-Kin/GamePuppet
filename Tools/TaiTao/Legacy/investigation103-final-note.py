from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r103'
for name in ['README.md','VIDEO_INDEX.md']:
 p=D/name;s=p.read_text(encoding='utf-8');s+='\nNhãn prompt bàn dựng được sửa sau capture: từ tên object kỹ thuật sang “bàn dựng lại” (dây/cánh cũng dùng tên thường). `final-table-prompt.png` là fixture API nạp lại vị trí save tuyến để xem chữ, **không** là bằng chứng đi controller. Video tuyến vẫn ghi đúng lượt di chuyển trước sửa chữ này; geometry/controller không đổi.\n';p.write_text(s,encoding='utf-8')
for name in ['PROJECT_GUIDE_AND_HISTORY.md','BLENDER_UNITY_WORKFLOW.md']:
 p=R/'Docs'/name;s=p.read_text(encoding='utf-8');i=s.index('\n\n');s=s[:i]+'''\n\nChốt bổ sung r103: raw R/P/Esc và xoay/zoom qua InputSystem đã kiểm với runtime fixture không phụ thuộc focus rồi khôi phục; lượt focus FAIL giữ riêng, không sửa controller/input asset. Nút xác nhận lớn và tua một giây đã kiểm qua UI; nhãn prompt bàn dựng/dây/cánh dùng lời thường thay tên object kỹ thuật. Video tuyến kết thúc theo END21/21 +1.5s; prompt chữ sửa sau video có ảnh fixture riêng. Reopen root prototype347 component,18 tương tác, Missing Script0, initial records0; không suy toàn map hoặc mọi ref.''' +s[i:];p.write_text(s,encoding='utf-8')
print('Added final scope and prompt-only capture note')
