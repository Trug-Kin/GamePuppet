from pathlib import Path
p=Path('Test/Tam/r107-comparison-views.cs.txt');s=p.read_text()
s=s.replace('var aa=a.sharedMesh;var bb=b.sharedMesh;','var g=root.transform.Find("Surface_142").GetComponent<MeshFilter>();var gg=g.sharedMesh;var aa=a.sharedMesh;var bb=b.sharedMesh;')
s=s.replace('int i=0;foreach(float yaw','g.sharedMesh=phase=="before"?AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>("Assets/Art/Environment/JourneyV48/TerrainR101/BankR91_Grade_017_IntegratedR101.asset"):gg;int i=0;foreach(float yaw')
s=s.replace('a.sharedMesh=aa;b.sharedMesh=bb;','a.sharedMesh=aa;b.sharedMesh=bb;g.sharedMesh=gg;')
s=s.replace('/map-','/map-final-').replace('comparison-views.txt','comparison-views-final.txt')
Path('Test/Tam/r107-comparison-final.cs.txt').write_text(s)
p=Path('Test/Tam/r107-path-plan.cs.txt');p.write_text(p.read_text().replace('new HashSet<int>','new List<int>'))
