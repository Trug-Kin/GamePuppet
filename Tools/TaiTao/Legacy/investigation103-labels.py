import bpy,math,sys,json
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R103/Workshop_Investigation_R103_Editable.blend';A=R/'Assets/Art/InvestigationV36/R103'
bpy.ops.wm.open_mainfile(filepath=str(S));col=bpy.data.collections['EVIDENCE'];scene=bpy.context.scene
def P(v):return Vector((-v[0],-v[2],v[1]))
def move(o):
 for c in list(o.users_collection):c.objects.unlink(o)
 col.objects.link(o);return o
for text,name in [('R-03','Label_R03'),('R-12','Label_R12'),('R-12 / TAY PHAI','RepairCard')]:
 old=bpy.data.objects.get(name)
 if old:bpy.data.objects.remove(old,do_unlink=True)
 bpy.ops.mesh.primitive_cube_add(size=1,location=P((0,1.19,.17)));back=move(bpy.context.object);back.scale=(.28,.012,.12);bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);back.data.materials.append(bpy.data.materials['R103 label ivory'])
 bpy.ops.object.text_add(location=P((.12,1.16,.18)));t=move(bpy.context.object);t.data.body=text;t.data.size=.075;t.data.extrude=.0005;t.rotation_euler=(math.pi/2,0,0);t.data.materials.append(bpy.data.materials['R103 charcoal impression']);bpy.ops.object.convert(target='MESH');bpy.ops.object.select_all(action='DESELECT');back.select_set(True);t.select_set(True);bpy.context.view_layer.objects.active=back;bpy.ops.object.join();back.name=name;scene.cursor.location=(0,0,0);bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
bpy.ops.wm.save_as_mainfile(filepath=str(S));bpy.ops.wm.open_mainfile(filepath=str(S));sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
obs=list(bpy.data.collections['EVIDENCE'].objects);ex=bx_export.export(str(A/'EVIDENCE.fbx'),'unity',objects=obs);rt=bx_export.round_trip(str(A/'EVIDENCE.fbx'),obs,preset='unity')
(R/'Test/KetQua/investigation_r103/label-export.json').write_text(json.dumps(dict(reopened=True,roundtrip=rt,export=ex),default=str,indent=2));print('Fixed label facing from front of rotated actors; exported',rt['pass'])
