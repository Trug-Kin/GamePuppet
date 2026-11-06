import sys,json,struct
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from scipy.spatial import cKDTree
def nearest3d(pp,q,k=48):
 tree=cKDTree(q.mean(1));result_ids=[];result_w=[];result_e=[]
 for start in range(0,len(pp),2000):
  p=pp[start:start+2000];_,ix=tree.query(p,k=min(k,len(q)));qq=q[ix];aa,bb,cc=qq[:,:,0],qq[:,:,1],qq[:,:,2];ee=bb-aa;ff=cc-aa;dd=p[:,None,:]-aa;d00=(ee*ee).sum(2);d01=(ee*ff).sum(2);d11=(ff*ff).sum(2);d20=(dd*ee).sum(2);d21=(dd*ff).sum(2);den=d00*d11-d01*d01;safe=np.where(abs(den)>1e-24,den,1);u=(d11*d20-d01*d21)/safe;w=(d00*d21-d01*d20)/safe;bw=np.stack([1-u-w,u,w],2);proj=(qq*bw[:,:,:,None]).sum(2);dist=np.linalg.norm(proj-p[:,None,:],axis=2);dist[(bw.min(2)<0)|(abs(den)<1e-24)]=np.inf
  for i,j in [(0,1),(1,2),(2,0)]:
   edge=qq[:,:,j]-qq[:,:,i];tt=np.clip(((p[:,None,:]-qq[:,:,i])*edge).sum(2)/np.maximum((edge*edge).sum(2),1e-30),0,1);proj=qq[:,:,i]+edge*tt[:,:,None];de=np.linalg.norm(proj-p[:,None,:],axis=2);ok=de<dist;dist[ok]=de[ok];ww=np.zeros_like(bw);ww[:,:,i]=1-tt;ww[:,:,j]=tt;bw[ok]=ww[ok]
  mi=np.argmin(dist,axis=1);ii=np.arange(len(p));result_ids.append(ix[ii,mi]);result_w.append(bw[ii,mi]);result_e.append(dist[ii,mi])
 return np.concatenate(result_ids),np.vstack(result_w),np.concatenate(result_e)
D=Path('Test/KetQua/journey_v48/i2_r93_complete');G=Path('Test/KetQua/journey_v48/ground_r93');inv=json.loads((G/'geometry-inventory.json').read_text());rows=json.loads((D/'retopology.json').read_text());lines=[];report=[]
for row in rows:
 old=next(r for r in inv if r['path']==row['path']);b=np.load(G/(old['cache']+'.npz'));a=np.load(D/(row['key']+'-source.npz'));pp=a['before'];v=b['v'].astype(float);tri=b['tri'];q=v[tri];aa,bb,cc=q[:,0],q[:,1],q[:,2];den=(bb[:,2]-cc[:,2])*(aa[:,0]-cc[:,0])+(cc[:,0]-bb[:,0])*(aa[:,2]-cc[:,2]);ok=abs(den)>1e-10;q=q[ok];den=den[ok];tids=np.flatnonzero(ok);tree=STRtree(shapely.polygons(q[:,:,[0,2]]));pi,ti=tree.query(shapely.points(pp[:,[0,2]]));aa,bb,cc=q[ti,0],q[ti,1],q[ti,2];x,z=pp[pi][:,[0,2]].T;dd=den[ti];u=((bb[:,2]-cc[:,2])*(x-cc[:,0])+(cc[:,0]-bb[:,0])*(z-cc[:,2]))/dd;w=((cc[:,2]-aa[:,2])*(x-cc[:,0])+(aa[:,0]-cc[:,0])*(z-cc[:,2]))/dd;mask=(u>=-1e-4)&(w>=-1e-4)&(u+w<=1.0001);pi,ti,u,w=pi[mask],ti[mask],u[mask],w[mask];ww=np.c_[u,w,1-u-w];positions=(q[ti]*ww[:,:,None]).sum(1);err=np.linalg.norm(positions-pp[pi],axis=1);order=np.lexsort((err,pi));pi,ti,ww,err=pi[order],ti[order],ww[order],err[order];first=np.r_[True,np.diff(pi)!=0];ids=np.full(len(pp),-1,int);weights=np.zeros((len(pp),3));errors=np.full(len(pp),np.inf);ids[pi[first]]=ti[first];weights[pi[first]]=ww[first];errors[pi[first]]=err[first];miss=np.flatnonzero(ids<0)
 if len(miss):
  ix=tree.nearest(shapely.points(pp[miss][:,[0,2]]));snap=shapely.get_coordinates(shapely.get_point(shapely.shortest_line(shapely.points(pp[miss][:,[0,2]]),shapely.polygons(q[ix][:,:,[0,2]])),-1));aa,bb,cc=q[ix,0],q[ix,1],q[ix,2];x,z=snap.T;dd=den[ix];u=((bb[:,2]-cc[:,2])*(x-cc[:,0])+(cc[:,0]-bb[:,0])*(z-cc[:,2]))/dd;w=((cc[:,2]-aa[:,2])*(x-cc[:,0])+(aa[:,0]-cc[:,0])*(z-cc[:,2]))/dd;ids[miss]=ix;weights[miss]=np.c_[u,w,1-u-w];errors[miss]=np.linalg.norm((q[ix]*weights[miss,:,None]).sum(1)-pp[miss],axis=1)
 oldids=tri[tids[ids]];n=len(pp)
 # Native vertical faces / undersides cannot be mapped by an XZ top-surface query.
 # Preserve exact old vertices first; project remaining failures to the true 3D surface.
 vd,vi=cKDTree(v).query(pp);same=vd<.0002;oldids[same]=np.repeat(vi[same,None],3,axis=1);weights[same]=[1,0,0];errors[same]=vd[same];bad=np.flatnonzero(errors>.005)
 if len(bad):
  ni,nw,ne=nearest3d(pp[bad],v[tri]);oldids[bad]=tri[ni];weights[bad]=nw;errors[bad]=ne
 with open(D/(row['key']+'-mapping.bin'),'wb') as f:f.write(struct.pack('<i',n));f.write(oldids.astype('<i4').tobytes());f.write(weights.astype('<f4').tobytes())
 with open(D/(row['key']+'-positions.bin'),'wb') as f:f.write(struct.pack('<i',n));f.write(a['v'].astype('<f4').tobytes())
 report.append(dict(key=row['key'],path=row['path'],vertices=n,max_source_distance=float(errors.max()),over_005=int((errors>.005).sum()),nearest_boundary_fallback=len(miss)));lines.append('\t'.join([row['key'],row['path'],old['asset']]))
 print(report[-1],flush=True)
(D/'unity-mapping.json').write_text(json.dumps(report,indent=2));(D/'integration-manifest.tsv').write_text('\n'.join(lines));assert all(r['max_source_distance']<.005 for r in report),'Provenance mismatch: no integration'
