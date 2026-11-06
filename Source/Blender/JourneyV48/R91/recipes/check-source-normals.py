import bpy
bpy.ops.wm.open_mainfile(filepath='D:/Game/GameHorror/Game/Docs/Reviews/journey_v48/r90_review/market-branches-r90-source.blend')
for n in ['Grade_017','Grade_000','Grade_057']:
 m=bpy.data.objects[n].data;print('SMOOTH',n,sum(p.use_smooth for p in m.polygons),len(m.polygons),flush=True)
