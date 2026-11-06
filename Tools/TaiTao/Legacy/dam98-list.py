import json
r=json.load(open('Test/KetQua/journey_v48/dam_r98/blender-inventory.json'))['rows']
for x in r:
 if x['key'] or x['name'].startswith('C95_065'):print(x['name'],x['key'],x['path'],x['faces'],x['hide_render'])
