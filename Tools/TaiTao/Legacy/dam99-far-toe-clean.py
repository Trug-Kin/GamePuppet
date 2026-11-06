import bpy,json,numpy as np,ast
from pathlib import Path
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'
bpy.ops.wm.open_mainfile(filepath=str(P));A=np.array([637.7344,528.2603]);B=np.array([681.4578,491.2729]);L=float(np.linalg.norm(B-A));T=(B-A)/L;N=np.array([T[1],-T[0]]);water=np.array(json.loads((R/'Test/KetQua/journey_v48/market_ground_r95/protected-actual-water.geojson').read_text())['coordinates'][0]);groups={'SCOPED_LEGACY_CUTS':bpy.data.collections['R98_SCOPED_LEGACY_CUTS']}
for file,names in [('dam98-author.py',['shore','pos']),('bank97-author.py',['half','difference'])]:
 tree=ast.parse((R/'Test/Tam'/file).read_text());exec(compile(ast.Module(body=[n for n in tree.body if isinstance(n,ast.FunctionDef) and n.name in names],type_ignores=[]),file,'exec'))
# Cut the confirmed retained terrain slivers extending past the unchanged water
# edge; do not hide them by covering or moving the water. No other bank touched.
def current_shore(s):
 u=max(0,min(1,(s+18)/15));v=max(0,min(1,(L+18-s)/15));w=u*u*(3-2*u)*v*v*(3-2*v);end=shore(s);return end+(4.5-end)*w
ss=np.linspace(-4.5,-3.,7);quads=[]
for a,b in zip(ss,ss[1:]):quads.append(np.array([pos(a,current_shore(a)-.02,0)[::2],pos(b,current_shore(b)-.02,0)[::2],pos(b,current_shore(b)+.75,0)[::2],pos(a,current_shore(a)+.75,0)[::2]]))
eligible=[bpy.data.objects['BankR91_Grade_017']];cut=[]
s=(R/'Test/Tam/dam98-author.py').read_text();block=s[s.index('for idx,o in enumerate(eligible):'):s.index('# 13 concrete bays')];block=block.replace("o['dam98_key']='L98_'+str(idx).zfill(2)","o['dam98_key']='L98_12'");block=block.replace('max()>-3','max()>-5').replace('min()<L+3','min()<-2.9');exec(block)
bpy.context.preferences.filepaths.save_version=0;bpy.ops.wm.save_as_mainfile(filepath=str(P),compress=True);(D/'far-toe-cleanup.json').write_text(json.dumps(dict(reason='Confirmed far-head Surface142 fin exposed after winding correction; cut only s[-4.5,-3] at candidate shoreline, no overlay or new water-level change',cut=cut,footprint=[q.tolist() for q in quads]),indent=2));print('RESULT toe cleanup',flush=True)
