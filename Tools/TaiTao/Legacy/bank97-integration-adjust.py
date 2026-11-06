from pathlib import Path
p=Path('Test/Tam/bank97-make-integration.py');s=p.read_text(encoding='utf-8')
start=s.index('RaycastHit hit;if(!probe.Raycast')
end=s.index('var ts=previous.triangles;',start)
s=s[:start]+'''RaycastHit hit=default;bool found=false;float closest=.002f;foreach(var direction in new[]{Vector3.down,Vector3.up,Vector3.left,Vector3.right,Vector3.forward,Vector3.back}){RaycastHit candidate;if(probe.Raycast(new Ray(world-direction*.03f,direction),out candidate,.06f)){float dd=Vector3.Distance(candidate.point,world);if(dd<closest){hit=candidate;closest=dd;found=true;}}}if(!found)throw new System.Exception("Attribute projection missing "+key+" "+world.ToString("F5"));'''+s[end:]
old='AssetDatabase.CreateAsset(mesh,folder+mesh.name+".asset");mesh.UploadMeshData(false);'
new='''string ap=folder+mesh.name+".asset";var existing=AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(ap);if(existing){EditorUtility.CopySerialized(mesh,existing);Object.DestroyImmediate(mesh);mesh=existing;var upv=mesh.vertices;var upn=mesh.normals;mesh.vertices=upv;mesh.normals=upn;mesh.RecalculateBounds();EditorUtility.SetDirty(mesh);}else AssetDatabase.CreateAsset(mesh,ap);mesh.UploadMeshData(false);'''
assert old in s;s=s.replace(old,new);p.write_text(s,encoding='utf-8')
