exec(open('Test/Tam/dam98-basis.py').read().split('for name')[0])
for s in [57.2696,60.2696]:
 p=A+t*s;hit=LineString([p,p+n*40]).intersection(LineString(w));print('end',s,p,hit.wkt)
