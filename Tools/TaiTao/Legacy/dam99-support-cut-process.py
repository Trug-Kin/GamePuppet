from pathlib import Path
import sys,ast,json
sys.path.insert(0,'Tools/python-deps')
import numpy as np
from shapely.geometry import LineString,box
from shapely.ops import unary_union,polygonize,triangulate
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';rows=json.loads((D/'support-throat-before.json').read_text());A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
tree=ast.parse((R/'Test/Tam/bank97-author.py').read_text(encoding='utf-8-sig'));exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in ['half','difference']],type_ignores=[]),'clip','exec'))
def position(s,d,h):
 x,z=A+T*s+N*d;return np.array([x,h,z])
out=[];stats=[]
for row in rows:
 lo=L if row['name']=='R99_SUPPORT_COL032' else -3.0
 quad=np.array([position(lo,0,0)[[0,2]],position(lo+3,0,0)[[0,2]],position(lo+3,4.5,0)[[0,2]],position(lo,4.5,0)[[0,2]]])
 polygons=[];changed=0
 for poly in row['polygons']:
  arr=[np.array(p) for p in poly['a']];pp=np.array(arr);sd=(pp[:,[0,2]]-A)@np.c_[T,N]
  if sd[:,0].max()<lo or sd[:,0].min()>lo+3 or sd[:,1].max()<0 or sd[:,1].min()>4.5:polygons.append(poly);continue
  parts=difference(arr,quad)
  def area(a):return sum(np.linalg.norm(np.cross(a[i][:3]-a[0][:3],a[i+1][:3]-a[0][:3]))*.5 for i in range(1,len(a)-1))
  removed=area(arr)-sum(area(p) for p in parts)
  if removed<1e-8:polygons.append(poly);continue
  changed+=1
  for part in parts:polygons.append(dict(a=[p.tolist() for p in part],slot=poly['slot'],smooth=poly['smooth']))
 # Intersect true source triangles with each cut plane and polygonize cross sections.
 v=np.array(row['vertices_unity']);sd=(v[:,[0,2]]-A)@np.c_[T,N];q3=np.c_[sd,v[:,1]];tris=q3[np.array(row['triangles'])];cap_stats=[]
 for axis,level,normal_sign,other_bounds in [(0,lo,1,(0,4.5)),(0,lo+3,-1,(0,4.5)),(1,0,1,(lo,lo+3)),(1,4.5,-1,(lo,lo+3))]:
  other=1-axis;segments=[]
  for tri in tris:
   delta=tri[:,axis]-level
   if delta.min()>1e-7 or delta.max()<-1e-7 or np.abs(delta).max()<1e-7:continue
   pts=[]
   for i,j in [(0,1),(1,2),(2,0)]:
    a,b=tri[i],tri[j];da,db=delta[i],delta[j]
    if abs(da)<1e-7:pts.append(a)
    if da*db<0:pts.append(a+(b-a)*(-da)/(db-da))
   if len(pts)<2:continue
   pts=np.unique(np.round(np.array(pts)[:,[other,2]],4),axis=0)
   if len(pts)>=2:
    far=max(((np.linalg.norm(a-b),a,b) for i,a in enumerate(pts) for b in pts[i+1:]),key=lambda x:x[0]);
    if far[0]>1e-7:segments.append(LineString([far[1],far[2]]))
  polys=list(polygonize(unary_union(segments)));area_added=0;count=0
  for cap in polys:
   cap=cap.intersection(box(other_bounds[0],-2,other_bounds[1],12))
   if cap.is_empty:continue
   caps=[cap] if cap.geom_type=='Polygon' else list(cap.geoms)
   for part in caps:
    if part.geom_type!='Polygon' or part.area<1e-8:continue
    captris=[t for t in triangulate(part) if part.covers(t)]
    assert abs(sum(t.area for t in captris)-part.area)<1e-5,(row['name'],'cap coverage')
    for tr in captris:
     coords=np.array(tr.exterior.coords)[:3];arr=[]
     for a,h in coords:
      s,d=(level,a) if axis==0 else (a,level);p=position(s,d,h);arr.append(np.r_[p,np.tile([a*.3,h*.3],len(row['uvs']))])
     native=np.array([[-a[0],-a[2],a[1]] for a in arr]);n=T if axis==0 else N;normal=np.array([-n[0],-n[1],0])*normal_sign
     if np.dot(np.cross(native[1]-native[0],native[2]-native[0]),normal)<0:arr.reverse()
     polygons.append(dict(a=[p.tolist() for p in arr],slot=0,smooth=False,cap=True));count+=1
    area_added+=part.area
  cap_stats.append(dict(axis=axis,level=level,polygons=len(polys),triangles=count,area=area_added))
 polygons=[dict(poly,a=[poly['a'][0],poly['a'][i],poly['a'][i+1]]) for poly in polygons for i in range(1,len(poly['a'])-1)]
 out.append(dict(name=row['name'],uvs=row['uvs'],colors=row['colors'],polygons=polygons));stats.append(dict(object=row['name'],polygons_before=len(row['polygons']),polygons_after=len(polygons),changed_faces=changed,caps=cap_stats))
(D/'support-throat-after.json').write_text(json.dumps(out));(D/'support-throat-cut.json').write_text(json.dumps(stats,indent=2));print(json.dumps(stats,indent=2))

