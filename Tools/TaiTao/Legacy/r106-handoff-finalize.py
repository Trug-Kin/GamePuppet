import json, hashlib, re
from pathlib import Path
root=Path('D:/ConRoi')
base=root/'Test/KetQua/investigation_r106'
out=base/'playtest_handoff'
def sha(p):
    h=hashlib.sha256()
    with p.open('rb') as f:
        for b in iter(lambda:f.read(1024*1024),b''): h.update(b)
    return h.hexdigest()
before=json.loads((out/'protected-start.json').read_text(encoding='utf-8-sig'))
rows=[]
for item in before:
    p=Path(item['path'])
    actual=sha(p) if p.is_file() else None
    rows.append({'path':str(p),'before':item['sha256'],'after':actual,'unchanged':actual==item['sha256']})
result={'scope':'Only enumerated files; not all project data or historical saves','total':len(rows),'unchanged':sum(r['unchanged'] for r in rows),'tracked_original_saves':sum('AppData' in r['path'] for r in rows),'files':rows}
(out/'protected-final.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
docs=[base/n for n in ['README.md','PLAYTEST_NGUOI_MOI.md','NGUOI_TO_CHUC.md','HUONG_DAN_NGUOI_CHOI.md','MAU_GHI_NHAN_PHIEN.md','MAU_TONG_HOP_VAN_DE.md']]
docs += [out/'README.md',out/'SESSIONS.md',root/'BAT_DAU_TAI_DAY.md',root/'Docs/PROJECT_GUIDE_AND_HISTORY.md',root/'Docs/BLENDER_UNITY_WORKFLOW.md']
broken=[]
for p in docs:
    s=p.read_text(encoding='utf-8-sig')
    assert '\ufffd' not in s,p
    for link in re.findall(r'\]\(([^)]+)\)',s):
        if '://' in link or link.startswith('#'): continue
        target=(p.parent/link.split('#')[0])
        if not target.exists(): broken.append({'file':str(p),'link':link})
files=docs+[out/'start-profile-check.txt',out/'editor-start-check.txt',out/'protected-start.json',out/'protected-final.json',root/'Test/BangChung/investigation_r106/playtest_handoff/start-screen.png',root/'Test/PlaytestProfiles/PT001_r106/InvestigationV36/r106/prototype.json']
manifest={'human_playtest':'NOT_RUN','product_changed':False,'evidence':'Synthetic F5/F6 empty-profile startup check only','excluded':'Live editor-startup.log; historical manifests unchanged','files':[{'path':str(p.relative_to(root)),'sha256':sha(p)} for p in files]}
(out/'delivery-manifest.json').write_text(json.dumps(manifest,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in result.items() if k!='files'}))
link_result={'handoff_links_missing':[b for b in broken if 'investigation_r106' in b['file']],'historical_common_document_links_missing':broken,'note':'Historical links retained; no product or archive repair in handoff scope'}
(out/'document-check.json').write_text(json.dumps(link_result,ensure_ascii=False,indent=2),encoding='utf-8')
print('Missing handoff links:',len(link_result['handoff_links_missing']))
