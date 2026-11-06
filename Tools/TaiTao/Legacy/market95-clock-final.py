import bpy,json,numpy as np,sys
from pathlib import Path
from mathutils import Matrix
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/market_ground_r95';P=R/'Source/Blender/JourneyV48/R95/Market_Ground_R95_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P))
col=bpy.data.collections.get('CLOCK_NATIVE_CONTACT_R95')
if col:
 for o in list(col.objects):bpy.data.objects.remove(o,do_unlink=True)
else:col=bpy.data.collections.new('CLOCK_NATIVE_CONTACT_R95');bpy.data.collections['MARKET_GROUND_NATIVE_R95'].children.link(col)
with bpy.data.libraries.load(str(R/'Docs/Reviews/journey_v48/r87/gate-clock-r87-source.blend'),link=False) as (a,b):b.objects=['R87_Plinth_ContactFoot']
o=b.objects[0];col.objects.link(o);o.name='R95_Clock_ContactFoot_NATIVE';o.matrix_world=Matrix.Translation((-746,-422,1.466958));bpy.context.view_layer.update()
# Existing source is local, r94 tower is an unchanged translation of it.
old=np.array([o.matrix_world@p.co for p in o.data.vertices]);uv=o.data.uv_layers.active
f=np.load(D/'continuous-design-field.npz')
def field(p):
 x,z=p.T;ix=np.clip(np.searchsorted(f['x'],x)-1,0,len(f['x'])-2);iz=np.clip(np.searchsorted(f['z'],z)-1,0,len(f['z'])-2);a=(x-f['x'][ix])/2;b=(z-f['z'][iz])/2;y=f['y'];return (1-a)*(1-b)*y[ix,iz]+a*(1-b)*y[ix+1,iz]+(1-a)*b*y[ix,iz+1]+a*b*y[ix+1,iz+1]
bottom=np.array([v.co.z<.5 for v in o.data.vertices]);new=old.copy();new[bottom,2]=field(np.c_[-old[bottom,0],-old[bottom,1]])-.025
assert np.max(new[bottom,2])<2.096958-.1,'Base top must remain above underside'
for i in np.flatnonzero(bottom):o.data.vertices[int(i)].co.z=float(new[i,2]-1.466958)
o.data.update();o.data.calc_loop_triangles();t=np.array([p.vertices for p in o.data.loop_triangles]);uu=np.array([[uv.data[l].uv[:] for l in p.loops] for p in o.data.loop_triangles]);slots=np.array([o.data.polygons[p.polygon_index].material_index for p in o.data.loop_triangles]);key='M95_ClockFoot';path='R20_Whole_Source_Export/GateClock_R87_ESTIMATED/R87_Plinth_ContactFoot'
o['unity_path']=path;o['market95_export_key']=key;o['native_source']='Docs/Reviews/journey_v48/r87/gate-clock-r87-source.blend';o['change']='Underside only; tower, cap, all XZ and upper base unchanged'
u=o.data.uv_layers.new(name='SourceIndex')
for l in o.data.loops:u.data[l.index].uv=(l.vertex_index,0)
world=np.c_[-new[:,0],new[:,2],-new[:,1]];before=np.c_[-old[:,0],old[:,2],-old[:,1]];np.savez_compressed(D/(key+'-source.npz'),v=world,before=before,tri=t,uv=uu,slots=slots)
for c in list(bpy.data.collections.get('PLACEMENT_EDITABLE_PENDING_CONTACT').all_objects):
 if c.get('unity_path')==path:c.hide_render=True;c['market95_export_key']=''
report=dict(key=key,path=path,object=o.name,source=o['native_source'],bottom_vertices=int(bottom.sum()),old_bottom=before[bottom,1].tolist(),new_bottom=world[bottom,1].tolist(),top_unchanged=bool(np.array_equal(old[~bottom],new[~bottom])),tower_translation=[0,0,0])
(D/'clock-contact-final.json').write_text(json.dumps(report,indent=2));bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));print('CLOCK_NATIVE_CONTACT',json.dumps(report),flush=True)
