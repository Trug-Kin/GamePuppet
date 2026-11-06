"""Write dated handoff addendum and verify the small archive, without product edits."""
import hashlib
import json
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2]
OUT = ROOT / 'Docs/QuanLy/Review20261107'
rows = [json.loads(x) for x in (OUT / 'archive-manifest.jsonl').read_text(encoding='utf-8-sig').splitlines()]
checks = []
for row in rows:
    paths = [Path(row['new']), Path(row['staging'])]
    checks.append(dict(old=row['old'], source_absent=not (ROOT / row['old']).exists(),
        destination_and_staging_match=all(p.is_file() and p.stat().st_size == row['bytes'] and hashlib.sha256(p.read_bytes()).hexdigest().lower() == row['sha256'].lower() for p in paths)))
(OUT / 'archive-recheck.json').write_text(json.dumps(checks, indent=2), encoding='utf-8')
assert all(x['source_absent'] and x['destination_and_staging_match'] for x in checks)

audit = json.loads((OUT / 'moved-tools-audit.json').read_text(encoding='utf-8'))
review = [x for x in audit['entries'] if x['status'] == 'REVIEW_REFERENCE']
assert all(all(t['type'] == 'from' for t in x['types_referenced_from_compiled_source']) for x in review)
# The raw lexical scan matches Python's "class from ..." in comments/text.
# Keep that raw evidence and record its manual disposition separately.
(OUT / 'tools-reference-disposition.json').write_text(json.dumps(dict(
    hashes_unchanged=852, lexical_review_rows=len(review),
    disposition='All five rows report identifier from in Python source, not a C# type definition. False positives of language-agnostic regex; no missing compiled C# type established.',
    clean_compile='NOT_RUN', raw_report='moved-tools-audit.json'), indent=2), encoding='utf-8')

text = '''## Rà soát bàn giao 07/11/2026 — checkout vẫn BLOCKED

Baseline sản phẩm được rà: **`90147d9382349575c5bc0385f7405525cad727cd`**, nhánh `chore/prepare-r107`. Đầu phiên không có thay đổi tracked; không remote, không push, không đổi sản phẩm. Commit bổ sung sau mốc này chỉ chứa tài liệu/công cụ kiểm. Entry giữ `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, Unity **6000.6.2f1**. Không tạo r108, build, ZIP hay runner sản phẩm.

[Kết quả chi tiết](REVIEW_LINK) là cập nhật hiện hành; số liệu phía dưới thuộc lần bàn giao trước. LFS máy gốc: **296 file / 217 object / 4.503.946.222 byte**, hash/size và materialization đều đúng. Chưa chứng minh clone chạy được.

Checkout độc lập hiện cần ước tính **27.709.685.653 byte (27,71 GB)**: working tree 6.223.984.553 + LFS 4.503.946.222 + Git 306.433.037 + Library mới dự trù 12.380.354.545 + dự phòng 4.294.967.296. Library dựa trên cache hiện có, chưa phải số đo import mới. C trống 10.197.442.560; D 2.464.395.264; G 9.687.568.384 byte. C thiếu 17.512.243.093; D thiếu 25.245.290.389; G thiếu 18.022.117.269 byte. G chưa xác nhận vị trí/vai trò, không sử dụng. Không dùng ổ ngoài/cloud.

Legacy 15.514 file đã phân loại tĩnh: **A 11.326 file / 3.562.022.051 byte** có consumer cụ thể (gồm Editor/historical tools); **D 4.188 file / 5.402.343.180 byte** chưa xác minh, giữ nguyên. A vẫn UNTRACKED: chưa được coi baseline đủ cho toàn bộ công cụ. Chuỗi GUID của entry và tập compiled source không thiếu file Git theo kiểm tĩnh. Chưa có AssetDatabase/import từ checkout sạch. Báo cáo refined thay thế phân loại raw vốn nhận quá rộng literal `Assets/`. Không suy không dùng từ việc vắng trong scene.

Nếu giữ toàn bộ A để bàn giao Editor, ngân sách bảo thủ tăng tối đa hai lần 3.562.022.051 byte (working tree + object), thành **34.833.729.755 byte**, chưa khử trùng LFS và nén Git. Cần vị trí local được xác nhận có **khoảng 36 GB trống** để bổ sung A và kiểm; sau chọn tập source sẽ đo lại. Không stage hàng GB khi D chỉ còn khoảng 2,46 GB. Nguồn B: Source/license đã tracked; editable trong Assets chưa xác minh recipe tiếp tục D. Không chuyển bất kỳ Assets/meta/GUID nào trong lần rà này.

852 script Tools: **852 hash giữ nguyên**. Năm cảnh báo lexical đều là từ Python `from`, không phải type C# bị mất. Kiểm tĩnh không thay cho compile mới. Scene/prefab text, source/media game và license giữ Git/LFS; cache, profile cá nhân, capture/build/log không thuộc source clone. Docs/Reviews còn trộn nguồn, giữ UNCLASSIFIED.

Lô C mới `D:/Game/ConRoi_LuuTru/git-review-20261107-001945-00b993`: **8 file / 2.108.862 byte** ảnh kiểm GitPreparation và log Editor đã dừng; staging/hash/move/hash được kiểm lại. Manifest ở bộ review; hoàn nguyên bằng old/new/SHA256, copy về khi old vắng, không ghi đè xung đột. Lô cũ `git-team-20261106-153912-ee41c6` giữ nguyên. Chuyển cùng D **không tăng dung lượng trống**; staging mới còn dùng thêm 2.108.862 byte. Không chuyển cache qua junction, runtime media hoặc save gốc.

**Checkout sạch BLOCKED_CAPACITY**; chưa tạo checkout/Library, chưa chạy compile/import/reference/onboarding/save thử trên checkout. Các ca E/ghi/R/P, save trước ghi, skip tutorial, điện thoại, 1080p và mật mã/đối chiếu/kết luận ở checkout đều **NOT_RUN**. Không mượn PASS máy gốc; người mới vẫn NOT_RUN. Không cần remote/login ở bước này.

Ba task branch giữ `a209c0e3b41a8b55e438aeabf1025b5ef6e7d940`, không tiến tới baseline chưa được clean-test. Ownership không đổi: người 1 Map/art/source; người 2 runtime/contracts và toàn bộ InvestigationSession partial tạm thời; người 3 UI/nội dung/scene tích hợp, gửi đề nghị qua người 2 nếu chạm partial dùng chung. Chưa đủ điều kiện tách clone để sửa độc lập. Thứ tự: xác nhận vị trí đủ chỗ → bổ sung A có kiểm → checkout/LFS/import/test → chốt commit đã kiểm → cập nhật task branch nếu không mất việc riêng. Trong lúc chờ chỉ đọc inventory/contracts và nhận task; không polish map/gameplay. CODEOWNERS chờ username thật.

'''
(OUT / 'README.md').write_text('# Rà soát Git 07/11/2026\n\n' + text.replace('REVIEW_LINK', 'dependency-refined.json') + '\nCác manifest: `git-start.json`, `disk-inventory.json`, `capacity.json`, `dependency-refined.json`, `dynamic-consumers.json`, `moved-tools-audit.json`, `tools-reference-disposition.json`, `lfs-verification.json`, `archive-manifest.jsonl`, `archive-recheck.json`. `legacy-classification.json` là raw superseded, không dùng để chốt A/D.\n', encoding='utf-8')
for name, link in [('Docs/QuanLy/HANDOFF.md', 'Review20261107/README.md'),
                   ('BAT_DAU_TAI_DAY.md', 'Docs/QuanLy/Review20261107/README.md'),
                   ('README.md', 'Docs/QuanLy/Review20261107/README.md')]:
    path = ROOT / name
    old = path.read_text(encoding='utf-8-sig')
    path.write_text(text.replace('REVIEW_LINK', link) + old, encoding='utf-8')
for name in ['Docs/QuanLy/CLONE_VA_BAT_DAU.md', 'Docs/QuanLy/TICH_HOP.md', 'Docs/QA/README.md']:
    path = ROOT / name
    old = path.read_text(encoding='utf-8-sig')
    link = '../QuanLy/Review20261107/README.md' if '/QA/' in name else 'Review20261107/README.md'
    path.write_text('> 07/11/2026: [Rà soát mới](' + link + '). Baseline rà `90147d9`; checkout sạch BLOCKED_CAPACITY, chưa nâng task branch. Không kế thừa kiểm máy gốc thành PASS checkout.\n\n' + old, encoding='utf-8')
print('Archive 8/8 verified; UTF-8 handoff addenda written; product unchanged.')
