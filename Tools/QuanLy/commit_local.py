"""Explicit allowlist commits only. Never adds a remote or calls push/fetch/rewrite."""
from pathlib import Path
import subprocess,json,shutil,sys,re
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy'
def git(*args,**kwargs):
 p=subprocess.run(['git',*args],cwd=R,capture_output=True,**kwargs)
 if p.returncode:raise RuntimeError(p.stderr.decode('utf-8',errors='replace')[:1600])
 return p.stdout
assert git('rev-parse','--show-toplevel').decode().strip().replace('\\','/').lower()==str(R).replace('\\','/').lower()
previous=git('diff','--cached','--name-only','-z').decode('utf-8').split('\0')
previous=set(filter(None,previous))
if previous:
 spec=D/'paths-tools-policy.nul'
 expected=set(filter(None,spec.read_bytes().decode('utf-8').split('\0'))) if spec.exists() else set()
 assert previous==expected,'Existing staged set is not the exact prior tool attempt; do not mix'
assert not git('remote').strip(),'Unexpected remote: stop, do not change it'
plan=json.loads((D/'git-stage-plan.json').read_text(encoding='utf-8'))
if set(plan['secret_pattern_files'])-{'Tools/python-deps/PIL/ImageFont.py'}:raise SystemExit('Unreviewed secret pattern; no staging')
(D/'secret-review.json').write_text(json.dumps(dict(pattern_matches=plan['secret_pattern_files'],review='PIL ImageFont.py line1272 embedded base64 default-font data, not credential; no matched values printed',scope='Pattern scan on explicit staged source text under 3MB; not a full credential audit'),indent=2),encoding='utf-8')
attrs=(R/'.gitattributes').read_text(encoding='utf-8')
for ext in ['exe','pyd']:
 if f'*.{ext} filter=lfs' not in attrs:attrs+=f'*.{ext} filter=lfs diff=lfs merge=lfs -text\n'
(R/'.gitattributes').write_text(attrs,encoding='utf-8')
paths=(D/'git-source-paths.txt').read_text(encoding='utf-8').splitlines()
paths=[p for p in paths if (R/p).is_file()]
if shutil.disk_usage(R).free<plan['estimated_unique_lfs_bytes']+1_600_000_000:raise SystemExit('BLOCKED: not enough space for LFS and conservative Git/staging reserve; no source stage')
groups=[('tools-policy',[p for p in paths if p.startswith('Tools/') or p in ['.gitignore','.gitattributes']],'chore: preserve reproduction tools and configure local Git LFS'),('product',[p for p in paths if p.startswith(('Assets/','Source/','Packages/','ProjectSettings/','.agents/'))],'chore: track preserved r107 dependencies and stable integration scene')]
commits=[]
for category,items,message in groups:
 spec=D/f'paths-{category}.nul';spec.write_bytes(b'\0'.join(p.encode('utf-8') for p in sorted(items))+b'\0')
 git('--literal-pathspecs','add','--pathspec-from-file='+str(spec),'--pathspec-file-nul')
 staged=git('diff','--cached','--name-only','-z').decode('utf-8').split('\0');assert set(filter(None,staged))==set(items),'Unexpected staged set'
 # Avoid outputting the diff: third-party text can contain credential-shaped examples.
 check=subprocess.run(['git','diff','--cached','--check'],cwd=R,capture_output=True)
 if check.returncode:
  # Initial commits include unchanged legacy source with pre-existing whitespace.
  # Preserve it and record the failed check; no automatic formatting/refactor.
  lines=check.stdout.decode('utf-8',errors='replace').splitlines()
  warning_paths=sorted(set(line.split(':',1)[0] for line in lines if re.match(r'^[^:]+:\d+:',line)))
  (D/f'diff-check-{category}.json').write_text(json.dumps(dict(status='WARN_INHERITED_INITIAL_SOURCE',exit=check.returncode,warning_lines=len(lines),paths=warning_paths,action='Preserved original legacy bytes; no blanket formatting'),ensure_ascii=False,indent=2),encoding='utf-8')
 git('commit','-m',message)
 commit=git('rev-parse','HEAD').decode().strip();commits.append(dict(category=category,commit=commit,files=len(items)))
 print(category,commit,'files',len(items),flush=True)
 (D/'commits-source.json').write_text(json.dumps(commits,indent=2),encoding='utf-8')
(D/'commits-source.json').write_text(json.dumps(commits,indent=2),encoding='utf-8')
print('Local commits only; no remote. Remaining disk bytes:',shutil.disk_usage(R).free)
