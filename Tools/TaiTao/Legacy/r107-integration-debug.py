from pathlib import Path
p=Path('Test/Tam/r107-integrate-map.cs.txt');s=p.read_text()
s=s.replace('if(File.Exists(fbx))throw new System.Exception("Already integrated; inspect instead");File.Copy("Test/KetQua/investigation_r107/export/TerrainR107.fbx",fbx);','if(!File.Exists(fbx))File.Copy("Test/KetQua/investigation_r107/export/TerrainR107.fbx",fbx);')
s=s.replace('if(normal==Vector3.zero){unmatched++;continue;}','if(normal==Vector3.zero){float nearest=100;Vector3 near=Vector3.zero;foreach(var bucket in grid.Values)foreach(var item in bucket){float ds=(item.p-p).sqrMagnitude;if(ds<nearest){nearest=ds;near=item.p;}}throw new System.Exception("Unmatched triangle="+i/3+" at="+p.ToString("F5")+" nearest="+near.ToString("F5")+" delta="+Mathf.Sqrt(nearest));}')
p.write_text(s)
