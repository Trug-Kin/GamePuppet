exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
vis={r['path']:r['render'] for r in json.loads((D/'inventory.json').read_text())['rows']};g=json.loads((D/'grading.json').read_text());pol={};corebox=shapely.box(704,165,860,435)
for r in g['objects']:
 if not vis.get(r['path']) or '/MarketIsland' in r['path']:continue
 a=np.load(D/(r['key']+'-graded.npz'))
 if 'is_floor' not in a:continue
 q=a['v'][a['tri']];c=q.mean(1);sel=a['is_floor']&(c[:,0]>700)&(c[:,0]<865)&(c[:,2]>160)&(c[:,2]<440)
 if sel.any():pol[r['path']]=shapely.union_all(shapely.set_precision(shapely.polygons(q[sel][:,:,[0,2]]),.0005)).intersection(corebox)
out=[];items=list(pol.items())
for i,(n,p) in enumerate(items):
 for m,q in items[i+1:]:
  overlap=p.intersection(q).area
  if overlap>.001:out.append(dict(a=n,b=m,area=float(overlap)))
(D/'render-overlaps.json').write_text(json.dumps(out,indent=2));print(out,flush=True)
