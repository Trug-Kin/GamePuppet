import re
from pathlib import Path
s=Path('Test/Tam/market95-motion.cs.txt').read_text();a=s.index('Physics.SyncTransforms();');b=s.index('double begin=',a);s=s[:a]+s[b:]
s=re.sub(r'Vector2\[\] path=\{.*?\};','Vector2[] path={new Vector2(845,125),new Vector2(850,140),new Vector2(850,175),new Vector2(845,175)};',s).replace('t<480','t<45').replace('controller-motion.txt','north-return-motion.txt').replace('one start fixture','continuous from current north-test endpoint; no placement').replace('One initial local fixture','Continuation from current north endpoint without placement')
Path('Test/Tam/market95-north-return-motion.cs.txt').write_text(s)
r=Path('Test/Tam/market95-record.py').read_text().replace("['--branch','--north']","['--branch','--north','--north-return']").replace("('145' if local=='branch' else '110')","('145' if local=='branch' else '50' if local=='north-return' else '110')")
Path('Test/Tam/market95-record.py').write_text(r)
