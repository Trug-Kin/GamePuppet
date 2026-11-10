# Bàn giao mô phỏng và tích hợp toàn game — Gameplay & Dữ liệu

**Cập nhật chức năng mới nhất:** FULL-READING-PRODUCER-01, nguồn `942ca056`, công cụ/UI thử `61892688`, trên lịch sử `69e9ca57`. [API, kiểm và tương thích bản chép thật](#reading-producer). Các kiểm f8b50889/e71da758/9dd5d6eb bên dưới giữ nguyên phạm vi/lịch sử; không suy chúng đã chạy trên nguồn mới.

## Phạm vi và acceptance trước triển khai — 10/11/2026

Nhận tại `81beb1d0`, nhánh `work/runtime-contracts-baseline`, worktree riêng. Tracked sạch; `.vscode/` và `Test/KetQua/W_route_Unity_v30_20261016/` giữ nguyên. Fetch: local/remote 0/0; baseline HANDOFF c93cb4f1e và tip tích hợp baf7335b đã có trong lịch sử. Không merge/reset. GDD chuẩn: [v3.6 chốt triển khai](../../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md); [ownership](../../QuanLy/OWNERSHIP.md), [contracts](../../QuanLy/HOP_DONG_MODULE.md), [registry](../../QuanLy/EVIDENCE_REGISTRY.md), [checklist trước mở rộng](../../QuanLy/DANG_LAM_NGUOI_2.md). Phạm vi prototype cũ được mở rộng theo yêu cầu mới; các PASS cũ giữ phạm vi gốc.

Đây là đầu mối **duy nhất** liệt kê mô phỏng toàn game. Các tên API/file mới ở bản acceptance này là dự kiến cho tới khi có source/kiểm được ghi bên dưới. Không sửa sản phẩm UI/scene chung/map. Chín quyết định ND-01…09 của [bàn giao trước](../Tasks/N2/BAN_GIAO_CHO_NOI_DUNG_UI_20261110.md#3-quyết-định-nội-dung) vẫn CHỜ CHỐT.

### Ma trận yêu cầu trước triển khai

| Mã / GDD | Hành vi người chơi | Đã có / phần thiếu | Phụ thuộc và mô phỏng | Acceptance / trạng thái ban đầu |
|---|---|---|---|---|
| F01 / 4,12 | Văn phòng → xưởng → hồ/chợ/trường/dốc linh hoạt → nhà hát/homestay theo đầu mối → Lan → hồ sơ → ga | Prototype chỉ xưởng–Hà–xưởng; thiếu runtime các vùng/điều kiện dữ liệu | Khu thử ID, travel adapter, gate TEST ONLY nếu GDD không chốt | Điều kiện recheck lúc đi, thứ tự linh hoạt, thiếu gate không mở, pause/modal/cancel; CHƯA TRIỂN KHAI |
| F02 / 2,3.6–7,4 | Gặp Bảo/Hà/Mai/Tùng/Lan/Khải/Phương/chủ homestay, đọc nguồn/lời khai | Evidence/Interview prototype có; thiếu dữ liệu và đầu mối toàn tuyến | NPC thay thế; thoại TEST ONLY không canon | Hỏi lặp không thưởng; không lộ câu/đáp án khóa; ND-08/09 chờ; cơ chế cũ đã kiểm, mở rộng nguồn cần kiểm |
| F03 / 5.1–3 | Gán A–Z, thử đọc năm thư và ghi từng thư, không buộc đủ năm | Chỉ thư Hà rút gọn | Năm bản rõ đúng GDD, pose ID kỹ thuật ổn định; glyph/animation thay thế Editor | Sai/trống không grant, không đèn đúng từng chữ, sửa/lặp/Load; CHƯA TRIỂN KHAI toàn bộ |
| F04 / 5.2 | Ghép các cụm hồi đáp và gửi tại kênh Hà đã biết | Chưa có | Bàn thử chọn cụm, điểm gửi TEST ONLY/gate authored | Không gửi khi chưa biết kênh; thứ tự/cụm sai không gửi; không timer thật; CHƯA TRIỂN KHAI |
| F05 / 5.3 | Tra Henri và tự mở ba mức gợi ý không mất điểm | Prototype đã có UI/hints | Hints theo nội dung đã mở, bảng ký hiệu thử | Không grant thư/ending, không tiết lộ thư chưa có; giữ source cũ; consumer toàn tuyến chưa có |
| F06 / 3.7,6.2 | So mép giấy/nháp, không lấy cùng loại giấy làm đủ | Viewer có, thiếu đối chiếu dữ liệu có điều kiện | Asset/pattern và feature key TEST ONLY | Sai mẫu/thiếu quan sát không đạt, lặp không nhân tiến trình; không tự nối draft Hà với Khải; CHƯA TRIỂN KHAI |
| F07 / 3.7,6.3 | Ghép phiếu–rối–bản dịch dựng đường ba thùng | Relation hiện có, thiếu bài đối chiếu nhiều nguồn | Dữ liệu M1…M6 theo GDD, bài thử riêng | Không suy hàng trong thùng từ phiếu đơn lẻ, đủ các nguồn authored mới ghi kết quả; CHƯA TRIỂN KHAI |
| F08 / 3.4,6.4 | R-03/R-12/buổi sửa cho thấy dàn dựng | Evidence/clip/relations có | Viewer/mô tả thật; task TEST ONLY | Không tự nhận hung thủ/ý định từ dàn dựng; cơ chế đối chiếu cần mở rộng |
| F09 / 3.4,6.5,7.4 | Áo/giày thật, mẫu gần giống, vải/đế khác biệt | Viewer/permission prototype có | Quyền truy cập và specimen descriptor authored; proxy không đo vật lý | Không mở mẫu trước permission; màu/cùng loại không đủ; không thao tác pixel; task toàn tuyến chưa có |
| F10 / 3.3–5,7.1,14 | Dựng các giả thuyết, hồi hữu hạn/clear/seek | ReturnGateMechanism/TrialTimeline sáu ca đã có/đã kiểm | Tái dùng cơ cấu thật trong proxy; Map & Asset kiểm hình học/animation | Không đổi luật; rời không chạy bù/trùng record; gate đủ chuỗi cần review nội dung; không chạy lại 103 ca |
| F11 / 7.2,11 | Khám khác ghi, ba loại nguồn và relation giả thuyết | Có Observe/NOTE/viewer, 6 entry/trial pending | Case query chỉ trả ghi nhận mở; nhãn chưa chốt giữ pending | Lặp an toàn, snapshot đọc thuần, không auto true; không làm lại gói cũ |
| F12 / 3.6–7,7.3 | Ghép nguồn M1–M6, P1–P3 và bộ chứng cứ tương đương | Prototype chưa grading | Rubric authored; TEST ONLY không xác nhận forensic | Không chấm bằng số vật; không buộc order hoặc mọi bổ trợ; thiếu rubric reviewed không chốt đánh giá; CHƯA TRIỂN KHAI |
| F13 / 7.3,8 | Bốn kết luận/chưa đủ, confirm trung tính/checkpoint | Prototype có draft/checkpoint, thiếu grading | Dữ liệu lựa chọn chuẩn GDD; equivalent sets/nhận diện specimen chờ review | Không báo đáp án trước chốt, sửa choice/grounds hủy confirm; CHƯA TRIỂN KHAI grading |
| F14 / 8 | C, A-treo, A-sai theo hồ sơ, hậu quả theo loại sai | Chưa có | Trình phát ending thử, không cinematic canon | Sai/thiếu chống đỡ ưu tiên A-sai; không sai+còn bỏ ngỏ→A-treo; đủ bốn→C; sai động cơ không biến thành bắt oan cú ngã; CHƯA TRIỂN KHAI |
| F15 / 5.2,8 | Hẹn ga, hồi đáp, bảo vệ Lan/rối không timer ẩn | Chưa có | Travel/ending state thử; content action theo GDD | Đi chậm/tra bảng không trừ; không đổi sự thật theo ending; CHƯA TRIỂN KHAI |
| F16 / 9,11 | Cầm/xem/cất/đặt ở vị trí hợp lệ, không mất/nhân đôi | Prototype thu hồ sơ, chưa có placement state toàn tuyến | Slot ID, placeholder pick/put; không mô phỏng ném tự do | Không có giới hạn ba vật; đặt sai không đổi state; hủy/Load không mất ID; CHƯA TRIỂN KHAI |
| F17 / 11 | Save/load toàn tuyến, import schema1 rõ ràng | Writer/guards r107 có; thiếu state toàn tuyến | File namespace riêng, import tường minh, profile mới | Không sửa file cũ, không đoán gate/history, null/reference lỗi từ chối trước state; CHƯA TRIỂN KHAI |
| F18 / 9,11 | Pause/modal/Load/hủy giữ input đúng | Prototype có | Adapter thử dùng runtime context, không sửa UI chính | Lệnh bị chặn không đổi state; Load về trạng thái an toàn, không giữ context cũ; mở rộng cần kiểm |
| F19 / 10,14 | Đọc dấu, nghe âm, vật lý/map/camera thật, người mới | Runtime không thay việc sản xuất/kiểm asset | Map & Asset + UI/UX & Tích hợp nhận hợp đồng | CHỜ ASSET-UI / CHƯA KIỂM SCENE CHUNG; người mới NOT_RUN |

### Acceptance theo gói

1. **FULL-CIPHER-01**: đủ năm bản rõ GDD5.2 và 26 pose key ổn định; mapping chung nhưng mỗi thư tự ghi; chưa nhận không query lộ; tra bảng/hint không grant; ghép hồi đáp đúng cụm/thứ tự và kênh được giới thiệu mới gửi. API tests và bảng thử riêng.
2. **FULL-CASE-01**: tiến trình khu/NPC/evidence/ghi/đối chiếu/cầm–đặt thực sự có điều kiện dữ liệu; không tự grant bởi travel/inspect/hỏi lại; dữ liệu TEST ONLY riêng Editor, không tự gắn vào scene chính. Logic hỗ trợ thứ tự linh hoạt và dữ liệu thiếu an toàn.
3. **FULL-CONCLUSION-01**: rubric các bộ căn cứ dữ liệu, gate review chuỗi gây ngã, bốn kết luận và ba ending đúng ưu tiên; confirm gắn nội dung, không lộ verdict trước chốt; checkpoint/IO thất bại không báo thành công. Thiếu rubric phải trả chờ nội dung, không suy thiếu thiết kế thành người chơi sai.
4. **FULL-PERSISTENCE-01**: file toàn tuyến cách ly r107, import schema1 tường minh chỉ những trạng thái có chứng cứ; guard trước gán, giữ bytes và state sau từ chối; save/load mốc chính, hủy/pause và khôi phục checkpoint. Không đụng save gốc.
5. **FULL-PROXY-01**: Editor-only môi trường thử có bảng hành động và các khu ID. Tuyến đi qua lệnh gameplay thật, không nút mở hết/hoàn tất. Fixture cho kiểm biên tách riêng, không là tuyến tự nhiên.

Tất cả kết quả hiện **NOT_RUN** tại lúc lập acceptance. Tiếp theo viết runtime rồi kiểm phần mới; không mở lại toàn bộ save/IO/trial. Các mục A–K vận hành/contract/placeholder/kiểm sẽ được điền bằng tên thật sau triển khai tại chính tài liệu này.
## A. Chạy môi trường thử

Nguồn runtime nền **`f8b50889`** (sáu file C# gồm adapter hội thoại cũ), cộng bản vá producer **`942ca056`**; namespace C# giữ `ConRoi.Investigation.V36.R107`. Nhận cả commit công cụ và tài liệu tiếp sau nguồn, không cherry-pick riêng báo cáo. Không tự gắn runtime này vào `InvestigationSession` đang chạy scene chung: đây là controller toàn tuyến riêng; scene prototype tiếp tục dùng contract cũ cho tới khi phía tích hợp chủ động chuyển consumer.

Unity **6000.6.2f1**, worktree Gameplay riêng. Mở `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`, chọn menu **Con Roi → Gameplay → Toan tuyen TEST ONLY**. Cửa sổ `FullCaseSandboxWindow` chạy trong Editor, không cần Play; không tạo/ghi lại scene. Mỗi lần mở tạo profile GUID riêng tại `Test/PlaytestProfiles/FullCaseSandbox_<GUID>`; đóng cửa sổ không xóa profile. F5 lưu, F6 tải chính profile đang hiển thị; Esc đóng modal, hoặc đảo pause khi không có modal. Khi đang nhập chữ, phím tắt không được bắt. Trở lại từ pause qua nút Tiếp tục/Esc. Không dùng save gốc.

Preview là cube/khu trong `PreviewRenderUtility`, không có traversal FPS/collider gameplay được chứng nhận. Các nút gọi `FullCaseSandboxController` và lệnh runtime thật, không seed chứng cứ. Giữ cửa sổ focus để media/timeline tiến; adapter chỉ gửi delta thấy được, tối đa 0,1 giây mỗi update, không chạy bù thời gian khuất cửa sổ. Không tự xuất hiện trong scene chính/player: toàn bộ catalog, proxy và cửa sổ ở thư mục Editor.

Kiểm độc lập không cần desktop: Unity batch `-executeMethod ConRoi.Gameplay.EditorChecks.FullCaseChecks.RunBatch --full-output <thư-mục-mới> --investigation-save-dir <profile-mới> --full-source <SHA>`. Bộ kiểm mở scene Gameplay nhưng gọi API EditMode; không phải chơi tự nhiên. `FullCaseWindowChecks.RunBatch --full-window-output <thư-mục-mới> --full-source <SHA>` kiểm F5/F6/Esc bằng `EditorWindow.SendEvent`, cần graphics; ghi riêng setup API và event Unity, không gọi input Windows. Không dùng lại thư mục kết quả/profile API cũ.

## B. Ma trận sau triển khai và giới hạn nội dung

Acceptance ban đầu ở trên được giữ làm lịch sử. Bảng này thay trạng thái CHƯA TRIỂN KHAI ở ma trận đầu; các kiểm R/W/D/I bên dưới chạy nguồn `f8b50889`; phần bổ sung bản chép tại `942ca056` có kiểm riêng K.2. TEST ONLY không có nghĩa canon đã hoàn tất.

**Cách đọc bằng chứng:** `R` = run03 của `FullCaseChecks` (nguồn f8b50889); `W` = window01 của `FullCaseWindowChecks`; `D` = kiểm bàn giao sau e71da758, `FullCaseDeliveryChecks` tại 9dd5d6eb; `I` = interview-regression PlayMode. K là danh mục kết quả/hash. Tên assertion/hành động dưới đây chỉ rõ hành vi đã kiểm, không biến tổng số PASS thành độ bao phủ GDD. D đi từ menu tới ga bằng lệnh adapter/API và event Unity; không phải gameplay scene chung.

| Yêu cầu / GDD | Runtime thực tế | Adapter/mô phỏng và loại dữ liệu | Kiểm đã chạy / phần còn thiếu |
|---|---|---|---|
| F01 / 4,12 — mở vùng theo đầu mối | `FullLocationDefinition.requires`, `CanTravel`, `TryTravel`; recheck lúc đi | Window → Controller.Interact(Location); cube ID; **gate TEST ONLY** B.1 | R `unknown_travel_and_gated_location_preserve`, `alternative_lead_order_works`; D travel office→workshop→lake/market/school/slope→office→theatre/homestay/garden→office→station. Thiếu gate kể chuyện được duyệt, scene routing/spawn/collider thật |
| F02 / 2,3.6–7,4 — hỏi các nguồn | `InvestigationInterview.ForSource`, `TryBeginInterview`, `TryAsk`, `Answered`, load-context | Window → Controller.Interact(Actor)/Ask; **tám topic TEST ONLY**, không thoại chính thức, không phát lời khai | R `locked_topic_hides_question_answer`, `<actor>_ask_repeat_no_reward`; I context/điều kiện/repeat; D hỏi tám actor ở tuyến. **Chưa kiểm hội thoại canon/producer lời khai**; các note như M4 hiện là item thử, không phải kết quả nghe Lan thật |
| F03 / 5.1–3 — năm thư, gán/đọc | `FullInvestigationLetters`, `TryAssign`, `GetLetter`, `TryRecordReading` | Text key thay glyph; **plaintext đúng GDD5.2**, pose ID kỹ thuật, không góc/rig canon | R `blank_reading_not_recorded`, `wrong_partial_not_recorded`, `five_letters_decoded`, `26_pose_keys_shared_and_word_separator`; D assign-from-book/read-letter cho năm ID. Bổ sung 942ca056: bản chép có dấu câu/nghĩa Việt từ hành động ghi thành công, kiểm ReadingProducer (K.2). Còn thiếu hình 26 pose/renderer/đọc bằng hình |
| F04 / 5.2 — hồi đáp qua kênh đã biết | `TrySendReply`, `replyChannelRequirements`, cờ replySent | Bốn ô cụm; **reply đúng GDD**, **gate ha:1 TEST ONLY** | R `unreceived_cannot_read_reply`, `reply_wrong_order_denied`, `known_channel_correct_reply`, `reply_repeat_no_duplicate`; D send-reply. Thiếu điểm nhận/trả giấy và lời giới thiệu chính thức; không chứng nhận hoạt cảnh gửi |
| F05 / 5.3 — bảng Henri/gợi ý | `GetHenriLookup` sau xem henri, `GetHints`, `Glossary`; không tự điền | Cửa sổ tra key→chữ; hướng dẫn hint cơ chế, không lời thoại sản phẩm | R `book_lookup_not_available_at_start`, `book_has_shared_26_keys_without_mutation`, `book_snapshot_isolated`, `uncollected_letter_notes_hidden`; D tra sổ rồi gán. Thiếu book/pose thật, kiểm mức trợ giúp có dễ hiểu với người mới |
| F06 / 3.7,6.2 — mép giấy và nháp | `GetComparisons`, `TryCompare`, lưu attempt; `ComparisonAccepted` nội bộ | Task paper, **feature TEST ONLY**; draft Hà không đổi thành paper.khai | R `wrong_comparison_citable_without_premature_verdict`, ending unsupported; D compare paper. Chưa kiểm mini game với hình khớp mép/nháp thật; không coi chuỗi test.feature.match là chứng cứ nhận diện |
| F07 / 3.7,6.3 — ba thùng | Cùng bộ comparison nhiều source/prerequisites | Task cargo: **M2/M5/r12 TEST ONLY**, notes mô tả thử | D collect/inspect/record các nguồn rồi compare cargo; R `alternative_lead_order_works`. Thiếu nội dung ngày/ký hiệu và mẫu rối được duyệt; chưa có bằng chứng người chơi suy ra hàng trong thùng |
| F08 / 3.4,6.4 — dàn dựng | Comparison staging và guard clip exposure | Text state/media thay hình; source r03/r12/clip **TEST ONLY** | R `clip_unseen_record_denied`, `seek_does_not_expose`, `visible_media_unlocks_record`; D phát clip trước ghi, compare staging. Thiếu footage liên tục và hình thật cho đọc quan sát, chưa chứng minh forensic |
| F09 / 3.4,6.5,7.4 — áo/giày/quyền tiếp cận | `TryRequestAccess`, evidence.requires, comparison shoe/fabric | Nút xin test.samples, nguồn/feature thử; không thủ tục pháp lý thật | R `sample_permission_before_Khai_denied`; D request tại office sau gặp khai, thu mẫu tại homestay rồi compare. Thiếu producer truyện cho phép truy cập/mẫu thật gần giống, kiểm zoom/mô tả/thị lực; pixel/đo vải không có trong runtime |
| F10 / 3.3–5,7.1,14 — bàn dựng | `TrialTimeline`, `TryStartTrial`, `TrySeekTrial`, `TryClearHolds`, `Advance`, context/manual pause | Sáu ca hữu hạn dùng cơ cấu đã bàn giao; **luật code giữ nguyên**, text thay hình nộm/animation | R `<trial>_away_pauses_context/manual_pause_stays/global_pause/return_no_catchup/seek_no_record/complete_exactly_one`; D phát release/help/hook+clear qua Tick. Thiếu kiểm hình học/độ hồi bằng asset, trạng thái cánh/chốt nhìn được và gate phản biện GDD14 |
| F11 / 7.2,11 — ghi, nguồn, giả thuyết | Thu/xem/ghi riêng, `TryRecordRelation`, note source/kind; session cũ có `GetProvenanceNotes` per-entry | Full catalog **toàn Unclassified**, text/index thử; kind/source hiện theo evidence, không đồng nghĩa metadata per-entry cũ đã được chuyển | R `collect_is_not_record`, `record_requires_inspection`, `record_repeat_unique_unlocks`, `invalid_observation_denied`; D record sau inspect, hỏi không grant. Thiếu metadata content toàn tuyến/per-entry khi nội dung khác loại; sáu entry/trial pending giữ nguyên. Gameplay nhận mapping/producer sau quyết định, không lấy metadata prototype gán cho câu TEST ONLY |
| F12 / 3.6–7,7.3 — bộ căn cứ tương đương | `FullSupportSet.alternatives`, `MeetsIn`, `Assess`, review gates | **Rubric TEST ONLY B.3**, không truth/forensic đã duyệt | R `alternative-set_priority`, `unsupported_priority`, `unreviewed_content_fails_closed`; D draft dùng bộ thử đã thu/ghi. Chưa có rubric canon đủ chuỗi, không buộc mọi bổ trợ hoặc phán bằng số vật |
| F13 / 7.3,8 — chốt có căn cứ | `TrySetConclusion`, `TryConfirm`, `Confirmed`, `TryFinalize`, checkpoint | Lựa chọn test.gdd-claim/test.alternative-claim, **không câu kết luận chính thức** | R `draft_edit_invalidates_confirmation`, `complete_neutral_before_confirm`, `finalize_locked_destination_rejected`, `recovery_checkpoint_loads`; D no-verdict/confirm/finalize/checkpoint. Thiếu câu hiển thị và review đủ chuỗi; confirm không chứng nhận đúng |
| F14 / 8 — C/A-treo/A-sai | `Assess`, `GetEnding`, UnsupportedDimensions/FallAccusationUnsupported; ưu tiên sai→open→complete | Enum/text presenter **ending mô phỏng**, rubric thử | R complete/open/wrong-sender/wrong-motive/wrong-fall/unsupported_priority và _fall_accusation_scope; D simulated complete. Chưa có ending sản phẩm/hoạt cảnh, nghiệp vụ/hậu quả được duyệt; không tuyên đã duyệt ending C |
| F15 / 5.2,8 — cuộc hẹn/bảo vệ | Reply flag, gate station theo Finalized, không clock lịch thực; ending state | Travel/text thử; **chưa có state thao tác giao nhận/bảo vệ hoặc cinematic** | R `<variant>_station_route`, _station_available_after_finalize; D station_persisted. Không có timer ẩn trong đường này; lời “Lan/rối được bảo vệ” hiện là presenter thử, chưa kiểm hành động sản phẩm; thiết kế dàn cảnh/UI nhận |
| F16 / 9,11 — cầm/cất/đặt | `TryHold`, `TryStow`, `TryPlace`, placement/slot/held | Nút thay thao tác world, **slot thử B.2** | R `invalid_place_keeps_item`, `valid_place_keeps_single_record`, `fixed_evidence_cannot_hold`; đường save/load state mới. Thiếu hand visual/world slot/collider và kiểm input thật; không có ném/ragdoll |
| F17 / 11 — save/checkpoint | `FullCaseSave`, `TrySave`, `TryLoad`, `TryImportPrototype`, validator/writer | Profile GUID Editor, namespace fullcase; **không đồng bộ r107** | R null/missing/reference/index/schema/catalog/duplicate/JSON giữ state/bytes và recover; explicit import; D F5/F6 workshop/station/checkpoint. Xem bảng F.1 về phần không nhập; thiếu profile/menu consumer sản phẩm, migration catalog canon |
| F18 / 9,11 — lifecycle/input | `Modal`, `Paused`, `CanMove`, `Cancel`, `ResetTransient` | Window/Controller context và key event Unity; **không FPS controller** | R proxy_modal_input/proxy_cancel các modal, unfocused tick/return; W F5/F6/Esc/pause/JSON/recover; D thêm F5/F6 trên tuyến. Thiếu raycast/cursor/focus và P/E/modal trong scene chung; không suy CanMove=true thành WASD đã kiểm |
| F19 / 10,14 — asset/đọc dấu/người mới | Contract viewer/controls cũ; query state không thay asset | Preview primitive và text; không đo thực địa hoặc footage | D menu mở/cách ly source player/scene dependency, scene bytes không đổi. Render readability/âm thanh/vật lý/scene chung/UI1080p/người mới **NOT_RUN** |

**Mức hoàn thành hiện tại:** lần đóng gói69e9ca57 chưa phát hiện producer bản chép còn thiếu; gói942ca056 đã bổ sung và kiểm riêng như K.2, không giữ kết luận “mọi cơ chế đã đủ” của lần trước. Những dòng “thiếu” trên là dữ liệu/gate/rubric cần được duyệt, producer sau quyết định, presentation/world adapter hoặc kiểm asset/tích hợp. Không đánh dấu chúng đã hoàn tất chỉ vì API chạy. Không có vụ thứ hai/ending mới.

### B.1. Giá trị gate/catalog tạm — phải thay ở đâu

Toàn bộ factory ở `Assets/ConRoi/Modules/Gameplay/Editor/FullCaseTestCatalog.cs`; đây là **TEST ONLY**, không phải file nội dung sản phẩm. `Create(bool reviewedTestRubric=true)` tạo `FullCaseCatalog {id="test.only.fullcase",revision="1",testOnly=true,startLocation="office"}`. Runtime copy/hash catalog lúc constructor; đổi nội dung catalog cần phiên mới và xem F.1, không sửa object factory rồi mong phiên đang chạy tự đổi.

Ký hiệu dưới đây: `N(id,i)` = Recorded(id,index=i); `C(id)` = Collected; `D(id)` = Decoded; `Q(id)` = Comparison; `T(id)` = TrialRecorded. Tất cả prerequisite trong một mảng là AND.

| Giá trị thực đang dùng | Căn cứ/nhãn giả định | Điểm thay trong data | Ca kiểm cần chạy lại sau thay |
|---|---|---|---|
| office/workshop không requires; lake/market/school/slope cần N(slide,0) | GDD4 có thứ tự vùng nhưng không chốt index mở bốn nơi; **giả định thử** | `locations[].requires/startLocation` | bắt đầu sạch, khám chưa ghi/ghi, tới vùng sớm, hai thứ tự tuyến, modal/pause/Load |
| theatre cần C(lead.theatre); homestay C(lead.homestay); garden N(M3,0) | Lead thử ở school/slope; GDD không chốt producer/index cụ thể | locations[].requires + evidence của hai lead | nguồn lead mới thực sự tạo được bằng thao tác; hỏi/thu/ghi lặp, không mở sớm, thứ tự/Load |
| station chỉ cần Finalized("",-1) | **Không cần replySent/đọc thư Lan trong catalog thử**; không suy đây là gate canon | locations[id=station].requires; nếu có kiểu requirement chưa hỗ trợ giao Gameplay | đủ/thiếu điều kiện hẹn đã duyệt, sau finalize, load/checkpoint và không timer |
| test.samples tại office cần C(khai); coat/shoe/samples cần Permission(test.samples) | Giả định bước Phương rút gọn; không tên thủ tục chính thức | permissions[].location/requires; evidence[].requires | trước/sau gặp Khải, vị trí/modal, thu mẫu chưa phép/đã phép, lặp và save/load |
| replyChannelRequirements=[N(ha,1)] | Nội dung kênh có ở GDD5.2, **producer/index cụ thể đang giả định** | replyChannelRequirements + source ha:1 | chưa biết/biết kênh, repeat, cụm sai/đúng và Load; không grant từ mở panel |
| M1/theatre, M2/market, M3+M5/school, M4/garden, M6+paper.khai+log.homestay/homestay; thư Bảo/lake, Mai/school, Tùng/slope, Lan/garden | Địa điểm từ GDD4/3.7; **text/source/observations thử**, không bản ghi chứng cứ chính thức | evidence[].location/text/source/observations/requires | thu/xem/ghi đúng nơi, chưa mở không lộ, thứ tự và index/save compatibility |
| Legacy ha/letter/receipt/draft/decoded ở market; samples/coat/shoe ở homestay; henri ở theatre; các legacy khác ở workshop | **Chỉ giữ ID**, không mang nguyên câu chữ/metadata prototype sang fullcase. Trước 942ca056, `decoded` là item text thử có thể thu trước giải. Nay GetLocalEvidence loại alias đó, TryCollect trả derived-reading; bản chép chính thức phải query từ D(letterId), không từ raw alias cũ | factory vòng `InvestigationContent.Evidence` + catalog sản phẩm được duyệt | source/producer chính thức, guard clip/decoded, query chưa mở và Load. Cần thay producer/requirement của bản đọc sản phẩm sau thiết kế; không coi item thử là đã giải |
| `Topics()` tám actor; ID test.full.actor/topic.<bao,ha,mai,tung,lan,khai,phuong,host>; câu hỏi/đáp án luôn TEST ONLY | ND-08/09 chưa chốt; không lời thoại chính thức | approved `InterviewTopicDefinition[]` truyền `ForSource` | locked/wrong/repeat/no Answer leak/Answered unsub/Load context, rewards chỉ nếu có producer được giao |
| Gate topic bao=N(letter.bao,0), ha=N(ha,0), mai=N(M3,0), tung=N(letter.tung,0), lan=N(M4,0), khai/phuong=N(khai,0), host=N(log.homestay,0) | **Giả định kiểm điều kiện**, không gate truyện | `Topics()`/catalog chính thức; hiện hai InterviewRequirementKind AcquiredEvidence/RecordedObservation | hỏi trước/sau ghi đúng entry, repeat, topic khác người, save/load và no progress grant |

### B.2. Đối chiếu, placement, media tạm

Mỗi task có options **`test.feature.match` / `test.feature.near`**, expectedOption=`test.feature.match`. Đây là đáp án kiểm máy cho dữ liệu thử, **không thuật toán nhận diện hình hoặc rubric forensic đã duyệt**. Thay bằng feature IDs, source IDs và observations được chủ nội dung/asset duyệt trong `FullComparisonDefinition {evidenceIds,requires,options,expectedOption}`. Presenter không được hiện expectedOption.

| Task | evidenceIds | requires hiện tại (AND) |
|---|---|---|
| paper | letter.bao, paper.khai | N(letter.bao,0), N(paper.khai,0) |
| cargo | M2, M5, r12 | N(M2,0), N(M5,0), N(r12,0) |
| staging | r03, r12, clip | N(r03,0), N(r12,0), N(clip,0) |
| shoe | shoeprint, shoe | N(shoeprint,0), N(shoe,0) |
| fabric | fabric, coat | N(fabric,0), N(coat,0) |
| counter | mechanism, hook, clip | N(mechanism,0), N(hook,0), N(clip,1), T(release), T(help), T(hook/clear-holds) |

Sau thay task: kiểm nguồn thiếu/nhầm/gần giống, inspect chưa ghi, lặp, sửa lựa chọn, không grade trước chốt, support sets bị ảnh hưởng và save/catalog. Không chạy lại IO không liên quan. `counter` không phải cờ chứng minh đã loại mọi giả thuyết thật: gate GDD14 vẫn cần review.

`portable=false` cho slide/shoeprint/mechanism/hook/khai/ha, các item khác portable=true; item portable có đúng một `placementSlots` là `<nơi-gốc>.table`. Vì vậy nút “đặt bàn thử tại nơi này” ở **nơi khác** có thể bị từ chối đúng luật slot; không có cơ chế tự sinh socket ở mọi nơi. Placement source/inventory/held là state key, không Transform. Map & Asset + UI/UX & Tích hợp thay danh sách slot/bind Transform qua adapter, kiểm cầm–cất–đặt hợp lệ/sai/đổi khu/Load/không nhân ID.

Clip minh họa dùng `MechanismPresentation.ClipLength`; exposure trước là giao khoảng [0.5,3.8] đủ 0.6 giây, trong sự cố [4,6.2] đủ 0.8 giây, chỉ tính visible playback. Đây là rule code kế thừa, **không xác nhận footage thật đọc được**; không đổi mốc chỉ để ép kiểm đạt. Thay asset phải cùng timeline/quan sát, nếu footage lệch thì giao Gameplay điều chỉnh rule có kiểm, không tự set clipSeen. Trial IDs release/help/hold/hook/wedge/after đi qua TrialTimeline; clear-holds chỉ sau LastAction và có điều kiện giữ. Không thay luật hồi hoặc lấy text report làm bằng chứng hình.

### B.3. Rubric và ending mô phỏng — chưa duyệt nội dung

Mỗi `FullConclusionRule` có claims=[`test.gdd-claim`,`test.alternative-claim`], correctClaim=`test.gdd-claim`. `Create()` mặc định **reviewed=true, fallChainReviewed=true chỉ cho TEST ONLY**, `Create(false)` kiểm fail-closed. Những bool này **không phải biên nhận review**; catalog sản phẩm phải dùng kết quả duyệt thật. Lựa chọn rỗng là chưa đủ. Các bộ dưới đây do factory đặt để kiểm thuật toán, không được coi là bộ chứng cứ tối thiểu canon:

| Rule | alternatives đang dùng |
|---|---|
| sender | Q(paper)+N(paper.khai,0)+D(letter.bao); hoặc cùng bộ nhưng D(letter) |
| presence | N(slide,0)+Q(shoe)+Q(staging) |
| fall | N(mechanism,0)+N(hook,0)+N(clip,0)+N(clip,1)+Q(fabric)+Q(shoe)+Q(staging)+Q(counter) |
| motive | N(M1,0)+N(M3,0)+N(M5,0)+N(M6,0) |

Thay ở `conclusions[].claims/correctClaim/alternatives/reviewed`, `fallChainReviewed`, cùng catalog feature/source liên quan. GDD7.3/8 chốt **quy tắc ưu tiên** chứ chưa xác nhận các mảng thử là đủ: sai/không support→Unsupported, không sai+còn rỗng→Open, bốn mục được support→Complete. Tên enum được presenter thử gắn với C/A-treo/A-sai; **chưa có ending sản phẩm được duyệt**. Sau nhận rubric, Gameplay chạy lại từng bộ tương đương, thiếu một mắt xích, counter chưa giải, wrong-sender/motive/fall, open, confirmation/checkpoint và catalog/save compatibility. UI/UX & Tích hợp chịu trách nhiệm lựa chọn/câu kết/hậu quả đúng phạm vi, Map & Asset footage/cơ cấu; không có tự phát bản án/bắt oan cho lỗi động cơ.

## C. Tuyến mô phỏng thực hiện theo thứ tự

1. Văn phòng → xưởng; Khám/thu `slide`, mở xem, Ghi mục 0. Chỉ khám hoặc xem chưa mở bốn đầu mối thử. Khám từng nguồn khác; ghi đúng từng mục sau xem, hủy trước khi thu nguồn kế.
2. `clip`: thu → Đoạn quay → Phát, xem qua cả khoảng trước và trong sự cố → mở hồ sơ clip và ghi. Tua thẳng đến cuối không thay việc xem hai khoảng. Preview dùng state/text minh họa; không thay footage GDD14.
3. Đi hồ/chợ/trường/dốc theo thứ tự tùy chọn. Thu/xem/ghi từng thư và nguồn tại đó; có `lead.theatre`, `lead.homestay` thì mở khu tương ứng. Các key gate này chỉ thử, chưa là lời khai sản phẩm. Hỏi NPC qua catalog thử không tự thu/ghi/phát thưởng.
4. Về văn phòng, mở hội thoại Phương, xin `test.samples` sau khi đã khám mục `khai`; đến homestay thu mẫu áo/giày. Nếu chưa được phép, chúng không nằm trong query nguồn mở. Không mô phỏng quyền pháp lý thật.
5. Nhà hát: `henri` và M1. Homestay: nháp riêng `paper.khai`, log và M6. Trường: M3/M5. Sau M3 thử, vườn mở M4 và thư Lan. Gate vườn này cần nội dung chốt, không mặc định là cốt truyện chính thức.
6. Mật mã: chọn thư đã xem, gán 26 key/chữ theo quan sát hoặc tra sổ Henri đã mở; ghi bản đọc từng thư. Không cần đủ năm mới giải một thư. Gợi ý/glossary không cấp tiến trình. Ghép bốn cụm hồi đáp đã hiểu theo GDD5.2; kênh thử cần `ha:1` đã ghi. Không đồng hồ ẩn.
7. Đối chiếu: chọn task đang mở, nguồn đã xem và feature option; ghi kết quả thử. `test.feature.match/near` là lựa chọn dữ liệu, không đèn đúng/sai hoặc đo ảnh. Sai vẫn là attempt để người chơi trích dẫn; không nâng sai thành proof.
8. Bàn dựng: release/help/hold/hook/wedge/after dùng cơ cấu thật; seek chỉ đổi playback không tạo record; bỏ giữ sau LastAction khi có lực/móc/chèn. Rời trang/focus chỉ dừng theo ngữ cảnh; pause tay vẫn giữ. Chạy đủ tạo record với lời nhắc mô hình không xác định ai/ý định.
9. Hồ sơ: bốn mục sender/presence/fall/motive, lựa chọn và căn cứ từ những gì đã ghi. Để trống nghĩa chưa đủ. Xác nhận trung tính rồi chốt; sửa draft/căn cứ/context làm confirm cũ hết hiệu lực. Lỗi ghi không báo chốt thành công.
10. Rubric thử đủ→Complete (C), còn bỏ ngỏ không sai→Open (A-treo), có khẳng định sai/thiếu support→Unsupported (A-sai), kể cả khi mục khác bỏ ngỏ. Sai động cơ/người gửi không tự báo bắt oan cú ngã. Đi ga sau chốt; màn ending chỉ là text/state thử. Load checkpoint để xem lại draft, không sửa record/ending cũ tại chỗ. Lưu/tải các mốc bằng profile cửa sổ.

Route kiểm tự động dùng chính các lệnh trên, chọn lời giải biết trước từ catalog thử; không seed trực tiếp evidence/progress, nhưng vẫn là **API action route**, không chứng minh người mới tự giải hoặc traversal liên tục.

## D. Bảng placeholder duy nhất

Tất cả file thử dưới `Assets/ConRoi/Modules/Gameplay/Editor/`; không có prefab/scene sản phẩm mới.

| ID | Chức năng / file thử | Runtime thật đã nối | Đối tượng thật cần cung cấp | Chủ nhận / trạng thái |
|---|---|---|---|---|
| office,workshop,lake,market,school,slope,theatre,homestay,garden,station | Cube anchors, `FullCaseSandbox.cs` / scene Gameplay rỗng | `TryTravel`, `CanTravel` | Vùng/travel interaction, điểm vào/ra; gate được duyệt | Map & Asset + UI/UX & Tích hợp; CHỜ |
| ID registry prototype; letter.bao/mai/tung/lan, lead.theatre/homestay, M1…M6, paper.khai, log.homestay | Cube nhỏ + `FullCaseTestCatalog.cs` | `TryCollect`, `TryInspect`, `TryRecordObservation` | Visual/source/observation authored; không đổi ID cũ hoặc source draft Hà | Map & Asset + UI/UX & Tích hợp; CHỜ NỘI DUNG/ASSET |
| test.full.actor.bao/ha/mai/tung/lan/khai/phuong/host | Chọn actor trong bảng thử; chưa có NPC rig proxy | `InvestigationInterview.ForSource`, `TryBeginInterview`, `TryAsk` | NPC thật, participant/topic ID/gate/thoại đã duyệt | UI/UX & Tích hợp + Map & Asset; TEST ONLY |
| pose.01…pose.26, word-break | Text key, `FullInvestigationLetters` + cửa sổ | `GetLetterPoses`, `TryAssign`, `GetHenriLookup` | 26 pose/rig/glyph dùng cùng key; dấu giữa dòng | Map & Asset; UI/UX & Tích hợp renderer; CHỜ |
| clip, release/help/hold/hook/wedge/after | Text `MechanismPresentation`, playback thử | `TryPlayMedia`, `TrySeekMedia`, `TryStartTrial`, `TrySeekTrial`, `TryClearHolds`, `Advance` | Clip liên tục, model cơ cấu/hình nộm/state presentation | Map & Asset; CHỜ REVIEW GDD14 |
| paper,cargo,staging,shoe,fabric,counter | Toggle nguồn/feature TEST ONLY | `TryCompare`, `GetComparisons` | Mẫu thật/feature IDs, rubric nguồn và quan sát được duyệt | Map & Asset + UI/UX & Tích hợp; CHỜ |
| test.samples | Nút permission thử | `TryRequestAccess`, `HasPermission` | Bước truyện cho truy cập mẫu do Phương liên lạc | UI/UX & Tích hợp; CHỜ |
| <location>.table, source, inventory, held | Nút cầm/cất/đặt; placement state | `TryHold`, `TryStow`, `TryPlace`, `HeldEvidenceId` | Socket/field binding vị trí hợp lệ, hand preview | Map & Asset + UI/UX & Tích hợp; CHỜ |
| sender,presence,fall,motive; test.gdd-claim/test.alternative-claim | Lựa chọn/căn cứ/text ending thử | `TrySetConclusion`, `TryConfirm`, `TryFinalize`, `GetEnding` | Lựa chọn tiếng Việt, rubric/set tương đương, ending presenter | UI/UX & Tích hợp; CHỜ NỘI DUNG |
| FullCaseSandbox_<GUID> | Profile/cửa sổ Editor TEST ONLY | `TrySave`, `TryLoad`, checkpoint, import chủ động | Profile resolver/menu sản phẩm, feedback/restore theo thiết kế | UI/UX & Tích hợp; CHỜ |

`FullCasePlaceholder` có field `StableId`, `LocationId`, `Kind`; `Interact(adapter)` đi tới adapter `Interact(kind,id,location)`. Kind Location gọi travel; Evidence kiểm location trước collect; Actor mở interview rồi Conversation. Các cube preview chưa được raycast bằng controller; nút chọn gọi cùng adapter. Không dùng tên object/Hierarchy/vị trí cube làm điều kiện.

## E. Hướng dẫn Map & Asset

Làm vùng cần thiết trong GDD4, giữ map/checkpoint cũ. Địa hình thử cách nhau 5 đơn vị chỉ để xem preview, **không phải kích thước level**. Chưa có thông số mới được duyệt về kích thước/pivot/socket/layer/tag; không biến cube thành chuẩn sản xuất. Dùng tỷ lệ Unity hiện hành và bàn giao transform/collider thật để kiểm lại với controller hiện có; hình phải khớp dấu đọc, không chỉ màu.

- Model ID r03/r12 phải khác, dây dàn dựng không lẫn dây sửa. Cơ cấu giữ cánh L/chốt C/dây D/móc M và luật `ReturnGateMechanism`/`TrialTimeline` có sẵn; không bake logic vào animation. Bind presentation từ `TrialState` (`GateState`) và thời gian hiện tại, không dùng frame animation để cấp evidence.
- Clip phải cho đọc chốt/cánh/hai tay trong chính sự kiện, không nhận diện người kéo. Text mô phỏng chưa chứng minh footage đủ. Cần kiểm GDD14 với hình thật trước đánh dấu `fallChainReviewed`.
- Mẫu giấy khớp mép/nháp đúng nguồn; áo/mảnh vải và giày/dấu có mẫu gần giống nhưng không khớp; model/mô tả cho zoom/đổi góc. Không coi cùng màu/cùng loại là match. Bàn giao feature IDs/rubric cho phía nội dung duyệt.
- Pose paper/puppet/book cùng `pose.01…26`; `word-break` là dấu chấm giữa dòng, không pose thứ 27. Góc cụ thể cần art sản xuất/kiểm đọc; không có góc/rig canon được suy từ mã số.
- Cầm/đặt: world adapter map slot ID tới Transform, tách record tồn tại và visual đang cầm. Trước gọi `TryPlace`, chọn slot có trong `FullEvidenceDefinition.placementSlots`; chỉ di chuyển visual sau Success. Cất/Load không destroy ID hoặc tạo vật chứng thứ hai.

## F. API/state/event và wiring UI thực tế

Các type/field catalog ở `Assets/Scripts/InvestigationV36R107/FullInvestigationData.cs`; code các lệnh ở `FullInvestigationRuntime.cs`, persistence ở `FullInvestigationPersistence.cs`; ví dụ thử ở `FullCaseTestCatalog.cs` và `FullCaseSandbox.cs`. Không có global singleton/framework mới.

1. Tạo **một** `new FullInvestigationRuntime(approvedCatalog)` cho phiên. `FullCaseCatalog` gồm `id,revision,startLocation,testOnly,locations,evidence,comparisons,permissions,conclusions,replyChannelRequirements,fallChainReviewed`. Catalog được copy/validate, ID trùng/requirement sai/vòng comparison-permission bị từ chối lúc tạo. Không đưa `FullCaseTestCatalog.Create()` vào sản phẩm. Những ID mới ngoài registry prototype hiện chỉ là key thử; phải duyệt trước khi xuất catalog sản phẩm.
2. `FullRequirement {kind,id,index=-1}` hỗ trợ Collected, Recorded (index bắt buộc hợp lệ), Comparison, Decoded, Permission, Finalized (id="", index=-1), TrialRecorded (sáu id hoặc hậu tố `/clear-holds`). `requires` là AND. `FullSupportSet[] alternatives` là OR các bộ AND; không chấm số vật. Không sửa catalog trong phiên: snapshot/hash giữ nhất quán.
3. Subscribe `runtime.Changed += Refresh`; gọi Refresh ban đầu. OnDisable/đổi phiên: `runtime.Changed -= Refresh`. Event không có payload, báo tiến trình/context, listener lỗi bị cách ly và lệnh mutation reentry bị chặn. **Không phát mỗi frame media/trial**: presenter lấy `TrialTime/TrialState/MediaTime` khi render. `LastMessage` được cập nhật theo kết quả lệnh; render ngay từ `FullActionResult.Message`, không chỉ chờ Changed vì lỗi không bắt buộc phát event.
4. `GetLocations()` trả id/label; `CanTravel(id)` chỉ phản ánh prerequisite, không thay kiểm pause/modal của `TryTravel(id)`. `GetLocalEvidence()` trả id/label/location/portable của nguồn mở ở nơi hiện tại. `TryCollect(id)` không tự ghi. Sau `TryInspect(id)`, đọc `GetDescription(id)`, nhãn `GetNoteKind/GetNoteSource(id)`, `GetRecordedNotes(id)`; UI lấy danh sách observation phù hợp từ catalog được duyệt, gọi `TryRecordObservation(id,index)` trên ý định ghi. `HasEvidence/HasRecordedObservation` dành gate; unknown trả false. Pending metadata hiển thị Chưa phân loại, không đổi thành Quan sát.
5. `SetModal(FullCaseModal)` và `SetPaused(bool)` là context runtime; `CanMove` → world movement/look input. Adapter sản phẩm chịu cursor/focus/drag/timeScale, không tự dựa tên panel. Mở panel xong mới gửi thao tác đúng modal; đóng gọi `Cancel()`. Media/trial chỉ chạy khi modal đúng, chưa pause và adapter gửi visible delta. Hủy không xóa tiến trình/không đổi pause tay trial. Load thành công reset transient về None/not paused, các bản trial đã ghi vẫn giữ; không resume timeline cũ từ elapsed wall-clock.
6. `TryHold(id)`, `TryStow()`, `TryPlace(slot)` trả lỗi rõ, world visual chỉ đổi sau Success. Có `HeldEvidenceId` và `GetRecords()` (bản copy gồm viewed/observations/placement/slot). Không hardcode 3 vật.
7. Mật mã: chỉ gọi `GetLetter(id)`/`GetLetterPoses(id)` sau player đã mở; GetLetter nullable và chưa có thư không lộ. `TryAssign(poseKey,letter)` một A–Z/rỗng; `TryRecordReading(id)` mới ghi bản đọc. Từ 942ca056, consumer lấy snapshot chuẩn GDD bằng `TryGetRecordedReading(id,out FullRecordedReading)` hoặc `GetRecordedReadings()`; không dùng raw evidence ID decoded để cấp/hiện bản chép mới. `GetHenriLookup()` chỉ sau xem henri, trả bảng copy, không điền mapping. `GetHints(id,1..3)` chỉ nội dung thư mở; `FullInvestigationLetters.Glossary` tra bất kỳ lúc nào. `TrySendReply(string[] orderedPhrases)` kiểm kênh và thứ tự, không timer/thưởng nhân đôi.
8. Đối chiếu: `GetComparisons()` chỉ task mở, trả options/source IDs; `TryCompare(taskId,evidenceIds,option)` giữ attempt dù lựa chọn không đủ support. `TryRecordRelation(first,second,note)` là giả thuyết, không proof. Viewer hai vật thật tiếp tục nhận controls qua contract [COMPARE](../Tasks/N2/COMPARE-VIEW-01/README.md); không feed cùng delta cả viewer/controller.
9. Hội thoại: `InvestigationInterview.ForSource(runtime,approvedTopics)`, `TryBeginInterview(participantId,out error)`, `GetTopics/GetAvailableTopics`, `TryAsk(topicId,out InterviewReply,out error)`, `EndInterview()`. `Answered += OnAnswer`, hủy `-= OnAnswer`; reply không phát chứng cứ. Adapter modal/query cũ không đổi constructor `new InvestigationInterview(session,topics)`. `DataContext` đổi sau Load làm context cũ hết hạn. Nội dung chính thức/producer ghi lời khai chờ quyết định; không lưu lén topic qua relation.
10. Kết luận: `GetConclusionChoices`, `GetConclusions`, `GetAvailableGrounds` chỉ căn cứ đã ghi, không kết quả chấm. `TrySetConclusion(id,claim,FullRequirement[])`, claim="" là chưa đủ; rồi `TryConfirm()`, `Confirmed`, `TryFinalize(profile)`. Không hiển thị `correctClaim/expectedOption` hoặc support rubric nội bộ. `GetEnding()` trước chốt None, sau chốt có `Ending`, `UnsupportedDimensions`, `FallAccusationUnsupported`; chỉ loại sai fall được phép có hậu quả tương ứng về cú ngã. Kết thúc chưa phải bản án.

Ví dụ adapter nguồn (catalog/content do chủ nội dung cung cấp):

```csharp
var game = new FullInvestigationRuntime(approvedCatalog);
game.Changed += Refresh;
var result = game.TryTravel(destinationId);
ShowFeedback(result.Message); // cả lỗi và thành công; không parse chuỗi để quyết định gameplay
if (result.Success) Refresh();
// Panel bên cạnh không tự cấp observation hoặc set state.
game.SetModal(FullCaseModal.Comparison);
var compared = game.TryCompare(taskId, selectedEvidenceIds, selectedFeatureId);
ShowFeedback(compared.Message);
// Khi tháo adapter: game.Changed -= Refresh; interview.Answered -= OnAnswer;
```

`FullActionResult` có `Success`, `Code`, `Message`, factory `Ok/Fail`; lỗi điển hình locked/modal/paused/not-collected/unseen/mapping/reading/channel/comparison/confirmation/content-pending/write/read/json/validation/missing/import/not-empty. UI hiển thị Message, không tự nhân đôi validator hoặc giữ thông báo lỗi sau lần thành công. Danh sách khóa/trống: query rỗng/nullable, không giả đáp án; Success mới cập nhật feedback thành công. Lệnh bị chặn không tự đóng modal/chuyển vùng.

### Save toàn tuyến và tương thích

Không đổi `InvestigationSave` schema1/namespace r107 hoặc guard gói trước. State mới ở `FullCaseSave` schema1, namespace **`conroi.investigation.v36.fullcase`**: location, evidence/viewed/observations/placement/held, mapping, decoded, permissions, comparisons, relations, trial reports, clip exposure, conclusions/grounds, finalized/ending. Không có per-topic history. Đây là namespace mới có mô hình lưu tường minh, không tăng schema r107 và không tự đoán tiến trình.

`TrySave(profile)`, `TryLoad(profile,bool checkpoint=false)`; path thật qua `SavePathFor(profile)` = `<profile>/InvestigationV36/fullcase/case.json`, `CheckpointPathFor(profile)` = `case.before-conclusion.json`. Dùng writer `InvestigationSaveFile.Write` đã bàn giao, không tự coi Copy-overwrite là atomic. Chốt: checkpoint draft trước, main đã finalized sau; chỉ đổi live state sau main ghi xong. Nếu main lỗi, checkpoint mới có thể đã tồn tại nhưng main/live không coi chốt xong; không tự fallback/load checkpoint.

Load kiểm schema/namespace/catalogHash, array/null/duplicate JSON/reference/index/grounds/ending trước gán; thất bại giữ state và bytes, kết quả có lý do không lộ exception. Catalog hash gồm revision và nội dung: catalog đổi (kể cả text) bị từ chối thay vì âm thầm gán index sai; **chuyển TEST ONLY→catalog sản phẩm cần profile mới hoặc kế hoạch migration tường minh sau duyệt nội dung**, không hứa tải chéo. Save cũ r107 tiếp tục tải bằng session cũ; muốn chuyển có `TryImportPrototype(sourcePath)` vào phiên toàn tuyến trống. Chỉ nhập ID/observations/mapping/relations/trials/clip/gate đã có, không đoán permission/location/decoded/history/kết luận; tệp nguồn không bị ghi lại. Import không phải migration tự động hoặc chứng minh mọi profile lịch sử.


### F.1. Hai luồng API/save — không nối nhầm

| Điểm nối | Prototype r107 hiện có | Toàn tuyến mới |
|---|---|---|
| Chủ phiên | `InvestigationSession` MonoBehaviour/partial trong scene cũ | `FullInvestigationRuntime(approvedCatalog)` object do adapter phiên tạo |
| Khám/ghi | `Acquire`, `OpenEvidence`, `Observe` | `TryCollect`, `TryInspect`, `TryRecordObservation` |
| Mật mã | `Assign`, `ReadCipher`, `RecordReading` | `TryAssign`, `GetLetter`, `TryRecordReading` |
| Context | `OpenPage/ClosePage`, session `Modal/Paused` và controller đã nối | `SetModal/Cancel/SetPaused`, `CanMove`; consumer mới phải nối movement/look/cursor |
| Hội thoại | `new InvestigationInterview(session,topics)` | `InvestigationInterview.ForSource(fullRuntime,topics)`; không dùng session cũ làm nguồn cho topic fullcase |
| Save main | `session.SavePath`: `<profile>/InvestigationV36/r107/prototype.json` | `FullInvestigationRuntime.SavePathFor(profile)`: `<profile>/InvestigationV36/fullcase/case.json` |
| Checkpoint | `BeforeConclusionSavePath`: cùng folder r107, `prototype.before-conclusion.json`; `LoadConclusionCheckpoint()` | `CheckpointPathFor(profile)`: cùng folder fullcase, `case.before-conclusion.json`; `TryLoad(profile,true)` |
| Save/Load | `session.Save()`, `session.Load()` dùng profile của session | `fullRuntime.TrySave(profile)`, `TryLoad(profile,false)`; **không** tự đọc profile override của session |
| Schema/namespace | schema1, conroi.investigation.v36.r107 | schema1 riêng, conroi.investigation.v36.fullcase + catalogHash; số schema giống không có nghĩa cùng format |

**Chọn profile:** prototype lấy `SaveDirectoryOverride` hoặc CLI `--investigation-save-dir` lúc Awake; nếu không có, rơi về Application.persistentDataPath. Với thử bắt buộc chỉ định profile riêng trước chạy, đừng để rơi về profile gốc. Toàn tuyến nhận profile **tham số từng lệnh**, không có fallback persistentDataPath; cửa sổ tự tạo GUID và hiện path. CLI `--investigation-save-dir` của runner API không đổi profile cửa sổ Editor. Trong cùng cửa sổ, F5/F6/khôi phục dùng duy nhất `adapter.Profile` hiện tại. Đóng/mở lại tạo profile trống mới; **hiện chưa có UI chọn lại profile cũ**, muốn tiếp tục qua API tạo Controller với đúng path đã ghi hoặc phía tích hợp làm profile picker được duyệt. Không sửa path trong save, không đổi tên/copy file r107 thành case.json.

Hai phiên **không live-sync, không dual-write, không tự merge/fallback** dù dùng chung root profile. Không nối một nút F5 tới cả session.Save và fullRuntime.TrySave. UI phải biết mode/controller đang sở hữu session; trước Load ghi nhận loại main/checkpoint, profile root, resolved path, namespace/schema và catalog revision; Success mới refresh phiên đúng nguồn. Lỗi namespace/catalog/thiếu tệp giữ phiên hiện tại, không thử tìm file khác “cho chạy được”. Không đề xuất tự hợp nhất hai hệ.

| Dữ liệu | Quan hệ r107→fullcase bằng `TryImportPrototype(sourcePath)` |
|---|---|
| ID/observations | Kiểm save r107, ID phải biểu diễn được trong catalog; copy indices, đặt `viewed=false` yêu cầu mở lại. `collected` cũ không còn một field fullcase; imported records tồn tại. Không suy loại/nguồn mới từ câu chữ |
| mapping/relations/trials/clip exposure/gate | Có copy explicit; pose chữ cũ đổi key pose.xx theo bảng. Relation/trial vẫn nội dung đã lưu, không tự chứng minh rubric mới |
| placement/held/world transform | **Không copy vị trí/held/player pose cũ**: portable→inventory, fixed→nơi catalog/source; fullcase bắt đầu startLocation. Transform Khang/map/preview/camera không có trong save toàn tuyến |
| samplesAllowed/visitedHa/returnedToWorkshop/tutorial/intro | **Không sync** sang permissions/location/progress mới; không đoán đã được phép hoặc đã tới vùng từ cờ cũ |
| conclusions/finalized/ending/decoded/replySent | **Không sync/không chấm lại tự động**; fullcase draft mới trống, chưa chốt/chưa reply/chưa đánh dấu thư đã giải. Import không biến hồ sơ prototype thành ending canon |
| Asked topics/Answered/history | Không lưu lịch sử topic ở cả cơ chế interview hiện tại; ND-09 chờ mục đích/state. Nếu producer lời khai mới cần persistence, giao Gameplay sau chốt |
| Playback/UI/pause/confirm | Không resume runtime timeline/media/modal/context cũ. Load main/checkpoint fullcase đều reset transient; saved trial reports giữ, timestamp đang phát không lưu |

Import chỉ vào phiên trống theo guard hiện có, không ghi lại tệp nguồn, không đồng bộ ngược và không tự lưu output. Sau Success phải xem lại mục/đi lại điều kiện chưa được nhập rồi chủ động `TrySave(profileToanTuyen)`. Dùng bản sao profile thử để kiểm chuyển; không đụng save gốc. Catalog thay đổi làm hash khác kể cả text/revision, sẽ từ chối Load; nhóm chốt dùng profile mới hay yêu cầu migration cụ thể, Gameplay không đoán tiến trình từ thiếu dữ liệu.

## G. Thay từng mô phỏng và giữ runtime

1. Thay catalog TEST ONLY bằng catalog duyệt; giữ ID cũ, ordinal observations đã có save hoặc giao migration trước khi đổi. `test.*` là key kiểm, không chuyển thẳng thành canon. Nhận xét/rubric nội dung không để lộ trên UI; `reviewed/fallChainReviewed` chỉ đặt sau review thực GDD14, không từ việc player nhặt đủ.
2. Khu: trigger/cửa/interaction thật gửi destinationId cho `TryTravel`; nếu lỗi giữ nguyên vị trí. Success mới dùng spawn/scene routing do tích hợp quản lý; adapter phải bảo đảm hoàn tất chuyển vật lý hoặc có bước điều phối khôi phục, chưa có orchestration scene async trong runtime thử. World không đọc anchors preview.
3. Vật/NPC: component phía scene giữ id/location đã duyệt, gọi lệnh trên khi input hợp lệ. Thay cube bằng visual thật; ID không lấy từ tên model. NPC location mapping do catalog/adapter scene quản lý, không dùng logic chọn actor rút gọn của cửa sổ thử. Không gắn `FullCasePlaceholder` (Editor) vào prefab sản phẩm.
4. Pose/media/rig: render từ pose key/TrialState/MediaTime; viewer controls dùng gói inspect/compare cũ. Chỉ gửi `Advance(visibleDelta)` khi panel thật hiện và game không pause, không dùng duration từ lúc đóng panel. Các asset không được tự ghi observation khi animation complete ngoài quy tắc runtime đã mở.
5. UI: tạo consumer một runtime duy nhất, subscribe/unsubscribe như F; bind states và error từng action; modal input/focus do adapter sở hữu. Tắt/loại `FullCaseSandboxWindow`, `FullCaseSandboxController`, `FullCaseTestCatalog` và tool kiểm khỏi flow sản phẩm; chúng vốn Editor-only nên không có trong player. Scene sản phẩm không auto spawn môi trường thử.
6. Save/restore: UI/profile resolver truyền profile thật được chỉ định; không hardcode path thử. Nút khôi phục checkpoint chỉ thêm khi thiết kế được chốt; API có sẵn không tự tạo yêu cầu nút mới. Không âm thầm import khi New Game/Load.


### G.1. Acceptance khi thay từng nhóm placeholder

Đây là thứ tự nhận có thể kiểm theo từng bước; phần API cụ thể ở F, dữ liệu tạm ở B. Không cần chờ mọi asset xong mới làm consumer, nhưng không đánh dấu bước chưa nối là đã kiểm.

| Bước / chủ nhận | Thay gì, giữ gì, nối thực tế | Acceptance scene chung |
|---|---|---|
| 1 — UI/UX & Tích hợp; Gameplay review data | approved `FullCaseCatalog` thay `FullCaseTestCatalog.Create`; giữ runtime/ID cũ, chọn controller prototype hoặc fullcase rõ ràng, tạo một instance/session; subscribe `Changed` và tháo listener khi đổi phiên | Không instantiate TEST catalog/window; profile/type rõ, query khóa không lộ; constructor từ chối ID/requirement sai. Chưa reviewed thì chưa chấm support |
| 2 — Map & Asset + UI/UX & Tích hợp | Visual vùng/cửa thay cube; adapter giữ destinationId, gọi CanTravel để gợi trạng thái và TryTravel để thực thi; Success mới route/spawn. Không dùng anchors/Hierarchy thử. Travel world lỗi cần cơ chế điều phối adapter, chưa có async scene orchestration | thiếu gate không rời vùng; đủ gate đổi vùng đúng, lặp/hủy/pause/modal/Load; không tách state và vị trí vật lý |
| 3 — Map & Asset + UI/UX & Tích hợp | Evidence visual/collider và source data; giữ StableId tương đương evidence.id, gọi TryCollect→TryInspect→TryRecordObservation trên từng ý định; source/kind/query không đọc raw đáp án; slots bind Transform sau Success | khám chưa ghi không tiến; chưa mở không lộ; lặp không nhân note/vật; đặt sai giữ vật; Load không tạo bản sao. Kiểm nguồn ghi lời khai thực, không item M4 thử |
| 4 — UI/UX & Tích hợp | NPC thật + approved topics, Begin/Ask/End và Answered; consumer tự chọn modal/input theo actorId; chỉ producer được duyệt mới ghi lời khai/thưởng; unsubscribe/cancel/Load hết hạn context | đúng actor/gate, khóa/sai không lộ Question/Answer, repeat không thưởng. ND-09 chưa chốt không lén lưu history |
| 5 — Map & Asset + UI/UX & Tích hợp | Pose/glyph/book/media và viewer; dùng GetLetterPoses/GetHenriLookup, TryAssign/TryRecordReading/TrySendReply và GetRecordedReadings; hai viewer dùng controls gói cũ một lần input. Advance chỉ visibleDelta, render TrialState/MediaTime thay text proxy | pose key khớp cả ba nguồn, chưa thư không hiện; đọc sai/không đủ clip không ghi; seek/pause/rời không grant/nhảy; quyền help không phạt ending |
| 6 — UI/UX & Tích hợp + Map & Asset | Mẫu thật/feature và task options, evidenceIds/requires/expectedOption ở catalog, UI chỉ gửi lựa chọn vào TryCompare; không hiện rubric | mẫu gần giống không đủ, đủ nguồn mới thao tác; sai vẫn là giả thuyết/attempt, không báo đúng trước chốt. GDD14 được review bằng asset thật |
| 7 — UI/UX & Tích hợp; Gameplay review rubric | Lựa chọn tiếng Việt/approved alternatives thay test claims; GetAvailableGrounds/GetConclusions→TrySetConclusion→TryConfirm→TryFinalize; GetEnding presenter hậu quả theo dimension; Load đúng mode/path/checkpoint F.1 | sửa làm confirm cũ mất; lỗi ghi không báo chốt; complete/open/wrong/unsupported ưu tiên đúng; không tự bắt oan cú ngã vì sai motive; error không lộ exception; Load thành công bỏ thông báo lỗi cũ |

Khi thay xong: loại/tắt đường mở cửa sổ/cube/catalog thử khỏi quy trình sản phẩm (chúng đã ở Editor), giữ runtime và catalog được duyệt, không chuyển component FullCasePlaceholder vào prefab player. Báo cáo nhận phải ghi commit scene/UI/catalog thực chạy và các bước trên đã/chưa kiểm; không kế thừa PASS từ proxy.

## H. Nội dung còn chờ và ảnh hưởng

Chín quyết định giữ nguyên câu hỏi/entry và căn cứ chi tiết tại [bảng ND-01…09](../Tasks/N2/BAN_GIAO_CHO_NOI_DUNG_UI_20261110.md#3-quyết-định-nội-dung); không chép thành quyết định mới.

| Điểm | Căn cứ / câu hỏi cần chốt | Phần đã làm, giả định thử | Sau chốt / kiểm lại / chủ nhận |
|---|---|---|---|
| ND-01 mechanism:1; ND-02 hook:0 | GDD3.3–4,7.1–2: tách khám hiện trường và kết quả bản sao; có tách entry không? | Cơ chế ID/ghi/nguồn an toàn; Unclassified, không gán nhãn thử thành chính thức | UI/UX & Tích hợp duyệt loại/nguồn/câu; Gameplay mapping, đánh giá index/save nếu tách; kiểm query/load |
| ND-03 samples:0; ND-04 letter:1 | GDD7.4/5.3: lời kể bàn giao hay thao tác; quan sát hình hay chú thích kỹ thuật? | Quyền và note tách, TEST ONLY không phát note từ permission | Duyệt source/label; producer mới thì Gameplay sửa/kiểm gate/record |
| ND-05 draft:0; ND-06 decoded:0 | GDD6.2/7.2: tách vật/nhận định; bản đọc là loại nào? | Không nối nháp Hà thành nháp Khải; thêm key paper.khai thử riêng | UI/UX & Tích hợp duyệt; Gameplay mapping/compatibility, kiểm không lộ/không đổi gate |
| ND-07 trial reports | GDD7.1: diễn biến mô phỏng không đồng nghĩa sự kiện thực đã chứng minh | Report nhắc mô hình, chưa phân loại; không cấp danh tính/ý định từ trial | UI/content chốt nhãn/nguồn; Gameplay cập nhật enum/mapping nếu được giao, không tự thêm loại |
| ND-08 Hà question/answer/participant/topic ID/gate | GDD4/6.7/5.2 không quy định từng gate | Cơ chế interview, TEST ONLY tách Editor; không canon | UI/UX & Tích hợp catalog duyệt; Gameplay chỉ thêm requirement/producer nếu hiện có không đủ; kiểm khóa/sai/repeat/no leak |
| ND-09 lịch sử hỏi lại để làm gì? | GDD11 lưu lời khai, không định nghĩa per-topic history/branch | Không thêm history/schema, hỏi lại không thưởng | Nhóm chốt mục đích/key/version; Gameplay migration/producer khi cần; kiểm save cũ/mới; UI modal/input |

Đầu vào sản xuất toàn tuyến ngoài chín điểm trên: bản ghi chính thức M1…M6/P1…P3; feature đối chiếu/alternative support sets; đầu mối vào từng khu và kênh reply; 26 pose; footage/geometry gate GDD14; lựa chọn kết luận và trình bày ending không tự tuyên tội. GDD đã chốt sự thật/ba ending; không hỏi lại ai gửi thư/hung thủ hoặc viết sự thật mới. Nhưng dữ liệu đủ để chấm chuỗi phải được review thật, không lấy mảng `test.feature.match` làm căn cứ forensic. Gameplay có thể nhận catalog với cơ chế hiện có; nếu cần requirement/producer/migration mới, giao cụ thể sau review. Kiểm lại content references, tuyến mở/khóa, bộ căn cứ tương đương, các ending, catalog/save compatibility; không chạy lại toàn bộ IO vô căn cứ.

## I. Thứ tự nhận và phụ thuộc

Nhận toàn lịch sử nhánh Gameplay từ baseline nhóm, gồm nền `f8b50889 → 8faf6a14 → e71da758 → 9dd5d6eb → 69e9ca57`, rồi bản vá `942ca056 → 61892688 → báo cáo gói này`. Các gói inspect/compare/provenance/interview trước tại `74a80c21`/`fc781aaa`/`a6fcc2af` vẫn theo [đầu mối nội dung/UI](../Tasks/N2/BAN_GIAO_CHO_NOI_DUNG_UI_20261110.md), không lặp kiểm vì chưa nối. Runtime mới tái sử dụng cơ cấu/writer/interview; không thay toàn scene hoặc đồng bộ bằng reset.

Thứ tự: (1) UI/UX & Tích hợp duyệt catalog và ND; (2) Gameplay xác nhận requirement/hash/migration, sửa phần được giao nếu cần; (3) Map & Asset giao ID/visual/rig/collider/media; (4) UI/UX & Tích hợp chuyển consumer sang controller toàn tuyến và wire input/modal/persistence; (5) review GDD14/rubric; (6) kiểm scene chung và người mới. Model/UI độc lập có thể sản xuất song song, không cần chờ mọi thứ mới bắt đầu.

**Có wiring mới cho controller toàn tuyến**. Không áp dụng câu “không cần wiring mới” của các guard save cũ cho gói này. Không có scene tích hợp mới, build/ZIP hoặc thay GDD trong gói.

## J. Checklist sau thay asset/UI trong scene chung

- Profile sạch, không fixture; đi văn phòng–xưởng–các đầu mối–nhà hát/homestay–Lan–hồ sơ–ga qua lệnh thật. Ghi commit scene/UI/catalog thực chạy, thứ tự, profile và input.
- Khám/xem chưa ghi không tính đã ghi; nguồn chưa mở không lộ transcript/answer; clip đủ quan sát hình thật, seek không cấp ghi; các nguồn thu/hỏi/ghi lặp không thưởng nhân đôi.
- Giấy cùng loại/vải cùng màu/giày gần giống chưa đủ; thử lựa chọn sai, hủy/đổi lại, nguồn còn thiếu và bộ căn cứ tương đương; không chỉ nhìn feature key trong tool để chứng minh match.
- Vật đang cầm: cất/đặt sai/đổi vùng/Load giữ ID, slot/visual khớp; modal khóa di chuyển/nhìn, Esc trở lại; bàn dựng rời/quay lại, pause tay/seek/clear không nhảy frame hoặc cấp sai record.
- Từng thư, tra Henri/gợi ý, reply kênh chưa mở/mở và thứ tự sai/đúng; không trừ thời gian chờ, không bắt đủ năm mới đọc một.
- Kết luận gắn nguồn không bị lộ grade trước chốt, edit làm confirm hết hạn, đủ/open/wrong/unsupported ưu tiên đúng; sai động cơ không báo bắt oan cú ngã; catalog chưa review không chấm như người chơi sai.
- Save/Load tại khu, media/cơ cấu/đối chiếu/kết luận; lỗi validation/đọc/ghi giữ tiến trình/bản tốt, không báo thành công; checkpoint chọn tường minh. Đường file profile đã xác nhận, không save gốc.
- Kiểm GDD14 chuỗi gây ngã/asset thật trước bật gate review; UI1080p/header/footer/readability và người mới ghi riêng. Người chơi tự nhiên không được cấp fixture.

## K. Bằng chứng, commit và lỗi OPEN


Chuỗi nhận: **`81beb1d0 → f8b50889` nguồn → `8faf6a14` công cụ/catalog/proxy đã kiểm → commit tài liệu chứa file này**. Không tạo commit vòng để tự ghi SHA báo cáo. Source tree runtime không đổi sau lượt run03; `git diff` runtime sau commit rỗng. Các output run03/interview có nhãn working-v3 tại lúc chạy trước commit; được đối chiếu với nguồn đã stage/commit f8b50889, không gọi là đã chạy một source khác. Probe cửa sổ đã ghi trực tiếp source f8b50889. Công cụ window chỉ thêm accessor Editor/probe, không sửa nguồn sau API run03.

| Lượt / môi trường | Kết quả thực chạy | Phạm vi và lịch sử |
|---|---|---|
| Attempt01, EditMode API TEST ONLY từ lượt trước được giữ local | 166/166 | Working tree cũ, không thay cho kiểm nguồn cuối |
| run01, Unity 6000.6.2f1 | COMPILE FAILED, suite NOT_RUN | Factory Ok/Fail của FullActionResult còn internal, Editor assembly không truy cập. Đã sửa public trước kiểm tiếp; không xóa log |
| run02, EditMode API TEST ONLY | 168/168 | Tuyến/ending/save ban đầu trước lookup/adapter bổ sung; không lấy PASS này thay bản cuối |
| run03, EditMode API + adapter TEST ONLY | **196/196** | Hai thứ tự tuyến lệnh thật; năm thư/reply, tám actor hỏi lặp không thưởng, bốn mục kết luận và complete/open/wrong-sender/wrong-motive/wrong-fall/unsupported/alternative-set; pending fail-closed; sáu trial context/seek/pause/return; copy/cycle/reentry; save từ chối giữ state/bytes/context, recover, import nguồn cũ không đổi bytes; proxy cancel/input/lookup |
| interview-regression, PlayMode API/proxy Gameplay | **48/48** | Chỉ bộ hội thoại chịu ảnh hưởng interface, constructor cũ/điều kiện/query/repeat/Answered/Load context; fixture cấp dữ kiện có ghi rõ, không tuyến tự nhiên |
| window01, Editor window + event Unity và setup API | **10/10** | F5/F6/Esc, JSON lỗi giữ bytes/state, modal CanMove/pause, Load hợp lệ rồi tiếp tục lưu/tải. Không input Windows, không raycast/controller FPS hoặc kiểm ảnh 1080p |

Output/save/log ở `Test/Output/FullCase20261110`, `Test/Output/fullcase-*.log` và `Test/PlaytestProfiles/FullCase*` chỉ local/ignore; không commit raw log/capture/save. SHA256 kết quả để đối chiếu khi cần:

- `run03/results.json`: `c048c00f7618ed5e37e965f5e524b5bf9d1f7a9280c93fce2015c8deec51c909` (196 kiểm).
- `interview-regression/results.json`: `9000502f742a9146bc7e003acc37d29718fc30851ee813e7e4e9dd81ff89033d` (48 kiểm).
- `window01/results.json`: `a3513469b89fa89f28405af2206b63cd81ce267155281246860e214c9a24fb70` (10 kiểm).


Các lỗi IO mô phỏng gói này: lock file đích main trong phạm vi profile riêng bằng `using FileStream(...,FileShare.None)` cho save/finalize; dispose khôi phục, kiểm bytes main và live state rồi finalize/checkpoint/save/load tiếp. Không đầy ổ thật/đổi quyền thư mục/kill Unity; không chứng minh chịu mất điện/crash. IO-OPEN-01 lịch sử **vẫn OPEN**, không đóng từ các lượt thành công này. Không chạy lại toàn bộ các bộ API/IO/trial cũ.

Runtime source mới không đổi luật cơ cấu/save r107/ID/GUID cũ, scene chung hoặc sản phẩm UI. Một lỗi compile công cụ và các cảnh báo legacy/Unity obsolete được giữ lịch sử. Hai ProjectSettings đang đổi từ lúc nhận và untracked `.vscode/`, `Test/KetQua/W_route_Unity_v30_20261016/` được giữ ngoài commit; không reset/clean/dọn asset.

**RUNTIME ĐÃ KIỂM** trong catalog/phạm vi trên. **MÔ PHỎNG ĐÃ KIỂM** ở API action route/adapter và các event cửa sổ đại diện; chưa kiểm trải nghiệm toàn tuyến bằng người thật hoặc render/readability proxy. **CHỜ NỘI DUNG** ND-01…09, bản ghi/gate/feature/rubric được duyệt và review chuỗi GDD14. **CHỜ ASSET-UI** art/footage/rig/consumer/input/travel thật. **CHƯA KIỂM SCENE CHUNG**, UI1080p và người mới **NOT_RUN**. Load feedback/P-close giữ mục OPEN riêng theo báo cáo phía tích hợp, không tự sửa hoặc tuyên đã hết lỗi; P-close lần gần nhất chưa tái hiện/nguyên nhân chưa xác định. Không chốt baseline nhóm/toàn game hoặc merge vào scene/nhánh tích hợp.

Việc Gameplay phải quay lại: cập nhật mapping provenance sau ND; thêm producer/gate/history/migration nếu catalog/nội dung chốt cần mà interface hiện chưa biểu diễn; đối chiếu/kiểm rubric đã duyệt; xử lý runtime defect có bước tái hiện từ scene thật. Chủ nhận UI/UX & Tích hợp điều phối nội dung/wiring và báo commit scene thực chạy; Map & Asset nhận bảng D/E/G để sản xuất. Chỉ báo TÍCH HỢP ĐÃ KIỂM sau biên nhận tương ứng.


### K.1. Đóng gói khả năng bàn giao sau e71da758

Nguồn runtime thực kiểm vẫn **f8b50889**, được chứa trong e71da758; **không sửa runtime** trong đợt này. Công cụ bổ sung **9dd5d6eb** chỉ là `Gameplay/Editor/FullCaseDeliveryChecks.cs` + meta. Ma trận B đã tách đủ F01…F19; B.1–3 ghi giá trị tạm và điểm thay/kiểm lại, F.1 tách hai hệ API/save, G.1 ghi acceptance từng bước; không dùng tổng PASS để tuyên phủ toàn GDD.

Chạy Unity6000.6.2f1 graphics batch: `-executeMethod ConRoi.Gameplay.EditorChecks.FullCaseDeliveryChecks.RunBatch --delivery-output <thư-mục-mới-tuyệt-đối> --delivery-source e71da758 -logFile <log-local>`. Không cần `--investigation-save-dir`: mở qua menu nên dùng profile GUID của cửa sổ. Không input desktop, không scene Play sản phẩm hoặc build. Thực chạy `Test/Output/FullCaseDelivery20261110/run01/results.json`: **26 assertion đạt**, trace **196 lệnh**, failures=0; đây là một lượt bổ sung, không chạy lại 196 API/48 interview/10 window cũ.

Chuỗi thực chạy: mở scene Gameplay và `EditorApplication.ExecuteMenuItem("Con Roi/Gameplay/Toan tuyen TEST ONLY")` → xác nhận office/trống/profile chưa tồn tại → workshop khám/xem/ghi, phát clip qua Tick visible tổng hợp → hỏi Khải → F5 → lake → F6 phục hồi workshop → lake/Bảo → market/Hà → school/Mai → slope/Tùng → office/Phương/xin mẫu → theatre/tra Henri → homestay/mẫu/chủ nhà → garden/Lan → gán từng key từ bảng tra đã mở/ghi năm bản đọc/hồi đáp → phát release/help/hook và bỏ giữ bằng cách tiến timeline qua adapter (delta tổng hợp, không seek lấy report) → đối chiếu các task → office/draft/confirm/chốt rubric thử → station/F5/F6 → chọn checkpoint chủ động về draft office. Có tám kiểm hỏi không đổi tiến trình, kiểm trước chốt không verdict, kiểm checkpoint không finalized/confirmed. Các M/nguồn/lời khai thử được thu qua TryCollect/Inspect/Record tại khu, **không phát ra từ câu trả lời NPC**; đây chưa phải tuyến hội thoại chính thức.

Mã route không gán state, không gọi import, không sửa prerequisite/catalog, không có nút cấp chứng cứ/mở khóa. Nó biết trước feature/claim thử và thứ tự reply để kiểm cơ chế; không chứng minh người chơi tự suy ra lời giải. Actor/evidence/placeholders thay presentation, còn thu/ghi/điều kiện/đối chiếu/cipher/trial/finalize đều qua runtime thật.

Kiểm cách ly riêng: tập source do `CompilationPipeline.GetAssemblies(AssembliesType.Player)` trả về chứa FullInvestigationRuntime nhưng không chứa FullCase test tools; dependency scene ConRoi_Chinh không có FullCase Editor; mở scene chung **EditMode/read-only** không có FullCasePlaceholder; mở Gameplay không tự tạo sandbox trước gọi menu. Hash file scene Gameplay và scene chung giữ nguyên trước/sau. Đây là bằng chứng cấu trúc/khởi tạo Editor, **không chạy Play scene chung hoặc build player**; không suy kết quả render/controller hay UI1080p. Không công bố scene chung đã đạt tích hợp.

SHA256 output local (không commit raw trace/log/save): `36d75c00b52fe2b91c04886355b64963b587f8b9e86b86b7a72729804f4ad594`. Profile mới thực dùng: `C:\ConRoi_Nguoi2\Test\PlaytestProfiles\FullCaseSandbox_71e095337edf4ad58281256cd465eab4`. Main và checkpoint ở subfolder fullcase của đúng profile này; không có truy cập profile người đang chơi.


Phần Gameplay độc lập chốt theo phạm vi cơ chế B, **không** chốt gameplay toàn game hoặc ending đã duyệt. Chủ nhận và dữ liệu thiếu ở B/H/G.1: UI/UX & Tích hợp chủ trì ND-01…09, approved catalog/gate/feature/rubric/consumer; Map & Asset sản xuất visual/collider/pose/footage. Gameplay quay lại mapping per-entry/producer sau review, requirement mới khi gate được chốt, history/migration khi có quyết định cụ thể và lỗi runtime từ scene thật. Không tự hợp nhất save hoặc đổi schema, ID/GUID; IO-OPEN-01/P-close vẫn OPEN, người mới/UI1080p/scene chung NOT_RUN cho controller toàn tuyến. Không tạo hệ thống mới hoặc mở audit tiếp.


<a id="reading-producer"></a>

### K.2. FULL-READING-PRODUCER-01 — acceptance trước sửa, sau69e9ca57

Căn cứ GDD5.1–5.2: người chơi tự gán/ghi bản đọc; bản chép phục hồi câu, mỗi thư độc lập;7.2/9: không tự phát đáp án chưa khám/giải. Hiện `TryRecordReading` ghi cờ nhưng chưa có query bản chép đã ghi; `decoded` còn là item thử thu/ghi độc lập tại chợ. Chọn hoàn thiện producer bản đọc, không thay producer lời khai/gate/rubric chưa được duyệt.

Acceptance: (1) baseline ghi được item decoded khi chưa giải phải được tái hiện riêng; (2) toàn tuyến mới không thu decoded để vượt giải mã; (3) chỉ TryRecordReading thành công mới query được bản chép đúng GDD của thư tương ứng, có dấu câu và nghĩa tiếng Việt; chưa thu/xem/giải hoặc chỉ gán mapping chưa ghi không lộ; (4) ghi lặp không nhân bản, sửa mapping sau khi đã ghi không làm sai bản chép đã có; (5) Decoded prerequisite cập nhật từ cùng hành động, event Changed hiện có; (6) Save/Load bằng profile riêng giữ các bản đọc, từ chối/giải sai không tạo tiến trình; save fullcase cũ có item decoded vẫn tải được, không tự biến item này thành bản đọc thật hoặc ghi lại khi Load; r107/schema/ID/GUID/catalogHash không đổi; (7) tuyến liên tục qua lệnh thật, không seed, UI thử chỉ trình bày query runtime. ND-06 về loại/nguồn bản đọc vẫn CHỜ CHỐT, không tự gán Quan sát/Lời kể/Giả thuyết.


#### Kết quả chức năng / contract nhận cho sản phẩm

Đã triển khai nguồn **942ca056**; công cụ và consumer thử **61892688**. Khác với mock note ở chợ, hành động `TryRecordReading(letterId)` thành công giữ ID trong `state.decoded`, phát `Changed` khi tiến trình mới, và là producer duy nhất của **bản chép chuẩn GDD** mà consumer mới thấy. Việc gán đúng mapping mà chưa ghi không tạo bản chép; thu/xem thư hoặc ghi alias cũ cũng không tạo. Không thêm/lưu text dẫn xuất, không sửa phép chấm đúng bản đọc, không mở thêm gate/rubric hoặc tự chốt ND-06.

| API thật | Input/output và trạng thái | Cách dùng/wiring |
|---|---|---|
| `TryRecordReading(string id)` (API cũ giữ tên) | Phải đang Cipher, có thư đã xem và mapping đọc đúng. Kết quả `FullActionResult`; sai/paused/context lỗi không thêm cờ/bản chép; lặp không nhân cờ/event | UI gửi ý định ghi, render Message từ kết quả; không tự set decoded hoặc phát note từ nút thành công |
| `TryGetRecordedReading(string id,out FullRecordedReading reading)` (mới) | False/default (Text/Meaning null) khi ID sai/chưa mở/chưa ghi. True chỉ thư đã ghi; Id/Recipient/Text/Meaning immutable | Query sau Success/Changed/Load. Text phục hồi dấu câu và Meaning lấy **nguyên bảng GDD5.2**, không phải câu chuyện/lời khai mới |
| `GetRecordedReadings()` (mới) | Mảng snapshot năm ID theo thứ tự GDD, chỉ những thư đã mở+ghi. Trống khi chưa ghi; không làm đổi state/Message/modal/pause/event/file | UI hồ sơ liệt kê mục đã ghi. Không đọc Transcript/Meaning bảng developer để hiện thư khóa |
| `GetLocalEvidence()` / `TryCollect("decoded")` (hành vi sửa có chủ ý) | Alias dẫn xuất không còn trong nguồn world; TryCollect luôn trả `derived-reading`, giữ state. Cả đã giải cũng không biến bản chép thành vật để nhặt | Thay caller collect decoded bằng flow thu thư→xem→gán→TryRecordReading→query. **Chỉ luồng fullcase thay đổi**; prototype `InvestigationSession.RecordReading/Acquire` giữ nguyên |
| `Changed` (event cũ) | Một notification khi ghi một thư lần đầu; query/lặp ghi không phát tiến trình mới. Load thay DataContext như trước | `runtime.Changed += RefreshReadings`; unsubscribe `-= RefreshReadings` khi đóng consumer/đổi phiên. Callback chỉ query/render; mutation reentry bị chặn như trước |

`FullRecordedReading` ở `FullInvestigationData.cs` có readonly `Id,Recipient,Text,Meaning`. Các ID không đổi: `letter.bao`, `letter`, `letter.mai`, `letter.tung`, `letter.lan`. `FullInvestigationLetters.Transcript/Meaning` là internal, không API UI để lộ thư chưa mở. Không có field Kind/Source hoặc nhãn nguồn mới: ND-06 vẫn CHỜ CHỐT, consumer giữ chưa phân loại theo hợp đồng provenance đã giao. Bản chép ghi nhận nội dung thông điệp, **không xác nhận các cáo buộc trong thư là sự thật**.

Ví dụ consumer fullcase (UI/UX & Tích hợp nối vào sản phẩm; cửa sổ Editor chỉ minh họa):

```csharp
void OnRecordReading(string letterId) {
    var result = runtime.TryRecordReading(letterId);
    ShowFeedback(result.Message);
    if (result.Success) RefreshReadings(); // ghi lặp có thể Success nhưng không phát Changed mới
}
void RefreshReadings() {
    var openedReadings = runtime.GetRecordedReadings();
    // Bind Id/Recipient/Text/Meaning. Không tự thêm note, chứng cứ hoặc verdict.
}
// Khi enable: runtime.Changed += RefreshReadings; RefreshReadings();
// Khi disable: runtime.Changed -= RefreshReadings;
```

UI/UX & Tích hợp nhận wiring consumer mới và kiểm hồ sơ/mật mã/Load trong scene chung. Không sửa Hierarchy/UI/scene chung trong gói. Map & Asset vẫn nhận pose/glyph/media cũ; **gói này không cần asset mới để producer hoạt động**, nhưng chưa kiểm khả năng đọc pose thật. UI thử `FullCaseSandboxWindow` đã thêm phần bản chép đọc trực tiếp query runtime, không tạo kết quả thử bằng chuỗi hardcode.

**Save/tương thích:** không thêm field vào `FullCaseSave`/`InvestigationSave`, không đổi schema/namespace/ID/GUID/catalog data hoặc hash. Bản chép dẫn xuất từ ID thư đã giải đúng và dữ liệu GDD cố định, không serialize text/Meaning. Sửa mapping thử sau khi đã ghi không thay bản chép đã ghi; không xóa lịch sử đã giải. Save fullcase cũ có decoded flags hợp lệ tự trả bản chép khi query sau Load, không ghi lại file khi đọc. Save fullcase cũ chỉ có raw evidence `decoded` mà chưa có D(letter) vẫn được validator chấp nhận và giữ nguyên record/bytes, **không tự promote thành bản chép**. Raw record/grounds lịch sử được bảo toàn, không xóa hoặc migration âm thầm; consumer mới không dùng `HasEvidence("decoded")`/`GetRecordedNotes("decoded")` làm trạng thái đã giải và nên bỏ raw alias khỏi danh sách bản chép để tránh hiển thị trùng. Prerequisite mới dùng `FullRequirementKind.Decoded` với ID thư, không Collected/Recorded của alias. Prototype r107 không thay hành vi, không tự đồng bộ sang fullcase; bảng F.1 vẫn áp dụng. Nội dung GDD thay nghĩa trong tương lai cần review compatibility, không tự suy lại tiến trình người dùng.

#### Kiểm và mức bao phủ gói

- **Trước sửa tại69e9ca57:** `ReadingProducerChecks.RunBaseline` chạy API action route office→workshop khám/ghi slide→market thu/xem/ghi decoded. Tái hiện raw note đã ghi trong khi D(letter)=false, lưu bản thử để kiểm load ngược. Đây là bằng chứng hạn chế trước sửa, **không PASS gameplay**; method tái hiện lịch sử chỉ kỳ vọng hành vi cũ và không dùng làm regression trên nguồn mới.
- **Sau sửa:** Unity6000.6.2f1, menu cửa sổ Gameplay/profile GUID mới, `ReadingProducerChecks.RunBatch` **33/33 assertion đạt**: **28 API/action route** và **5 nhóm kiểm bằng event Unity F5/F6/Esc với setup API**. Không gọi là input Windows/người chơi thật hoặc UI sản phẩm đã đạt.
- Tuyến hành động liên tục không seed/import/set progress: xưởng khám/ghi slide→chợ khám thư Hà, gán sai và thử ghi bị từ chối→trường ghi M3/đầu mối thử→nhà hát mở sổ Henri→gán từng key lấy từ sổ→ghi Hà→F5, sửa mapping, F6→hồ/Bảo→trường/Mai→dốc/Tùng→vườn/Lan, lần lượt khám/xem/ghi và ghi bản đọc→pause/resume→F5/đổi vùng/F6 giữ đủ năm. Chỉ dùng SetModal/SetPaused để mô phỏng context UI, không setter chứng cứ/gate/decoded. Gate đi vùng vẫn TEST ONLY, không phải gate canon được duyệt.
- Từng Text/Meaning được test đối chiếu với **bảng thư trong file GDD hiện hành**, không dùng hàm Transcript làm oracle. Chưa mở/ID sai/correct mapping chưa ghi không trả nội dung; đọc sai/paused không đổi tiến trình; ghi lần đầu mở đúng D(id)+Changed, ghi lặp/query không nhân bản/event; snapshot không có tham chiếu mutable; mapping thử đổi không đổi bản chép đã ghi.
- Hai kiểm Load riêng chỉ đọc profile thử có trước sửa: bản baseline raw alias không bị promote; bản đầy đủ từ run03 cũ trả đúng năm bản chép. Cả hai giữ bytes, catalog/hash/schema hợp lệ. Không lặp toàn bộ bộ save/IO/trial. Bộ kiểm parser/writer/r107 không sửa.

Output local `baseline/baseline.json`: SHA256 `c318ff6ebd97d20cbcceed17dd976bdb6ccf81519853592b0f9505a748878f4c`.

Output local `verify01/results.json`: SHA256 `637e0a8c4b85555d907df7ceb8293feeb8c4c31a987db8f8b7162efc517ba7a5`.


Chạy lại gói khi có thay đổi liên quan: Unity `-batchmode -executeMethod ConRoi.Gameplay.EditorChecks.ReadingProducerChecks.RunBatch --reading-output <output-mới> --reading-legacy-profile <profile-thử-cũ-chỉ-alias> --reading-old-valid-profile <profile-thử-cũ-đã-giải-năm-thư> --reading-source <SHA> -logFile <log-local>`. Hai profile cũ chỉ được đọc; nếu cần tạo fixture tương thích phải tách khỏi bằng chứng tuyến hành động. Output/save/log/capture không commit. Working state của runtime đã chạy verify01 khớp nguồn942ca056 sau commit; công cụ61892688 chứa đúng source/UI thử đã chạy. Baseline, lệch nguồn và giới hạn được giữ lịch sử, không thay nhãn các kiểm cũ thành kiểm nguồn mới.

#### Phần tiếp theo bị chặn chính xác ở đầu vào nào

| Nhóm cần chốt / chủ nhận UI/UX & Tích hợp điều phối nội dung | Đầu vào cụ thể để mở việc Gameplay | Phần Gameplay quay lại, không tự duyệt |
|---|---|---|
| Producer lời khai/NPC, ND-08/09 | Từng participant/topic ID, Question/Answer được duyệt; điều kiện mở theo entry/action thực; câu trả lời nào cung cấp bản ghi nào (ID/index), chỉ nghe hay phải chủ động ghi; history cần lưu để hiển thị/tiếp nhánh hay chỉ nhắc lại | Nối producer đúng topic với note/progress; thêm requirement chỉ khi hiện có không biểu diễn; thiết kế/kiểm history/save tương thích khi có yêu cầu. Không biến item M4 hoặc Answered TEST ONLY thành lời Lan canon |
| Gate nhà hát/homestay/vườn/ga và kênh giấy | Entry/đầu mối chính thức thay lead.theatre/lead.homestay/M3:0/ha:1; ga cần điều kiện hẹn nào ngoài finalized; producer nào tạo các entry đó | Thay data/gate tạm bằng prerequisites được duyệt, kiểm chưa đủ/đúng thứ tự khác/lặp/Load. Không tự yêu cầu đủ năm thư hoặc reply nếu chưa được chốt |
| Đối chiếu và rubric, GDD7.3/14 | Specimen/feature keys và hình/mô tả đủ phân biệt; từng observation ID/index được dùng; bộ support tương đương/giả thuyết đối chiếu đủ và biên nhận review chuỗi gây ngã | Nối đối chiếu/producer thực từ đặc điểm người chơi chọn, thay test.feature.match/near và test claims; kiểm nhánh đúng/sai/thiếu, không để feature thử tự làm proof |
| Ending/ga | Nội dung/dàn cảnh và hành động giao nhận theo từng dimension, điểm gọi presenter/event được UI/asset sở hữu; giữ quy tắc ưu tiên GDD và không tự tuyên tội | Gameplay chỉ bổ sung action/state cụ thể nếu dàn cảnh đã chốt yêu cầu, không dựng thêm minigame/hình phạt/timer hoặc coi enum hiện tại là hoạt cảnh đã tích hợp |
| ND-01…07 provenance, gồm bản chép/kết quả bàn dựng | Sáu entry cần loại/nguồn/tách câu hay không và nhãn trial mô phỏng khác sự kiện thực; ND-06 chưa được gói này quyết định | Mapping per-entry/producer/compatibility nếu đổi index; không dùng việc đọc thư đúng để gán lời trong thư thành Quan sát hoặc fact |

Các sự thật vụ án, năm plaintext và ưu tiên ending **đã có trong GDD**, không hỏi lại hoặc đổi chúng. Các đầu vào trên là câu/producer/gate/rubric/hiển thị chưa được duyệt trong dữ liệu hiện hành, không phải quyền tự sáng tác. Không tìm thấy phần producer độc lập tiếp theo đủ đầu vào để sửa chắc chắn sau gói này; giữ những việc ấy CHỜ NỘI DUNG/ASSET-UI, không báo “hết chức năng” hoặc coi mock là sản phẩm. RUNTIME BẢN CHÉP ĐÃ KIỂM → CHỜ UI/TÍCH HỢP; scene chung/UI1080p/người mới NOT_RUN với bản sửa; ND-01…09, IO-OPEN-01/P-close giữ riêng. Không merge/build/ZIP/chốt baseline nhóm.
