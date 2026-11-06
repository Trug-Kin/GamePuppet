import bpy,sys,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r102';E=R/'Test/BangChung/investigation_r102';O=R/'Assets/Art/InvestigationV36/R102';O.mkdir(parents=True,exist_ok=True)
sys.path.extend([str(R/'.agents/skills/scenario-blender-expert/scripts'),str(R/'.agents/skills/scenario-blender-export/scripts')]);import bx_audit,bx_export
source=R/'Source/Blender/JourneyV48/R102/Workshop_Investigation_R102_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(source));s=bpy.context.scene
assert not bpy.data.libraries
assert not any(i.source=='FILE' and not i.packed_file and not Path(bpy.path.abspath(i.filepath)).exists() for i in bpy.data.images)
assert s.unit_settings.scale_length==1
g=json.loads((D/'blender-gates.json').read_text());assert all(x['pass'] for x in g.values()),'Source gate errors remain'
result={}
for col in ['WORKSHOP','MECHANISM','EVIDENCE']:
 obs=list(bpy.data.collections[col].objects)
 target=O/(col+'.fbx');result[col]=bx_export.export(str(target),'unity',objects=obs)
 result[col]['roundtrip']=bx_export.round_trip(str(target),obs,preset='unity')
for name in ['Khai','Ha','R03','R12']:
 rig=bpy.data.objects[name+'_Rig'];body=bpy.data.objects[name+'_Body'];rig.location=(0,0,0)
 target=O/(name+'.fbx');result[name]=bx_export.export(str(target),'unity_skeletal',objects=[rig,body],bake_anim=False)
 result[name]['roundtrip']=bx_export.round_trip(str(target),[rig,body],preset='unity_skeletal',bake_anim=False)
(D/'fbx-roundtrip.json').write_text(json.dumps(result,default=str,indent=2))
(D/'source-reopen.json').write_text(json.dumps({'file':str(source),'opened':True,'units':s.unit_settings.scale_length,'missing_textures':False,'external_libraries':0,'objects':len(s.objects),'exported':list(result)},indent=2))
print('SOURCE REOPEN / FBX EXPORTED AND ROUNDTRIP CHECKED')
