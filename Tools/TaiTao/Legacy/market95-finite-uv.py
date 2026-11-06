import bpy,json,numpy as np
from pathlib import Path
from mathutils import kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(P));log=[]
for o in list(bpy.data.collections['MARKET_GROUND_NATIVE_R95'].all_objects):
 key=o.get('market95_export_key')
 if not key:continue
 m=o.data
 for u in m.uv_layers:
  vals=np.array([p.uv[:] for p in u.data]);bad=~np.isfinite(vals).all(1)
  if not bad.any():continue
  good=np.flatnonzero(~bad);assert len(good);kd=kdtree.KDTree(len(good))
  for i,l in enumerate(good):kd.insert(m.vertices[m.loops[int(l)].vertex_index].co,i)
  kd.balance()
  for l in np.flatnonzero(bad):
   co,ix,dist=kd.find(m.vertices[m.loops[int(l)].vertex_index].co);u.data[int(l)].uv=vals[good[ix]]
  log.append(dict(key=key,layer=u.name,repaired_loops=int(bad.sum()),method='Nearest valid source loop by local position; Unity final UV restored from actual r94 barycentric source'))
 m.calc_loop_triangles();a=np.load(D/(key+'-source.npz'));data={k:a[k] for k in a.files};uv=m.uv_layers[0];data['uv']=np.array([[uv.data[l].uv[:] for l in t.loops] for t in m.loop_triangles]);np.savez_compressed(D/(key+'-source.npz'),**data)
(D/'finite-uv-repair.json').write_text(json.dumps(log,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));print('FINITE_UV_SOURCE',log)
