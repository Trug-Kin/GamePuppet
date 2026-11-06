"""One editable bridge-head grass-bank cluster; immutable R96 input.
Photo topology is evidenced. Dimensions are an adaptation to the existing game
road/water datum, not a surveyed reconstruction. No dam/bridge/market regrade.
"""
import bpy,bmesh,json,math,sys,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97';M=R/'Test/BangChung/journey_v48/bank_r97';S=R/'Source/Blender/JourneyV48/R97';S.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R96/Market_Ground_R96_Editable.blend'))
# Native=(-UnityX,-UnityZ,UnityY). Preserve the actual water polygon.
coast=np.array([[486.249314,672.422123],[481.787168,674.963951],[476.974594,675.900316],[471.846449,675.05722],[460.297645,670.699549],[439.996352,661.236382],[426.079479,652.028465],[411.186473,641.841459]])[::-1]
z0,z1=418.,483.;width=16.;left=-.24
def shore(z):return float(np.interp(z,coast[:,0],coast[:,1]))
base=bpy.data.objects['BankR91_Grade_017'];bm=np.array(base.matrix_world,dtype=float);bv=np.array([v.co for v in base.data.vertices],dtype=float)@bm[:3,:3].T+bm[:3,3];bt=BVHTree.FromPolygons([Vector(q) for q in bv],[tuple(p.vertices) for p in base.data.polygons])
support_trees=[]
for nn in ['BankR91_Grade_017','TerrainJoin_R90','BankR91_Grade_000','BankR91_Grade_052','EastFootwayJoin_R90','BankR91_Grade_057']:
 oo=bpy.data.objects[nn];mm=np.array(oo.matrix_world,dtype=float);vv=np.array([v.co for v in oo.data.vertices],dtype=float)@mm[:3,:3].T+mm[:3,3];support_trees.append(BVHTree.FromPolygons([Vector(q) for q in vv],[tuple(p.vertices) for p in oo.data.polygons]))
def old(x,z):
 hits=[]
 for t in support_trees:
  a,n,i,d=t.ray_cast(Vector((-x,-z,30)),Vector((0,0,-1)),60)
  if a:hits.append(float(a.z))
 if not hits:raise RuntimeError('No stable inherited support at '+str((x,z)))
 return max(hits)
def fade(z):
 t=max(0,min(1,min(z-z0,z1-z)/4.));return t*t*(3-2*t)
def levels(z):
 hi=old(shore(z)+width,z);lo=.22;mid=lo+(hi-lo)*.52
 return lo,mid,hi
def profile(d,z):
 lo,mid,hi=levels(z)
 if d<=1.8:h=lo
 elif d<5.8:h=lo+(mid-lo)*(d-1.8)/4
 elif d<=7.3:h=mid
 elif d<13:h=mid+(hi-mid)*(d-7.3)/5.7
 else:h=hi
 return old(shore(z)+d,z)*(1-fade(z))+h*fade(z)
def col(n):
 c=bpy.data.collections.new(n);bpy.context.scene.collection.children.link(c);return c
main=col('BANK_R97_EDITABLE_PHOTO_CLUSTER');groups={n:col('R97_'+n) for n in ['UPPER_WALK','UPPER_GRASS','INTERMEDIATE_WALK','LOWER_GRASS','LOWER_WALK','STAIRS_LANDINGS','QUAY','PHOTO_RAIL_SEGMENT']}
for c in groups.values():bpy.context.scene.collection.children.unlink(c);main.children.link(c)
clipped=col('R97_EXISTING_LAYERS_CUT_NO_OVERLAY')
def material(name,color):
 m=bpy.data.materials.new(name);m.diffuse_color=(*color,1);p=m.node_tree.nodes.get('Principled BSDF');p.inputs['Base Color'].default_value=(*color,1);p.inputs['Roughness'].default_value=.88;return m
mats={n:material('R97_'+n,c) for n,c in [('Grass',(.28,.36,.13)),('Paving',(.52,.48,.40)),('Concrete',(.48,.46,.40)),('Quay',(.28,.29,.26)),('Rail',(.22,.34,.31))]}
def mesh(n,verts,faces,group,mat):
 # Source in native world axes; origin at zero is deliberate map datum.
 m=bpy.data.meshes.new(n);m.from_pydata([(-x,-z,y) for x,y,z in verts],[],faces);m.update();o=bpy.data.objects.new(n,m);group.objects.link(o);m.materials.append(mats[mat]);uv=m.uv_layers.new(name='UVMap')
 for p in m.polygons:
  for li in p.loop_indices:
   v=m.vertices[m.loops[li].vertex_index].co;uv.data[li].uv=(v.x*.5,v.y*.5)
 b=bmesh.new();b.from_mesh(m);bmesh.ops.recalc_face_normals(b,faces=list(b.faces));b.to_mesh(m);b.free()
 if 'WALK' in n or 'GRASS' in n:
  for p in m.polygons:
   if p.normal.z<0:p.flip()
  m.update()
 o.color=mats[mat].diffuse_color;o['bank97_export']=True;o['dimension_status']='Estimated game adaptation, 1 unit=1m authoring, not surveyed';return o
# Exact footprint subdivision for replacing all inherited render/collision sheets.
breaks=sorted(set([z0,z1]+[float(z) for z,x in coast if z0<z<z1]))
quads=[np.array([[shore(a)+left,a],[shore(b)+left,b],[shore(b)+width,b],[shore(a)+width,a]]) for a,b in zip(breaks,breaks[1:])]
def half(poly,a,b,inside):
 out=[]
 def signed(p):return (b[0]-a[0])*(p[2]-a[1])-(b[1]-a[1])*(p[0]-a[0])
 for p,q in zip(poly,poly[1:]+poly[:1]):
  dp,dq=signed(p),signed(q);ip=dp>=-1e-9 if inside else dp<=1e-9;iq=dq>=-1e-9 if inside else dq<=1e-9
  if ip:out.append(p)
  if ip!=iq:
   t=dp/(dp-dq);out.append(p+(q-p)*t)
 return out
def difference(poly,q):
 keep=[];cur=poly
 # q above is clockwise in XZ, so reverse to CCW.
 if np.cross(q[1]-q[0],q[2]-q[1])<0:q=q[::-1]
 for a,b in zip(q,np.roll(q,-1,axis=0)):
  if len(cur)<3:break
  ext=half(cur,a,b,False)
  if len(ext)>=3:keep.append(ext)
  cur=half(cur,a,b,True)
 return keep
cut=[]
with bpy.data.libraries.load(str(R/'Source/Blender/JourneyV48/R91/north-bank-r91-source-final.blend'),link=False) as (a,b):
 b.objects=['BankR91_Context125_R91']
extra=b.objects[0];clipped.objects.link(extra);extra['unity_path']='LakeGateMarketV32/Context_125';extra['market95_export_key']='M97_Context125';extra.hide_render=True
for name,key in [('C95_0182_Surface_149','M97_Surface149'),('Grade_085','M97_Edge085')]:
 o=bpy.data.objects[name];o['market95_export_key']=key
eligible=['M95_000','M95_001','M95_002','M95_011','M95_012','M95_013','M95_014','M95_015','M95_016','M95_017','M95_018','M95_019','M95_020','M95_021','M95_022','M95_023','M95_027','M95_032','M97_Context125','M97_Surface149','M97_Edge085']
for o in list(bpy.context.scene.objects):
 k=o.get('market95_export_key','')
 if o.type!='MESH' or k not in eligible:continue
 me=o.data;mat=np.array(o.matrix_world,dtype=float);nv=np.array([v.co for v in me.vertices],dtype=float)@mat[:3,:3].T+mat[:3,3];v=np.c_[-nv[:,0],nv[:,2],-nv[:,1]];uvs=list(me.uv_layers);polys=[];changed=0;removed_area=0.
 for p in me.polygons:
  pp=v[list(p.vertices)];mn=pp.min(0);mx=pp.max(0)
  # A cheap conservative envelope before precise clipping.
  if mx[2]<z0 or mn[2]>z1 or mx[0]<646 or mn[0]>693:polys.append((p,None));continue
  arr=[np.r_[v[me.loops[li].vertex_index],*[list(uv.data[li].uv) for uv in uvs]] for li in p.loop_indices];parts=[arr]
  for q in quads:
   if mx[2]<q[:,1].min() or mn[2]>q[:,1].max() or mx[0]<q[:,0].min() or mn[0]>q[:,0].max():continue
   parts=[b for a in parts for b in difference(a,q)]
  # Compare area; tangential contact should not rebuild unaffected faces.
  def area(a):return sum(np.linalg.norm(np.cross(a[i][:3]-a[0][:3],a[i+1][:3]-a[0][:3]))*.5 for i in range(1,len(a)-1))
  removed=area(arr)-sum(area(a) for a in parts)
  if removed<1e-8:polys.append((p,None));continue
  changed+=1;removed_area+=removed
  for a in parts:polys.append((p,a))
 if not changed:continue
 verts=[];faces=[];slots=[];uvout=[[] for uv in uvs];dedup={}
 for p,arr in polys:
  if arr is None:arr=[np.r_[v[me.loops[li].vertex_index],*[list(uv.data[li].uv) for uv in uvs]] for li in p.loop_indices]
  ids=[]
  for a in arr:
   key=tuple(np.round(a,8));ix=dedup.get(key)
   if ix is None:ix=len(verts);dedup[key]=ix;verts.append((-a[0],-a[2],a[1]));[uvout[j].append(a[3+j*2:5+j*2]) for j in range(len(uvs))]
   ids.append(ix)
  for i in range(1,len(ids)-1):
   xyz=np.array([verts[j] for j in [ids[0],ids[i],ids[i+1]]])
   if np.linalg.norm(np.cross(xyz[1]-xyz[0],xyz[2]-xyz[0]))<1e-9:continue
   faces.append((ids[0],ids[i],ids[i+1]));slots.append(p.material_index)
 new=bpy.data.meshes.new(k+'_R97_Cut');new.from_pydata(verts,[],faces);new.update()
 for m in me.materials:new.materials.append(m)
 for j,uv in enumerate(uvs):
  layer=new.uv_layers.new(name=uv.name)
  for p in new.polygons:
   for li in p.loop_indices:layer.data[li].uv=uvout[j][new.loops[li].vertex_index]
 for p,slot in zip(new.polygons,slots):p.material_index=slot
 o.data=new;o.matrix_world.identity();o['bank97_cut']=True;o['bank97_export']=True
 if o.name not in clipped.objects:clipped.objects.link(o)
 cut.append(dict(key=k,object=o.name,path=o['unity_path'],faces_cut=changed,removed_area=removed_area,faces_after=len(faces)))
 print('CUT',k,changed,removed_area,flush=True)
# All replacement bands share the same sampled ring vertices. Stairs occupy
# explicit holes in both grass bands, so no hidden grass/collider under flights.
stations=[429.,450.,471.];stairwidth=2.2
zs=sorted(set([z0,z1]+[round(float(z),6) for z in np.arange(z0,z1,.65)]+[s+q for s in stations for q in [-stairwidth/2,stairwidth/2]]+[float(z) for z,x in coast if z0<z<z1]))
made=[]
for name,a,b,mat in [('LOWER_WALK',0,1.8,'Paving'),('LOWER_GRASS',1.8,5.8,'Grass'),('INTERMEDIATE_WALK',5.8,7.3,'Paving'),('UPPER_GRASS',7.3,13,'Grass'),('UPPER_WALK',13,16,'Paving')]:
 vv=[];ff=[];ds=np.linspace(a,b,max(2,math.ceil((b-a)/.6)+1))
 for z in zs:
  for d in ds:vv.append((shore(z)+d,profile(d,z),z))
 for i in range(len(zs)-1):
  if 'GRASS' in name and any(abs((zs[i]+zs[i+1])/2-s)<stairwidth/2-1e-6 for s in stations):continue
  for j in range(len(ds)-1):
   k=i*len(ds)+j;ff.append((k,k+1,k+1+len(ds),k+len(ds)))
 made.append(mesh('R97_'+name,vv,ff,groups[name],mat))
flights=[]
for station in stations:
 for tag,a,b in [('LOWER',1.8,5.8),('UPPER',7.3,13)]:
  low=profile(a,station);high=profile(b,station);n=max(2,round((high-low)/.13));run=(b-a)/n
  # A closed continuous stepped prism per flight, not separate floating cubes.
  section=[(a,low-.16),(b,low-.16),(b,high)]
  for i in range(n-1,-1,-1):section.extend([(a+i*run,low+(high-low)*(i+1)/n),(a+i*run,low+(high-low)*i/n)])
  # remove adjacent duplicates before extrusion
  section=[p for i,p in enumerate(section) if i==0 or p!=section[i-1]]
  vv=[]
  for z in [station-stairwidth/2,station+stairwidth/2]:
   for d,h in section:
    # each stair edge joins the side grass profile at its own local base.
    ratio=(h-low)/(high-low) if high>low else 0
    hh=profile(a,z)+(profile(b,z)-profile(a,z))*ratio
    vv.append((shore(z)+d,hh,z))
  N=len(section);ff=[tuple(range(N-1,-1,-1)),tuple(range(N,2*N))]+[(i,(i+1)%N,(i+1)%N+N,i+N) for i in range(N)]
  made.append(mesh('R97_STAIR_'+str(int(station))+'_'+tag,vv,ff,groups['STAIRS_LANDINGS'],'Concrete'));flights.append(dict(z=station,tier=tag,steps=n,rise=(high-low)/n,tread=run,width=stairwidth,low=low,high=high))
# Quay from walk lip to below existing water (Y=0); no alteration of water mesh.
vv=[];ff=[]
for z in zs:
 for d,h in [(0,profile(0,z)),(-.24,-.35)]:vv.append((shore(z)+d,h,z))
for i in range(len(zs)-1):ff.append((i*2,i*2+1,i*2+3,i*2+2))
made.append(mesh('R97_QUAY_WATER_EDGE',vv,ff,groups['QUAY'],'Quay'))
# Rail ONLY at the short bridge-return segment seen beside concrete apron.
# No continuous technical fence around the lake or through stair openings.
def beam(vv,ff,a,b,r):
 a=np.array(a);b=np.array(b);d=b-a;u=np.cross(d,[0,1,0])
 if np.linalg.norm(u)<1e-9:u=np.cross(d,[1,0,0])
 u/=np.linalg.norm(u);w=np.cross(d,u);w/=np.linalg.norm(w);k=len(vv)
 for p in [a,b]:
  for angle in np.arange(8)*math.pi/4:vv.append(tuple(p+r*(u*math.cos(angle)+w*math.sin(angle))))
 ff.extend([tuple(k+i for i in range(7,-1,-1)),tuple(k+8+i for i in range(8))]);ff.extend([(k+i,k+(i+1)%8,k+(i+1)%8+8,k+i+8) for i in range(8)])
vv=[];ff=[]
for a,b in zip(np.arange(474,480,.65),np.arange(474.65,480.65,.65)):
 for h in [.15,.92]:beam(vv,ff,(shore(a)+.10,profile(0,a)+h,a),(shore(b)+.10,profile(0,b)+h,b),.025)
for z in np.arange(474,480.1,.65):
 h=profile(0,z);beam(vv,ff,(shore(z)+.10,h,z),(shore(z)+.10,h+.95,z),.022)
made.append(mesh('R97_PHOTO_SHORT_RAIL',vv,ff,groups['PHOTO_RAIL_SEGMENT'],'Rail'))
# Shared source context: do not render independent legacy colliders or missing
# facade comparisons as if they were reconstructed by this task.
for o in list(bpy.context.scene.objects):
 if o.type=='MESH':
  path=o.get('unity_path','')
  if path.startswith('LakeGateMarketV32/') or 'COLLIDER' in o.name.upper() or o.name.startswith('I2_BEFORE_'):o.hide_render=True
  if o in made:o.hide_render=False;o.hide_set(False)
sc=bpy.context.scene;sc.unit_settings.system='METRIC';sc.unit_settings.scale_length=1
sc['bank97_scope']='Gate-side grass shore, Unity Z418..483, actual water edge to 16m landward; market entry/I2/water/bridge railing/dam unchanged. Upper boundary follows actual intended inherited support, not a fixed minimum datum.'
sc['bank97_photos']='BugModel181826/181952/182010 topology,181809 short return rail context; 181347/181402/181938 opposite viewpoint/side not copied over all shore.'
sc['bank97_dimensions']='Estimated/constrained game adaptation; current upper ground only ~0.5..2m over unchanged Y0 water. Does not reproduce surveyed/photo stair counts.'
water=json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0][:-1]
wc=col('R97_CONTEXT_EXISTING_WATER_NOT_EXPORT');wm=bpy.data.meshes.new('Existing_Water_Context');wm.from_pydata([(-x,-z,0) for x,z in water],[],[tuple(range(len(water)))]);wm.update();wo=bpy.data.objects.new('CONTEXT_WATER_Y0_UNCHANGED',wm);wc.objects.link(wo)
wmat=material('R97_CONTEXT_WATER',(.15,.35,.42));wm.materials.append(wmat)
for p in wm.polygons:
 if p.normal.z<0:p.flip()
sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1280;sc.render.resolution_y=800;sc.render.resolution_percentage=100
sh=sc.display.shading;sh.light='STUDIO';sh.color_type='MATERIAL';sh.show_shadows=False;sh.show_cavity=True;sh.show_backface_culling=True;sh.background_type='VIEWPORT';sh.background_color=(.14,.18,.22)
cam=bpy.data.objects.new('R97_REVIEW_CAMERA',bpy.data.cameras.new('R97_REVIEW_CAMERA'));sc.collection.objects.link(cam);sc.camera=cam;cam.data.lens=30
shots=[('wide',[645,24,482],[670,.7,447]),('upper',[682,2.7,475],[660,.9,431]),('lower',[668,1.85,453],[655,.6,430]),('stairs',[666,2.4,449],[674,.9,450])]
for name,eye,at in shots:
 cam.location=(-eye[0],-eye[2],eye[1]);cam.rotation_euler=(Vector((-at[0],-at[2],at[1]))-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('blender-'+name+'.png'));bpy.ops.render.render(write_still=True)
for o in list(bpy.context.selected_objects):o.select_set(False)
for o in made:o.select_set(True)
bpy.context.view_layer.objects.active=made[0]
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(S/'Bridgehead_Bank_R97_Editable.blend'),compress=True)
(D/'author-result.json').write_text(json.dumps(dict(source=str(S/'Bridgehead_Bank_R97_Editable.blend'),cut=cut,parts=[o.name for o in made],flights=flights,coast=coast.tolist(),scope=[z0,z1,left,width],levels=[dict(z=z,lo=levels(z)[0],mid=levels(z)[1],upper=levels(z)[2]) for z in np.arange(z0,z1+.1,1)]),indent=2))
print('RESULT',len(cut),len(made),'saved',flush=True)
