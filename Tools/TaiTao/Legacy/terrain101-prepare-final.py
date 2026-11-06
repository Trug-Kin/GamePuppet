from pathlib import Path
import shutil
R=Path('D:/ConRoi');P=R/'Test/Tam';D=R/'Test/KetQua/journey_v48/terrain_r101';E=R/'Test/BangChung/journey_v48/terrain_r101'
for root,names in [(D,['route-motion.txt','route-progress.txt','route-response.json','capture-fixed-live.txt','capture-fixed.json']),(E,['terrain-controller.mp4'])]:
 target=root/'first-route-blocked';target.mkdir(exist_ok=True)
 for name in names:
  if (root/name).exists():assert not (target/name).exists();shutil.move(str(root/name),str(target/name))
s=(P/'terrain101-final-export.py').read_text();s=s.replace('TerrainR101_Final.fbx','TerrainR101_Closed.fbx');(P/'terrain101-closed-export.py').write_text(s)
s=(P/'terrain101-final-integrate.cs.txt').read_text();s=s.replace('TerrainR101_Final.fbx','TerrainR101_Closed.fbx').replace('_FinalR101','_ClosedR101');(P/'terrain101-closed-integrate.cs.txt').write_text(s)
s=(P/'terrain101-route.cs.txt').read_text();s=s.replace('new Vector2(650,414)','new Vector2(652,414)');(P/'terrain101-route.cs.txt').write_text(s)
