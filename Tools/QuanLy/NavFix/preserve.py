"""Snapshot checkout changes and private profiles without overwrites or source moves."""
from pathlib import Path
import hashlib,json,subprocess,shutil,sys,datetime,uuid
C=Path('C:/ConRoi_CheckoutTest');R=Path(__file__).resolve().parents[3];label=sys.argv[1];assert label.replace('-','').isalnum()
def git(*a):return subprocess.check_output(['git','-c','safe.directory='+str(C),'-C',str(C),*a])
names={s[3:] for s in git('status','--porcelain','-z','--untracked-files=no').decode().split('\0') if s}
names.update(filter(None,git('ls-files','--others','--exclude-standard','-z','Assets').decode().split('\0')))
names.update(p.relative_to(C).as_posix() for p in (C/'Test/PlaytestProfiles').rglob('*') if p.is_file())
stamp=datetime.datetime.now(datetime.timezone.utc).strftime('%Y%m%d_%H%M%S')+'_'+uuid.uuid4().hex[:6]
S=C/'Test/Output'/('NavPreserved_'+label+'_'+stamp);S.mkdir();rows=[]
for name in sorted(names):
    p=C/name;assert p.resolve().is_relative_to(C.resolve()) and p.is_file() and not p.lstat().st_file_attributes&0x400
    h=hashlib.sha256(p.read_bytes()).hexdigest();q=S/'Files'/name;q.parent.mkdir(parents=True,exist_ok=True);shutil.copyfile(p,q)
    assert h==hashlib.sha256(q.read_bytes()).hexdigest()==hashlib.sha256(p.read_bytes()).hexdigest()
    rows.append(dict(path=name,snapshot=str(q),bytes=p.stat().st_size,sha256=h))
(S/'diff.patch').write_bytes(git('diff','--binary'))
report=dict(commit=git('rev-parse','HEAD').decode().strip(),snapshot=str(S),files=rows,source_moved=False)
(S/'manifest.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
out=R/'Docs/QuanLy/NavFix20261108'/('preserve-'+label+'.json');assert not out.exists();out.write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(dict(snapshot=str(S),files=len(rows),bytes=sum(x['bytes'] for x in rows))))
