"""Editable source for r91 rigid head/socket/hand/limb repairs. No gameplay code."""
import bpy,bmesh,sys,json,math
from pathlib import Path
from mathutils import Vector
R=Path('D:/Game/GameHorror/Game'); O=R/'Docs/Reviews/journey_v48/r91'; A=R/'Assets/Art/Characters/R91'
A.mkdir(parents=True,exist_ok=True)
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(R/'Docs/Reviews/journey_v48/r47/puppet-identity-r47-source.blend'))
h=bpy.data.objects['R47_Minh_Derived_Object_3'];h=h.copy();h.data=h.data.copy();bpy.context.scene.collection.objects.link(h);h.name='R91_PuppetHead_Legacy'
bm=bmesh.new();bm.from_mesh(h.data)
bmesh.ops.bisect_plane(bm,geom=list(bm.verts)+list(bm.edges)+list(bm.faces),dist=.000001,plane_co=(0,0,1.478),plane_no=(0,0,1),clear_inner=True)
boundary=[e for e in bm.edges if e.is_boundary];bmesh.ops.holes_fill(bm,edges=boundary,sides=0)
bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bmesh.ops.triangulate(bm,faces=list(bm.faces));bm.to_mesh(h.data);bm.free()
for p in h.data.polygons:p.use_smooth=True
portrait=h.copy();portrait.data=h.data.copy();bpy.context.scene.collection.objects.link(portrait);portrait.name='R91_Portrait'
for v in portrait.data.vertices:v.co=Vector((-(v.co.x-.013),v.co.y,v.co.z-1.586))
portrait.location=(0,0,0);portrait.rotation_euler=(0,0,0);portrait.scale=(1,1,1)
bm=bmesh.new();bm.from_mesh(portrait.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(portrait.data);bm.free()
wood=h.data.materials[0]
def finish(o,name):
 o.name=name;o.data.name=name;o.data.materials.clear();o.data.materials.append(wood)
 bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bmesh.ops.triangulate(bm,faces=list(bm.faces));bm.to_mesh(o.data);bm.free()
 if not o.data.uv_layers:o.data.uv_layers.new(name='UVMap')
 for p in o.data.polygons:p.use_smooth=True
 return o
bpy.ops.mesh.primitive_uv_sphere_add(segments=24,ring_count=16,location=(-3,0,1.483));socket=bpy.context.object;socket.scale=(.063,.052,.044)
bpy.ops.object.transform_apply(location=True,rotation=True,scale=True);finish(socket,'R91_NeckSocket_Legacy')
# Centered, one metre link; runtime uses the actual imported extent, never a cylinder-height assumption.
bpy.ops.mesh.primitive_cylinder_add(vertices=16,radius=.5,depth=1);link=bpy.context.object;finish(link,'R91_ArmLink')
bev=link.modifiers.new('rounded wooden edges','BEVEL');bev.width=.055;bev.segments=3;bpy.context.view_layer.objects.active=link;bpy.ops.object.modifier_apply(modifier=bev.name);finish(link,'R91_ArmLink')
# Hand origin is the wrist; fingers point down local Z. One joined rigid wooden mesh.
pieces=[]
def cube(p,s):
 bpy.ops.mesh.primitive_cube_add(size=1,location=p);o=bpy.context.object;o.scale=s;bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);pieces.append(o);return o
cube((0,0,-.045),(.075,.035,.09))
for x in (-.027,-.009,.009,.027):cube((x,0,-.111),(.014,.028,.055))
cube((.047,0,-.048),(.025,.029,.06))
bpy.ops.object.select_all(action='DESELECT')
for o in pieces:o.select_set(True)
bpy.context.view_layer.objects.active=pieces[0];bpy.ops.object.join();hand=pieces[0];bpy.context.scene.cursor.location=(0,0,0);bpy.ops.object.origin_set(type='ORIGIN_CURSOR');finish(hand,'R91_RigidHand_Legacy')
bev=hand.modifiers.new('soft carved hand edges','BEVEL');bev.width=.003;bev.segments=2;bpy.ops.object.modifier_apply(modifier=bev.name);finish(hand,'R91_RigidHand_Legacy')
bpy.ops.mesh.primitive_uv_sphere_add(segments=20,ring_count=12,radius=.5);joint=finish(bpy.context.object,'R91_Joint')
parts=[h,portrait,socket,link,hand,joint]
# The push lean is an authored rig channel, sampled here for the existing R28 owner.
# Legacy puppet-root axes are Z-up; the hand and paper interaction remain under R28.
pivot=bpy.data.objects.new('R91_PushLean',None);bpy.context.scene.collection.objects.link(pivot);pivot.location=(-3,0,.82);pivot.rotation_mode='AXIS_ANGLE'
axis=Vector((-.65,.76,0)).normalized()
for frame,angle in ((0,0),(9,0),(27,26),(54,26)):
 pivot.rotation_axis_angle=(math.radians(angle),*axis);pivot.keyframe_insert(data_path='rotation_axis_angle',frame=frame)
angles=[]
for frame in range(55):bpy.context.scene.frame_set(frame);angles.append(math.degrees(pivot.rotation_axis_angle[0]))
push={'source':'Docs/Reviews/journey_v48/r91/character-repairs-r91-source.blend','duration':1.8,'axis':dict(zip('xyz',axis)),'angles':angles,'upperLength':.37,'foreLength':.36}
(A/'SchoolPushPoseR91.json').write_text(json.dumps(push,indent=2))
routing={'anchors':[[.5,.75,1],[1,.85,.02],[0,.85,.02],[1,.03,.85],[0,.03,.85]],'clearance':[[0,.02,.06],[.07,.08,.06],[-.07,.08,.06],[.15,.14,.05],[-.15,.14,.05]],'topClearance':.24}
for k in ('anchors','clearance'):routing[k]=[dict(zip('xyz',v)) for v in routing[k]]
(A/'ThreadRoutingR91.json').write_text(json.dumps(routing,indent=2))
for i in range(5):
 e=bpy.data.objects.new('R91_Normalized_Thread_Anchor_'+str(i),None);bpy.context.scene.collection.objects.link(e);e.location=tuple(routing['anchors'][i].values());e['coordinates']='normalized mesh bounds; runtime follows the articulated piece, finite routed cord ends .24m above crown'

bpy.context.scene.frame_set(0)
for o in bpy.context.scene.objects:
 o.hide_render=o not in parts;o.hide_set(o not in parts)
 for m in o.modifiers:
  if m.type=='ARMATURE':raise RuntimeError('Rigid assets must not inherit an armature')
for o in parts:
 o['r91_source']='R47 Minh-derived head; jaw crop removes unintended neck bulb. Socket and rigid hand authored here.'
 o['units']='metres; legacy objects retain original Blender Z-up mesh coordinates; portrait is centered, faces +Y before Unity export'
audit={}
for o in parts:
 bm=bmesh.new();bm.from_mesh(o.data);audit[o.name]={'vertices':len(bm.verts),'triangles':sum(len(f.verts)-2 for f in bm.faces),'boundary_edges':sum(e.is_boundary for e in bm.edges),'nonmanifold_edges':sum(not e.is_manifold for e in bm.edges),'bounds':[[min(v.co[i] for v in o.data.vertices),max(v.co[i] for v in o.data.vertices)] for i in range(3)]};bm.free()
assert audit[h.name]['boundary_edges']==0
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(O/'character-repairs-r91-source.blend'),compress=True)
exports={}
for o in parts:
 exports[o.name]=bx_export.export(str(A/(o.name+'.fbx')),'unity',[o])
(O/'character-source-audit.json').write_text(json.dumps({'audit':audit,'exports':exports},indent=2,default=str))
print('R91_CHARACTER_SOURCE_COMPLETE',audit,flush=True)
