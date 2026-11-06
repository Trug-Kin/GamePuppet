import bpy
for file in ['Casual_Male.blend','BaseCharacter.blend']:
 bpy.ops.wm.open_mainfile(filepath='D:/ConRoi/Source/Downloads/Quaternius_R104/'+file,load_ui=False,use_scripts=False)
 rig=next(o for o in bpy.context.scene.objects if o.type=='ARMATURE');print(file,[(b.name,list(b.head_local),list(b.tail_local)) for b in rig.data.bones])
