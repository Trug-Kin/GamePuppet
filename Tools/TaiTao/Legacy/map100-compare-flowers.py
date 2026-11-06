import bpy,numpy as np,json,struct
from pathlib import Path
from mathutils import Vector,kdtree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'));out=[]
for line in (D/'actual-flower-sources/index.tsv').read_text().splitlines():
 name,path,mat=line.split('\t');o=next(o for o in bpy.context.scene.objects if o.get('dam99_binding_path')==path);data=(D/'actual-flower-sources'/f'{name}.bin').read_bytes();nv,ns=struct.unpack_from('<ii',data);u=np.frombuffer(data,dtype='<f4',count=nv*5,offset=8).reshape(-1,5)[:,:3];v=np.array([o.matrix_world@p.co for p in o.data.vertices]);v=np.c_[-v[:,0],v[:,2],-v[:,1]];tree=kdtree.KDTree(len(v))
 for i,q in enumerate(v):tree.insert(Vector(q),i)
 tree.balance();err=np.array([tree.find(Vector(q))[2] for q in u]);out.append(dict(source=o.name,actual=name,source_count=len(v),actual_count=nv,max_error=float(err.max()),mean_error=float(err.mean()),vertices_over_1mm=int((err>.001).sum())))
(D/'flower-source-vs-unity.json').write_text(json.dumps(out,indent=2));print('RESULT',json.dumps(out),flush=True)
