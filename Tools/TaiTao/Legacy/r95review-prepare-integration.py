import json
from pathlib import Path
D=Path('Test/KetQua/journey_v48/r95_visual_review')
keys=[x['key'] for x in json.loads((D/'winding-source-fix.json').read_text())['fixes']]
s=Path('Test/Tam/market95-winding-bind.cs.txt').read_text()
s=s.replace('new[]{"M95_016","M95_023"}','new[]{'+','.join('"'+k+'"' for k in keys)+'}')
s=s.replace('if(e[0]!="M95_016"&&e[0]!="M95_023")continue;','if(!new[]{'+','.join('"'+k+'"' for k in keys)+'}.Contains(e[0]))continue;')
s=s.replace('string d="Test/KetQua/journey_v48/market_ground_r95/",folder="Assets/Art/Environment/JourneyV48/MarketGroundR95/";', 'EditorSceneManager.SaveScene(sc,"Assets/Scenes/JourneyPresentation_v48_r96.unity");string d="Test/KetQua/journey_v48/market_ground_r95/",folder="Assets/Art/Environment/JourneyV48/MarketGroundR96/";Directory.CreateDirectory(folder);')
s=s.replace('Test/Output/journey_v48/MarketGroundR95/','Test/Output/journey_v48/MarketGroundR96/')
a=s.index('var target=AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(ap);');b=s.index('var uploadV=mesh.vertices;',a)
s=s[:a]+'if(AssetDatabase.LoadAssetAtPath<UnityEngine.Mesh>(ap))throw new System.Exception("Do not overwrite derivative assets");AssetDatabase.CreateAsset(mesh,ap);'+s[b:]
a=s.index('var migration=Object.FindFirstObjectByType<KhoiPhucCaoDoSauTaiR73>');s=s[:a]+'''AssetDatabase.SaveAssets();EditorSceneManager.MarkSceneDirty(sc);EditorSceneManager.SaveScene(sc);File.WriteAllText("Test/KetQua/journey_v48/r95_visual_review/winding-integration.txt",log.ToString());result.Log("R96 derivative scene and 15 new mesh assets; original r95 scene/assets untouched; winding only, no controller/migration changes");}}'''
Path('Test/Tam/r95review-integrate.cs.txt').write_text(s)
# Preserve failed route. Use different outputs and robust Play-stop cleanup.
s=Path('Test/Tam/r95review-route-motion.cs.txt').read_text()
s=s.replace('route-progress.txt','route-fixed-progress.txt').replace('route-motion.txt','route-fixed-motion.txt')
s=s.replace('EditorApplication.CallbackFunction cb=null;cb=()=>{double t=', 'EditorApplication.CallbackFunction cb=null;cb=()=>{if(!EditorApplication.isPlaying){EditorApplication.update-=cb;InputSystem.onBeforeUpdate-=input;return;}double t=')
s=s.replace('b.AppendLine("END reached="+leg+"/"+path.Length+" seconds="+t);InputSystem.onBeforeUpdate-=input;', 'EditorApplication.update-=cb;b.AppendLine("END reached="+leg+"/"+path.Length+" seconds="+t);InputSystem.onBeforeUpdate-=input;')
Path('Test/Tam/r95review-route-fixed.cs.txt').write_text(s)
s=Path('Test/Tam/r95review-record.py').read_text().replace('r95review-route-motion.cs.txt','r95review-route-fixed.cs.txt').replace('route-response.json','route-fixed-response.json').replace('bridge-market-return-controller.mp4','bridge-market-return-fixed-controller.mp4').replace('route-motion.txt','route-fixed-motion.txt').replace('capture-live.txt','capture-fixed-live.txt').replace("data/'capture.json'","data/'capture-fixed.json'")
Path('Test/Tam/r95review-record-fixed.py').write_text(s)
print('prepared',len(keys),'new assets only')
