import bpy,bmesh,numpy as np,json,sys,math
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';M=R/'Test/BangChung/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend';old=R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'
A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]])
fit=json.loads((D/'photo-fit-initial.json').read_text());params=fit['solution'][2:]
def camera(sc,i):
 ex,ez,ey,yaw,pitch,roll,f=params[i*7:i*7+7];xz=A+T*ex+N*ez;fw=np.array([np.cos(pitch)*np.cos(yaw),np.sin(pitch),np.cos(pitch)*np.sin(yaw)]);xzfw=T*fw[0]+N*fw[2]
 cam=sc.camera;cam.location=(-xz[0],-xz[1],ey);cam.rotation_euler=Vector((-xzfw[0],-xzfw[1],fw[1])).to_track_quat('-Z','Y').to_euler();cam.rotation_euler.rotate_axis('Z',-roll);cam.data.type='PERSP';cam.data.sensor_fit='HORIZONTAL';cam.data.lens=f*36/1920;cam.data.sensor_width=36;sc.render.resolution_x=1280;sc.render.resolution_y=720;sc.render.resolution_percentage=100
for rev,path in [('r98',old),('candidate',P)]:
 bpy.ops.wm.open_mainfile(filepath=str(path));sc=bpy.context.scene
 if rev=='candidate':
  # Restore historical BEFORE objects and display-only water proxy from protected input.
  names=[o.name for o in sc.objects if o.name.startswith('I2_BEFORE_')]+['CONTEXT_WATER_Y0_UNCHANGED']
  for name in names:
   o=bpy.data.objects.get(name)
   if o:bpy.data.objects.remove(o,do_unlink=True)
  with bpy.data.libraries.load(str(old),link=False) as (src,dst):dst.objects=[n for n in names if n in src.objects]
  reference=bpy.data.collections.new('R99_UNCHANGED_ARCHIVAL_REFERENCES');sc.collection.children.link(reference)
  for o in dst.objects:
   if o:reference.objects.link(o);o.hide_render=True;o.hide_set(True)
  # Remove welded junctions between capped metal tubes by splitting nonmanifold
  # edges into independent capped components; do not claim rail gate passed.
  for o in sc.objects:
   if o.type=='MESH' and o.name.startswith('R99_'):
    if 'dam98_export' in o:del o['dam98_export']
  root=bpy.data.collections['DAM_R99_SECTION_CANDIDATE_NOT_INTEGRATED'];root.name='DAM_R99_SECTION_CANDIDATE__NOT_APPROVED'
  sc['dam99_status']='NOT APPROVED / NOT INTEGRATED. Section ratio study. Both camera fits approximate. Shoreline plan warp unverified; full support/continuity and rail gate outstanding. R98 remains product.'
  sc['dam99_original_source']=str(old)
 else:
  # This proxy is present in R98 authoring; keep the same source/water preview policy.
  pass
 # Show actual water mesh for comparing footprint, never a second overlapping proxy.
 proxy=bpy.data.objects.get('CONTEXT_WATER_Y0_UNCHANGED')
 if proxy:proxy.hide_render=True
 water=bpy.data.objects.get('C95_0103_Surface_051')
 if water:water.hide_render=False
 for i,ph in enumerate(fit['photos']):
  camera(sc,i);sc.render.filepath=str(M/(rev+'-matched-'+ph['id']+'.png'));bpy.ops.render.render(write_still=True)
 if rev=='candidate':
  sys.path.insert(0,str(R/'.agents/skills/scenario-blender-expert/scripts'));import bx_audit
  report=[]
  for o in list(sc.objects):
   if o.type!='MESH' or not o.name.startswith('R99_'):continue
   bm=bmesh.new();bm.from_mesh(o.data);edges=sum(not e.is_manifold for e in bm.edges);deg=sum(f.calc_area()<1e-10 for f in bm.faces);bm.free()
   prev=o.location.copy();vs=np.array([v.co for v in o.data.vertices]);o.location-=Vector(vs.mean(0));bpy.context.view_layer.update();gate=bx_audit.deliverable([o],profile='game');o.location=prev;bpy.context.view_layer.update()
   report.append(dict(object=o.name,nonmanifold_edges=edges,degenerate_faces=deg,gate=gate))
  (D/'candidate-local-gate.json').write_text(json.dumps(report,indent=2,default=str))
  missing=[im.filepath for im in bpy.data.images if not im.packed_file and im.source not in ['GENERATED','VIEWER'] and not Path(bpy.path.abspath(im.filepath)).exists()]
  (D/'candidate-dependencies.json').write_text(json.dumps(dict(libraries=[li.filepath for li in bpy.data.libraries],missing_images=missing,unit_scale=sc.unit_settings.scale_length),indent=2))
  # Both fixed comparison cameras live inside the editable file.
  basecam=sc.camera
  for i,ph in enumerate(fit['photos']):
   camera(sc,i);cam=basecam.copy();cam.data=basecam.data.copy();cam.name='R99_APPROX_PHOTO_CAMERA_'+ph['id'];sc.collection.objects.link(cam)
  sc.camera=next(o for o in sc.objects if o.name=='R99_APPROX_PHOTO_CAMERA_181347')
  bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True)
  print('RESULT gates',sum(bool(r['gate']['pass']) for r in report),'of',len(report),'candidate remains NOT_APPROVED',flush=True)
