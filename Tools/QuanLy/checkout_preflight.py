"""Inventory the approved A manifest before staging; no source changes."""
import hashlib, json, os, shutil, subprocess, zlib
from pathlib import Path
R=Path(__file__).resolve().parents[2]
O=R/'Docs/QuanLy/CheckoutTest20261107'
O.mkdir(exist_ok=True)
def git(*args):
    return subprocess.check_output(['git','-c','safe.directory='+str(R),*args],cwd=R)
assert git('branch','--show-current').decode().strip()=='chore/prepare-r107'
assert not git('status','--porcelain','--untracked-files=no').strip(), 'Preserve dirty tracked files before proceeding'
assert not Path('C:/ConRoi_CheckoutTest').exists(), 'Never overwrite checkout target'
manifest=json.loads((R/'Docs/QuanLy/Review20261107/dependency-refined.json').read_text(encoding='utf-8'))
A=[x for x in manifest['entries'] if x['category']=='A']
D=[x for x in manifest['entries'] if x['category']=='D']
assert len(A)==11326 and len(D)==4188
tracked=set(git('ls-files','-z').decode().split('\0'))
paths=[x['path'] for x in A]
attrs=git('check-attr','filter','--',*[]) if False else subprocess.check_output(['git','-c','safe.directory='+str(R),'check-attr','--stdin','-z','filter'],cwd=R,input=('\0'.join(paths)+'\0').encode()).decode().split('\0')
filters={attrs[i]:attrs[i+2] for i in range(0,len(attrs)-1,3)}
new_lfs=[]; rows=[]; needed_lfs={}; regular_compressed=0
for x in A:
    n=x['path']; p=R/n
    assert n.startswith('Assets/') and n not in tracked and p.is_file()
    assert p.stat().st_size==x['bytes'], n+' changed size since review'
    assert not p.lstat().st_file_attributes&0x400
    if not n.endswith('.meta'): assert (R/(n+'.meta')).is_file()
    # Individually list large serialized meshes; never apply LFS to every .asset.
    lfs=filters.get(n)=='lfs' or (p.suffix=='.asset' and x['bytes']>16*1024**2)
    if lfs and filters.get(n)!='lfs': new_lfs.append(n)
    digest=hashlib.sha256(); compressor=zlib.compressobj(); compressed=0
    with p.open('rb') as f:
        while block:=f.read(1024**2):
            digest.update(block)
            if not lfs: compressed+=len(compressor.compress(block))
    sha=digest.hexdigest()
    if lfs:
        obj=R/'.git/lfs/objects'/sha[:2]/sha[2:4]/sha
        if not obj.exists(): needed_lfs[sha]=x['bytes']
    else: regular_compressed+=compressed+len(compressor.flush())+256
    rows.append(dict(path=n,bytes=x['bytes'],sha256=sha,lfs=lfs,reason=x['reason']))
for x in D: assert (R/x['path']).is_file(), 'D missing: '+x['path']
main_need=sum(needed_lfs.values())+regular_compressed+512*1024**2
c_need=27709685653+sum(x['bytes'] for x in A)*2
report=dict(start_head=git('rev-parse','HEAD').decode().strip(),branch='chore/prepare-r107',
    C_free=shutil.disk_usage('C:/').free,D_free=shutil.disk_usage('D:/').free,
    C_required_conservative=c_need,D_additional_estimate=main_need,
    new_unique_lfs_bytes=sum(needed_lfs.values()),regular_git_zlib_estimate=regular_compressed,
    A_files=len(A),A_bytes=sum(x['bytes'] for x in A),D_preserved_files=len(D),
    additional_explicit_lfs_meshes=new_lfs,entries=rows)
assert report['C_free']>c_need and report['D_free']>main_need, 'Insufficient space before staging'
(O/'preflight.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
(O/'A-pathspec.nul').write_bytes(('\0'.join(paths)+'\0').encode())
(O/'A-paths.txt').write_text('\n'.join(paths)+'\n',encoding='utf-8')
(O/'additional-lfs-attributes.txt').write_text(''.join('"'+n+'" filter=lfs diff=lfs merge=lfs -text\n' for n in new_lfs),encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k not in ['entries','additional_explicit_lfs_meshes']},indent=2))
