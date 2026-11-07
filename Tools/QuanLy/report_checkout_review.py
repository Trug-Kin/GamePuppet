"""Preserve observed import changes and report scope honestly; never stop or edit Unity."""
import datetime,hashlib,json,os,re,shutil,subprocess
from pathlib import Path
R=Path(__file__).resolve().parents[2];T=Path('C:/ConRoi_CheckoutTest');O=R/'Docs/QuanLy/CheckoutTest20261107'
def git(root,*args):return subprocess.check_output(['git','-c','safe.directory='+str(root),'-C',str(root),*args])
stamp=datetime.datetime.now(datetime.timezone.utc).strftime('%Y%m%d_%H%M%S')
snapshot=T/'Test/Output/CleanCheckout'/('ImportedChanges_'+stamp);snapshot.mkdir()
changed=list(filter(None,git(T,'diff','--name-only','-z').decode().split('\0')))
snap=[]
for n in changed:
    src=T/n;dst=snapshot/n
    assert src.is_file() and not src.lstat().st_file_attributes&0x400
    dst.parent.mkdir(parents=True,exist_ok=True);shutil.copyfile(src,dst)
    sha=hashlib.sha256(src.read_bytes()).hexdigest();assert hashlib.sha256(dst.read_bytes()).hexdigest()==sha
    snap.append(dict(path=n,snapshot=str(dst),bytes=src.stat().st_size,sha256=sha))
(snapshot/'manifest.json').write_text(json.dumps(snap,indent=2),encoding='utf-8')
(snapshot/'changes.patch').write_bytes(git(T,'diff','--binary'))
meta_mismatches=[];entry='Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity'
rows=json.loads((O/'preflight.json').read_text(encoding='utf-8'))['entries']
for x in rows:
    if not x['path'].endswith('.meta'):continue
    n=x['path'];a=re.search(rb'^guid: ([a-f0-9]{32})',(R/n).read_bytes(),re.M);b=re.search(rb'^guid: ([a-f0-9]{32})',(T/n).read_bytes(),re.M)
    if (a[1] if a else None)!=(b[1] if b else None):meta_mismatches.append(n)
assert not meta_mismatches
assert (R/entry).read_bytes().replace(b'\r\n',b'\n')==(T/entry).read_bytes().replace(b'\r\n',b'\n')
name='Assets/_Recovery/0 (2).unity';pointer=git(R,'show','HEAD:'+name).decode();sha=re.search(r'oid sha256:([a-f0-9]+)',pointer)[1]
obj=R/'.git/lfs/objects'/sha[:2]/sha[2:4]/sha
assert hashlib.sha256(obj.read_bytes()).hexdigest()==sha==hashlib.sha256((R/name).read_bytes()).hexdigest()
for n in ['reference-audit.txt','entry-dependencies.txt','before-record.txt','before-record-calibrated.txt','click-probe.txt']:
    p=T/'Test/Output/CleanCheckout'/n
    if p.is_file():shutil.copyfile(p,O/n)
shutil.copyfile(T/'Test/Output/CleanCheckout/intro-1080.png',O/'intro-1080.png')
# Preserve first fixture result as a transcript extract; its raw output was overwritten
# by a second fixture attempt before the first archive copy could complete.
(O/'FIXTURE_ATTEMPTS.md').write_text('''# Phạm vi input và các lần fixture chưa đạt

- Lần đầu: log tool ghi resolution=1536x864, FAIL 1080p render dimensions. Raw file bị lần thử sau ghi đè trước khi copy thành công; transcript này không giả là bản raw đầy đủ.
- Sau resize: ScreenCapture thực 1920x1080. Các file before-record*.txt vẫn FAIL tại nhấn Start, không được đổi thành PASS sản phẩm. File before-record-fixture1536-FAIL.txt có tên nhầm: nội dung thực là lần 1920x1080 không kích hoạt nút, giữ nguyên và đính chính tại đây.
- Các thử EditorWindow event và native mouse là input tổng hợp, không phải người mới. Hai lần native input bị guard từ chối vì foreground PID không phải Editor; không suy thành lỗi game. Các thao tác native đầu chưa có xác nhận state đáng tin cậy.
- Trang intro sau đó chuyển sang page rỗng, introAccepted=true nhưng chưa xác định chắc nguồn thao tác. Đã hỏi người dùng có đang điều khiển Unity, chưa có phản hồi lúc viết báo cáo. Không tính việc đóng intro này thành UI PASS.
- Reflection geometry bị Unity bridge từ chối namespace; đã dùng API public để đọc geometry. Không thay quyền bảo vệ bridge.
- Các ca controller/E/ghi/R/P/điện thoại, save/load trước ghi, skip, cipher/compare/conclusions chưa có bằng chứng UI đạt của checkout. NOT_RUN/INCOMPLETE; người mới NOT_RUN.

Client cũ chỉ giữ cải tiến chọn project rõ ràng và title chung; các thử native focus/click đã bỏ khỏi bản bàn giao. Không thêm runner/component runtime.
''',encoding='utf-8')
library_bytes=sum(p.stat().st_size for p in (T/'Library').rglob('*') if p.is_file() and not p.lstat().st_file_attributes&0x400)
result=dict(utc=datetime.datetime.now(datetime.timezone.utc).isoformat(),source_commit=git(R,'rev-parse','HEAD').decode().strip(),checkout_commit=git(T,'rev-parse','HEAD').decode().strip(),
    source_branch=git(R,'branch','--show-current').decode().strip(),checkout=str(T),unity='6000.6.2f1',
    status='BLOCKED_EDITOR_CONTROL_AND_PENDING_CORRECTED_BASELINE_TEST',
    checkout_creation='PASS',initial_lfs='PASS: 877 materialized files / 795 independently hashed objects',
    compile_initial='PASS: Assembly-CSharp and Editor compiled; no CS errors observed',
    entry_reference='PASS at 8ea91d4: 32958 components, 0 MissingScript, 0 broken serialized reference, 4582 dependencies present',
    render_1080='PASS rendering only, not UI traversal',ui_flow='INCOMPLETE / NOT_RUN; see FIXTURE_ATTEMPTS.md',
    original_saves_modified=False,entry_bytes_equal_after_line_normalization=True,A_meta_guid_mismatches=meta_mismatches,
    binary_checkpoint_LFS_object_sha256=sha,D_unclassified_remaining=4183,
    preserved_checkout_changes=snap,live_snapshot=str(max((T/'Test/Output/CleanCheckout').glob('LiveSnapshot_*'))),
    private_profile=str(T/'Test/PlaytestProfiles/Clean_20261107'),
    measured_Library_bytes=library_bytes,C_free_now=shutil.disk_usage('C:/').free,D_free_now=shutil.disk_usage('D:/').free,
    task_branches=git(R,'for-each-ref','--format=%(refname:short) %(objectname)','refs/heads/work/').decode().splitlines(),remotes=git(R,'remote').decode().splitlines(),
    next='Confirm checkout Editor is not being used; preserve/close controlled Play, apply corrected Git baseline a789222 without losing recorded auto changes, materialize new binary-scene LFS object, reopen/test entry and all missing UI cases, then advance task branches safely. No automatic reset/discard/overwrite.')
(O/'current-status.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
text='''## Checkout độc lập trên C — cập nhật 07/11/2026

Source giữ `D:/ConRoi`, nhánh `chore/prepare-r107`. **Baseline source hiện `a789222286f1a56db5b6cc21e1ffb5a0e7542be4`**; baseline đã mở lần đầu trong checkout là `8ea91d4d64236f5d8507f216112997402d9b46f6`. Commit tài liệu sau hai mốc này không được gọi là đã kiểm sản phẩm.

[Kết quả hiện hành](LINK) thay thế tình trạng BLOCKED_CAPACITY của lần trước: đã tạo checkout thực tại **`C:/ConRoi_CheckoutTest`**, Git objects độc lập, không remote/alternates, không copy file untracked và không dùng chung Library. Trước import C còn 43.972.919.296 byte. LFS đã kiểm hash/materialize **877 file / 795 object / 6.136.024.587 byte**. Library thực được tạo mới bởi Unity **6000.6.2f1**, không copy từ D.

Đã commit đủ **11.326 file A / 3.562.022.051 byte** cùng meta. Sau import phát hiện thêm 5 file D thực sự thuộc closure GUID cấu hình HDRP có sẵn: hai asset, hai meta và folder meta; đã bổ sung có manifest, không đổi pipeline. **D chưa xác minh còn 4.183 file, giữ nguyên tại D**, không archive bừa.

Có lỗi Git thật trong lần checkout đầu: scene legacy nhị phân `Assets/_Recovery/0 (2).unity` bị quy tắc `*.unity text` làm mất 10 byte khi normalize CRLF. Commit `a789222` sửa bằng **LFS đúng đường dẫn**, object hash `00eac743343893f614abbb6cbb1ddc286565d95f330e4913ced30345384a7939`, khớp bản gốc 7.619.172 byte. Không sửa/reserialize checkpoint. Scene entry và prefab text khác vẫn review bình thường. Không rewrite lịch sử; `8ea91d4` giữ làm bằng chứng chưa hoàn chỉnh.

Trên checkout `8ea91d4`: compile C#/Editor đạt; entry **`Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`** mở được, **32.958 component, 0 Missing Script, 0 reference hỏng**, AssetDatabase có đủ 4.582 dependency. GUID meta A khớp và entry không đổi sau chuẩn hóa xuống dòng. Có ảnh render thực **1920×1080**. Những kết quả này không thay cho kiểm UI hoặc xác nhận baseline sửa `a789222` đã chạy.

**Tổng thể vẫn BLOCKED — cần xác nhận quyền điều khiển Editor và hoàn tất kiểm baseline sửa.** Công cụ input tổng hợp chưa xác nhận được Start/luồng UI; intro về sau đã đóng nhưng nguồn thao tác chưa phân biệt chắc. Đã hỏi người dùng có đang điều khiển Unity; chưa có câu trả lời tại lúc ghi. Không gửi thêm input, không dừng Play/discard/lưu runtime vào scene. State live và các thay đổi Unity tự sinh đã snapshot, xem `current-status.json`. Hồ sơ CLI riêng tại `C:/ConRoi_CheckoutTest/Test/PlaytestProfiles/Clean_20261107`; không ghi vào save gốc. Chưa có bằng chứng F5/F6 thành công trong checkout.

Các ca **E/khám/ghi/R/P, save/load trước ghi, skip rồi quay lại, điện thoại, cipher/compare/conclusions qua navigation** còn **NOT_RUN/INCOMPLETE**. Người mới **NOT_RUN**. Lỗi fixture/giới hạn bằng chứng được giữ trong `FIXTURE_ATTEMPTS.md`; không chuyển FAIL cũ thành PASS. Không tuyên bố toàn legacy/map PASS.

Tiếp tục theo thứ tự: xác nhận Editor checkout không được người dùng thao tác → bảo toàn và đóng Play có kiểm soát → đưa checkout tới `a789222` bằng Git/LFS, không copy bù untracked và không ghi đè thay đổi tự sinh đã snapshot → mở lại entry/kiểm reference và các ca UI còn thiếu → chỉ sau đạt mới cập nhật ba task branch nếu bảo toàn commit riêng. Ba task branch vẫn ở **`a209c0e`**; chưa giao clone để sửa độc lập. Ownership không đổi theo OWNERSHIP/TICH_HOP; người 2 tạm sở hữu toàn bộ InvestigationSession partial, người 3 không cùng sửa file đó.

Không tạo thêm archive/build/ZIP/runner, không xóa dữ liệu, không tạo remote hoặc push. Log import dài và cache/profile/snapshot giữ ngoài Git trong checkout C; manifest và báo cáo nhỏ ở đây. Dữ liệu archive cũ tại D:/Game giữ nguyên. Chưa cần thông tin remote/login.

'''
(O/'README.md').write_text('# Kiểm checkout sạch 07/11/2026\n\n'+text.replace('LINK','current-status.json'),encoding='utf-8')
for n,link in [('Docs/QuanLy/HANDOFF.md','CheckoutTest20261107/README.md'),('BAT_DAU_TAI_DAY.md','Docs/QuanLy/CheckoutTest20261107/README.md'),('README.md','Docs/QuanLy/CheckoutTest20261107/README.md')]:
    p=R/n;p.write_text(text.replace('LINK',link)+p.read_text(encoding='utf-8-sig'),encoding='utf-8')
for n,link in [('Docs/QuanLy/CLONE_VA_BAT_DAU.md','CheckoutTest20261107/README.md'),('Docs/QuanLy/TICH_HOP.md','CheckoutTest20261107/README.md'),('Docs/QA/README.md','../QuanLy/CheckoutTest20261107/README.md')]:
    p=R/n;p.write_text('> 07/11/2026: [Checkout C thực và giới hạn hiện hành]('+link+'). Source a789222; checkout mở ban đầu 8ea91d4. Compile/reference entry đạt; UI và baseline sửa chưa kiểm xong, task branch chưa tiến.\n\n'+p.read_text(encoding='utf-8-sig'),encoding='utf-8')
print(json.dumps({k:result[k] for k in ['source_commit','checkout_commit','status','D_unclassified_remaining','measured_Library_bytes','C_free_now']},indent=2))
