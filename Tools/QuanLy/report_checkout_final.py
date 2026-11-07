"""Collect scoped, reproducible handoff evidence without modifying Unity product files."""
import datetime,hashlib,json,re,shutil,subprocess
from pathlib import Path
R=Path(__file__).resolve().parents[2];C=Path('C:/ConRoi_CheckoutTest');O=R/'Docs/QuanLy/CheckoutFinal20261107';Q=C/'Test/Output/CleanCheckout'
def git(root,*args):return subprocess.check_output(['git','-c','safe.directory='+str(root),'-C',str(root),*args]).decode('utf-8').strip()
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
baseline='a789222286f1a56db5b6cc21e1ffb5a0e7542be4'
assert git(C,'rev-parse','HEAD')==baseline
assert not git(R,'diff','--name-only','HEAD','--','Assets','Packages','ProjectSettings','Source'), 'Unexpected source product change'
assert (Q/'post-tests-close.txt').read_text().startswith('Play stopped'), 'Controlled close not confirmed'
texts=['reference-audit.txt','before-record-calibrated.txt','onboarding-route.txt','onboarding-ui-complete.txt','phone-route.txt','nav-fixture-disclosure.txt','nav-setup.txt','navigation-ui.txt','navigation-continuation-ui.txt','navigation-remaining-ui.txt','navigation-save-ui.txt','skip-ui.txt','skip-route.txt','skip-revisit-ui.txt','post-tests-close.txt']
for name in texts:shutil.copyfile(Q/name,O/name)
assert 'FAIL Back then P returns to dossier' in (Q/'navigation-continuation-ui.txt').read_text(encoding='utf-8')
for name in ['before-record-calibrated.txt','onboarding-ui-complete.txt','navigation-remaining-ui.txt','navigation-save-ui.txt','skip-ui.txt','skip-revisit-ui.txt']:
    assert 'FAIL' not in (Q/name).read_text(encoding='utf-8'), name
metas=[x for x in git(C,'ls-files','Assets').splitlines() if x.endswith('.meta')]
bad=[]
for n in metas:
    # Compare GUIDs, not newline serialization or file mtimes.
    a=re.search(rb'^guid: (\S+)',(R/n).read_bytes(),re.M);b=re.search(rb'^guid: (\S+)',(C/n).read_bytes(),re.M)
    if (a.group(1) if a else None)!=(b.group(1) if b else None):bad.append(n)
assert not bad,bad
orig=json.loads((R/'Docs/QuanLy/profiles-final.json').read_text(encoding='utf-8'))['rows'][0]
assert sha(Path(orig['new']))==orig['sha256']
recovery=sha(C/'Assets/_Recovery/0 (2).unity');assert recovery=='00eac743343893f614abbb6cbb1ddc286565d95f330e4913ced30345384a7939'
evidence=[]
selected=texts+['skip-intro-1080.png','first-inspection-720.png','first-floor-recorded.png','first-note-dossier.png','phone-inspection-1080.png','cipher-typed-1080.png','compare-selected-1080.png','conclusions-saved-1080.png','compare-relation-1080.png','skip-revisited-load-1080.png','post-tests-live.json','pre-skip-stop-live.json']
for name in selected:
    p=Q/name;evidence.append(dict(path=str(p),bytes=p.stat().st_size,sha256=sha(p),in_git=name in texts))
(O/'evidence-index.json').write_text(json.dumps(evidence,indent=2),encoding='utf-8')
state=dict(utc=datetime.datetime.now(datetime.timezone.utc).isoformat(),source_branch=git(R,'branch','--show-current'),source_head_before_report_commit=git(R,'rev-parse','HEAD'),tested_commit=baseline,checkout=str(C),unity='6000.6.2f1',entry='Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity',source_product_diff=[],checkout_tracked_status=git(C,'status','--short','--untracked-files=no'),meta_guid_compared=len(metas),meta_guid_mismatch=bad,recovery_sha256_after_import=recovery,original_archived_profile_sha256=sha(Path(orig['new'])),unclassified_assets_files=len(git(R,'ls-files','--others','--exclude-standard','Assets').splitlines()),task_branches=git(R,'for-each-ref','--format=%(refname:short) %(objectname)','refs/heads/work/').splitlines(),source_remotes=git(R,'remote').splitlines(),checkout_remotes=git(C,'remote').splitlines(),free_C=shutil.disk_usage('C:/').free,free_D=shutil.disk_usage('D:/').free,overall='BLOCKED_UI_REGRESSION',newcomer='NOT_RUN',independent_checkout_import='PASS',ui_scope='Synthetic controller/InputSystem + GameView/native mouse; letter-only API fixture for later navigation',library='Originally created fresh at 8ea91d4; incremental import of a789222, never shared/copied from D')
(O/'final-audit.json').write_text(json.dumps(state,indent=2,ensure_ascii=False),encoding='utf-8')
state['checkout_content_diff']=git(C,'diff','--name-only').splitlines()
assert not state['checkout_content_diff'], 'Checkout product content changed; report before claiming tested baseline'
(O/'final-audit.json').write_text(json.dumps(state,indent=2,ensure_ascii=False),encoding='utf-8')
print(json.dumps(state,indent=2,ensure_ascii=False))
