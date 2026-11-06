import bpy,json,numpy as np
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';P=R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend';assert not (D/'fan-delta.json').exists();bpy.ops.wm.open_mainfile(filepath=str(P))
o=bpy.data.objects['BankR91_Grade_017'];v=[o.matrix_world@p.co for p in o.data.vertices];neighbors=[set() for _ in v]
for e in o.data.edges:
 a,b=e.vertices;neighbors[a].add(b);neighbors[b].add(a)
changes=[]
for i,w in enumerate(v):
 x,z=-w.x,-w.y
 if not (649<x<651 and 411<z<418 and len(neighbors[i])==4):continue
 ns=[v[j] for j in neighbors[i]];mid=sum(ns,Vector())/4
 if abs(mid.x-w.x)>.001 or abs(mid.y-w.y)>.001:continue
 if abs(mid.z-w.z)<1e-6:continue
 changes.append(dict(vertex=i,x=x,z=z,before=w.z,after=mid.z));w.z=mid.z;o.data.vertices[i].co=o.matrix_world.inverted()@w
o.data.update();assert changes
o['terrain101_fan_fix']='Valence4 ramp fan centres linearly interpolated from fixed corners, X649..651/Z411..418. XY and all corners held. Correct technical sawtooth, not shoreline or market regrade.'
fan_changes=changes[:];code=(R/'Test/Tam/terrain101-close-joins.py').read_text();exec(code[code.index("base=bpy.data.objects"):code.index('# Remove only')].replace("max(abs(r['after']-r['before']) for r in changed)","max((abs(r['after']-r['before']) for r in changed),default=0)"));changes=fan_changes
(D/'fan-delta.json').write_text(json.dumps(dict(changes=changes,vertices=len(changes),max_abs_dy=max(abs(p['after']-p['before']) for p in changes),join_rematch=[{k:v for k,v in r.items() if k!='changes'} for r in rows]),indent=2));bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P));print('RESULT fan vertices',len(changes),flush=True)
