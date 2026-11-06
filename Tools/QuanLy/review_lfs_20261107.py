"""Verify real local LFS objects without moving corrupt/missing objects or smudging files."""
from pathlib import Path
import subprocess,json,hashlib
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy/Review20261107';D.mkdir(exist_ok=True)
p=D/'lfs-verification.json'
if p.exists():raise SystemExit('Keep existing report')
raw=subprocess.check_output(['git','-c','safe.directory='+str(R),'lfs','ls-files','--json'],cwd=R)
rows=json.loads(raw)['files'];unique={x['oid']:x['size'] for x in rows};results=[]
for oid,size in unique.items():
 path=R/'.git/lfs/objects'/oid[:2]/oid[2:4]/oid
 h=hashlib.sha256()
 if path.is_file():
  with path.open('rb') as f:
   for b in iter(lambda:f.read(256*1024),b''):h.update(b)
  ok=path.stat().st_size==size and h.hexdigest()==oid
 else:ok=False
 results.append(dict(oid=oid,bytes=size,exists=path.is_file(),hash_ok=ok))
working=[]
for x in rows:
 file=R/x['name'];pointer=False
 if file.is_file():
  with file.open('rb') as f:pointer=f.read(100).startswith(b'version https://git-lfs.github.com/spec/v1')
 working.append(dict(path=x['name'],exists=file.is_file(),is_pointer=pointer,size_matches=bool(file.is_file() and file.stat().st_size==x['size'])))
report=dict(scope='Existing repository only; not clean checkout',files=len(rows),unique_objects=len(results),bytes=sum(unique.values()),objects_ok=all(x['hash_ok'] for x in results),materialized_current_tree=all(x['exists'] and not x['is_pointer'] and x['size_matches'] for x in working),objects=results,working_tree=working,no_mutation=True)
p.write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k not in ['objects','working_tree']},ensure_ascii=False,indent=2))
