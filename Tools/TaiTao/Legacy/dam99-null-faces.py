import bpy,bmesh,json,numpy as np
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend';bpy.ops.wm.open_mainfile(filepath=str(P));names={x['object'] for x in json.loads((D/'actual-context-selection.json').read_text()) if x['render'] or x['collider']};report=[]
for o in bpy.context.scene.objects:
 if o.type!='MESH' or not (o.name in names or o.get('dam99_collider_only')):continue
 me=o.data;me.calc_loop_triangles();v=np.array([o.matrix_world@p.co for p in me.vertices]);tri=np.array([tuple(p.vertices) for p in me.loop_triangles]);q=v[tri];a=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)*.5;bad=np.flatnonzero(a<1e-10)
 if not len(bad):continue
 badpolys={me.loop_triangles[int(i)].polygon_index for i in bad}
 bm=bmesh.new();bm.from_mesh(me);bm.faces.ensure_lookup_table();bmesh.ops.triangulate(bm,faces=[bm.faces[i] for i in badpolys if len(bm.faces[i].verts)>3]);null=[f for f in bm.faces if len(f.verts)==3 and np.linalg.norm(np.cross(np.array(o.matrix_world@f.verts[1].co)-np.array(o.matrix_world@f.verts[0].co),np.array(o.matrix_world@f.verts[2].co)-np.array(o.matrix_world@f.verts[0].co)))*.5<1e-10];bmesh.ops.delete(bm,geom=null,context='FACES_ONLY');bm.to_mesh(me);bm.free();me.update();report.append(dict(object=o.name,removed_null_triangles=len(badpolys),largest_removed_area=float(a[bad].max()),reason='Null/tiny triangles below 1e-10 square authoring units; no measurable surface removed'))
(D/'null-triangle-cleanup.json').write_text(json.dumps(report,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);print('RESULT null face cleanup',report,flush=True)
