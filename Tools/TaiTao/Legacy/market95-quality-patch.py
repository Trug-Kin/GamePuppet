from pathlib import Path
p=Path('Test/Tam/market95-grade.py');s=p.read_text();s=s.replace('from scipy.spatial import Delaunay','from scipy.spatial import Delaunay\nimport triangle as trmesh')
a=s.index('  dt=Delaunay(points);');b=s.index(' if not newtris:continue',a)
s=s[:a]+'''  # Constrained quality triangulation avoids the clipped long slivers exposed by full-face audit.
  vertices=[];segments=[];holes=[]
  for ring in [poly.exterior]+list(poly.interiors):
   co=np.asarray(ring.coords)[:-1];start=len(vertices);vertices.extend(co.tolist());segments.extend([(start+j,start+(j+1)%len(co)) for j in range(len(co))])
  for ring in poly.interiors:
   q=shapely.Polygon(ring).representative_point();holes.append([q.x,q.y])
  inp={'vertices':np.array(vertices),'segments':np.array(segments,dtype='i4')}
  if holes:inp['holes']=np.array(holes)
  result=trmesh.triangulate(inp,'pq20a0.85Q')
  if 'triangles' in result:newtris.extend(result['vertices'][result['triangles']].tolist())
'''+s[b:]
s=s.replace('shape=shapely.union_all(shapely.polygons(q[select][:,:,[0,2]]));area0=shape.area','shape=shapely.set_precision(shapely.union_all(shapely.polygons(q[select][:,:,[0,2]])),.0005);area0=shape.area')
s=s.replace("oq=q[~select].reshape(-1,3);allv=", "oq=q[~select].reshape(-1,3).copy();adj=np.isin(tri[~select].ravel(),np.unique(tri[select]));op=oq[:,[0,2]];oq[adj,1]+=weight(op[adj])*(target(op[adj])-oq[adj,1]);allv=")
s=s.replace('allb=np.vstack([oq,old]);','allb=np.vstack([q[~select].reshape(-1,3),old]);')
s=s.replace('tt,uu,sl=tt[keep],uu[keep],sl[keep]','floor_flags=np.r_[np.zeros(int((~select).sum()),bool),np.ones(len(nt),bool)][good][keep];tt,uu,sl=tt[keep],uu[keep],sl[keep]')
s=s.replace('before=bb,uv=uu,slots=sl)','before=bb,uv=uu,slots=sl,is_floor=floor_flags)')
p.write_text(s,encoding='utf-8')
p=Path('Test/Tam/market95-measure.py');s=p.read_text();s=s.replace('valid=(abs(nn[:,1])>1e-7)&(w>.99);transition=(abs(nn[:,1])>1e-7)&(w>1e-3);','floors=a[\'is_floor\'] if \'is_floor\' in a else np.ones(len(t),bool);valid=(abs(nn[:,1])>1e-7)&(w>.99)&floors;transition=(abs(nn[:,1])>1e-7)&(w>1e-3)&floors;');p.write_text(s,encoding='utf-8')
