import bpy,bmesh,numpy as np,json
from pathlib import Path
R=Path('D:/ConRoi');P=R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend';D=R/'Test/KetQua/journey_v48/bank_r97'
bpy.ops.wm.open_mainfile(filepath=str(P));out=[]
for o in list(bpy.context.scene.objects):
 if not o.get('bank97_export') or o.type!='MESH':continue
 o.data.calc_loop_triangles();v=np.array([v.co for v in o.data.vertices],dtype=float);t=np.array([tuple(p.vertices) for p in o.data.loop_triangles]);q=v[t];area=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)/2
 ids=set(int(o.data.loop_triangles[i].polygon_index) for i in np.flatnonzero(area<1e-9))
 if ids:
  bm=bmesh.new();bm.from_mesh(o.data);bm.faces.ensure_lookup_table();bmesh.ops.delete(bm,geom=[bm.faces[i] for i in ids],context='FACES_ONLY');edges=[e for e in bm.edges if not e.link_faces]
  if edges:bmesh.ops.delete(bm,geom=edges,context='EDGES')
  verts=[v for v in bm.verts if not v.link_edges]
  if verts:bmesh.ops.delete(bm,geom=verts,context='VERTS')
  bm.to_mesh(o.data);bm.free();o.data.update()
 out.append(dict(object=o.name,removed_degenerate_faces=len(ids),threshold_native_m2=1e-9,reason='Clip intersections quantized to Blender float32; no boundary displacement'))
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'topology-cleanup.json').write_text(json.dumps(out,indent=2));print('RESULT',sum(r['removed_degenerate_faces'] for r in out),flush=True)
