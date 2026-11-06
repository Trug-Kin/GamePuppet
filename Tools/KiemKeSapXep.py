"""Kiểm kê trước tái cấu trúc; không chạy Unity, không sửa nội dung sản phẩm."""
from pathlib import Path
import hashlib, json, os, stat, zipfile, subprocess

ROOT = Path(__file__).resolve().parents[1]
OUT = ROOT / 'Docs/Reviews/journey_v48/restructure-20261102'
OUT.mkdir(exist_ok=True)

def sha(p):
    h = hashlib.sha256()
    with p.open('rb') as f:
        for b in iter(lambda: f.read(4*1024*1024), b''): h.update(b)
    return h.hexdigest()

def files(root):
    if not root.exists(): return
    for base, dirs, names in os.walk(root, followlinks=False):
        dirs[:] = [d for d in dirs if not (Path(base)/d).lstat().st_file_attributes & stat.FILE_ATTRIBUTE_REPARSE_POINT]
        for n in names: yield Path(base)/n

def save(name, value):
    (OUT/name).write_text(json.dumps(value,ensure_ascii=False,indent=2),encoding='utf-8')

if __name__ == '__main__':
    inventory=[]
    for p in ROOT.iterdir():
        s=p.lstat(); link=bool(s.st_file_attributes & stat.FILE_ATTRIBUTE_REPARSE_POINT)
        entry={'path':p.name,'link':link,'target':str(p.resolve()) if link else None}
        if p.is_dir() and not link:
            ff=list(files(p)); entry.update(files=len(ff),bytes=sum(f.stat().st_size for f in ff))
            if any(ord(c)>127 for c in p.name) or p.name in ['H','Bug']:
                entry['contents']=[str(f.relative_to(ROOT)) for f in ff]
        inventory.append(entry)
    save('root-inventory.json',inventory)
    git=subprocess.run(['git','status','--porcelain=v1','--untracked-files=all'],cwd=ROOT,capture_output=True,text=True)
    save('git-status.json',{'exit':git.returncode,'stdout':git.stdout,'stderr':git.stderr,'git_exists':(ROOT/'.git').exists()})
    work=ROOT/'.build-work/r90-windows-20261101'
    delta=[]; hashes={}
    for folder in ['Assets','Packages','ProjectSettings','BuildExcluded']:
        for p in files(work/folder):
            rel=p.relative_to(work).as_posix(); dst=ROOT/rel
            h=sha(p); hashes[rel]=h
            if not dst.exists() or h!=sha(dst):
                delta.append({'path':rel,'work_sha256':h,'main_sha256':sha(dst) if dst.exists() else None})
    save('work-delta.json',delta)
    save('work-source-hashes.json',hashes)
    manifest=json.loads((ROOT/'Docs/Reviews/journey_v48/r92/source-r92-manifest.json').read_text(encoding='utf-8'))
    mismatch=[]
    for section in ['final_source_files','editable_sources']:
        for rel,info in manifest[section].items():
            p=ROOT/rel
            if not p.exists() or sha(p)!=info['sha256']: mismatch.append(rel)
    save('r92-source-mismatch.json',mismatch)
    # Chỉ sao lưu các tệp có thể sửa trong đợt này; mesh/ảnh/Blender/cache không nhân đôi.
    checkpoint=OUT/'checkpoint-before-restructure.zip'
    if checkpoint.exists(): raise RuntimeError('Checkpoint đã tồn tại; không ghi đè')
    selected=[]
    for folder in ['Assets','Packages','ProjectSettings','Tools','.vscode','UserSettings']:
        for p in files(ROOT/folder):
            if folder!='Assets' or p.suffix.lower() in ['.cs','.meta','.asmdef','.asmref','.unity','.prefab','.asset']:
                if folder=='Assets' and p.suffix=='.asset' and p.stat().st_size>2*1024*1024: continue
                if '__pycache__' not in p.parts and 'python-deps' not in p.parts: selected.append(p)
    selected += [ROOT/'README.md', ROOT/'Docs/PROJECT_GUIDE_AND_HISTORY.md', ROOT/'Docs/BLENDER_UNITY_WORKFLOW.md']
    with zipfile.ZipFile(checkpoint,'x',zipfile.ZIP_DEFLATED,compresslevel=3) as z:
        for p in selected: z.write(p,p.relative_to(ROOT).as_posix())
    save('checkpoint.json',{'zip':checkpoint.name,'sha256':sha(checkpoint),'files':len(selected),'scope':'Các file có thể sửa; toàn bộ file khác được giữ tại chỗ rồi di chuyển nguyên trạng, không backup cache/build.'})
    print(json.dumps({'work_delta':delta,'manifest_mismatch':mismatch,'checkpoint_bytes':checkpoint.stat().st_size,'inventory':inventory},ensure_ascii=True))
