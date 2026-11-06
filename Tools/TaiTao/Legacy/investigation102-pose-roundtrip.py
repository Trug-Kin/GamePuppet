import bpy,json
from pathlib import Path
from mathutils import Matrix
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r102'
def bounds(body):
 dg=bpy.context.evaluated_depsgraph_get();o=body.evaluated_get(dg);m=o.to_mesh();p=[o.matrix_world@v.co for v in m.vertices];o.to_mesh_clear();return [[min(v[a] for v in p),max(v[a] for v in p)] for a in range(3)]
def read(imported=False):
 rig=next(o for o in bpy.context.scene.objects if o.type=='ARMATURE' and o.name.startswith('R03_Rig'));body=next(o for o in bpy.context.scene.objects if o.type=='MESH' and o.name.startswith('R03_Body'));rig.location=(0,0,0);out={}
 for name in ['Pose_'+c for c in 'ABCDEGHINORSTWY']+['RepairBothHands']:
  action=next(a for a in bpy.data.actions if a.name.endswith(name));rig.animation_data_create();rig.animation_data.action=action
  if imported and action.slots:rig.animation_data.action_slot=action.slots[0]
  bpy.context.scene.frame_set(2);bpy.context.scene.frame_set(1);bpy.context.view_layer.update();out[name]=bounds(body)
 return out
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R102/Puppet_Poses_R102_Editable.blend'));before=read()
bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.import_scene.fbx(filepath=str(R/'Assets/Art/InvestigationV36/R102/PuppetPoseLibrary.fbx'));after=read(True)
errors={k:max(abs(before[k][a][j]-after[k][a][j]) for a in range(3) for j in range(2)) for k in before}
(D/'pose-matched-roundtrip.json').write_text(json.dumps({'scope':'Same action/frame evaluated before/after, unlike initial active-action vs first-imported-action bounds comparison','max_bounds_delta':max(errors.values()),'per_pose':errors,'pass':max(errors.values())<.001},indent=2))
print(errors)
