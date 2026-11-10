# INTERVIEW-REVISIT-01 — Cơ chế hỏi lại theo chủ đề

## 1. Căn cứ và acceptance trước nguồn — 10/11/2026

Xuất phát `fc781aaa`, nhánh work/runtime-contracts-baseline, tracked sạch và hai untracked cũ bảo toàn. Chỉ cơ chế độc lập được giao lần này; đề xuất chờ trước tại afe461c2 được thay quyền triển khai **cơ chế**, không coi nội dung Hà đã được duyệt.

Trích [GDD4](../../../../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md): “Mỗi lần trở lại một nơi phải có câu hỏi hoặc phản hồi mới.” GDD6, mục mini game7: “Đối chiếu lời khai và tài liệu để hỏi lại NPC.” GDD11 yêu cầu lưu “lời khai đã nghe”, nhưng không quy định khóa lịch sử từng câu hỏi, cấu trúc save hội thoại hay gate từng chủ đề Hà. GDD5.2 quy định kênh hồi đáp ở quầy Hà, không quy định điều kiện mở hội thoại follow-up từ receipt/decoded/visitedHa. Không biến các thông tin đó thành gate mặc định.

Code hiện hành Interact(Hà) cấp các ID cũ/mở Evidence, Actor.Greet chỉ có cử chỉ chào; không có catalog chủ đề hỏi lại. Giữ nguyên hai luồng đó. Không đưa lời Hà tĩnh hiện có vào catalog mới rồi coi đó là thoại follow-up đã duyệt.

Acceptance:

1. Catalog bất biến do caller truyền vào, không catalog/thoại/gate sản phẩm mặc định; ID chủ đề/người nói là khóa ổn định ordinal, không tên nút/Hierarchy hoặc chuỗi câu hỏi dùng làm ID. Dữ liệu TEST ONLY chỉ nằm trong Gameplay/Editor.
2. Query trả snapshot chủ đề hiện tại và danh sách đang mở, không chứa Answer. Khóa/không tồn tại/sai người/context chưa mở/pause/điều kiện không hợp lệ không trả đáp án hoặc phát event trả lời.
3. Chọn hỏi kiểm điều kiện lại tại thời điểm gọi, không tin snapshot/UI đang enabled. Hỏi lặp hợp lệ trả cùng dữ liệu, không có thưởng, không Acquire/Observe/đổi mục tiêu/ghi relation/claim/finalized.
4. Chỉ hỗ trợ điều kiện dữ liệu tối thiểu tường minh: có ID evidence hoặc observation đã ghi/được đọc qua API hiện có; AND các điều kiện, rỗng là không có gate dữ liệu. Đây là khả năng cơ chế, **không phải quyết định Hà cần thu/ghi entry nào**.
5. Mở/kết thúc conversation chỉ đổi context riêng của cơ chế, không tự điều hướng/modal/input. Pause chặn hỏi; tiếp tục giữ context. Khi source.data bị thay (Load/profile replace), context cũ hết hiệu lực và không trả đáp án cho đến Begin mới. Không ghi lịch sử vào schema1 hoặc dùng visitedHa/relations làm history.
6. Kết quả/event chỉ khi hỏi hợp lệ; callback lỗi không hủy kết quả, reentrant ask trong callback bị từ chối an toàn. Không lưu last answer làm nội dung sẵn cho topic khóa. Hủy/kết thúc không tạo ghi nhận.
7. Kiểm API/fixture trong scene Gameplay/profile riêng: dữ liệu khóa/sai/lặp/mất điều kiện, AND, clip guard, pause/context/Load, event/unsubscribe/reentry và state/bytes không đổi do hỏi. Không chạy bộ save/IO/trial cũ. Nội dung Hà/scene chung/UI1080p/người mới chưa được suy PASS.

## 2. Nội dung còn chờ quyết định

- Câu hỏi/đáp án chính thức, participant/topic ID được duyệt và phạm vi lượt hỏi lại Hà của prototype hay lát cắt.
- Điều kiện cụ thể: nhận vật, đã ghi observation, đọc thử mật mã hay decoded; không tự dùng “nhặt đủ” hoặc visitedHa làm gate.
- Có cần lưu lần nghe/lựa chọn/nhánh không? Nếu có, duyệt key bền vững/representation/migration tương thích; chưa thêm field save, cờ hoặc log sự kiện.
- Nếu câu trả lời cung cấp ghi nhận mới: quy tắc người chơi ghi/thu nhận và phân loại nguồn; cơ chế hiện tại không tự phát vật chứng/ghi quan sát.
- UI/UX & Tích hợp chủ trì review **6 entry provenance đang chờ** và **phân loại kết quả bàn dựng** riêng. Không tự coi report mô hình là quan sát sự kiện thực; gói hội thoại không chốt lại nhãn nào.

## 3. Nguồn và trạng thái bàn giao

**CƠ CHẾ ĐÃ KIỂM / CHỜ NỘI DUNG & UI.** Không phải hội thoại Hà đã hoàn chỉnh.

- Nguồn `97b8048e` + `e92a2ea6`: `Assets/Scripts/InvestigationV36R107/InvestigationInterviewData.cs`, `InvestigationInterview.cs` và meta mới. Commit thứ hai giữ snapshot mặc định không thể hỏi; không dùng ordinal enum như định dạng save.
- Công cụ/catalog TEST ONLY: `d5b67c9a`, `Assets/ConRoi/Modules/Gameplay/Editor/InterviewRevisitChecks.cs` +meta. Đây là commit thực chạy **d5b67c9a6ba1ae0310047284555a4221e5023502**; runtime nguyên hai nguồn trên. Nhận toàn chuỗi cùng báo cáo, không chỉ commit tài liệu cuối.
- Không sửa session partial hiện có, UI, map, scene, GDD, registry, provenance mapping hoặc save types/guards. Không đăng ký catalog mới vào sản phẩm. Runtime không cần Test/Output/Editor fixture để hoạt động với catalog được duyệt sau này.

## 4. API thực tế

Namespace `ConRoi.Investigation.V36.R107`; dùng trên Unity main thread với source InvestigationSession thật đã khởi tạo. Cơ chế là plain C# object, không tự tìm NPC hoặc object theo Hierarchy.

| API/type | Dữ liệu / kết quả |
|---|---|
| `InterviewRequirement(InterviewRequirementKind kind, string evidenceId, int observationIndex = -1)` | Điều kiện khai báo; `AcquiredEvidence` dùng index−1, `RecordedObservation` dùng index0-based. Các điều kiện AND, mảng rỗng không có gate dữ liệu. Không mặc định thành luật Hà |
| `InterviewTopicDefinition(string id, string participantId, string question, string answer, params InterviewRequirement[] requirements)` | Dữ liệu bất biến; ID/question/answer phải có, ID không có khoảng trắng đầu/cuối; clone mảng vào và khi đọc Requirements. Không có catalog official mặc định |
| `new InvestigationInterview(InvestigationSession source, IEnumerable<InterviewTopicDefinition> definitions)` | Giữ catalog theo thứ tự truyền vào, clone collection; topic ID duy nhất toàn catalog và so ordinal/case-sensitive. Null source/catalog/topic, ID trùng hoặc definition thiếu nội dung bị ArgumentException/ArgumentNullException cho developer; không hiện stack trace ra UI |
| `TryBeginInterview(string participantId, out string error)` | true khi có participant trong catalog/source sẵn sàng/không pause; mở context riêng. Không kiểm khoảng cách NPC hay tự cho phép truy cập nội dung truyện. false có lý do, giữ context cũ |
| `ActiveParticipantId` | ID context hợp lệ; rỗng khi chưa mở/đã End/source.data đã thay. Pause vẫn giữ ID |
| `GetTopics()` | Snapshot mới các chủ đề của participant active, gồm Available/Locked/lỗi; không có Answer. Rỗng khi không có context hợp lệ |
| `GetAvailableTopics()` | Chỉ các snapshot CanAsk=true; không tự đánh dấu đã nghe |
| `GetTopicState(string topicId)` | Kiểm context và điều kiện hiện tại. Snapshot Id/Question/Availability/Reason/CanAsk; Question rỗng khi không Available; không có trường Answer |
| `TryAsk(string topicId, out InterviewReply reply, out string error)` | Recheck context/điều kiện tại lúc chọn. true: Reply có TopicId/ParticipantId/Question/Answer, error rỗng. false: Reply mặc định không text và error tiếng Việt; không phát Answered |
| `EndInterview()` | Dọn context riêng, lặp an toàn; không ghi lịch sử, không cấp vật/đổi data, không tự đóng UI |
| `event Action<InterviewReply> Answered` | Mỗi lần hỏi thành công phát một lần, **hỏi lặp thành công vẫn phát lại**. Không phải event thưởng/đã ghi lời khai. Callback exception được cô lập bằng warning tên type, không hủy kết quả; TryAsk đệ quy trong callback bị từ chối |

Chỉ hai requirement kind hiện có. AcquiredEvidence đọc membership evidence như Has hiện hành, **không** đồng nghĩa đã xem/đã ghi. RecordedObservation dùng TryGetRecordedNote, nên giữ guard clip và không yêu cầu provenance đã phân loại: một entry pending nhưng đã ghi vẫn là đã ghi, không dùng review status làm gate mới. ID/index/kind điều kiện sai trả InvalidCondition, không fallback mở. Không hỗ trợ tự suy điều kiện từ Question/Answer, mã mật thư, choice, visitedHa hay từ khóa.

Dữ liệu definition chứa Answer cho author/loader; **không truyền definition trực tiếp vào renderer người chơi**. Renderer chỉ lấy TopicState; đáp án chỉ lấy Reply sau TryAsk=true. Không hứa giữ bí mật trước công cụ đọc asset/source của developer, mà bảo đảm biên API không trả nội dung chưa được mở.

## 5. State, lỗi và lifecycle

| Availability | Ý nghĩa / xử lý UI |
|---|---|
| Available | Có thể hỏi tại thời điểm query; vẫn phải gọi TryAsk để recheck |
| Locked | Chưa đạt điều kiện, Question rỗng, không Reply; Reason trung tính, không lộ ID/chứng cứ hoặc đáp án đang khóa |
| UnknownTopic | ID null/lạ/sai chữ hoa hoặc topic thuộc participant khác; không trả lời |
| NoConversation | Chưa Begin/đã End; default snapshot cũng không CanAsk |
| ContextExpired | source.data đã được thay, ví dụ Load; Begin lại với nguồn hiện hành |
| Paused | Không hỏi/Begin mới; giữ participant để resume, GetAvailableTopics rỗng |
| InvalidCondition | Dữ liệu điều kiện không hợp lệ, giữ khóa và gửi lỗi cấu hình cho owner; không tự sửa catalog |
| SessionUnavailable | Source bị hủy hoặc data chưa sẵn sàng; không hỏi |

Không có last reply hoặc danh sách đã hỏi trong cơ chế. Query thuần không phát event/đổi source state; TryAsk không ghi evidence, observations, mục tiêu, trial, relation, claim, confirmation, finalized, Message hoặc file. Không có phần thưởng để nhận lặp. Reply đã phát cho action hợp lệ không được coi là ghi nhận đã lưu hoặc chứng cứ đúng; consumer không tự cấp vật trong Answered.

Không có event tự động cho Begin/End/đổi điều kiện/Load. UI refresh state sau các action đó hoặc khi vẽ view; **không gọi TryAsk trong repaint/Update để ép câu trả lời**. Đăng ký Answered một lần khi adapter được tạo/bật, hủy khi tắt/hủy, tránh tạo nhiều instance còn giữ handler cũ. Snapshot mảng có thể thay phần tử mà không đổi catalog.

Context giữ reference source.data tại Begin; Load hợp lệ đổi data nên context hết hiệu lực, không trả lại đáp án cũ dù UI vẫn giữ snapshot Available trước Load. Đổi điều kiện **trong cùng data** không cần Begin lại nhưng TryAsk luôn recheck. Nếu đổi profile bằng SaveDirectoryOverride mà chưa thay data, adapter phải End tường minh; cơ chế không tự đoán profile ID từ đường dẫn. Không tái sử dụng Reply cũ cho profile hoặc topic khác.

## 6. Dữ liệu mẫu TEST ONLY và cách chạy riêng

Catalog mẫu thực nằm trong **Editor**: `ConRoi.Gameplay.EditorChecks.InterviewRevisitChecks.CreateTestOnlyCatalog()`. Participant `test.only.interview.a` / `test.only.interview.b`, mọi câu/đáp án bắt đầu **TEST ONLY**, không dùng tên Hà hoặc trình bày như thoại chính thức. Không copy catalog mẫu vào data sản phẩm hoặc build; source runtime không chứa các câu thử.

| Topic ID TEST ONLY | Điều kiện thử, không phải GDD gate | Đáp án thử |
|---|---|---|
| test.open | Không requirement | TEST ONLY: đáp án A |
| test.acquired | Có shoe | TEST ONLY: đáp án B |
| test.recorded | Đã ghi slide:0 | TEST ONLY: đáp án C |
| test.and | Có shoe AND đã ghi slide:1 | TEST ONLY: đáp án D |
| test.clip | Đã ghi/được đọc clip:0 | TEST ONLY: đáp án E |
| test.other | Participant B, không requirement | TEST ONLY: đáp án người B |
| test.invalid-id / index / kind / acquired-index | Cấu hình cố ý sai | Không được trả đáp án |

Công cụ `InterviewRevisitChecks.RunBatch` mở scene `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`, dựng proxy tạm, dùng profile mới. Không sửa/lưu scene thử hoặc scene chung. Fixture cấp evidence/observation và thay data reference có nhãn rõ, không tuyến chơi tự nhiên. Một roundtrip Save/Load riêng chỉ để kiểm context hết hiệu lực và không thêm history; không chạy lại bộ save/IO/trial/provenance cũ.

```powershell
$project = (Get-Location).Path
$unity = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe' # đổi thành Editor6000.6.2f1 thật trên máy
& $unity -batchmode -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.InterviewRevisitChecks.RunBatch --interview-source (git rev-parse HEAD) --interview-output "$project/Test/Output/InterviewNew" --investigation-save-dir "$project/Test/PlaytestProfiles/InterviewNew" -logFile "$project/Test/Output/InterviewNew.log"
```

Output/profile phải chưa tồn tại; source save gốc không dùng. API cơ chế không cần file save hoặc dữ liệu TEST ONLY trên máy tác giả.

## 7. Wiring cần UI/UX & Tích hợp cung cấp

**Cần wiring mới và nội dung được duyệt**, không có conversation Hà chơi được trong sản phẩm ở gói này.

1. Nội dung cung cấp catalog chính thức: participant/topic ID ổn định, Question/Answer và requirements được duyệt. Gói hiện tại không có loader asset hoặc danh mục chính thức; adapter/loader sau đó chuyển dữ liệu được duyệt thành InterviewTopicDefinition. Không hardcode đáp án vào cơ chế.
2. Adapter nhận reference InvestigationSession và tạo một InvestigationInterview với catalog đã duyệt. Khi người chơi chủ động nói chuyện đúng NPC/context được thiết kế, gọi TryBeginInterview; xử lý false/error trung tính, không lộ exception cấu hình.
3. Vẽ GetAvailableTopics hoặc GetTopics theo quyết định UI. Locked có Question rỗng, nên dùng nhãn trung tính hoặc ẩn, không lấy raw definition để hiện câu chưa mở. Chỉ gọi TryAsk khi người chơi chọn, dùng Reply chỉ khi true; false xóa nội dung cũ gắn topic mới và hiển thị Reason/error phù hợp.
4. Nối Answered hoặc dùng trực tiếp Reply tùy adapter, **không xử lý cả hai để trình bày/ghi hai lần**. Event lặp không được tự cấp thưởng/ghi nhận; việc ghi lời khai nếu được yêu cầu sau này cần contract riêng được duyệt.
5. End khi người chơi hủy/trở lại, rời NPC hoặc đổi view/profile; xóa reply UI cũ và refresh query sau Load. Pause game chặn hỏi, resume có thể tiếp tục context còn hợp lệ. Không dùng pause game để thay cho modal hội thoại rồi đòi TryAsk hoạt động trong pause.
6. Modal, focus, khóa đi/nhìn và trả P/E/Esc thuộc wiring/navigation cần được phía UI thiết kế/kiểm. Cơ chế không tự OpenPage, SetPaused hoặc khóa movement/look. **Chưa có trang interview trong UI sản phẩm**; không tự gọi OpenPage với tên trang bịa. Nếu cần sửa session partial để điều hướng, gửi yêu cầu cụ thể cho Gameplay & Dữ liệu, không sửa song song.
7. Kiểm callback subscribe/unsubscribe và tuổi thọ instance. Câu trả lời chỉ là lời người nói; nếu hiển thị nhãn nguồn, dùng ParticipantId/cách ghi nhận được duyệt, không coi Answered là bằng chứng đã xác nhận.

Ví dụ adapter minh họa — `approvedTopics`/`approvedParticipantId` là dữ liệu phía nội dung phải cung cấp, **không phải catalog hoặc API có sẵn trong repo**:

```csharp
var interview = new InvestigationInterview(session, approvedTopics);
interview.Answered += OnReply; // OnReply(InterviewReply) do adapter triển khai
if (interview.TryBeginInterview(approvedParticipantId, out string error))
{
    InterviewTopicState[] available = interview.GetAvailableTopics();
    // Vẽ lựa chọn; tại action chọn của người chơi:
    // bool ok = interview.TryAsk(selectedTopicId, out var reply, out error);
    // Chỉ hiển thị reply nếu ok; không Acquire/Observe từ event mặc định.
}
// Khi view đóng/hủy:
interview.EndInterview();
interview.Answered -= OnReply;
```

## 8. Lịch sử/save và quyết định còn mở

Không thêm history/field/save ID. Begin/End/đã hỏi chỉ tồn tại tạm trong instance; hỏi nhiều lần cùng điều kiện cho cùng đáp án, instance mới không biết người chơi đã hỏi trước. ID chủ đề mới không đưa vào evidence/relations hoặc visitedHa để giả làm lịch sử.

Phía nội dung/UI cần chốt:

- **Thông tin đã có, chỉ hỏi lại:** có thể giữ cơ chế không lưu history; điều kiện suy từ record cũ. Cần xác nhận có cần hiển thị “đã nghe” sau Load không, không mặc định có.
- **Câu trả lời mới cần ghi vào hồ sơ:** chỉ định entry nguồn/ID/index và hành động ghi rõ ràng được duyệt; cân nhắc dùng record hiện có nếu đúng ngữ nghĩa, không tái sử dụng slot sai nghĩa. Cơ chế hiện tại không tự ghi hoặc phát vật chứng.
- **Nhớ từng topic/nhánh/lựa chọn:** cần contract/history representation, ID/version và tương thích schema cũ trước khi thay serialized state. Không tự thêm list/cờ hoặc xóa lịch sử cũ để có demo.
- **Gate không thuộc hai requirement kind hiện có:** giao yêu cầu rõ theo GDD/nội dung, mở rộng contract có kiểm; không đổi gate âm thầm hoặc tự suy từ decoded/visitedHa/pose text.

Review6 entry provenance vẫn **CHỜ REVIEW NỘI DUNG**. **Review kết quả bàn dựng/trial report là đầu việc nội dung riêng**: phân biệt điều thấy trên mô hình với sự kiện thật, chưa chọn Quan sát/Lời kể/Giả thuyết thay nhóm. Gói này không đọc/chốt nhãn đó hoặc dùng trạng thái review để khóa/mở câu hỏi.

## 9. Kết quả và giới hạn

**48/48 assertion API/fixture đạt,0 lỗi suite** trên `d5b67c9a6ba1ae0310047284555a4221e5023502`, Unity6000.6.2f1. [Bằng chứng đã lược](verification.json) có hash kết quả và từng assertion. Source là97b8048e +e92a2ea6; runtime không sửa sau phiên. Warning `Interview listener failed: InvalidOperationException` là callback cố ý trong fixture; kết quả trả lời vẫn true và reentry bị chặn.

Đã kiểm catalog/context/ID sai/khóa, không có Answer trong query, recheck snapshot cũ, acquired khác recorded, AND, clip exposure, repeat không đổi state/thưởng, dữ liệu immutable, pause/resume/cancel/switch participant, event/unsubscribe/reentry, save bytes không bị hỏi ghi lại, Load làm context hết hiệu lực và instance mới không có history. Đây là **API/proxy với TEST ONLY**, không UI/controller/input Windows hoặc chơi tự nhiên.

Raw profile/log/results ở Test/PlaytestProfiles/InterviewRevisitApi20261110 và Test/Output/InterviewRevisit20261110, không commit. Scene chung/UI1080p/người mới **NOT_RUN**; thoại Hà/gate chính thức/history chưa hoàn tất. P-close và IO-OPEN-01 OPEN riêng. Không lấy kết quả cơ chế làm PASS hội thoại chính thức hoặc toàn tuyến điều tra.

Khi nội dung và UI sẵn sàng, kiểm tích hợp trên commit thực chạy: trở lại NPC đúng tuyến → chủ đề chưa mở không lộ → đạt điều kiện được duyệt → chọn/lặp/hủy → pause/resume → Load và Begin lại → không mất/nhân đôi ghi nhận, input/modal đúng. Ghi riêng ca fixture với tuyến chơi tự nhiên và các phụ thuộc chưa nối. Không chốt baseline nhóm từ gói này.
