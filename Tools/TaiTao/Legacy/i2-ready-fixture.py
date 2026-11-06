from pathlib import Path
for file in ['i2-fixture.cs.txt','i2-motion.cs.txt']:
 p=Path('Test/Tam')/file;s=p.read_text(encoding='utf-8');s=s.replace('s.Load();s.SetMenu(false);','s.SetMenu(false);');p.write_text(s,encoding='utf-8')
