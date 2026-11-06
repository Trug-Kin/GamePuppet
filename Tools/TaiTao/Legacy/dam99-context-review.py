import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99'
P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'))
before={}
for o in bpy.context.scene.objects:
 if o.type=='MESH':
  before[o.name]=(np.array([o.matrix_world@v.co for v in o.data.vertices]),[tuple(p.vertices) for p in o.data.polygons])
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
def pos(s,d,h):
 x,z=A+T*s+N*d;return Vector((-x,-z,h))
delta=[];trees={};attrs=[]
for o in sc.objects:
 if o.type!='MESH':continue
 v=np.array([o.matrix_world@p.co for p in o.data.vertices]);faces=[tuple(p.vertices) for p in o.data.polygons]
 if o.name in before:
  old,oldfaces=before[o.name];topo=old.shape!=v.shape or oldfaces!=faces
  changed=topo or np.max(np.abs(v-old),initial=0)>.0001
  if changed:delta.append(dict(object=o.name,path=o.get('dam99_source_path','') or o.get('unity_path',''),topology_changed=topo,vertices=len(v),uv_layers=[u.name for u in o.data.uv_layers],colors=[u.name for u in o.data.color_attributes],hide_render=o.hide_render,material_names=[m.name if m else None for m in o.data.materials],old_5p04_changed=int(np.sum((np.abs(old[:,2]-5.04)<.001)&(np.linalg.norm(old-v,axis=1)>.0001))) if not topo and o.name=='BankR91_Grade_000' else None))
 if not len(v) or o.hide_render or any(c.hide_render for c in o.users_collection):continue
 if o.name=='C95_0103_Surface_051' or 'WATER' in o.name:continue
 trees[o.name]=BVHTree.FromPolygons([Vector(p) for p in v],faces)
out=[]
for tag,ss,ds in [('deck longitudinal',np.linspace(-22,L+22,405),[-13,-7,-1]),('far lower return',np.linspace(-18,-3,121),[4.0]),('gate lower return',np.linspace(L+3,L+18,121),[4.0])]:
 for d in ds:
  pts=[]
  for s in ss:
   hits=[]
   for name,t in trees.items():
    p,n,i,dist=t.ray_cast(pos(s,d,8),Vector((0,0,-1)),12)
    if p and n.z>.7:hits.append((float(p.z),name))
   # Exclude lamps/rail/props for road profile, retain layers in raw data.
   surfaces=[p for p in hits if not any(k in p[1] for k in ['Surface_1','Surface_2','Surface_3','Surface_4','Surface_5','Surface_6','Surface_7','Surface_8','RAIL','GRILLE']) or 'BankR91' in p[1]]
   pts.append(dict(s=float(s),d=d,hits=sorted(hits,reverse=True)[:8]))
  out.append(dict(tag=tag,d=d,points=pts))
(D/'candidate-actual-delta.json').write_text(json.dumps(delta,indent=2));(D/'context-surface-profiles.json').write_text(json.dumps(out,indent=2))
cam=sc.camera.copy();cam.data=sc.camera.data.copy();sc.collection.objects.link(cam);sc.camera=cam;cam.data.type='PERSP';cam.data.lens=28
for tag,eye,at in [('bridge-landward',(L/2,-30,14),(L/2,-4,2)),('far-approach',(-23,-8,4),(-1,-5,2.8)),('gate-approach',(L+24,-6,5),(L+1,-5,2.8)),('gate-bank-return',(L+9,9,2),(L+1,3,1)),('far-bank-return',(-12,7,2),(-2,3,1))]:
 cam.location=pos(*eye);cam.rotation_euler=(pos(*at)-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('context-'+tag+'.png'));bpy.ops.render.render(write_still=True)
print('RESULT actual changed objects',len(delta),'market',[x['old_5p04_changed'] for x in delta if x['object']=='BankR91_Grade_000'],flush=True)
