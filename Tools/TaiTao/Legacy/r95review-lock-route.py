from pathlib import Path
import re
D='Test/KetQua/journey_v48/r95_visual_review/'
p=Path('Test/Tam/r95review-fixture.cs.txt');s=p.read_text().replace('internal class CommandScript:IRunCommand{','internal class CommandScript:IRunCommand{[System.Serializable]public class Lighting{public Vector3 sunEuler;public float intensity;public bool fogClear,geometryClear,globalFog;}')
s=s.replace('var sun=RenderSettings.sun;sun.transform.rotation=', 'var initialSun=RenderSettings.sun;var initialFog=s.GetComponent<SuongKhongGianR22>();var initialGeometry=s.GetComponent<SuongDaLatV48>();File.WriteAllText(d+"original-lighting.json",JsonUtility.ToJson(new Lighting{sunEuler=initialSun.transform.eulerAngles,intensity=initialSun.intensity,fogClear=initialFog&&initialFog.clearReview,geometryClear=initialGeometry&&initialGeometry.clearGeometryReview,globalFog=RenderSettings.fog}));var sun=RenderSettings.sun;sun.transform.rotation=')
p.write_text(s,encoding='utf-8')
out=[(662,517),(675,505),(690,491),(703,479),(714,474),(729,468),(731,455),(730,438),(731,403),(740,370),(750,337),(741,306),(725,302),(737,300),(750,260),(766,226),(784,220),(805,211),(812,191),(804,171),(784,163),(761,173),(756,194),(766,216),(784,220)]
pts=out+[(766,226),(750,260),(737,300),(725,302),(741,306),(750,337),(740,370),(731,403),(730,438),(731,455),(729,468),(714,474),(703,479),(690,491),(675,505),(662,517),(650,528)]
p=Path('Test/Tam/r95review-route-motion.cs.txt');s=p.read_text();s=s.replace('new Vector3(714.94f,15,535)','new Vector3(650,15,528)').replace('s.Place(new Vector3(714.94f,','s.Place(new Vector3(650,').replace(',535),Quaternion.Euler',',528),Quaternion.Euler')
s=re.sub(r'Vector2\[\] path=\{.*?\};','Vector2[] path={'+','.join(f'new Vector2({x}f,{z}f)' for x,z in pts)+'};',s)
s=s.replace('internal class CommandScript:IRunCommand{','internal class CommandScript:IRunCommand{[System.Serializable]public class Lighting{public Vector3 sunEuler;public float intensity;public bool fogClear,geometryClear,globalFog;}')
s=s.replace('int leg=0;','bool gameplayLight=false;int leg=0;')
s=s.replace('leg++;hold=now+.35;', '''leg++;if(leg==25&&!gameplayLight){gameplayLight=true;var lighting=JsonUtility.FromJson<Lighting>(File.ReadAllText("'''+D+'''original-lighting.json"));RenderSettings.sun.transform.eulerAngles=lighting.sunEuler;RenderSettings.sun.intensity=lighting.intensity;RenderSettings.fog=lighting.globalFog;var fog=s.GetComponent<SuongKhongGianR22>();var geom=s.GetComponent<SuongDaLatV48>();if(fog)fog.clearReview=lighting.fogClear;if(geom)geom.clearGeometryReview=lighting.geometryClear;b.AppendLine("LIGHTING gameplay t="+(now-begin));}hold=now+.35;''')
p.write_text(s,encoding='utf-8')
Path('Test/KetQua/journey_v48/r95_visual_review/route-plan.json').write_text(__import__('json').dumps(dict(start=[650,528],waypoints=pts,lighting_switch_after_waypoint=24,story='copied valid market save; ground/structure review, not story progression'),indent=2))
