"""Copy and hash current checkout changes before any controlled Play stop or Git update."""
import datetime,hashlib,json,os,shutil,subprocess,uuid
from pathlib import Path
R=Path(__file__).resolve().parents[2];T=Path('C:/ConRoi_CheckoutTest')
O=R/'Docs/QuanLy/CheckoutFinal20261107';O.mkdir(exist_ok=True)
def git(*args):return subprocess.check_output(['git','-c','safe.directory='+str(T),'-C',str(T),*args])
stamp=datetime.datetime.now(datetime.timezone.utc).strftime('%Y%m%d_%H%M%S')+'_'+uuid.uuid4().hex[:6]
S=T/'Test/Output/CleanCheckout'/('BeforeSync_'+stamp);S.mkdir()
raw=git('status','--porcelain','-z','--untracked-files=no').decode().split('\0')
names={x[3:] for x in raw if x}
names.update(filter(None,git('ls-files','--others','--exclude-standard','-z','Assets').decode().split('\0')))
P=T/'Test/PlaytestProfiles'
if P.exists():names.update(p.relative_to(T).as_posix() for p in P.rglob('*') if p.is_file())
rows=[];total=0
for n in sorted(names):
    src=T/n
    assert src.resolve().is_relative_to(T.resolve()) and not src.lstat().st_file_attributes&0x400
    assert src.is_file()
    total+=src.stat().st_size;assert total<128*1024**2,'Unexpected snapshot size; preserve manually'
    sha=hashlib.sha256(src.read_bytes()).hexdigest();dst=S/'Files'/n;dst.parent.mkdir(parents=True,exist_ok=True)
    shutil.copyfile(src,dst)
    assert hashlib.sha256(dst.read_bytes()).hexdigest()==sha==hashlib.sha256(src.read_bytes()).hexdigest(), 'Concurrent writer: '+n
    rows.append(dict(path=n,snapshot=str(dst),bytes=src.stat().st_size,sha256=sha))
(S/'changes.patch').write_bytes(git('diff','--binary'))
report=dict(utc=datetime.datetime.now(datetime.timezone.utc).isoformat(),checkout=str(T),commit=git('rev-parse','HEAD').decode().strip(),snapshot=str(S),bytes=total,files=rows,play_not_stopped=True,no_source_files_moved=True)
(S/'manifest.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
(O/'preservation-before-sync.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps(dict(snapshot=str(S),files=len(rows),bytes=total,commit=report['commit'])))
