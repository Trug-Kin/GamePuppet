import bpy,json,numpy as np,ast
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98';P=R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]]);water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);groups={'SCOPED_LEGACY_CUTS':bpy.data.collections['R98_SCOPED_LEGACY_CUTS']}
for file,names in [('dam98-author.py',['shore','pos']),('bank97-author.py',['half','difference'])]:
 tree=ast.parse((R/'Test/Tam'/file).read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in names],type_ignores=[]),file,'exec'))
# Cut the confirmed retained terrain slivers extending past the unchanged water
# edge; do not hide them by covering or moving the water. No other bank touched.
ss=sorted(set([0.,L]+[float((p-A)@T) for p in water if 0<(p-A)@T<L and 0<(p-A)@N<15]));quads=[]
for a,b in zip(ss,ss[1:]):quads.append(np.array([pos(a,shore(a)-.20,0)[::2],pos(b,shore(b)-.20,0)[::2],pos(b,shore(b)+.75,0)[::2],pos(a,shore(a)+.75,0)[::2]]))
eligible=[bpy.data.objects['BankR91_Grade_017']];cut=[]
s=(R/'Test/Tam/dam98-author.py').read_text();block=s[s.index('for idx,o in enumerate(eligible):'):s.index('# 13 concrete bays')];block=block.replace("o['dam98_key']='L98_'+str(idx).zfill(2)","o['dam98_key']='L98_12'");exec(block)
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'toe-cleanup.json').write_text(json.dumps(dict(reason='Confirmed narrow legacy Surface142 fins beyond unchanged water edge in low comparison views; remove source faces, no overlay/water move',cut=cut,footprint=[q.tolist() for q in quads]),indent=2));print('RESULT toe cleanup',flush=True)
