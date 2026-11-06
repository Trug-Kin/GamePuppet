import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));linked=[dict(name=i.name,type=type(i).__name__,library=i.library.filepath) for i in bpy.data.user_map() if i.library];assert not linked,linked
removed=[]
for lib in list(bpy.data.libraries):removed.append(dict(path=lib.filepath,users=lib.users));bpy.data.libraries.remove(lib)
bpy.context.scene['dam99_status']='SCOPED EDITABLE SECTION INTEGRATED IN UNITY R99; estimated design, no player build or release.'
bpy.context.scene['dam99_visual_gate']='Two fixed approximate views and endpoint renders reviewed. Rail/post, fin, throat, winding, import ngons and flower support issues corrected. Absolute datums, bay/riser counts and calibrated architectural camera fit remain UNVERIFIED. Controller route covers both end stairs and R97 low return; not a whole-map or all-water-edge certificate.'
collection=bpy.data.collections.get('DAM_R99_SECTION_CANDIDATE__NOT_APPROVED')
if collection:collection.name='DAM_R99_EDITABLE_SECTION_AND_END_TIES'
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True)
bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P));missing=[im.filepath for im in bpy.data.images if not im.packed_file and im.source not in ['GENERATED','VIEWER'] and not Path(bpy.path.abspath(im.filepath)).exists()];assert not bpy.data.libraries and not missing
(D/'candidate-dependencies-final.json').write_text(json.dumps(dict(source=str(P),version=bpy.app.version_string,libraries=0,missing_images=missing,removed_unused_library_handles=removed,linked_ids_before=linked,unit_scale=bpy.context.scene.unit_settings.scale_length,status=bpy.context.scene['dam99_status'],visual_gate=bpy.context.scene['dam99_visual_gate'],reopened_after_final_save=True),indent=2));print('RESULT source saved/closed/reopened; scoped Unity derivative exists, no release',flush=True)
