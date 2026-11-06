from pathlib import Path
p=Path('Test/Tam/bank97-integrate.cs.txt');s=p.read_text();s=s.replace('r96.unity','r97.unity').replace('Clean r96 required','Clean r97 required')
s=s.replace('if(File.Exists("Assets/Scenes/JourneyPresentation_v48_r97.unity"))throw new System.Exception("Derivative already exists");','')
start=s.index('Directory.CreateDirectory(folder);');end=s.index('var log=',start)
s=s[:start]+'''File.Copy("Test/Output/journey_v48/BankR97/M95_022.fbx",folder+"M95_022.fbx",true);AssetDatabase.Refresh();
'''+s[end:]
s=s.replace('new GameObject("R97_Bridgehead_GateSide_Bank")','GameObject.Find("R97_Bridgehead_GateSide_Bank")')
s=s.replace('string key=e[0],path=e[1];','string key=e[0],path=e[1];if(key!="M95_022")continue;')
s=s.replace('PrefabUtility.SaveAsPrefabAssetAndConnect(root,folder+"BridgeheadBankR97.prefab",InteractionMode.AutomatedAction);','')
s=s.replace('dir+"integration.txt"','dir+"wateredge-integration.txt"')
Path('Test/Tam/bank97-wateredge-integrate.cs.txt').write_text(s)
