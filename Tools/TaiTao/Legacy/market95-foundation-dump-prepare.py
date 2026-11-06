from pathlib import Path
p=Path('Test/Tam/market95-dump.cs.txt').read_text();start=p.index('if(m.isReadable&&File.ReadAllText');end=p.index('pick.Add(f);',start)+len('pick.Add(f);')
p=p[:start]+'if(m.isReadable&&path.Contains("Langfarm_R93/R93_SOLID_"))pick.Add(f);'+p[end:]
p=p.replace('d+"inventory.json"','d+"foundation-inventory.json"').replace('scene-ground.meshbin','foundation-baseline.meshbin').replace('scene-components.txt','foundation-components.txt')
Path('Test/Tam/market95-foundation-dump.cs.txt').write_text(p,encoding='utf-8')
