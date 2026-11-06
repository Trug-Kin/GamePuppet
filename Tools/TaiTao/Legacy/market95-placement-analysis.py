exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
from scipy.spatial import cKDTree
from scipy.sparse import coo_matrix
from scipy.sparse.csgraph import connected_components
import re
inventory={r['path']:r for r in json.loads((D/'inventory.json').read_text())['rows']}
surface_catalog={r['mesh']:r for r in json.loads(Path('Assets/Art/Environment/JourneyV48/ExportPolicyR20/surfaces.json').read_text())['meshes']}
g=json.loads((D/'grading.json').read_text());ground={r['path'] for r in g['objects']};meta=json.loads((D/'placement-native-groups.json').read_text());groupnames=meta['groups'];contextnames=json.loads((D/'placement-context-native-groups.json').read_text())['groups'];cache=json.loads((D/'props-geometry-inventory.json').read_text())+json.loads((D/'props-extra-geometry-inventory.json').read_text());parts=[];points={};attribution=[]
def rename(k):
 if k.startswith('LOCAL_'):
  _,x,z=k.split('_');x,z=int(x),int(z)
  if 143<=x<=146 and 82<=z<=83:return 'BUILDING_GATE'
 return k
for row in cache:
 p=row['path'];ir=inventory[p]
 if p in ground or not ir['active'] or not (ir['render'] or ir['collider']):continue
 a=np.load(D/(row['cache']+'.npz'));v=a['v'];name=p.split('/')[-1];name='Surface_106' if name=='R64_Context_Surface106_R63' else name;npz=D/('placement-native-'+name+'.npz')
 if name in surface_catalog and all(n.startswith(('Core_Road_','Local_Road_')) for n in surface_catalog[name]['sources']):continue
 if (p.startswith('R20_Whole_Source_Export/Surface_') or p.startswith('LakeGateMarketV32/Context_') or p.endswith('/R64_Context_Surface106_R63')) and npz.exists():
  native=np.load(npz);dist,ix=cKDTree(native['v']).query(v);ids=native['group'][ix];names=contextnames if name.startswith('Context_') else groupnames;ks=np.array([rename(names[k]) for k in ids]);attribution.append(dict(path=p,max_error=float(dist.max()),p99=float(np.percentile(dist,99))))
  # Source ground/lake/bridge faces are not placement props, regardless of name.
  if name in ['Surface_049','Surface_050','Surface_051']:continue
 elif p.startswith('R20_Whole_Source_Export/') and '/' in p[len('R20_Whole_Source_Export/'):]:
  group=re.sub(r'_(?:RenderResiduals|CollisionResiduals|CoarseCollision|Residuals)$','',p.split('/')[1])
  if group in ['MarketUtilityRoof_R70','MarketEntrance_R71','MarketEntrance_R72']:group='BUILDING_MARKET'
  ks=np.repeat(group,len(v))
 elif p.startswith('R22_Market_Front_Detail/'):ks=np.repeat('BUILDING_MARKET',len(v))
 elif p.startswith('RouteSurfaces/'):
  centre=v.mean(0);ks=np.repeat('BUILDING_GATE' if centre[2]>400 else 'STALL_Hien',len(v))
 elif p.startswith(('R21_','R11_Route_Wayfinding/')):
  # Independent authored planter/tree/sign assemblies remain rigid.
  depth=3 if '/Grounded_Cypress_Boundary/' in p else 2
  ks=np.repeat('/'.join(p.split('/')[:depth]),len(v))
 else:continue
 # One rigid physical part cannot receive multiple independent height writes.
 # Weld UV seams by world position, then choose the modal source assembly for
 # each connected part; retain a common semantic group across its materials.
 if len(np.unique(ks))>1:
  _,weld=np.unique(np.round(v,4),axis=0,return_inverse=True);t=weld[a['tri']];edges=np.vstack([t[:,[0,1]],t[:,[1,2]],t[:,[2,0]]]);graph=coo_matrix((np.ones(len(edges)),(edges[:,0],edges[:,1])),shape=(weld.max()+1,weld.max()+1)).tocsr();nc,comp=connected_components(graph,directed=False);labels=comp[weld]
  for part in np.unique(labels):
   ix=np.flatnonzero(labels==part);names,counts=np.unique(ks[ix],return_counts=True);ks[ix]=names[np.argmax(counts)]
 for k in np.unique(ks):points.setdefault(str(k),[]).append(v[ks==k])
 parts.append(dict(**row,groups=ks));
floors=[];baselines={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())}
for r in g['objects']:
 if not inventory.get(r['path'],{}).get('render') or '/MarketIsland' in r['path']:continue
 if r['path'].split('/')[-1] not in ['Surface_176','Surface_142','Surface_104','Surface_125','Surface_153','Surface_134','Surface_173','Surface_174','Surface_175','Surface_127','Surface_128','Surface_129','RoadLoop_125314010_R65']:continue
 old=np.load(D/(baselines[r['path']]['cache']+'.npz'));new=np.load(D/(r['key']+'-graded.npz'));floors.append((Sampler(old['v'],old['tri'],np.zeros((len(old['tri']),3,2))),Sampler(new['v'],new['tri'],new['uv'])))
def supports(p,foot,state):
 values=[]
 for pair in floors:
  sam=pair[state];h=sam.sample(p)[0][:,1];h[~sam.last_valid]=np.nan;values.append(h)
 a=np.array(values);below=a.copy();below[below>foot[None,:]+.5]=np.nan
 result=np.nanmax(below,axis=0) if state==0 else np.nanmax(a,axis=0)
 for i in np.flatnonzero(~np.isfinite(result)):
  valid=a[:,i][np.isfinite(a[:,i])]
  if len(valid):result[i]=valid[np.argmin(abs(valid-foot[i]))]
 return result
def paired_supports(p,foot):
 old=[];new=[]
 for pair in floors:
  for sam,out in zip(pair,[old,new]):
   h=sam.sample(p)[0][:,1];h[~sam.last_valid]=np.nan;out.append(h)
 a=np.array(old);b=np.array(new);eligible=np.isfinite(a)&(a<=foot[None,:]+.5);score=np.where(eligible,a,-np.inf);ix=np.argmax(score,axis=0);cols=np.arange(len(p));before=a[ix,cols];after=b[ix,cols]
 for i in range(len(p)):
  if not eligible[:,i].any():
   ids=np.flatnonzero(np.isfinite(a[:,i]))
   if len(ids):ix[i]=ids[np.argmin(abs(a[ids,i]-foot[i]))];before[i]=a[ix[i],i];after[i]=b[ix[i],i]
  if not np.isfinite(after[i]):
   vals=b[:,i][np.isfinite(b[:,i])]
   if len(vals):after[i]=vals[np.argmin(abs(vals-target(p[i:i+1])[0]))]
 return before,after
anchors=[]
for k,ps in points.items():
 v=np.vstack(ps);bottom=float(v[:,1].min());low=v[v[:,1]<bottom+.15];xz=np.median(low[:,[0,2]],axis=0);anchors.append(dict(group=k,xz=xz.tolist(),bottom=bottom,box=[v.min(0).tolist(),v.max(0).tolist()],vertices=len(v)))
pp=np.array([r['xz'] for r in anchors]);ff=np.array([r['bottom'] for r in anchors]);old,new=paired_supports(pp,ff)
for i,r in enumerate(anchors):
 dy=float(new[i]-old[i]) if np.isfinite(new[i]+old[i]) else 0
 if r['group'].startswith('BUILDING_') and np.isfinite(new[i]):dy=float(new[i]-r['bottom']-.02)
 if r['group'] in ['BUILDING_TTC','Langfarm_R93','GateClock_R87_ESTIMATED']:dy=0
 if weight(pp[i:i+1])[0]<1e-6:dy=0
 r.update(old_support=float(old[i]),new_support=float(new[i]),delta=dy,method='Rigid translation; original footprint and proportions retained',status='CANDIDATE_PENDING_VISUAL_CONTACT')
by={r['group']:r for r in anchors};out=[]
for n,r in enumerate(parts):
 a=np.load(D/(r['cache']+'.npz'));v=a['v'];new=v.copy();dy=np.array([by[str(k)]['delta'] for k in r['groups']]);new[:,1]+=dy
 foundation=False
 if '/MarketEntrance_R72/' in r['path']:
  # Relevel only this existing entrance's lowest storey to the new plaza;
  # upper canopy ties retain the common market-building translation.
  falloff=1-ss((v[:,1]-9.818)/2.5);new[:,1]+=(target(v[:,[0,2]])-(9.818+dy))*falloff;foundation=True
 if abs(new[:,1]-v[:,1]).max()<.015:continue
 key='M95_P'+str(n).zfill(4);np.savez_compressed(D/(key+'-placement.npz'),v=new,before=v,tri=a['tri'],groups=r['groups']);out.append(dict(key=key,path=r['path'],asset=r['asset'],cache=r['cache'],groups=sorted(set(r['groups'].tolist())),vertices=len(v),foundation_geometry=foundation,delta_range=[float((new[:,1]-v[:,1]).min()),float((new[:,1]-v[:,1]).max())]))
(D/'placement-attribution-errors.json').write_text(json.dumps(attribution,indent=2))
(D/'placement-candidates.json').write_text(json.dumps(dict(status='NOT_APPLIED_TO_BLEND_OR_UNITY',groups=anchors,objects=out),indent=2,allow_nan=True));print('PLACEMENT_CANDIDATES',len(out),'groups',len(anchors),flush=True)
for r in anchors:
 if abs(r['delta'])>.05:print(r['group'],r['xz'],'old/new',round(r['old_support'],3),round(r['new_support'],3),'delta',round(r['delta'],3),flush=True)
