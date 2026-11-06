import bpy
from pathlib import Path
p=Path('D:/ConRoi/Source/Blender/JourneyV48/R93/Ground_I2_R93_Editable.blend')
bpy.ops.wm.open_mainfile(filepath=str(p))
bpy.context.scene['status']='BLENDER_ACCEPTED_R94_DERIVED_LOCAL_UNITY_VALIDATED'
for o in bpy.data.collections['GROUND_NATIVE_MATCHED_R93'].all_objects:
 if o.get('i2_export_key'):o['i2_status']='BLENDER_ACCEPTED_R94_DERIVED_LOCAL_UNITY_VALIDATED'
t=bpy.data.texts['I2_README'];t.clear();t.write('I2 native editable ground source. Select GROUND_NATIVE_MATCHED_R93 / I2_ROAD, I2_FOOTWAY, I2_TERRAIN; independent COLLIDERS_NOT_FOR_RENDER hidden. I2_BEFORE_DO_NOT_EXPORT is immutable baseline copy, excluded from export. I2_REVIEW_CAMERAS contains six fixed review cameras. Units: 1 authoring metre =1 Blender unit =1 Unity unit, not surveyed accuracy. Native mapping Unity=(-BlenderX, BlenderZ,-BlenderY). Nine meshes exported and roundtripped, integrated as scene mesh overrides into JourneyPresentation_v48_r94; r93 untouched. Local synthetic-keyboard CharacterController routes, pause/private save-load including old-height migration, return from room and saved scene reopening observed. This is not all-branch controller coverage, all-map flatness or field accuracy acceptance. Existing banks, bridge, island and curbs preserved. Full-width sections, change envelope and route logs in Test/KetQua/journey_v48/i2_r93_complete; directly reviewed renders and two separate local controller clips in Test/BangChung/journey_v48/i2_r93_complete. No user save edits, gameplay/controller code edits, build or runtime fixture component. Source status metadata updated after validation; geometry identical to FBX export. See existing Docs/BLENDER_UNITY_WORKFLOW.md for reproduction and limits.\n')
bpy.context.preferences.filepaths.save_version=0
bpy.ops.wm.save_as_mainfile(filepath=str(p))
print('HANDOFF_SOURCE_METADATA_SAVED',p)
