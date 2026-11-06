import bpy,json,numpy as np,hashlib
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r107';P=R/'Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));rows=[]
for name in ['TerrainJoin_R90','EastFootwayJoin_R90','TerrainColliderJoin_R90','M95_Service4_NATIVE','BankR91_Grade_017']:
 o=bpy.data.objects[name];v=np.array([o.matrix_world@p.co for p in o.data.vertices]);o.data.calc_loop_triangles();f=np.array([tuple(t.vertices) for t in o.data.loop_triangles]);a=v[f];n=np.cross(a[:,1]-a[:,0],a[:,2]-a[:,0]);c=a.mean(axis=1);norm=np.linalg.norm(n,axis=1);valid=norm>1e-12;ny=np.zeros(len(n));ny[valid]=n[valid,2]/norm[valid]
 scope=(-c[:,0]>=560)&(-c[:,0]<=720)&(-c[:,1]>=320)&(-c[:,1]<=460)&(c[:,2]>-.05)
 rows.append(dict(name=name,vertices=len(v),triangles=len(f),finite=bool(np.isfinite(v).all()),zero_area=int((norm<1e-12).sum()),remaining_down_top_faces_in_scope=int((scope&(ny<-.2)).sum()),scale=list(o.scale),uv_layers=len(o.data.uv_layers)))
missing=[im.filepath for im in bpy.data.images if im.source=='FILE' and not im.packed_file and im.filepath and not Path(bpy.path.abspath(im.filepath)).exists()]
assert all(r['finite'] and r['remaining_down_top_faces_in_scope']==0 for r in rows)
(D/'source-final-audit.json').write_text(json.dumps(dict(source=str(P),blender=bpy.app.version_string,rows=rows,missing_images=missing,libraries=[l.filepath for l in bpy.data.libraries],scope='Orientation only, not a complete terrain topology certificate'),indent=2))
print([(r['name'],r['remaining_down_top_faces_in_scope'],r['zero_area']) for r in rows])
