"""One R99 cross-section candidate, isolated from R98 and Unity until visual gates.
Relative model: crest 3.0, toe .08, transverse run 4.5, all game units.
Local shoreline relocation is explicit candidate geometry, NEVER a surveyed shoreline.
"""
import bpy,bmesh,numpy as np,json,math,ast
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99';S=R/'Source/Blender/JourneyV48/R99';S.mkdir(parents=True,exist_ok=True)
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'))
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);tree=ast.parse((R/'Test/Tam/dam98-author.py').read_text(encoding='utf-8-sig'));exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in ['shore','pos','mesh']],type_ignores=[]),'helpers','exec'))
groups={n:bpy.data.collections['R98_'+n] for n in ['CONCRETE_BAYS','CREST_SUPPORT','TOE_GRILLES','END_STAIRS','LOWER_LANDINGS','LOCAL_RAILS','SCOPED_LEGACY_CUTS']};mats={n:bpy.data.materials['R98_'+n] for n in ['Concrete','Toe','Rail']}
H=3.0;W=4.5;delta=H-1.140076

def smooth(x):x=np.clip(x,0,1);return x*x*(3-2*x)
def weight(s):return float(smooth((s+18)/15)*smooth((L+18-s)/15))
def warp_xyz(v):
 x,h,z=v;s,d=(np.array([x,z])-A)@np.c_[T,N];w=weight(s)
 if w<1e-9:return np.array(v)
 en=shore(s);u=max(0,min(1,d/en)) if d<=en else max(0,1-(d-en)/20)
 dn=d+(W-en)*w*u
 # Uplift only the bridge and immediate end ties. Low surfaces/water remain at datum.
 across=float(smooth((d+20)/6)) if d<0 else max(0,1-d/en)
 h+=delta*w*across*float(np.clip((h-.22)/(.920076),0,1))
 xx,zz=A+T*s+N*dn;return np.array([xx,h,zz])
# Vectorized continuous field, avoid per-vertex polygon intersection.
sg=np.linspace(-18,L+18,1001);eg=np.array([shore(float(v)) for v in sg])
def warp_points(world):
 sd=(world[:,[0,2]]-A)@np.c_[T,N];ss,dd=sd.T
 w=smooth((ss+18)/15)*smooth((L+18-ss)/15);en=np.interp(ss,sg,eg)
 u=np.where(dd<=en,np.clip(dd/en,0,1),np.clip(1-(dd-en)/20,0,1))
 dn=dd+(W-en)*w*u
 across=np.where(dd<0,smooth((dd+20)/6),np.clip(1-dd/en,0,1))
 hh=world[:,1]+delta*w*across*np.clip((world[:,1]-.22)/.920076,0,1)
 xz=A+ss[:,None]*T+dn[:,None]*N
 return np.c_[xz[:,0],hh,xz[:,1]]
# Transform the actual existing editable context. Geometry, not scene transform patches.
changes=[]
for o in list(bpy.context.scene.objects):
 if o.type!='MESH' or o.name.startswith('R98_') and not o.get('dam98_cut'):continue
 mm=np.array(o.matrix_world,dtype=float);v=np.array([p.co for p in o.data.vertices],dtype=float)@mm[:3,:3].T+mm[:3,3];world=np.c_[-v[:,0],v[:,2],-v[:,1]]
 if not len(v):continue
 sd=(world[:,[0,2]]-A)@np.c_[T,N]
 if sd[:,0].max()<-18 or sd[:,0].min()>L+18 or sd[:,1].max()<-20 or sd[:,1].min()>45:continue
 out=warp_points(world);moved=np.linalg.norm(out-world,axis=1)>1e-6
 if not moved.any():continue
 me=o.data.copy();o.data=me
 inv=np.linalg.inv(mm)
 local=np.c_[-out[:,0],-out[:,2],out[:,1]]@inv[:3,:3].T+inv[:3,3]
 for p,a in zip(me.vertices,local):p.co=a
 me.update();o['dam99_export']=True
 o['dam99_source_path']=o.get('unity_path','') or ('R97_Bridgehead_GateSide_Bank/'+o.name if o.name.startswith('R97_') else '')
 changes.append(dict(object=o.name,path=o['dam99_source_path'],changed_vertices=int(moved.sum()),max_displacement=float(np.linalg.norm(out-world,axis=1).max())))
# Rebuild cross section before ribs/detail. Existing 13-bay allocation is unverified.
for o in list(bpy.context.scene.objects):
 if o.name.startswith('R98_') and o.get('dam98_export') and not o.get('dam98_cut'):bpy.data.objects.remove(o,do_unlink=True)
stations=[]
for i in range(14):
 s=L*i/13;stations.extend([(s,.10),(min(s+.13,L),0)] if i==0 else [(max(s-.13,0),0),(s,.10)] if i==13 else [(s-.13,0),(s,.10),(s+.13,0)])
vs=[];fs=[]
for s,ridge in stations:
 for d,h in [(0,H),(.25,H),(W-.5,.17+ridge),(W,.08),(W,-.35),(0,H-.42)]:vs.append(pos(s,d,h))
K=6
for i in range(len(stations)-1):
 for j in range(K):a=i*K+j;b=i*K+(j+1)%K;fs.append((a,b,b+K,a+K))
fs.extend([tuple(range(K-1,-1,-1)),tuple((len(stations)-1)*K+j for j in range(K))]);apron=mesh('R99_CONCRETE_DAM_13_ESTIMATED_BAYS',vs,fs,'CONCRETE_BAYS','Concrete');apron['dam99_export']=True
# Two closed stair flights: 16 risers, minimum .22 tread, .65 bottom landing.
for tag,a,b in [('FAR',-3.,0.),('GATE',L,L+3.)]:
 n=14;start=.45;end=W;run=(end-.65-start)/n;sections=[];vs=[]
 for s in [a,b]:
  q=[(0,-.35),(end,-.35),(end,.22),(end-.65,.22)]
  for i in range(n-1,-1,-1):q.extend([(start+(i+1)*run,H-(i+1)*(H-.22)/n),(start+(i+1)*run,H-i*(H-.22)/n)])
  q.extend([(start,H),(0,H)]);q=[p for i,p in enumerate(q) if not i or abs(p[0]-q[i-1][0])+abs(p[1]-q[i-1][1])>1e-7];sections.append(q);vs.extend(pos(s,d,h) for d,h in q)
 k=len(sections[0]);fs=[(j,(j+1)%k,(j+1)%k+k,j+k) for j in range(k)]
 for side,s in enumerate([a,b]):
  cen=len(vs);vs.append(pos(s,W*.5,-.20))
  for j in range(k):fs.append((cen,side*k+j,side*k+(j+1)%k))
 o=mesh('R99_'+tag+'_STAIR_AND_LOW_LANDING',vs,fs,'END_STAIRS','Concrete');o['dam99_export']=True
# Rebuild metal tubes at actual stepped bases, do not scale tube thickness or rail height.
tree=ast.parse((R/'Test/Tam/bank97-author.py').read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name=='beam'],type_ignores=[]),'beam','exec'))
def metal(name,vv,ff,group,mat):
 o=mesh(name,vv,ff,group,mat);o['dam99_export']=True;return o
for tag,s in [('FAR_OUT',-3.),('FAR_IN',0.),('GATE_IN',L),('GATE_OUT',L+3.)]:
 vv=[];ff=[];limit=W-.75 if tag.endswith('OUT') else W-.15
 def hh(d):return H-(H-.22)/14*max(0,min(14,math.ceil((d-.45)/((W-1.10)/14)-1e-7)))
 def top(d):return H+1.03-(H-.22)*max(0,min(1,(d-.45)/(W-1.10)))
 beam(vv,ff,pos(s,.2,top(.2)),pos(s,limit,top(limit)),.027)
 for d in np.linspace(.2,limit,12):beam(vv,ff,pos(s,d,hh(d)-.012),pos(s,d,top(d)+.004),.023)
 metal('R99_'+tag+'_STAIR_RAIL',vv,ff,'LOCAL_RAILS','Rail')
for i in range(13):
 s0=(i+.2)*L/13;s1=(i+.8)*L/13;vv=[];ff=[]
 def toe(s,d):return pos(s,d,.17+(.08-.17)*(d-(W-.5))/.5+.018)
 for s in np.linspace(s0,s1,15):beam(vv,ff,toe(s,W-.47),toe(s,W-.03),.015)
 for d in [W-.47,W-.03]:beam(vv,ff,toe(s0,d),toe(s1,d),.025)
 metal('R99_TOE_GRILLE_'+str(i+1).zfill(2),vv,ff,'TOE_GRILLES','Toe')
# Candidate is explicitly NOT cleared for Unity until full continuity/support gate.
root=bpy.data.collections['DAM_R98_EDITABLE_COMPLETE_CLUSTER'];root.name='DAM_R99_SECTION_CANDIDATE_NOT_INTEGRATED'
sc=bpy.context.scene;sc['dam99_status']='SECTION CANDIDATE ONLY. Local shoreline plan warp and bridge uplift need full continuity/support review. Do not export/integrate as a finished asset.';sc['dam99_units']='1 authoring unit = 1 m convention, absolute dimensions unmeasured';sc['dam99_cross_section']=json.dumps(dict(crest=H,toe=.08,low=.22,water=0,run=W,delta_bridge=delta,water_level_change=0,stair_risers=14,stair_rise=(H-.22)/14,stair_tread=(W-1.1)/14))
sc.render.resolution_x=1280;sc.render.resolution_y=720;sc.render.resolution_percentage=100
sc.render.engine='BLENDER_WORKBENCH';sc.display.shading.color_type='MATERIAL';sc.display.shading.show_cavity=True;sc.display.shading.show_shadows=False;sc.display.shading.show_backface_culling=True
# Camera fitted from architectural crest/rail and toe landmarks, positions in local (s,h,d).
fit=json.loads((D/'photo-fit-initial.json').read_text());params=fit['solution'][2:]
for i,ph in enumerate(fit['photos']):
 ex,ez,ey,yaw,pitch,roll,f=params[i*7:i*7+7];eye=pos(ex,ez,ey);fw=np.array([np.cos(pitch)*np.cos(yaw),np.sin(pitch),np.cos(pitch)*np.sin(yaw)]);fwUnity=np.r_[T*fw[0]+N*fw[2],0] # overridden explicitly below
 xz=T*fw[0]+N*fw[2];forward=Vector((-xz[0],-xz[1],fw[1]));cam=sc.camera;cam.location=(-eye[0],-eye[2],eye[1]);cam.rotation_euler=forward.to_track_quat('-Z','Y').to_euler();cam.rotation_euler.rotate_axis('Z',-roll);cam.data.type='PERSP';cam.data.sensor_fit='HORIZONTAL';cam.data.lens=f*36/1920;cam.data.sensor_width=36;sc.render.filepath=str(M/('candidate-'+ph['id']+'.png'));bpy.ops.render.render(write_still=True)
# A true geometric side section, not a camera-fidelity judgement.
cam=sc.camera;eye=pos(L/2-1,18,5);at=pos(L/2,2,1.3);cam.location=(-eye[0],-eye[2],eye[1]);cam.rotation_euler=(Vector((-at[0],-at[2],at[1]))-cam.location).to_track_quat('-Z','Y').to_euler();cam.data.lens=32;sc.render.filepath=str(M/'candidate-context.png');bpy.ops.render.render(write_still=True)
for o in bpy.context.selected_objects:o.select_set(False)
apron.select_set(True);bpy.context.view_layer.objects.active=apron
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(S/'OngDao_Dam_R99_SectionCandidate.blend'),compress=True)
(D/'candidate-author.json').write_text(json.dumps(dict(source=bpy.data.filepath,changed=changes,section=json.loads(sc['dam99_cross_section']),status='NOT_INTEGRATED',preserved='R98 untouched; no Unity asset/scene modified'),indent=2));print('RESULT one isolated candidate saved',flush=True)
