from pathlib import Path
p=Path('Test/Tam/r107-ground-probe.cs.txt');s=p.read_text();a=s.index('var pts=new[]{');b=s.index(';foreach(var p in pts)',a)
s=s[:a]+'var pts=new[]{new Vector2(681,430),new Vector2(665,430),new Vector2(650,440),new Vector2(660,455),new Vector2(700,440)}'+s[b:]
s=s.replace('triangle-probe.txt','adjacent-triangle-probe.txt');Path('Test/Tam/r107-adjacent-probe.cs.txt').write_text(s)
