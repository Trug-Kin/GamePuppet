import bpy,bmesh,json
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));assert not bpy.context.scene.get('dam99_cut_soil_plants_removed',False)
audit=json.loads((D/'flower-support-audit.json').read_text());removed=[dict(color=x['color'],index=x['index'],reason='Original planter soil was removed from the authored stair throat; remove its plants with that same source geometry, not an arbitrary relocation.') for x in audit if not any(h['object']=='C95_0045_OD35_Surface_5' and abs(h['root_gap'])<.02 for h in x['supports'])]
assert len(removed)==12,removed
placements=json.loads((R/'Docs/Reviews/journey_v48/r22/flower-placements.json').read_text())
for color in ['Red','Yellow']:
 count=sum(bool(x['yellow'])==(color=='Yellow') for x in placements);discard={x['index'] for x in removed if x['color']==color}
 for part in range(3):
  o=bpy.data.objects['D99_FLOWERS_'+color+'_'+str(part)];me=o.data;step=len(me.vertices)//count;attr=me.attributes.new('r22_plant_index','INT','POINT')
  for i,x in enumerate(attr.data):x.value=i//step
  bm=bmesh.new();bm.from_mesh(me);bm.verts.ensure_lookup_table();bmesh.ops.delete(bm,geom=[v for v in bm.verts if v.index//step in discard],context='VERTS');bm.to_mesh(me);bm.free();me.update()
  o['dam99_removed_plants']=json.dumps(sorted(discard))
bpy.context.scene['dam99_cut_soil_plants_removed']=True
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'removed-stair-throat-plants.json').write_text(json.dumps(removed,indent=2));print('RESULT removed',len(removed),'plants belonging to removed soil in stair throat',flush=True)
