exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
for r in rows:
 if '/MarketIsland' not in r['path'] or not any(s in r['path'] for s in ['Island_Paved','Platform_Tier','Pedestal','STUDY_Figure']):continue
 a=np.load(D/(r['key']+'-native.npz'));v=a['v'];print(r['path'].split('/')[-1],v.min(0).tolist(),v.max(0).tolist(),'Y',np.unique(np.round(v[:,1],4))[:20].tolist())
 if 'Island_Paved' in r['path']:
  s=Sampler(v,a['tri'],a['uv']);p=np.array([[783.948,191.167],[780,188],[790,200]]);print('BASE_TOP',s.sample(p)[0].tolist(),flush=True)
