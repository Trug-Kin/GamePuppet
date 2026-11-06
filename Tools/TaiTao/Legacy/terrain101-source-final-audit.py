import bpy,json,numpy as np,hashlib
from pathlib import Path
from mathutils import Vector
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/terrain_r101';P=R/'Source/Blender/JourneyV48/R101/Terrain_R101_Editable.blend';allhash=[]
def hashes():
 out={}
 for o in bpy.context.scene.objects:
  if o.type!='MESH':continue
  v=np.empty(len(o.data.vertices)*3,dtype=np.float32);o.data.vertices.foreach_get('co',v);l=np.empty(len(o.data.loops),dtype=np.int32);o.data.loops.foreach_get('vertex_index',l);out[o.name]=hashlib.sha256(v.tobytes()+l.tobytes()+np.array(o.matrix_world,dtype=np.float32).tobytes()).hexdigest()
 return out
bpy.ops.wm.open_mainfile(filepath=str(R/'Source/Blender/JourneyV48/R100/OngDao_Dam_R100_Editable.blend'));old=hashes();bpy.ops.wm.open_mainfile(filepath=str(P));new=hashes();changed=[n for n in old if old[n]!=new.get(n)];expected=['BankR91_Grade_017','R97_LOWER_WALK','R97_LOWER_GRASS','M95_Service3_NATIVE','TerrainColliderJoin_R90'];assert set(changed)==set(expected),changed
col=bpy.data.collections['TERRAIN_R101_EDITABLE_REPAIR']
for n in expected:
 o=bpy.data.objects[n]
 if n not in col.objects:col.objects.link(o)
sc=bpy.context.scene;sc['terrain101_status']='Final source, local tear/joins/collider repair. Game adaptation; M2 remains unverified.';sc['terrain101_unity_mapping']='Unity=(-BlenderX,BlenderZ,-BlenderY). FBX Unity preset; bound meshes keep original scene transforms.'
for screen in bpy.data.screens:
 for area in screen.areas:
  if area.type=='VIEW_3D':
   region=area.spaces.active.region_3d;region.view_location=Vector((-648,-416,1));region.view_distance=18;area.spaces.active.clip_end=3000
bpy.ops.wm.save_as_mainfile(filepath=str(P));bpy.ops.wm.read_factory_settings(use_empty=True);bpy.ops.wm.open_mainfile(filepath=str(P))
missing=[im.filepath for im in bpy.data.images if im.source=='FILE' and not im.packed_file and im.filepath and not Path(bpy.path.abspath(im.filepath)).exists()];libraries=[l.filepath for l in bpy.data.libraries];assert not missing and not libraries
report=dict(mesh_objects_before=len(old),mesh_objects_after=len(new),changed_objects=changed,unchanged_objects=len(old)-len(changed),missing_images=missing,libraries=libraries,unit_scale=bpy.context.scene.unit_settings.scale_length,collection=col.name if False else 'TERRAIN_R101_EDITABLE_REPAIR',reopen_file=bpy.data.filepath,scope='Five source objects only. Source142 winding/fan/seam; two r97 end ties; two obsolete collider cuts in finite rectangle. No claim of all-map topology/field dimensions.')
(D/'source-final-audit.json').write_text(json.dumps(report,indent=2));print('RESULT',json.dumps(report),flush=True)
