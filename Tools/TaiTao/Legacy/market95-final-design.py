from pathlib import Path
p=Path('Test/Tam/market95-grade.py');s=p.read_text();a=s.index('profile=PchipInterpolator');b=s.index('# Full continuous site polygon',a)
s=s[:a]+'''# Final authorized game floor: flat core through the north map edge.
# Only Z320..435 is the external I2 approach. C1 easing over 10m at
# either end, constant grade between; 4.32m rise across 115m.
def target(p):
 z=p[:,1];t=np.clip(z-320,0,115);L=115.;e=10.;slope=4.32/(L-e)
 drop=np.where(t<e,slope*t*t/(2*e),np.where(t>L-e,4.32-slope*(L-t)**2/(2*e),slope*(t-e/2)))
 return 5.04-drop
''' +s[b:]
s=s.replace('w*=ss((p[:,1]-70)/95)','w*=1. # no artificial north rim: the source map ends near Z58.76')
p.write_text(s)
p=Path('Test/Tam/market95-continuous-field.py');s=p.read_text().replace('zs=np.arange(70.,546.1,2)','zs=np.arange(0.,546.1,2)').replace('box(450,70,1000,545)','box(450,0,1000,545)').replace('(zz>=155)&(zz<=437)','(zz>=0)&(zz<=437)').replace('box(704,165,860,435)','box(704,0,870,435)')
s=s.replace("skirt=pin&(blend>.999999)&(xz[:,0]>=704)&(xz[:,0]<=860)&(xz[:,1]>=165)&(xz[:,1]<=435);v[skirt,1]=final_height(xz[skirt])", "skirt=pin&(blend>.999999);v[skirt,1]=final_height(xz[skirt])")
s=s.replace('flat game-core radial profile constraints','flat 5.04 core and monotonic external I2 approach constraints')
p.write_text(s)
p=Path('Test/Tam/market95-preserve-boundary.py');s=p.read_text().replace('box(450,70,1000,545)','box(450,0,1000,545)').replace("newfloor=np.repeat(new[4],3)","newfloor=np.ones(len(new[0]),bool)")
p.write_text(s)
p=Path('Test/Tam/market95-island-foundation.py');s=p.read_text().replace('top=6.18','top=5.12').replace('road_level=6.1','road_level=5.04');p.write_text(s)
p=Path('Test/Tam/market95-placement-analysis.py');s=p.read_text().replace("if r['group'] in ['BUILDING_TTC','Langfarm_R93']:dy=0", "if r['group'] in ['BUILDING_TTC','Langfarm_R93','GateClock_R87_ESTIMATED']:dy=0")
p.write_text(s)
