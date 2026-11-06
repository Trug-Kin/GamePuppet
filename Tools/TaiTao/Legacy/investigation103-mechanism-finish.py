import bpy,sys,json,math
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R103/Workshop_Investigation_R103_Editable.blend';A=R/'Assets/Art/InvestigationV36/R103';D=R/'Test/KetQua/investigation_r103'
bpy.ops.wm.open_mainfile(filepath=str(S));col=bpy.data.collections['MECHANISM'];scene=bpy.context.scene
def P(v):return Vector((-v[0],-v[2],v[1]))
def move(o):
 for c in list(o.users_collection):c.objects.unlink(o)
 col.objects.link(o);return o
post=bpy.data.objects.get('ControlPost_DK')
if not post:
 bpy.ops.mesh.primitive_cube_add(size=1,location=P((0,.6,0)));post=move(bpy.context.object);post.name='ControlPost_DK';post.scale=(.09,.09,1.2);bpy.ops.object.transform_apply(location=False,rotation=False,scale=True);scene.cursor.location=(0,0,0);bpy.ops.object.origin_set(type='ORIGIN_CURSOR');post.data.materials.append(bpy.data.materials['Dark metal'])
wedge=bpy.data.objects['Wedge']
if wedge.dimensions.z<.1:
 for v in wedge.data.vertices:v.co.z*=2
 wedge.location.z=.08
old=bpy.data.objects.get('Hook_M');bpy.data.objects.remove(old,do_unlink=True);parts=[]
points=[(-.12,0,0),(-.12,.14,0),(.02,.14,0),(.02,.02,0)]
for aa,bb in zip(points,points[1:]):
 a=P(aa);b=P(bb);bpy.ops.mesh.primitive_cylinder_add(vertices=12,radius=.018,depth=(b-a).length+.025,location=(a+b)/2);o=move(bpy.context.object);o.rotation_euler=(b-a).to_track_quat('Z','Y').to_euler();bpy.ops.object.transform_apply(location=False,rotation=True,scale=True);o.data.materials.append(bpy.data.materials['Dark metal']);parts.append(o)
bpy.ops.object.select_all(action='DESELECT')
for o in parts:o.select_set(True)
bpy.context.view_layer.objects.active=parts[0];bpy.ops.object.join();o=bpy.context.object;o.name='Hook_M';scene.cursor.location=(0,0,0);bpy.ops.object.origin_set(type='ORIGIN_CURSOR')
bpy.ops.wm.save_as_mainfile(filepath=str(S));bpy.ops.wm.open_mainfile(filepath=str(S));sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
obs=list(bpy.data.collections['MECHANISM'].objects);ex=bx_export.export(str(A/'MECHANISM.fbx'),'unity',objects=obs);rt=bx_export.round_trip(str(A/'MECHANISM.fbx'),obs,preset='unity');(D/'mechanism-source.json').write_text(json.dumps(dict(reopened=True,roundtrip=rt,export=ex),default=str,indent=2));print('Control post / hook / wedge source exported',rt['pass'])
