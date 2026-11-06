import bpy,sys,json,bmesh
from pathlib import Path
R=Path('D:/ConRoi');p=R/'Source/Blender/JourneyV48/R102/Workshop_Investigation_R102_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(p));o=bpy.data.objects['RepairTool'];mats=list(o.data.materials);verts=[];faces=[];ids=[]
rings=[(-.05,.06,.05),(.07,.06,.05),(.08,.014,.014),(.25,.014,.014),(.26,.025,.008),(.295,.025,.008)]
for y,w,d in rings:
 for x,z in [(-w/2,-d/2),(w/2,-d/2),(w/2,d/2),(-w/2,d/2)]:verts.append((-x,-z,y))
faces.append((3,2,1,0));ids.append(0)
for k in range(len(rings)-1):
 for i in range(4):faces.append((k*4+i,k*4+(i+1)%4,(k+1)*4+(i+1)%4,(k+1)*4+i));ids.append(0 if k<2 else 1)
faces.append(tuple(range((len(rings)-1)*4,len(rings)*4)));ids.append(1)
mesh=bpy.data.meshes.new('RepairToolConnectedMesh');mesh.from_pydata(verts,[],faces);mesh.update();o.data=mesh
for m in mats:mesh.materials.append(m)
for i,f in enumerate(mesh.polygons):f.material_index=ids[i]
bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(mesh);bm.free();bpy.ops.wm.save_as_mainfile(filepath=str(p));bpy.ops.wm.open_mainfile(filepath=str(p));o=bpy.data.objects['RepairTool'];sys.path.extend([str(R/'.agents/skills/scenario-blender-expert/scripts'),str(R/'.agents/skills/scenario-blender-export/scripts')]);import bx_audit,bx_export
g=bx_audit.deliverable([o],profile='game');assert g['pass'],g;obs=list(bpy.data.collections['EVIDENCE'].objects);target=R/'Assets/Art/InvestigationV36/R102/EVIDENCE.fbx';ex=bx_export.export(str(target),'unity',objects=obs);rt=bx_export.round_trip(str(target),obs,preset='unity');(R/'Test/KetQua/investigation_r102/repair-tool-source.json').write_text(json.dumps(dict(source=str(p),reopened=True,gate=g,roundtrip=rt,export=ex),default=str,indent=2));print('CONNECTED TOOL GATE/REOPEN/ROUNDTRIP PASS')
