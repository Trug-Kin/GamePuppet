# NOTE-PROVENANCE-01 — Nguồn của từng ghi nhận

## Acceptance trước triển khai — 10/11/2026

Xuất phát `afe461c2`, tracked sạch, hai untracked cũ giữ nguyên. GDD7.2/11 yêu cầu phân biệt cách thu nhận thông tin, **không phải chấm đúng/sai**. Chỉ bổ sung read model/metadata runtime; không đổi Observe, Acquire, AddRelation, lựa chọn kết luận, save schema1/ID/GUID hoặc UI/scene. Bảng này là ánh xạ triển khai cho entry rõ, không tuyên đã được phía nội dung review toàn bộ.

1. Phân loại explicit theo ID/index; không đoán từ từ khóa, tên NPC hay trạng thái finalized. Lời Hà là lời kể; ngoại hình Khải là quan sát. Liên hệ/claim người chơi là giả thuyết cả trước/sau xác nhận/chốt.
2. Query chỉ trả entry đã ghi; Acquire chưa Observe không tạo ghi nhận. Clip giữ cùng guard exposure trước/trong sự cố, không trả text khi chưa đủ. Không lấy raw transcript để lấp entry chưa mở.
3. Entry thiếu metadata trả Chưa phân loại/Thiếu metadata; entry mơ hồ trả Chưa phân loại/CHỜ REVIEW NỘI DUNG. ID/index không có hoặc chưa ghi: thất bại rõ, không text/nguồn và không exception.
4. Snapshot không chứa tham chiếu mutable vào save; query không ghi state/file, đổi điều kiện mở, Message, pause/input hay tiến trình. Ghi lặp không nhân đôi. Đổi câu chữ giữ cùng ID/index không tự đổi loại; thay ý nghĩa nội dung cần review mapping.
5. Save schema1 có trước gói tải được, loại được suy ra mà không thêm field; query không sửa bytes. Kiểm roundtrip liên quan và luồng Observe hiện có, không chạy bộ IO/trial cũ.
6. Kiểm bằng API/fixture trong scene Gameplay/profile mới. UI sản phẩm, scene tích hợp,1080p/người mới chưa được suy PASS. INTERVIEW-REVISIT-01 không triển khai.

## Bảng ánh xạ prototype

Khóa `id:index` dùng index observation 0-based hiện có, **không phải ID save mới**. Chỉ phân loại nội dung đã được ghi; đây là bảng phát triển có thể chứa thông tin chưa mở, không đưa nguyên bảng lên UI. Cột loại theo cách thu nhận, không xác minh sự thật. Transcript tĩnh không tự được phát như một ghi nhận mới.

| Khóa | Loại | Nguồn | Căn cứ / review |
|---|---|---|---|
| `slide:0` | Quan sát | Xưởng • mặt sàn cạnh mép gác | Nhìn dấu bụi và vị trí trên sàn. |
| `slide:1` | Quan sát | Xưởng • mặt sàn cạnh mép gác | Quan sát lớp dấu giao nhau, không kết luận người gây ngã. |
| `shoeprint:0` | Quan sát | Xưởng • dấu đế trên gác | Nhìn phần dấu đè trên bụi. |
| `shoeprint:1` | Quan sát | Xưởng • dấu đế trên gác | Nhìn vân/gót, chưa nhận diện tuyệt đối. |
| `r03:0` | Quan sát | Xưởng • nhãn R-03 và dây quấn | Đọc nhãn rối và đối chiếu vật đang sửa. |
| `r03:1` | Quan sát | Xưởng • nhãn R-03 và dây quấn | Quan sát đầu dây và đường nối vật thể; chưa xác định ai đặt. |
| `r12:0` | Quan sát | Xưởng • thẻ sửa và rối R-12 | Đọc các mã trên thẻ/vật và đối chiếu. |
| `r12:1` | Quan sát | Xưởng • thẻ sửa và rối R-12 | So hai vật có nhãn khác nhau; không suy người dàn dựng. |
| `mechanism:0` | Quan sát | Xưởng • cánh, chốt và dây nhả | Khám/thao tác trực tiếp cơ cấu hiện tại, không suy trạng thái lúc ngã. |
| `mechanism:1` | Chưa phân loại | Xưởng • cánh, chốt và dây nhả | CHỜ REVIEW NỘI DUNG: câu trộn khám khe và kết quả lượt thử, chưa rõ cùng nguồn cơ cấu thật hay bản sao. |
| `mechanism:2` | Quan sát | Xưởng • cánh, chốt và dây nhả | Nhìn vị trí tay nắm so với gác. |
| `hook:0` | Chưa phân loại | Xưởng / bàn mô hình • móc M | CHỜ REVIEW NỘI DUNG: câu trộn bụi hiện trường và so dấu thử trên mô hình. |
| `fabric:0` | Quan sát | Xưởng • mảnh vải tại tay nắm D | Nhìn mép rách và đường chỉ, không suy thời điểm. |
| `fabric:1` | Quan sát | Xưởng • mảnh vải tại tay nắm D | Nhìn sợi chỉ và đường dệt. |
| `phone:0` | Quan sát | Xưởng • điện thoại và giá quay | Quan sát hướng giá quay và trường hình. |
| `phone:1` | Quan sát | Xưởng • điện thoại và giá quay | Đối chiếu đồ trong đoạn quay với vật ở xưởng; không kết luận danh tính người kéo. |
| `clip:0` | Quan sát | Điện thoại • đoạn quay sửa R-12 | Quan sát hình trong đoạn quay sau guard exposure hiện có. |
| `clip:1` | Quan sát | Điện thoại • đoạn quay sửa R-12 | Quan sát diễn biến chốt/cánh và hai tay trong hình. |
| `clip:2` | Quan sát | Điện thoại • đoạn quay sửa R-12 | Ghi giới hạn trường hình; không suy người kéo hoặc ý định. |
| `khai:0` | Quan sát | Cửa xưởng • ngoại hình Khải nhìn thấy | Nhìn áo/giày tại cuộc gặp, khác với lời Khải trong transcript. |
| `samples:0` | Chưa phân loại | Phương • bàn giao mẫu prototype | CHỜ REVIEW NỘI DUNG: câu là trạng thái cho phép truy cập kèm lời bàn giao, chưa rõ loại entry mong muốn. |
| `coat:0` | Quan sát | Bàn mẫu xưởng • cổ tay áo được bàn giao | Nhìn phần thiếu và đường may trên áo thật. |
| `coat:1` | Quan sát | Bàn mẫu xưởng • cổ tay áo được bàn giao | Nhìn mép thiếu và điểm dừng mũi chỉ. |
| `shoe:0` | Quan sát | Bàn mẫu xưởng • đế giày được bàn giao | Nhìn góc khuyết và vân đế. |
| `shoe:1` | Quan sát | Bàn mẫu xưởng • đế giày được bàn giao | Nhìn các dãy vân và vị trí gót. |
| `shoecontrol:0` | Quan sát | Bàn mẫu xưởng • giày mẫu B | Nhìn mẫu đối chứng, không tự xác nhận khớp. |
| `ha:0` | Lời kể | Quầy Hà • lời Hà kể | Cả phần vận chuyển và giới hạn phiếu được tóm tắt từ lời Hà; không tự xác minh nội dung lời kể. |
| `ha:1` | Lời kể | Quầy Hà • lời Hà kể | Kênh nhận giấy được Hà giới thiệu; loại nguồn không chứng nhận lời kể đúng. |
| `letter:0` | Quan sát | Quầy Hà • dải thư tư thế | So hình mở đầu trên dải với bảng tập đã giới thiệu. |
| `letter:1` | Chưa phân loại | Quầy Hà • dải thư tư thế | CHỜ REVIEW NỘI DUNG: câu nêu pose ID/ánh xạ dùng chung, trộn quan sát hình và quy ước dữ liệu. |
| `receipt:0` | Quan sát | Hà giao • nội dung đọc trên phiếu vận chuyển | Đọc điều ghi trên giấy, không chứng nhận chuyến hàng thực tế hoặc hàng trong thùng. |
| `henri:0` | Quan sát | Sổ nghề Henri • nội dung bảng tập | Đọc chú giải trong sổ, không coi là lời Henri đang kể trực tiếp. |
| `draft:0` | Chưa phân loại | Hà giao • giấy mẫu và bản nháp | CHỜ REVIEW NỘI DUNG: câu trộn đặc điểm vật thể và đánh giá thiếu nguồn độc lập. |
| `decoded:0` | Chưa phân loại | Người chơi • bản đọc mật mã đã ghi | CHỜ REVIEW NỘI DUNG: bản đọc suy ra từ mapping, không ép thành quan sát hay lời kể. |
| `relations[index]` | Giả thuyết | Người chơi • liên hệ tự ghi | Note tự do đã ghi, không nâng thành sự thật khi chọn kết luận. |
| `conclusions[index].claim` | Giả thuyết | Người chơi • nhận định tự ghi | Claim không rỗng; choice/confirmation/finalized không đổi loại. |
| `trials[index]` | Chưa phân loại | Bàn dựng • kết quả mô hình | CHỜ REVIEW NỘI DUNG: kết quả thử mô hình không đồng nhất với quan sát sự kiện thật; không tự chọn loại. |

## API thực và ranh giới dữ liệu

Namespace `ConRoi.Investigation.V36.R107`; file `Assets/Scripts/InvestigationV36R107/InvestigationNoteProvenance.cs`. Nguồn `386fdfdc` chỉ thêm partial/metadata, không sửa các producer/validator/save types trước đó. Công cụ `e0b9e58a`; bảng/acceptance `57c6ad35`. Commit thực chạy **57c6ad35ceda838d2569590d65d19a5016dc7d31**, chứa nguyên nguồn/công cụ/bảng; xem kết quả bên dưới. Bản báo cáo sau đó chỉ cập nhật tài liệu.

| API | Kết quả |
|---|---|
| `bool TryGetRecordedNote(string evidenceId, int observationIndex, out InvestigationNote note, out string error)` | true: có observation đã ghi/được phép đọc, note có text/nguồn/loại/status, error rỗng. Thiếu mapping vẫn true nhưng Unclassified/MissingMetadata. false: ID/index không có, chưa ghi hoặc clip chưa đủ exposure; error tiếng Việt, note mặc định không chứa text/nguồn. Bỏ qua note khi false |
| `InvestigationNote[] GetProvenanceNotes()` | Mảng snapshot mới gồm observation đã ghi và được đọc, relation tự ghi có hai ID đã thu, claim không rỗng, report trial đã tồn tại. Không trả transcript chung, kết luận mẫu chưa viết, live trial chưa ghi hoặc mọi câu có trong definition |

`InvestigationNote` là readonly struct: `Origin`, `EvidenceId`, `RelatedEvidenceId`, `EntryIndex`, `Text`, `Source`, `Kind`, `Status`, `KindLabel`, `StatusLabel`. Không chứa tham chiếu mutable tới save. Consumer có thể thay phần tử mảng mình nhận mà không đổi state. Origin có `RecordedEvidence`, `PlayerRelation`, `PlayerConclusion`, `TrialResult`; EntryIndex là observation index hoặc vị trí list hiện tại cho origin còn lại, **không dùng vị trí list như ID bền vững để lưu liên kết mới**. Với claim, EvidenceId rỗng; với trial, đó là trial ID, không phải ID vật chứng.

| Kind / nhãn tiếng Việt | Ý nghĩa |
|---|---|
| `Observation` / Quan sát | Thông tin từ nhìn/đọc/thao tác trực tiếp. Đọc một nội dung trên phiếu chỉ xác định phiếu ghi gì, không chứng minh điều đó đã xảy ra |
| `Statement` / Lời kể | Thông tin do người khác thuật lại; chưa kiểm chứng vẫn cùng loại, không có cờ “đã đúng” |
| `Hypothesis` / Giả thuyết | Note quan hệ hoặc claim người chơi tự viết; choice/xác nhận/finalized không đổi thành Quan sát |
| `Unclassified` / Chưa phân loại | Không tự đoán cách thu nhận; phải đọc Status để biết chờ nội dung hay thiếu mapping |

| Status / nhãn | Consumer |
|---|---|
| `Classified` / Đã phân loại nguồn | Chỉ có nghĩa đã gán loại nguồn, **không** phải sự thật đã xác nhận |
| `ContentReviewPending` / CHỜ REVIEW NỘI DUNG | Hiển thị Chưa phân loại, giữ nguồn hiện có; không ẩn nội dung đã ghi chỉ vì đang review |
| `MissingMetadata` / Thiếu metadata | Entry hợp lệ/đã ghi nhưng chưa có mapping; hiển thị Chưa phân loại và báo owner, không mặc định Quan sát |

34 observation entry thuộc20 ID thực tế:28 entry rõ,6 entry chờ nội dung (`mechanism:1`, `hook:0`, `samples:0`, `letter:1`, `draft:0`, `decoded:0`). Ngoài ra, mọi trial report được giữ Unclassified/ContentReviewPending cho sáu trial hiện có (và suffix `/clear-holds`); trial ID lạ trong state in-memory trả MissingMetadata. Không xếp kết quả mô hình thành quan sát sự kiện thật. Gói này không xác nhận producer in-memory bất kỳ là hợp lệ để lưu; Load guards giữ nguyên.

Bảng [metadata.json](metadata.json) dành cho developer review, gồm ID/index/kind/status/source/rationale, không phải feed UI công khai để liệt kê nội dung chưa mở. Không thay các ID/observation index cũ. Runtime ánh xạ ordinal theo khóa, không đọc keyword trong Text, title, source hoặc lựa chọn kết luận để đoán loại. Thay chữ nhưng giữ cùng ý nghĩa/khóa không làm đổi loại; thay **ý nghĩa/cách thu nhận** hoặc sắp lại observation phải review metadata cùng nội dung. Không hứa ánh xạ cũ tự hiểu nội dung mới.

## Bàn giao UI/UX & Tích hợp

**Cần wiring consumer metadata**, UI sản phẩm chưa dùng API mới. Đề xuất nhóm ghi nhận theo Kind, kèm nguồn ngay cạnh nội dung; giữ nhãn Chưa phân loại cho pending/missing. Không dùng màu hoặc nhãn Đã phân loại để biểu thị đúng/sai. Không thêm điểm, gate kết luận, hoặc đổi điều kiện mở chứng cứ dựa trên nhãn.

1. Nhận đầy đủ chuỗi nguồn/công cụ/bảng/báo cáo; lấy reference session thật qua binding hiện có, không tìm object theo tên.
2. Lấy `GetProvenanceNotes()` khi view hồ sơ bật và sau thao tác ghi/Load/đổi claim/quan hệ; mỗi snapshot là đọc thuần. **Không có event mới trong gói**, vì không đổi producer/mọi call site cũ. Không tự gọi Observe/Acquire để tạo dữ liệu cho view. Nếu cần event vì kiến trúc UI, gửi contract yêu cầu cho Gameplay & Dữ liệu trước, không sửa partial song song.
3. Vẽ Origin/Kind/Source/Text theo snapshot. Giữ các action ghi hiện hành, không thay việc Acquire thành đã ghi. Không liệt kê raw EvidenceDefinition.observations hoặc metadata.json để dựng các mục chưa được ghi.
4. Chỉ render `note` của TryGetRecordedNote nếu true; khi false ẩn nội dung cũ và xử lý error trong action phù hợp, không spam lỗi khi repaint. Khi pending vẫn hiển thị nội dung đã ghi cùng nhãn Chưa phân loại; không tự phân loại lại trong UI bằng từ khóa.
5. Gửi6 mục mơ hồ và kết quả trial cho chủ nội dung review cách thu nhận. Đặc biệt ha0 là lời tóm tắt **từ Hà** dù có nhắc phiếu; receipt0 là điều đọc trên phiếu, không gộp hai nguồn này thành xác nhận chuyến hàng. Khai0 là nhìn trang phục, không phải transcript lời Khải. Transcript không được tự biến thành observation đã ghi trong gói này; muốn phân loại từng câu transcript cần tách nội dung được duyệt riêng.
6. Nếu consumer phải sửa legacy partial, gửi thay đổi cho Gameplay & Dữ liệu theo ownership; không sửa session partial song song. Map & Asset không có việc mới trong gói.

```csharp
// Trong adapter UI sau khi đã có reference session và view thật:
InvestigationNote[] visible = session.GetProvenanceNotes();
foreach (var note in visible)
{
    // Render note.KindLabel, note.Source, note.Text;
    // Pending/missing vẫn giữ KindLabel = Chưa phân loại.
}
if (session.TryGetRecordedNote("ha", 0, out var entry, out var error))
{
    // Chỉ render entry ở đây: Lời kể khi ha:0 đã được ghi.
}
// Không gọi Acquire/Observe chỉ để làm ví dụ này thành true.
```

Query không tự đổi trang, pause, input/focus, Message, xác nhận kết luận hay bytes file; được gọi cả khi pause hoặc ở world. Không có vòng đời event cần subscribe/unsubscribe. Cache phải bỏ/refresh sau Load/profile hoặc state thay đổi; không dùng snapshot từ phiên trước để vẽ phiên mới. Modal/input vẫn thuộc navigation hiện hành; query không cấp phép action gameplay khi modal mở.

## Save, dữ liệu thử và cách kiểm lại

Không serialize Kind/Status/Source vào InvestigationSave; Load schema1 cũ rồi suy loại từ definition/index hiện có. Không thêm migration/field/GUID cũ hoặc sửa nội dung/lời giải. Unknown index bị query từ chối, không repair state/file. Query không thay thế validator hiện hành.

Công cụ: `ConRoi.Gameplay.EditorChecks.NoteProvenanceChecks.RunBatch`. Môi trường Unity6000.6.2f1, scene `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`, môi trường proxy tạm của Gameplay và profile mới. Các vật/observations/claims/trial report cấp bằng fixture ghi rõ, không tuyến chơi tự nhiên, không kiểm UI/controller/Windows input. Fixture cố ý đổi text/definition trong try/finally, khôi phục trước lưu; không sửa asset nội dung trên đĩa.

Dữ liệu mẫu trong bảng là các ID đang có; snapshot lỗi không trả văn bản của entry chưa ghi. Save cũ lấy bản sao **profile kiểm** ComparisonRenderFixtureFixed20261109 từ gói trước (không save người chơi); hash input `28dd3593c3b06245f664709d69961e79bc1ad6a2e58a4f6417d34d2fde7e3af8`. Kiểm Load/tiếp tục Observe/lưu/tải trên profile mới, giữ source fixture cũ chỉ đọc. Không suy mọi save lịch sử hoặc mọi lỗi IO đã kiểm từ một mẫu tương thích này.

Chạy từ worktree riêng với thư mục output/profile chưa tồn tại; `$oldFixture` là **bản sao save thử schema1 có shoe/shoecontrol của fixture được mô tả**, không đường dẫn save gốc. Tool đọc file đó, chỉ ghi SavePath thuộc profile mới. Chỉnh đường dẫn Editor theo máy thực:

```powershell
$project = (Get-Location).Path
$unity = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe'
$oldFixture = "$project/Test/Output/NoteProvenance20261110/legacy-schema1-input.json"
& $unity -batchmode -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.NoteProvenanceChecks.RunBatch --provenance-source (git rev-parse HEAD) --provenance-output "$project/Test/Output/ProvenanceNew" --provenance-old-save $oldFixture --investigation-save-dir "$project/Test/PlaytestProfiles/ProvenanceNew" -logFile "$project/Test/Output/ProvenanceNew.log"
```

File fixture/save/log raw giữ local, không bắt buộc có sẵn trên máy thành viên mới; nếu cần chạy nguyên suite, yêu cầu bản sao fixture kỹ thuật theo hash, không thay bằng save live. Có thể dùng metadata/query runtime bình thường mà không có Test/Output hoặc đường dẫn tác giả.

**Kiểm lại khi tích hợp:** chơi ghi một quan sát dấu sàn → hỏi/ghi lời Hà → ghi liên hệ và nhận định → xem đủ clip rồi ghi → lưu/tải → kiểm nhóm/nguồn chính xác, entry chưa ghi không hiện, giả thuyết không biến thành Quan sát sau chọn/xác nhận, pending/missing không bị gán loại mặc định. Kiểm UI1080p/modal/input ở commit tích hợp thực chạy. Chưa có consumer nên không nhận scene chung đã đạt. Người mới NOT_RUN; IO-OPEN-01 và P-close OPEN riêng. Không triển khai hội thoại mới.

## Kết quả thực chạy

**68/68 assertion API/fixture đạt,0 lỗi suite**, trên commit **57c6ad35ceda838d2569590d65d19a5016dc7d31** với runtime **386fdfdce4b16c4943d2f7a0d936854b8b04187e**. Unity6000.6.2f1 import/compile xong và chạy suite PlayMode trong scene Gameplay; log không có compiler error hoặc exception assertion ngoài dự kiến. Không kiểm lại toàn bộ reference/asset scene chung.

- 34 entry khớp loại/status/nguồn trong bảng; Acquire chưa Observe rỗng; ID/null/index sai không trả text. Missing mapping và6 pending được phân biệt, không fallback Quan sát.
- Ghi lặp/firstFloorRecorded/guard clip giữ hành vi, query không cấp chứng cứ hoặc đổi Message/page/input/state; mảng snapshot consumer sửa không chạm save.
- Đổi text/source hiển thị trong fixture không làm đổi loại/nguồn metadata; try/finally khôi phục trước lưu. Quan hệ/claim vẫn Giả thuyết qua choice/confirmation/finalized bằng fixture, không chạy chốt ending mới.
- Clip dù có record trong dữ liệu vẫn không lộ text khi thiếu một/hai exposure; kết quả mô hình không bị gán thành quan sát hiện trường.
- Roundtrip profile mới và bản sao save thử schema1 có trước gói đều đạt; tiếp tục ghi/lưu/tải được, file fixture cũ và bytes sau query/Load chỉ đọc giữ nguyên. Không thêm metadata vào save.

[Bằng chứng đã lược](verification.json) giữ tên assertion, nguồn chính xác, môi trường và hash; profile/save/log raw ở Test/Output/NoteProvenance20261110 và Test/PlaytestProfiles/NoteProvenanceApi20261110, không commit. Không chạy input UI/Windows, không phải playtest người mới. **RUNTIME ĐÃ KIỂM → CHỜ UI/TÍCH HỢP**;6 entry/nhóm trial chờ review nội dung, không suy toàn bộ classification đã được nhóm duyệt. API không cần các file Test/Output để chạy sản phẩm.

UI/UX & Tích hợp nhận cả chuỗi `386fdfdc →e0b9e58a →57c6ad35` cùng commit bàn giao tài liệu sau đó. Không nhận riêng README. Bản nguồn không đổi scene/UI/asset/GDD/EvidenceDefinition/save guards và không triển khai INTERVIEW-REVISIT-01.
