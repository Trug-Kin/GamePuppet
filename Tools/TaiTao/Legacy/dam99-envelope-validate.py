from pathlib import Path
p=Path('Test/Tam/dam99-throat-cut.py');s=p.read_text(encoding='utf-8-sig');s=s.replace('o.data=o.data.copy();o.data.transform(o.matrix_world);','oldtree=BVHTree.FromPolygons([o.matrix_world@v.co for v in o.data.vertices],[tuple(p.vertices) for p in o.data.polygons]);oldpoints=[o.matrix_world@Vector(q) for q in o.bound_box];lo=np.min(np.array(oldpoints),axis=0);hi=np.max(np.array(oldpoints),axis=0);o.data=o.data.copy();o.data.transform(o.matrix_world);')
s=s.replace("assert outside_max<.002,(name,'changed outside cut',outside_max)",'''envelope=[];absent=0
 for q in samples:
  for axis in range(3):
   for sign in [-1,1]:
    start=q.copy();start[axis]=float(hi[axis]+1 if sign<0 else lo[axis]-1);direction=Vector((0,0,0));direction[axis]=sign
    a,an,ai,ad=oldtree.ray_cast(start,direction,400)
    if not a:continue
    ss,dd=(np.array([-a.x,-a.y])-A)@np.c_[T,N]
    if L-.08<ss<L+3.08 and -.06<dd<4.64:continue
    b,bn,bi,bd=tree.ray_cast(start,direction,400)
    if not b:absent+=1
    else:envelope.append(float((a-b).length))
 envelope_max=max(envelope,default=0);print('EXPOSED_ENVELOPE',name,'samples',len(envelope),'missing',absent,'max',envelope_max,flush=True)
 assert not absent and envelope_max<.002,(name,'exposed geometry changed outside cut',absent,envelope_max)''')
s=s.replace('outside_samples=len(samples),outside_max_distance=outside_max','outside_samples=len(samples),nearest_centroid_distance_including_internal_caps=outside_max,envelope_samples=len(envelope),envelope_missing=absent,envelope_max_distance=envelope_max');p.write_text(s,encoding='utf-8')
