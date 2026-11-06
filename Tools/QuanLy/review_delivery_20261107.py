"""Read-only delivery audit; no Unity launch, move, checkout or Git mutation."""
import os,re,json,hashlib,subprocess,ctypes,collections,shutil,datetime
from pathlib import Path
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy/Review20261107';D.mkdir(parents=True,exist_ok=True)
def git(*args):
 p=subprocess.run(['git','-c','safe.directory='+str(R),*args],cwd=R,capture_output=True)
 if p.returncode:raise RuntimeError(p.stderr.decode('utf-8',errors='replace')[:1500])
 return p.stdout.decode('utf-8',errors='replace')
def write(name,obj):
 p=D/name
 if p.exists():raise RuntimeError('Preserve existing audit: '+str(p))
 p.write_text(json.dumps(obj,ensure_ascii=False,indent=2),encoding='utf-8')
def walk(folder):
 for here,dirs,files in os.walk(folder,followlinks=False):
  dirs[:]=[n for n in dirs if not (os.lstat(Path(here)/n).st_file_attributes&0x400)]
  for name in files:
   p=Path(here)/name
   if not (os.lstat(p).st_file_attributes&0x400):yield p
tracked=set(filter(None,git('ls-files','-z').split('\0')));untracked=set(filter(None,git('ls-files','--others','--exclude-standard','-z').split('\0')))
write('git-start.json',dict(utc=datetime.datetime.now(datetime.timezone.utc).isoformat(),branch=git('branch','--show-current').strip(),head=git('rev-parse','HEAD').strip(),status=git('status','--porcelain=v1','--untracked-files=no'),tracked=len(tracked),untracked=sorted(untracked),remote_names=git('remote').splitlines(),branches=git('for-each-ref','--format=%(refname:short) %(objectname)','refs/heads').splitlines(),worktrees=git('worktree','list').splitlines()))
kernel=ctypes.windll.kernel32;drives=[]
for i in range(26):
 name=chr(65+i)+':/'
 if kernel.GetLogicalDrives()&(1<<i) and kernel.GetDriveTypeW(name)==3:
  u=shutil.disk_usage(name);drives.append(dict(path=name,type='FIXED_LOCAL',total=u.total,free=u.free))
sizes=collections.defaultdict(lambda:dict(files=0,bytes=0));reparse=[]
for p in R.iterdir():
 st=os.lstat(p)
 if st.st_file_attributes&0x400:reparse.append(dict(path=p.name,target=os.readlink(p),followed=False));continue
 if p.name=='.git':continue
 if p.is_file():continue
 for f in walk(p):
  rel=f.relative_to(R).as_posix();key='/'.join(rel.split('/')[:2]);sizes[key]['files']+=1;sizes[key]['bytes']+=f.stat().st_size
write('disk-inventory.json',dict(drives=drives,reparse_points=reparse,groups=dict(sorted(sizes.items(),key=lambda x:-x[1]['bytes']))))
# Materialized Git tree, not pointer sizes. Existing library is a conservative import budget only.
lfs=json.loads(git('lfs','ls-files','--json'))['files'];lfs_by_path={x['name']:x for x in lfs}
tree_bytes=sum(lfs_by_path[n]['size'] if n in lfs_by_path else (R/n).stat().st_size for n in tracked if (R/n).is_file())
object_bytes=sum({x['oid']:x['size'] for x in lfs}.values())
library_bytes=sizes.get('.build-cache/r92-unity-library',{}).get('bytes',0)
git_metadata_bytes=sum(p.stat().st_size for p in walk(R/'.git/objects'))
reserve=4*1024**3;need=tree_bytes+object_bytes+library_bytes+git_metadata_bytes+reserve
write('capacity.json',dict(materialized_checkout_bytes=tree_bytes,independent_lfs_object_bytes=object_bytes,git_object_bytes=git_metadata_bytes,library_budget_bytes=library_bytes,library_budget_basis='Existing original Library cache measured without following Library junction; conservative estimate, new import not run',reserve_bytes=reserve,total_required_bytes=need,drives=[dict(**x,shortfall_bytes=max(0,need-x['free']),sufficient=x['free']>=need) for x in drives],checkout_status='BLOCKED_CAPACITY',unity_launched=False,external_cloud_or_removable_used=False,moving_to_D_Game_increases_D_free=False))
# Build GUID graph from serialized Unity assets, including untracked legacy. No extension implies unused.
all_assets=[p for p in walk(R/'Assets')];files={p.relative_to(R).as_posix():p for p in all_assets};guidmap={};duplicate_guids=[]
for name,p in files.items():
 if name.endswith('.meta'):
  m=re.search(rb'^guid: ([a-f0-9]{32})',p.read_bytes(),re.M)
  if m:
   g=m[1].decode();asset=name[:-5]
   if g in guidmap and guidmap[g]!=asset:duplicate_guids.append(dict(guid=g,paths=[guidmap[g],asset]))
   guidmap[g]=asset
edges={};guid_re=re.compile(rb'guid:\s*([a-f0-9]{32})')
for name,p in files.items():
 if name.endswith('.meta'):continue
 with p.open('rb') as f:
  head=f.read(64)
  if not(head.startswith(b'%YAML') or head.lstrip().startswith(b'{')):continue
  f.seek(0);refs=set();tail=b''
  while True:
   block=f.read(1024**2)
   if not block:break
   data=tail+block;refs.update(m[1].decode() for m in guid_re.finditer(data));tail=data[-100:]
  edges[name]=[guidmap[g] for g in refs if g in guidmap]
# Review compiled files and dynamic/path consumers: hold full literal folders conservatively.
compiled=[n for n in files if n.endswith(('.cs','.asmdef','.asmref','.rsp'))];strings=[];special=[];tool_types={};path_roots={};resource_keys=[];runtime_external=[]
for name in compiled:
 p=files[name]
 try:s=p.read_text(encoding='utf-8-sig')
 except UnicodeError:continue
 flags=[flag for flag,pat in [('Resources',r'Resources\.Load'),('StreamingAssets',r'StreamingAssets'),('Addressables',r'Addressables'),('reflection',r'Type\.GetType|Assembly\.Load|GetComponent\("'),('Define',r'^\s*#if|DefineConstraints'),('AssetDatabase',r'AssetDatabase\.') ] if re.search(pat,s,re.M)]
 if flags:special.append(dict(path=name,flags=flags))
 for m in re.finditer(r'"(Assets[/\\][^"\r\n]*)"',s):
  value=m[1].replace('\\\\','/').replace('\\','/').rstrip('/')
  if (R/value).exists():path_roots.setdefault(value,[]).append(name)
 for m in re.finditer(r'Resources\.Load(?:<[^>]+>)?\(\s*"([^"\r\n]+)"',s):resource_keys.append(dict(path=name,key=m[1]))
 if 'D:/Game' in s or 'D:\\Game' in s:runtime_external.append(dict(path=name,editor_or_test='/Editor/' in name or '/KiemThu' in name))
roots={n:'Already tracked baseline Asset/compile/dependency' for n in tracked if n.startswith('Assets/') and n in files and not n.endswith('.meta')}
for name in compiled:roots[name]='Compiled source/asmdef/define; never discard by scene dependency'
for name in files:
 if '/Resources/' in name or '/StreamingAssets/' in name or 'Addressable' in name:roots[name]='Special runtime load/config path retained'
for prefix,consumers in path_roots.items():
 for name in files:
  if name==prefix or name.startswith(prefix+'/'):roots.setdefault(name,'Literal asset path/folder used by compiled source: '+consumers[0])
queue=list(roots);reasons=dict(roots)
while queue:
 name=queue.pop()
 for target in edges.get(name,[]):
  if target not in reasons:reasons[target]='Serialized GUID dependency of '+name;queue.append(target)
for name in list(reasons):
 if name+'.meta' in files:reasons.setdefault(name+'.meta','GUID/meta pair of '+name)
 parent=Path(name).parent.as_posix()
 while parent not in ['Assets','.']:
  if parent+'.meta' in files:reasons.setdefault(parent+'.meta','Folder meta for retained child')
  parent=Path(parent).parent.as_posix()
legacy=[]
for name in sorted(n for n in untracked if n.startswith('Assets/') and n in files):
 category='A' if name in reasons else 'D'
 legacy.append(dict(path=name,bytes=files[name].stat().st_size,category=category,reason=reasons.get(name,'No verified current/static consumer; absence from closure alone is not proof unused. Preserve pending source/checkpoint/recipe review.'),action='HOLD_IN_PLACE',tracked=False))
summ=collections.defaultdict(lambda:dict(files=0,bytes=0))
for x in legacy:summ[x['category']]['files']+=1;summ[x['category']]['bytes']+=x['bytes']
write('legacy-classification.json',dict(scope='All current untracked Assets; static conservative GUID+compiled literal paths, no new Unity dependency query because no checkout capacity',groups=dict(summ),entries=legacy,duplicate_guids=duplicate_guids))
write('dynamic-consumers.json',dict(compiled_files=len(compiled),flags=special,literal_asset_roots=path_roots,resources_literal_keys=resource_keys,absolute_D_Game_consumers=runtime_external,policy='No asset moved; historical Editor path consumers require retention or explicit deprecation decision, not scene-only pruning'))
# All 852 moved sources: verify original move hash and whether types are still in compilation.
manifest=[json.loads(x) for x in (R/'Docs/QuanLy/archive-manifest.jsonl').read_text(encoding='utf-8-sig').splitlines()]
tools=[x for x in manifest if x['action']=='TOOLS_MOVE'];toolrows=[];asset_cs_text={n:files[n].read_text(encoding='utf-8-sig',errors='replace') for n in compiled if n.endswith('.cs')}
for x in tools:
 p=Path(x['new']);actual=hashlib.sha256(p.read_bytes()).hexdigest() if p.is_file() else None
 text=p.read_text(encoding='utf-8-sig',errors='replace') if p.is_file() else ''
 types=set(re.findall(r'\b(?:class|struct|interface|enum)\s+(\w+)',text))
 consumers=[]
 for t in types-{'CommandScript','Report','Entry','Plan','Row','Sample','Event','State'}:
  hits=[n for n,s in asset_cs_text.items() if re.search(r'\b'+re.escape(t)+r'\b',s)]
  if hits:consumers.append(dict(type=t,compiled_consumers=hits,definition_in_Assets=any(re.search(r'\b(?:class|struct|interface|enum)\s+'+re.escape(t)+r'\b',s) for s in asset_cs_text.values())))
 toolrows.append(dict(old=x['old'],path=p.relative_to(R).as_posix(),hash_unchanged=bool(actual and actual.lower()==x['sha256'].lower()),types_referenced_from_compiled_source=consumers,status='REVIEW_REFERENCE' if any(not h['definition_in_Assets'] for h in consumers) else 'SOURCE_HELD_NOT_COMPILED'))
write('moved-tools-audit.json',dict(files=len(toolrows),hashes_unchanged=sum(x['hash_unchanged'] for x in toolrows),entries=toolrows,limitation='Static identifier/definition scan; no compile-on-clean-checkout claim. CommandScript snippets remain outside Assets intentionally. All original Assets C# kept, no type removal performed.'))
print(json.dumps(dict(branch=git('branch','--show-current').strip(),head=git('rev-parse','HEAD').strip(),disks=drives,required_bytes=need,legacy_groups=dict(summ),moved_tools=len(toolrows),tools_hash_unchanged=sum(x['hash_unchanged'] for x in toolrows),unresolved_tool_type_rows=sum(x['status']=='REVIEW_REFERENCE' for x in toolrows)),ensure_ascii=False,indent=2))
