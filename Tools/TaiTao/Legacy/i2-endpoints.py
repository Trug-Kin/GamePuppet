exec(open('Test/Tam/i2-gate.py').read().split('rows=json.loads')[0])
inv=json.loads((D/'native-input.json').read_text())['objects'];ss={}
for r in inv:
 if r['path'].split('/')[-1] not in ['Surface_128','Surface_129','Surface_142','Surface_176','Surface_153']:continue
 a=np.load(D/(r['key']+'.npz'));ss[r['path'].split('/')[-1]]=Surf(a['v'],a['tri'])
out=[]
for deg in [0,30,60,90,120,150,180]:
 u=np.array([math.cos(math.radians(deg)),math.sin(math.radians(deg))]);rr=np.arange(12,61,2);p=np.array([cx,cz])+rr[:,None]*u
 row=dict(angle=deg,radii=rr.tolist(),layers={})
 for name,s in ss.items():
  h,_=s.query(p);row['layers'][name]=[round(float(x),3) if np.isfinite(x) else None for x in h]
 out.append(row)
(D/'stable-endpoint-survey.json').write_text(json.dumps(out,indent=2));print(json.dumps(out))
