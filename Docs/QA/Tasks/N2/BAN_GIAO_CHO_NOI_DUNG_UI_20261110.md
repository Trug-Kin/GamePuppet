# Bàn giao tập trung — các gói chờ nội dung/UI

Ngày 10/11/2026. Gameplay & Dữ liệu bàn giao cho **UI/UX & Tích hợp**; **Map & Asset** phối hợp model. Mốc source tổng hợp local: **a6fcc2affde4fc6477bda34842065b0d11eb0cce**, nhánh `work/runtime-contracts-baseline`. Tài liệu này điều phối việc nhận bốn gói; API đầy đủ, lệnh kiểm, lịch sử fail và bằng chứng giữ ở báo cáo gốc. Không có kết quả kiểm mới hoặc xác minh remote trong đợt viết tài liệu.

Mục lục: [1. Nguồn và phụ thuộc](#1-nguồn-và-phụ-thuộc) · [2. Công việc tích hợp](#2-công-việc-tích-hợp) · [3. Quyết định nội dung](#3-quyết-định-nội-dung) · [4. Chuyển việc theo ownership](#4-chuyển-việc-theo-ownership) · [5. Kiểm và biên nhận](#5-kiểm-và-biên-nhận).

## 1. Nguồn và phụ thuộc

Nhận toàn bộ lịch sử tới mốc source trên theo [TICH_HOP](../../../QuanLy/TICH_HOP.md), [HANDOFF](../../../QuanLy/HANDOFF.md) và [ownership](../../../QuanLy/OWNERSHIP.md), bảo toàn commit/working tree riêng; không chỉ nhận commit báo cáo cuối. Nhánh tích hợp được các tài liệu hiện hành chỉ định là `review/runtime-integration-20261109`, **chưa kiểm tip/remote mới trong lượt này**. Không mặc định main có nguồn, không đổi base/head hoặc tự merge theo tài liệu này. Quy trình nhận thực tế do UI/UX & Tích hợp kiểm lại trước thao tác.

| Gói / báo cáo gốc | Chuỗi cần nhận theo thứ tự thời gian | Source thực được kiểm / kết quả đã báo cáo | Phụ thuộc runtime |
|---|---|---|---|
| [INSPECT-CONTROLS-01](INSPECT-CONTROLS-01/README.md) | b8cc108f → 4c2ef425 → 0720006e → 61f26ad0 → 64403b70 | Nguồn b8cc108f, kiểm 61f26ad0: 27/27 API/fixture; hồi quy ở d671a845: 27/27 | InvestigationSession hiện hành và binding viewer/world source |
| [COMPARE-VIEW-01](COMPARE-VIEW-01/README.md) | 5ef32f00 → 23fc0cb7 → 175a248c → 0b783b8c → 49c242a6 → d671a845 → 74a80c21 | Nguồn 5ef32f00 + 0b783b8c, kiểm d671a845: 43/43 API/fixture render | INSPECT: dùng chung EvidenceViewPose/EvidenceViewMath; phải nhận bản camera đã cô lập 0b783b8c |
| [NOTE-PROVENANCE-01](NOTE-PROVENANCE-01/README.md) | Đề xuất afe461c2 → 386fdfdc → e0b9e58a → 57c6ad35 → fc781aaa | Nguồn 386fdfdc, kiểm 57c6ad35: 68/68 API/fixture | Record evidence/observation/relation/claim/trial đang có; không phụ thuộc wiring viewer |
| [INTERVIEW-REVISIT-01](INTERVIEW-REVISIT-01/README.md) | 97b8048e → e92a2ea6 → d5b67c9a → a6fcc2af | Nguồn 97b8048e + e92a2ea6, kiểm d5b67c9a: 48/48 API/fixture TEST ONLY | NOTE: RecordedObservation dùng TryGetRecordedNote; catalog nội dung phải được duyệt riêng |

Các source/test SHA trên là bằng chứng cũ có phạm vi, không phải kiểm scene chung trên mốc a6fcc2af. Không cộng các bộ/lượt lặp thành tổng ca độc lập. Hai viewer và provenance giữ **CHỜ UI/TÍCH HỢP**; interview giữ **CƠ CHẾ ĐÃ KIỂM / CHỜ NỘI DUNG & UI**.

Thứ tự đề nghị nhận: xác lập source/binding → viewer một vật → viewer hai vật; song song nối provenance với 28 entry rõ và trạng thái pending → sau khi NOTE đã có source, thử cơ chế interview bằng dữ liệu riêng. Phần nội dung hỏi lại chính thức và history chỉ nối khi được chốt. Không cần chờ review 6 entry để làm controls/model/consumer provenance; cũng không được coi review 6 entry là gate mở topic.

Namespace API chung: `ConRoi.Investigation.V36.R107`. Gọi trên Unity main thread; không tìm session bằng tên Hierarchy hoặc tự grant dữ liệu để làm view trông đủ.

## 2. Công việc tích hợp

| Gói | API/contract hiện có — xem chữ ký đầy đủ ở báo cáo gốc | UI/UX & Tích hợp nhận ngay | Model/content và phần cần Gameplay sau quyết định |
|---|---|---|---|
| INSPECT | InspectionView, TryAdjustInspection, TryResetInspection, InspectionViewChanged | Renderer Preview/Availability/CanControl, action reset, phản hồi out error; đăng ký/gỡ event đúng lifecycle; tránh feed mouse delta hai lần vì native mouse đã chạy. Getter cần refresh khi preset legacy chưa phát event | Map & Asset kiểm inspectionPrefab/source đúng ID, mặt cần nhìn, bounds/material/readability. Nếu đổi native driver/preset/consumer legacy partial, UI gửi action/patch yêu cầu cho Gameplay sửa; không cùng sửa partial |
| COMPARE | GetComparisonView(side), TrySelectComparison (có overload source), TryAdjustComparison, TryResetComparison, TryClearComparison, ComparisonViewChanged | Hai texture/controls độc lập theo Left/Right; route input đúng bên; text-only khi MissingVisual; xóa binding texture cũ khi rời/đổi. Chốt một nguồn ID lựa chọn để nối action AddRelation cũ, không trộn selectedA/B legacy và state mới | Map & Asset cung cấp model/inspectionPrefab đúng ID, kiểm góc/shader/ảnh thật; preview chuẩn hóa không dùng để đo tỷ lệ vật lý. Sửa selection/consumer trong partial hoặc driver runtime phải giao Gameplay; hình học/material thuộc Map & Asset |
| NOTE | TryGetRecordedNote(...), GetProvenanceNotes(); snapshot Origin/Kind/Status/Source/Text và nhãn tiếng Việt; **không event mới** | Nhóm/nhãn Quan sát/Lời kể/Giả thuyết/Chưa phân loại; hiển thị nguồn, không trình bày Classified như “đã chứng minh”. Refresh sau ghi/Load/đổi note/claim; không liệt kê raw definition hoặc bảng developer để lộ mục chưa ghi | Nội dung review bảng mục 3. Sau quyết định đổi loại/nguồn, Gameplay cập nhật mapping ID:index và kiểm phần bị ảnh hưởng. UI không tự phân loại bằng keyword. Nếu cần event phải giao contract cụ thể, chưa mặc định đã có |
| INTERVIEW | InterviewTopicDefinition/InterviewRequirement, InvestigationInterview; Begin/End, GetTopics/GetAvailableTopics/GetTopicState, TryAsk, ActiveParticipantId, Answered | Thiết kế adapter/render lựa chọn và câu trả lời từ Reply khi true; gỡ event, đóng context/view đúng lifecycle. Chọn dùng Reply hoặc Answered để tránh xử lý hai lần; query không chứa Answer | Nội dung cung cấp catalog Question/Answer/participant/topic ID/gate đã duyệt; UI/module nội dung nối loader/adapter. Cần sửa Gameplay nếu thêm requirement kind, phát ghi nhận, history hoặc điều hướng partial. Không có catalog Hà chính thức hoặc trang interview sản phẩm hiện có |

**Modal/input phải kiểm ở phía tích hợp:** viewer dùng Page inspect/compare hiện có, khóa đi/nhìn theo SetModalState; controls khóa khi pause và lỗi đúng context. Cặp viewer chưa route mouse tự động. Interview chỉ quản lý context riêng, **không** tự khóa movement/look, mở trang hoặc chiếm focus. Không bịa OpenPage("interview"); UI chốt cách mở/đóng hội thoại rồi giao phần partial cần sửa cho Gameplay. Không dùng SetPaused(true) để chỉ biểu diễn modal hội thoại vì cơ chế sẽ chặn TryAsk.

**Save/lifecycle:** viewer pose/selection và metadata không vào schema1. Viewer một vật Load/reopen về default; cặp preview bị dọn khi Load hợp lệ/rời trang, Load bị guard từ chối giữ view/state. Interview không lưu đã hỏi; Load thay source.data làm context cũ hết hiệu lực, Begin mới recheck. Adapter xóa reply/cache cũ khi đổi profile/view; đổi riêng SaveDirectoryOverride mà chưa thay data phải End tường minh. Không tự sửa schema hoặc dùng visitedHa/relations làm history.

## 3. Quyết định nội dung

Chủ trì/chủ nhận **mọi hàng** dưới đây: **UI/UX & Tích hợp (quản lý nội dung/GDD)**, lấy thống nhất nhóm nếu ảnh hưởng ý nghĩa/luật. Gameplay & Dữ liệu đánh giá contract/triển khai sau chốt; Map & Asset cung cấp bằng chứng hình khi liên quan. Chưa có biên nhận review thì **CHỜ CHỐT**, runtime tiếp tục Unclassified/ContentReviewPending như hiện có. Tài liệu này không sửa mapping hoặc nguồn nội dung.

Đối chiếu [bảng metadata gốc](NOTE-PROVENANCE-01/metadata.json) và [GDD hiện hành](../../../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md). Cách phân loại theo **cách thu nhận**, không theo mức độ đúng. “Đã phân loại nguồn” không có nghĩa sự thật đã được chứng minh.

| Mã / entry | Vấn đề và nguồn cần xác định | Căn cứ đã có trong GDD | Quyết định còn thiếu — **CHỜ CHỐT** | Đầu ra giao lại Gameplay |
|---|---|---|---|---|
| ND-01 `mechanism:1` | Một câu trộn khám khe không chèn với cơ cấu hoạt động trong lượt thử; nhãn nguồn hiện tại “Xưởng • cánh, chốt và dây nhả” chưa nói rõ cơ cấu thật hay bản sao | 3.3/3.4: khám trước thử, dùng bản sao cho thử;7.1: mô hình không là sự kiện thật;7.2: tách nguồn | Câu mô tả cùng một lượt khám/thao tác thật hay ghép kết quả mô hình? Giữ một entry hay cần nội dung tách riêng? Chưa tự chọn Quan sát | Loại/nguồn được duyệt và chỉ rõ có đổi câu/index hay không; nếu tách, Gameplay đánh giá tương thích index save trước khi làm |
| ND-02 `hook:0` | Câu trộn bụi móc ở hiện trường với tiếp xúc tạo khi thử trên mô hình; nguồn hiện tại “Xưởng / bàn mô hình • móc M” | 3.4: bụi ghi trước thử, phân biệt dấu mới;7.1/7.2: phân biệt mô hình và quan sát/nguồn | Cần giữ hai vế chung hay biểu diễn tách? Người chơi nhìn trực tiếp từng vế ở đâu, có điều kiện nào đã được duyệt? | Mapping/source và phạm vi nội dung sau chốt; không tự sửa gate hoặc biến kết quả mô hình thành sự kiện thật |
| ND-03 `samples:0` | Trạng thái có quyền xem mẫu kèm lời bàn giao, nguồn hiện tại Phương; chưa rõ entry là lời Phương kể hay ghi nhận thao tác bàn giao | 7.4: bước truy cập được thể hiện, quy trình dùng mô tả trung tính;13: mẫu thật qua bước rút gọn có giải thích | Loại ghi nhận và cách diễn đạt nguồn cụ thể; không tự coi cờ permission là quan sát đã được chứng minh | Loại/nguồn; nếu chỉ đổi nhãn không cần save/gate mới. Nếu đổi producer/cách thu, phải giao Gameplay rõ |
| ND-04 `letter:1` | Câu nói cùng pose ID trên giấy/bảng/rối, trộn quan sát hình và quy ước dữ liệu; nguồn hiện tại dải thư Hà | 5.1/5.3: tư thế ánh xạ chữ, dùng cùng pose ID;7.2: không lộ nội dung chưa xem | Entry dành người chơi quan sát hình hay ghi chú kỹ thuật nên trình bày khác? Chưa gán loại từ việc code dùng chung ID | Nội dung/loại/nguồn được duyệt, giữ thứ tự index; không tự thêm điều kiện đã tra sổ |
| ND-05 `draft:0` | Đặc điểm mép/nét thử đi kèm nhận định thiếu nguồn nối người gửi; nguồn hiện tại giấy mẫu Hà giao | 6.2/7.3: đối chiếu giấy/nguồn thư;13: prototype rút gọn. Code hiện tại khẳng định mẫu này chưa có nguồn độc lập nối Khải | Tách mô tả vật và nhận định giới hạn thế nào? Không đổi mẫu Hà thành nháp riêng Khải hoặc suy danh tính từ nguồn chưa có | Nội dung/loại/nguồn; thay nghĩa/nguồn thu cần nhóm duyệt, Gameplay rà keys/save trước triển khai |
| ND-06 `decoded:0` | Bản đọc suy ra từ mapping, gợi kiểm đường ba gói và giới hạn suy luận; nguồn hiện tại bản đọc người chơi đã ghi | 5.1/5.3: người chơi tự gán/đọc thử;7.2: tách quan sát và giả thuyết;13: một thư Hà | Đặt bản đọc/diễn giải ở loại nào và nhãn nguồn nào? Không tự ép vào Quan sát/Lời kể hoặc coi đọc được là chứng minh vụ bán rối | Mapping/nhãn nguồn được duyệt; không đổi điều kiện RecordReading/decoded hiện hành chỉ vì quyết định nhãn |
| ND-07 `trials[index]` | Report là kết quả diễn biến mô hình, không phải camera sự kiện hoặc chứng minh người gây ngã. Runtime hiện Unclassified/pending cho trial đã ghi | 7.1: “Mô hình là công cụ kiểm giả thuyết, không là camera tua lại sự thật”; mô hình đúng code không tự xác nhận vật lý;3.5/14: kiểm giả thuyết thay thế | Loại/nguồn/nhãn riêng của kết quả mô phỏng; cách tách điều quan sát trên mô hình với việc đã chứng minh về hiện trường. GDD chưa chốt enum provenance cho report | Đặc tả loại và nguồn; nếu dùng enum hiện có, Gameplay cập nhật mapping có kiểm. Nếu cần loại mới, giao thay đổi contract/consumer; không tự thêm loại hoặc đổi luật kết luận |
| ND-08 Chủ đề hỏi lại Hà | Chưa có Question/Answer, participant/topic ID, gate chính thức hoặc catalog sản phẩm. TEST ONLY không phải nội dung được duyệt | 4: quay lại cần câu hỏi/phản hồi mới;6, mini game7: hỏi lại theo tài liệu;5.2: đã giới thiệu kênh nhận giấy. Không điều nào quy định gate cụ thể bằng shoe/slide/decoded/visitedHa | Chốt phạm vi một lượt hỏi prototype hay lát cắt; từng câu/đáp án/ID; entry/điều kiện chính xác; câu trả lời chỉ nhắc nội dung cũ hay cung cấp thông tin mới. Không sao chép thoại/gate TEST ONLY làm mặc định | Catalog được duyệt. Nếu dùng hai requirement hiện có và không phát ghi nhận: UI/content có thể nối với cơ chế hiện tại; kiểu gate/cơ chế ghi mới phải giao Gameplay |
| ND-09 Lịch sử hỏi lại | Hiện không có history, không biết đã hỏi sau Load; hỏi lặp cho cùng trả lời khi điều kiện không đổi | 11: lưu lời khai đã nghe;7.2: ghi nhận đã mở. GDD chưa quy định per-topic history, branch, save key hoặc mục đích “đã hỏi” | Có cần lưu không, **để làm gì**: chỉ nhãn đã nghe, giữ bản ghi nội dung mới, tiếp tục nhánh hay chặn hỏi lặp? Quyền sửa/bỏ nhãn và ý nghĩa sau Load? Chưa mặc định chặn hỏi lặp | Chốt “không lưu” hoặc danh mục state/mục đích/key/version/compatibility. Gameplay thiết kế và kiểm save/contract chỉ sau khi có yêu cầu rõ; không thêm cờ vào schema hoặc giấu history trong relations |

GDD cung cấp hướng và giới hạn trong cột căn cứ, không tự cung cấp đáp án cho các quyết định trên. Nguồn hiện tại ở bảng metadata là nhãn triển khai đang chờ xác định cách thu nhận, không phải bằng chứng việc đó đã được xác minh. Nếu cần sửa nội dung gốc/GDD, UI/UX & Tích hợp điều phối riêng; đợt này không sửa các file đó.

Biên nhận quyết định nên ghi: mã ND, loại/nguồn hoặc thoại/ID/gate đã duyệt, lý do/căn cứ, phạm vi prototype, có đổi nội dung/index/serialized state không, người chốt và commit tài liệu liên quan. Không ghi “đã chốt” khi mới có một gợi ý hoặc chưa có người nhận.

## 4. Chuyển việc theo ownership

| Việc | Chủ sửa/nhận | Có cần Gameplay sửa không? |
|---|---|---|
| Renderer controls/preview/nhãn, adapter trong `Assets/ConRoi/Modules/GiaoDien`, scene trong `Modules/TichHop`, quản lý nội dung `Modules/NoiDung` | UI/UX & Tích hợp | Chưa cần nếu dùng API hiện có ngoài partial; gửi yêu cầu nếu cần consumer/navigation legacy |
| Model/material/collider/inspectionPrefab/source editable | Map & Asset; UI/UX & Tích hợp wire scene | Chưa cần nếu dữ liệu/binding đúng hợp đồng; lỗi resolve/cloning hoặc API thiếu gửi Gameplay có bước tái hiện |
| Bảng quyết định ND-01…09, thoại/catalog chính thức, thiết kế modal/history | UI/UX & Tích hợp chủ trì nội dung; nhóm chốt điểm ảnh hưởng thiết kế | Chốt nội dung trước; Gameplay phản hồi ảnh hưởng kỹ thuật, không tự viết lời thoại/đáp án thay nhóm |
| Đổi metadata trong `InvestigationNoteProvenance.cs`, thêm kind/event/requirement, ghi nhận mới, state lưu, consumer trong **mọi session partial** | Gameplay & Dữ liệu; contracts ở `Docs/QuanLy/HOP_DONG_MODULE.md` | Cần yêu cầu được duyệt; commit nguồn và kiểm phần chịu ảnh hưởng, không sửa song song với phía tích hợp |
| Tổng hợp HANDOFF/GDD/ownership/tài liệu chung và báo cáo scene tích hợp | UI/UX & Tích hợp | Gameplay chỉ giao nguồn/báo cáo/đề nghị, không sửa thay trong đợt này |

Có thể triển khai UI/content không chờ sửa runtime: controls/reset/preview đúng API, hỗ trợ model thiếu bằng text, nhãn pending/missing, hiển thị 28 entry rõ, chuẩn bị catalog đã duyệt trong phạm vi sở hữu, adapter interview theo hai requirement hiện có. **Không** tự resolve 6 pending trong UI bằng keyword hoặc biến Answered thành Observe/Acquire.

Phiếu chuyển Gameplay cần tối thiểu: mã gói/ND, quyết định được duyệt, file/contract dự kiến chạm, dữ liệu mẫu chính thức hoặc TEST ONLY rõ ràng, cách tái hiện/acceptance, thay đổi index/save hay không và commit xuất phát. Gameplay không cần thay runtime chỉ để cùng sửa bố cục; phía tích hợp không trực tiếp sửa session partial, kể cả partial có chữ UI.

## 5. Kiểm và biên nhận

**Cách kiểm độc lập đã có — chỉ dẫn, không chạy trong đợt này:** Unity 6000.6.2f1, scene `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`, working directory/Library riêng và profile/output mới. Dùng lệnh đầy đủ trong báo cáo từng gói, sửa đường dẫn Editor theo máy; không dùng save gốc hoặc copy file untracked vào source.

| Gói | Entry point/cấu hình để người nhận chạy khi cần | Giới hạn phải giữ |
|---|---|---|
| INSPECT | EvidenceViewChecks.RunBatch, suite mặc định; [báo cáo mục 6](INSPECT-CONTROLS-01/README.md#6-kiểm-đã-chạy-và-cách-kiểm-lại) | Mouse ảo + InputSystem.Update và gọi Update trực tiếp, không phải input Windows/natural UI |
| COMPARE | EvidenceViewChecks.RunBatch, `--view-suite comparison`; [báo cáo mục 7](COMPARE-VIEW-01/README.md#7-kiểm-độc-lập-và-lịch-sử) | Cube/line và pixel render fixture, không xác nhận readability model thật |
| NOTE | NoteProvenanceChecks.RunBatch; [báo cáo phần kiểm](NOTE-PROVENANCE-01/README.md#save-dữ-liệu-thử-và-cách-kiểm-lại) | Cần bản sao fixture schema1/hash mô tả ở gói nếu chạy nguyên suite; không thay bằng save live. Source sản phẩm không cần fixture này |
| INTERVIEW | InterviewRevisitChecks.RunBatch; [báo cáo mục 6](INTERVIEW-REVISIT-01/README.md#6-dữ-liệu-mẫu-test-only-và-cách-chạy-riêng) | Catalog Editor TEST ONLY, không thoại Hà/gate/historical state chính thức |

Tất cả entry point có namespace `ConRoi.Gameplay.EditorChecks`. Khi chỉ viết/đọc tài liệu không cần chạy lại các suite. Khi có wiring/thay đổi, chọn kiểm compile/reference và luồng chịu ảnh hưởng; không mở lại toàn bộ save/IO/trial vì nhận tài liệu.

**UI/UX & Tích hợp kiểm sau nhận, ghi commit thực chạy:** mở vật → xoay/phóng/reset → chọn hai vật và đổi bên/text-only → ghi quan sát/lời kể/liên hệ → xem nhãn/nguồn/pending → hỏi chủ đề đã được duyệt → hủy/quay lại/pause → Load và thao tác tiếp. Đặc biệt kiểm native mouse không bị feed đôi, không ảnh/đáp án cũ dưới topic khóa, không lộ nội dung chưa ghi, không grant từ Answered, không lọt di chuyển khi modal. Ca không có nội dung/wiring vẫn NOT_RUN, không dựng thoại thử để gọi là tuyến Hà hoàn chỉnh.

Báo cáo tích hợp tách: scene chung, UI1080p, API/fixture/proxy, input Windows hoặc event tổng hợp, người chơi tự nhiên. Hiện scene chung/UI1080p/người mới **NOT_RUN với các gói này**; P-close và IO-OPEN-01 OPEN riêng. Các PASS cũ không suy thành PASS scene chung.

Biên nhận cần: mốc source đã nhận, commit scene/UI thực chạy, quyết định ND đã dùng/chưa dùng, file đổi theo ownership, ca đạt/lỗi/chưa chạy, ảnh hưởng API/save, phụ thuộc còn lại/chủ nhận. Chỉ chuyển sang trạng thái tích hợp đã kiểm khi có báo cáo tương ứng; không reset nhánh công việc hoặc chốt baseline nhóm từ tài liệu này.
