import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'))
A=np.array([637.7344,528.2603]);T=np.array([.76346655,-.64584737]);N=np.array([-.64584737,-.76346655]);rows=[]
for tag,stations in [('FAR',[-2.75,-1.5,-.25]),('GATE',[57.519568,58.769568,60.019568])]:
 o=bpy.data.objects['R99_'+tag+'_STAIR_AND_LOW_LANDING'];tree=BVHTree.FromPolygons([o.matrix_world@p.co for p in o.data.vertices],[tuple(p.vertices) for p in o.data.polygons])
 for s in stations:
  for d in [.15,.4,.5714286,.8142857,1.0571429,1.3,1.5428571,1.7857143,2.0285714,2.2714286,2.5142857,2.7571429,3.,3.2428571,3.4857143,3.7285714,4.15,4.4]:
   x,z=A+T*s+N*d;p,n,_,_=tree.ray_cast(Vector((-x,-z,8)),Vector((0,0,-1)),12);assert p and n.z>.9;rows.append((s,d,p.z))
(D/'source-stair-expected.tsv').write_text('\n'.join('\t'.join(map(str,row)) for row in rows))
old=D/'actual-support-grid.txt';old.rename(D/'support-grid-initial-wrong-step-formula.txt')
p=R/'Test/Tam/map100-support-grid.cs.txt';s=p.read_text();a=s.index('foreach(float s in new[]{-2.75f');b=s.index('var hits=Physics.RaycastAll',a);s=s[:a]+'''foreach(var line in File.ReadAllLines("Test/KetQua/journey_v48/map_r99_closeout/source-stair-expected.tsv")){var e=line.Split('\\t');float s=float.Parse(e[0],System.Globalization.CultureInfo.InvariantCulture),d=float.Parse(e[1],System.Globalization.CultureInfo.InvariantCulture),expected=float.Parse(e[2],System.Globalization.CultureInfo.InvariantCulture);var p=a+t*s+n*d;'''+s[b:];s=s.replace('Actual Unity Physics rays; finite samples, no claim for every edge.','Actual Unity Physics rays vs reopened editable render mesh BVH at identical positions; finite samples, no claim for every edge. Initial ceil formula was one step early and is retained separately, not a product failure.');p.write_text(s)
print('RESULT',len(rows),'expected samples from source triangles, no assumed step formula')
