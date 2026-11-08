from pathlib import Path
import subprocess
R=Path(__file__).resolve().parents[3]
def git(*a):return subprocess.check_output(['git','-c','safe.directory='+str(R),'-C',str(R),*a]).decode('utf-8').strip()
assert git('rev-parse','HEAD')=='074caf3dd3da4107b4fc9ac1c089b20216b4705b'
assert git('branch','--show-current')=='chore/prepare-r107'
assert not git('diff','--cached','--name-only')
paths=['BAT_DAU_TAI_DAY.md','README.md']+['Docs/QuanLy/'+x for x in ['HANDOFF.md','OWNERSHIP.md','TICH_HOP.md','CLONE_VA_BAT_DAU.md','DANG_LAM_NGUOI_1.md','DANG_LAM_NGUOI_2.md','DANG_LAM_NGUOI_3.md']]
paths += [p.relative_to(R).as_posix() for d in ['Docs/QuanLy/NavFix20261108','Tools/QuanLy/NavFix'] for p in (R/d).iterdir() if p.is_file()]
for n in paths:
    if n.startswith('Tools/'):
        p=R/n;p.write_text(p.read_text(encoding='utf-8-sig').rstrip()+'\n',encoding='utf-8')
git('add','--',*paths);assert set(git('diff','--cached','--name-only').splitlines())==set(paths)
git('diff','--cached','--check')
print(git('commit','-m','docs: certify NAV-01 fix and scoped independent checkout regression'))
print('REPORT_COMMIT='+git('rev-parse','HEAD'))
assert not git('diff','--name-only','074caf3','HEAD','--','Assets','Packages','ProjectSettings','Source')
