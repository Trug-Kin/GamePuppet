from pathlib import Path
p=Path('Test/Tam/r107-integrate-map.cs.txt');s=p.read_text()
# Unity Vector3.normalized zeros vectors shorter than 1e-5; rescale cross products
# before normalization so existing small but nonzero terrain triangles can match.
s=s.replace('Vector3.Cross(c-a,d-a).normalized','(Vector3.Cross(c-a,d-a)*1000000f).normalized')
p.write_text(s)
