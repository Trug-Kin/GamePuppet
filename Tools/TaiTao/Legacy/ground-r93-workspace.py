import bpy,json,math
from pathlib import Path
from mathutils import Vector,kdtree
from mathutils.bvhtree import BVHTree
import numpy as np
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/ground_r93';S=R/'Source/Blender/JourneyV48/R93';S.mkdir(parents=True,exist_ok=True)
stats=json.loads((D/'geometry-inventory.json').read_text());audit=json.loads((D/'editable-source-audit.json').read_text());target={r['path']:r for r in stats if r.get('cache') and r['active'] and (r['role'] in ['terrain','road','footway'] or r['path'].split('/')[-1] in ['Surface_127','Surface_134','Surface_142']) and not r['path'].startswith('RouteSurfaces/')}
for r in stats:
 if r['path'].split('/')[-1] in [n+'#COLLIDER' for n in ['Context_106','Context_108','Context_110','Context_111','Context_116','Context_124','Context_132','Context_136']]:target[r['path'].replace('#COLLIDER','')]=r
bpy.ops.wm.read_factory_settings(use_empty=True);sc=bpy.context.scene;sc.name='GROUND_R93_SOURCE_AUDIT_NOT_INTEGRATED';sc.unit_settings.system='METRIC';col=bpy.data.collections.new('GROUND_NATIVE_MATCHED_R93');sc.collection.children.link(col);staging=bpy.data.collections.new('SOURCE_CANDIDATES_HIDDEN');sc.collection.children.link(staging);staging.hide_render=True;staging.hide_viewport=True
best={};reports=[];trees={}
def tree_for(v,tri):return BVHTree.FromPolygons([Vector(p) for p in v],tri.tolist(),all_triangles=True)
def samples(v,tri):
 used=np.unique(tri);ix=np.unique(np.linspace(0,len(used)-1,min(1500,len(used))).astype(int));it=np.unique(np.linspace(0,len(tri)-1,min(1500,len(tri))).astype(int));return np.vstack([v[used[ix]],v[tri[it]].mean(1)])
for source in audit:
 candidates=[o for o in source['objects'] if o['path'] in target]
 with bpy.data.libraries.load(str(R/source['source']),link=False) as (a,b):b.objects=[o['name'] for o in candidates]
 for info,o in zip(candidates,b.objects):
  if o is None:continue
  staging.objects.link(o);path=info['path'];current=target[path];cache=np.load(D/(current['cache']+'.npz'));actual=cache['v'];at=cache['tri'];v=np.array([o.matrix_world@x.co for x in o.data.vertices]);v=np.c_[-v[:,0],v[:,2],-v[:,1]];o.data.calc_loop_triangles();tri=np.array([t.vertices for t in o.data.loop_triangles])
  if path not in trees:trees[path]=tree_for(actual,at)
  tree=trees[path];pa=samples(v,tri);err=[tree.find_nearest(Vector(p))[3] for p in pa];other=tree_for(v,tri);pb=samples(actual,at);err2=[other.find_nearest(Vector(p))[3] for p in pb];score=max(max(err,default=100),max(err2,default=100));rep=dict(path=path,source=source['source'],object=info['name'],source_vertices=len(v),unity_vertices=len(actual),sampled_bidirectional_max_distance=score,comparison='nearest triangle surface; used vertices and face centroids; not nearest vertex',coordinate='Unity=(-BlenderX, BlenderZ, -BlenderY)',sample_count_each_direction=[len(pa),len(pb)])
  reports.append(rep)
  if path not in best or score<best[path][0]:best[path]=(score,o,rep)
accepted=[]
for path,(score,o,rep) in best.items():
 if score>.005:continue
 staging.objects.unlink(o);col.objects.link(o);o['authoring_source']=rep['source'];o['source_object']=rep['object'];o['r93_world_position_check_max']=score;o.hide_set(False);o.hide_render=False;accepted.append(rep)
for o in list(staging.objects):bpy.data.objects.remove(o,do_unlink=True)
bpy.data.collections.remove(staging)
# Measurements are curves, outside export collection. No diagnostic Unity mesh becomes authoring source.
guide=bpy.data.collections.new('MEASUREMENTS_DO_NOT_EXPORT');sc.collection.children.link(guide)
for entry in json.loads((D/'sections.json').read_text()):
 if entry['name'].startswith('ring_') or entry['name'] in ['X_zoff0','Z_xoff0']:
  data=bpy.data.curves.new(entry['region']+'_'+entry['name'],'CURVE');data.dimensions='3D';sp=data.splines.new('POLY');sp.points.add(len(entry['xz'])-1)
  for p,(x,z) in zip(sp.points,entry['xz']):p.co=(-x,-z,26,1)
  o=bpy.data.objects.new(data.name,data);guide.objects.link(o)
guide.hide_render=True;guide.hide_viewport=True
sc['status']='Editable native ground audit baseline. No accepted geometry change and no export/Unity integration in this file. Provenance check tolerance 0.005 authoring units; record actual discrepancies, not exact identity.'
sc['north']='Blender +Y = UTM north = Unity -Z; east Blender +X = Unity -X'
sc['accuracy']='Units are authoring metres; local surveyed grade and absolute elevation UNVERIFIED. Sampled vertex matching is provenance evidence, not full surface equivalence.'
text=bpy.data.texts.new('READ_ME_GROUND_R93');text.write('Open collection GROUND_NATIVE_MATCHED_R93. Sources are actual editable r90/r91 authoring meshes, selected using unity_path and sampled bidirectional world surface distance. Vertex counts may differ after FBX seam split / removal of unused vertices. No Unity REVIEW mesh is used as replacement source. See Test/KetQua/journey_v48/ground_r93/source-workspace.json. No geometry has been corrected or exported from this audit baseline.\n')
sc.render.engine='BLENDER_WORKBENCH';sc.render.resolution_x=1400;sc.render.resolution_y=1000;sc.render.resolution_percentage=100;sc.display.shading.light='STUDIO';sc.display.shading.color_type='MATERIAL';sc.display.shading.show_shadows=True;sc.display.shading.show_cavity=True
camdata=bpy.data.cameras.new('GROUND_PLAN');cam=bpy.data.objects.new('GROUND_PLAN',camdata);sc.collection.objects.link(cam);cam.location=(-660,-325,550);cam.rotation_euler=(0,0,0);camdata.type='ORTHO';camdata.ortho_scale=600;sc.camera=cam
bpy.context.preferences.filepaths.save_version=0;sc.render.filepath=str(R/'Test/BangChung/journey_v48/ground_r93/native-ground-plan.png');bpy.ops.wm.save_as_mainfile(filepath=str(S/'GroundSurvey_R93_Editable.blend'));bpy.ops.render.render(write_still=True)
(D/'source-workspace.json').write_text(json.dumps(dict(file=str(S/'GroundSurvey_R93_Editable.blend'),accepted=accepted,candidates=reports,unmatched_paths=sorted(set(target)-{r['path'] for r in accepted}),status='AUDIT_BASELINE_NOT_A_CORRECTED_OR_INTEGRATED_MAP'),indent=2));print('WORKSPACE',len(accepted),'matched sources;',len(target)-len(accepted),'unmatched',flush=True)
