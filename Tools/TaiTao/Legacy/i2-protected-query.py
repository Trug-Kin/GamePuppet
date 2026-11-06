exec(open('Test/Tam/i2-gate.py').read().split('rows=json.loads')[0])
inv=json.loads((D/'native-input.json').read_text())['objects'];pp=np.array([[714.94,473.7],[702.3,494],[744,497],[707,487],[733.5,485.4]]);out=[]
for r in inv:
 if r['path'].startswith('LakeGateMarket') or r['path'].split('/')[-1] in ['Surface_128','Surface_129','Surface_142','Surface_176','Surface_153']:continue
 a=np.load(D/(r['key']+'.npz'));h,_=Surf(a['v'],a['tri']).query(pp);out.append(dict(path=r['path'],heights=[float(x) if np.isfinite(x) else None for x in h]))
print(json.dumps(out,indent=2));(D/'protected-floor-query.json').write_text(json.dumps(dict(xz=pp.tolist(),layers=out),indent=2))
