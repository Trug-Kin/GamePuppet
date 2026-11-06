import bpy,json
from pathlib import Path
R=Path('D:/ConRoi');out=[]
for file in ['Casual_Hoodie.blend','Casual_2.blend']:
 bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Downloads/Quaternius_R104'/file),load_ui=False,use_scripts=False)
 objs=[]
 for o in bpy.context.scene.objects:
  if o.type in ['MESH','ARMATURE']:
   row=dict(name=o.name,type=o.type,loc=list(o.location),rot=list(o.rotation_euler),scale=list(o.scale),dimensions=list(o.dimensions))
   if o.type=='MESH':row.update(vertices=len(o.data.vertices),materials=[m.name if m else '' for m in o.data.materials],groups=[g.name for g in o.vertex_groups],modifiers=[m.type for m in o.modifiers])
   else:row.update(bones=[dict(name=b.name,head=list(b.head_local),tail=list(b.tail_local),parent=b.parent.name if b.parent else None,deform=b.use_deform) for b in o.data.bones])
   objs.append(row)
  out.append(dict(file=file,objects=objs,actions=[a.name for a in bpy.data.actions],images=[dict(name=i.name,path=i.filepath) for i in bpy.data.images]))
(R/'Test/KetQua/investigation_r104/downloaded-model-probe.json').write_text(json.dumps(out,indent=2))
print(json.dumps(out,indent=2)[:15000])
