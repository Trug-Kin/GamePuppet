import bpy,json,numpy as np,hashlib
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/i2_r93_complete';P=R/'Source/Blender/JourneyV48/R93/Ground_I2_R93_Editable.blend';gate=json.loads((D/'blender-reopen.json').read_text());assert not gate['libraries'] and not gate['missing_images'];assert all(x['wrong_winding_local']==0 and x['zero_area_local']==0 and x['world_error']<.001 for x in gate['objects']);bpy.ops.wm.open_mainfile(filepath=str(P));sc=bpy.context.scene;root=bpy.data.collections['GROUND_NATIVE_MATCHED_R93']
groups={}
for name in ['I2_ROAD','I2_FOOTWAY','I2_TERRAIN']:
 c=bpy.data.collections.get(name) or bpy.data.collections.new(name)
 if c.name not in root.children:root.children.link(c)
 groups[name]=c
for o in list(root.objects):
 if not o.get('i2_export_key'):continue
 role='I2_ROAD' if o['unity_path'].split('/')[-1] in ['Surface_128','Surface_129'] else 'I2_FOOTWAY' if o['unity_path'].endswith('/Surface_153') else 'I2_TERRAIN';groups[role].objects.link(o);root.objects.unlink(o)
for o in root.all_objects:
 if o.get('i2_export_key'):o['i2_status']='BLENDER_TECHNICAL_SCOPE_ACCEPTED_UNITY_PENDING'
cc=bpy.data.collections.get('I2_REVIEW_CAMERAS') or bpy.data.collections.new('I2_REVIEW_CAMERAS')
if cc.name not in sc.collection.children:sc.collection.children.link(cc)
shots=[('I2_OVERVIEW',[-685,-496,22],[-715,-470,.6]),('I2_PLAYER_LOW',[-715,-484,2.3],[-715,-471,.3]),('I2_BRIDGE_JOIN',[-696,-480,2.4],[-708,-472,.5]),('I2_EAST_JOIN',[-738,-477,2.5],[-725,-472,.5]),('I2_CONNECTOR_LOW',[-697,-491,2.7],[-705,-481,.8]),('I2_BRANCH_LOW',[-744,-497,1.8],[-731,-482,.3])]
for name,eye,target in shots:
 o=bpy.data.objects.get(name)
 if not o:o=bpy.data.objects.new(name,bpy.data.cameras.new(name));cc.objects.link(o)
 o.location=eye;o.rotation_euler=(Vector(target)-o.location).to_track_quat('-Z','Y').to_euler();o.data.lens=32
sc.camera=bpy.data.objects['I2_OVERVIEW'];sc['status']='BLENDER_TECHNICAL_SCOPE_ACCEPTED_UNITY_PENDING';text=bpy.data.texts['I2_README'];text.clear();text.write('I2 working editable source. Open collections I2_ROAD, I2_FOOTWAY, I2_TERRAIN within GROUND_NATIVE_MATCHED_R93; independent colliders in COLLIDERS_NOT_FOR_RENDER. I2_BEFORE_DO_NOT_EXPORT remains immutable source copies. Review cameras in I2_REVIEW_CAMERAS. No Unity diagnostic context is saved as authoring geometry.\nOne unit =1 authoring metre; Blender native to Unity=(-X,Z,-Y). Local origins at I2 reduce float precision loss; do not compensate scale in Unity.\nOriginal GroundSurvey_R93_Editable.blend and r93 scene preserved. Heightfield full-width ring + source connector reference + unchanged bridge boundary pins, stable outer/adjacent-floor guards. Remove hidden overlapping terrain within existing walking footprints; no overlay. All UV restoration and mesh bindings documented in Test/KetQua/journey_v48/i2_r93_complete.\nTechnical correction only: island plane and bridge unchanged. Local floor slopes are not a field survey; banks and existing external joins remain separately limited. No whole-map flatness claim. Reproduction: i2-retopology.py -> i2-save-retopology.py -> i2-verify-blend.py -> fixed-camera renders + i2-metrics.py + i2-gate.py -> finalization -> FBX roundtrip -> scoped derived Unity integration.\n')
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P));accept=dict(accepted=True,scope='I2 road/footway transition and authored terrain meeting existing walking footprints; technical/visual Blender gate only',source=str(P),sha256=hashlib.sha256(P.read_bytes()).hexdigest(),visual_review=['overview','player-low','bridge-join','east-join','connector-low','branch-low'],limits=['Not a surveyed height datum or field-accuracy acceptance','Steep banks / existing exterior joins remain separate','Unity/controller/load still pending','No product scene or checkpoint changed']);(D/'blender-acceptance.json').write_text(json.dumps(accept,indent=2));print('BLENDER_SCOPE_ACCEPTED',accept['sha256'])
