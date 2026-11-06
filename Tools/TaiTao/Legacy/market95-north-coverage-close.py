from pathlib import Path
import re
D=Path('Test/KetQua/journey_v48/market_ground_r95');M=Path('Test/BangChung/journey_v48/market_ground_r95')
for a,b in [(D/'north-final-motion.txt',D/'north-obstruction-motion.txt'),(M/'market95-north-final-controller.mp4',M/'market95-north-obstruction-controller.mp4')]:
 if a.exists() and not b.exists():a.rename(b)
p=Path('Test/Tam/market95-north-final-motion.cs.txt');s=p.read_text();s=re.sub(r'Vector2\[\] path=\{.*?\};','Vector2[] path={new Vector2(845,120),new Vector2(845,138),new Vector2(845,120),new Vector2(845,85)};',s).replace('t<70','t<45');p.write_text(s)
