import bpy,json,math
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/ground_r93';M=R/'Test/BangChung/journey_v48/ground_r93';P=R/'Source/Blender/JourneyV48/R93/Ground_I2_Transition_Trial_R93.blend'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R93/GroundSurvey_R93_Editable.blend'));sc=bpy.context.scene;col=bpy.data.collections['GROUND_NATIVE_MATCHED_R93'];before=bpy.data.collections.new('BEFORE_R93_DO_NOT_EXPORT');sc.collection.children.link(before);before.hide_viewport=True;before.hide_render=True
cx,cz=714.94,459.48;grid=np.load(D/'layer-grid.npz');h=grid['L159'];xx,zz=np.meshgrid(grid['xs'],grid['zs']);sel=np.isfinite(h)&(np.hypot(xx-cx,zz-cz)<12.5);plane=np.linalg.lstsq(np.c_[np.ones(sel.sum()),xx[sel]-cx,zz[sel]-cz],h[sel],rcond=None)[0]
ref=[]
for name,offset in [('Surface_128',0),('Surface_129',.015)]:
 o=next(o for o in col.objects if o.get('unity_path','').endswith('/'+name));o.data.calc_loop_triangles();v=[o.matrix_world@v.co for v in o.data.vertices];tri=[list(t.vertices) for t in o.data.loop_triangles];flat=[Vector((p.x,p.y,0)) for p in v];ref.append((BVHTree.FromPolygons(flat,tri,all_triangles=True),BVHTree.FromPolygons(v,tri,all_triangles=True),np.array(v),np.array(tri),offset))
oldfield=np.load(R/'Docs/Reviews/journey_v48/r88/bridge-field.npz');den=(oldfield['desired']-oldfield['before_reference'])*oldfield['weight'];factor=np.divide(oldfield['delta'],den,out=np.ones_like(den),where=abs(den)>.002);factor=np.clip(factor,0,1)
def smooth(v):v=max(0,min(1,v));return v*v*(3-2*v)
def pin(x,z):
 u=(x-oldfield['xs'][0])*2;v=(z-oldfield['zs'][0])*2;i=max(0,min(factor.shape[1]-2,int(u)));j=max(0,min(factor.shape[0]-2,int(v)));a=max(0,min(1,u-i));b=max(0,min(1,v-j));return factor[j,i]*(1-a)*(1-b)+factor[j,i+1]*a*(1-b)+factor[j+1,i]*(1-a)*b+factor[j+1,i+1]*a*b
def reference(x,z):
 hits=[]
 for tree,real,vs,ts,off in ref:
  top,normal,face,distance=real.ray_cast(Vector((-x,-z,40)),Vector((0,0,-1)),80)
  if top is not None:
   hits.append((0,float(top.z)-off));continue
  p,n,i,dist=tree.find_nearest(Vector((-x,-z,0)))
  if p is None:continue
  q=vs[ts[i]];a,b,c=q;den=(b[1]-c[1])*(a[0]-c[0])+(c[0]-b[0])*(a[1]-c[1])
  if abs(den)<1e-12:continue
  u=((b[1]-c[1])*(p.x-c[0])+(c[0]-b[0])*(p.y-c[1]))/den;v=((c[1]-a[1])*(p.x-c[0])+(a[0]-c[0])*(p.y-c[1]))/den;y=u*a[2]+v*b[2]+(1-u-v)*c[2]-off;hits.append((dist,y))
 if not hits:return None
 closest=min(q[0] for q in hits);height=max(q[1] for q in hits if q[0]<closest+.005);return closest,height
def delta(x,y,z,terrain=False):
 r=math.hypot(x-cx,z-cz)
 if r<=13.2 or r>=24:return 0
 rr=reference(x,z)
 if rr is None:return 0
 dist,old=rr;w=smooth((r-13.2)/.8)*smooth((24-r)/3)*smooth((6-dist)/3)
 if terrain:w*=smooth(y/.3)
 t=smooth((r-13.35)/(24-13.35));inner=plane[0]+plane[1]*(x-cx)+plane[2]*(z-cz)-.04;outer=.72-.004*(x-cx)-.004*(z-cz);target=inner*(1-t)+outer*t
 return (target-.04-y if terrain else target-old)*w
reports=[]
for o in list(col.objects):
 if o.type!='MESH':continue
 path=o.get('unity_path','');native=np.array([o.matrix_world@v.co for v in o.data.vertices]);world=np.c_[-native[:,0],native[:,2],-native[:,1]];near=(np.hypot(world[:,0]-cx,world[:,2]-cz)>13.2)&(np.hypot(world[:,0]-cx,world[:,2]-cz)<24)
 if not near.any():continue
 terrain=path.endswith(('Surface_176','Surface_142','Context_124'));dy=np.zeros(len(world))
 for i in np.flatnonzero(near):dy[i]=delta(*world[i],terrain=terrain)
 changed=abs(dy)>1e-6
 if not changed.any():continue
 orig=o.copy();orig.data=o.data.copy();orig.name='BEFORE_'+o.name;before.objects.link(orig);orig.hide_render=True
 oldnorm=np.array([list(n.vector) for n in o.data.corner_normals]);mi=o.matrix_world.inverted()
 for i in np.flatnonzero(changed):p=native[i].copy();p[2]+=dy[i];o.data.vertices[i].co=mi@Vector(p)
 o.data.update();o.data.calc_loop_triangles();tri=np.array([t.vertices for t in o.data.loop_triangles]);local=np.array([v.co for v in o.data.vertices]);q=local[tri];nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);acc=np.zeros_like(local)
 for j in range(3):np.add.at(acc,tri[:,j],nn)
 acc/=np.maximum(np.linalg.norm(acc,axis=1)[:,None],1e-12);norm=oldnorm.copy()
 for l in o.data.loops:
  if changed[l.vertex_index]:norm[l.index]=acc[l.vertex_index]
 o.data.normals_split_custom_set(norm.tolist());o['trial_correction']='I2 annulus 13.2..24; retain island, bridge, old r88 pins and outer edge; smooth intermediate road transition; no survey accuracy claim';o['trial_changed_vertices']=int(changed.sum());o['trial_min_dy']=float(dy.min());o['trial_max_dy']=float(dy.max());reports.append(dict(path=path,object=o.name,changed_vertices=int(changed.sum()),delta_range=[float(dy.min()),float(dy.max())],changed_bounds=[world[changed].min(0).tolist(),world[changed].max(0).tolist()]))
sc.name='I2_GROUND_TRANSITION_TRIAL_NOT_INTEGRATED';sc['status']='UNACCEPTED TRIAL. Smooth proven road kink while retaining the inherited tilted island, bridge and footprints. Old r88 pin weighting is removed from road correction; prop contacts require separate verification. Requires surface sections, contacts and Unity validation before integration.';sc['island_plane']=plane.tolist();sc['trial_curbs']='0.04 authoring units at inner road boundary is a design estimate, not a measured curb.'
# Diagnostic close views, no buildings/props omitted from images may be inferred absent in product.
cam=sc.camera;cam.data.type='ORTHO';cam.data.ortho_scale=65;cam.location=(-714.94,-459.48,90);cam.rotation_euler=(0,0,0);sc.display.shading.color_type='OBJECT'
for o in col.objects:
 o.color=(.55,.63,.7,1) if 'Surface_128' in o.get('unity_path','') else (.35,.43,.32,1)
 if o.get('unity_path','').startswith('LakeGateMarketV32/Context_'):o.hide_render=True
sc.render.filepath=str(M/'I2-trial-plan.png');bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.render.render(write_still=True)
out=dict(status='TRIAL_NOT_INTEGRATED',source=str(P),plane_island_y_dx_dz=plane.tolist(),inner_radius=13.2,outer_radius=24,preserve='island footprint and geometry; bridge; outside radius24; source UV/topology/materials',changed_method='Old r88 pin attenuation removed from correction; prop contacts need explicit validation.',inner_curb_estimate=.04,objects=reports)
(D/'transition-trial.json').write_text(json.dumps(out,indent=2));print('TRIAL',len(reports),'meshes',flush=True)
