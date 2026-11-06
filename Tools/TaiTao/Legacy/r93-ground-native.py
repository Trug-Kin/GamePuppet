import bpy
with bpy.data.libraries.load('D:/ConRoi/Docs/Reviews/journey_v48/r90_review/terrain-stage-r90-source.blend') as (a,b):b.objects=['TerrainJoin_R90']
o=b.objects[0];print('NATIVE_GROUND',list(o.location),list(o.data.vertices[0].co),o.data.materials[:])
