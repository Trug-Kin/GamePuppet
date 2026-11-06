"""Conservative classification and bounded, explicit move plan. No movement here."""
from pathlib import Path
import json,datetime,uuid,collections,re,hashlib
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy';rows=json.loads((D/'KiemKe/files.json').read_text(encoding='utf-8'))
batch='git-team-'+datetime.datetime.now().strftime('%Y%m%d-%H%M%S')+'-'+uuid.uuid4().hex[:6]
archive='D:/Game/ConRoi_LuuTru/'+batch
plan=[];classes=collections.defaultdict(lambda:dict(files=0,bytes=0));unclassified=[]
for x in rows:
 p=x['path'];ext=Path(p).suffix.lower();kind='UNCLASSIFIED'
 if p.startswith(('Assets/','Packages/','ProjectSettings/','Source/','.agents/','Tools/')):kind='KEEP_SOURCE_OR_COMPILE'
 elif p.startswith(('.build-cache/','.build-temp/','Logs/','UserSettings/','.tmp/')):kind='REGENERABLE_CACHE_HELD'
 elif p.startswith(('Test/BangChung/','Test/BanChoiThu/','Test/Output/','Test/PlaytestProfiles/')):kind='ARCHIVE_CANDIDATE'
 elif p.startswith('Test/KetQua/') and ext=='.md':kind='KEEP_QA_REPORT'
 elif p.startswith('Docs/') and ext=='.md':kind='KEEP_DOCUMENTATION'
 # Exact known technical capture folders; no blanket video rule.
 if p.startswith('Test/BangChung/investigation_r10') or p.startswith('Test/PlaytestProfiles/') or p.startswith('Test/Output/'):
  if sum(e['bytes'] for e in plan if e['action']=='ARCHIVE')+x['bytes']<450*1024**2:
   plan.append(dict(old=p,new=archive+'/DuLieu/'+p,action='ARCHIVE',reason='Known technical capture/profile/output outside Unity Assets; source scripts held',bytes=x['bytes']))
 if p.startswith('Test/Tam/') and (ext in ['.py','.ps1','.cs'] or p.endswith('.cs.txt')):
  plan.append(dict(old=p,new=str(R/'Tools/TaiTao/Legacy'/p[len('Test/Tam/'):]),action='TOOLS_MOVE',reason='Preserve reproducibility code before ignoring generated Test/Tam output; historical recipes need path review',bytes=x['bytes']))
 if kind=='UNCLASSIFIED':unclassified.append(x)
 classes[kind]['files']+=1;classes[kind]['bytes']+=x['bytes']
out=dict(batch=batch,archive=archive,root=str(R),entries=plan,classes=dict(classes),unclassified=unclassified,big_batches_blocked=['Test/BangChung remaining ~43GB','Test/BanChoiThu ~15GB','Docs/Reviews ~29GB (mixed source and outputs: not bulk moved)','.build-cache ~12GB (junction target; held)'])
(D/'classification.json').write_text(json.dumps(out,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(dict(batch=batch,archive=archive,entries=len(plan),move_bytes=sum(x['bytes'] for x in plan),archive_bytes=sum(x['bytes'] for x in plan if x['action']=='ARCHIVE'),tools=sum(x['action']=='TOOLS_MOVE' for x in plan),classes=dict(classes)),ensure_ascii=False,indent=2))
