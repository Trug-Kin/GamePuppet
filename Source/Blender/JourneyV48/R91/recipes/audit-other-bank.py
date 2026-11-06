"""Local north-bank heightfield repair in editable Blender, not an overlay.
Shore XY and road/dry endpoints are retained; local cross section is an estimate.
"""
import bpy,bmesh,json,sys,math,struct
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/Game/GameHorror/Game');O=R/'Docs/Reviews/journey_v48/r91';A=R/'Assets/Art/Environment/JourneyV48/BankR91';A.mkdir(parents=True,exist_ok=True);C=Path('C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101')
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-export/scripts'));import bx_export
bpy.ops.wm.open_mainfile(filepath=str(R/'Docs/Reviews/journey_v48/r90_review/market-branches-r90-source.blend'))
names=['Grade_017','Grade_000','Grade_057','Grade_015','Grade_007']
trees={}
def tree(o):return BVHTree.FromPolygons([o.matrix_world@v.co for v in o.data.vertices],[list(p.vertices) for p in o.data.polygons])
for n in names:trees[n]=tree(bpy.data.objects[n])
def hit(t,x,y):
 p,n,idx,d=t.ray_cast(Vector((x,y,40)),Vector((0,0,-1)),80)
 return None if p is None else p.z
def ground(x,y):
 p=hit(trees['Grade_017'],x,y)
 if p is not None and p>-.9:return p
 for n in ('Grade_000','Grade_057'):
  q=hit(trees[n],x,y)
  if q is not None and q>-.9:return q-.04
 return p
curve=[(517.03,514.71),(565.15,531.84),(590.44,540.67),(596.05,542.17),(601,542.37),(606.82,539.77),(648.81,505.94)]
def closest(x,y):
 p=Vector((-x,-y));best=None
 for a,b in zip(curve,curve[1:]):
  a=Vector(a);b=Vector(b);d=b-a;t=max(0,min(1,(p-a).dot(d)/d.length_squared));q=a+d*t;n=Vector((-d.y,d.x)).normalized()
  if n.y<0:n=-n
  dist=(p-q).length
  if best is None or dist<best[0]:best=(dist,(p-q).dot(n),q,n)
 return best
def smooth(v):v=max(0,min(1,v));return v*v*(3-2*v)
def target(x,y):
 if not -643<x<-545:return None
 dist,d,q,n=closest(x,y)
 if d<-1.8 or d>7 or dist>9:return None
 edge=ground(-(q.x+n.x*7),-(q.y+n.y*7))
 if edge is None or edge<.15:return None
 t=max(0,min(1,d/7));h=.025+(edge-.025)*t
 if d<0:h=-1+1.025*((d+1.8)/1.8)
 fade=smooth(((-x)-545)/10)*smooth((643-(-x))/10)
 return h,fade,d

for o in bpy.context.scene.objects:
 if not o.name.startswith('Grade_') or o.name in names:continue
 points=[o.matrix_world@v.co for v in o.data.vertices]
 hits=[(w,target(w.x,w.y)) for w in points if -653<w.x<-535 and -559<w.y<-506]
 hits=[(w,t) for w,t in hits if t is not None]
 if hits:print('OTHER_SUPPORT',o.name,len(hits),'maxdelta',round(max(abs(t[0]-w.z)*t[1] for w,t in hits),3),flush=True)
