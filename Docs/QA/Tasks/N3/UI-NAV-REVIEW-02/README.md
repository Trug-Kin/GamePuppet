# UI-NAV-REVIEW-02 — P-close và UI liên quan1080p

UI/UX & Tích hợp, 09/11/2026. Acceptance ghi trước lượt thao tác. **Đã hoàn tất lượt kiểm mục tiêu; REVIEW_REQUIRED do modal không hiển thị lỗi Load.** Nguồn thực chạy b6feeb0f418eee00336ec919ab519ea6a59921b9 trên review/runtime-integration-20261109 (product tree không đổi so22acc961).

## Acceptance và giới hạn trước kiểm

- Scene chung ConRoi_Chinh, Unity6000.6.2f1, profile mới UIFeedbackPClose20261109, Library C độc lập import tăng dần. Render1920×1080, chụp ảnh Game view và đọc layout thực; không dùng riêng kích thước ảnh làm kết luận.
- Dùng bản sao save thử có4 vật chứng đã tạo bằng UI; có thể chuẩn bị thêm letter và mapping trong fixture riêng để mở navigation, phải ghi rõ. Không cấp fixture rồi gọi là tuyến chơi tự nhiên. Mọi chuyển trang/Save/Load được chứng minh bằng input Windows tổng hợp, không gọi API thay thao tác.
- P mở/đóng nhiều lần ở world/dossier; từ cipher nhập dở/sai/đúng rồi click Trở lại, P mở/đóng; phím P trong ô nhập vẫn chỉ nhập chữ. Esc từ cipher, world pause/resume; modal khóa movement/look.
- Sau F6 lỗi từ world/dossier/pause: data/identity/tiến trình/bytes giữ nguyên; P/E/Esc dùng đúng ngữ cảnh; sau đó tải hợp lệ rồi lưu/tải. Không lặp toàn bộ API/IO. Ghi rõ khả năng nhìn thấy thông báo diagnostics=false ở modal; nếu thiếu, giữ yêu cầu UI-LOAD-FEEDBACK-01, không tự sửa partial.
- Kiểm ảnh và thao tác các màn bị ảnh hưởng: world, hồ sơ, pause, cipher, đối chiếu/kết luận nếu truy cập được. Tách lỗi layout với lỗi P. Chỉ sửa P nếu xác định được nguyên nhân và chủ file nhận sửa. Không tự chốt baseline khi còn lỗi đã xác nhận.
- Mất focus/PID hoặc ai thao tác: dừng input, ghi INTERRUPTED, không suy FAIL gameplay hoặc PASS phần chưa chạy. Probe bổ sung gói trước giữ NOT_RUN riêng, người mới NOT_RUN.

Snapshot trước phiên tại Test/Output/UIFeedbackPClose20261109/Before. Không sửa runtime/UI partial/scene chung, map, schema/ID/GUID hoặc save gốc. Không push/cập nhật nhánh nhóm/chốt baseline.

## Kết quả trên đúng commit đã chạy

Unity đã chạy `b6feeb0f418eee00336ec919ab519ea6a59921b9` (commit phiếu yêu cầu; product tree khớp22acc961). Nhánh vẫn `review/runtime-integration-20261109`. Không sửa sản phẩm/session partial hoặc scene; chỉ có phiếu yêu cầu và tài liệu kết quả. **Compile/reference đạt phạm vi mở scene:**32.958 component,0 Missing Script,0 reference serialized hỏng, đủ binding; log không có error CS/NullReferenceException/MissingReferenceException. Còn warning Burst/WDAC/licensing/cloud của môi trường. C còn13.629.087.744byte trước import; dùng Library riêng hiện có, không tạo clone hoặc dùng Library gốc.

| Nhóm kiểm UI/controller | Kết quả thực tế |
| --- | --- |
| P ở world/hồ sơ |3 chu kỳ đóng/mở ban đầu đạt. |
| Cipher nhập dở, sai, đúng → click Trở lại → P |Mỗi biến thể3 chu kỳ mở/đóng đạt. Sai: nhập Z cho15 ô; đúng: nhập15 ô theo Alphabet hiện có, bản đọc khớp HaPlain. Gõ trực tiếp qua Windows, không API Assign. |
| Focus nhập chữ/modal |P khi focus cipher/compare/claim không đổi trang; W trong cipher và bàn dựng không dịch vị trí; movement/look bị khóa. Esc rời cipher, world Esc mở pause, W khi pause không dịch; Esc resume. P trong pause giữ pause đúng ngữ cảnh hiện có. |
| F6/nút Tải từ chối |F6 ở world/dossier/pause, nút Tải ở dossier/pause đều từ chối evidence:null. Toàn bộ trường state chụp trong manifest và bytes file giữ nguyên, không báo Message thành công. |
| Sau Load lỗi |3 chu kỳ P đóng/mở đạt; aim chuột/E mở bàn dựng thật, W bị chặn, R rời trang. Phục hồi file thử hợp lệ bằng F6, tiếp tục controller/F5/đổi vị trí/F6 khôi phục data và vị trí (sai số<0,002). Không chạy lại bộ API/IO. |
| UI-LOAD-FEEDBACK-01 |**CONFIRMED OPEN**: ảnh1920×1080 của dossier/pause ngay sau lỗi không có lý do; Message trong state là “Không tải: Danh sách evidence trong hồ sơ không hợp lệ; không nhận null hoặc mục null.” World hiện rõ đủ hai dòng. diagnostics:0 trên scene. Không có giả báo thành công trong Message, nhưng modal không cung cấp phản hồi cần thiết. |
| P-close lịch sử |**Chưa tái hiện**, các đường trên đạt. Chưa xác định nguyên nhân lỗi lịch sử; không sửa NAV-01/partial hoặc tuyên bố đã chữa. Giữ OPEN theo dõi, tách khỏi thiếu feedback có căn cứ. |

[Kết quả riêng](results.json), [audit scene](reference-audit.json), [so state/bytes pause](pause-rejection-results.json), [hồ sơ](dossier-rejection-results.json), [bảo toàn](preservation.json), [index local](evidence-index.json).

## Rà UI1080p có phạm vi

Đã xem ảnh Game view thực1920×1080 và dùng nút/ô nhập của intro, world, hồ sơ, pause, cipher, compare, conclusions; tên/nút Trở lại/Lưu/Tải trong phạm vi chụp đọc được và truy cập được. Cipher cả15 ô và câu đọc đúng hiện trọn vùng ảnh. So trang world có lý do lỗi với dossier/pause không có lý do để xác nhận lỗi hiển thị.

**Không gọi UI1080p tổng thể PASS:** thiếu feedback modal đã xác nhận. Compare mới kiểm trang chưa chọn vật/ô liên hệ và navigation; các tổ hợp nội dung sau chọn hai vật chưa chạy. Conclusions kiểm mở/ô nhận định/Trở lại, chưa xác nhận mọi lựa chọn hoặc kết luận cuối. Không lặp toàn bộ clip/reconstruction/playthrough, không suy từ screenshot thành mọi màn đạt. Các phần chưa kiểm đó NOT_RUN, độc lập với P thao tác đạt.

## Fixture, sai lệch công cụ và focus

- Profile mới `Test/PlaytestProfiles/UIFeedbackPClose20261109`; Start/F5 tạo save mới, sau đó F6 bản sao save UI từ lượt SAVE-EVIDENCE-04 integration. Trong **bản sao fixture**, thêm record letter observations=[] và xóa mapping/page để mở các navigation: tổng5 vật chứng. Không cấp API live, không nói đây là tuyến chơi tự nhiên hoặc kiểm bốn vật chứng mới. [Nguồn/hash và biến đổi fixture](fixture-provenance.json). Save gốc và fixture nguồn không ghi đè.
- Tất cả phím/chuột là **input Windows tổng hợp** có foreground PID22784 và nhật ký, không người mới. Không gọi OpenPage/Assign/Load/Save hoặc teleport để chứng minh UI. Đọc state/audit/capture bằng tool Editor đã có.
- Lượt Đối chiếu đầu: P đã mở dossier nhưng click ngắn chưa chuyển trang. Giữ snapshot/input, chưa xác định timing/nguồn; foreground PID đúng, không gọi lỗi P. Chỉ kiểm lại nút đó với mouse-down0,25s, mở compare được. Không sửa game để bù input.
- Lượt claim: assert công cụ đòi `N` trong khi phím N không Shift nhập `n`; dừng trước P, snapshot/log bảo toàn. Sửa kỳ vọng của **bài kiểm local**, không đổi sản phẩm. Lượt tiếp theo từ đúng state `n` hoàn tất P/focus/Trở lại/pause/resume đạt. Không gộp lượt sai assert thành PASS gameplay.
- Không có lượt mất foreground được helper phát hiện trong phiên này; lượt Esc mất focus của SAVE-EVIDENCE-04 integration trước vẫn giữ INTERRUPTED theo báo cáo cũ. **Probe bổ sung nguồn trước NOT_RUN; người mới NOT_RUN.** Không đóng hồi tố các mục đó.

## Bàn giao và phần còn chặn

- Phiếu [UI-LOAD-FEEDBACK-01](../UI-LOAD-FEEDBACK-01/README.md) commit `b6feeb0f`: đề nghị Gameplay & Dữ liệu sửa duy nhất InvestigationSessionUI.OnGUI để trình bày Message hiện có trong dossier/pause, không nhân đôi validation, không thay guard, focus, schema/ID/GUID. Chưa có commit sửa từ chủ partial; request là tài liệu local, chưa gửi tin ngoài ứng dụng/push.
- Chờ chủ file bàn giao nguồn riêng, UI/UX & Tích hợp nhận rồi kiểm lại world/dossier/pause ở1080p và navigation chịu ảnh hưởng trên commit tích hợp thật. Không yêu cầu chạy toàn bộ API/IO.
- **Chưa chốt hoặc cập nhật baseline/nhánh nhóm**, vì acceptance Load feedback còn lỗi đã xác nhận. P-close không có căn cứ để sửa trong lượt này. Chưa push/merge main, không build/ZIP.
- Đã snapshot trước stop, dừng Play ở sceneDirty=false, đóng bình thường không save scene. Năm file Unity đã lưu After/hash, đối chiếu normalized và giữ/phục hồi exact Before bytes. Product diff so commit thực chạy rỗng; các M/untracked trước phiên vẫn giữ. Raw profile/capture/log ở Test/Output và Test/PlaytestProfiles ngoài Git, tìm bằng index; không phải dependency runtime. Không sửa project D hoặc tiến độ Gameplay & Dữ liệu.
