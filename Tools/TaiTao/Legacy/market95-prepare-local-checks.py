import re
from pathlib import Path
s=Path('Test/Tam/market95-motion.cs.txt').read_text()
for tag,start,points,limit in [
 ('branch',(710,431),[(696,413),(697,398),(705,385),(714,401),(725,431),(730,438),(714,401),(705,385),(697,398),(696,413),(710,431)],140),
 ('north',(845,175),[(845,140),(845,110),(845,85),(830,100),(845,120),(830,140),(845,160),(845,175)],105)]:
 x,z=start;p=s.replace('new Vector3(730,10,438)',f'new Vector3({x},15,{z})').replace('s.Place(new Vector3(730,',f's.Place(new Vector3({x},').replace(',438),Quaternion.Euler',f',{z}),Quaternion.Euler')
 p=re.sub(r'Vector2\[\] path=\{.*?\};', 'Vector2[] path={'+','.join(f'new Vector2({x},{z})' for x,z in points)+'};',p)
 p=p.replace('t<480',f't<{limit}').replace('controller-motion.txt',f'{tag}-motion.txt').replace('max480s',f'max{limit}s')
 Path(f'Test/Tam/market95-{tag}-motion.cs.txt').write_text(p)
r=Path('Test/Tam/market95-record.py').read_text();r=r.replace("seam='--seam' in sys.argv","seam='--seam' in sys.argv\nlocal=next((a[2:] for a in sys.argv if a in ['--branch','--north']),None)")
r=r.replace("'Test/Tam/i2-seam-motion.cs.txt' if seam else 'Test/Tam/market95-motion.cs.txt'", "'Test/Tam/market95-'+local+'-motion.cs.txt' if local else ('Test/Tam/i2-seam-motion.cs.txt' if seam else 'Test/Tam/market95-motion.cs.txt')")
r=r.replace("'485'", "('145' if local=='branch' else '110') if local else '485'").replace("'ultrafast'","'fast'").replace("'19'","'23'")
r=r.replace("'market95-controller.mp4'", "'market95-'+local+'-controller.mp4' if local else 'market95-controller.mp4'")
r=r.replace("done=data/'controller-motion.txt'", "done=data/(local+'-motion.txt' if local else 'controller-motion.txt')")
r=r.replace("tag='probe' if probe else", "tag=local if local else 'probe' if probe else")
Path('Test/Tam/market95-record.py').write_text(r)
