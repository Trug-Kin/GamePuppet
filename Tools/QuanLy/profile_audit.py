from pathlib import Path
import json,hashlib
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy';rows=json.loads((D/'KiemKe/files.json').read_text(encoding='utf-8'));out=[]
for row in rows:
 if not row['path'].startswith('Test/PlaytestProfiles/'):continue
 p=R/row['path'];st=p.stat() if p.exists() else None
 out.append(dict(path=row['path'],size_mtime_unchanged=bool(st and st.st_size==row['bytes'] and st.st_mtime_ns==row['mtime_ns']),sha256_now=hashlib.sha256(p.read_bytes()).hexdigest() if st else None))
(D/'profiles-final.json').write_text(json.dumps(dict(scope='Profiles inventoried before move; no pretask hash for these, compare size+mtime only. Newly created technical GitPreparation profile excluded. Original LocalLow saves not loaded/migrated or written by this task.',rows=out),ensure_ascii=False,indent=2),encoding='utf-8')
print('Original inventoried profile files',len(out),'unchanged size+mtime',sum(x['size_mtime_unchanged'] for x in out))
