from pathlib import Path
P=Path('Test/Tam');proof=(P/'terrain101-staticbatch-proof.cs.txt').read_text();methods=proof[proof.index('static string Key'):proof.index('public void Execute')]
s=(P/'terrain101-region-return.cs.txt').read_text();s=s.replace('using UnityEngine;','using UnityEngine;using UnityEngine.Rendering;using Unity.Collections;',1).replace('public void Execute',methods+'public void Execute',1)
s=s.replace('bool valid=path.EndsWith(e[1]+"_IntegratedR101.asset")&&(e[3]=="1"||f.sharedMesh==c.sharedMesh);','string shapeDetail="";bool sameShape=e[3]=="1"||f.sharedMesh==c.sharedMesh||BatchedSame(t,c.sharedMesh,out shapeDetail);bool valid=path.EndsWith(e[1]+"_IntegratedR101.asset")&&sameShape;log.AppendLine(shapeDetail);')
assert 'sameShape' in s
(P/'terrain101-region-return-final.cs.txt').write_text(s,encoding='utf8')
