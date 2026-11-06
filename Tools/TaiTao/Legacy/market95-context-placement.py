import bpy,json,numpy as np,sys
from pathlib import Path
from mathutils import Matrix
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));root=bpy.data.collections['MARKET_GROUND_NATIVE_R95'];old=bpy.data.collections.get('PLACEMENT_EDITABLE_PENDING_CONTACT')
if old:
 for o in list(old.all_objects):bpy.data.objects.remove(o,do_unlink=True)
 bpy.data.collections.remove(old)
col=bpy.data.collections.new('PLACEMENT_EDITABLE_PENDING_CONTACT');root.children.link(col)
code=(R/'Docs/Reviews/journey_v48/r74/mesh_io.py').read_text();exec(code.replace('O=Path(__file__).parent','O=D').replace('r73-source.meshbin','props-baseline.meshbin'));asset_rows=read_source();exec(code.replace('O=Path(__file__).parent','O=D').replace('r73-source.meshbin','props-extra-baseline.meshbin'));asset_rows+=read_source()
inventory={r['path']:r for r in json.loads((D/'inventory.json').read_text())['rows']};ground={r['path'] for r in json.loads((D/'grading.json').read_text())['objects']};candidates={r['path']:r for r in json.loads((D/'placement-candidates.json').read_text())['objects']};origin=np.array([-750.,-300.,0.]);out=[]
for i,r in enumerate(asset_rows):
 p=r['path'];ir=inventory[p]
 if p in ground or not ir['active'] or not (ir['render'] or ir['collider']) or '/Langfarm_R93/' in p:continue
 v=world(r);before=v.copy();candidate=candidates.get(p)
 if candidate:v=np.load(D/(candidate['key']+'-placement.npz'))['v']
 m=bpy.data.meshes.new('C95_'+str(i).zfill(4));t=np.vstack(r['sub'])[:,::-1].copy();slots=np.repeat(np.arange(len(r['sub'])),[len(s) for s in r['sub']]);q=v[t];valid=np.linalg.norm(np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]),axis=1)>1e-10;ix=np.flatnonzero(valid);_,first=np.unique(np.sort(t[ix],axis=1),axis=0,return_index=True);ix=ix[np.sort(first)];t=t[ix];slots=slots[ix];m.from_pydata((np.c_[-v[:,0],-v[:,2],v[:,1]]-origin).tolist(),[],t.tolist());m.update();o=bpy.data.objects.new('C95_'+str(i).zfill(4)+'_'+p.split('/')[-1],m);col.objects.link(o);o.matrix_world=Matrix.Translation(origin.tolist());o['unity_path']=p;o['current_asset_source']=r['asset'];o['market95_placement_study']='Editable authored seating; original XZ and shape; candidate until visual gate';o['candidate_key']=candidate['key'] if candidate else ''
 o.hide_render=not ir['render'];o['market95_collider_enabled']=bool(ir['collider']);o['market95_render_enabled']=bool(ir['render'])
 if candidate:
  o['market95_export_key']=candidate['key']
 u=m.uv_layers.new(name='UVMap')
 if len(r['uv'])==len(v):
  for l in m.loops:u.data[l.index].uv=r['uv'][l.vertex_index]
 for j,sub in enumerate(r['sub']):
  mat=bpy.data.materials.get('C95_Slot_'+str(j)) or bpy.data.materials.new('C95_Slot_'+str(j));m.materials.append(mat)
 for f,s in zip(m.polygons,slots):f.material_index=int(s)
 if candidate:
  u=m.uv_layers.new(name='SourceIndex')
  for l in m.loops:u.data[l.index].uv=(l.vertex_index,0)
  np.savez_compressed(D/(candidate['key']+'-source.npz'),v=v,before=before,tri=t,uv=r['uv'][t] if len(r['uv'])==len(v) else np.zeros((len(t),3,2)),slots=slots)
 o.color=(.55,.48,.40,1)
 out.append(dict(object=o.name,path=p,candidate=bool(candidate),source=r['asset'],vertices=len(v)))
(D/'context-placement-editable.json').write_text(json.dumps(out,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));print('SAVED_EDITABLE_PLACEMENT_STUDY',len(out),flush=True)
