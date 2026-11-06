import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
placements=json.loads((R/'Docs/Reviews/journey_v48/r22/flower-placements.json').read_text());reports=json.loads((D/'supplemental-flower-placement.json').read_text());trees=[]
corrected={(x['color'],x['index']):x['target_root'] for x in json.loads((D/'flower-support-correction.json').read_text())} if (D/'flower-support-correction.json').exists() else {}
removed={(x['color'],x['index']) for x in json.loads((D/'removed-stair-throat-plants.json').read_text())} if (D/'removed-stair-throat-plants.json').exists() else set()
for o in bpy.context.scene.objects:
 if o.type!='MESH' or o.hide_render or any(c.hide_render for c in o.users_collection) or o.name.startswith('D99_FLOWERS') or o.name.startswith('R99_SUPPORT'):continue
 v=[o.matrix_world@p.co for p in o.data.vertices]
 if not v:continue
 lo=Vector(tuple(min(p[k] for p in v) for k in range(3)));hi=Vector(tuple(max(p[k] for p in v) for k in range(3)))
 trees.append((o.name,lo,hi,BVHTree.FromPolygons(v,[tuple(p.vertices) for p in o.data.polygons])))
out=[]
for key in ['Red','Yellow']:
 r=next(x for x in reports if x['object']=='D99_FLOWERS_'+key+'_0');p=[x for x in placements if bool(x['yellow'])==(key=='Yellow')];o=bpy.data.objects[r['object']];v=np.array([o.matrix_world@x.co for x in o.data.vertices]);step=len(v)//len(p)
 for move in r['moved']:
  i=move['placement']
  if (key,i) in removed:continue
  root=np.array(corrected.get((key,i),np.array(p[i]['position'])+move['shift']));x,y,z=root
  ids=o.data.attributes.get('r22_plant_index');bottom=float(v[np.array([a.value for a in ids.data])==i,2].min()) if ids else float(v[i*step:(i+1)*step,2].min());hits=[]
  for name,lo,hi,tr in trees:
   if not (lo.x<=-x<=hi.x and lo.y<=-z<=hi.y):continue
   q,n,f,d=tr.ray_cast(Vector((-x,-z,y+1)),Vector((0,0,-1)),3)
   if q and n.z>.5:hits.append(dict(object=name,height=float(q.z),root_gap=float(y-q.z),mesh_bottom_gap=float(bottom-q.z)))
  out.append(dict(color=key,index=i,root=root.tolist(),stem_mesh_min=bottom,mode=move['mode'],supports=sorted(hits,key=lambda h:abs(h['root_gap']))[:3]))
(D/'flower-support-audit.json').write_text(json.dumps(out,indent=2));print('RESULT',len(out),'plant roots checked',flush=True)
