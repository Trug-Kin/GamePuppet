"""Concrete dam cluster, photo-backed topology adapted to locked game datums.
Immutable input R97. Geometry editing, not a Unity review import/overlay.
"""
import bpy,bmesh,json,math,numpy as np
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';M=R/'Test/BangChung/journey_v48/dam_r98';S=R/'Source/Blender/JourneyV48/R98';S.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'))
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0])
def shore(s):
 p=A+T*s;hits=[]
 for a,b in zip(water,water[1:]):
  try:d,u=np.linalg.solve(np.column_stack([N,a-b]),a-p)
  except np.linalg.LinAlgError:continue
  if d>=0 and -.000001<=u<=1.000001:hits.append(d)
 if not hits:raise ValueError(('shore',s))
 return min(hits)
def pos(s,d,h):
 x,z=A+T*s+N*d;return (float(x),float(h),float(z))
def col(n,parent=None):
 c=bpy.data.collections.new(n);(parent.children if parent else bpy.context.scene.collection.children).link(c);return c
root=col('DAM_R98_EDITABLE_COMPLETE_CLUSTER');groups={n:col('R98_'+n,root) for n in ['CONCRETE_BAYS','CREST_SUPPORT','TOE_GRILLES','END_STAIRS','LOWER_LANDINGS','LOCAL_RAILS','SCOPED_LEGACY_CUTS']}
def material(n,c):
 m=bpy.data.materials.new('R98_'+n);m.diffuse_color=(*c,1);m.use_nodes=True;p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*c,1);p.inputs['Roughness'].default_value=.9;return m
mats={n:material(n,c) for n,c in [('Concrete',(.62,.59,.51)),('Ridge',(.46,.44,.38)),('Toe',(.25,.28,.26)),('Paving',(.49,.46,.39)),('Rail',(.24,.34,.31))]}
def mesh(n,vs,fs,group,mat,closed=True):
 me=bpy.data.meshes.new(n);me.from_pydata([(-x,-z,y) for x,y,z in vs],[],fs);me.update();o=bpy.data.objects.new(n,me);groups[group].objects.link(o);me.materials.append(mats[mat]);uv=me.uv_layers.new(name='UVMap')
 for p in me.polygons:
  for li in p.loop_indices:
   v=me.vertices[me.loops[li].vertex_index].co;uv.data[li].uv=(v.x*.3,v.y*.3)
 bm=bmesh.new();bm.from_mesh(me);bmesh.ops.remove_doubles(bm,verts=list(bm.verts),dist=.00001);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(me);bm.free();me.update()
 o['dam98_export']=True;o['dam98_closed']=closed;o['dimension_status']='Estimated game adaptation; 1 unit=1m authoring, no surveyed metre datum';o.hide_render=False;return o
def strip(n,ss,profile,group,mat):
 vs=[];fs=[]
 for s in ss:
  for d,h in profile(s):vs.append(pos(s,d,h))
 k=len(profile(ss[0]))
 for i in range(len(ss)-1):
  for j in range(k-1):a=i*k+j;fs.append((a,a+1,a+1+k,a+k))
 return mesh(n,vs,fs,group,mat,False)
# Convex tile subtraction with all UV channels retained, float64 math.
oldcode=(R/'Test/Tam/bank97-author.py').read_text();exec(oldcode[oldcode.index('def half('):oldcode.index('cut=[]')])
ss=sorted(set([-3.,0.,L,L+3.]+[float((p-A)@T) for p in water if -3<(p-A)@T<L+3 and (p-A)@N>0 and (p-A)@N<15]))
# Only source shoreline vertices on the relevant branch should split the strip.
ss=[-3.,0.,10.,25.,40.,50.,L,L+3.]
quads=[np.array([pos(a,.001,0)[::2],pos(b,.001,0)[::2],pos(b,shore(b),0)[::2],pos(a,shore(a),0)[::2]]) for a,b in zip(ss,ss[1:])]
names=['C95_0040_OD35_Surface_0','C95_0637_PlanterWalkConnection_2','C95_0650_R21_Conformed_Authored_Bridge_Links','C95_0651_R21_SE_Promenade_Construction_Edges','C95_0652_Garden_Bank_Return_Walk','C95_0653_FootwayOverShoulder_Garden_Bank_Return_Walk_Surface_125','C95_0654_FootwayOverShoulder_Garden_Bank_Return_Walk_Surface_153']
keys=['M95_000','M95_001','M95_011','M95_013','M95_014','M95_015','M95_016','M95_017','M95_018','M95_019','M95_021','M95_022','M95_023','M95_027','M97_Context125','M97_Edge085']
cut=[];eligible=[o for o in list(bpy.context.scene.objects) if o.type=='MESH' and (o.name in names or o.get('market95_export_key') in keys or o.name.startswith('R97_') and o.get('bank97_export'))]
# Actual separate collider is editable as its own scoped source; do not replace
# the entire legacy collider with a render mesh whose faces differ.
raw=(D/'garden-collider-before.bin').read_bytes();nv,ni=np.frombuffer(raw[:8],dtype='<i4');v=np.frombuffer(raw,dtype='<f4',count=int(nv)*3,offset=8).reshape(-1,3);tri=np.frombuffer(raw,dtype='<i4',count=int(ni),offset=8+int(nv)*12).reshape(-1,3)
cm=bpy.data.meshes.new('Garden_Actual_Collider_Before');cm.from_pydata([(-x,-z,y) for x,y,z in v],[],tri.tolist());cm.update();co=bpy.data.objects.new('R98_LEGACY_GARDEN_COLLIDER_ONLY',cm);groups['SCOPED_LEGACY_CUTS'].objects.link(co);co['unity_path']='R21_Boundary_Landscape_Finish/Garden_Bank_Return_Walk';co['dam98_collider_only']=True;co.hide_render=True;eligible.append(co)
for idx,o in enumerate(eligible):
 me=o.data;mm=np.array(o.matrix_world,dtype=float);vv=np.array([v.co for v in me.vertices],dtype=float)@mm[:3,:3].T+mm[:3,3];v=np.c_[-vv[:,0],vv[:,2],-vv[:,1]];sd=(v[:,[0,2]]-A)@np.c_[T,N];uvs=list(me.uv_layers);polys=[];changed=0;removed_area=0
 for p in me.polygons:
  ids=list(p.vertices);q=sd[ids];candidate=q[:,0].max()>-3 and q[:,0].min()<L+3 and q[:,1].max()>.001 and q[:,1].min()<12.5
  if not candidate:polys.append((p,None));continue
  arr=[np.r_[v[me.loops[li].vertex_index],*[list(uv.data[li].uv) for uv in uvs]] for li in p.loop_indices];parts=[arr];mn=np.min(arr,axis=0);mx=np.max(arr,axis=0)
  for quad in quads:
   if mx[2]<quad[:,1].min() or mn[2]>quad[:,1].max() or mx[0]<quad[:,0].min() or mn[0]>quad[:,0].max():continue
   parts=[b for a in parts for b in difference(a,quad)]
  def area(a):return sum(np.linalg.norm(np.cross(a[i][:3]-a[0][:3],a[i+1][:3]-a[0][:3]))*.5 for i in range(1,len(a)-1))
  removed=area(arr)-sum(area(a) for a in parts)
  if removed<1e-8:polys.append((p,None));continue
  changed+=1;removed_area+=removed
  for a in parts:polys.append((p,a))
 if not changed:print('UNCHANGED',o.name,flush=True);continue
 verts=[];faces=[];slots=[];uvout=[[] for uv in uvs];dedup={}
 for p,arr in polys:
  if arr is None:arr=[np.r_[v[me.loops[li].vertex_index],*[list(uv.data[li].uv) for uv in uvs]] for li in p.loop_indices]
  ids=[]
  for a in arr:
   k=tuple(np.round(a,8));ix=dedup.get(k)
   if ix is None:ix=len(verts);dedup[k]=ix;verts.append((-a[0],-a[2],a[1]));[uvout[j].append(a[3+j*2:5+j*2]) for j in range(len(uvs))]
   ids.append(ix)
  for i in range(1,len(ids)-1):
   xyz=np.array([verts[j] for j in [ids[0],ids[i],ids[i+1]]],dtype=np.float32).astype(float)
   if np.linalg.norm(np.cross(xyz[1]-xyz[0],xyz[2]-xyz[0]))<2e-9:continue
   faces.append((ids[0],ids[i],ids[i+1]));slots.append(p.material_index)
 new=bpy.data.meshes.new(o.name+'_R98_Cut');new.from_pydata(verts,[],faces);new.update()
 for m in me.materials:new.materials.append(m)
 for j,uv in enumerate(uvs):
  layer=new.uv_layers.new(name=uv.name)
  for p in new.polygons:
   for li in p.loop_indices:layer.data[li].uv=uvout[j][new.loops[li].vertex_index]
 for p,slot in zip(new.polygons,slots):p.material_index=slot
 o.data=new;o.matrix_world.identity();o['dam98_export']=True;o['dam98_cut']=True;o['dam98_key']='L98_'+str(idx).zfill(2);groups['SCOPED_LEGACY_CUTS'].objects.link(o) if o.name not in groups['SCOPED_LEGACY_CUTS'].objects else None
 # Display actual enabled current render layers in the editable source.
 if o.name in names:o.hide_render=False
 cut.append(dict(key=o['dam98_key'],object=o.name,path=o.get('unity_path',''),collider_only=bool(o.get('dam98_collider_only')),faces_cut=changed,removed_area=removed_area,faces_after=len(faces)));print('CUT',o.name,changed,round(removed_area,3),flush=True)
# 13 concrete bays: ridges are continuous mesh profile, not overlay strips.
stations=[]
for i in range(14):
 s=L*i/13
 stations.extend([(s,.10),(min(s+.13,L),0)] if i==0 else [(max(s-.13,0),0),(s,.10)] if i==13 else [(s-.13,0),(s,.10),(s+.13,0)])
vs=[];fs=[]
for s,ridge in stations:
 end=shore(s)
 for d,h in [(.0,1.140076),(.35,1.140076),(end-.75,.17),(end,.08),(end,-.35),(.0,.72)]:
  if .35<d<end-.01:h+=ridge
  vs.append(pos(s,d,h))
K=6
for i in range(len(stations)-1):
 for j in range(K):a=i*K+j;b=i*K+(j+1)%K;fs.append((a,b,b+K,a+K))
fs.extend([tuple(range(K-1,-1,-1)),tuple((len(stations)-1)*K+j for j in range(K))]);apron=mesh('R98_CONCRETE_DAM_13_BAYS',vs,fs,'CONCRETE_BAYS','Concrete')
# Single closed section includes crest support under preserved railing/pots.
flights=[]
for tag,a,b in [('FAR',-3.,0.),('GATE',L,L+3.)]:
 # Wide shallow steps are forced by retained low game bridge datum; not surveyed.
 steps=8;vs=[];fs=[]
 for s in [a,b]:
  end=shore(s);section=[(0,.72),(end,-.35),(end,.22),(end-1.6,.22)]
  start=.7;run=(end-1.6-start)/steps
  for i in range(steps-1,-1,-1):section.extend([(start+(i+1)*run,1.140076-(i+1)*(.920076/steps)),(start+(i+1)*run,1.140076-i*(.920076/steps))])
  section.extend([(start,1.140076),(0,1.140076)])
  vs.extend([pos(s,d,h) for d,h in section])
 k=len(section);fs=[tuple(range(k-1,-1,-1)),tuple(range(k,2*k))]+[(j,(j+1)%k,(j+1)%k+k,j+k) for j in range(k)]
 st=mesh('R98_'+tag+'_END_STAIR_AND_LOW_LANDING',vs,fs,'END_STAIRS','Concrete');flights.append(dict(tag=tag,s=[a,b],steps=steps,rise=.920076/steps,width=b-a,tread_range=[(shore(s)-2.3)/steps for s in [a,b]],low=.22,high=1.140076))
# Rail only at the external stair sides shown in the photos; no rail along toe
# and no new continuous route across the apron. Existing bridge railing retained.
exec(oldcode[oldcode.index('def beam('):oldcode.index('vv=[];ff=[]\nfor a,b in zip(np.arange(474')])
for tag,s in [('FAR',-3.),('GATE',L+3.)]:
 end=shore(s);vv=[];ff=[];ds=np.linspace(.3,end-.3,20)
 def hh(d):return 1.140076-.920076*max(0,min(1,(d-.7)/(end-2.3)))
 for d,e in zip(ds,ds[1:]):beam(vv,ff,pos(s,d,hh(d)+.94),pos(s,e,hh(e)+.94),.027)
 for d in np.linspace(.3,end-.3,14):beam(vv,ff,pos(s,d,hh(d)),pos(s,d,hh(d)+.96),.023)
 mesh('R98_'+tag+'_STAIR_SIDE_RAIL',vv,ff,'LOCAL_RAILS','Rail')
# Toe grilles on the lower inclined lip, small visible physical bars only.
for i in range(13):
 s0=(i+.2)*L/13;s1=(i+.8)*L/13;vv=[];ff=[]
 def toe(s,d):
  end=shore(s);return pos(s,d,.17+(.08-.17)*(d-(end-.75))/.75+.018)
 for s in np.linspace(s0,s1,15):beam(vv,ff,toe(s,shore(s)-.70),toe(s,shore(s)-.03),.015)
 for off in [.70,.03]:beam(vv,ff,toe(s0,shore(s0)-off),toe(s1,shore(s1)-off),.025)
 mesh('R98_TOE_GRILLE_'+str(i+1).zfill(2),vv,ff,'TOE_GRILLES','Toe')
sc=bpy.context.scene;sc.unit_settings.system='METRIC';sc.unit_settings.scale_length=1
sc['dam98_photos']='BugModel181809 near/apron-gate;181347/181402 independent across-lake view,181524 upper sidewalk. Visible structure only; concealed interior unknown.'
sc['dam98_scope']='Existing lake-side bridge edge s0..57.2696, stairs s[-3,0] and[57.2696,60.2696], footprint to unchanged water edge; precise cut legacy layers, retain R97 grass outside end tie.'
sc['dam98_limit']='Bridge Y1.140076 and waterY0 locked. Result apron slope ~10%, visibly shallower than photo. Dimensions/stair count/13bay allocation are game adaptation, not photogrammetric accuracy.'
sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1280;sc.render.resolution_y=800;sc.render.resolution_percentage=100;sh=sc.display.shading;sh.light='STUDIO';sh.color_type='MATERIAL';sh.show_shadows=False;sh.show_cavity=True;sh.show_backface_culling=True;sh.background_type='VIEWPORT';sh.background_color=(.15,.19,.23)
cam=bpy.data.objects.new('R98_REVIEW_CAMERA',bpy.data.cameras.new('R98_REVIEW_CAMERA'));sc.collection.objects.link(cam);sc.camera=cam;cam.data.lens=30
shots=[('wide',[625,10,489],[656,.6,513]),('oblique',[670,2,488],[648,.7,519]),('gate-stair',[678,3,480],[682,.6,488]),('far-stair',[629,3,525],[637,.6,530])]
for tag,eye,at in shots:
 cam.location=(-eye[0],-eye[2],eye[1]);cam.rotation_euler=(Vector((-at[0],-at[2],at[1]))-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('blender-'+tag+'.png'));bpy.ops.render.render(write_still=True)
for o in list(bpy.context.selected_objects):o.select_set(False)
apron.select_set(True);bpy.context.view_layer.objects.active=apron
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(S/'OngDao_Dam_R98_Editable.blend'),compress=True)
(D/'author-result.json').write_text(json.dumps(dict(source=bpy.data.filepath,cut=cut,flights=flights,basis=dict(A=A.tolist(),B=B.tolist(),T=T.tolist(),N=N.tolist(),L=L),footprint=[q.tolist() for q in quads],sections=[dict(s=s,run=shore(s),crest=1.140076,toe=.08,slope_percent=1.060076/(shore(s)-.35)*100) for s in [0,10,25,40,L]],parts=[o.name for o in sc.objects if o.get('dam98_export') and not o.get('dam98_cut')]),indent=2))
print('RESULT source saved, cuts',len(cut),flush=True)
