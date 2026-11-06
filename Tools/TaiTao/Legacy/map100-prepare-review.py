from pathlib import Path
import difflib,json,re,shutil
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';M=R/'Test/BangChung/journey_v48/map_r99_closeout';T=R/'Test/Tam'
for name in ['fixture','route','pause','save-review','region-return','contact-views','record','resume-review','stop']:
 ext='.py' if name=='record' else '.cs.txt';s=(T/('dam99-'+name+ext)).read_text(encoding='utf-8-sig');s=s.replace('Test/KetQua/journey_v48/dam_r99/','Test/KetQua/journey_v48/map_r99_closeout/').replace('Test/BangChung/journey_v48/dam_r99/','Test/BangChung/journey_v48/map_r99_closeout/').replace("Path('Test/BangChung/journey_v48/dam_r99')","Path('Test/BangChung/journey_v48/map_r99_closeout')").replace("Path('Test/KetQua/journey_v48/dam_r99')","Path('Test/KetQua/journey_v48/map_r99_closeout')")
 if name=='record':s=s.replace('Test/Tam/dam99-route.cs.txt','Test/Tam/map100-route.cs.txt')
 (T/('map100-'+name+ext)).write_text(s,encoding='utf-8')
s=(T/'dam99-play.cs.txt').read_text().replace('r99.unity','r100.unity');(T/'map100-play.cs.txt').write_text(s)
for srcname in ['map100-views.cs.txt','map100-bed-views.cs.txt']:
 s=(T/srcname).read_text().replace('-r99.png','-r100.png').replace('actual-layer-rays.txt','r100-actual-layer-rays.txt').replace('bed-layer-rays.txt','r100-bed-layer-rays.txt');(T/(srcname.replace('.cs','-final.cs'))).write_text(s)
a=(R/'Assets/Scenes/JourneyPresentation_v48_r99.unity').read_text().splitlines();b=(R/'Assets/Scenes/JourneyPresentation_v48_r100.unity').read_text().splitlines();(D/'r99-r100-scene.diff').write_text('\n'.join(difflib.unified_diff(a,b,fromfile='r99',tofile='r100',n=2)))
for name in ['migrated-private.json']:
 shutil.copy2(R/'Test/KetQua/journey_v48/dam_r99'/name,D/name)
print('Review snippets adapted from existing R99 tools; no Assets runner. Scene diff lines',len(list(difflib.unified_diff(a,b))))
