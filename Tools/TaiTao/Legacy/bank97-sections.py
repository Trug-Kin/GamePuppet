import bpy,json,numpy as np
from pathlib import Path
from mathutils.bvhtree import BVHTree
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R96/Market_Ground_R96_Editable.blend'))
trees=[]
for o in list(bpy.context.scene.objects):
 k=o.get('market95_export_key','');p=o.get('unity_path','')
 if o.type!='MESH' or not k or len(o.data.polygons)<2 or len(o.data.polygons)>500000:continue
 m=np.array(o.matrix_world,dtype=float);v=np.array([x.co for x in o.data.vertices],dtype=float)@m[:3,:3].T+m[:3,3];u=np.c_[-v[:,0],v[:,2],-v[:,1]];t=[tuple(p.vertices) for p in o.data.polygons]
 if u[:,0].min()>720 or u[:,0].max()<630 or u[:,2].min()>490 or u[:,2].max()<405:continue
 if k.startswith('M95_P') or k in ['M95_ClockFoot']:continue
 tree=BVHTree.FromPolygons([Vector(q) for q in v],t)
 trees.append((k,p,tree));np.savez_compressed(D/(k+'-existing.npz'),v=u,tri=np.array(t),native=v)
rows=[]
for z in [412,426,440,450,460,470,480,488]:
 for x in np.arange(635,716,2):
  hits={}
  for k,p,tr in trees:
   a,n,idx,d=tr.ray_cast(Vector((-x,-z,35)),Vector((0,0,-1)),60)
   if a is not None:hits[k]=round(a.z,4)
  rows.append(dict(x=float(x),z=z,h=hits))
(D/'existing-section-layers.json').write_text(json.dumps(rows,indent=2));print('RESULT',len(trees),len(rows),flush=True)
