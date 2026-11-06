import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/i2_r93_complete');M=Path('Test/BangChung/journey_v48/i2_r93_complete');rows=json.loads((D/'geometry-change.json').read_text())['objects'];F=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',22);S=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',16);cx,cz=714.94,459.48
im=Image.new('RGB',(1600,1300),'#f4f5f6');dr=ImageDraw.Draw(im)
def pix(p):return (100+(cx+55-p[0])*10,120+(p[2]-(cz-17))*10)
colors={'Surface_142':'#698c52','Surface_176':'#7a9e63','Surface_128':'#407d9c','Surface_129':'#465fb3','Surface_153':'#b99d65'}
for row in rows:
 n=row['path'].split('/')[-1]
 if n not in colors:continue
 a=np.load(D/(row['key']+'-source.npz'));v=a['v'];tri=a['tri'];q=v[tri];cen=q.mean(1);sel=(np.linalg.norm(cen[:,[0,2]]-[cx,cz],axis=1)<53)&(cen[:,2]>cz-15)
 for qq in q[sel]:dr.polygon([pix(p) for p in qq],fill=colors[n])
 changed=abs(v[:,1]-a['before'][:,1])>.001
 for p in v[changed]:
  x,y=pix(p);dr.ellipse((x-1,y-1,x+1,y+1),fill='#d63829')
center=pix([cx,0,cz]);dr.ellipse((center[0]-125,center[1]-125,center[0]+125,center[1]+125),outline='#222',width=3)
for x in [cx-2,cx,cx+2]:dr.line([pix([x,0,cz+13]),pix([x,0,cz+28])],fill='#ffdc32',width=2)
for z in [474,480,488,494]:dr.line([pix([cx-30,0,z]),pix([cx+30,0,z])],fill='#e2cc30',width=1)
dr.text((30,20),'I2 AUTHORING REPAIR / actual changed vertices > 1 mm in red',font=F,fill='black');dr.text((30,55),'North = -Unity Z; East = -Unity X. Authoring units only, field heights UNVERIFIED.',font=S,fill='black')
dr.line((1430,220,1430,120),fill='black',width=4);dr.polygon([(1430,110),(1420,130),(1440,130)],fill='black');dr.text((1422,80),'N',font=F,fill='black')
for i,(n,col) in enumerate(colors.items()):dr.rectangle((1220,280+i*35,1245,302+i*35),fill=col);dr.text((1255,280+i*35),n,font=S,fill='black')
dr.text((1210,500),'I2 island unchanged',font=S,fill='black');dr.text((1210,535),'Yellow: fixed sections',font=S,fill='black');dr.text((1210,570),'No claim whole map flat',font=S,fill='black')
dr.line((110,1120,210,1120),fill='black',width=4);dr.text((110,1135),'10 authoring m',font=S,fill='black');dr.text((30,1190),'Original source/checkpoint: GroundSurvey_R93_Editable.blend / scene r93',font=S,fill='black');im.save(M/'changed-area-north-up.png')
# Same-position sections with explicit XZ and unchanged end levels.
sections=json.loads((D/'sections.json').read_text());measure=json.loads((D/'layer-measurements.json').read_text());out=[]
for r in measure:
 for tag in ['before','after']:
  for s in r[tag+'_sections']:
   name=sections[s['section']]['name']
   if name not in ['radial90-width0','radial60-width0','radial120-width0','transverse-z474','transverse-z480','diagonal-east']:continue
   h=np.array([np.nan if v is None else v for v in s['top']]);ok=np.isfinite(h)
   if ok.sum()<2:continue
   ds=np.array(sections[s['section']]['distance']);ix=np.flatnonzero(ok);runs=np.split(ix,np.flatnonzero(np.diff(ix)>1)+1)
   for run in runs:
    if len(run)<2:continue
    i,j=run[0],run[-1];out.append(dict(layer=r['path'],state=tag,section=name,start_xz=sections[s['section']]['xz'][i],end_xz=sections[s['section']]['xz'][j],length=float(ds[j]-ds[i]),start_height=float(h[i]),end_height=float(h[j]),delta_height=float(h[j]-h[i]),overall_grade_percent=float((h[j]-h[i])/(ds[j]-ds[i])*100),peak_sampled_grade_percent=s['max_abs_grade']))
(D/'section-extents-and-levels.json').write_text(json.dumps(out,indent=2))
# Local wireframe from the actual saved authoring meshes, no proxy overlay used as geometry.
im=Image.new('RGB',(1600,1100),'white');dr=ImageDraw.Draw(im)
def w(p):return (100+(740-p[0])*30,100+(p[2]-469)*30)
for row in rows:
 n=row['path'].split('/')[-1]
 if n not in colors:continue
 a=np.load(D/(row['key']+'-source.npz'));q=a['v'][a['tri']];cen=q.mean(1);sel=(cen[:,0]>690)&(cen[:,0]<740)&(cen[:,2]>469)&(cen[:,2]<499)
 for qq in q[sel]:dr.line([w(p) for p in [*qq,qq[0]]],fill=colors[n],width=1)
dr.text((20,15),'I2 real topology / full road, footway and terrain width / native XZ',font=F,fill='black');im.save(M/'topology-wireframe.png')
print('EVIDENCE_PLAN_SECTIONS_WIREFRAME',len(out))
changed_faces=[]
for row in rows:
 a=np.load(D/(row['key']+'-source.npz'));v=a['v'];t=a['tri'];changed=abs(v[:,1]-a['before'][:,1])>1e-6;sel=np.any(changed[t],axis=1)
 if sel.any():changed_faces.append(v[t[sel]].reshape(-1,3))
vv=np.vstack(changed_faces);lo=vv[:,[0,2]].min(0);hi=vv[:,[0,2]].max(0);(D/'changed-envelope.tsv').write_text('\t'.join(str(float(x)) for x in [lo[0],lo[1],hi[0],hi[1]]));(D/'changed-face-envelope.json').write_text(json.dumps(dict(min_xz=lo.tolist(),max_xz=hi.tolist(),method='All vertices of faces incident on a changed-height vertex; not only the edit-point mask'),indent=2))
