"""Local north-bank heightfield repair in editable Blender, not an overlay.
Shore XY and road/dry endpoints are retained; local cross section is an estimate.
"""
import bpy,bmesh,json,sys,math,struct
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/Game/GameHorror/Game');O=R/'Docs/Reviews/journey_v48/r91';A=R/'Assets/Art/Environment/JourneyV48/BankR91';A.mkdir(parents=True,exist_ok=True)
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(R/'Docs/Reviews/journey_v48/r90_review/market-branches-r90-source.blend'))
names=['Grade_017','Grade_000','Grade_057','Grade_015','Grade_007']
trees={}
def tree(o):return BVHTree.FromPolygons([o.matrix_world@v.co for v in o.data.vertices],[list(p.vertices) for p in o.data.polygons])
for n in names:trees[n]=tree(bpy.data.objects[n])
def hit(t,x,y):
 p,n,idx,d=t.ray_cast(Vector((x,y,40)),Vector((0,0,-1)),80)
 return None if p is None else p.z
def ground(x,y):
 p=hit(trees['Grade_017'],x,y)
 if p is not None and p>-.9:return p
 for n in ('Grade_000','Grade_057'):
  q=hit(trees[n],x,y)
  if q is not None and q>-.9:return q-.04
 return p
curve=[(517.03,514.71),(565.15,531.84),(590.44,540.67),(596.05,542.17),(601,542.37),(606.82,539.77),(648.81,505.94)]
def closest(x,y):
 p=Vector((-x,-y));best=None
 for a,b in zip(curve,curve[1:]):
  a=Vector(a);b=Vector(b);d=b-a;t=max(0,min(1,(p-a).dot(d)/d.length_squared));q=a+d*t;n=Vector((-d.y,d.x)).normalized()
  if n.y<0:n=-n
  dist=(p-q).length
  if best is None or dist<best[0]:best=(dist,(p-q).dot(n),q,n)
 return best
def smooth(v):v=max(0,min(1,v));return v*v*(3-2*v)
def target(x,y):
 if not -643<x<-545:return None
 dist,d,q,n=closest(x,y)
 if d<-1.8 or d>7 or dist>9:return None
 edge=ground(-(q.x+n.x*7),-(q.y+n.y*7))
 if edge is None or edge<.15:return None
 t=max(0,min(1,d/7));h=.025+(edge-.025)*t
 if d<0:h=-1+1.025*((d+1.8)/1.8)
 fade=smooth(((-x)-545)/10)*smooth((643-(-x))/10)
 return h,fade,d
# Bring the exact still-live secondary terrain collider into the editable source.
data=memoryview((O/'context125-before.bin').read_bytes());off=0
def read(fmt):
 global off
 n=struct.calcsize(fmt);v=struct.unpack_from(fmt,data,off);off+=n;return v
n=read('<i')[0];vs=[];uv=[]
for i in range(n):v=read('<5f');vs.append(v[:3]);uv.append(v[3:])
faces=[];slots=[]
for k in range(read('<i')[0]):
 size=read('<i')[0];ix=read('<'+'i'*size)
 for i in range(0,size,3):faces.append(ix[i:i+3]);slots.append(k)
m=bpy.data.meshes.new('Context125_R91');m.from_pydata(vs,[],faces);m.update();layer=m.uv_layers.new(name='UVMap')
for p in m.polygons:
 for li in p.loop_indices:layer.data[li].uv=uv[m.loops[li].vertex_index]
c=bpy.data.objects.new('Context125_R91',m);bpy.context.scene.collection.objects.link(c)
changes=[];parts=[]
for name,terrain in [('Context125_R91',True)]:
 o=bpy.data.objects[name];before=[];count=0;max_change=0
 # Retessellate the bank strip before grading; coarse triangles must not bridge
 # the dry and submerged profiles. The source UV mapping is rebuilt barycentrically.
 old_vertices=[o.matrix_world@v.co for v in o.data.vertices]
 old_faces=[list(p.vertices) for p in o.data.polygons]
 oldtree=BVHTree.FromPolygons(old_vertices,old_faces)
 old_ids={}
 if name!='Context125_R91':
  for p in o.data.polygons:
   for li in p.loop_indices:old_ids[o.data.loops[li].vertex_index]=round(o.data.uv_layers[1].data[li].uv.x)
  raw=(R/'Docs/Reviews/journey_v48/r88'/(name+'-mapping.bin')).read_bytes();n=struct.unpack_from('<i',raw)[0]
  base_ids=struct.unpack_from('<'+'i'*(n*3),raw,4);base_weights=struct.unpack_from('<'+'f'*(n*3),raw,4+n*12)
 bm=bmesh.new();bm.from_mesh(o.data)
 for axis,coordinate in ((0,-535),(0,-653),(1,-506),(1,-559)):
  point=[0,0,0];point[axis]=coordinate;normal=[0,0,0];normal[axis]=1
  bmesh.ops.bisect_plane(bm,geom=list(bm.verts)+list(bm.edges)+list(bm.faces),dist=.00001,plane_co=point,plane_no=normal,clear_inner=False,clear_outer=False)
 bmesh.ops.triangulate(bm,faces=[f for f in bm.faces if len(f.verts)>3])
 def inrect(e):
  a,b=[o.matrix_world@v.co for v in e.verts]
  return min(a.x,b.x)<-535 and max(a.x,b.x)>-653 and min(a.y,b.y)<-506 and max(a.y,b.y)>-559
 for iteration in range(12):
  edges=[e for e in bm.edges if inrect(e) and (o.matrix_world@e.verts[0].co-o.matrix_world@e.verts[1].co).length>.70]
  if not edges:break
  bmesh.ops.subdivide_edges(bm,edges=edges,cuts=1,use_grid_fill=True)
  faces=[f for f in bm.faces if len(f.verts)>3];bmesh.ops.triangulate(bm,faces=faces)
 bm.to_mesh(o.data);bm.free();o.data.update()
 if name!='Context125_R91':
  from mathutils.geometry import barycentric_transform
  outids=[];outweights=[]
  for v in o.data.vertices:
   w=o.matrix_world@v.co;co,no,fi,dist=oldtree.find_nearest(w)
   face=old_faces[fi];assert len(face)==3
   aa,bb,cc=[old_vertices[i] for i in face]
   bary=barycentric_transform(co,aa,bb,cc,Vector((1,0,0)),Vector((0,1,0)),Vector((0,0,1)))
   for index,weight in zip(face,bary):
    source_index=old_ids[index]
    outids.extend(base_ids[source_index*3:source_index*3+3]);outweights.extend(float(q*weight) for q in base_weights[source_index*3:source_index*3+3])
  for p in o.data.polygons:
   for li in p.loop_indices:o.data.uv_layers[1].data[li].uv=(o.data.loops[li].vertex_index,0)
  (O/('BankR91_'+name+'-mapping.bin')).write_bytes(struct.pack('<ii',len(o.data.vertices),9)+struct.pack('<'+'i'*len(outids),*outids)+struct.pack('<'+'f'*len(outweights),*outweights))

 for v in o.data.vertices:
  w=o.matrix_world@v.co;t=target(w.x,w.y)
  if t is None:continue
  h,fade,d=t
  if terrain:new=h
  else:
   old=ground(w.x,w.y)
   if old is None:continue
   new=h+max(-.12,min(.15,w.z-old))
  nz=w.z+(new-w.z)*fade
  if abs(nz-w.z)<.000001:continue
  max_change=max(max_change,abs(nz-w.z));before.append((v.index,float(w.z)));w.z=nz;v.co=o.matrix_world.inverted()@w;count+=1
 if count:
  o.name='BankR91_'+name;o.data.name=o.name
  bm=bmesh.new();bm.from_mesh(o.data)
  affected=[v for v in bm.verts if target(*(o.matrix_world@v.co)[:2]) is not None]
  # Coincident seam vertices preserve UV/material boundaries.
  bad=[f for f in bm.faces if f.calc_area()<1e-9 and any(target(*(o.matrix_world@v.co)[:2]) is not None for v in f.verts)]
  bmesh.ops.delete(bm,geom=bad,context='FACES_ONLY')
  bm.to_mesh(o.data);bm.free();o.data.update()
  o['r91_scope']='North bank between Thuy Ta and Ong Dao, X545..643. Shoreline XY fixed. Existing dry boundary retained at signed offset7m. No DSM local heights.'
  changes.append({'object':o.name,'source_object':name,'vertices_changed':count,'maximum_change_m':max_change,'original_z':before});parts.append(o)
old_source=Path('C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101/north-bank-r91-source.blend')
with bpy.data.libraries.load(str(old_source),link=False) as (src,dst):dst.objects=[n for n in src.objects if n.startswith('BankR91_Grade_')]
for ob in dst.objects:bpy.context.scene.collection.objects.link(ob);parts.append(ob)

# Every duplicate render/collider sample gets the same map-space height change;
# endpoint heights outside the mask and all island/bridge/city footprints stay unchanged.
bpy.context.preferences.filepaths.save_version=0
for o in bpy.context.scene.objects:
 if o not in parts:o.hide_render=True;o.hide_set(True)
bpy.ops.wm.save_as_mainfile(filepath='C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101/north-bank-r91-source-refined.blend',compress=True)
exports=[]
for o in [bpy.data.objects['BankR91_Context125_R91']]:exports.append(bx_export.export(str(A/(o.name+'.fbx')),'unity',[o]))
(O/'context-refinement.json').write_text(json.dumps({'source':str(O/'north-bank-r91-source.blend'),'scope_xy':{'x':[545,643],'signed_dry_offset':[-1.8,7]},'cross_section_status':'estimated continuous technical repair, not a survey','changes':changes,'exports':exports},indent=2,default=str))
print('R91_BANK_EXPORTED',[(c['object'],c['vertices_changed'],round(c['maximum_change_m'],3)) for c in changes],flush=True)
