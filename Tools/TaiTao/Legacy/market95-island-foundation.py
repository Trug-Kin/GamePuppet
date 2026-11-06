exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
islands=[r for r in rows if '/MarketIsland_R62/' in r['path']];base=next(r for r in islands if r['path'].endswith('/Island_Paved_Base'));a=np.load(D/(base['key']+'-native.npz'))
# Unity coordinates reflect native handedness. The upper base sheet has negative
# Y winding here. Outside the base perimeter a nearest query over BOTH top and
# underside alternated between sheets, creating 24cm teeth in the outer curb.
q=a['v'][a['tri']];normal=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);upper=normal[:,1]<-.001
sam=Sampler(a['v'],a['tri'][upper],a['uv'][upper]);centre=np.array([[783.948,191.167]]);old_base=float(sam.sample(centre)[0][0,1]);top=5.12;shift=top-old_base;g=json.loads((D/'grading.json').read_text());out=[]
for row in islands:
 a=np.load(D/(row['key']+'-native.npz'));v=a['v'];new=v.copy();name=row['path'].split('/')[-1];support=sam.sample(v[:,[0,2]])[0][:,1];delta=top-support
 if name.startswith(('STUDY_','Pedestal_')) or name in ['Platform_Tier_1','Platform_Tier_2']:
  new[:,1]+=shift;mode='Rigid upper monument translation; preserve upright shape and stack'
 elif name=='Platform_Tier_0':
  blend=ss((v[:,1]-(v[:,1].max()-.05))/.05);new[:,1]+=delta+(shift-delta)*blend;mode='Flat upper tread retained, underside follows relevelled island foundation'
 else:new[:,1]+=delta;mode='Relevel from actual native paved-base support, not fitted road plane'
 np.savez_compressed(D/(row['key']+'-graded.npz'),v=new,tri=a['tri'],before=v,uv=a['uv'],slots=a['slots']);entry=next(r for r in g['objects'] if r['key']==row['key']);entry['mode']=mode;entry['delta_range']=[float((new[:,1]-v[:,1]).min()),float((new[:,1]-v[:,1]).max())];out.append(dict(path=row['path'],mode=mode,delta_range=entry['delta_range']))
g['island_support_design']=dict(native_base_centre=old_base,design_base_top=top,upper_monument_rigid_shift=shift,road_level=5.04,XZ_unchanged=True);(D/'grading.json').write_text(json.dumps(g,indent=2));(D/'island-foundation.json').write_text(json.dumps(out,indent=2));print('ISLAND_FOUNDATION',old_base,top,shift,len(out),flush=True)
