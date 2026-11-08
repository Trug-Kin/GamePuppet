"""Materialize tracked LFS pointers from verified independent local object copies."""
from pathlib import Path
import hashlib,json,shutil,subprocess,datetime
R=Path(__file__).resolve().parents[3];C=Path('C:/ConRoi_CheckoutTest');O=R/'Docs/QuanLy/NavFix20261108'
def git(*a):return subprocess.check_output(['git','-c','safe.directory='+str(C),'-C',str(C),*a])
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
raw=json.loads(git('lfs','ls-files','--json'));rows=raw.get('files',[]) if isinstance(raw,dict) else raw
needed=[]
for row in rows:
    p=C/row['name']
    if sha(p)==row['oid']:continue
    data=p.read_bytes();assert data.startswith(b'version https://git-lfs.github.com/spec/v1\n') and ('oid sha256:'+row['oid']).encode() in data,'Refuse overwrite non-pointer: '+row['name']
    needed.append(row)
assert all(x['name'].startswith('Docs/') for x in needed),'Inspect runtime pointer before proceeding'
S=C/'Test/Output'/('NavLfsBefore_'+datetime.datetime.now().strftime('%Y%m%d_%H%M%S'));S.mkdir()
for row in needed:
    n=row['name'];dst=S/n;dst.parent.mkdir(parents=True,exist_ok=True);shutil.copyfile(C/n,dst)
    oid=row['oid'];rel=Path('.git/lfs/objects')/oid[:2]/oid[2:4]/oid
    src=R/rel;target=C/rel;assert sha(src)==oid and src.stat().st_size==row['size']
    target.parent.mkdir(parents=True,exist_ok=True)
    if target.exists():assert sha(target)==oid
    else:shutil.copyfile(src,target)
    assert sha(target)==oid
    git('lfs','checkout','--',n);assert sha(C/n)==oid
out=dict(commit=git('rev-parse','HEAD').decode().strip(),initially_pointer=needed,pointer_snapshot=str(S),objects_verified=True,working_tree_untracked_copied=0,runtime_asset_pointers=0)
(O/'lfs-doc-correction.json').write_text(json.dumps(out,indent=2),encoding='utf-8');print(json.dumps(out,indent=2))
