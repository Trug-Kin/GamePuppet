# Kiểm checkout sạch 07/11/2026

## Checkout độc lập trên C — cập nhật 07/11/2026

Source giữ `D:/ConRoi`, nhánh `chore/prepare-r107`. **Baseline source hiện `a789222286f1a56db5b6cc21e1ffb5a0e7542be4`**; baseline đã mở lần đầu trong checkout là `8ea91d4d64236f5d8507f216112997402d9b46f6`. Commit tài liệu sau hai mốc này không được gọi là đã kiểm sản phẩm.

[Kết quả hiện hành](current-status.json) thay thế tình trạng BLOCKED_CAPACITY của lần trước: đã tạo checkout thực tại **`C:/ConRoi_CheckoutTest`**, Git objects độc lập, không remote/alternates, không copy file untracked và không dùng chung Library. Trước import C còn 43.972.919.296 byte. LFS đã kiểm hash/materialize **877 file / 795 object / 6.136.024.587 byte**. Library thực được tạo mới bởi Unity **6000.6.2f1**, không copy từ D.

Đã commit đủ **11.326 file A / 3.562.022.051 byte** cùng meta. Sau import phát hiện thêm 5 file D thực sự thuộc closure GUID cấu hình HDRP có sẵn: hai asset, hai meta và folder meta; đã bổ sung có manifest, không đổi pipeline. **D chưa xác minh còn 4.183 file, giữ nguyên tại D**, không archive bừa.

Có lỗi Git thật trong lần checkout đầu: scene legacy nhị phân `Assets/_Recovery/0 (2).unity` bị quy tắc `*.unity text` làm mất 10 byte khi normalize CRLF. Commit `a789222` sửa bằng **LFS đúng đường dẫn**, object hash `00eac743343893f614abbb6cbb1ddc286565d95f330e4913ced30345384a7939`, khớp bản gốc 7.619.172 byte. Không sửa/reserialize checkpoint. Scene entry và prefab text khác vẫn review bình thường. Không rewrite lịch sử; `8ea91d4` giữ làm bằng chứng chưa hoàn chỉnh.

Trên checkout `8ea91d4`: compile C#/Editor đạt; entry **`Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`** mở được, **32.958 component, 0 Missing Script, 0 reference hỏng**, AssetDatabase có đủ 4.582 dependency. GUID meta A khớp và entry không đổi sau chuẩn hóa xuống dòng. Có ảnh render thực **1920×1080**. Những kết quả này không thay cho kiểm UI hoặc xác nhận baseline sửa `a789222` đã chạy.

**Tổng thể vẫn BLOCKED — cần xác nhận quyền điều khiển Editor và hoàn tất kiểm baseline sửa.** Công cụ input tổng hợp chưa xác nhận được Start/luồng UI; intro về sau đã đóng nhưng nguồn thao tác chưa phân biệt chắc. Đã hỏi người dùng có đang điều khiển Unity; chưa có câu trả lời tại lúc ghi. Không gửi thêm input, không dừng Play/discard/lưu runtime vào scene. State live và các thay đổi Unity tự sinh đã snapshot, xem `current-status.json`. Hồ sơ CLI riêng tại `C:/ConRoi_CheckoutTest/Test/PlaytestProfiles/Clean_20261107`; không ghi vào save gốc. Chưa có bằng chứng F5/F6 thành công trong checkout.

Các ca **E/khám/ghi/R/P, save/load trước ghi, skip rồi quay lại, điện thoại, cipher/compare/conclusions qua navigation** còn **NOT_RUN/INCOMPLETE**. Người mới **NOT_RUN**. Lỗi fixture/giới hạn bằng chứng được giữ trong `FIXTURE_ATTEMPTS.md`; không chuyển FAIL cũ thành PASS. Không tuyên bố toàn legacy/map PASS.

Tiếp tục theo thứ tự: xác nhận Editor checkout không được người dùng thao tác → bảo toàn và đóng Play có kiểm soát → đưa checkout tới `a789222` bằng Git/LFS, không copy bù untracked và không ghi đè thay đổi tự sinh đã snapshot → mở lại entry/kiểm reference và các ca UI còn thiếu → chỉ sau đạt mới cập nhật ba task branch nếu bảo toàn commit riêng. Ba task branch vẫn ở **`a209c0e`**; chưa giao clone để sửa độc lập. Ownership không đổi theo OWNERSHIP/TICH_HOP; người 2 tạm sở hữu toàn bộ InvestigationSession partial, người 3 không cùng sửa file đó.

Không tạo thêm archive/build/ZIP/runner, không xóa dữ liệu, không tạo remote hoặc push. Log import dài và cache/profile/snapshot giữ ngoài Git trong checkout C; manifest và báo cáo nhỏ ở đây. Dữ liệu archive cũ tại D:/Game giữ nguyên. Chưa cần thông tin remote/login.
