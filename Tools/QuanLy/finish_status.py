from pathlib import Path
import subprocess,json,collections
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy'
def git(*args):
 p=subprocess.run(['git',*args],cwd=R,capture_output=True)
 if p.returncode:raise RuntimeError(p.stderr.decode(errors='replace'))
 return p.stdout.decode('utf-8',errors='replace').strip()
rows=[json.loads(x) for x in (D/'archive-manifest.jsonl').read_text(encoding='utf-8-sig').splitlines()];profiles=[x for x in rows if x['old'].startswith('Test/PlaytestProfiles/')]
p=D/'HANDOFF.md';s=p.read_text(encoding='utf-8');s+='''
## Đính chính phạm vi profile và checkpoint task

Manifest thực tế có **1 profile PT001r106 đã chuyển** trong lô593file, không phải mọi profile vẫn ở đường cũ. Profile đó được copy/hash rồi move nguyên byte sang archive, có staging cùng SHA256 và không migration/ghi lại; tra `profiles-final.json` cùng manifest để hoàn nguyên khi cần. Profile mới GitPreparation chỉ phục vụ kiểm kỹ thuật, không dùng cho người mới. Báo cáo profile đầu kiểm path cũ thấy absent là do move có chủ ý, không phải mất save; audit đã được sửa để resolve mapping, baseline không đổi.

Commit bàn giao ownership/index: `a209c0e3b41a8b55e438aeabf1025b5ef6e7d940`. Ba nhánh task đã tạo local từ đúng commit này, chưa checkout/worktree hoặc push. Commit sau chỉ bổ sung đính chính/audit, không đổi sản phẩm. Các username cộng tác viên vẫn chưa được cung cấp.
''';p.write_text(s,encoding='utf-8')
status=dict(branch=git('branch','--show-current'),handoff_commit='a209c0e3b41a8b55e438aeabf1025b5ef6e7d940',source_commit='40bcb67035b73776c760b0d84d37dda12ab1fe67',task_branches={name:git('rev-parse',name) for name in ['work/map-asset-baseline','work/runtime-contracts-baseline','work/ui-onboarding-review']},remote_names=git('remote').splitlines(),tracked_ignored=git('ls-files','-ci','--exclude-standard').splitlines(),lfs_objects_status='fsck OK from git-audit.json; only documentation/tools changed afterward',clean_checkout='BLOCKED_CAPACITY_NOT_OPENED',profile_archive_files=len(profiles),push=False)
(D/'FINAL_STATUS.json').write_text(json.dumps(status,ensure_ascii=False,indent=2),encoding='utf-8')
git('add','Docs/QuanLy/HANDOFF.md','Docs/QuanLy/FINAL_STATUS.json','Docs/QuanLy/profiles-final.json','Tools/QuanLy/profile_audit.py','Tools/QuanLy/finish_status.py')
git('commit','-m','docs: clarify verified profile archive and actual task branch commits')
print('Final local commit',git('rev-parse','HEAD'));print('Tracked ignored',status['tracked_ignored'],'remote',status['remote_names'])
print('Tracked modifications',git('status','--porcelain','--untracked-files=no'))
