exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());by={r['path'].split('/')[-1]:r for r in g['objects']};baseline={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())};rows=json.loads((D/'render-collider-layer-comparison.json').read_text())
for r in rows:
 if not r['worst']:continue
 pp=np.array([q['xz'] for q in r['worst']]);heights=[]
 for name in [r['render'],r['collider']]:
  old=baseline[by[name]['path']];a=np.load(D/(old['cache']+'.npz'));sam=Sampler(a['v'],a['tri'],np.zeros((len(a['tri']),3,2)));h=sam.sample(pp)[0][:,1];heights.append(h)
 for q,a,b in zip(r['worst'],*heights):q.update(old_render=float(a),old_collider=float(b),old_difference=float(abs(a-b)))
 print(r['render'],r['collider'],r['worst'][-1],flush=True)
(D/'render-collider-layer-comparison.json').write_text(json.dumps(rows,indent=2))
