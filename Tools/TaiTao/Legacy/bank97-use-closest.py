from pathlib import Path
p=Path('Test/Tam/bank97-make-integration.py');s=p.read_text();s=s.replace("+helper+body,encoding='utf-8')","+helper+(R/'Test/Tam/bank97-map-closest.cs.part').read_text()+body,encoding='utf-8')")
s=s.replace('Vector3[] priorV=legacy?previous.vertices:null;','Vector3[] priorV=legacy?previous.vertices:null;Vector3[] priorWorld=legacy?priorV.Select(p=>f.transform.TransformPoint(p)).ToArray():null;int[] priorTriangles=legacy?previous.triangles:null;')
start=s.index('RaycastHit hit=default;bool found=false;');end=s.index('interpolated++;',start)+len('interpolated++;')
s=s[:start]+'''Vector3Int ix;Vector3 bary;float dist;if(!ClosestAttribute(world,priorWorld,priorTriangles,out ix,out bary,out dist))throw new System.Exception("Exact mesh attribute correspondence missing "+key+" "+world.ToString("F5"));srcIndices[i]=ix;weights[i]=bary;error=Mathf.Max(error,dist);interpolated++;'''+s[end:]
p.write_text(s,encoding='utf-8')
