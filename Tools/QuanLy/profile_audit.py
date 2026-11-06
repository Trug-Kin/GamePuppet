from pathlib import Path
import json,hashlib
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy';rows=json.loads((D/'KiemKe/files.json').read_text(encoding='utf-8'));out=[]
moved={x['old']:x for x in [json.loads(line) for line in (D/'archive-manifest.jsonl').read_text(encoding='utf-8-sig').splitlines()]}
for row in rows:
 if not row['path'].startswith('Test/PlaytestProfiles/'):continue
 p=R/row['path'];st=p.stat() if p.exists() else None
 if not st and row['path'] in moved:
  m=moved[row['path']];q=Path(m['new']);h=hashlib.sha256(q.read_bytes()).hexdigest() if q.exists() else None
  out.append(dict(path=row['path'],status='ARCHIVED_VERIFIED' if h and h.lower()==m['sha256'].lower() else 'FAIL',new=m['new'],sha256=h,original_hash_at_verified_move=m['sha256'],no_migration=True))
 else:out.append(dict(path=row['path'],status='HELD_LOCAL',size_mtime_unchanged=bool(st and st.st_size==row['bytes'] and st.st_mtime_ns==row['mtime_ns']),sha256_now=hashlib.sha256(p.read_bytes()).hexdigest() if st else None))
(D/'profiles-final.json').write_text(json.dumps(dict(scope='Profiles inventoried before move; no pretask hash for these, compare size+mtime only. Newly created technical GitPreparation profile excluded. Original LocalLow saves not loaded/migrated or written by this task.',rows=out),ensure_ascii=False,indent=2),encoding='utf-8')
print('Original inventoried profile files',len(out),'archived verified',sum(x['status']=='ARCHIVED_VERIFIED' for x in out))
