from pathlib import Path
import subprocess,json
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy';paths=set()
for top in ['Assets','Source']:
 for p in (R/top).rglob('*'):
  if not p.is_file():continue
  name=p.name.lower()
  if any(x in name for x in ['license','licence','copyright','attribution','asset_sources']):
   paths.add(p.relative_to(R).as_posix());meta=Path(str(p)+'.meta')
   if meta.exists():paths.add(meta.relative_to(R).as_posix())
   parent=p.parent
   while parent!=R and parent!=R/'Assets':
    m=Path(str(parent)+'.meta')
    if m.exists():paths.add(m.relative_to(R).as_posix())
    parent=parent.parent
for x in ['Tools/QuanLy/commit_local.py','Tools/QuanLy/final_audit.py','Tools/QuanLy/git_audit.py','Tools/QuanLy/supplement_source.py']:
 paths.add(x)
spec=D/'paths-supplement.nul';spec.write_bytes(b'\0'.join(x.encode('utf-8') for x in sorted(paths))+b'\0')
def git(*args):
 p=subprocess.run(['git',*args],cwd=R,capture_output=True)
 if p.returncode:raise RuntimeError(p.stderr.decode(errors='replace'))
 return p.stdout.decode('utf-8',errors='replace')
assert not git('diff','--cached','--name-only').strip(),'Unexpected existing staged changes'
git('--literal-pathspecs','add','--pathspec-from-file='+str(spec),'--pathspec-file-nul')
staged=git('diff','--cached','--name-only').splitlines()
if staged:
 git('commit','-m','chore: retain asset licenses and finalize preservation audit tools')
 head=git('rev-parse','HEAD').strip()
 (D/'commit-supplement.json').write_text(json.dumps(dict(commit=head,files=staged),ensure_ascii=False,indent=2),encoding='utf-8');print(head,'files',len(staged))
