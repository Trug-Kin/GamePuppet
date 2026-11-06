import bpy,bmesh,numpy as np,json,math,ast
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';M=R/'Test/BangChung/journey_v48/dam_r98';P=R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);groups={n:bpy.data.collections['R98_'+n] for n in ['CONCRETE_BAYS','CREST_SUPPORT','TOE_GRILLES','END_STAIRS','LOWER_LANDINGS','LOCAL_RAILS','SCOPED_LEGACY_CUTS']};mats={n:bpy.data.materials['R98_'+n] for n in ['Concrete','Toe','Rail']}
for file,names in [('dam98-author.py',['shore','pos','mesh']),('bank97-author.py',['beam'])]:
 tree=ast.parse((R/'Test/Tam'/file).read_text(encoding='utf-8-sig'));exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in names],type_ignores=[]),file,'exec'))
for o in list(groups['LOCAL_RAILS'].objects):bpy.data.objects.remove(o,do_unlink=True)
for tag,s in [('FAR_OUT',-3.),('FAR_IN',0.),('GATE_IN',L),('GATE_OUT',L+3.)]:
 end=shore(s);vv=[];ff=[]
 def hh(d):return 1.140076-.920076/8*max(0,min(8,math.ceil((d-.7)/((end-2.3)/8)-1e-7)))
 ds=np.linspace(.3,end-.3,24)
 for d,e in zip(ds,ds[1:]):
  def ramp(x):return 1.140076-.920076*max(0,min(1,(x-.7)/(end-2.3)))
  beam(vv,ff,pos(s,d,ramp(d)+1.03),pos(s,e,ramp(e)+1.03),.027)
 for d in np.linspace(.3,end-.3,15):
  h=hh(d);beam(vv,ff,pos(s,d,h-.012),pos(s,d,1.140076-.920076*max(0,min(1,(d-.7)/(end-2.3)))+1.05),.023)
 mesh('R98_'+tag+'_STAIR_SIDE_RAIL',vv,ff,'LOCAL_RAILS','Rail')
# Show actual preserved bridge, rail, lamp and planter meshes as editable context.
shown=[]
for o in list(sc.objects):
 if o.type=='MESH' and o.name.startswith('C95_') and o.get('unity_path','').startswith('R11_Local_Bridge_Supports/'):
  o.hide_render=False;o.hide_set(False);shown.append(o.name)
for name in ['C95_0051_R11_Bridge_Deck_Fascia','C95_0052_R11_Bridge_Deck_Soffit']:
 o=bpy.data.objects.get(name)
 if o:o.hide_render=False;o.hide_set(False);shown.append(name)
# The former endpoint grass blend is now the photo-backed lower-path end tie.
# Only the 480..483 end connector is edited; preserved bank stays unchanged south.
coast=np.array(json.loads((R/'Test/KetQua/journey_v48/bank_r97/author-result.json').read_text())['coast']);changed=[]
for name in ['R97_LOWER_WALK','R97_LOWER_GRASS','R97_QUAY_WATER_EDGE']:
 o=bpy.data.objects[name];count=0
 for v in o.data.vertices:
  x,z,h=-v.co.x,-v.co.y,v.co.z
  if not 480<=z<=483.0001:continue
  d=x-float(np.interp(z,coast[:,0],coast[:,1]))
  if name=='R97_LOWER_GRASS':w=max(0,min(1,(3.0-d)/1.2));target=h*(1-w)+.22*w
  elif name=='R97_QUAY_WATER_EDGE' and h<0:continue
  else:target=.22
  if abs(target-h)>.00001:v.co.z=target;count+=1
 if name=='R97_LOWER_WALK':
  for p in o.data.polygons:
   if -p.center.y>=479.999:p.material_index=0
 o.data.update();changed.append(dict(object=name,vertices=count))
sc['dam98_endtie']='R97 lower walk endpoint Z480..483 now levelY.22/paved to join gate stair landing; lower grass within3m offset blends to it; remaining R97 grass/market unchanged.'
sc.render.resolution_x=1280;sc.render.resolution_y=800
for tag,eye,at in [('wide',[628,8,491],[657,1,515]),('oblique',[663,2.2,489],[649,1,520]),('gate-stair',[678,3,480],[682,.6,488]),('far-stair',[629,3,525],[637,.6,530])]:
 sc.camera.location=(-eye[0],-eye[2],eye[1]);sc.camera.rotation_euler=(Vector((-at[0],-at[2],at[1]))-sc.camera.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('blender-'+tag+'-review.png'));bpy.ops.render.render(write_still=True)
sc.display.shading.type='WIREFRAME';sc.camera.location=(-628,-491,11);sc.camera.rotation_euler=(Vector((-657,-515,.5))-sc.camera.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/'blender-wire.png');bpy.ops.render.render(write_still=True);sc.display.shading.type='SOLID'
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True)
(D/'refine-result.json').write_text(json.dumps(dict(shown_preserved_bridge_context=shown,endtie=changed,rails='Both sides of end stairs only, planted post bases at stepped surface; no new toe route'),indent=2));print('RESULT reopened/refined/rendered source saved',flush=True)

