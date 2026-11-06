from pathlib import Path
import shutil
r=Path('D:/ConRoi');d=r/'Test/KetQua/journey_v48/dam_r98';m=r/'Test/BangChung/journey_v48/dam_r98';out=d/'pre-toe-review';out.mkdir(exist_ok=True)
for n in ['route-motion.txt','route-progress.txt','capture-fixed.json','capture-fixed-live.txt','route-summary.json','pause.txt','old-height-load.txt','region-return.txt']:
 p=d/n
 if p.exists():shutil.copy2(p,out/n)
p=m/'dam-end-stairs-controller.mp4';shutil.copy2(p,m/'dam-end-stairs-controller-pre-toe.mp4')
