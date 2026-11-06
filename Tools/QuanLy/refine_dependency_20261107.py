"""Refine static classification; generic Assets folder scans are not required-asset proof."""
import json,re,os,subprocess,collections
from pathlib import Path
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy/Review20261107';P=D/'dependency-refined.json'
if P.exists():raise SystemExit('Keep prior refined report')
previous=json.loads((D/'dynamic-consumers.json').read_text(encoding='utf-8'))
start=json.loads((D/'git-start.json').read_text(encoding='utf-8'))
tracked=set(filter(None,subprocess.check_output(['git','-c','safe.directory='+str(R),'ls-files','-z'],cwd=R).decode('utf-8').split('\0')))
files={}
for here,dirs,names in os.walk(R/'Assets',followlinks=False):
 dirs[:]=[n for n in dirs if not os.lstat(Path(here)/n).st_file_attributes&0x400]
 for n in names:
  p=Path(here)/n
  if not os.lstat(p).st_file_attributes&0x400:files[p.relative_to(R).as_posix()]=p
guidmap={}
for n,p in files.items():
 if n.endswith('.meta'):
  m=re.search(rb'^guid: ([a-f0-9]{32})',p.read_bytes(),re.M)
  if m:guidmap[m[1].decode()]=n[:-5]
refs={};pattern=re.compile(rb'guid:\s*([a-f0-9]{32})')
for n,p in files.items():
 if n.endswith('.meta'):continue
 with p.open('rb') as f:
  h=f.read(64)
  if not(h.startswith(b'%YAML') or h.lstrip().startswith(b'{')):continue
  f.seek(0);g=set();tail=b''
  while True:
   b=f.read(1024**2)
   if not b:break
   data=tail+b;g.update(m[1].decode() for m in pattern.finditer(data));tail=data[-100:]
  refs[n]=[guidmap[x] for x in g if x in guidmap]
generic={'Assets','Assets/Art','Assets/Scenes','Assets/Model','Assets/Blender','Assets/Scripts'}
roots={n:'Tracked baseline dependency/compile asset' for n in tracked if n in files and not n.endswith('.meta')}
for n in files:
 if n.endswith(('.cs','.asmdef','.asmref','.rsp')):roots[n]='Participates in compilation/defines'
 if '/Resources/' in n or '/StreamingAssets/' in n or 'Addressable' in n:roots[n]='Special runtime loading/config folder'
path_consumers={}
for prefix,consumers in previous['literal_asset_roots'].items():
 if prefix in generic:continue
 for n in files:
  if n==prefix or n.startswith(prefix+'/'):
   roots.setdefault(n,'Compiled literal file/folder consumer: '+consumers[0]);path_consumers.setdefault(n,dict(prefix=prefix,consumer=consumers[0]))
def closure(roots):
 reasons=dict(roots);queue=list(roots)
 while queue:
  n=queue.pop()
  for dep in refs.get(n,[]):
   if dep not in reasons:reasons[dep]='Serialized GUID dependency of '+n;queue.append(dep)
 return reasons
needed=closure(roots)
entry='Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity';entry_closure=closure({entry:'Scene entry'})
for n in list(needed):
 if n+'.meta' in files:needed.setdefault(n+'.meta','Meta pair of '+n)
 parent=Path(n).parent.as_posix()
 while parent not in ['Assets','.']:
  if parent+'.meta' in files:needed.setdefault(parent+'.meta','Folder meta for retained child')
  parent=Path(parent).parent.as_posix()
entries=[];groups=collections.defaultdict(lambda:dict(files=0,bytes=0))
for n in sorted(x for x in start['untracked'] if x in files and x.startswith('Assets/')):
 category='A' if n in needed else 'D';row=dict(path=n,bytes=files[n].stat().st_size,category=category,reason=needed.get(n,'No verified specific consumer; broad folder scan cannot prove necessity or dispensability. Source/checkpoint provenance unresolved.'),git_status='UNTRACKED_HELD',action='KEEP_IN_PLACE_PENDING_CAPACITY_AND_REVIEW')
 entries.append(row);groups[category]['files']+=1;groups[category]['bytes']+=row['bytes']
missing_entry=[n for n in entry_closure if n in files and n not in tracked]
missing_compiled=[n for n in files if n.endswith(('.cs','.asmdef','.asmref','.rsp')) and n not in tracked]
P.write_text(json.dumps(dict(scope='Specific compiled literal roots + serialized GUID closure; source-only classification, not AssetDatabase importer/clean compile proof',supersedes='legacy-classification.json overconservatively treated generic Assets folder literal as requiring every asset; raw prior report retained',generic_roots_held_unresolved={n:v for n,v in previous['literal_asset_roots'].items() if n in generic},groups=dict(groups),entries=entries,serialized_entry_assets=len(entry_closure),serialized_entry_missing_from_git=missing_entry,compiled_sources_missing_from_git=missing_compiled,specific_consumers=path_consumers,source_b_policy='Entire Source tree and licenses already tracked; Assets editable not tied to a verified source recipe stays D, never archived based on suffix',no_assets_moved=True),ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps(dict(groups=dict(groups),entry_missing=missing_entry,compiled_missing=missing_compiled),ensure_ascii=False,indent=2))
