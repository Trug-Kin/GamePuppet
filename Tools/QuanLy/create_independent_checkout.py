"""Create a Git-only checkout and independently verified LFS store, never copy source extras."""
import hashlib,json,os,shutil,subprocess
from pathlib import Path
R=Path(__file__).resolve().parents[2]; T=Path('C:/ConRoi_CheckoutTest')
O=R/'Docs/QuanLy/CheckoutTest20261107'
def git(root,*args,env=None):
    return subprocess.check_output(['git','-c','safe.directory='+str(root),*args],cwd=root,env=env)
assert not T.exists(), 'Destination already exists; do not overwrite'
assert shutil.disk_usage('C:/').free>34833729755
assert not git(R,'status','--porcelain','--untracked-files=no').strip()
commit=git(R,'rev-parse','HEAD').decode().strip()
assert commit!=json.loads((O/'preflight.json').read_text())['start_head'], 'Commit A baseline first'
env=dict(os.environ,GIT_LFS_SKIP_SMUDGE='1')
T.mkdir()
git(T,'init','--quiet')
git(T,'config','core.autocrlf','false')
git(T,'fetch','--no-tags',str(R),commit)
git(T,'checkout','--quiet','--detach','FETCH_HEAD',env=env)
git(T,'lfs','install','--local')
rows=json.loads(git(R,'lfs','ls-files','--json'))
if isinstance(rows,dict):rows=rows.get('files',[])
objects={x['oid']:x['size'] for x in rows}
for sha,size in objects.items():
    src=R/'.git/lfs/objects'/sha[:2]/sha[2:4]/sha
    dst=T/'.git/lfs/objects'/sha[:2]/sha[2:4]/sha
    assert src.is_file() and src.stat().st_size==size
    dst.parent.mkdir(parents=True,exist_ok=True)
    shutil.copyfile(src,dst)
    assert hashlib.sha256(dst.read_bytes()).hexdigest()==sha
git(T,'lfs','checkout')
fsck=git(T,'lfs','fsck','--objects').decode().strip()
material=[]
for x in rows:
    p=T/x['name']; sha=x['oid']
    assert hashlib.sha256(p.read_bytes()).hexdigest()==sha, x['name']
    material.append(dict(path=x['name'],oid=sha,bytes=x['size']))
assert not (T/'Library').exists(), 'No Library may be copied'
assert not (T/'.git/objects/info/alternates').exists()
assert not git(T,'remote').strip()
assert not git(T,'status','--porcelain','--untracked-files=no').strip()
report=dict(commit=commit,checkout=str(T),independent_git_objects=True,independent_lfs=True,
    files_copied_from_untracked=0,library_before_unity=False,remotes=[],lfs_files=len(rows),
    unique_lfs_objects=len(objects),unique_lfs_bytes=sum(objects.values()),lfs_fsck=fsck,
    C_free_before_import=shutil.disk_usage('C:/').free,materialized=material)
(O/'checkout-created.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k!='materialized'},indent=2))
