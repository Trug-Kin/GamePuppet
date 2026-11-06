import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector,geometry
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
placements=json.loads((R/'Docs/Reviews/journey_v48/r22/flower-placements.json').read_text());reports=json.loads((D/'supplemental-flower-placement.json').read_text())
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'))
bed=bpy.data.objects['C95_0045_OD35_Surface_5'];old=[bed.matrix_world@v.co for v in bed.data.vertices];faces=[tuple(p.vertices) for p in bed.data.polygons];tree=BVHTree.FromPolygons(old,faces)
bpy.ops.wm.open_mainfile(filepath=str(P));assert not bpy.context.scene.get('dam99_flowers_surface_mapped',False),'Already mapped'
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P.parent/'Stages/Before_Flower_Surface_Mapping.blend'),copy=True,compress=True)
captured=next(x for x in json.loads((D/'throat-before-geometry.json').read_text()) if x['name']=='C95_0045_OD35_Surface_5')
# The source throat cut reindexed vertices. This capture retains the original
# 600-vertex soil order after its authored displacement and before the cut.
new=[Vector((-x,-z,y)) for x,y,z in captured['vertices_unity']]
assert len(old)==len(new),(len(old),len(new))
print('SOIL_VERTEX_ORDER',len(old),'pre-cut source correspondence retained',flush=True)
out=[]
for color in ['Red','Yellow']:
 rows=[x for x in placements if bool(x['yellow'])==(color=='Yellow')];rep=next(x for x in reports if x['object']=='D99_FLOWERS_'+color+'_0')
 for move in rep['moved']:
  i=move['placement'];root=np.array(rows[i]['position']);x,y,z=root;hit,n,fi,dist=tree.ray_cast(Vector((-x,-z,y+1)),Vector((0,0,-1)),3)
  if not hit or n.z<.5:raise RuntimeError(('original soil unavailable',color,i))
  face=faces[fi];found=False
  for j in range(1,len(face)-1):
   ids=(face[0],face[j],face[j+1]);a,b,c=[old[k] for k in ids];ab=np.array(b-a);ac=np.array(c-a);uv=np.linalg.lstsq(np.column_stack([ab,ac]),np.array(hit-a),rcond=None)[0]
   if min(uv)>=-1e-4 and sum(uv)<=1.0001:
    target=geometry.barycentric_transform(hit,a,b,c,*[new[k] for k in ids]);found=True;break
  assert found,(color,i,fi)
  target+=Vector((0,0,y-hit.z));native=np.array([-target.x,target.z,-target.y]);prior=root+np.array(move['shift']);delta=native-prior
  for part in range(3):
   o=bpy.data.objects['D99_FLOWERS_'+color+'_'+str(part)];step=len(o.data.vertices)//len(rows)
   for v in o.data.vertices[i*step:(i+1)*step]:v.co+=Vector((-delta[0],-delta[2],delta[1]))
   o.data.update()
  out.append(dict(color=color,index=i,prior_root=prior.tolist(),target_root=native.tolist(),delta=delta.tolist(),soil_face=fi,original_soil_gap=float(y-hit.z),support='C95_0045_OD35_Surface_5'))
bpy.context.scene['dam99_flowers_surface_mapped']=True
bpy.context.scene['dam99_supplemental_flowers']='Original plant roots mapped barycentrically from actual R98 planter soil to corresponding R99 soil faces. Each plant rigid, original soil penetration preserved, other plants unchanged.'
bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'flower-support-correction.json').write_text(json.dumps(out,indent=2));print('RESULT',len(out),'plants mapped to corresponding editable soil faces',flush=True)
