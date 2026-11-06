import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');out=[]
for rel in ['Source/Blender/JourneyV48/R91/north-bank-r91-source-final.blend','Source/Blender/JourneyV48/R91/north-bank-r91-extra-final.blend','Docs/Reviews/journey_v48/r90_review/terrain-stage-r90-source.blend','Docs/Reviews/journey_v48/r90_review/market-branches-r90-source.blend']:
 bpy.ops.wm.open_mainfile(filepath=str(R/rel));obs=[]
 for o in bpy.context.scene.objects:
  if o.type!='MESH':continue
  path=o.get('unity_path',o.get('source_path',''))
  obs.append(dict(name=o.name,path=path,vertices=len(o.data.vertices),triangles=sum(len(p.vertices)-2 for p in o.data.polygons),keys={k:str(o[k])[:180] for k in o.keys() if k!='_RNA_UI'}))
 out.append(dict(source=rel,objects=obs));print(rel,'objects',len(obs),flush=True)
(R/'Test/KetQua/journey_v48/ground_r93/editable-source-audit.json').write_text(json.dumps(out,indent=2))
print('SOURCE_AUDIT_DONE')
