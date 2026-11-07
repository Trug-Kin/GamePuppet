"""Commit only this verification's management docs/tools; never product or user files."""
from pathlib import Path
import subprocess
R=Path(__file__).resolve().parents[2]
def git(*args):return subprocess.check_output(['git','-c','safe.directory='+str(R),'-C',str(R),*args]).decode('utf-8').strip()
assert git('branch','--show-current')=='chore/prepare-r107'
assert git('rev-parse','HEAD')=='137ce0eb396557c77af8bb042ab5f3399cde5487'
existing_staged=git('diff','--cached','--name-only').splitlines()
paths=['BAT_DAU_TAI_DAY.md','README.md','Docs/QuanLy/HANDOFF.md','Docs/QuanLy/OWNERSHIP.md','Docs/QuanLy/TICH_HOP.md','Docs/QuanLy/CLONE_VA_BAT_DAU.md']
paths += [p.relative_to(R).as_posix() for p in (R/'Docs/QuanLy/CheckoutFinal20261107').iterdir() if p.is_file()]
tools=['checkout_native_input.py','make_nav_continuation.py','make_nav_remaining.py','make_nav_save.py','make_skip_revisit.py','make_skip_test.py','preserve_checkout_after_exit.py','preserve_checkout_after_tests.py','preserve_checkout_before_sync.py','report_checkout_final.py','sync_preserved_checkout.ps1','update_checkout_final_docs.py','commit_checkout_final.py']
paths += ['Tools/QuanLy/'+p for p in tools]
paths += [p.relative_to(R).as_posix() for p in (R/'Tools/QuanLy/Verification').glob('checkout-final-*.cs.txt')]
paths += ['Tools/QuanLy/Verification/checkout-post-tests-close.cs.txt']
assert len(paths)==len(set(paths))
assert all((R/p).is_file() for p in paths)
assert set(existing_staged)<=set(paths), 'Unrelated staged changes must not be included'
# Normalize only newly authored snippet whitespace, no behavior or source modification.
for n in paths:
    if n.startswith('Tools/QuanLy/'):
        p=R/n;t=p.read_text(encoding='utf-8-sig');p.write_text(t.rstrip()+'\n',encoding='utf-8')
git('add','--',*paths)
staged=git('diff','--cached','--name-only').splitlines()
assert set(staged)==set(paths),(set(staged)-set(paths),set(paths)-set(staged))
check=subprocess.run(['git','-c','safe.directory='+str(R),'-C',str(R),'diff','--cached','--check'],capture_output=True,text=True)
if check.returncode:
    # Preserve the three original stack-trace lines byte-for-byte in raw FAIL evidence.
    diagnostics=[x for x in check.stdout.splitlines() if x.startswith('Docs/')]
    assert len(diagnostics)==3 and all(x.startswith('Docs/QuanLy/CheckoutFinal20261107/navigation-continuation-ui.txt:') and x.endswith('trailing whitespace.') for x in diagnostics),check.stdout
print(git('diff','--cached','--stat'))
print(git('commit','-m','docs: verify a789222 checkout and retain unresolved cipher navigation evidence'))
print('REPORT_COMMIT='+git('rev-parse','HEAD'))
assert not git('diff','--name-only','a789222','HEAD','--','Assets','Packages','ProjectSettings','Source'), 'Product tree must match tested baseline'
