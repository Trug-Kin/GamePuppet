from pathlib import Path
p=Path('Test/Tam/dam99-author.py');s=p.read_text(encoding='utf-8-sig');a=s.index('# Transform the actual existing');s=s[:a]+'''# Vectorized continuous field, avoid per-vertex polygon intersection.
sg=np.linspace(-18,L+18,1001);eg=np.array([shore(float(v)) for v in sg])
def warp_points(world):
 sd=(world[:,[0,2]]-A)@np.c_[T,N];ss,dd=sd.T
 w=smooth((ss+18)/15)*smooth((L+18-ss)/15);en=np.interp(ss,sg,eg)
 u=np.where(dd<=en,np.clip(dd/en,0,1),np.clip(1-(dd-en)/20,0,1))
 dn=dd+(W-en)*w*u
 across=np.where(dd<0,smooth((dd+20)/6),np.clip(1-dd/en,0,1))
 hh=world[:,1]+delta*w*across*np.clip((world[:,1]-.22)/.920076,0,1)
 xz=A+ss[:,None]*T+dn[:,None]*N
 return np.c_[xz[:,0],hh,xz[:,1]]
''' +s[a:];s=s.replace('out=np.array([warp_xyz(p) for p in world])','out=warp_points(world)');p.write_text(s,encoding='utf-8')
