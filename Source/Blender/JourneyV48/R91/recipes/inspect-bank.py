import bpy,json,sys
from pathlib import Path
p=Path('C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101/north-bank-r91-source-refined.blend')
bpy.ops.wm.open_mainfile(filepath=str(p))
print('BANK_COUNTS',[(o.name,len(o.data.vertices),len(o.data.polygons),o.hide_get(),o.select_get()) for o in bpy.context.scene.objects if o.type=='MESH' and o.name.startswith('BankR91_')],flush=True)
