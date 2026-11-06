exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());bi={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())}
for name in ['Surface_176','Context_124']:
 r=next(r for r in g['objects'] if r['path'].endswith('/'+name))
 for state,fn in [('baseline',bi[r['path']]['cache']),('graded',r['key']+'-graded')]:
  a=np.load(D/(fn+'.npz'));q=a['v'][a['tri']];p=shapely.Point(624,390);poly=shapely.polygons(q[:,:,[0,2]]);ii=np.flatnonzero(shapely.covers(poly,p));sam=Sampler(a['v'],a['tri'],np.zeros((len(q),3,2)));h=sam.sample(np.array([[624,390]]))[0];print(name,state,h.tolist(),'faces',[(int(i),q[i].tolist(),bool(a['is_floor'][i]) if 'is_floor' in a else None) for i in ii],flush=True)
