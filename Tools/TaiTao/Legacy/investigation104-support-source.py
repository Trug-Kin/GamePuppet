import bpy,math,json,sys
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R104';A=R/'Assets/Art/InvestigationV36/R104';D=R/'Test/KetQua/investigation_r104'
bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'),load_ui=False,use_scripts=False)
col=bpy.data.collections.get('R104_R12_REPAIR_SUPPORT') or bpy.data.collections.new('R104_R12_REPAIR_SUPPORT')
if col.name not in bpy.context.scene.collection.children:bpy.context.scene.collection.children.link(col)
def P(v):return Vector((-v[0],-v[2],v[1]))
material=bpy.data.materials.new('R104_repair_stand_wood');material.diffuse_color=(.24,.14,.06,1)
def beam(name,a,b,r):
 p=P(a);q=P(b);bpy.ops.mesh.primitive_cylinder_add(vertices=12,radius=r,depth=(q-p).length,location=(p+q)*.5);o=bpy.context.object;o.name=name;o.rotation_euler=(q-p).to_track_quat('Z','Y').to_euler();bpy.ops.object.transform_apply(location=True,rotation=True,scale=True);o.data.materials.append(material)
 for c in list(o.users_collection):c.objects.unlink(o)
 col.objects.link(o);return o
objects=[beam('R12_stand_left',(-.55,0,.35),(-.55,2.1,.35),.035),beam('R12_stand_right',(.55,0,.35),(.55,2.1,.35),.035),beam('R12_stand_top',(-.55,2.1,.35),(.55,2.1,.35),.045),beam('R12_stand_foot_left',(-.55,.025,-.2),(-.55,.025,.65),.04),beam('R12_stand_foot_right',(.55,.025,-.2),(.55,.025,.65),.04)]
bpy.ops.wm.save_as_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'));names=[o.name for o in objects];bpy.ops.wm.open_mainfile(filepath=str(S/'Repair_R12_R104_Animation.blend'),load_ui=False,use_scripts=False);objects=[bpy.data.objects[n] for n in names]
sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export as X
export=X.export(str(A/'R12Support.fbx'),'unity',objects);rt=X.round_trip(str(A/'R12Support.fbx'),objects,preset='unity')
(D/'repair-support-source.json').write_text(json.dumps(dict(source=str(S/'Repair_R12_R104_Animation.blend'),collection=col.name if False else 'R104_R12_REPAIR_SUPPORT',reopened=True,export=export,roundtrip=rt),default=str,indent=2));print('R12 editable stand exported',rt.get('pass'))
