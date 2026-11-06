import bpy,bmesh,numpy as np,json,sys,math
from pathlib import Path
from mathutils import Vector
from mathutils.bvhtree import BVHTree
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene
sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
rows=[]
for o in list(sc.objects):
 if o.type!='MESH' or (not o.name.startswith('R99_') or o.name.startswith('R99_SUPPORT_')):continue
 bm=bmesh.new();bm.from_mesh(o.data);nm=sum(not e.is_manifold for e in bm.edges);deg=sum(f.calc_area()<1e-10 for f in bm.faces);bm.free();prev=o.location.copy();v=np.array([p.co for p in o.data.vertices]);o.location-=Vector(v.mean(0));bpy.context.view_layer.update();g=bx_audit.deliverable([o],profile='game');o.location=prev;bpy.context.view_layer.update();rows.append(dict(object=o.name,nonmanifold_edges=nm,degenerate_faces=deg,gate=g))
(D/'candidate-local-gate-final.json').write_text(json.dumps(rows,indent=2,default=str))
for id in ['181347','181402']:
 sc.camera=bpy.data.objects['R99_APPROX_PHOTO_CAMERA_'+id];sc.render.filepath=str(M/('candidate-final-'+id+'.png'));bpy.ops.render.render(write_still=True)
# Direct diagnostic of deck-side objects, stairs, candidate shoreline and lower tie.
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
def pos(s,d,h):
 x,z=A+T*s+N*d;return Vector((-x,-z,h))
cam=sc.camera.copy();cam.data=sc.camera.data.copy();cam.name='R99_TECHNICAL_CLOSE_CAMERA';sc.collection.objects.link(cam);sc.camera=cam;cam.data.lens=28
for tag,eye,at in [('gate-tie',(L+5,8,2),(L+1,2,1.3)),('far-tie',(-7,8,2),(-1,2,1.4)),('toe-middle',(L*.5,8,.9),(L*.5,4.2,.15))]:
 cam.location=pos(*eye);cam.rotation_euler=(pos(*at)-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/('candidate-final-'+tag+'.png'));bpy.ops.render.render(write_still=True)
sc.display.shading.type='WIREFRAME';cam.location=pos(L/2,14,8);cam.rotation_euler=(pos(L/2,2,1.0)-cam.location).to_track_quat('-Z','Y').to_euler();sc.render.filepath=str(M/'candidate-final-wire.png');bpy.ops.render.render(write_still=True);sc.display.shading.type='SOLID'
# Cross-section BVH measurements are same coordinates and include visible apron only.
measure=[]
for rev,path in [('r99',P),('r98',R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend')]:
 if rev=='r98':bpy.ops.wm.open_mainfile(filepath=str(path))
 o=bpy.data.objects['R99_CONCRETE_DAM_13_ESTIMATED_BAYS' if rev=='r99' else 'R98_CONCRETE_DAM_13_BAYS'];tree=BVHTree.FromPolygons([o.matrix_world@v.co for v in o.data.vertices],[tuple(p.vertices) for p in o.data.polygons])
 for s in [2.2,10.,25.,40.,55.]:
  points=[]
  for d in np.arange(-.2,13.001,.05):
   hit,n,ix,dist=tree.ray_cast(pos(s,float(d),12),Vector((0,0,-1)),20)
   points.append(dict(d=float(d),h=float(hit.z) if hit else None))
  measure.append(dict(revision=rev,s=s,samples=points))
(D/'same-position-sections.json').write_text(json.dumps(measure,indent=2))
# No saving: all renders/audits from reopened persisted candidate, R98 untouched.
print('RESULT reopened candidate gates',sum(bool(r['gate']['pass']) for r in rows),'/',len(rows),'; render and same-position sections complete, GLOBAL GATE STILL OPEN',flush=True)
