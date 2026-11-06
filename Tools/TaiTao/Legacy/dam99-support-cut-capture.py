import bpy,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'));rows=[]
for name in ['R99_SUPPORT_COL032','C95_0638_PlanterWalkConnection_3','R99_SUPPORT_COL006']:
 o=bpy.data.objects[name];me=o.data;me.calc_loop_triangles();v=np.array([o.matrix_world@v.co for v in me.vertices]);uvs=list(me.uv_layers);cols=list(me.color_attributes);polys=[]
 for tri in me.loop_triangles:
  p=me.polygons[tri.polygon_index]
  arr=[]
  for li in tri.loops:
   ix=me.loops[li].vertex_index;q=v[ix];a=[float(-q[0]),float(q[2]),float(-q[1])]
   for uv in uvs:a.extend(uv.data[li].uv)
   for co in cols:a.extend(co.data[li if co.domain=='CORNER' else ix].color)
   arr.append(a)
  polys.append(dict(a=arr,slot=p.material_index,smooth=p.use_smooth))
 rows.append(dict(name=name,uvs=[uv.name for uv in uvs],colors=[dict(name=c.name,data_type=c.data_type,domain=c.domain) for c in cols],polygons=polys,vertices_unity=np.c_[-v[:,0],v[:,2],-v[:,1]].tolist(),triangles=[list(t.vertices) for t in me.loop_triangles]))
(D/'support-throat-before.json').write_text(json.dumps(rows));print('RESULT source geometry/attributes captured',[(r['name'],len(r['polygons']),len(r['uvs']),len(r['colors'])) for r in rows],flush=True)

