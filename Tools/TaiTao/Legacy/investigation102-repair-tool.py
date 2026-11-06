import bpy,sys,json,shutil,bmesh
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');p=R/'Source/Blender/JourneyV48/R102/Workshop_Investigation_R102_Editable.blend';b=p.with_name('Workshop_R102_Before_RepairTool.blend');assert not b.exists();shutil.copy2(p,b);bpy.ops.wm.open_mainfile(filepath=str(p));assert 'RepairTool' not in bpy.data.objects
verts=[];faces=[];mi=[]
def box(center,size,mat):
 start=len(verts)
 for i in range(8):
  x=center[0]+size[0]*(.5 if i&1 else -.5);y=center[1]+size[1]*(.5 if i&2 else -.5);z=center[2]+size[2]*(.5 if i&4 else -.5);verts.append((-x,-z,y))
 for f in [(0,4,6,2),(1,3,7,5),(0,1,5,4),(2,6,7,3),(0,2,3,1),(4,5,7,6)]:faces.append([start+i for i in f]);mi.append(mat)
box((0,.02,0),(.06,.14,.05),0);box((0,.17,0),(.014,.19,.014),1);box((0,.28,0),(.025,.03,.008),1)
mesh=bpy.data.meshes.new('RepairToolMesh');mesh.from_pydata(verts,[],faces);mesh.update();o=bpy.data.objects.new('RepairTool',mesh);bpy.data.collections['EVIDENCE'].objects.link(o);grip=bpy.data.materials.new('Repair tool ochre');grip.diffuse_color=(.95,.57,.06,1);mesh.materials.append(grip);metal=bpy.data.materials.get('Oxidized steel') or bpy.data.materials.new('Repair tool steel');metal.diffuse_color=(.4,.46,.5,1);mesh.materials.append(metal)
for i,f in enumerate(mesh.polygons):f.material_index=mi[i]
bm=bmesh.new();bm.from_mesh(mesh);bmesh.ops.recalc_face_normals(bm,faces=list(bm.faces));bm.to_mesh(mesh);bm.free();bpy.ops.wm.save_as_mainfile(filepath=str(p));bpy.ops.wm.open_mainfile(filepath=str(p));o=bpy.data.objects['RepairTool'];assert not bpy.data.libraries
sys.path.extend([str(R/'.agents/skills/scenario-blender-expert/scripts'),str(R/'.agents/skills/scenario-blender-export/scripts')]);import bx_export,bx_audit
gate=bx_audit.deliverable([o],profile='game');assert gate['pass'],gate
obs=list(bpy.data.collections['EVIDENCE'].objects);target=R/'Assets/Art/InvestigationV36/R102/EVIDENCE.fbx';ex=bx_export.export(str(target),'unity',objects=obs);rt=bx_export.round_trip(str(target),obs,preset='unity');(R/'Test/KetQua/investigation_r102/repair-tool-source.json').write_text(json.dumps(dict(source=str(p),reopened=True,gate=gate,roundtrip=rt,export=ex),default=str,indent=2));print('REPAIR TOOL REOPEN/EXPORT/ROUNDTRIP DONE')
