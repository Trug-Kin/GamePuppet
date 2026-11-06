exec(compile(open('D:/ConRoi/Test/Tam/investigation103-evidence.py',encoding='utf-8').read().split("for n in ['ShoeSample'")[0],'helpers','exec'))
remove('TornFabric')
edge=[(-.06,0),(-.04,.015),(-.02,-.007),(.004,.024),(.024,.006),(.06,.018)]
outline=edge+[(.06,-.08),(-.06,-.08)];N=len(outline)
mesh=bpy.data.meshes.new('R103_FragmentSolid');mesh.from_pydata([P((x,y,z)) for y in (0,.002) for x,z in outline],[],[tuple(range(N-1,-1,-1)),tuple(range(N,2*N))]+[(i,(i+1)%N,(i+1)%N+N,i+N) for i in range(N)]);mesh.update();frag=bpy.data.objects.new('FragmentBase',mesh);col.objects.link(frag);mesh.materials.append(cloth);parts=[frag]
for i,(x,z) in enumerate(edge):parts.append(rod('FrayedFibre',(x,.002,z-.003),(x+.002,.002,z+.009+(i%2)*.006),.00055,thread))
for i in range(6):parts.append(box('Stitch',(-.045+i*.016,.0024,-.036),(.007,.0006,.003),thread))
for i in range(8):parts.append(box('Weave',(-.05+i*.014,.0022,-.055),(.0008,.0004,.042),thread))
join(parts,'TornFabric')
coat=bpy.data.objects['CoatSample'];bm=bmesh.new();bm.from_mesh(coat.data);bad=[f for f in bm.faces if coat.data.materials[f.material_index]==thread];bmesh.ops.delete(bm,geom=bad,context='FACES');loose=[v for v in bm.verts if not v.link_faces];bmesh.ops.delete(bm,geom=loose,context='VERTS');bm.to_mesh(coat.data);bm.free()
from mathutils.bvhtree import BVHTree
verts=[coat.matrix_world@v.co for v in coat.data.vertices];tree=BVHTree.FromPolygons(verts,[list(p.vertices) for p in coat.data.polygons]);parts=[coat]
for i in range(6):
 x=.335+i*.016;z=-.20;hit=tree.ray_cast(P((x,1,z)),P((0,-1,0)))
 if hit[0]:parts.append(box('AttachedCuffStitch',(x,hit[0].z+.0008,z),(.007,.001,.003),thread))
join(parts,'CoatSample')
scene.render.engine='BLENDER_WORKBENCH';scene.display.shading.background_type='VIEWPORT';scene.display.shading.background_color=(.13,.15,.16);scene.display.shading.color_type='MATERIAL';scene.render.resolution_x=1100;scene.render.resolution_y=800;scene.render.resolution_percentage=100
camera=scene.camera;camera.data.type='ORTHO'
for name,campos,look,scale in [('TornFabric',(.06,.25,.14),(0,0,-.025),.19),('CoatSample',(.7,.9,-.6),(.27,.07,-.15),.70),('TraceCrossing',(.32,.6,.45),(0,0,.04),.67),('ShoeSample',(.20,-.42,.30),(0,.03,0),.48),('ShoeControl',(.20,-.42,.30),(0,.03,0),.48)]:
 for o in scene.objects:o.hide_render=o.type=='MESH' and o.name!=name
 camera.location=P(campos);camera.rotation_euler=(P(look)-camera.location).to_track_quat('-Z','Y').to_euler();camera.data.ortho_scale=scale;scene.render.filepath=str(E/('source-'+name+'.png'));bpy.ops.render.render(write_still=True)
for o in scene.objects:o.hide_render=False
bpy.ops.wm.save_as_mainfile(filepath=str(S));bpy.ops.wm.open_mainfile(filepath=str(S));print('Repaired fibre contact and true cuff stitching; source reopened')
