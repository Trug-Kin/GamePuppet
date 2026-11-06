import bpy,sys,json
from pathlib import Path
R=Path('D:/ConRoi');s=R/'Source/Blender/JourneyV48/R103/Workshop_Investigation_R103_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(s))
report={}
for name in ['ShoeSample','ShoeControl']:
 o=bpy.data.objects[name];mi=next(i for i,m in enumerate(o.data.materials) if m.name=='R103 worn rubber')
 ids={v for p in o.data.polygons if p.material_index==mi for v in p.vertices}
 lo=min(o.data.vertices[i].co.z for i in ids);hi=max(o.data.vertices[i].co.z for i in ids)
 for i in ids:o.data.vertices[i].co.z=-.001+(.0012*(o.data.vertices[i].co.z-lo)/(hi-lo))
 o.data.update();report[name]={'old_min':lo,'old_max':hi,'new_min':-.001,'new_max':.0002,'reason':'Tread bottom protrudes below sole; small penetration at top attaches tread rather than hiding it inside base.'}
bpy.ops.wm.save_as_mainfile(filepath=str(s));bpy.ops.wm.open_mainfile(filepath=str(s))
sys.path.append(str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
obs=list(bpy.data.collections['EVIDENCE'].objects);fbx=str(R/'Assets/Art/InvestigationV36/R103/EVIDENCE.fbx')
report['export']=bx_export.export(fbx,'unity',objects=obs);report['roundtrip']=bx_export.round_trip(fbx,obs,preset='unity')
(R/'Test/KetQua/investigation_r103/sole-final.json').write_text(json.dumps(report,indent=2,default=str))
