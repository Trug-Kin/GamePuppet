from pathlib import Path
import shutil
P=Path('Test/Tam');D=Path('Test/KetQua/journey_v48/terrain_r101');E=Path('Test/BangChung/journey_v48/terrain_r101')
for root,names in [(D,['route-motion.txt','route-progress.txt','route-response.json','capture-fixed-live.txt','capture-fixed.json']),(E,['terrain-controller.mp4'])]:
 target=root/'route-before-seam-sliver-clean';target.mkdir(exist_ok=True)
 for name in names:
  if (root/name).exists():assert not (target/name).exists();shutil.move(str(root/name),str(target/name))
s=(P/'terrain101-delivery-export.py').read_text(encoding='utf-8-sig').replace('TerrainR101_Delivery.fbx','TerrainR101_Integrated.fbx');(P/'terrain101-integrated-export.py').write_text(s,encoding='utf8')
s=(P/'terrain101-delivery-integrate.cs.txt').read_text(encoding='utf-8-sig').replace('TerrainR101_Delivery.fbx','TerrainR101_Integrated.fbx').replace('_DeliveryR101','_IntegratedR101');(P/'terrain101-integrated-integrate.cs.txt').write_text(s,encoding='utf8')
for name in ['terrain101-final-check.cs.txt','terrain101-region-return.cs.txt']:
 s=(P/name).read_text().replace('_DeliveryR101','_IntegratedR101');(P/name).write_text(s,encoding='utf8')
s=(P/'map100-back-diagonal.cs.txt').read_text(encoding='utf-8-sig').replace('map_r99_closeout','terrain_r101').replace('bridge route','terrain route').replace('on bridge','on terrain').replace('t>=2&&t<2.75?new KeyboardState(Key.W)','t>=2&&t<3.5?new KeyboardState(Key.S)').replace('t>=4&&t<4.75?new KeyboardState(Key.S)','t>=5&&t<6.5?new KeyboardState(Key.W)').replace('t>=6&&t<6.75?new KeyboardState(Key.W,Key.A)','t>=8&&t<8.8?new KeyboardState(Key.S,Key.A)').replace('t>=8&&t<8.75?new KeyboardState(Key.S,Key.D)','t>=10&&t<10.8?new KeyboardState(Key.W,Key.D)').replace('t<11','t<13');(P/'terrain101-back-diagonal.cs.txt').write_text(s,encoding='utf8')
s=(P/'map100-short-capture.py').read_text().replace('map_r99_closeout','terrain_r101').replace('Test/Tam/map100-back-diagonal.cs.txt','Test/Tam/terrain101-back-diagonal.cs.txt').replace("'15'","'17'").replace('bridge-backward-diagonal.mp4','terrain-backward-diagonal.mp4');(P/'terrain101-short-capture.py').write_text(s)
