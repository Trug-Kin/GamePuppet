from pathlib import Path
s=Path('Test/Tam/map100-bed-views.cs.txt').read_text()
s=s.replace('string dir="','var rr=GameObject.Find("StoryProps/R22_Detailed_Route_Flowers").GetComponentsInChildren<MeshRenderer>(true);var states=rr.Select(r=>r.enabled).ToArray();foreach(var r in rr)r.enabled=false;string dir="')
s=s.replace('"gate-bed","gate-overhead","gate-lamp-side","far-lamp-side"','"isolate-gate-bed","isolate-gate-overhead","isolate-gate-lamp-side","isolate-far-lamp-side"').replace('sun.intensity=power;','for(int j=0;j<rr.Length;j++)rr[j].enabled=states[j];sun.intensity=power;').replace('bed-layer-rays.txt','isolate-layer-rays.txt')
Path('Test/Tam/map100-isolate.cs.txt').write_text(s)
