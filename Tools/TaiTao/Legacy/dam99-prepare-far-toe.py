from pathlib import Path
R=Path('D:/ConRoi');s=(R/'Test/Tam/dam98-toe-clean.py').read_text(encoding='utf-8-sig');s=s.replace("D=R/'Test/KetQua/journey_v48/dam_r98';P=R/'Source/Blender/JourneyV48/R98/OngDao_Dam_R98_Editable.blend'","D=R/'Test/KetQua/journey_v48/dam_r99';P=R/'Source/Blender/JourneyV48/R99/OngDao_Dam_R99_SectionCandidate.blend'")
a=s.index('ss=sorted(');b=s.index('eligible=',a)
s=s[:a]+'''def current_shore(s):
 u=max(0,min(1,(s+18)/15));v=max(0,min(1,(L+18-s)/15));w=u*u*(3-2*u)*v*v*(3-2*v);end=shore(s);return end+(4.5-end)*w
ss=np.linspace(-4.5,-3.,7);quads=[]
for a,b in zip(ss,ss[1:]):quads.append(np.array([pos(a,current_shore(a)-.02,0)[::2],pos(b,current_shore(b)-.02,0)[::2],pos(b,current_shore(b)+.75,0)[::2],pos(a,current_shore(a)+.75,0)[::2]]))
''' + s[b:]
s=s.replace('exec(block)',"block=block.replace('max()>-3','max()>-5').replace('min()<L+3','min()<-2.9');exec(block)")
s=s.replace("'toe-cleanup.json'","'far-toe-cleanup.json'").replace('Confirmed narrow legacy Surface142 fins beyond unchanged water edge in low comparison views; remove source faces, no overlay/water move','Confirmed far-head Surface142 fin exposed after winding correction; cut only s[-4.5,-3] at candidate shoreline, no overlay or new water-level change')
(R/'Test/Tam/dam99-far-toe-clean.py').write_text(s,encoding='utf-8')
