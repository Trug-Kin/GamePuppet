import numpy as np,json
from pathlib import Path
from PIL import Image,ImageDraw
D=Path('Test/KetQua/journey_v48/terrain_r101');a=np.load(D/'surface142-before.npz');v=a['v'];f=a['faces'];n=a['normals'];c=v[f].mean(1);lines=[]
for z in range(390,426,2):
 mask=(c[:,0]<-630)&(c[:,0]>-670)&(abs(c[:,1]+z)<1);bad=mask&(n[:,2]<-.01)
 lines.append(dict(z=z,faces=int(mask.sum()),down=int(bad.sum()),x_range=[float(-c[bad,0].max()),float(-c[bad,0].min())] if bad.any() else None))
print(json.dumps(lines,indent=2));(D/'winding-extent.json').write_text(json.dumps(lines,indent=2))
im=Image.new('RGB',(1400,1000),'white');dr=ImageDraw.Draw(im)
for i in np.flatnonzero((c[:,0]<-630)&(c[:,0]>-675)&(c[:,1]<-390)&(c[:,1]>-430)):
 p=[((-v[j,0]-630)*25+40,(-v[j,1]-390)*22+40) for j in f[i]];dr.polygon(p,fill=(195,65,60) if n[i,2]<0 else (90,160,100),outline=(50,50,50))
for z in range(390,431,5):dr.text((1170,(z-390)*22+40),str(z),fill='black')
dr.text((40,945),'Unity X630..675 / Z390..430. RED: downward source faces, GREEN: upward. N points up (-Z).',fill='black');im.save('Test/BangChung/journey_v48/terrain_r101/winding-plan-before.png')
