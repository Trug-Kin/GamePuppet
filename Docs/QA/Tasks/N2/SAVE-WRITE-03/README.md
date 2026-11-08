# SAVE-WRITE-03 — acceptance trước sửa

Gameplay & Dữ liệu; nhánh `work/runtime-contracts-baseline`, xuất phát `148440b7a851f9578701377b81367b61b46e7e16`. Giữ schema1/ID/GUID, profile gốc và scene tích hợp. Giữ UI-LOAD-FEEDBACK-01/P-close OPEN; UI1080p/người mới NOT_RUN.

## Luồng hiện tại và điều kiện đạt

Save cập nhật snapshot data trong bộ nhớ → CreateDirectory → WriteAllText `<SavePath>.tmp` (truncate nếu có) → File.Copy(temp,SavePath,true) → File.Delete(temp) → Message thành công. Mọi exception chung trả Message lỗi. Không có File.Replace, flush xuống thiết bị hoặc commit marker; File.Copy overwrite không được coi là giao dịch nguyên tử.

1. Lần đầu/ghi đè thành công: file đích parse/Load được, tiến trình mới đúng, Message thành công.
2. Chặn tạo `.tmp` bằng directory cùng tên; chặn mở ghi `.tmp` bằng handle chỉ cho đọc: báo lỗi, bytes/hash save cũ giữ và Load được. Không làm đầy ổ hoặc đổi ACL.
3. Chặn cập nhật đích bằng handle chia sẻ chỉ đọc: cùng yêu cầu; file tạm có thể còn lại nhưng Load chỉ đọc file đích.
4. Giữ `.tmp` mở cho đọc/ghi nhưng không chia sẻ delete: xác minh lỗi sau copy hoặc tại commit. Nếu báo lỗi, save tốt trước đó phải giữ nguyên. Đây là mô phỏng lỗi handle có kiểm soát, không phải crash/mất điện.
5. Giữ `.tmp` dở/cũ, tải save đích đúng, gỡ điều kiện lỗi trong finally, lưu/tải lại đúng tiến trình mới. Không tự chọn `.tmp` làm tiến trình.
6. Nếu cần sửa nguồn: chèn lỗi ghi sau một phần bytes trong helper ghi; kiểm save cũ nguyên và lưu/tải phục hồi. Không tuyên bố kiểm mid-write nếu chỉ khóa trước mở file.
7. Kiểm F5/F6 bằng input tổng hợp với profile riêng khi khả thi; tách fixture/API và route tự nhiên. Hồi quy validator vì cùng Save/Load; không sửa UI, map hoặc scene tích hợp.

Mỗi ca API tạo thư mục con mới trong profile thử, ghi hash trước/sau và Message. Handle được đóng trong finally, blocker/temporary file được giữ lại dưới tên evidence trong profile thử. Chưa có kết quả; không mặc định đã xác nhận lỗi.


## Kết quả triển khai và nguyên nhân đã xác minh

**Nguồn đã kiểm:** `b76828a92eb47ced8fed09e73bc644a8d016ff6d`; bộ kiểm43 ca tại `b26e8404021e061e8ea92cd4ad01df9e30cb7002`; công cụ kiểm riêng tương thích tại `b1858a78aeab10edf95d1e8be014132eeabb7353`. Unity6000.6.2f1, scene Gameplay_ThuNghiem, Library hiện có của worktree riêng. Không dùng scene tích hợp hoặc save gốc. Đây không phải kiểm checkout/Library sạch mới.

### Baseline trước sửa

[Báo cáo gốc](baseline-results.json) giữ đủ37 entry:34 đạt, **hai assertion lỗi thực** ở `temp_delete_locked_old_bytes_preserved`/`temp_delete_locked_load_after_attempt`; một entry false khác `partial_write_injection_available` ghi **NOT_RUN** vì helper chưa tồn tại. Không tính nó là lỗi runtime thứ ba.

Trong profile thử, giữ handle đọc `.tmp` với FileShare.ReadWrite nhưng không cho delete: WriteAllText và File.Copy thành công, File.Delete ném exception. Save báo lỗi trong khi file đích đã nhận tiến trình mới. Hash cũ `5a539a6a38e8544a10988e2b797b4718231a8d92837b096bc4706b325961c204` đổi thành `285ff5f5d2a8f92b759582b24aed3c53f682aba5c6a7c909f50ac634f01318c5`; Load thấy quan sát mới thay vì save trước. File vẫn parse được; đây là sai kết quả lỗi/ranh giới commit, **không phải bằng chứng mất điện, save JSON hỏng hoặc crash thật**.

### Sửa nguồn tối thiểu

- `InvestigationSession.Save()` gọi helper `InvestigationSaveFile.Write`; chữ ký void và Message hiện có giữ nguyên, thành công chỉ được đặt sau helper trả về. Catch hiện có báo “Không lưu được: …” khi IO lỗi.
- Helper nội bộ tạo folder, ghi UTF8 không BOM vào `.tmp` cùng thư mục qua FileStream, `Flush(true)`, đóng handle, rồi **File.Move** khi chưa có file đích hoặc **File.Replace(temp,path,null)** khi đã có. Không copy/delete đích, không fallback phá hủy nếu Replace không hỗ trợ hoặc thất bại.
- Không thao tác cleanup sau commit có thể biến một lần ghi đã xong thành thông báo lỗi. Không tạo `.bak`, không tự chọn bản dự phòng/tiến trình khác. Load chỉ đọc `prototype.json` và validator hiện có.
- File `.tmp` có thể còn sau lỗi: Load bỏ qua; Save chủ động tiếp theo truncate/ghi lại từ state đang chơi rồi mới commit. Không coi `.tmp` là save đã hoàn tất. Một writer cho mỗi profile; không thêm giao thức nhiều writer.
- Overload private nhận delegate ghi phục vụ chèn IOException sau12 byte trong kiểm Editor; production luôn gọi writer mặc định, không có setting/hook lỗi toàn cục. Ngoại lệ được truyền ra, handle đóng bằng using. Không sửa UI hoặc nhân đôi validator.
- Giữ schema1, namespace/ID, GUID hiện có và khả năng đọc save cũ. Helper mới có GUID/meta riêng. Flush/replace được kiểm trên ổ C Windows của máy này; không tuyên bố độ bền mất điện/crash thật, network filesystem hoặc mọi nền tảng. Nếu filesystem không hỗ trợ Replace, trả lỗi qua Message, không xóa save tốt để thử copy khác.

### Kết quả sau sửa và hồi quy

| Lượt / loại bằng chứng | Kết quả | Phạm vi |
|---|---|---|
| [Ghi save mới](fixed-results.json), PlayMode fixture/API | **43/43 đạt** | Lần đầu/ghi đè; chặn tạo temp; chặn mở temp để ghi; khóa đích; chặn delete/rename temp; temp dở bị Load bỏ qua; lỗi commit lần đầu; chèn IOException sau một phần bytes; gỡ lỗi và Save/Load lại |
| [Hồi quy đầu](regression-first-results.json), fixture/API | **53/54 đạt**, giữ nguyên kết quả | 51 ca khác + hai kiểm tiếp tục/không đổi fixture đạt; ca `load_actual_baseline_schema1_file` dùng nhầm fixture UI có tiến trình khác với fixture baseline mà assertion yêu cầu |
| [Tương thích chạy riêng](compatibility-results.json), fixture/API | **5/5 đạt** | Hai tiền điều kiện scene/profile + đúng ba ca tương thích Load baseline/tiếp tục Save-Load/bytes nguồn không đổi; không chạy lại51 ca đã đạt |
| F5/F6 qua UI/controller cho SAVE-WRITE-03 | **NOT_RUN** | Đã hỏi desktop dành riêng, chưa nhận xác nhận trong lượt này; không gửi input hoặc mở GUI tranh điều khiển |

Fixture đúng cho kiểm tương thích được xác minh từ log SAVE-REFERENCE-02 API_Final: `Test/PlaytestProfiles/SaveContractBaselineRetry_20261108/InvestigationV36/r107/prototype.json` (slide/shoeprint đều chỉ observation0, mapping1, relation1). Lượt đầu dùng `UI_20261108/valid-ui.json` có observations0/1 và không mapping nên assertion không phù hợp. Không sửa dữ liệu nguồn để làm ca đó đạt, không đổi FAIL cũ thành PASS. Cả lượt đầu và lượt riêng đều so bytes fixture nguồn không đổi. Tổng hợp: toàn bộ acceptance hồi quy54 ca có kết quả đạt qua các lượt được nêu rõ, **không gọi lượt đầu là54/54 PASS**.

Các lỗi IO được mô phỏng bằng directory tên `.tmp`, FileStream sharing locks và delegate ném IOException sau12 byte. Không làm đầy ổ, đổi ACL, giết Unity hoặc đụng profile người dùng. Handle dùng using/finally; directory blocker được move sang tên `.blocker-preserved`, file temp/evidence giữ trong profile. Các ca session đọc Message ngay sau Save, so bytes/hash file tốt rồi Load bản cũ; tiếp đó gỡ lỗi, ghi tiến trình mới và Load tiếp. Ca chèn partial-write gọi helper qua reflection và xác nhận IOException, bytes cũ/Load/recovery; **không phải input UI hoặc kiểm Message của partial-write qua F5**.

Compile trong các lượt sau sửa đạt, không có error CS/NullReferenceException/MissingReferenceException trong log. Missing Script kiểm ở root fixture mới và template thử, **không phải audit toàn bộ reference scene tích hợp**; shader warning của batch nographics không thay kiểm render. Không chạy UI 1080p hoặc người mới.

### Chạy bộ kiểm trên checkout có công cụ

Đóng Editor đang ghi cùng checkout trước; PowerShell tại root project, sửa đường dẫn Editor theo máy:

```powershell
$editor = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe'
$project = (Get-Location).Path
$id = [guid]::NewGuid().ToString('N')
$profile = Join-Path $project "Test/PlaytestProfiles/SaveWrite03_$id"
$output = Join-Path $project "Test/Output/SaveWrite03/$id"
New-Item -ItemType Directory -Path $output | Out-Null
& $editor -batchmode -nographics -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.SaveWriteContractChecks.RunBatch -logFile (Join-Path $output 'Editor.log') --investigation-save-dir $profile --save-write-output $output --source-commit (git rev-parse HEAD)
```

Công cụ yêu cầu profile tuyệt đối chưa tồn tại, Unity đúng version; dựng fixture tạm trong Play trên scene thử và thoát không lưu scene. Mặc định43 ca. Chế độ `--save-write-compat <file-fixture-baseline>` chỉ chạy5 entry riêng; fixture này là bằng chứng local theo manifest, yêu cầu Gameplay & Dữ liệu cung cấp nếu cần, không bắt buộc để runtime chạy. Không chọn save riêng/người chơi làm fixture. Raw logs/profiles không cần đưa vào Git.

### Bàn giao và giới hạn còn mở

Nhận theo thứ tự sau baseline đã push148440b7: **b76828a9 (nguồn) → b26e8404 (công cụ) → b1858a78 (kiểm tương thích riêng) → commit tài liệu/bằng chứng của lượt này**. Không chỉ lấy commit báo cáo. UI/UX & Tích hợp review cùng chuỗi hai gói trước; Save/F5 và Load/F6 hiện có tự dùng helper/guard, không wiring mới. Contracts bổ sung ranh giới ghi, không đổi schema/API công khai. Chưa push/merge, không tạo build/ZIP.

**Còn thiếu riêng SAVE-WRITE-03:** F5/F6 với lỗi temp/đích rồi gỡ lỗi và lưu/tải lại qua UI/controller ở profile riêng. Chưa báo hoàn tất kiểm đầu-cuối bằng UI. Message lỗi dùng cơ chế cũ; UI/UX & Tích hợp cần kiểm việc trình bày lỗi Save trong modal khi nhận, không sao chép logic IO sang UI.

**Giữ các điểm cũ:** UI-LOAD-FEEDBACK-01 và P-close **OPEN**; UI1080p/người mới **NOT_RUN**. Không đóng các điểm đó bằng PASS fixture/API của task này.

[Manifest nguồn, log/profile và hash](evidence-manifest.json). Settings tự sinh line-ending-only đã copy/hash rồi khôi phục riêng sau khi các Editor batch thoát. Scene, save gốc, archive/legacy và hai thư mục untracked cũ giữ nguyên. Không stage Library, output, profile, log dài hoặc snapshot.
