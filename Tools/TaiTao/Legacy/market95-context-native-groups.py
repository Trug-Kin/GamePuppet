from pathlib import Path
s=Path('Test/Tam/market95-native-placement-groups.py').read_text()
s=s.replace("selected=[r for r in catalog", "catalog=[dict(r,mesh=r['name']) for r in json.loads((R/'Assets/Art/Environment/LakeGateMarketV32/context.json').read_text())['meshes']]\nselected=[r for r in catalog")
s=s.replace("'R20_Whole_Source_Export/'+r['mesh']", "'LakeGateMarketV32/'+r['mesh']").replace("inventory[p]['render']", "(inventory[p]['render'] or inventory[p]['collider'])")
s=s.replace('ntmk_full_map_route_materials_v33.blend','ntmk_full_map_lake_gate_street_v32.blend').replace('placement-native-groups.json','placement-context-native-groups.json')
exec(compile(s,'context-native-attribution','exec'))
