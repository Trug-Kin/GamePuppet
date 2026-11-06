import bpy,numpy as np,json,sys
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/bank_r97'
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R97/Bridgehead_Bank_R97_Editable.blend'))
code=(R/'Test/Tam/bank97-author.py').read_text();exec(code[code.index('def half('):code.index('cut=[]')])
co=np.array(json.loads((D/'author-result.json').read_text())['coast']);shore=lambda z:float(np.interp(z,co[:,0],co[:,1]));breaks=sorted(set([418.,483.]+[float(z) for z,x in co if 418<z<483]));quads=[np.array([[shore(a)-8,a],[shore(b)-8,b],[shore(b)-.24,b],[shore(a)-.24,a]]) for a,b in zip(breaks,breaks[1:])]
o=next(o for o in bpy.data.objects if o.get('market95_export_key')=='M95_022');k='M95_022';me=o.data;mat=np.array(o.matrix_world,dtype=float);nv=np.array([v.co for v in me.vertices])@mat[:3,:3].T+mat[:3,3];v=np.c_[-nv[:,0],nv[:,2],-nv[:,1]];uvs=list(me.uv_layers);polys=[];changed=0;removed_area=0.
for p in me.polygons:
 arr=[np.r_[v[me.loops[li].vertex_index],*[list(uv.data[li].uv) for uv in uvs]] for li in p.loop_indices];parts=[arr]
 if v[list(p.vertices),2].max()<418 or v[list(p.vertices),2].min()>483:polys.append((p,None));continue
 for q in quads:parts=[b for a in parts for b in difference(a,q)]
 def area(a):return sum(np.linalg.norm(np.cross(a[i][:3]-a[0][:3],a[i+1][:3]-a[0][:3]))*.5 for i in range(1,len(a)-1))
 removed=area(arr)-sum(area(a) for a in parts)
 if removed<1e-8:polys.append((p,None));continue
 changed+=1;removed_area+=removed
 for a in parts:polys.append((p,a))
verts=[];faces=[];slots=[];uvout=[[] for uv in uvs];dedup={}
exec(code[code.index(' for p,arr in polys:')+1:code.index(" if o.name not in clipped.objects:")].replace('\n ', '\n'))
o['wateredge_cleanup']='Remove inherited terrain triangles projecting waterward of replacement quay, Z418..483, d=-8..-.24; no water displacement.'
report=dict(object=o.name,key=k,changed_faces=changed,removed_area_m2=removed_area,water_unchanged=True,strip='Z418..483, shore offset -8..-.24',reason='Observed residual old terrain fins in controller video, not new quay geometry')
(D/'wateredge-cleanup.json').write_text(json.dumps(report,indent=2));bpy.ops.wm.save_as_mainfile(filepath=bpy.data.filepath);print('RESULT',report,flush=True)
