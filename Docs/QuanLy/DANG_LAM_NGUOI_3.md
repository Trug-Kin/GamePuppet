# Người 3 — UI/UX/nội dung/tích hợp

Trạng thái: **CHUA_NHAN**. Nhánh chính xác: `work/ui-onboarding-review`.

Unity **6000.6.2f1**. Baseline nhận việc **fcbb7e19c094a2a52d99237d03587df16679d579**; biên nhận **55d7f04**.
Entry: `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, chỉ người 3 được sửa.
Checkpoint `Assets/Scenes/JourneyPresentation_v48_r107.unity` giữ nguyên. Scene thử dưới đây là **template rỗng**, không phải bản game độc lập.
Mỗi người dùng checkout/Library riêng; không dùng chung project đang mở. Đọc [quyền sở hữu](OWNERSHIP.md), [tích hợp](TICH_HOP.md) và [hợp đồng hiện hành](HOP_DONG_MODULE.md).
Không đổi GUID/meta, schema/save ID, pipeline/packages; không chạy author/derive/bind một lần. Không dọn 4.183 legacy hoặc archive.

Được sửa theo task: `Assets/ConRoi/Modules/GiaoDien/`, `NoiDung/`, `TichHop/`; duy nhất người 3 sửa scene tích hợp, GDD/tài liệu chung và Docs/QA. GDD chỉ đổi sau quyết định thiết kế có review; task đầu không mở rộng nội dung.
Không sửa map/art, save/profile gốc, runtime/contracts hoặc bất kỳ partial `Assets/Scripts/InvestigationV36R107/` (kể cả UI/onboarding); gửi đề nghị/patch cho người 2. Checkpoint vẫn bất biến dù Assets/Scenes do người 3 quản lý.
Scene thử: `Assets/ConRoi/Modules/GiaoDien/Scenes/GiaoDien_ThuNghiem.unity`.

## Việc đầu tiên

Tổ chức **người chưa đọc GDD** chơi thử trước khi mở rộng onboarding. Dùng [phiếu trung tính](../QA/investigation_r107/PLAYTEST_NGUOI_MOI.md), [hướng dẫn chỉ cho người chơi](../QA/investigation_r107/HUONG_DAN_NGUOI_CHOI.md), mẫu [ghi phiên](../QA/investigation_r106/MAU_GHI_NHAN_PHIEN.md) và [tổng hợp vấn đề](../QA/investigation_r106/MAU_TONG_HOP_VAN_DE.md).

Đóng Editor của checkout mình trước khi mở với `--investigation-save-dir <thu-muc-moi-chua-dung>` theo CLONE_VA_BAT_DAU. Xác nhận intro/hồ sơ trống, path F5/F6 thực tế; không dùng profile đã giải. Người chơi tự đi xưởng → Hà → xưởng, không cho đáp án hoặc thời điểm clip. Chỉ gợi ý thao tác khi họ yêu cầu; ghi nguyên văn lời hỏi/gợi ý.

Hoàn tất khi có biên bản mã người thử (không cần thông tin cá nhân), quan sát trực tiếp tách suy luận, chỗ không hiểu cơ cấu/không đọc hình/không biết thao tác/hiểu nhưng chưa đủ chứng minh, vấn đề theo mức ảnh hưởng và bước tái hiện. Không chấm bằng đoán đúng Khải. Chưa có người thật: giữ **NOT_RUN**, bàn giao lịch/phiếu sẵn sàng rồi dừng, không dùng agent hoặc thêm polish.

Bàn giao báo cáo riêng `Docs/QA/Tasks/N3/`, commit docs; đề xuất UI dựa trên vấn đề cụ thể, gửi người 2 nếu chạm session. Tích hợp thay đổi người 1/2 sau review và kiểm đúng phạm vi; không tự sửa thay cả ba module.
Prompt nhận việc: “Trên work/ui-onboarding-review từ fcbb7e19, chuẩn bị và ghi nhận playtest người mới trung thực bằng phiếu trung tính; ưu tiên phản hồi trước mở rộng onboarding, giữ quyền sửa session cho người 2 và scene chính cho người 3.”

## Điền khi nhận

- Người nhận / ngày:
- HEAD thực tế / file sẽ chạm:
- Mã phiên / trạng thái người thử thực tế:
- Kiểm và bằng chứng / đề nghị gửi người 2:
- Commit kết quả / giới hạn / bàn giao tích hợp:


## UI/UX & Tích hợp — nhận RUNTIME-INTEGRATION-01

- Người dùng giao nhận/review năm gói Gameplay & Dữ liệu đến91835c80 từ baseline chung0d878e3b. D giữ nguyên nhánh/ebd92771 và deletion/untracked của người dùng; snapshot trước thao tác ở checkout C.
- Dùng nhánh review/runtime-integration-20261109 tại C:/ConRoi_CheckoutTest, merge đầy đủ d765ed92 giữ commit UI riêng, không main/chore/nhánh thành viên. Không sửa session partial hoặc scene/map.
- [Acceptance/review và kết quả scene riêng](../QA/Tasks/N3/RUNTIME-INTEGRATION-01/README.md). Scene ConRoi_Chinh/profile mới, UI1080p phải đo render thật; người mới NOT_RUN. Runtime lỗi sẽ lập bước tái hiện cho Gameplay & Dữ liệu, không sửa song song.

### Kết quả RUNTIME-INTEGRATION-01 — 09/11/2026

- Nhận đủ runtime91835c80 trên review branch C, giữ lịch sử UI riêng; HEAD đã chạy c18524a3b. Compile/reference và tuyến controller ghi→mục tiêu→phone/clip/cơ cấu→bàn→rời/quay lại→F5/F6 đạt phạm vi đại diện. Không sửa session partial/scene/map.
- Kiểm 7 mẫu Load từ chối và4 lỗi IO F5 trong profile riêng; state/bytes/recovery và P/E/Esc/modal đạt lượt này. Phát hiện evidence=null được F6 nhận, evidence4→0 hai lần: INT-SAVE-NULL-LIST-01 OPEN gửi Gameplay & Dữ liệu, không vá song song.
- UI1920×1080 kiểm các trang tuyến đại diện; mật mã/toàn tuyến/seek-clear-holds trong scene chung chưa kiểm. Load feedback/P-close OPEN riêng; người mới NOT_RUN. Không chốt baseline task mới hoặc push.
- Chi tiết, bước tái hiện, hash và giới hạn: [báo cáo](../QA/Tasks/N3/RUNTIME-INTEGRATION-01/README.md).

## UI/UX & Tích hợp — nhận SAVE-EVIDENCE-04

- Nhận đủ nguồn17b1f6e2/công cụf73631ae/báo cáo597369a5, merge vào review branch C hiện có tại22acc961; giữ riêng tiến độ D/các task, không chốt baseline hoặc push.
- Kiểm scene thật ConRoi_Chinh/profile mới,4 evidence từ bản sao save UI của lượt integration trước, không API grant/proxy. Acceptance trước Play và review: [SAVE-EVIDENCE-04-INTEGRATION](../QA/Tasks/N3/SAVE-EVIDENCE-04-INTEGRATION/README.md).
- File được sửa: báo cáo N3/tiến độ/HANDOFF sau kiểm; không sửa session partial, UI sản phẩm, scene/map. Probe phụ và UI1080p/người mới/Load feedback/P-close giữ trạng thái riêng.

### Kết quả SAVE-EVIDENCE-04-INTEGRATION — 09/11/2026

- Nguồn thực chạy `22acc9611aa8c6d0f86741798b2108d68f9b85d0`, nhận đủ nguồn/công cụ/báo cáo đến597369a5. Scene chung ConRoi_Chinh/profile riêng: null bị từ chối giữ bốn vật chứng/toàn bộ state/bytes; thao tác tiếp, F6 hợp lệ, F5/F6 tiếp tục và evidence:[] đều đạt bằng input Windows tổng hợp. Fixture khởi tạo là bản sao save UI cũ, không gọi là tuyến chơi tự nhiên/người mới.
- Compile/reference đạt phạm vi mở scene; không đổi runtime/scene/UI. Giữ một lượt Esc bị gián đoạn do foreground và lượt kiểm lại đạt. Đã dừng/đóng Editor không save scene; bảo toàn diff/user data. Chỉ đề xuất nguồn22acc961 làm ứng viên, chưa chốt baseline hoặc cập nhật nhánh nhóm/push.
- Probe bổ sung nguồn NOT_RUN, UI1080p đầy đủ/người mới NOT_RUN; Load feedback/P-close OPEN riêng. [Kết quả, hash và giới hạn](../QA/Tasks/N3/SAVE-EVIDENCE-04-INTEGRATION/README.md). Việc tiếp theo là nhóm review/chốt ứng viên và xử lý đúng các mục OPEN; không tự mở rộng task.

## UI/UX & Tích hợp — UI Load feedback/P-close/1080p

- Bắt đầu từ2eac5528 trên review/runtime-integration-20261109. Ownership giữ toàn bộ partial cho Gameplay & Dữ liệu. Commit riêng phiếu UI-LOAD-FEEDBACK-01:b6feeb0f; chưa sửa code vì điểm render nằm trong partial InvestigationSessionUI.OnGUI. Yêu cầu chủ file trình bày Message hiện có, không đổi validation/focus/save.
- Unity thực chạy b6feeb0f (product khớp22acc961), scene chung/profile mới: P cơ bản và sau cipher dở/sai/đúng, pause, Load lỗi đạt trong các ca mục tiêu. P-close lịch sử chưa tái hiện, không có nguồn xác định nên không sửa; giữ OPEN. F6 và nút Tải lỗi giữ state/bytes, E/modal/R và Load hợp lệ/F5/F6 tiếp tục đạt.
- UI1080p mục tiêu đã chụp/đọc và thao tác world/dossier/pause/cipher/compare/conclusion; xác nhận lý do lỗi không hiện trong dossier/pause. Tổng thể REVIEW_REQUIRED, các biến thể nội dung chưa chạy NOT_RUN. Ghi riêng click chưa chuyển/ca assert N-n của công cụ và kiểm lại; không coi là lỗi gameplay. Probe nguồn trước, lượt mất focus cũ, người mới giữ riêng.
- [Báo cáo và acceptance](../QA/Tasks/N3/UI-NAV-REVIEW-02/README.md), [phiếu chủ partial cần nhận](../QA/Tasks/N3/UI-LOAD-FEEDBACK-01/README.md). Việc tiếp theo: nhận commit sửa từ Gameplay & Dữ liệu rồi kiểm feedback và navigation liên quan trên commit tích hợp thật. Không sửa UI song song, không chốt baseline/push/cập nhật nhánh nhóm. Đã đóng Unity và bảo toàn diff/untracked; không đổi guard save.
