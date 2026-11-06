from pathlib import Path
p=Path('Test/Tam/map100-views.cs.txt');s=p.read_text()
s=s.replace('new Vector3(683.53369f,3.12f,490.63019f),new Vector3(624.46228f,1.5f,535.79663f),new Vector3(693,2,507),new Vector3(643.5f,2,414)','new Vector3(687.4f,2.7f,486.5f),new Vector3(683.53369f,3.12f,490.63019f),new Vector3(683.53369f,3.12f,490.63019f),new Vector3(624.46228f,1.5f,535.79663f)')
s=s.replace('"gate-lamp","far-lamp","historic-693-507","historic-643-414"','"gate-bed","gate-overhead","gate-lamp-side","far-lamp-side"').replace('at[i]+new Vector3(-2,1.15f,-2)','at[i]+(i==1?new Vector3(0,12,-.1f):new Vector3(2,1.15f,2))').replace('actual-layer-rays.txt','bed-layer-rays.txt')
Path('Test/Tam/map100-bed-views.cs.txt').write_text(s)
