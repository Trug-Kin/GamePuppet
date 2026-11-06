import json
from pathlib import Path
D=Path('Test/KetQua/journey_v48/map_r99_closeout');rows=json.loads((D/'r100-export-selection.json').read_text());(D/'r100-integration.tsv').write_text('\n'.join('\t'.join(r[k] for k in ['alias','key','path']) for r in rows))
p=Path('Test/Tam/map100-integrate.cs.txt');s=p.read_text();start=s.index('var rows=JsonUtility.FromJson<Rows>');end=s.index(';var ts=',start);s=s[:start]+'var rows=File.ReadAllLines(d+"r100-integration.tsv").Select(l=>l.Split(\'\\t\')).Select(e=>new Row{alias=e[0],key=e[1],path=e[2]}).ToArray()'+s[end:];p.write_text(s)
