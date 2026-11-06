import bpy
bpy.ops.wm.open_mainfile(filepath='C:/Users/kaito/AppData/Local/Temp/ConRoi-r91-authoring-20261101/character-repairs-r91-source.blend')
o=bpy.data.objects['R91_Portrait'];print('PORTRAIT',o.parent.name if o.parent else None,o.matrix_world,flush=True)
