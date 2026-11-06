import bpy,bmesh,sys,math,json
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R103/Workshop_Investigation_R103_Editable.blend';D=R/'Test/KetQua/investigation_r103';E=R/'Test/BangChung/investigation_r103';A=R/'Assets/Art/InvestigationV36/R103'
bpy.ops.wm.open_mainfile(filepath=str(S));scene=bpy.context.scene
def P(v):return Vector((-v[0],-v[2],v[1]))
col=bpy.data.collections['EVIDENCE']
def mat(n,c):
 m=bpy.data.materials.get(n) or bpy.data.materials.new(n);m.diffuse_color=(*c,1);m.use_nodes=True;m.node_tree.nodes['Principled BSDF'].inputs['Base Color'].default_value=(*c,1);return m
ink=mat('R103 charcoal impression',(.035,.029,.021));dust=mat('R103 pale slide dust',(.58,.43,.26));rubber=mat('R103 dark rubber',(.025,.027,.029));worn=mat('R103 worn rubber',(.22,.22,.20));cloth=mat('R103 blue weave',(.10,.23,.32));thread=mat('R103 seam thread',(.72,.65,.48));paper=mat('R103 label ivory',(.86,.77,.56))
def move(o):
 for c in list(o.users_collection):c.objects.unlink(o)
 col.objects.link(o);return o
def box(n,p,s,m):
 bpy.ops.mesh.primitive_cube_add(size=1,location=P(p));o=move(bpy.context.object);o.name=n;o.scale=(s[0],s[2],s[1]);bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m);return o
def rod(n,a,b,r,m):
 a=P(a);b=P(b);bpy.ops.mesh.primitive_cylinder_add(vertices=8,radius=r,depth=(b-a).length,location=(a+b)/2);o=move(bpy.context.object);o.name=n;o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler();bpy.ops.object.transform_apply(location=False,rotation=True,scale=True);o.data.materials.append(m);return o
def join(parts,n):
 bpy.ops.object.select_all(action='DESELECT')
 for o in parts:o.select_set(True)
 bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();o=bpy.context.object;o.name=n;scene.cursor.location=(0,0,0);bpy.ops.object.origin_set(type='ORIGIN_CURSOR');return o
def remove(n):
 o=bpy.data.objects.get(n)
 if o:bpy.data.objects.remove(o,do_unlink=True)
def sole(n,height,m,variant=False):
 # Real concave heel outline: the missing corner is geometry, not a colour label.
 notch=-.102 if not variant else -.117
 outline=[(-.055,-.145),(.010,-.145),(.010,notch),(.055,notch),(.055,.105),(.035,.145),(-.035,.145),(-.055,.105)]
 vertices=[P((x,y,z)) for y in (0,height) for x,z in outline];N=len(outline)
 mesh=bpy.data.meshes.new(n);mesh.from_pydata(vertices,[],[tuple(range(N-1,-1,-1)),tuple(range(N,2*N))]+[(i,(i+1)%N,(i+1)%N+N,i+N) for i in range(N)]);mesh.update();o=bpy.data.objects.new(n,mesh);col.objects.link(o);o.data.materials.append(m);return o
def tread(m,y,variant=False):
 parts=[]
 for row,z in enumerate([-.12,-.075,-.03,.015,.06,.105]):
  for side in (-1,1):
   if row==0 and side==1:continue
   # Same worn row in sample and trace. Similar control retains the middle lug.
   width=.010 if row==2 and side==1 and not variant else .038
   o=box('TreadContact', (side*.027,y,z),(width,.0012,.025),m);o.rotation_euler.z=math.radians(side*20);parts.append(o)
 return parts
for n in ['ShoeSample','ShoePrint','SlideMarks']:remove(n)
shoe=join([sole('Sole_base',.035,rubber),box('ShoeUpper',(0,.085,.025),(.105,.11,.22),rubber)]+tread(worn,-.002),'ShoeSample')
other=join([sole('Control_base',.035,rubber,True),box('ControlUpper',(0,.085,.025),(.105,.11,.22),rubber)]+tread(worn,-.002,True),'ShoeControl')
printmesh=join(tread(ink,.0015)+[box('HeelEdge',(-.05,.001,-.12),(.008,.001,.045),ink),box('HeelBar',(-.024,.001,-.14),(.055,.001,.008),ink)],'ShoePrint')
# Two long slide tracks and discontinuous dusty feathering. Actual ink lugs cross them.
parts=[]
for i in range(30):
 for x in (-.035,.035):parts.append(box('SlideGrain',(x+.003*math.sin(i*1.3),.0004,-.26+i*.023),(.018,.0008,.026),dust))
slide=join(parts,'SlideMarks')
# Use a common local origin for both scene and inspection; print sits above uninterrupted slide grains.
parts=[]
for ob in [slide,printmesh]:
 copy=ob.copy();copy.data=ob.data.copy();col.objects.link(copy);parts.append(copy)
cross=join(parts,'TraceCrossing');cross['observation']='Dark heel/tread lies above pale elongated drag traces; order of marks is observed, identity/timing of fall remains inference.'
# Frayed asymmetrical edge and visible stitch/different weave direction. Existing shape stays source of both pieces.
frag=bpy.data.objects['TornFabric'];parts=[frag]
for i in range(7):parts.append(rod('LooseFibre',(-.052+i*.016,.003,-.004),(-.057+i*.017,.004,.014+(i%3)*.008),.0008,thread))
for i in range(6):parts.append(box('Stitch',(-.045+i*.016,.004,-.036),(.007,.0015,.003),thread))
for i in range(8):parts.append(box('Weave',(-.05+i*.014,.003,-.055),(.001,.001,.045),thread))
join(parts,'TornFabric')
coat=bpy.data.objects['CoatSample'];parts=[coat]
# Companion seam strip is visibly attached at the cuff, and remains distinct from an identity claim.
for i in range(6):parts.append(box('CoatStitch',(.31+i*.015,.01,-.28),(.006,.002,.003),thread))
join(parts,'CoatSample')
def label(text,n):
 backing=box(n+'_plate',(0,1.19,.17),(.28,.12,.012),paper)
 bpy.ops.object.text_add(location=P((-.12,1.16,.18)));t=move(bpy.context.object);t.name=n+'_text';t.data.body=text;t.data.size=.075;t.rotation_euler=(math.pi/2,0,math.pi);t.data.extrude=.0005;t.data.materials.append(ink);bpy.context.view_layer.objects.active=t;bpy.ops.object.convert(target='MESH');return join([backing,bpy.context.object],n)
label('R-03','Label_R03');label('R-12','Label_R12')
label('R-12  /  TAY PHAI','RepairCard')
# Explicit assembly for focused inspection while preserving measured separation of handle and latch.
mechan=bpy.data.collections['MECHANISM']
for o in list(col.objects):
 if o.type=='MESH':
  bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(o.data);bm.free();o.data.update()
scene.render.engine='BLENDER_WORKBENCH';scene.display.shading.color_type='MATERIAL';scene.display.shading.light='STUDIO';scene.display.shading.show_cavity=True;scene.render.resolution_x=1100;scene.render.resolution_y=800;scene.render.resolution_percentage=100
camera=scene.camera;camera.data.type='ORTHO'
for name,campos,look,scale in [('TraceCrossing',(.38,.6,.55),(0,0,.04),.65),('ShoeSample',(.30,-.5,.42),(0,.03,0),.48),('ShoeControl',(.30,-.5,.42),(0,.03,0),.48),('TornFabric',(.12,.3,.22),(0,0,0),.22),('CoatSample',(1,1,1),(0,.05,0),1.4)]:
 for o in scene.objects:o.hide_render=o.type=='MESH' and o.name!=name
 for c in bpy.data.collections:c.hide_render=False
 camera.location=P(campos);camera.rotation_euler=(P(look)-camera.location).to_track_quat('-Z','Y').to_euler();camera.data.ortho_scale=scale;scene.render.filepath=str(E/('source-'+name+'.png'));bpy.ops.render.render(write_still=True)
for o in scene.objects:o.hide_render=False
bpy.ops.wm.save_as_mainfile(filepath=str(S));bpy.ops.wm.open_mainfile(filepath=str(S))
assert not bpy.data.libraries
sys.path.extend([str(R/'.agents/skills/scenario-blender-expert/scripts'),str(R/'.agents/skills/scenario-blender-export/scripts')]);import bx_audit
checks={}
for name in ['ShoeSample','ShoeControl','ShoePrint','SlideMarks','TraceCrossing','TornFabric','CoatSample','Label_R03','Label_R12']:
 o=bpy.data.objects[name];checks[name]=dict(vertices=len(o.data.vertices),faces=len(o.data.polygons),negative_scale=any(v<0 for v in o.scale),degenerate=sum(p.area<1e-12 for p in o.data.polygons))
(D/'evidence-source.json').write_text(json.dumps(dict(blender=bpy.app.version_string,reopened=True,units=bpy.context.scene.unit_settings.scale_length,checks=checks),indent=2));print('R103 evidence source saved/reopened; review images before export')
