import sys,json,math
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from scipy.spatial import cKDTree
from PIL import Image,ImageDraw,ImageFont
D=Path('Test/KetQua/journey_v48/i2_r93_complete');M=Path('Test/BangChung/journey_v48/i2_r93_complete');cx,cz=714.94,459.48
exec(Path('Test/Tam/i2-overlap.py').read_text().split('g=np.load')[0].split('D=Path')[1].split('class Surf:')[0] if False else 'pass')
code=Path('Test/Tam/i2-metrics.py').read_text();exec('class Surf:'+code.split('class Surf:')[1].split('def clean')[0])
rows=json.loads((D/'retopology.json').read_text());S={};O={};bounds={};out=[]
for row in rows:
 name=row['path'].split('/')[-1];a=np.load(D/(row['key']+'-source.npz'));b=np.load(D/(row['native_key']+'.npz'));v=a['v'].astype(float);tri=a['tri'];S[name]=Surf(v,tri);O[name]=Surf(b['v'].astype(float),b['tri']);q=v[tri];center=q.mean(1);r=np.linalg.norm(center[:,[0,2]]-[cx,cz],axis=1);dy=v[:,1]-a['before'][:,1];affected=np.any(abs(dy[tri])>1e-5,axis=1)
 n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);area=abs(n[:,1])/2;gr=np.linalg.norm(n[:,[0,2]],axis=1)/np.maximum(abs(n[:,1]),1e-30)*100;sel=(r<40)&(center[:,2]>cz-9);finite=sel&(area>.01);changed=finite&affected
 ix=np.flatnonzero(changed);ix=ix[np.argsort(gr[ix])[-15:]];stats=dict(name=name,changed_max_grade=float(gr[changed].max(initial=0)),changed_area=float(area[affected].sum()),changed_worst=[dict(xyz=center[i].tolist(),grade=float(gr[i]),area=float(area[i])) for i in ix],degenerate_3d=int((np.linalg.norm(n[sel],axis=1)<1e-10).sum()),wrong_winding=int((n[sel,1]>1e-10).sum()))
 # Footprint boundary, sampled independently of mesh topology.
 clip=shapely.box(cx-42,cz-10,cx+42,cz+44);local=(r<60)&(center[:,2]>cz-20);poly=shapely.union_all(shapely.set_precision(shapely.polygons(q[local][:,:,[0,2]]),.00001));poly=shapely.intersection(poly,clip)
 poly=shapely.union_all([g for g in shapely.get_parts(poly) if g.geom_type in ['Polygon','MultiPolygon']]);bd=poly.boundary;dist=np.arange(0,bd.length,.15);pp=shapely.get_coordinates(shapely.line_interpolate_point(bd,dist));ok=(np.linalg.norm(pp-[cx,cz],axis=1)<39)&(pp[:,1]>cz-8);pp=pp[ok];hh,_=S[name].query(pp);hb,_=O[name].query(pp);bounds[name]=(pp,hh,hb);stats['boundary_sampled']=len(pp);out.append(stats)
junctions=[]
render=['Surface_128','Surface_129','Surface_142','Surface_176','Surface_153']
for i,rn in enumerate(render):
 for cn in render[i+1:]:
  pp,rh,rb=bounds[rn];cp,ch,cb=bounds[cn];dist,ix=cKDTree(cp).query(pp);ok=(dist<.03)&np.isfinite(rh)&np.isfinite(ch[ix]);ids=np.flatnonzero(ok)
  if not len(ids):continue
  d=rh[ids]-ch[ix[ids]];db=rb[ids]-cb[ix[ids]];worst=ids[np.argsort(abs(d))[-12:]];junctions.append(dict(a=rn,b=cn,samples=len(ids),max_after=float(abs(d).max()),max_before=float(np.nanmax(abs(db))),worst=[dict(xz=pp[k].tolist(),xz_gap=float(dist[k]),after=float(rh[k]-ch[ix[k]]),before=float(rb[k]-cb[ix[k]])) for k in worst]))
main=[]
for x in [cx-2,cx,cx+2]:
 p=np.c_[np.full(301,x),np.linspace(cz+13,cz+28,301)];ha,_=S['Surface_128'].query(p);hb,_=O['Surface_128'].query(p);main.append(dict(x=x,length=15,max_grade_before=float(np.nanmax(abs(np.diff(hb)/.05*100))),max_grade_after=float(np.nanmax(abs(np.diff(ha)/.05*100)))))
p=np.array([[cx,473.48],[cx,473.98]]);ha,_=S['Surface_128'].query(p);hb,_=O['Surface_128'].query(p)
result=dict(status='REVIEW_NOT_AUTO_ACCEPT',main_kink=dict(xz=p.tolist(),before=hb.tolist(),after=ha.tolist(),before_grade=float(np.diff(hb)[0]/.5*100),after_grade=float(np.diff(ha)[0]/.5*100)),full_width_main=main,layers=out,interfaces=junctions)
(D/'full-width-gate.json').write_text(json.dumps(result,indent=2));print(json.dumps(dict(main=result['main_kink'],full_width=main,layers=[{k:r[k] for k in ['name','changed_max_grade','wrong_winding','degenerate_3d']} for r in out],interfaces=[{k:r[k] for k in ['a','b','samples','max_after','max_before']} for r in junctions]),indent=2))
