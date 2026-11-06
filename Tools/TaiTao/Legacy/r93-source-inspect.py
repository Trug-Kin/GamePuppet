import bpy,json
bpy.ops.wm.open_mainfile(filepath='D:/ConRoi/Source/Blender/JourneyV48/R93/langfarm-r93-editable-diagnostic.blend')
for o in bpy.data.objects:
 if any(k in o.name for k in ['Name','Subtitle','Orange','Buffet','Store','UpperPier','RearClosure']):
  print(o.name,o.type,o.hide_render,o.hide_viewport, list(o.location),[list(min(v.co[i] for v in o.data.vertices) for i in range(3)),list(max(v.co[i] for v in o.data.vertices) for i in range(3))] if o.type=='MESH' else '')
