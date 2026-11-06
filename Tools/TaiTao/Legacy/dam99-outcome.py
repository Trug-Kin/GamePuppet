import bpy,json,numpy as np,hashlib
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
# Measure actual delta against protected original; current file alone is never proof of a checkpoint.
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'));before={}
for o in bpy.context.scene.objects:
 if o.type=='MESH':before[o.name]=np.array([o.matrix_world@v.co for v in o.data.vertices],dtype=float)
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene;delta=[]
for o in sc.objects:
 if o.type!='MESH' or o.name not in before:continue
 a=before[o.name];b=np.array([o.matrix_world@v.co for v in o.data.vertices],dtype=float)
 if a.shape!=b.shape:
  delta.append(dict(object=o.name,before_vertices=len(a),after_vertices=len(b),topology_changed=True));continue
 dd=np.linalg.norm(b-a,axis=1);mask=dd>1e-4
 if mask.any():
  changed=b[mask];delta.append(dict(object=o.name,path=o.get('dam99_source_path',''),changed_vertices=int(mask.sum()),max_delta=float(dd.max()),max_vertical_delta=float(np.max(np.abs((b-a)[:,2]))),old_height_5p04_vertices_changed=int(np.sum(mask&(np.abs(a[:,2]-5.04)<.001))) if o.name=='BankR91_Grade_000' else None,bounds_changed_unity=[np.c_[-changed[:,0],changed[:,2],-changed[:,1]].min(0).tolist(),np.c_[-changed[:,0],changed[:,2],-changed[:,1]].max(0).tolist()]))
(D/'candidate-actual-delta.json').write_text(json.dumps(delta,indent=2))
sc['dam99_visual_gate']='FAIL / NOT APPROVED. Reopened low views show FAR stair-to-old-terrain void and GATE legacy planter/support mass intruding above stair. Two outer handrails still have visible post overshoot. No export or Unity integration.'
sc['dam99_status']='INCOMPLETE SECTION CANDIDATE ONLY; product remains R98. Do not interpret 20 mesh gates as visual acceptance.'
# Give an opening view that exposes the unresolved gate-side geometry.
sc.camera=bpy.data.objects['R99_APPROX_PHOTO_CAMERA_181402']
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True)
(D/'candidate-review-outcome.json').write_text(json.dumps(dict(status='INCOMPLETE_NOT_INTEGRATED',source=str(P),visual_gate='FAIL',issues=[dict(id='FAR_JOIN',evidence='candidate-final-far-tie.png',issue='Unclosed old terrain / abrupt unsupported endpoint visible beside stair'),dict(id='GATE_SUPPORT',evidence='candidate-final-gate-tie.png',issue='Legacy support/planter mass intrudes above upper stair'),dict(id='OUTER_RAIL',evidence='candidate-final-far-tie.png',issue='Outer post tips overshoot straight top chord; local mesh gate alone did not catch visual defect')],remaining=['Calibrate/validate comparison camera; current fit uses manual approximate points and a focal bound','Resolve endpoint ground/supports in this same source, without restoring market slope or changing controller','Validate proposed local shoreline plan before accepting it; no global lake level change','FBX roundtrip, Unity integration, Bao contacts, controller, repeated copied-save loads: NOT_RUN'],next_step='Finish FAR_JOIN and GATE_SUPPORT in this same candidate, lock the local shoreline/crest section, then rerender the existing fixed cameras before any FBX export.'),indent=2))
print('RESULT source saved NOT_APPROVED; actual changed original objects',len(delta),'market5p04changed',[d.get('old_height_5p04_vertices_changed') for d in delta if d['object']=='BankR91_Grade_000'],flush=True)
