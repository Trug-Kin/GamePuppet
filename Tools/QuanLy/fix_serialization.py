from pathlib import Path
p=Path('Tools/QuanLy/git_dependency_roots.cs.txt');s=p.read_text(encoding='utf-8')
s=s.replace('Newtonsoft.Json.JsonConvert.SerializeObject(new Report{roots=roots,paths=deps},Newtonsoft.Json.Formatting.Indented)','"{\\\"roots\\\":["+string.Join(",",roots.Select(Q))+"],\\\"paths\\\":["+string.Join(",",deps.Select(Q))+"]}"')
s=s.replace('internal class CommandScript:IRunCommand{','internal class CommandScript:IRunCommand{string Q(string s)=>"\\\""+s.Replace("\\\\","/").Replace("\\\"","\\\\\\\"")+"\\\"";')
p.write_text(s,encoding='utf-8')
p=Path('Tools/QuanLy/unity_inventory.cs.txt');s=p.read_text(encoding='utf-8').replace('Newtonsoft.Json.JsonConvert.SerializeObject(report,Newtonsoft.Json.Formatting.Indented)','JsonUtility.ToJson(report,true)')
s=s.replace('if(EditorApplication.isPlaying&&session)', 'File.WriteAllLines("Docs/QuanLy/unity-assets.tsv",new[]{"path\\tguid\\tr107\\tbuild\\tresource\\tspecial"}.Concat(report.assets.Select(a=>a.path+"\\t"+a.guid+"\\t"+a.r107+"\\t"+a.build+"\\t"+a.resource+"\\t"+a.special)));if(EditorApplication.isPlaying&&session)')
p.write_text(s,encoding='utf-8')
