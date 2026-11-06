import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely.geometry import Polygon
from shapely.ops import unary_union
D=Path('Test/KetQua/journey_v48/ground_r93');stats=json.loads((D/'geometry-inventory.json').read_text());out=[]
for path in ['R20_Whole_Source_Export/Surface_104','R20_Whole_Source_Export/Surface_128','R20_Whole_Source_Export/RoadLoop_125314010_R65']:
 r=next(x for x in stats if x['path']==path);z=np.load(D/(r['cache']+'.npz'));p=z['v'][z['tri']][:,:,[0,2]];area=np.abs(np.cross(p[:,1]-p[:,0],p[:,2]-p[:,0]))/2;p=p[area>1e-6];shape=unary_union([Polygon(t) for t in p]);polys=list(shape.geoms) if hasattr(shape,'geoms') else [shape]
 for poly in polys:
  for ring in poly.interiors:
   po=Polygon(ring)
   if po.area>3:out.append({'path':path,'area':po.area,'bounds':list(po.bounds),'center':list(po.centroid.coords)[0],'outline':list(ring.coords)})
(D/'road-holes.json').write_text(json.dumps(out,indent=2));print([(r['path'],round(r['area'],1),[round(v,2) for v in r['center']]) for r in out])
