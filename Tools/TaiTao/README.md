# Nguồn kiểm và author

`Legacy` chứa source kiểm/author chuyển nguyên byte từ `Test/Tam`, tra `Docs/QuanLy/archive-manifest.jsonl`. Nhiều script là thao tác **một lần**, chứa output/fixture path lịch sử và tên revision; không chạy lại để "khôi phục" scene. Chưa đánh giá từng recipe thành tool tái sử dụng: trạng thái **PROVISIONAL**. C# trong Assets/Editor/KiemThuLichSu vẫn giữ nguyên để compilation/consumer theo string hoạt động.

Tool quản lý mới trong `Tools/QuanLy` dùng project root từ vị trí script. Plan/inventory không tự move. ChuyenLo.ps1 chỉ apply lô riêng sau snapshot, copy/hash toàn lô rồi native Move-Item; không xóa staging hoặc mirror. Không chạy lại batch có sẵn. Xem README tổ chức về cách hoàn nguyên.
