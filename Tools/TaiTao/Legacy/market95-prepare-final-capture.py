from pathlib import Path
import re
# Capture only affected regressions; keep all earlier diagnostic footage.
s=Path('Test/Tam/market95-branch-motion.cs.txt').read_text().replace('branch-motion.txt','branch-final-motion.txt');Path('Test/Tam/market95-branch-final-motion.cs.txt').write_text(s)
s=Path('Test/Tam/market95-north-motion.cs.txt').read_text().replace('new Vector3(845,15,175)','new Vector3(845,15,85)').replace(',175),Quaternion.Euler',',85),Quaternion.Euler')
s=re.sub(r'Vector2\[\] path=\{.*?\};','Vector2[] path={new Vector2(845,120),new Vector2(845,140),new Vector2(845,175),new Vector2(845,140),new Vector2(845,120),new Vector2(845,85)};',s).replace('north-motion.txt','north-final-motion.txt').replace('t<105','t<70');Path('Test/Tam/market95-north-final-motion.cs.txt').write_text(s)
r=Path('Test/Tam/market95-record.py').read_text().replace("['--branch','--north','--north-return']","['--branch','--north','--north-return','--branch-final','--north-final']").replace("'145' if local=='branch'","'145' if local in ['branch','branch-final']").replace("else '110') if local","else '75' if local=='north-final' else '110') if local")
Path('Test/Tam/market95-record.py').write_text(r)
# Update reproducible integration recipe: CopySerialized must be followed by
# explicit upload; comparison screenshots demonstrate why this is necessary.
p=Path('Test/Tam/market95-winding-bind.cs.txt');s=p.read_text();s=s.replace('EditorUtility.SetDirty(mesh);','var uploadV=mesh.vertices;var uploadN=mesh.normals;var uploadT=Enumerable.Range(0,mesh.subMeshCount).Select(i=>mesh.GetTriangles(i)).ToArray();mesh.vertices=uploadV;mesh.normals=uploadN;for(int sub=0;sub<uploadT.Length;sub++)mesh.SetTriangles(uploadT[sub],sub);mesh.RecalculateBounds();mesh.UploadMeshData(false);EditorUtility.SetDirty(mesh);');p.write_text(s)
