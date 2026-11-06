import bpy,ast,json,numpy as np,math,hashlib
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';M=R/'Test/BangChung/journey_v48/map_r99_closeout';P=R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'
assert not P.exists(),'Never overwrite derivative'
t=ast.parse((R/'Test/Tam/map100-contact-audit.py').read_text());exec(compile(ast.Module(body=[x for x in t.body if isinstance(x,ast.FunctionDef)],type_ignores=[]),'helpers','exec'))
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'))
A=np.array([637.7344,528.2603]);T=np.array([.76346655,-.64584737]);N=np.array([-.64584737,-.76346655]);sc=bpy.context.scene
before={o.name:hashlib.sha256(np.array([v.co[:] for v in o.data.vertices],dtype=np.float32).tobytes()).hexdigest() for o in sc.objects if o.type=='MESH'}
pv=bpy.data.objects['C95_0040_OD35_Surface_0'];tree=BVHTree.FromPolygons([pv.matrix_world@v.co for v in pv.data.vertices],[tuple(p.vertices) for p in pv.data.polygons])
rows=json.loads((D/'gate-components.json').read_text());base=next(x for x in rows if x['object']=='C95_0043_OD35_Surface_3' and x['count']==72 and x['y_min']<3.01);o=bpy.data.objects[base['object']];v=np.array([o.matrix_world@p.co for p in o.data.vertices]);bottom=v[base['indices']];bottom=bottom[bottom[:,2]<3.001];heights=[]
for q in bottom:
 hit,n,_,_=tree.ray_cast(Vector((q[0],q[1],4)),Vector((0,0,-1)),3);assert hit and n.z>.9;heights.append(float(hit.z))
delta=min(heights)-.002-base['y_min'];assert -.08<delta<-.04
changed=[]
for name in ['C95_0043_OD35_Surface_3','C95_0044_OD35_Surface_4']:
 o=bpy.data.objects[name];v,groups=components(o);ids=[]
 for group in groups:
  q=v[group];mid=(q.min(0)+q.max(0))*.5;s,d=(-mid[:2]-A)@np.c_[T,N]
  if 58.5<s<60 and -1.5<d<-.25:ids.extend(group)
 assert len(ids)==(612 if name.endswith('_3') else 1800),(name,len(ids))
 shift=o.matrix_world.inverted().to_3x3()@Vector((0,0,delta))
 for ix in ids:o.data.vertices[ix].co+=shift
 o.data.update();o['map100_change']='Rigid gate lamp reseat only; unchanged XY/shape; no collider exists on this decorative assembly.';changed.append(dict(object=name,vertices=len(ids),vertical_delta=delta))
changes=[]
for o in sc.objects:
 if o.type=='MESH' and hashlib.sha256(np.array([v.co[:] for v in o.data.vertices],dtype=np.float32).tobytes()).hexdigest()!=before[o.name]:changes.append(o.name)
assert sorted(changes)==sorted(x['object'] for x in changed)
bpy.data.collections['DAM_R99_EDITABLE_SECTION_AND_END_TIES'].name='DAM_R100_EDITABLE_SECTION_AND_END_TIES'
sc['map100_scope']='Gate lamp rigid reseat. R99 flower editable meshes unchanged; Unity needs fresh meshes/GPU upload. Water, market, all walking surfaces and colliders unchanged.'
P.parent.mkdir(parents=True,exist_ok=True);bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True)
# Real reopen of saved editable source before any export.
bpy.ops.wm.open_mainfile(filepath=str(P));assert bpy.context.scene.unit_settings.scale_length==1;assert not bpy.data.libraries
missing=[im.filepath for im in bpy.data.images if not im.packed_file and im.source not in ['GENERATED','VIEWER'] and not Path(bpy.path.abspath(im.filepath)).exists()];assert not missing
sc=bpy.context.scene;cam=sc.camera;cam.data.lens=32;sc.render.resolution_x=1280;sc.render.resolution_y=800;sc.render.resolution_percentage=100
def bpos(v):return Vector((-v[0],-v[2],v[1]))
for tag,eye,at in [('lamp-source-r100',(685.53369,4.27,492.63019),(683.53369,3.12,490.63019)),('gate-source-r100',(683.53369,15.12,490.53019),(683.53369,3.12,490.63019)),('cluster-source-r100',(666,11,479),(659,1.5,515))]:
 cam.location=bpos(eye);cam.rotation_euler=(bpos(at)-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/(tag+'.png'));bpy.ops.render.render(write_still=True)
(D/'source-delta.json').write_text(json.dumps(dict(source=str(P),reopened=True,unit_scale=1,missing_images=missing,linked_libraries=0,changed=changed,unchanged_mesh_objects=len(before)-len(changes),base_before=base['y_min'],paving_min=min(heights),paving_max=max(heights),base_after=base['y_min']+delta,post_contact_gap_range=[min(heights)-.002-max(heights),-.002],flower_geometry_change=False,walking_surface_change=False,collider_change=False),indent=2));print('RESULT',json.dumps(changed),flush=True)
