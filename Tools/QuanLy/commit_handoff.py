from pathlib import Path
import subprocess,json
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy'
def git(*args):
 p=subprocess.run(['git',*args],cwd=R,capture_output=True)
 if p.returncode:raise RuntimeError(p.stderr.decode('utf-8',errors='replace')[:1800])
 return p.stdout.decode('utf-8',errors='replace')
assert not git('diff','--cached','--name-only').strip(),'Unexpected staged files; no mixed commit'
assert not git('remote').strip(),'Unexpected remote'
source=next(x['commit'] for x in json.loads((D/'commits-source.json').read_text(encoding='utf-8')) if x['category']=='product')
branches=['work/map-asset-baseline','work/runtime-contracts-baseline','work/ui-onboarding-review']
(D/'task-branches.json').write_text(json.dumps(dict(source_base=source,branches=branches,branch_base='This handoff commit after creation; inspect git show-ref for concrete SHA',created_local_only=True,worktrees_created=False),indent=2),encoding='utf-8')
paths=set()
for top in ['Docs/QuanLy','Docs/QA']:
 for p in (R/top).rglob('*'):
  if not p.is_file() or p.suffix not in ['.md','.json','.jsonl','.txt','.tsv','.asset']:continue
  name=p.relative_to(R).as_posix()
  if name in ['Docs/QuanLy/KiemKe/files.json','Docs/QuanLy/live-state-preserved.json','Docs/QA/GitPreparation/technical-live-state.json']:continue
  paths.add(name)
for top in ['Tools/QuanLy','Tools/TaiTao']:
 for p in (R/top).rglob('*'):
  if p.is_file() and '__pycache__' not in p.parts:paths.add(p.relative_to(R).as_posix())
for p in (R/'Test/KetQua').rglob('*.md'):paths.add(p.relative_to(R).as_posix())
for p in (R/'Docs').rglob('*.md'):
 if 'Reviews' not in p.parts and 'Archives' not in p.parts:paths.add(p.relative_to(R).as_posix())
paths.update(['README.md','BAT_DAU_TAI_DAY.md','ConRoi.code-workspace','Docs/PROJECT_GUIDE_AND_HISTORY.md','Docs/BLENDER_UNITY_WORKFLOW.md','Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md','.gitignore','.gitattributes'])
spec=D/'paths-handoff.nul';spec.write_bytes(b'\0'.join(x.encode('utf-8') for x in sorted(paths))+b'\0')
git('--literal-pathspecs','add','--pathspec-from-file='+str(spec),'--pathspec-file-nul')
staged=git('diff','--cached','--name-only').splitlines()
assert set(staged).issubset(paths),'Unexpected staged set'
check=subprocess.run(['git','diff','--cached','--check'],cwd=R,capture_output=True)
if check.returncode:
 (D/'diff-check-handoff.json').write_text(json.dumps(dict(status='WARN_PRESERVED_HISTORICAL_DOCS_OR_RECIPES',exit=check.returncode,warning_lines=len(check.stdout.splitlines()),automatic_reformat=False),indent=2),encoding='utf-8')
 git('add','Docs/QuanLy/diff-check-handoff.json')
git('commit','-m','docs: record archive hashes ownership and blocked clean-checkout handoff')
head=git('rev-parse','HEAD').strip()
for name in branches:
 exists=subprocess.run(['git','show-ref','--verify','--quiet','refs/heads/'+name],cwd=R).returncode==0
 if exists:raise RuntimeError('Task branch already exists; preserve it: '+name)
 git('branch',name,head)
print('Handoff local commit',head,'files',len(staged),'task branches',len(branches),'remote none')
