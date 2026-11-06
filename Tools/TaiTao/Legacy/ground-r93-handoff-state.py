import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/ground_r93';S=R/'Source/Blender/JourneyV48/R93';rows=[]
files=[('GroundSurvey_R93_Editable.blend','BASELINE_ONLY'),('Ground_I2_Transition_Trial_R93.blend','REJECTED_LAYER_REGRESSION'),('Ground_I2_South_Transition_Study_R93.blend','UNACCEPTED_TRANSITIONS'),('Ground_I2_Wide_Transition_Study_R93.blend','UNACCEPTED_TRANSITIONS'),('Ground_I2_Inner_Transition_Study_R93.blend','UNACCEPTED_TRANSITIONS')]
for name,status in files:
 p=S/name
 if not p.exists():continue
 bpy.ops.wm.open_mainfile(filepath=str(p));sc=bpy.context.scene;native=bpy.data.collections['GROUND_NATIVE_MATCHED_R93'];cc=bpy.data.collections.get('COLLIDERS_NOT_FOR_RENDER')
 if cc is None:cc=bpy.data.collections.new('COLLIDERS_NOT_FOR_RENDER');native.children.link(cc)
 for o in list(native.objects):
  if o.get('unity_path','').startswith('LakeGateMarketV32/Context_'):
   native.objects.unlink(o);cc.objects.link(o);o.hide_render=True
 cc.hide_render=True;cc.hide_viewport=True
 sc['handoff_status']=status;sc['integration']='NONE. Existing r93 scene and all product assets unchanged. This file does not authorize export/integration.'
 sc['units_note']='Authoring metres only; no surveyed local heights. Source matching tolerance 0.005 is provenance tolerance, not proof of field accuracy.'
 if status!='BASELINE_ONLY':
  sc['rejection_reason']='Road ray improvements alone do not accept a whole ground patch. Layer transitions, thin faces, render/collider equivalence, prop contacts and Unity/controller checks remain unresolved. Not a finished ground asset.'
  if 'Wide' in name:sc['study_envelope']='I2 southern sector, inner radius13.2 outer32 authoring units; earlier object text mentioning24 is historical; generator wide-study.py is exact recipe.'
  elif 'Inner' in name:sc['study_envelope']='I2 southern sector, inner radius12 outer24; visible island unchanged, inner road target includes estimated0.04 curb gap.'
  else:sc['study_envelope']='I2 radius13.2..24; see generator and numerical report for exact version.'
 txt=bpy.data.texts.get('READ_ME_GROUND_R93_HANDOFF') or bpy.data.texts.new('READ_ME_GROUND_R93_HANDOFF');txt.clear();txt.write('STATUS: '+status+'\nOpen GROUND_NATIVE_MATCHED_R93 for native editable render meshes. COLLIDERS_NOT_FOR_RENDER contains independent authoring collider meshes and is hidden. BEFORE_R93_DO_NOT_EXPORT and MEASUREMENTS_DO_NOT_EXPORT are diagnostic only.\nNo FBX exported, no prefab/scene integrated in this ground survey. GroundSurvey_R93_Editable is the audit baseline; the I2 files are unaccepted studies, not release candidates.\nSource/Blender/JourneyV48/R93/Langfarm_R93_Editable.blend remains the existing building source, untouched. Existing scene Assets/Scenes/JourneyPresentation_v48_r93.unity remains authoritative.\nRead the newest ground-survey entry in Docs/PROJECT_GUIDE_AND_HISTORY.md and Docs/BLENDER_UNITY_WORKFLOW.md. Exact layers/sections/provenance and rejection evidence are in Test/KetQua/journey_v48/ground_r93.\n')
 sc.display.shading.show_shadows=False
 bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(p))
 rows.append(dict(file=str(p.relative_to(R)),status=status,native_render_meshes=len([o for o in native.objects if o.type=='MESH']),independent_colliders=len(cc.objects),product_integration=False))
(D/'blender-handoff-state.json').write_text(json.dumps(rows,indent=2));print('HANDOFF_STATE',json.dumps(rows),flush=True)
