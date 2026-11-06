"""Blender read-only source reopen, no save/export/render or model edits."""
import bpy,json
from pathlib import Path
R=Path(__file__).resolve().parents[2];rows=[]
for name in ['Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend','Source/Blender/JourneyV48/R105/Repair_R12_R105_Animation.blend']:
 p=R/name;bpy.ops.wm.open_mainfile(filepath=str(p),load_ui=False)
 rows.append(dict(path=name,opened=True,objects=len(bpy.data.objects),actions=len(bpy.data.actions),missing_external=[i.filepath for i in bpy.data.images if i.source=='FILE' and i.filepath and not i.packed_file and not Path(bpy.path.abspath(i.filepath)).exists()],modified=False))
(R/'Docs/QA/GitPreparation/blender-source-open.json').write_text(json.dumps(dict(version=bpy.app.version_string,rows=rows),ensure_ascii=False,indent=2),encoding='utf-8')
print('SOURCE_OPEN_RESULT',json.dumps(rows))
