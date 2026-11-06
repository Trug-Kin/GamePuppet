from pathlib import Path
import shutil
P=Path('Test/Tam');D=Path('Test/KetQua/journey_v48/terrain_r101');E=Path('Test/BangChung/journey_v48/terrain_r101')
for root,names in [(D,['route-motion.txt','route-progress.txt','route-response.json','capture-fixed-live.txt','capture-fixed.json']),(E,['terrain-controller.mp4'])]:
 target=root/'second-route-blocked';target.mkdir(exist_ok=True)
 for name in names:
  if (root/name).exists():assert not (target/name).exists();shutil.move(str(root/name),str(target/name))
s=(P/'terrain101-verified-export.py').read_text(encoding='utf-8-sig').replace("'M95_Service3_NATIVE'];","'M95_Service3_NATIVE','TerrainColliderJoin_R90'];").replace("name=='M95_Service3_NATIVE'","name in ['M95_Service3_NATIVE','TerrainColliderJoin_R90']").replace("assert len(out)==4","assert len(out)==5").replace('TerrainR101_Verified.fbx','TerrainR101_Delivery.fbx');(P/'terrain101-delivery-export.py').write_text(s,encoding='utf8')
s=(P/'terrain101-verified-integrate.cs.txt').read_text(encoding='utf-8-sig').replace('TerrainR101_Verified.fbx','TerrainR101_Delivery.fbx').replace('_VerifiedR101','_DeliveryR101');(P/'terrain101-delivery-integrate.cs.txt').write_text(s,encoding='utf8')
