import bpy,collections,json,struct
from pathlib import Path
R=Path('D:/Game/GameHorror/Game');O=R/'Docs/Reviews/journey_v48/r91'
bpy.ops.wm.open_mainfile(filepath=str(R/'Docs/Reviews/journey_v48/r90_review/market-branches-r90-source.blend'))
for name in ['Grade_017','Grade_000','Grade_057','Grade_015','Grade_007']:
 m=bpy.data.objects[name].data
 keys=[tuple(sorted(p.vertices)) for p in m.polygons]
 print('BASE',name,len(m.vertices),len(m.polygons),'duplicate_indices',len(keys)-len(set(keys)),flush=True)
raw=(O/'context125-before.bin').read_bytes();n=struct.unpack_from('<i',raw)[0];pos=4+n*20;k=struct.unpack_from('<i',raw,pos)[0];pos+=4;faces=[]
for _ in range(k):
 size=struct.unpack_from('<i',raw,pos)[0];pos+=4;ids=struct.unpack_from('<'+'i'*size,raw,pos);pos+=size*4;faces.extend(tuple(sorted(ids[i:i+3])) for i in range(0,size,3))
print('CONTEXT',n,len(faces),'duplicates',len(faces)-len(set(faces)),flush=True)
