from pathlib import Path
p=Path('Test/Tam/market95-continuous-field.py');s=p.read_text();a=s.index('w=ss((xx-450)/254)');b=s.index('solution=np.array',a)
s=s[:a]+'''w=ss((xx-450)/254)
desired=target(pp).reshape(xx.shape)
edge=raw[:,np.argmin(abs(zs-435))][:,None]
fraction=np.clip(np.maximum(raw,0)/np.maximum(edge,.1),0,1)
bank=raw+(.72-edge)*fraction
desired=np.where(zz>435,bank,desired)
values=raw+(desired-raw)*w
# Conform to the actual PROTECTED EDGE height, not remote legacy hills.
# Reusing old height at each distant point retained a new hump around the
# rectangle. This correction is zero at its outer feather and matches the
# measured boundary, over the full width, with smooth end derivatives.
protected=shapely.box(400,390,704,600).union(shapely.box(650,448,756,600)).union(shapely.Point(714.94,459.48).buffer(18,quad_segs=128))
nearest=shapely.get_coordinates(shapely.get_point(shapely.shortest_line(shapely.points(pp),protected.boundary),-1))
edgeh=sam.sample(nearest)[0][:,1]
basefield=RegularGridInterpolator((xs,zs),values,bounds_error=False,fill_value=None)
d=shapely.distance(shapely.points(pp),protected)
correction=(edgeh-basefield(nearest))*(1-ss(d/80))
values+=correction.reshape(xx.shape)
''' +s[b:]
s=s.replace('blend=ss(distance/30)','blend=ss(distance/10)')
p.write_text(s)
