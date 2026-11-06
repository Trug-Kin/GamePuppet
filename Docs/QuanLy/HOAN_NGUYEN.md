# Hoàn nguyên không xóa dữ liệu

Manifest từng file: `archive-manifest.jsonl`; batch thực tế trong `archive-result.json`. Không apply lại plan có archive đã tồn tại. `Staging` giữ bản sao kiểm hash trước move; `DuLieu` giữ bản nguồn đã chuyển. Không có lệnh xóa/mirror trong quy trình.

1. Đóng riêng Editor Con Rối an toàn; nếu có dirty/live chưa phân loại thì dừng, không discard hoặc save runtime vào scene.
2. Với từng dòng MOVED_VERIFIED, kiểm SHA256 của `new` hoặc `staging`. Đích hoàn nguyên là `D:/ConRoi/<old>`; xác minh nằm dưới root và không qua junction.
3. Nếu đích cũ đã tồn tại, **không ghi đè**. So hash: nếu khác thì giữ cả hai và ghi conflict. Nếu chưa tồn tại, Copy-Item từ bản xác minh về vị trí cũ; giữ archive nguyên vẹn. Đây là cách khôi phục có chủ ý, không cần xóa file mới.
4. Script TOOLS_MOVE có nguồn hiện tại trong `Tools/TaiTao/Legacy`; khôi phục tương tự khi cần công cụ cũ. Không chạy recipe một lần để thay thế file.
5. Muốn trả cấu hình entry cũ: kiểm `Docs/QuanLy/EditorBuildSettings.before.asset` rồi copy lại có review; đây là thay đổi cấu hình đã cho phép, không thay checkpoint scene. README/.gitignore trước chuẩn bị nằm trong `Before`; hash baseline không được ghi lại để che thay đổi.

Git repo mới tạo vì ban đầu không có `.git` ở root hoặc cha; không có lịch sử/uncommitted index cũ bị reset. Không xóa `.git` để hoàn nguyên và không reset hard/clean -fd. Nếu đổi ý về tổ chức, tạo commit đảo có review sau khi đối chiếu manifest; file chưa phân loại vẫn giữ nguyên trên máy gốc.
