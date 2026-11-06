import hashlib,json,re,sys,subprocess
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import imageio_ffmpeg
from PIL import Image,ImageDraw
root=Path.cwd();d=root/'Test/KetQua/investigation_r107';e=root/'Test/BangChung/investigation_r107'
# Keep failed attempts and unfinished direct checks visible.
p=d/'CHECKLIST.md';s=p.read_text(encoding='utf-8').replace('| Save/load giữa tutorial trước ghi; quay lại vật sau skip | PENDING | Bổ sung log targeted trước chốt nếu chạy được |','| Save/load giữa tutorial trước ghi; quay lại vật sau skip | NOT_RUN | Guard fresh intro thất bại vì live Editor đã có tiến trình ngoài fixture; không reset/xóa tiến trình để ép kiểm |');p.write_text(s,encoding='utf-8')
p=d/'README.md';s=p.read_text(encoding='utf-8');s+='''
## Đối chiếu bảo vệ và ca còn thiếu

`protected-final.json`: **1733/1733 entry không đổi**, gồm tập checkpoint/asset/nguồn đã theo dõi và17save gameplay gốc cùng1profile PT001r106. Không suy rộng thành toàn bộ mọi save trên máy. Hồ sơ kỹ thuật mới nằm trong thư mục kết quả r107. Bản sao live chưa phân loại được giữ riêng, không tự load vào phiên người dùng hoặc ghi thành kết quả playtest.

Lượt định kiểm tiếp trước lần ghi đầu bị guard fresh-intro từ chối: live Editor đã được thao tác ngoài fixture và có thêm tiến trình. Không tiếp tục reset/input cạnh tranh. Do đó **NOT_RUN**: save/load trước lần ghi đầu, quay lại vật sau skip, controller tới điện thoại theo mục tiêu thứ hai, layout1080p và UI mật mã/đối chiếu/kết luận sau đổi navigation. Những chức năng không đổi giữ kết quả r106; những điều kiện navigation mới chỉ được đọc code, không có PASS UI. Không hẹn rằng người mới chắc chắn hiểu mở đầu.
''';p.write_text(s,encoding='utf-8')
# Namespace-only copies of the stable mechanics must remain equivalent.
names=['ReturnGateMechanism.cs','MechanismPresentation.cs','TrialTimeline.cs','RepairHandBinding.cs','PresentationHands.cs','InvestigationActor.cs']
stable=[]
for name in names:
 a=(root/'Assets/Scripts/InvestigationV36R106'/name).read_text(encoding='utf-8-sig').replace('R106','RXXX').replace('r106','rxxx')
 b=(root/'Assets/Scripts/InvestigationV36R107'/name).read_text(encoding='utf-8-sig').replace('R107','RXXX').replace('r107','rxxx')
 stable.append(dict(file=name,equal_except_revision=a==b))
(d/'stable-code-audit.json').write_text(json.dumps(stable,indent=2),encoding='utf-8')
# Inspectable media metadata and contact sheet. This extracts existing capture; no new render/run.
ff=imageio_ffmpeg.get_ffmpeg_exe();videos=[]
for name in ['map-controller.mp4','onboarding-start.mp4','onboarding-short.mp4']:
 p=e/name
 raw=subprocess.run([ff,'-hide_banner','-i',str(p)],capture_output=True).stderr.decode(errors='replace')
 dur=re.search(r'Duration: (\d+):(\d+):([\d.]+)',raw);size=re.search(r'Video:.*?(\d{3,5})x(\d{3,5})',raw)
 videos.append(dict(file=name,duration_seconds=(int(dur[1])*3600+int(dur[2])*60+float(dur[3])) if dur else None,size=list(map(int,size.groups())) if size else None,audio='Audio:' in raw,input='synthetic keyboard/mouse through controller and GUI'))
(d/'final-video-metadata.json').write_text(json.dumps(videos,indent=2),encoding='utf-8')
frames=[]
for i,t in enumerate([4,45,68,82,95,110]):
 p=e/f'map-review-{i:02d}.png'
 subprocess.run([ff,'-hide_banner','-loglevel','error','-y','-ss',str(t),'-i',str(e/'map-controller.mp4'),'-frames:v','1',str(p)],check=True)
 im=Image.open(p).convert('RGB');im.thumbnail((480,290));frames.append((t,im.copy()))
sheet=Image.new('RGB',(1440,640),(25,25,25));draw=ImageDraw.Draw(sheet)
for i,(t,im) in enumerate(frames):x=(i%3)*480;y=(i//3)*320;sheet.paste(im,(x,y+24));draw.text((x+10,y+5),f'Controller synthetic / t={t}s',fill='white')
sheet.save(e/'map-controller-filmstrip.jpg',quality=90)
# UTF-8 validation and inventories (metadata only; hashes below exclude their own manifest).
docs=[root/'BAT_DAU_TAI_DAY.md',root/'Docs/PROJECT_GUIDE_AND_HISTORY.md',root/'Docs/BLENDER_UNITY_WORKFLOW.md',*d.glob('*.md')]
for p in docs:
 text=p.read_text(encoding='utf-8-sig');assert '\ufffd' not in text,p
(d/'utf8-audit.json').write_text(json.dumps(dict(files=[str(p.relative_to(root)) for p in docs],utf8_decode=True,replacement_character=False),ensure_ascii=False,indent=2),encoding='utf-8')
paths=[root/'Assets/Scenes/JourneyPresentation_v48_r107.unity',*sorted((root/'Assets/Scripts/InvestigationV36R107').glob('*')),*sorted((root/'Assets/Art/Environment/JourneyV48/TerrainR107').glob('*')),root/'Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend']
rows=[dict(path=str(p.relative_to(root)),bytes=p.stat().st_size,sha256=hashlib.sha256(p.read_bytes()).hexdigest()) for p in paths if p.is_file()]
(d/'product-manifest.json').write_text(json.dumps(rows,indent=2),encoding='utf-8')
(d/'evidence-index.json').write_text(json.dumps([dict(file=p.name,bytes=p.stat().st_size) for p in sorted(e.iterdir()) if p.is_file()],indent=2),encoding='utf-8')
print(json.dumps(dict(stable_code=stable,videos=videos,product_files=len(rows),utf8_files=len(docs)),ensure_ascii=False,indent=2))
