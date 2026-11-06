import bpy,bmesh,sys,math,json
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi'); O=R/'Source/Blender/JourneyV48/R102';O.mkdir(parents=True,exist_ok=True)
E=R/'Test/BangChung/investigation_r102';E.mkdir(parents=True,exist_ok=True)
D=R/'Test/KetQua/investigation_r102';D.mkdir(parents=True,exist_ok=True)
sys.path.extend([str(R/'.agents/skills/scenario-blender-expert/scripts'),str(R/'.agents/skills/scenario-blender-export/scripts')])
import bx_audit,bx_export
bpy.ops.wm.read_factory_settings(use_empty=True)
scene=bpy.context.scene;scene.unit_settings.system='METRIC';scene.unit_settings.scale_length=1
COL={}
for name in ['WORKSHOP','MECHANISM','EVIDENCE','CHARACTERS','PUPPETS','REVIEW']:
 c=bpy.data.collections.new(name);scene.collection.children.link(c);COL[name]=c
def move(o,col):
 for c in list(o.users_collection):c.objects.unlink(o)
 COL[col].objects.link(o);return o
def mat(n,c):
 m=bpy.data.materials.new(n);m.diffuse_color=(*c,1);m.use_nodes=True;m.node_tree.nodes['Principled BSDF'].inputs['Base Color'].default_value=(*c,1);m.node_tree.nodes['Principled BSDF'].inputs['Roughness'].default_value=.78;return m
wood=mat('Warm worn wood',(.27,.12,.054));pale=mat('Paper ivory',(.78,.7,.52));steel=mat('Dark metal',(.08,.12,.13));plaster=mat('Workshop warm plaster',(.49,.43,.31));green=mat('Gate muted teal',(.10,.26,.23));skin=mat('Warm skin',(.56,.33,.19));hair=mat('Dark hair',(.035,.025,.02));jacket=mat('Khai blue cotton',(.12,.22,.3));apron=mat('Ha ochre apron',(.55,.28,.08));trouser=mat('Charcoal fabric',(.065,.075,.085));rubber=mat('Rubber sole',(.045,.043,.038));ink=mat('Blue black ink',(.025,.055,.075));mark=mat('Trace chalk ochre',(.58,.38,.16))
def P(p):return Vector((-p[0],-p[2],p[1]))
def box(n,p,s,m,col='WORKSHOP'):
 bpy.ops.mesh.primitive_cube_add(size=1,location=P(p));o=bpy.context.object;o.name=n;o.scale=(s[0],s[2],s[1]);bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m);move(o,col);return o
def ball(n,p,s,m,col):
 bpy.ops.mesh.primitive_uv_sphere_add(segments=16,ring_count=8,radius=1,location=P(p));o=bpy.context.object;o.name=n;o.scale=(s[0],s[2],s[1]);bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);o.data.materials.append(m);move(o,col);return o
def rod(n,a,b,r,m,col='WORKSHOP'):
 a=P(a);b=P(b);bpy.ops.mesh.primitive_cylinder_add(vertices=12,radius=r,depth=(b-a).length,location=(a+b)/2);o=bpy.context.object;o.name=n;o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler();bpy.ops.object.transform_apply(location=False,rotation=True,scale=True);o.data.materials.append(m);move(o,col);return o
def join(parts,n,col):
 bpy.ops.object.select_all(action='DESELECT')
 for o in parts:o.select_set(True)
 bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();o=bpy.context.object;o.name=n;bpy.context.scene.cursor.location=(0,0,0);bpy.ops.object.origin_set(type='ORIGIN_CURSOR');return o
def torn_sleeve():
 # Same asymmetric seam used by TornFabric. A solid sleeve with an actually missing lower patch.
 outline=[(.29,1.14),(.42,1.14)]+[(.38+x,.90+y) for x,y in reversed(edge)]
 verts=[P((x,y,z)) for z in (-.02,.085) for x,y in outline];n=len(outline)
 faces=[tuple(range(n-1,-1,-1)),tuple(range(n,2*n))]+[(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)]
 mesh=bpy.data.meshes.new('KhaiTornCuff_Source');mesh.from_pydata(verts,[],faces);mesh.update();o=bpy.data.objects.new('KhaiRightSleeve_MatchingSeam',mesh);COL['CHARACTERS'].objects.link(o);o.data.materials.append(jacket);return o
def sole_mesh(name,height,material):
 outline=[(-.05,-.13),(.015,-.13),(.015,-.10),(.05,-.10),(.05,.10),(.03,.13),(-.03,.13),(-.05,.10)]
 verts=[P((x,y,z)) for y in (0,height) for x,z in outline];n=len(outline);faces=[tuple(range(n-1,-1,-1)),tuple(range(n,2*n))]+[(i,(i+1)%n,(i+1)%n+n,i+n) for i in range(n)]
 mesh=bpy.data.meshes.new(name);mesh.from_pydata(verts,[],faces);mesh.update();o=bpy.data.objects.new(name,mesh);COL['EVIDENCE'].objects.link(o);o.data.materials.append(material);return o
# Workshop local origin is floor level; doorway opens towards existing market.
parts=[box('WorkshopFloor',(0,.04,0),(10,.08,12),wood)]
parts += [box('WestWall',(-5,2.5,0),(.18,5,12),plaster),box('EastWall',(5,2.5,0),(.18,5,12),plaster),box('NorthWall',(0,2.5,6),(10,5,.18),plaster)]
parts += [box('DoorLeft',(-3.25,2.5,-6),(3.5,5,.18),plaster),box('DoorRight',(3.25,2.5,-6),(3.5,5,.18),plaster),box('DoorLintel',(0,4,-6),(3,2,.18),plaster)]
for x in (-4.7,0,4.7):parts.append(box('RoofBeam',(x,4.9,0),(.15,.22,12),wood))
parts.append(box('Loft',(0,2.9,3.6),(9.6,.2,4.2),wood))
for i in range(18):parts.append(box('Stair_%02d'%i,(-3.9,(i+1)*3/18/2,-3.6+(i+.5)*5.1/18),(1.3,(i+1)*3/18,5.1/18),wood))
for x in (-4.65,-3.2):
 rod('StairHandrail', (x,1,-3.6),(x,4,1.5),.035,steel)
 for z in (-3.6,-1.05,1.5):rod('StairPost',(x,3*(z+3.6)/5.1,z),(x,1+3*(z+3.6)/5.1,z),.035,steel)
for a,b in [((-3,4.1,1.5),(0,4.1,1.5)),((1.5,4.1,1.5),(4.8,4.1,1.5))]:
 rod('FixedRail',a,b,.045,steel)
 for i in range(7):
  x=a[0]+(b[0]-a[0])*i/6;rod('RailPost',(x,3,1.5),(x,4.1,1.5),.032,steel)
for x,z in [(-2,0),(2,-2)]:
 parts.append(box('BenchTop',(x,.85,z),(2,.12,1),wood))
 for dx in (-.85,.85):
  for dz in (-.38,.38):parts.append(box('BenchLeg',(x+dx,.4,z+dz),(.10,.8,.10),wood))
# Remaining rods joined with room so exported collision matches source.
parts += [o for o in COL['WORKSHOP'].objects if o not in parts]
room=join(parts,'WorkshopShell','WORKSHOP')
# Gate pivot exactly at hinge, separate latch and release rope; no open-lock catch.
gate=join([rod('GateTop',(0,1.05,0),(1.5,1.05,0),.045,green,'MECHANISM'),rod('GateBottom',(0,.15,0),(1.5,.15,0),.04,green,'MECHANISM')]+[rod('GatePicket',(x,.15,0),(x,1.05,0),.028,green,'MECHANISM') for x in (0,.3,.6,.9,1.2,1.5)],'GateLeaf_L','MECHANISM')
latch=box('Latch_C',(1.51,.92,0),(.18,.07,.10),steel,'MECHANISM')
rope=rod('ReleaseRope_D',(1.54,.92,.03),(1.54,-1.8,-6),.014,pale,'MECHANISM')
hook=rod('Hook_M',(0,0,0),(.12,.03,.02),.022,steel,'MECHANISM')
handle=box('Handle_DK',(0,0,0),(.18,.14,.08),wood,'MECHANISM')
wedge=box('Wedge',(0,.04,0),(.25,.08,.12),wood,'MECHANISM')
# Inspection meshes: not runtime-created primitives.
paper=box('EvidencePaper',(0,0,0),(.24,.006,.34),pale,'EVIDENCE')
phone=join([box('PhoneBody',(0,0,0),(.085,.012,.15),steel,'EVIDENCE'),box('PhoneScreen',(0,.007,0),(.072,.002,.13),ink,'EVIDENCE')],'RepairPhone','EVIDENCE')
# Ragged cuff/fragment uses identical asymmetric edge, not only similar colour.
edge=[(-.06,0),(-.04,.015),(-.02,-.007),(.004,.024),(.024,.006),(.06,.018)]
verts=[(x,0,z) for x,z in edge]+[(.06,0,-.08),(-.06,0,-.08)]
mesh=bpy.data.meshes.new('TornSeamGeometry');mesh.from_pydata(verts,[],[tuple(range(8))]);mesh.update();frag=bpy.data.objects.new('TornFabric',mesh);COL['EVIDENCE'].objects.link(frag);frag.data.materials.append(jacket)
sol=frag.modifiers.new('FabricThickness','SOLIDIFY');sol.thickness=.004;bpy.context.view_layer.objects.active=frag;frag.select_set(True);bpy.ops.object.modifier_apply(modifier=sol.name);frag.select_set(False)
shoe=join([sole_mesh('NotchedHeelSole',.05,rubber),box('Upper',(0,.07,.02),(.094,.09,.20),trouser,'EVIDENCE')]+[box('Tread',(-.027+i*.027,-.002,-.08+j*.04),(.018,.005,.022),mark,'EVIDENCE') for i in range(3) for j in range(5) if not(i==2 and j==0)],'ShoeSample','EVIDENCE')
printmesh=join([sole_mesh('ShoePrint_HeelNotch',.0006,ink)]+[box('PrintTread',(-.027+i*.027,.0015,-.08+j*.04),(.018,.001,.022),mark,'EVIDENCE') for i in range(3) for j in range(5) if not(i==2 and j==0)],'ShoePrint','EVIDENCE')
trace=join([box('SlideTrace',(.02*math.sin(i),.001,i*.055),(.09,.002,.04),mark,'EVIDENCE') for i in range(15)],'SlideMarks','EVIDENCE')
staged=join([rod('StagedRope',a,b,.012,pale,'EVIDENCE') for a,b in zip([(-1.4,.9,1),(-2,3.8,1.45),(-2,4.15,1.45),(-2,4.15,1.57),(-2,3.8,1.57)],[(-2,3.8,1.45),(-2,4.15,1.45),(-2,4.15,1.57),(-2,3.8,1.57),(-2.1,2.5,1.6)])],'StagedRope_R03','EVIDENCE')
def actor(name,cloth,puppet=False):
 col='PUPPETS' if puppet else 'CHARACTERS';pieces=[]; assignments=[]
 def add(o,b):pieces.append(o);assignments.append((o,b));return o
 bodymat=wood if puppet else cloth;sm=wood if puppet else skin
 add(ball(name+'Torso',(0,1.17,0),(.23,.28,.145),bodymat,col),'spine')
 add(ball(name+'Hip',(0,.94,0),(.18,.13,.125),bodymat,col),'root')
 add(rod(name+'Neck',(0,1.38,0),(0,1.53,0),.07,sm,col),'head')
 add(ball(name+'Head',(0,1.60,0),(.13,.18,.125),sm,col),'head')
 if not puppet:
  add(ball(name+'Hair',(0,1.70,-.015),(.134,.085,.126),hair,col),'head')
  add(ball(name+'Nose',(0,1.59,.126),(.03,.035,.03),skin,col),'head')
  for x in (-.047,.047):add(ball(name+'Eye',(x,1.64,.119),(.012,.009,.009),ink,col),'head')
  if name=='Ha':add(ball('HaHairBun',(0,1.69,-.12),(.075,.075,.075),hair,col),'head');add(box('HaApron',(0,1.07,.146),(.30,.52,.014),apron,col),'spine')
  if name=='Khai':
   for y in (1.04,1.18,1.32):add(ball('JacketButton',(0,y,.144),(.012,.012,.008),steel,col),'spine')
 bones=[('root',(0,.86,0),(0,1.01,0),None),('spine',(0,1.01,0),(0,1.41,0),'root'),('head',(0,1.41,0),(0,1.76,0),'spine')]
 for sign,suf in [(-1,'L'),(1,'R')]:
  shoulder=(sign*.22,1.37,0);elbow=(sign*.33,1.10,.025);wrist=(sign*.38,.88,.04)
  hip=(sign*.11,.94,0);knee=(sign*.115,.52,.025);ankle=(sign*.115,.13,0)
  bones += [('upper'+suf,shoulder,elbow,'spine'),('fore'+suf,elbow,wrist,'upper'+suf),('hand'+suf,wrist,(sign*.39,.79,.045),'fore'+suf),('thigh'+suf,hip,knee,'root'),('shin'+suf,knee,ankle,'thigh'+suf)]
  for n,a,b,r,m in [('upper',shoulder,elbow,.075,bodymat),('fore',elbow,wrist,.058,bodymat),('thigh',hip,knee,.09,wood if puppet else trouser),('shin',knee,ankle,.064,wood if puppet else trouser)]:add(torn_sleeve() if name=='Khai' and n=='fore' and suf=='R' else rod(name+n+suf,a,b,r,m,col),n+suf)
  for n,p,r in [('upper',shoulder,.09),('fore',elbow,.064),('hand',wrist,.056),('thigh',hip,.09),('shin',knee,.068)]:add(ball(name+'Joint'+n+suf,p,(r,r,r),sm,col),n+suf)
  add(ball(name+'Hand'+suf,(sign*.39,.82,.045),(.047,.07,.04),sm,col),'hand'+suf)
  add(box(name+'Foot'+suf,(sign*.115,.065,.055),(.13,.13,.27),wood if puppet else rubber,col),'shin'+suf)
 # Join weighted rigid islands; each vertex has exactly one explicit source bone.
 for o,b in assignments:
  vg=o.vertex_groups.new(name=b);vg.add(list(range(len(o.data.vertices))),1,'REPLACE')
 body=join(pieces,name+'_Body',col)
 arm=bpy.data.armatures.new(name+'_Skeleton');rig=bpy.data.objects.new(name+'_Rig',arm);COL[col].objects.link(rig);bpy.context.view_layer.objects.active=rig;rig.select_set(True);body.select_set(False);bpy.ops.object.mode_set(mode='EDIT')
 for n,a,b,parent in bones:
  bone=arm.edit_bones.new(n);bone.head=P(a);bone.tail=P(b)
  if parent:bone.parent=arm.edit_bones[parent]
 bpy.ops.object.mode_set(mode='OBJECT');mod=body.modifiers.new('SinglePoseOwner','ARMATURE');mod.object=rig;body.parent=rig
 # Cuff sample is deliberately a separate inspectable evidence object; mesh actor is provisional.
 rig['prototype_art']='Self-authored stylized proportions; no face-Minh dependency';rig['rig_owner']='InvestigationActor only, no Animator second owner'
 return rig,body
khai,kbody=actor('Khai',jacket);ha,hbody=actor('Ha',plaster);r03,rbody=actor('R03',wood,True);r12,r12body=actor('R12',wood,True)
# Garment sample is the very same source jacket faces, not a paper representing an unseen coat.
indices=[p for p in kbody.data.polygons if kbody.data.materials[p.material_index]==jacket]
used=sorted({v for p in indices for v in p.vertices});lookup={v:i for i,v in enumerate(used)}
cm=bpy.data.meshes.new('KhaiCoatSameGeometry');cm.from_pydata([kbody.data.vertices[v].co for v in used],[],[[lookup[v] for v in p.vertices] for p in indices]);cm.update();coat=bpy.data.objects.new('CoatSample',cm);COL['EVIDENCE'].objects.link(coat);cm.materials.append(jacket)
# Lay the same jacket geometry on the inspection bench; retain the torn seam, no per-scene pivot patch.
for v in cm.vertices:
 ux,uy,uz=-v.co.x,v.co.z,-v.co.y
 v.co=P((ux,-uz,uy-1.17))
minimum=min(v.co.z for v in cm.vertices)
for v in cm.vertices:v.co.z-=minimum
for rig,p in [(khai,(-2,0,-1)),(ha,(2,0,-1)),(r03,(0,0,-1)),(r12,(0,0,2))]:rig.location=P(p)
# Snapshot all source meshes with clean normals and applied unit scale.
for o in scene.objects:
 if o.type=='MESH':
  bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(o.data);bm.free()
# Material-only meshes do not require external texture files.
scene.render.engine='BLENDER_WORKBENCH';scene.display.shading.light='STUDIO';scene.display.shading.color_type='MATERIAL';scene.display.shading.show_shadows=True;scene.display.shading.show_cavity=True;scene.render.resolution_x=1200;scene.render.resolution_y=850;scene.render.resolution_percentage=100
bpy.ops.object.camera_add(location=P((13,11,-16)));cam=bpy.context.object;move(cam,'REVIEW');cam.rotation_euler=(P((0,1.7,0))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.type='PERSP';cam.data.lens=42;scene.camera=cam
scene.render.filepath=str(E/'blender-workshop.png');bpy.ops.render.render(write_still=True)
# Actor review outside workshop for silhouette / neck / joints.
COL['WORKSHOP'].hide_render=True;COL['MECHANISM'].hide_render=True;COL['EVIDENCE'].hide_render=True
cam.location=P((4,2.4,7));cam.rotation_euler=(P((0,1,-1))-cam.location).to_track_quat('-Z','Y').to_euler();scene.render.filepath=str(E/'blender-actors.png');bpy.ops.render.render(write_still=True)
for c in COL.values():c.hide_render=False
bpy.ops.wm.save_as_mainfile(filepath=str(O/'Workshop_Investigation_R102_Editable.blend'))
# Gate each prop separately; room intentionally consists of disconnected construction members.
gates={}
for o in [gate,latch,handle,phone,paper,frag,shoe,coat,kbody,hbody,rbody,r12body]:
 gates[o.name]=bx_audit.deliverable([o],profile='game')
(D/'blender-gates.json').write_text(json.dumps(gates,default=str,indent=2))
# Evaluate external candidate without copying into project assets.
try:
 bpy.ops.import_scene.gltf(filepath=str(R/'Test/Tam/r102-kenney-character.glb'))
 ext=[o for o in scene.objects if o.name not in {x.name for c in COL.values() for x in c.objects}]
 (D/'external-candidate.json').write_text(json.dumps([{'name':o.name,'type':o.type,'bounds':[list(v) for v in o.bound_box] if o.type=='MESH' else None} for o in ext],indent=2))
except Exception as ex:(D/'external-candidate.json').write_text(json.dumps({'error':str(ex)}))
print('AUTHOR SOURCE SAVED; review renders before export')
