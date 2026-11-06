from pathlib import Path
s=Path('Test/Tam/market95-west-render-proof.cs.txt').read_text()
s=s.replace('new[]{"unlit-double-sided"}','new[]{"actual-reopened"}').replace('mat.SetFloat("_Cull",0);r.sharedMaterials=old.Select(x=>mat).ToArray();','')
Path('Test/Tam/market95-west-render-reopened.cs.txt').write_text(s)
