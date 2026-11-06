"""Tach output kiem thu; plan/apply/verify, khong xoa file hoac thay source san pham."""
from pathlib import Path
import os,re,json,hashlib,zipfile,argparse,collections,time
R=Path(__file__).resolve().parents[1];H=R/'Docs/Organization/Test-20261102'
SRC_EXT={'.blend','.blend1','.fbx','.obj','.mtl','.asset','.meta','.cs','.py','.ps1','.cmd','.bat','.shader','.compute','.asmdef','.asmref','.sln','.csproj','.unity','.prefab','.exr','.hdr','.tga','.dds','.bin','.npz','.npy','.osm','.geojson','.glb','.gltf','.html','.patch','.svg'}
ADMIN={'checkpoint-before-restructure.zip','checkpoint.json','deleted-paths.json','delivered-zip-moves.json','directory-moves.json','git-status.json','handoff.json','input-tool-origin.json','outer-moves.json','protected-data-final.json','r92-source-mismatch.json','reorganized-script-hashes.json','root-inventory.json','root-move-plan.json','root-move-result.json','script-guide.json','script-moves.json','script-scene-bindings.json','tool-moves.json','unityhub-projects-before.json','user-saves-before.json','work-delta.json','work-source-hashes.json','work-unique-before-cleanup.zip'}
def files(root):
 for d,ds,fs in os.walk(root,followlinks=False):
  ds[:]=[n for n in ds if not os.path.islink(Path(d)/n) and n not in {'python-deps','__pycache__','.git'}]
  for n in fs:
   p=Path(d)/n
   if not p.is_symlink():yield p

def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
def dump(p,x):p.write_text(json.dumps(x,ensure_ascii=False,indent=2),encoding='utf-8')
def batch(n):
 if 'Reorganized' in n:return 'journey_v48/restructure-20261102'
 m=re.search(r'ConRoi-r(90|91|92)',n)
 if m:return 'journey_v48/'+('r90_windows_playtest' if m[1]=='90' else 'r'+m[1])
 m=re.search(r'(?:v48[-_]r|Windows_v48_r)(\d+)',n)
 if m:return 'journey_v48/r'+m[1]
 m=re.search(r'v(\d+)',n)
 return 'journey_v'+m[1] if m else 'BuildsKhac'
def plan():
 H.mkdir(parents=True,exist_ok=True);assert not (H/'moves.jsonl').exists(),'Ke hoach da co; khong ghi de'
 code=[];input_names=set();literal_paths=set();script_count=0
 for top in ['Assets','Tools','Docs/Archives','Docs/Reviews','.vscode','.agents']:
  for p in files(R/top):
   if p.suffix.lower() not in {'.cs','.py','.ps1','.cmd','.bat','.json','.yml','.yaml'} or p.stat().st_size>2_000_000:continue
   if p.suffix.lower()=='.json' and '.vscode' not in p.parts and '.agents' not in p.parts:continue
   try:s=p.read_text(encoding='utf-8-sig')
   except (UnicodeError,OSError):continue
   script_count+=1
   for token in re.findall(r'''["']([^"'\r\n]+\.(?:json|csv|txt|png|jpg|jpeg|zip|blend|fbx))["']''',s,re.I):
    name=token.replace('\\\\','/').replace('\\','/').split('/')[-1]
    if not re.fullmatch(r'(?:frame[-_ ]*)?\d+\.(jpg|png)',name,re.I):input_names.add(name.lower())
   for token in re.findall(r'''["']((?:Docs[/\\]Reviews|Builds)[^"'\r\n]*)["']''',s):literal_paths.add(token.replace('\\\\','/').replace('\\','/'))
   if ('Docs/Reviews' in s or 'Builds/' in s) and top not in ['Docs/Archives','Docs/Reviews']:code.append({'path':p.relative_to(R).as_posix(),'references':len(re.findall('Docs/Reviews|Builds/',s))})
 dump(H/'consumer-audit.json',{'scanned_scripts_configs':script_count,'input_or_output_literal_names_conservatively_held':sorted(input_names),'literal_paths':sorted(literal_paths),'current_consumers':code})
 candidates=[];held=[];allreviews=list(files(R/'Docs/Reviews'));player_roots=[]
 for p in allreviews:
  if p.name=='ConRoi.exe' and (p.parent/'ConRoi_Data').is_dir():player_roots.append(p.parent)
 sessions=set()
 for p in allreviews:
  if p.name.lower() in {'player.log','actions.jsonl','frames.csv','frame-times.csv','experience-events.jsonl','movement.csv','telemetry.csv'}:sessions.add(p.parent)
 def retain(p,why):held.append({'path':p.relative_to(R).as_posix(),'reason':why,'bytes':p.stat().st_size})
 def move(p,group,rel,why):candidates.append({'old':p.relative_to(R).as_posix(),'new':'Test/'+group+'/'+rel,'reason':why,'bytes':p.stat().st_size,'mtime_ns':p.stat().st_mtime_ns})
 for p in files(R/'Builds'):
  rel=p.relative_to(R/'Builds');top=rel.parts[0]
  if p.name=='README.md' and len(rel.parts)==1:retain(p,'Tài liệu chỉ mục hiện hành');continue
  if top=='Verification':dest='journey_v48/restructure-20261102/'+rel.as_posix()
  else:dest=batch(top)+'/'+rel.as_posix()
  move(p,'BangChung' if p.suffix.lower() in {'.mp4','.webm','.avi','.mov'} else 'BanChoiThu',dest,'ZIP/player đã bàn giao hoặc video gameplay đã ghi; không phải nguồn sản phẩm')
 for p in allreviews:
  rel=p.relative_to(R/'Docs/Reviews');q=rel.as_posix();low=q.lower();ext=p.suffix.lower();parts=[x.lower() for x in rel.parts]
  root=next((a for a in player_roots if p.is_relative_to(a)),None)
  if root:move(p,'BanChoiThu',q,'Bản player giải nén xác minh bằng ConRoi.exe + ConRoi_Data');continue
  if 'checkpoint' in low or (rel.parts[0]=='journey_v48' and 'restructure-20261102' in parts and p.name in ADMIN):retain(p,'Checkpoint/bảng hoàn nguyên/tài liệu nguồn; giữ ngoài Test');continue
  if ext in SRC_EXT or ext=='.md':retain(p,'Nguồn/fixture/công cụ/tài liệu phát triển hoặc chưa đủ bằng chứng tách');continue
  if ext=='.zip':
   if p.name=='ConRoi-v48-r4-binary-only.zip':move(p,'BanChoiThu',q,'ZIP chỉ chứa binary chơi thử')
   elif p.name=='relocated-build-crash-evidence.zip':move(p,'KetQua',q,'Archive crash/log, không phải checkpoint nguồn')
   else:retain(p,'Archive nguồn/fixture/checkpoint hoặc chưa chứng minh chỉ binary')
   continue
  if any(x in {'sources','source','references','reference','textures','texture','inputs','input','mapping','config','recipes'} for x in parts) or re.search(r'(?:source|reference|texture|albedo|roughness|normalmap|basecolor|tuoitre|download|satellite|osm)',p.name,re.I):retain(p,'Ảnh/dữ liệu nguồn dựng model hoặc đầu vào chưa phân tách');continue
  numeric=bool(re.fullmatch(r'(?:frame[-_ ]*)?\d+\.(?:png|jpg|jpeg)',p.name,re.I))
  framepath=any(x in {'frames','recording','film'} or x.endswith('-frames') for x in parts)
  session=p.parent in sessions or any(a in sessions for a in p.parents if a!=R)
  if ext in {'.mp4','.webm','.avi','.mov','.f32','.ffconcat'}:move(p,'BangChung',q,'Video/capture/audio thô/danh sách ghép bằng chứng; giữ nguyên byte');continue
  if ext in {'.png','.jpg','.jpeg'}:
   if p.name.lower() in input_names and not (numeric and framepath):retain(p,'Tên ảnh có trong script/tool: giữ thận trọng như đầu vào')
   elif framepath or session or re.search(r'(?:comparison|before.after|gameview|sceneview|screenshot|verified-|turnfixed-|winding-|final-editor|final-inspector|phase-\d+)',p.name,re.I):move(p,'BangChung',q,'Frame/capture/ảnh so sánh xác định từ phiên chạy hoặc tên bằng chứng và producer')
   else:retain(p,'Ảnh hỗn hợp chưa chứng minh không phải nguồn/reference')
   continue
  if p.name.lower() in input_names:retain(p,'Tên file được code/tool nhắc tới; giữ đầu vào và consumer lịch sử thận trọng');continue
  if ext in {'.log','.jsonl','.dmp','.wer'}:move(p,'KetQua',q,'Log/telemetry/crash của lượt kiểm, không có literal consumer bắt buộc tìm thấy');continue
  if ext in {'.json','.csv','.txt','.info'} and (session or re.search(r'(?:result|report|verification|audit|telemetry|metrics|timing|summary|build|package|comparison|fps|profile|smoke|launch|frame|inspect|observed)',p.name,re.I)):
   move(p,'KetQua',q,'Báo cáo/telemetry kiểm; nguồn/consumer literal được giữ riêng');continue
  retain(p,'Chưa rõ hoặc đầu vào phát triển; không chuyển dựa vào tên review/test')
 for p in files(R/'Logs'):
  if re.match(r'(?:AssetImport|shadercompiler|Editor(?:-prev|-UDS)?\.)',p.name,re.I):retain(p,'Log nội bộ Unity/cache, để engine quản lý')
  elif p.suffix.lower() in {'.log','.json','.jsonl','.txt'}:move(p,'KetQua','UnityLogs/'+p.name,'Log kiểm/build theo đợt, không là cache hoạt động')
 for d in (R/'.build-temp').iterdir():
  if d.is_dir() and not d.is_symlink():
   for p in files(d):move(p,'Tam','UnityTemp/'+p.relative_to(R/'.build-temp').as_posix(),'TEMP biên dịch/kiểm Unity đã đóng; dữ liệu có thể tạo lại')
 # Báo cáo audit mới của đợt này, không phải dữ liệu gốc.
 p=R/'.tmp/test-path-consumers-scan.txt'
 if p.exists():move(p,'KetQua','to-chuc-test-20261102/test-path-consumers-scan.txt','Quét consumer đợt hiện tại')
 assert len({x['new'].lower() for x in candidates})==len(candidates),'Trùng đích'
 for e in candidates:assert not (R/e['new']).exists(),e['new']
 with (H/'plan.jsonl').open('w',encoding='utf-8') as f:
  for e in candidates:f.write(json.dumps(e,ensure_ascii=False)+'\n')
 with (H/'retained.jsonl').open('w',encoding='utf-8') as f:
  for e in held:f.write(json.dumps(e,ensure_ascii=False)+'\n')
 dump(H/'plan-summary.json',{'move_files':len(candidates),'move_bytes':sum(e['bytes'] for e in candidates),'groups':dict(collections.Counter(e['new'].split('/')[1] for e in candidates)),'retained_files':len(held),'retained_reasons':dict(collections.Counter(e['reason'] for e in held)),'player_roots':[p.relative_to(R).as_posix() for p in player_roots],'deletions':0})
 print(json.dumps(json.loads((H/'plan-summary.json').read_text(encoding='utf-8')),ensure_ascii=True),flush=True)
def apply():
 from concurrent.futures import ThreadPoolExecutor
 assert not (H/'moves.jsonl').exists(),'Không áp lại; xem journal nếu bị ngắt'
 for n in ['BanChoiThu','BangChung','KetQua','Tam']:(R/'Test'/n).mkdir(parents=True,exist_ok=True)
 rows=[json.loads(l) for l in (H/'plan.jsonl').read_text(encoding='utf-8').splitlines()];began=time.monotonic();made=set()
 with (H/'moves.jsonl').open('x',encoding='utf-8') as f, ThreadPoolExecutor(max_workers=8) as pool:
  for offset in range(0,len(rows),512):
   chunk=rows[offset:offset+512]
   for e in chunk:
    a=R/e['old'];b=R/e['new'];st=a.stat();assert st.st_size==e['bytes'] and st.st_mtime_ns==e['mtime_ns'],e['old'];assert not b.exists()
   before=list(pool.map(sha,[R/e['old'] for e in chunk]))
   for e in chunk:
    a=R/e['old'];b=R/e['new']
    if b.parent not in made:b.parent.mkdir(parents=True,exist_ok=True);made.add(b.parent)
    a.rename(b)
   after=list(pool.map(sha,[R/e['new'] for e in chunk]))
   for e,h1,h2 in zip(chunk,before,after):
    e['sha256_before']=h1;e['sha256_after']=h2;f.write(json.dumps(e,ensure_ascii=False)+'\n');assert h1==h2,e['new']
   f.flush()
   if offset%4096==0:print('Moved/hash',offset+len(chunk),'/',len(rows),'seconds',round(time.monotonic()-began),flush=True)
 dump(H/'move-result.json',{'files':len(rows),'bytes':sum(x['bytes'] for x in rows),'hash_mismatches':[],'method':'rename cùng volume; SHA256 trước/sau từng file; không xóa, không copy payload','seconds':round(time.monotonic()-began,2)})
 print('DONE',len(rows),flush=True)
def verify():
 from concurrent.futures import ThreadPoolExecutor
 from DuongDanKiem import goc_test
 def payload(n):return goc_test()/n[5:] if n.startswith('Test/') else R/n
 rows=[json.loads(l) for l in (H/'moves.jsonl').read_text(encoding='utf-8').splitlines()]
 engine={'Logs/Editor-UDS-prev.log','Logs/relay.txt','Logs/trace-config.json','Logs/traces.jsonl','Logs/UDS-Service.log','Logs/upm.log'}
 def check(e):
  p=payload(e['new'])
  return e['old'] if not p.is_file() or sha(p)!=e['sha256_before'] or (payload(e['old']).is_file() and e['old'] not in engine) else None
 with ThreadPoolExecutor(max_workers=8) as pool:bad=[x for x in pool.map(check,rows) if x]
 regenerated=[{'path':n,'bytes':(R/n).stat().st_size,'mtime_ns':(R/n).stat().st_mtime_ns,'role':'Unity engine recreated internal log/config; original evidence hash retained in Test'} for n in sorted(engine) if (R/n).is_file()]
 from DuongDanKiem import ket_qua,phien
 out=ket_qua(phien('kiem-hash-to-chuc'));out.mkdir(parents=True,exist_ok=False)
 dump(out/'verify-final.json',{'files':len(rows),'mismatches':bad,'regenerated_engine_paths':regenerated});print(len(rows),'verified',len(bad),'mismatches','report',str(out),flush=True);assert not bad
if __name__=='__main__':
 p=argparse.ArgumentParser();p.add_argument('mode',choices=['plan','apply','verify']);a=p.parse_args();globals()[a.mode]()
