import bpy,json,math
bpy.ops.wm.open_mainfile(filepath='D:/ConRoi/Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend')
for o in list(bpy.context.scene.objects):
 if o.name.startswith('R97_') and o.type=='MESH':print('OBJECT',o.name,o.hide_render,o.hide_get(),len(o.data.polygons),tuple(o.data.polygons[0].normal),[tuple(m.diffuse_color) for m in o.data.materials],sum(not math.isfinite(c) for v in o.data.vertices for c in v.co),flush=True)
print('SHADE',bpy.context.scene.display.shading.color_type)
import numpy as np
for o in list(bpy.context.scene.objects):
 if not o.get('bank97_cut'):continue
 m=np.array(o.matrix_world,dtype=float);v=np.array([x.co for x in o.data.vertices],dtype=float)@m[:3,:3].T+m[:3,3];u=np.c_[-v[:,0],v[:,2],-v[:,1]]
 print('CUT_BOUNDS',o.name,u.min(0),u.max(0),'MATRIX',m.tolist(),flush=True)
 for p in o.data.polygons:
  c=o.matrix_world@p.center
  if 680<-c.x<685 and 448<-c.y<452:print('EDGEFACE',o.name,tuple(c),tuple(p.normal),flush=True);break
