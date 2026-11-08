"""Fast-forward only unchanged task refs, then record actual hashes without rewriting history."""
from pathlib import Path
import subprocess,json,datetime
R=Path(__file__).resolve().parents[3];source='074caf3dd3da4107b4fc9ac1c089b20216b4705b';report='fcbb7e19c094a2a52d99237d03587df16679d579';old='a209c0e3b41a8b55e438aeabf1025b5ef6e7d940'
def git(*a):return subprocess.check_output(['git','-c','safe.directory='+str(R),'-C',str(R),*a]).decode('utf-8').strip()
assert git('rev-parse','HEAD')==report and git('branch','--show-current')=='chore/prepare-r107'
assert not git('diff','--name-only') and not git('diff','--cached','--name-only')
audit=json.loads((R/'Docs/QuanLy/NavFix20261108/audit.json').read_text(encoding='utf-8'));assert audit['status']=='PASS_TECHNICAL_NAV01' and audit['tested_source']==source
assert not git('diff','--name-only',source,report,'--','Assets','Packages','ProjectSettings','Source')
branches=['work/map-asset-baseline','work/runtime-contracts-baseline','work/ui-onboarding-review'];worktrees=git('worktree','list','--porcelain')
for b in branches:
    assert 'branch refs/heads/'+b not in worktrees,'Task has a worktree; preserve separately'
    assert git('rev-parse',b)==old,'Task ref changed; no force update allowed'
    git('merge-base','--is-ancestor',old,report)
    assert git('rev-list','--count',report+'..'+b)=='0','Private task commits must not be replaced'
commands='start\n'+''.join('update refs/heads/'+b+' '+report+' '+old+'\n' for b in branches)+'prepare\ncommit\n'
subprocess.run(['git','-c','safe.directory='+str(R),'-C',str(R),'update-ref','--stdin'],input=commands.encode('ascii'),check=True)
actual={b:git('rev-parse',b) for b in branches};assert set(actual.values())=={report}
receipt=dict(utc=datetime.datetime.now(datetime.timezone.utc).isoformat(),tested_source=source,report_commit=report,team_baseline=report,old_task_baseline=old,task_branches=actual,private_commits_replaced=0,worktrees_before=worktrees,product_diff_source_to_team=[],remote_created=False,pushed=False)
p=R/'Docs/QuanLy/NavFix20261108/team-baseline.json';assert not p.exists();p.write_text(json.dumps(receipt,indent=2),encoding='utf-8')
handoff=R/'Docs/QuanLy/HANDOFF.md';text=handoff.read_text(encoding='utf-8');marker='<!-- nav01-team-receipt-20261108 -->';assert marker not in text
note=f'''{marker}
## Baseline nhóm đã chốt — biên nhận 08/11/2026

- **Nguồn đã chạy kiểm trong checkout C: `{source}`.**
- **Commit báo cáo chính và baseline nhóm: `{report}`.** Commit này chỉ thêm tài liệu/công cụ kiểm sau nguồn; product tree Assets/Packages/ProjectSettings/Source khớp nguồn đã kiểm.
- Ba nhánh `work/map-asset-baseline`, `work/runtime-contracts-baseline`, `work/ui-onboarding-review` **đã fast-forward từ a209c0e tới fcbb7e19** bằng transaction expected-old. Trước cập nhật: không commit riêng, không worktree task; không reset/force hoặc thay đổi stash. [Biên nhận hash đầy đủ](NavFix20261108/team-baseline.json).
- Nhóm đủ điều kiện nhận ba task theo ownership/DANG_LAM trên baseline này trong phạm vi kỹ thuật đã kiểm. Partial session vẫn do người 2 giữ trọn. NAV-01 PASS; người mới NOT_RUN, 4.183 file legacy chưa xác minh giữ nguyên; không suy toàn map PASS.
- Checkout thử vẫn detached ở 074caf3, độc lập với project D. Commit biên nhận sau báo cáo chỉ là metadata, không phải một bản sản phẩm mới được chạy kiểm. Không remote/push; không cần đăng nhập.

---

'''
handoff.write_text(note+text,encoding='utf-8')
paths=['Docs/QuanLy/HANDOFF.md','Docs/QuanLy/NavFix20261108/team-baseline.json','Tools/QuanLy/NavFix/finalize_branches.py']
git('add','--',*paths);assert set(git('diff','--cached','--name-only').splitlines())==set(paths)
git('diff','--cached','--check');print(git('commit','-m','docs: record verified team baseline fast-forwards'))
print(json.dumps(dict(receipt_commit=git('rev-parse','HEAD'),**receipt),indent=2))
