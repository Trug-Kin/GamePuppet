exec(open('Test/Tam/dam99-photo-fit.py',encoding='utf-8-sig').read().split('best=None')[0])
from scipy.optimize import least_squares
base=json.loads((D/'photo-fit-initial.json').read_text());p=np.array(base['solution']);rows=[]
for w in [2.5,3.9,5.5,7.,10.2]:
 def fun(q):return residual(np.r_[q[0],w,q[1:]])
 q=least_squares(fun,np.r_[p[0],p[2:]],bounds=([.4]+lo[2:] if 'lo' in globals() else [.4]+[-150,5,.3,-3.14,-.7,-.12,700]*2,[7]+[200,220,20,3.14,.3,.12,2200]*2),max_nfev=2200)
 rows.append(dict(run=w,height=float(q.x[0]),rms=float(np.sqrt(np.mean(q.fun**2))),params=q.x.tolist()))
(D/'section-fit-run-sensitivity.json').write_text(json.dumps(rows,indent=2));print([(r['run'],r['height'],r['rms']) for r in rows])
