import sys,json,math,struct,hashlib
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from PIL import Image,ImageDraw,ImageFont
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/ground_r93';M=R/'Test/BangChung/journey_v48/ground_r93';M.mkdir(parents=True,exist_ok=True)
code=(R/'Docs/Reviews/journey_v48/r74/mesh_io.py').read_text();exec(code.replace("O=Path(__file__).parent","O=D").replace('r73-source.meshbin','scene-ground.meshbin'))
rows=read_source();exec(code.replace("O=Path(__file__).parent","O=D").replace('r73-source.meshbin','scene-colliders.meshbin'));rows+=read_source();inv={r['path']:r for r in json.loads((D/'inventory.json').read_text(encoding='utf-8'))['rows']+json.loads((D/'collider-inventory.json').read_text(encoding='utf-8'))['rows']}
def role(r):
 p=r['path'];n=p.split('/')[-1].replace('#COLLIDER','');mt=' '.join(inv[p]['materials']).lower()
 if '#COLLIDER' in p:return 'collision'
 if 'MarketIsland_R62/' in p:return 'island'
 if 'OD35_' in n or 'Bridge' in p or 'Island_' in n:return 'bridge/island'
 if n in ['Surface_176','Context_124','Restored_029']:return 'terrain'
 if n in ['Surface_104','Surface_173','Surface_128','Surface_129'] or 'RoadLoop' in n:return 'road'
 if n in ['Surface_125','Surface_153','Surface_174','Surface_175','Restored_028'] or 'Footway' in p or 'Paving' in p:return 'footway'
 if any(t in mt for t in ['grass','terrain','road','asphalt','pavement','paving','footway','sidewalk','kerb','curb']):return 'ground-candidate'
 return 'other'
def compact():
 out=[]
 for r in rows:
  meta=inv[r['path']];v=world(r);t=np.vstack(r['sub']);q=v[t];nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);area=np.linalg.norm(nn,axis=1)/2;pa=abs(nn[:,1])/2;flat=(pa>1e-8)&(area>1e-9);grade=np.sqrt(nn[flat,0]**2+nn[flat,2]**2)/np.maximum(abs(nn[flat,1]),1e-9)*100
  rr={'path':r['path'],'asset':r['asset'],'role':role(r),'active':meta['active'],'render':meta['render'],'collider':meta['collider'],'vertices':len(v),'triangles':len(t),'bounds':[v.min(0).tolist(),v.max(0).tolist()],'projected_area':float(pa.sum()),'grade_percentiles':np.percentile(grade,[0,25,50,90,99,100]).tolist() if len(grade) else [],'degenerate':int((area<1e-9).sum()),'horizontal_projected_area_over20percent':float(pa[flat][grade>20].sum())};out.append(rr)
  # Cache exact world-space geometry independently for each layer; never highest-surface only.
  if meta['active'] and (role(r)!='other' or r['path'].startswith(('R20_Whole_Source_Export/Surface_','LakeGateMarketV32/Context_','RouteSurfaces/','R11_'))):
   key='L'+str(len(out)-1).zfill(3);rr['cache']=key;np.savez_compressed(D/(key+'.npz'),v=v,tri=t,uv=r['uv'],slots=np.concatenate([np.full(len(tt),i) for i,tt in enumerate(r['sub'])]))
 (D/'geometry-inventory.json').write_text(json.dumps(out,indent=2));return out
stats=compact()
F=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',22);S=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',16)
im=Image.new('RGB',(1900,2200),'#e8eef3');dr=ImageDraw.Draw(im)
# Preliminary diagram +X right,+Z down. North arrow added only once geo mapping is traced.
def pix(x,z):return (80+(x-600)*4.0,100+(z-100)*4.0)
cols={'terrain':'#d6c7b0','road':'#668da9','footway':'#c0b9a9','island':'#76a56b','bridge/island':'#a29280','ground-candidate':'#a7b594','other':'#cac8c5'}
for role1 in ['terrain','ground-candidate','footway','road','bridge/island','island']:
 for r in rows:
  if role(r)!=role1 or not inv[r['path']]['active'] or not inv[r['path']]['render']:continue
  v=world(r);q=v[np.vstack(r['sub'])];nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);use=(abs(nn[:,1])>.05*np.linalg.norm(nn,axis=1))&(q[:,:,1].mean(1)<20)
  for p in q[use]:dr.polygon([pix(x,z) for x,y,z in p],fill=cols[role1])
for title,x,z in [('market island',783.948,191.167),('bridge island',714.94,459.48),('TTC / Langfarm',724,298),('gate',747,422)]:
 p=pix(x,z);dr.ellipse((p[0]-6,p[1]-6,p[0]+6,p[1]+6),fill='red');dr.text((p[0]+10,p[1]),title,font=F,fill='black',stroke_width=2,stroke_fill='white')
dr.text((30,20),'r93 preliminary exact render layers | X right, +Z down | surveyed elevations UNKNOWN',font=F,fill='black')
im.save(M/'inventory-plan-initial.png')
short=[r for r in stats if r['active'] and r['role']!='other'];print('cached',sum('cache' in r for r in stats),'active classified layers',len(short))
for r in short:
 if r['role'] in ['terrain','road','footway','ground-candidate']:
  print(r['path'],r['role'],'area',round(r['projected_area']),'grade50/90',[round(x,1) for x in r['grade_percentiles'][2:4]])
