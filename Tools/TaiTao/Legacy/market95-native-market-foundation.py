import bpy,json,numpy as np,sys
from pathlib import Path
from mathutils import Matrix
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend';bpy.ops.wm.open_mainfile(filepath=str(P))
class cKDTree:
 def __init__(self,v):self.v=v
 def query(self,p):
  d=np.linalg.norm(p[:,None,:]-self.v[None,:,:],axis=2);ix=d.argmin(1);return d[np.arange(len(p)),ix],ix
records=json.loads((D/'placement-geometry-bindings.json').read_text());pick=[r for r in records if '/MarketEntrance_R72/' in r['path']]
for oldcol in list(bpy.data.collections):
 if oldcol.name.startswith('MARKET_NATIVE_FOUNDATION_R95'):
  for oldobj in list(oldcol.all_objects):bpy.data.objects.remove(oldobj,do_unlink=True)
  bpy.data.collections.remove(oldcol)
with bpy.data.libraries.load(str(R/'Docs/Reviews/journey_v48/r72/market-entrance-r72-source.blend'),link=False) as (a,b):b.collections=['R72_EDITABLE_Four_Closed_Bays']
col=b.collections[0];col.name='MARKET_NATIVE_FOUNDATION_R95';bpy.data.collections['MARKET_GROUND_NATIVE_R95'].children.link(col);objects={o.name:o for o in col.all_objects};report=[]
for row in pick:
 name=row['path'].split('/')[-1];native=next(o for k,o in objects.items() if k==name or k.removesuffix('.001')==name);origin=np.array([-792.,-152.,9.]);native.matrix_world=Matrix.Translation(origin.tolist());v=np.array([p.co for p in native.data.vertices])+origin;world=np.c_[-v[:,0],v[:,2],-v[:,1]];a=np.load(D/(row['key']+'-source.npz'));dist,ix=cKDTree(a['before']).query(world);assert dist.max()<.001,(name,dist.max());delta=a['v'][ix]-a['before'][ix];new=world+delta;local=np.c_[-new[:,0],-new[:,2],new[:,1]]-origin;native.data.vertices.foreach_set('co',local.astype('f4').ravel());native.data.update();native.data.calc_loop_triangles();t=np.array([p.vertices for p in native.data.loop_triangles]);uv=native.data.uv_layers[0];uu=np.array([[uv.data[l].uv[:] for l in p.loops] for p in native.data.loop_triangles]);slots=np.array([native.data.polygons[p.polygon_index].material_index for p in native.data.loop_triangles]);u=native.data.uv_layers.get('SourceIndex') or native.data.uv_layers.new(name='SourceIndex')
 for l in native.data.loops:u.data[l.index].uv=(l.vertex_index,0)
 native['unity_path']=row['path'];native['market95_export_key']=row['key'];native['native_source']='Docs/Reviews/journey_v48/r72/market-entrance-r72-source.blend';native['change']='Existing lowest foundation/contact only; footprint and upper facade unchanged';native.hide_render=False
 for o in list(bpy.data.collections['PLACEMENT_EDITABLE_PENDING_CONTACT'].all_objects):
  if o.get('unity_path')==row['path']:o.hide_render=True;o['market95_export_key']=''
 np.savez_compressed(D/(row['key']+'-source.npz'),v=new,before=world,tri=t,uv=uu,slots=slots);row['object']=native.name;row['native_source']=native['native_source'];report.append(dict(path=row['path'],native_object=native.name,source_error=float(dist.max()),vertices=len(world)))
# The remaining native context copies do not create overlays or exports.
chosen={r['object'] for r in pick}
for o in list(col.all_objects):
 if o.name not in chosen:o.hide_render=True
(D/'placement-geometry-bindings.json').write_text(json.dumps(records,indent=2));(D/'market-native-foundation.json').write_text(json.dumps(report,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));print('NATIVE_MARKET_CONTACT',len(report),flush=True)
