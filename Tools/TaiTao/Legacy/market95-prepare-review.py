from pathlib import Path
s=Path('Test/Tam/i2-unity-review.cs.txt').read_text().replace('i2_r93_complete','market_ground_r95').replace('"r93":"r94"','"r94":"r95"').replace('_BoundR94','_BoundR95').replace('I2_TemporaryComparisonCamera','Market95_TemporaryComparisonCamera')
a=s.index('string[] tags=');b=s.index('\nforeach(var state',a)
s=s[:a]+'''string[] tags={"whole","clock","Langfarm","plaza","north","east-branch","west-branch","split"};Vector3[] eyes={new Vector3(955,280,490),new Vector3(757,4,430),new Vector3(750,6.7f,315),new Vector3(798,7.8f,231),new Vector3(830,6.7f,165),new Vector3(720,5,400),new Vector3(780,5,400),new Vector3(780,7,265)};Vector3[] ats={new Vector3(750,3,300),new Vector3(742,2,408),new Vector3(724,5.04f,300),new Vector3(785,6,188),new Vector3(827,5.5f,125),new Vector3(715,3,370),new Vector3(778,3,370),new Vector3(790,5.04f,240)};
''' +s[b:]
# Find inactive collision-only scene nodes explicitly.
s=s.replace('var g=GameObject.Find(e[1]);','var g=Find(e[1]);')
s=s.replace('internal class CommandScript:IRunCommand{','''internal class CommandScript:IRunCommand{static GameObject Find(string path){var names=path.Split('/');var t=EditorSceneManager.GetActiveScene().GetRootGameObjects().Single(x=>x.name==names[0]).transform;for(int j=1;j<names.Length;j++)t=t.Find(names[j]);return t?t.gameObject:null;}''')
s=s.replace('||!mr||mr.sharedMaterials.Any(m=>!m)','||(mr&&mr.sharedMaterials.Any(m=>!m))').replace('" renderer="+mr.enabled','" renderer="+(mr&&mr.enabled)')
Path('Test/Tam/market95-unity-review.cs.txt').write_text(s,encoding='utf-8')
