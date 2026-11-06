import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector,Matrix
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));root=bpy.data.collections['MARKET_GROUND_NATIVE_R95'];old=bpy.data.collections.get('LANGFARM_NATIVE_FOUNDATIONS')
if old:
 for o in list(old.all_objects):bpy.data.objects.remove(o,do_unlink=True)
 bpy.data.collections.remove(old)
with bpy.data.libraries.load(str(R/'Source/Blender/JourneyV48/R93/Langfarm_R93_Editable.blend'),link=False) as (a,b):b.collections=['R93_BUILDING_EDITABLE']
lc=b.collections[0];root.children.link(lc);lc.name='LANGFARM_NATIVE_FOUNDATIONS'
u=np.array([-.22935149,.97334367,0]);n=np.array([-.97334367,-.22935149,0]);origin=np.array([-713.656372,-306.396576,5.62]);g=json.loads((D/'grading.json').read_text());g['objects']=[r for r in g['objects'] if not r['key'].startswith('M95_F')];report=[]
for i,o in enumerate(list(lc.all_objects)):
 if o.type!='MESH':continue
 v=np.array([p.co for p in o.data.vertices],float);world=v[:,0,None]*u+v[:,1,None]*n+np.c_[np.zeros(len(v)),np.zeros(len(v)),v[:,2]]+origin
 o.data=o.data.copy();before=world.copy();o.matrix_world=Matrix.Translation(origin.tolist())
 # Three closed stair solids: preserve every tread, trim/extend only bottom
 # corners to meet the newly designed horizontal apron with a 3cm overlap.
 if o.name.startswith('R93_SOLID_EntryStep_'):
  top=v[:,2].max();bottom=v[:,2]<top-.06;world[bottom,2]=5.01
 elif o.name.startswith('R93_SOLID_') and 'EntranceRecess' not in o.name:
  bottom=v[:,2]<v[:,2].min()+.001
  world[bottom,2]=np.minimum(world[bottom,2],5.01)
 local=world-origin;o.data.vertices.foreach_set('co',local.astype('f4').ravel());o.data.update();o.hide_render=False;o.hide_set(False);o['market95_foundation_only']='No facade restyling; native foundation contact with game-grade apron; XZ unchanged'
 if np.max(abs(world-before))<1e-6:continue
 key='M95_F'+str(i).zfill(3);matches=[r['path'] for r in json.loads((D/'inventory.json').read_text())['rows'] if '/Langfarm_R93/' in r['path'] and r['path'].split('/')[-1].removesuffix('.001')==o.name];assert len(matches)==1,(o.name,matches);path=matches[0];o['unity_path']=path;o['market95_export_key']=key;o['market95_status']='BLENDER_GATE_PENDING';o.data.calc_loop_triangles();t=np.array([p.vertices for p in o.data.loop_triangles]);loops=np.array([p.loops for p in o.data.loop_triangles]);uv=np.array([list(p.uv) for p in o.data.uv_layers[0].data])[loops] if o.data.uv_layers else np.zeros((len(t),3,2))
 uvindex=o.data.uv_layers.get('SourceIndex') or o.data.uv_layers.new(name='SourceIndex')
 for l in o.data.loops:uvindex.data[l.index].uv=(l.vertex_index,0)
 vv=np.c_[-world[:,0],world[:,2],-world[:,1]];bb=np.c_[-before[:,0],before[:,2],-before[:,1]];np.savez_compressed(D/(key+'-source.npz'),v=vv,tri=t,before=bb,uv=uv,slots=np.zeros(len(t),dtype='i4'));np.savez_compressed(D/(key+'-graded.npz'),v=vv,tri=t,before=bb,uv=uv,slots=np.zeros(len(t),dtype='i4'))
 row=dict(key=key,path=path,object=o.name,source='Source/Blender/JourneyV48/R93/Langfarm_R93_Editable.blend',mode='Foundation underside only; tread elevations and facade unchanged',delta_range=[float((vv[:,1]-bb[:,1]).min()),float((vv[:,1]-bb[:,1]).max())]);g['objects'].append(row);report.append(row)
(D/'foundation-adjustments.json').write_text(json.dumps(report,indent=2));(D/'grading.json').write_text(json.dumps(g,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));print('SAVED_NATIVE_FOUNDATION',len(report),flush=True)
