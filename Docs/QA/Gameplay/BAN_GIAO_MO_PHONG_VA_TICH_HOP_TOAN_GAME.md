# Bàn giao mô phỏng và tích hợp toàn game — Gameplay & Dữ liệu

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

Nguồn runtime **`f8b50889`** (sáu file C# gồm adapter hội thoại cũ); namespace C# giữ `ConRoi.Investigation.V36.R107`. Nhận cả commit công cụ và tài liệu tiếp sau nguồn, không cherry-pick riêng báo cáo. Không tự gắn runtime này vào `InvestigationSession` đang chạy scene chung: đây là controller toàn tuyến riêng; scene prototype tiếp tục dùng contract cũ cho tới khi phía tích hợp chủ động chuyển consumer.

Unity **6000.6.2f1**, worktree Gameplay riêng. Mở `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`, chọn menu **Con Roi → Gameplay → Toan tuyen TEST ONLY**. Cửa sổ `FullCaseSandboxWindow` chạy trong Editor, không cần Play; không tạo/ghi lại scene. Mỗi lần mở tạo profile GUID riêng tại `Test/PlaytestProfiles/FullCaseSandbox_<GUID>`; đóng cửa sổ không xóa profile. F5 lưu, F6 tải chính profile đang hiển thị; Esc đóng modal, hoặc đảo pause khi không có modal. Khi đang nhập chữ, phím tắt không được bắt. Trở lại từ pause qua nút Tiếp tục/Esc. Không dùng save gốc.

Preview là cube/khu trong `PreviewRenderUtility`, không có traversal FPS/collider gameplay được chứng nhận. Các nút gọi `FullCaseSandboxController` và lệnh runtime thật, không seed chứng cứ. Giữ cửa sổ focus để media/timeline tiến; adapter chỉ gửi delta thấy được, tối đa 0,1 giây mỗi update, không chạy bù thời gian khuất cửa sổ. Không tự xuất hiện trong scene chính/player: toàn bộ catalog, proxy và cửa sổ ở thư mục Editor.

Kiểm độc lập không cần desktop: Unity batch `-executeMethod ConRoi.Gameplay.EditorChecks.FullCaseChecks.RunBatch --full-output <thư-mục-mới> --investigation-save-dir <profile-mới> --full-source <SHA>`. Bộ kiểm mở scene Gameplay nhưng gọi API EditMode; không phải chơi tự nhiên. `FullCaseWindowChecks.RunBatch --full-window-output <thư-mục-mới> --full-source <SHA>` kiểm F5/F6/Esc bằng `EditorWindow.SendEvent`, cần graphics; ghi riêng setup API và event Unity, không gọi input Windows. Không dùng lại thư mục kết quả/profile API cũ.

## B. Ma trận sau triển khai và giới hạn nội dung

Acceptance ban đầu ở trên được giữ làm lịch sử. Bảng này thay trạng thái CHƯA TRIỂN KHAI ở ma trận đầu; tất cả commit runtime bên dưới là `f8b50889`. TEST ONLY không có nghĩa canon đã hoàn tất.

| Yêu cầu | Runtime thực có | Mức hoàn thành / còn phụ thuộc | Chủ nhận |
|---|---|---|---|
| F01 | `CanTravel`, `TryTravel`, `FullLocationDefinition.requires` recheck khi chuyển | RUNTIME ĐÃ KIỂM; mười khu và hai thứ tự mô phỏng. Gate chi tiết prototype thử cần content duyệt | UI/UX & Tích hợp; Map & Asset |
| F02 | `InvestigationInterview.ForSource`, `IInvestigationInterviewSource`, dữ liệu `InterviewTopicDefinition` | CƠ CHẾ ĐÃ KIỂM; tám participant TEST ONLY, không có thoại chính thức/history mới | UI/UX & Tích hợp chủ trì nội dung |
| F03–F05 | `FullInvestigationLetters`, mapping chung, `GetLetter`, `TryRecordReading`, `TrySendReply`, `GetHints`, `GetHenriLookup` | RUNTIME ĐÃ KIỂM năm bản rõ GDD; 26 key kỹ thuật, tra sổ không tự điền/đổi điểm. CHỜ hình pose, bản chép có câu, kênh/thoại sản phẩm | Map & Asset + UI/UX & Tích hợp |
| F06–F09 | `GetComparisons`, `TryCompare`, permission, stored attempt và prerequisites | RUNTIME ĐÃ KIỂM sáu task; exact feature key/rubric là TEST ONLY, không nhận diện vải/giày thật hoặc chứng minh tương đương vật lý | UI/UX & Tích hợp duyệt dữ liệu; Map & Asset specimen |
| F10 | Tái dùng `TrialTimeline`, `MechanismPresentation`; start/pause/seek/clear/context; sáu ca | RUNTIME ĐÃ KIỂM; không sửa luật hồi. Hình nộm, đọc clip thực tế, giả thuyết thay thế GDD14 CHỜ ASSET/REVIEW | Map & Asset + UI/UX & Tích hợp |
| F11 | ID ghi nhận/nguồn, relation và các query chỉ trả dữ liệu đã mở | RUNTIME ĐÃ KIỂM; `GetNoteKind` của catalog thử vẫn Unclassified, không tự chốt sáu entry hoặc trial | UI/UX & Tích hợp; Gameplay cập nhật mapping sau review |
| F12–F14 | Bộ support tương đương authored, bốn draft, confirm theo nội dung, checkpoint, ba ending đúng ưu tiên | RUNTIME ĐÃ KIỂM bằng rubric TEST ONLY; catalog chưa reviewed trả `content-pending`. Không chứng nhận chuỗi gây ngã/cinematic/nguồn nghiệp vụ | UI/UX & Tích hợp chủ trì review nội dung; Gameplay nhận rubric/gate |
| F15 | Reply không timer, khu ga sau finalize, ending result tách phạm vi sai | RUNTIME ĐÃ KIỂM ở mức state/nhánh; bảo vệ Lan/rối và hậu quả hiển thị chưa có cinematic/hành động sản phẩm | UI/UX & Tích hợp + Map & Asset |
| F16 | Cầm/cất/đặt theo slot, không giới hạn ba vật, giữ một ID | RUNTIME ĐÃ KIỂM; không mô phỏng ném/ragdoll hoặc thay asset thật | Map & Asset sockets; UI/UX & Tích hợp controller |
| F17–F18 | File riêng, validation trước gán, import r107 chủ động, reset transient sau Load, pause/modal | RUNTIME ĐÃ KIỂM; không chuyển save người dùng tự động, không thêm topic history; UI/controller sản phẩm chưa nối | Gameplay & Dữ liệu hỗ trợ chuyển consumer; UI/UX & Tích hợp |
| F19 | Giữ các API viewer/controls gói cũ | CHỜ ASSET-UI / CHƯA KIỂM SCENE CHUNG; hình học/âm thanh/animation/camera/người mới không được thay bằng API PASS | Map & Asset + UI/UX & Tích hợp |

Không còn cơ chế độc lập rõ nào trong bảng bị để CHƯA TRIỂN KHAI. **Chưa hoàn thành gameplay sản phẩm toàn tuyến**: nội dung, gate chính thức, rubric đủ chuỗi, asset và consumer UI mới vẫn cần sản xuất/duyệt. GDD không cho phép tự chế dữ kiện forensic hoặc thoại để đóng các mục đó. Không có vụ thứ hai/siêu nhiên/ending mới.

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
7. Mật mã: chỉ gọi `GetLetter(id)`/`GetLetterPoses(id)` sau player đã mở; GetLetter nullable và chưa có thư không lộ. `TryAssign(poseKey,letter)` một A–Z/rỗng; `TryRecordReading(id)` mới ghi bản đọc. `GetHenriLookup()` chỉ sau xem henri, trả bảng copy, không điền mapping. `GetHints(id,1..3)` chỉ nội dung thư mở; `FullInvestigationLetters.Glossary` tra bất kỳ lúc nào. `TrySendReply(string[] orderedPhrases)` kiểm kênh và thứ tự, không timer/thưởng nhân đôi.
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

## G. Thay từng mô phỏng và giữ runtime

1. Thay catalog TEST ONLY bằng catalog duyệt; giữ ID cũ, ordinal observations đã có save hoặc giao migration trước khi đổi. `test.*` là key kiểm, không chuyển thẳng thành canon. Nhận xét/rubric nội dung không để lộ trên UI; `reviewed/fallChainReviewed` chỉ đặt sau review thực GDD14, không từ việc player nhặt đủ.
2. Khu: trigger/cửa/interaction thật gửi destinationId cho `TryTravel`; nếu lỗi giữ nguyên vị trí. Success mới dùng spawn/scene routing do tích hợp quản lý; adapter phải bảo đảm hoàn tất chuyển vật lý hoặc có bước điều phối khôi phục, chưa có orchestration scene async trong runtime thử. World không đọc anchors preview.
3. Vật/NPC: component phía scene giữ id/location đã duyệt, gọi lệnh trên khi input hợp lệ. Thay cube bằng visual thật; ID không lấy từ tên model. NPC location mapping do catalog/adapter scene quản lý, không dùng logic chọn actor rút gọn của cửa sổ thử. Không gắn `FullCasePlaceholder` (Editor) vào prefab sản phẩm.
4. Pose/media/rig: render từ pose key/TrialState/MediaTime; viewer controls dùng gói inspect/compare cũ. Chỉ gửi `Advance(visibleDelta)` khi panel thật hiện và game không pause, không dùng duration từ lúc đóng panel. Các asset không được tự ghi observation khi animation complete ngoài quy tắc runtime đã mở.
5. UI: tạo consumer một runtime duy nhất, subscribe/unsubscribe như F; bind states và error từng action; modal input/focus do adapter sở hữu. Tắt/loại `FullCaseSandboxWindow`, `FullCaseSandboxController`, `FullCaseTestCatalog` và tool kiểm khỏi flow sản phẩm; chúng vốn Editor-only nên không có trong player. Scene sản phẩm không auto spawn môi trường thử.
6. Save/restore: UI/profile resolver truyền profile thật được chỉ định; không hardcode path thử. Nút khôi phục checkpoint chỉ thêm khi thiết kế được chốt; API có sẵn không tự tạo yêu cầu nút mới. Không âm thầm import khi New Game/Load.

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

Nhận toàn lịch sử nhánh Gameplay từ baseline nhóm, gồm nguồn `f8b50889` → commit công cụ → báo cáo gói này. Các gói inspect/compare/provenance/interview trước tại `74a80c21`/`fc781aaa`/`a6fcc2af` vẫn theo [đầu mối nội dung/UI](../Tasks/N2/BAN_GIAO_CHO_NOI_DUNG_UI_20261110.md), không lặp kiểm vì chưa nối. Runtime mới tái sử dụng cơ cấu/writer/interview; không thay toàn scene hoặc đồng bộ bằng reset.

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
