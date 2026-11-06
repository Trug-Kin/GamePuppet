from pathlib import Path
p=Path('Test/Tam/ground-r93-inventory.cs.txt').read_text()
p=p.replace('r93.unity','r94.unity').replace('clean r93','clean r94').replace('Test/KetQua/journey_v48/ground_r93/','Test/KetQua/journey_v48/market_ground_r95/')
p=p.replace('inventory.json','props-inventory.json').replace('scene-ground.meshbin','props-baseline.meshbin').replace('scene-components.txt','props-scene-components.txt')
p=p.replace('if(env&&ground&&broad&&wb.min.y<25&&m.isReadable)pick.Add(f);','if(env&&m.isReadable&&f.gameObject.activeInHierarchy&&((rr&&rr.enabled)||(cc&&cc.enabled))&&wb.max.x>450&&wb.min.x<1000&&wb.max.z>110&&wb.min.z<545)pick.Add(f);')
Path('Test/Tam/market95-props-dump.cs.txt').write_text(p,encoding='utf-8')
p=Path('Test/Tam/market95-cache.py').read_text().replace('scene-ground.meshbin','props-baseline.meshbin').replace("key='unity-'","key='props-unity-'").replace('geometry-inventory.json','props-geometry-inventory.json')
Path('Test/Tam/market95-props-cache.py').write_text(p,encoding='utf-8')
