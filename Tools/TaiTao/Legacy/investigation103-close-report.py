from pathlib import Path
import json,sys,subprocess
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r103';E=R/'Test/BangChung/investigation_r103'
capture=json.loads((D/'controller-capture-final.json').read_text())
assert capture['exit']==0 and capture['completion'].startswith('21/21'),capture
f=imageio_ffmpeg.get_ffmpeg_exe();videos=[]
for name in ['controller-route-final.mp4','clip-six-cases-ui.mp4']:
 g=imageio_ffmpeg.read_frames(str(E/name));meta=next(g);g.close();videos.append(dict(file=name,metadata=meta,audio=False,input='synthetic InputSystem keyboard/mouse' if name.startswith('controller') else 'synthetic GameView UI mouse; controlled game illustration'))
duration=videos[0]['metadata']['duration']
for t,tag in [(2,'begin'),(30,'middle'),(max(1,duration-1),'end')]:
 subprocess.run([f,'-v','error','-y','-ss',str(t),'-i',str(E/'controller-route-final.mp4'),'-frames:v','1',str(E/('controller-final-'+tag+'.png'))],check=True)
(D/'video-metadata.json').write_text(json.dumps(videos,indent=2),encoding='utf-8')
readme=D/'README.md';s=readme.read_text(encoding='utf-8');s+='''
## Chốt lượt cuối

Đã mở lại scene sạch: **347 component** trong root prototype không có reference hiển thị bị dangling với ID khác0;18 interaction, Missing Script0, pose refs8. Initial records0, finalized=false, namespace R103, diagnostics=false, clip `Scene`12s. Đây là phạm vi kiểm cụ thể, không mọi serialized field toàn project.

`input-edges.txt` đã qua raw R/P, Esc đóng/pause/resume, chặn E cùng frame đóng UI, xoay/zoom bằng chuột tổng hợp. Fixture tạm cấu hình runtime InputSettings để nhận thiết bị ngoài focus GameView rồi khôi phục; lượt phụ thuộc focus bị lỗi giữ ở `input-edges-focus-failed.txt`. Không sửa input asset/controller để làm test đạt. Native phím thử trước đó không kích hoạt vào GameView, không tính là lượt pause đạt.

Clip tua +1s đã kiểm qua nút; slider click-track trước đó không có lượt đạt, không gọi slider đã kiểm. Bản ghi kết luận sau đổi checkbox nhỏ thành nút lớn đã lưu và ba lần Tải qua UI. Bố cục kết luận/mật mã/đối chiếu/inspection/bàn dựng được chụp lại ở cả720p/1080p.

Video ưu tiên: `controller-route-final.mp4` dừng theo END21/21 +1.5s (xem `controller-capture-final.json`), và `clip-six-cases-ui.mp4`87s. Các lượt capture cắt sớm/đen, click sai tọa độ và hook chắn ray không được dùng kết luận đạt. Xem `VIDEO_INDEX.md`, `CHECKLIST.md`. Art/clip còn provisional; người mới NOT_RUN.
''';readme.write_text(s,encoding='utf-8')
print('Final evidence:',capture['completion'],videos[0]['metadata'])
