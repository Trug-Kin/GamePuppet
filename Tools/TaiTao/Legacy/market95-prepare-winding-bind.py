from pathlib import Path
s=Path('Test/Tam/market95-integrate.cs.txt').read_text()
a=s.index('public void Execute');h=s[:a]
body=s[s.index('foreach(var line in File.ReadAllLines'):s.index('var env=')]
body=body.replace('var e=line.Split(\'\\t\');','var e=line.Split(\'\\t\');if(e[0]!="M95_016"&&e[0]!="M95_023")continue;')
body=body.replace('var original=f.sharedMesh;var old=Readable(original);if(AssetDatabase.GetAssetPath(original)!=e[2])throw new System.Exception("Baseline changed: "+e[1]);','var original=AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(e[2]);var old=Readable(original);')
body=body.replace('AssetDatabase.CreateAsset(mesh,ap);','var target=AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(ap);EditorUtility.CopySerialized(mesh,target);Object.DestroyImmediate(mesh);mesh=target;EditorUtility.SetDirty(mesh);')
x=body.index('if(co&&co.enabled){');y=body.index('Physics.BakeMesh',x)
body=body[:x]+'if(co&&co.enabled){'+body[y:]
begin='''public void Execute(ExecutionResult result){if(EditorApplication.isPlaying||EditorApplication.isCompiling)throw new System.Exception("Edit required");var sc=EditorSceneManager.GetActiveScene();if(sc.path!="Assets/Scenes/JourneyPresentation_v48_r95.unity"||sc.isDirty)throw new System.Exception("Clean r95 required");string d="Test/KetQua/journey_v48/market_ground_r95/",folder="Assets/Art/Environment/JourneyV48/MarketGroundR95/";foreach(string key in new[]{"M95_016","M95_023"})File.Copy("Test/Output/journey_v48/MarketGroundR95/"+key+".fbx",folder+key+".fbx",true);AssetDatabase.Refresh();var log=new System.Text.StringBuilder();
'''
end='''var migration=Object.FindFirstObjectByType<KhoiPhucCaoDoSauTaiR73>(FindObjectsInactive.Include);migration.loadProbeAbove=4;EditorUtility.SetDirty(migration);AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(sc);EditorSceneManager.SaveScene(sc);File.WriteAllText(d+"winding-integration.txt",log.ToString()+"\\nloadProbeAbove=4; below=25; load only; controller unchanged");result.Log("Two Blender FBX winding updates bound retaining GUIDs; r95 load-only above span 4m saved");}}'''
Path('Test/Tam/market95-winding-bind.cs.txt').write_text(h+begin+body+end)
