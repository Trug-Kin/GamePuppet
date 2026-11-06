from pathlib import Path
s=Path('Test/Tam/market95-unity-review.cs.txt').read_text()
a=s.index('string[] tags=');b=s.index('foreach(var state',a)
s=s[:a]+'''string[] tags={"bridge","I2","gate","Langfarm","plaza"};Vector3[] eyes={new Vector3(650,2.84f,528),new Vector3(703,2.54f,479),new Vector3(731,2.5f,438),new Vector3(750,6.74f,306),new Vector3(784,6.74f,220)};Vector3[] ats={new Vector3(680,2.4f,500),new Vector3(714,1.8f,459),new Vector3(746,2.5f,422),new Vector3(724,5.8f,301),new Vector3(784,6.2f,188)};
''' +s[b:]
s=s[:s.index('var sc=EditorSceneManager.GetActiveScene();Physics.SyncTransforms();')]+'''File.WriteAllText(d+"comparison-camera-notes.txt","Same world eye/target/FOV58, 1280x720, daylight render, r94 before vs r95 after; fixed-eye diagnostic, not controller traversal.");result.Log("Five fixed-camera r94/r95 pairs captured");}}'''
s=s.replace('market_ground_r95/','r95_visual_review/')
Path('Test/Tam/r95review-comparison.cs.txt').write_text(s)
