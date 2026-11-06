exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());bi={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())}
p=np.array([[x,z] for z in [58.8,70,125,165,320,390,405,420,435,440,445,450] for x in [624,690,704,715,730,746,760,780,810,850]])
out={}
for name in ['Surface_104','Surface_134','Surface_176','Context_124']:
 r=next(r for r in g['objects'] if r['path'].endswith('/'+name));a=np.load(D/(bi[r['path']]['cache']+'.npz'));sam=Sampler(a['v'],a['tri'],np.zeros((len(a['tri']),3,2)));h=sam.sample(p)[0][:,1];h[~sam.last_valid]=np.nan;out[name]=np.round(h,3).reshape(-1,10).tolist()
print('columns X624,690,704,715,730,746,760,780,810,850; rows Z58.8,70,125,165,320,390,405,420,435,440,445,450');print(json.dumps(out,indent=1))
