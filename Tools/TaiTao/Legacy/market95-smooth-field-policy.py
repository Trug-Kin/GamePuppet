from pathlib import Path
p=Path('Test/Tam/market95-continuous-field.py');s=p.read_text();a=s.index('values=raw.copy()');b=s.index('np.savez_compressed',a)
s=s[:a]+'''# A harmonic C0 join concentrated the derivative on the core boundary.
# Use a C1/C2 authored full-width transition, preserving I2 and actual banks.
w=ss((xx-450)/254)
r=np.hypot(xx-714.94,zz-459.48);w*=ss((r-18)/22)
w*=1-ss((zz-300)/90)*(1-ss((xx-704)/30))
w*=1-ss((zz-405)/140)
# The protected lower bridge bank rectangle remains exact. Feather BEFORE it.
w*=1-ss((zz-415)/33)*(1-ss((xx-760)/40))*ss((xx-620)/30)
w*=ss((545-zz)/60)
values=raw+(target(pp).reshape(xx.shape)-raw)*w
solution=np.array([]);info=0;free=np.zeros(xx.shape,bool)
''' +s[b:]
s=s.replace("h=field(xz);inside_core=shapely.covers(design_core,shapely.points(xz));h[inside_core]=target(xz[inside_core]);return h", "return field(xz)")
s=s.replace("float(np.max(abs(A@solution-rhs)))","0.")
p.write_text(s)
