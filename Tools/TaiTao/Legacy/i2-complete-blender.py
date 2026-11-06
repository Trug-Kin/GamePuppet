import bpy,bmesh,json,math,sys
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/i2_r93_complete';M=R/'Test/BangChung/journey_v48/i2_r93_complete';M.mkdir(parents=True,exist_ok=True);P=R/'Source/Blender/JourneyV48/R93/Ground_I2_R93_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R93/GroundSurvey_R93_Editable.blend'));sc=bpy.context.scene;col=bpy.data.collections['GROUND_NATIVE_MATCHED_R93'];bc=bpy.data.collections.new('I2_BEFORE_DO_NOT_EXPORT');sc.collection.children.link(bc);bc.hide_render=True;bc.hide_viewport=True
cx,cz=714.94,459.48;plane=[.7439415448549217,-.00900005029080838,-.0429999587064346]
def ss(t):t=np.clip(t,0,1);return t*t*t*(10+t*(-15+6*t))
def field(p,offset):
 p=np.atleast_2d(p);dx=p[:,0]-cx;dz=p[:,2]-cz;r=np.hypot(dx,dz);c=dx/np.maximum(r,1e-10);s=dz/np.maximum(r,1e-10)
 # Hermite heights evaluated at the boundary, never extrapolated island plane.
 ri,ro=13.35,24.;L=ro-ri;t=np.clip((r-ri)/L,0,1);yi=plane[0]+plane[1]*ri*c+plane[2]*ri*s-.04;yo=.72-.004*ro*c-.004*ro*s
 mi=np.zeros_like(t);mo=-.004*c-.004*s
 target=(2*t**3-3*t*t+1)*yi+(t**3-2*t*t+t)*L*mi+(-2*t**3+3*t*t)*yo+(t**3-t*t)*L*mo+offset
 # End at stable source beyond24, full width; no road-nearest lookup that changes branches.
 w=ss((r-11.5)/1.6)*(1-ss((r-24)/6))*ss(dz/7)
 return (target-p[:,1])*w
paths={'Surface_128':0.,'Context_110':0.,'Surface_129':.015,'Context_111':.015,'Surface_153':0.,'Context_136':0.,'Surface_142':-.06,'Surface_176':-.06,'Context_124':-.06}
rows=[]
for o in list(col.all_objects):
 n=o.get('unity_path','').split('/')[-1]
 if n not in paths:continue
 orig=o.copy();orig.data=o.data.copy();orig.name='I2_BEFORE_'+o.name;bc.objects.link(orig);orig.hide_render=True
 me=o.data;bm=bmesh.new();bm.from_mesh(me)
 def relevant(v):
  p=o.matrix_world@v.co;return math.hypot(-p.x-cx,-p.y-cz)<32 and -p.y>cz-1
 near=[v for v in bm.verts if relevant(v)];oldcount=(len(bm.verts),len(bm.faces));bmesh.ops.remove_doubles(bm,verts=near,dist=.00012)
 # Eliminate near-coincident technical fragments without touching distant topology.
 for _ in range(2):
  ed=[e for e in bm.edges if any(relevant(v) for v in e.verts) and e.calc_length()<.0005]
  if ed:bmesh.ops.dissolve_degenerate(bm,dist=.0005,edges=ed)
 bmesh.ops.triangulate(bm,faces=[f for f in bm.faces if any(relevant(v) for v in f.verts)],quad_method='BEAUTY',ngon_method='BEAUTY')
 # Split long triangles before deformation; no vertex edit can travel across an unsplit long face.
 splits=0
 for _ in range(7):
  ed=[e for e in bm.edges if any(relevant(v) for v in e.verts) and e.calc_length()>1.0001]
  if not ed:break
  splits+=len(ed);bmesh.ops.subdivide_edges(bm,edges=ed,cuts=1,use_grid_fill=True)
  bmesh.ops.triangulate(bm,faces=[f for f in bm.faces if len(f.verts)>3],quad_method='BEAUTY',ngon_method='BEAUTY')
 bm.to_mesh(me);bm.free();me.update();me.calc_loop_triangles()
 before=np.array([o.matrix_world@v.co for v in me.vertices]);wp=np.c_[-before[:,0],before[:,2],-before[:,1]];dy=field(wp,paths[n]);changed=abs(dy)>1e-7
 mi=o.matrix_world.inverted()
 for i in np.flatnonzero(changed):p=before[i].copy();p[2]+=dy[i];me.vertices[i].co=mi@Vector(p)
 # Explicitly remove exact zero-area/duplicate faces in changed region, preserve intentional sidewalls.
 bm=bmesh.new();bm.from_mesh(me);bm.faces.ensure_lookup_table();seen=set();bad=[]
 for f in bm.faces:
  if not any(relevant(v) for v in f.verts):continue
  key=tuple(sorted(tuple(round(float(v.co[i]),5) for i in range(3)) for v in f.verts))
  if f.calc_area()<1e-10 or key in seen:bad.append(f)
  else:seen.add(key)
 if bad:bmesh.ops.delete(bm,geom=bad,context='FACES_ONLY')
 bm.to_mesh(me);bm.free();me.update();me.calc_loop_triangles()
 # Remove stale inherited custom normals; derive local smooth normals from the actual edited mesh.
 if me.has_custom_normals:
  me.normals_split_custom_set([(0,0,0)]*len(me.loops))
 for f in me.polygons:f.use_smooth=True
 # Store source position per corner for roundtrip and UV mapping back to the exact source layer.
 # Topology unchanged after zero-face removal; vertex coordinates correspond to wp.
 for name,values in [('I2_SourceIndex',np.c_[np.arange(len(me.vertices)),np.zeros(len(me.vertices))])]:
  prev=me.uv_layers.get(name)
  if prev:me.uv_layers.remove(prev)
  u=me.uv_layers.new(name=name)
  for l in me.loops:u.data[l.index].uv=values[l.vertex_index]
 final=np.array([o.matrix_world@v.co for v in me.vertices]);v=np.c_[-final[:,0],final[:,2],-final[:,1]];tri=np.array([t.vertices for t in me.loop_triangles]);uv=np.array([list(q.uv) for q in me.uv_layers[0].data]);loops=np.array([t.loops for t in me.loop_triangles]);key='I2_'+n;np.savez_compressed(D/(key+'-source.npz'),v=v,tri=tri,before=wp,uv=uv[loops],slots=np.array([t.material_index for t in me.loop_triangles]))
 o['i2_repair']='Single full-width Hermite transition; native editable source; local weld, degenerate/duplicate removal and <=1-unit subdivision; island/bridge geometry unchanged';o['i2_status']='BLENDER_VALIDATION_PENDING';o['i2_offset_estimate']=paths[n];o['i2_before_source']=str(R/'Source/Blender/JourneyV48/R93/GroundSurvey_R93_Editable.blend');o['i2_export_key']=key
 rows.append(dict(path=o.get('unity_path'),object=o.name,key=key,before_counts=oldcount,vertices=len(v),triangles=len(tri),split_edges=splits,removed_zero_duplicate_faces=len(bad),changed_vertices=int(changed.sum()),delta_range=[float(dy.min()),float(dy.max())],changed_bounds=[wp[changed].min(0).tolist(),wp[changed].max(0).tolist()] if changed.any() else None,offset=paths[n]))
 print('COMPLETE_GEOMETRY',n,len(v),len(tri),flush=True)
sc.name='I2_R93_SINGLE_EDITABLE';sc['status']='ONE_WORKING_SOLUTION_PENDING_GATE_NOT_INTEGRATED';sc['i2_boundary']='Inner r13.35 on inherited island plane minus estimated0.04; stable outer r24 road plane0.72-0.004dx-0.004dz; feather to exact original by r30; angular feather over south7 units; layers kept distinct.';sc['source_units']='1 Blender unit =1 authoring metre =1 Unity world unit; NOT surveyed metric accuracy';sc['island_plane']=plane
sc.display.shading.show_shadows=False;sc.display.shading.show_cavity=True
txt=bpy.data.texts.new('I2_README');txt.write('Single working solution. Collections: GROUND_NATIVE_MATCHED_R93 (render), COLLIDERS_NOT_FOR_RENDER (collider), I2_BEFORE_DO_NOT_EXPORT (baseline). No scene product change until validation passes. GroundSurvey is immutable input for this recipe. See Test/KetQua/journey_v48/i2_r93_complete.\n')
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));(D/'geometry-change.json').write_text(json.dumps(dict(source=str(P),status='BLENDER_VALIDATION_PENDING',boundary=dict(center=[cx,cz],inner=13.35,outer=24,unchanged_radius=30,island_plane=plane,inner_road_gap_estimate=.04),objects=rows),indent=2));print('SAVED_SINGLE_SOLUTION',str(P),flush=True)
