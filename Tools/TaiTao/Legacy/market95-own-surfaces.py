exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());by={r['path'].split('/')[-1]:r for r in g['objects']};log=[]
scope=shapely.box(640,155,861,452).difference(shapely.Point(714.94,459.48).buffer(18,quad_segs=96)).difference(shapely.box(400,390,704,650))
groups=[['Surface_104','Surface_125','Surface_153','Surface_134','RoadLoop_125314010_R65','Surface_173','Surface_174','Surface_175','Surface_127','Surface_128','Surface_129','Surface_176'],['Context_108','Context_106','Context_136','Context_110','Context_111','Context_116','Context_133','Context_130','Restored_028','Restored_029','Context_124']]
for priority in groups:
 owned=shapely.Polygon()
 for name in priority:
  if name not in by:continue
  row=by[name];fp=D/(row['key']+'-graded.npz');a=np.load(fp);v=a['v'];t=a['tri'];uv=a['uv'];slots=a['slots'];before=a['before'];flags=a['is_floor'] if 'is_floor' in a else np.zeros(len(t),bool);q=v[t];c=q.mean(1);pick=flags&(c[:,0]>635)&(c[:,0]<866)&(c[:,2]>150)&(c[:,2]<457)
  if not pick.any():continue
  pp=shapely.set_precision(shapely.polygons(q[pick][:,:,[0,2]]),.00001);foot=shapely.union_all(pp).intersection(scope)
  cut=foot.intersection(owned);area=cut.area;owned=owned.union(foot)
  if area<.01:continue
  cut=shapely.union_all([p for p in pieces(cut) if p.area>1e-7]);shapely.prepare(cut)
  ppoly=shapely.polygons(q[:,:,[0,2]]);affected=np.flatnonzero(flags&shapely.intersects(cut,ppoly));covered=shapely.covers(cut,ppoly);keep=np.ones(len(t),bool);keep[affected]=False;vs=[v[t[keep]].reshape(-1,3)];bs=[before[t[keep]].reshape(-1,3)];us=[uv[keep].reshape(-1,2)];sslot=[slots[keep]];ff=[flags[keep]]
  native=np.load(D/(row['key']+'-native.npz'));sam=Sampler(native['v'],native['tri'],native['uv']);made=0
  for i in affected:
   if covered[i]:continue
   poly=ppoly[i].difference(cut)
   coords=[]
   for part in pieces(poly):
    if part.area<1e-9:continue
    coords.extend(np.array(p.exterior.coords)[:3].tolist() for p in shapely.constrained_delaunay_triangles(part).geoms if p.area>1e-10)
   if not coords:continue
   xz=np.array(coords).reshape(-1,2);aa,bb,cc=q[i][:,[0,2]];den=np.cross(bb-aa,cc-aa);ww1=np.cross(bb-xz,cc-xz)/den;ww2=np.cross(cc-xz,aa-xz)/den;bw=np.c_[ww1,ww2,1-ww1-ww2];new=bw@q[i];new[:,0]=xz[:,0];new[:,2]=xz[:,1];new=new.reshape(-1,3,3);nu=(bw@uv[i]).reshape(-1,3,2);nb=sam.sample(xz)[0].reshape(-1,3,3);fl=np.cross(new[:,1]-new[:,0],new[:,2]-new[:,0])[:,1]>0;new[fl]=new[fl][:,::-1];nu[fl]=nu[fl][:,::-1];nb[fl]=nb[fl][:,::-1];vs.append(new.reshape(-1,3));us.append(nu.reshape(-1,2));bs.append(nb.reshape(-1,3));sslot.append(np.full(len(new),slots[i]));ff.append(np.ones(len(new),bool));made+=len(new)
  vv=np.vstack(vs);bb=np.vstack(bs);uu=np.vstack(us).reshape(-1,3,2);sl=np.concatenate(sslot);fl=np.concatenate(ff);tri=np.arange(len(vv)).reshape(-1,3);_,ix,inv=np.unique(np.round(vv,7),axis=0,return_index=True,return_inverse=True);vv=vv[ix];bb=bb[ix];tri=inv[tri];nn=np.cross(vv[tri[:,1]]-vv[tri[:,0]],vv[tri[:,2]]-vv[tri[:,0]]);valid=np.linalg.norm(nn,axis=1)>1e-10
  np.savez_compressed(fp,v=vv,tri=tri[valid],before=bb,uv=uu[valid],slots=sl[valid],is_floor=fl[valid]);log.append(dict(path=row['path'],removed_overlap_area=float(area),clipped_faces=len(affected),remaining_faces=len(tri[valid]),added_faces=made));print('OWNERSHIP',name,area,len(affected),flush=True)
(D/'surface-ownership.json').write_text(json.dumps(dict(scope='Full market ground footprint; renderer and collider ownership handled independently; structural bank/I2 excluded',priority=groups,changes=log),indent=2))
