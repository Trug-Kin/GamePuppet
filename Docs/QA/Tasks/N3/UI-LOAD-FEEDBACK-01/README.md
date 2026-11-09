# UI-LOAD-FEEDBACK-01 — yêu cầu sửa theo ownership

UI/UX & Tích hợp, 09/11/2026. **OPEN — chờ Gameplay & Dữ liệu nhận file sửa.** Nhánh review/runtime-integration-20261109, nguồn sản phẩm 22acc961, HEAD trước task 2eac5528. Không chuyển quyền sửa session partial trong đợt này.

## Căn cứ và phạm vi

- `Assets/Scripts/InvestigationV36R107/InvestigationSession.cs`: `Load()` trả bool, catch trả false và đặt Message="Không tải: "+e.Message. Message tồn tại 6 giây theo unscaledTime. Các guard save hiện có giữ nguyên.
- `Assets/Scripts/InvestigationV36R107/InvestigationSessionUI.cs`, `OnGUI()`: nhánh `!Modal` vẽ Message, modal sidebar chỉ gọi Small(Message) khi diagnostics=true. Hồ sơ/pause mặc định không hiển thị lý do dù runtime đã trả lỗi. Nguồn đã có NAV-01 clearTextFocus; không thay focus để chữa lỗi hiển thị.
- OWNERSHIP giao toàn bộ partial Assets/Scripts cho Gameplay & Dữ liệu. Không tạo presenter/overlay thứ hai để né ownership, không nhân đôi validator hoặc wiring scene.

## Phiếu nhận sửa gửi Gameplay & Dữ liệu

**File đề nghị sửa duy nhất:** `Assets/Scripts/InvestigationV36R107/InvestigationSessionUI.cs`, điểm render Message trong OnGUI. Chủ file giữ session partial, UI/UX & Tích hợp review bố cục và nhận commit sau đó.

1. Hiển thị Message hiện có trong hồ sơ và pause ngay sau F6/nút Tải bị từ chối, diagnostics=false. Dùng đúng kết quả runtime; không parse JSON/check ID tại UI, không đổi validation/save/Load return.
2. Không hiện thông báo tải thành công khi Load=false; không tự đóng modal hoặc sửa file save. Không bật diagnostics để xem lỗi. Có thể giữ thời hạn Message hiện có; không thêm schema hoặc lưu trạng thái UI vào save.
3. Bố trí vùng thông báo có wrap, đọc được tại1920×1080, không che nút Trở lại/Lưu/Tải hoặc nội dung; thông báo dài phải có cách đọc đủ. Phân biệt thông báo và kết quả thành công theo nội dung runtime, không suy thành công từ việc bấm nút.
4. Giữ luồng world hiển thị đang có, NAV-01/focus và toàn bộ guard save. Trả commit nguồn riêng, mô tả vị trí UI và hướng kiểm; không sửa scene chung hoặc cập nhật nhánh nhóm.

## Acceptance nhận lại

- Profile riêng, scene ConRoi_Chinh, render1920×1080. Mở hồ sơ rồi F6 save lỗi và bấm Tải; tương tự pause. Thấy lý do trực tiếp trong modal, không cần thoát world; không thấy báo thành công.
- So toàn bộ data/identity/progress/position/page/paused/input flags và bytes save trước/sau từ chối; P/E/Esc/controller vẫn đúng ngữ cảnh. Modal khóa di chuyển.
- Load save hợp lệ rồi F5/F6 tiếp tục bình thường; thông báo mới đúng kết quả. Kiểm layout world/hồ sơ/pause và các navigation chịu ảnh hưởng. Chỉ hồi quy liên quan, không chạy toàn bộ API/IO.
- UI/UX & Tích hợp giữ P-close riêng: chỉ đề nghị sửa nếu tái hiện và xác định nguyên nhân; không sửa NAV-01 phỏng đoán. Người mới NOT_RUN, probe bổ sung bên giao giữ riêng.

## Trạng thái bàn giao

Yêu cầu được ghi local để chủ file nhận, chưa gửi tin nhắn ngoài ứng dụng/chưa push. Chưa có commit bản vá từ Gameplay & Dữ liệu; **không tuyên bố đã sửa Load feedback**. Kiểm P-close/UI sẽ ghi báo cáo riêng trên đúng commit thực chạy.

## Tái xác nhận trên scene chung

Đã kiểm trên b6feeb0f, render1920×1080, diagnostics:0: F6/nút Tải lỗi trong hồ sơ và pause giữ đúng state/bytes nhưng ảnh không có lý do; world có. Không có bản vá nguồn trong đợt này. [Báo cáo UI-NAV-REVIEW-02](../UI-NAV-REVIEW-02/README.md) ghi các ca P/Load và bằng chứng. Chủ partial Gameplay & Dữ liệu cần nhận yêu cầu trên, bàn giao commit nguồn trước khi UI/UX & Tích hợp kiểm lại/chốt.

## Biên nhận sau bản vá — 09/11/2026

Nhận đủ25f14632→5be056e4→a374a93a; nguồn tích hợp thực chạy f242b60a. [Kiểm scene chung1080p](../UI-LOAD-FEEDBACK-01-INTEGRATION/README.md) đã đạt acceptance Load feedback có phạm vi; giữ nguyên kết quả trước vá ở trên. P-close OPEN/chưa tái hiện/nguyên nhân chưa rõ; người mới và probe cũ NOT_RUN. Chỉ đề xuất ứng viên baseline, chưa cập nhật nhánh nhóm/push.
