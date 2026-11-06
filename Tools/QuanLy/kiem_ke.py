"""Read-only inventory. Never follows Windows reparse points. No credential content."""
import os,sys,json,re,collections,shutil,hashlib,datetime
from pathlib import Path
ROOT=Path(__file__).resolve().parents[2]; OUT=ROOT/'Docs/QuanLy/KiemKe';OUT.mkdir(parents=True,exist_ok=True)
rows=[];links=[];errors=[]
def walk(folder):
 try: entries=list(os.scandir(folder))
 except OSError as e:errors.append(dict(path=str(folder),error=type(e).__name__));return
 for e in entries:
  p=Path(e.path);rel=p.relative_to(ROOT).as_posix()
  try:
   st=e.stat(follow_symlinks=False)
   if e.is_symlink() or (getattr(st,'st_file_attributes',0)&0x400):
    links.append(dict(path=rel,target=os.readlink(p) if e.is_symlink() else 'REPARSE_POINT',followed=False));continue
   if e.is_dir(follow_symlinks=False):walk(p)
   else:
    row=dict(path=rel,bytes=st.st_size,mtime_ns=st.st_mtime_ns)
    if p.suffix=='.meta':
     m=re.search(r'^guid: ([a-f0-9]{32})',p.read_text(encoding='utf-8-sig',errors='replace'),re.M)
     if m:row['guid']=m[1]
    rows.append(row)
  except OSError as ex:errors.append(dict(path=rel,error=type(ex).__name__))
walk(ROOT)
groups=collections.defaultdict(lambda:dict(files=0,bytes=0))
for r in rows:g=groups[r['path'].split('/')[0]];g['files']+=1;g['bytes']+=r['bytes']
usage=shutil.disk_usage(ROOT)
summary=dict(utc=datetime.datetime.now(datetime.timezone.utc).isoformat(),root=str(ROOT),files=len(rows),bytes=sum(r['bytes'] for r in rows),disk=dict(total=usage.total,free=usage.free),groups=dict(sorted(groups.items(),key=lambda x:-x[1]['bytes'])),reparse_points=links,errors=errors,large_files=sorted([r for r in rows if r['bytes']>50*1024**2],key=lambda r:-r['bytes']))
(OUT/'files.json').write_text(json.dumps(rows,ensure_ascii=False,indent=2),encoding='utf-8')
(OUT/'summary.json').write_text(json.dumps(summary,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in summary.items() if k not in ['large_files']},ensure_ascii=False,indent=2))
