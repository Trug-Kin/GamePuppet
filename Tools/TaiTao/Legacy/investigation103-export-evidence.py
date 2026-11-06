import bpy,sys,json,bmesh
from pathlib import Path
R=Path('D:/ConRoi');S=R/'Source/Blender/JourneyV48/R103/Workshop_Investigation_R103_Editable.blend';A=R/'Assets/Art/InvestigationV36/R103';D=R/'Test/KetQua/investigation_r103'
bpy.ops.wm.open_mainfile(filepath=str(S))
for name in ['ShoeSample','ShoeControl']:
 o=bpy.data.objects[name]
 for v in o.data.vertices:
  if v.co.z<-.001:v.co.z+=.0015
 o.data.update()
frag=bpy.data.objects['TornFabric']
for v in frag.data.vertices:
 if abs(v.co.z-.002)<1e-6:v.co.z=.0018
for o in list(bpy.data.collections['EVIDENCE'].objects):
 if o.type=='MESH':
  bm=bmesh.new();bm.from_mesh(o.data);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(o.data);bm.free();o.data.update()
sys.path.extend([str(R/'.agents/skills/scenario-blender-export/scripts'),str(R/'.agents/skills/scenario-blender-expert/scripts')]);import bx_export,bx_audit
gate={n:bx_audit.deliverable([bpy.data.objects[n]],profile='game') for n in ['ShoeSample','ShoeControl','TornFabric','CoatSample']}
bpy.ops.wm.save_as_mainfile(filepath=str(S));bpy.ops.wm.open_mainfile(filepath=str(S));obs=list(bpy.data.collections['EVIDENCE'].objects)
ex=bx_export.export(str(A/'EVIDENCE.fbx'),'unity',objects=obs);rt=bx_export.round_trip(str(A/'EVIDENCE.fbx'),obs,preset='unity')
(D/'evidence-export.json').write_text(json.dumps(dict(gates=gate,export=ex,roundtrip=rt,reopened=True),default=str,indent=2));print('EVIDENCE roundtrip',rt['pass'],'gate errors',{n:g['errors'] for n,g in gate.items()})
