from pathlib import Path
import json,hashlib
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r106'
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
protected=json.loads((D/'protected-start.json').read_text(encoding='utf-8-sig'))
rows=[]
for v in protected:
 p=Path(v['path']);after=sha(p) if p.is_file() else None
 rows.append(dict(path=v['path'],before=v['sha256'],after=after,unchanged=after==v['sha256']))
saves=[r for r in rows if 'AppData' in r['path'] and r['path'].endswith('.json')]
report=dict(enumerated=len(rows),unchanged=sum(r['unchanged'] for r in rows),changed=[r for r in rows if not r['unchanged']],tracked_original_saves=len(saves),tracked_original_saves_unchanged=sum(r['unchanged'] for r in saves),scope='Only the enumerated protected files and seventeen tracked original saves; not every file/save on machine.',files=rows)
(D/'protected-final.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
paths=[R/'Assets/Scenes/JourneyPresentation_v48_r106.unity',R/'Assets/Scenes/JourneyPresentation_v48_r106.unity.meta']
for folder in ['Source/Blender/JourneyV48/R105','Assets/Art/InvestigationV36/R105','Assets/Scripts/InvestigationV36R106','Source/Downloads/Quaternius_R104']:
 paths.extend(p for p in (R/folder).rglob('*') if p.is_file() and not p.name.endswith('.blend1'))
 meta=Path(str(R/folder)+'.meta')
 if meta.is_file():paths.append(meta)
paths.extend(R/p for p in ['BAT_DAU_TAI_DAY.md','Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md'])
(D/'delivery-manifest.json').write_text(json.dumps([dict(path=str(p.relative_to(R)),sha256=sha(p),bytes=p.stat().st_size) for p in paths],ensure_ascii=False,indent=2),encoding='utf-8')
evidence=[p for p in (R/'Test/BangChung/investigation_r106').rglob('*') if p.is_file()]
(D/'evidence-manifest.json').write_text(json.dumps([dict(path=str(p.relative_to(R)),sha256=sha(p),bytes=p.stat().st_size) for p in evidence],ensure_ascii=False,indent=2),encoding='utf-8')
results=[p for p in D.rglob('*') if p.is_file() and not p.name.endswith('manifest.json')]
(D/'results-manifest.json').write_text(json.dumps([dict(path=str(p.relative_to(R)),sha256=sha(p),bytes=p.stat().st_size) for p in results],ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k!='files'},ensure_ascii=False))
