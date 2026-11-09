# TRIAL-CONTEXT-01 — Tạm ngưng diễn biến khi rời bàn dựng

**HOÀN TẤT PHẠM VI, CHỜ REVIEW/TÍCH HỢP — 09/11/2026.** Gameplay & Dữ liệu thực hiện theo yêu cầu triển khai sau đề xuất `7cfcd7ca`. Nhánh `work/runtime-contracts-baseline`, worktree `C:\ConRoi_Nguoi2`; không reset, push hoặc merge. Phần audit phía dưới được giữ nguyên như lịch sử; các câu “chưa giao/chưa chạy” ở đó không phải trạng thái hiện hành.

## Kết quả và cách nhận

- Nguồn: `8c37b8665d7758a090ff34b138fae164200d75cf` (`8c37b866`). Chỉ thêm `Page=="reconstruction"` vào điều kiện tự bước timeline trong `InvestigationSession.Update()`. Không gọi pause/resume khi đổi trang; `TrialRunning` tiếp tục biểu thị lựa chọn chạy/pause tay.
- Công cụ và commit thực sự kiểm: `b09c77908219efe47b9e6876ce21b82c6606ab74` (`b09c7790`), chứa nguồn trên. API/GUI đều chạy Unity **6000.6.2f1**. Nhận cả chuỗi **7cfcd7ca → 8c37b866 → b09c7790 → commit tài liệu kết quả**; không chỉ nhận báo cáo.
- Không wiring mới. UI/UX & Tích hợp review điều kiện Page, quy tắc quay lại và bằng chứng; sau nhận gói kiểm chức năng chịu ảnh hưởng trên scene tích hợp do mình sở hữu. Không cần sửa view, binding, schema, ID hoặc GUID; các .meta công cụ mới có GUID riêng, GUID cũ giữ nguyên.
- Không sửa UI/presentation, TrialTimeline, ReturnGateMechanism, tốc độ hồi, GDD, map/asset, scene Gameplay hoặc scene tích hợp. Không thêm điều kiện “đã xem”, không khóa kết luận hoặc thay record cũ. `AdvanceTrial()` vẫn phục vụ seek/API tường minh; thay đổi chỉ chặn đồng hồ tự động ngoài trang.

## Acceptance và bằng chứng đã chạy

| Điều kiện | Kết quả / loại kiểm |
|---|---|
| Baseline thực sự chạy nền | API: 6/6 ca tái hiện; UI: R, Esc, P, nút Trở lại/Hồ sơ thực sự đổi trang và timeline vẫn tiến |
| Sáu ID release/help/hold/hook/wedge/after giữ thời gian, gate, report, record và ý định chạy ngoài trang | PASS API, gồm vắng lâu hơn duration và world/dossier/compare/inspect |
| Quay lại không cộng thời gian vắng mặt; qua lại nhiều lần | PASS API cả sáu ca; PASS UI ca release, P vắng12 giây |
| Pause tay vẫn pause; global pause/resume | PASS API cả sáu ca; PASS UI nút pause, hai lượt rời/quay lại và Esc pause/resume |
| Seek không cấp record; đổi góc giữ thời điểm; clear-holds giữ luật | PASS API seek cả sáu, clear-holds release/hook/wedge khi đủ điều kiện; PASS UI seek/đổi góc khi pause |
| Đổi ca, hoàn tất đúng một record; mở lại không sửa record cũ | PASS API; PASS UI hoàn tất release sau quay lại và mở lại ca hoàn tất |
| Save/load theo thiết kế hiện có | PASS một roundtrip API và UI: giữ records, reset trial sống; chơi/chạy tiếp được. Không thêm lưu timeline |
| P/R/E/Esc, đi lại và khóa modal | PASS UI vào cơ cấu/bàn bằng E, các đường thoát, world movement, modal và global pause khóa movement |

**API sau sửa: 103/103 assertions đạt, 0 lỗi** ([fixed-api.json](fixed-api.json)). Đây là fixture/API Play Mode với `Time.captureDeltaTime=.25`, mở trang/pause/seek tường minh; không phải controller hoặc nhịp thời gian tự nhiên. So gate đầu ra với `TrialTimeline.At` và settled; không tuyên bố đo vật lý thực địa. Kiểm đúng ba clear-holds hợp lệ; không áp clear cho ca không đủ điều kiện.

**UI/controller: 26/26 assertions đạt**, input Windows tổng hợp ([ui-summary.json](ui-summary.json)). Từ intro sạch → bỏ hướng dẫn → đi ngang/nhìn → E khám mechanism → R → đi lại → E mở bàn; không cấp evidence hoặc mở trang bằng bridge/API. Dossier có nút bàn sau khi tự khám cơ cấu. Chọn release, rời qua R/Esc/P/header Trở lại/header Hồ sơ rồi quay lại qua E/nút bàn; thời gian và record giữ nguyên ngoài trang. Pause tay → qua lại hai lần vẫn pause → đổi góc/seek → R/Esc pause game → di chuyển → E vào modal/khóa movement → chọn release → P/vắng12 giây → quay lại/chạy hết, record0→1 → mở lại giữ record → F5 → đổi ca → F6 giữ record/reset trial → E/chạy tiếp tạo record thứ hai. Không có lượt input mất focus hoặc ngoại lệ sản phẩm ghi nhận trong tuyến này; mỗi lần gửi input đều kiểm PID foreground, snapshot được lưu trước input và trước stop.

UI dùng **môi trường proxy tạm** dựng bởi công cụ trong `Gameplay_ThuNghiem.unity`, không lưu scene: floor/controller, ba tương tác và stage/modelGate proxy. Nó kiểm navigation/lifecycle runtime, không chứng minh người chơi tìm bàn trong xưởng thật hoặc chất lượng model/ánh sáng. Game view **1536×746**, desktop1920×1080; không gọi đây là kiểm UI1080p hoặc playtest người mới. Compare/inspect exit và clear-holds chi tiết chỉ API; không tuyên bố đã đi hết sáu ca bằng controller. Drag thanh đang giữ chuột rồi đổi trang chưa kiểm controller riêng; lifecycle drag không sửa.

## Baseline trước sửa

[baseline-api.json](baseline-api.json): scene/profile đạt; sáu assertion freeze thất bại đúng lỗi đề xuất. Từ0,5 giây, ngoài trang dossier vượt hết duration và mỗi ca thêm một record:

| Ca | Trước (s) | Sau (s) | Record tổng trước→sau |
|---|---:|---:|---:|
| release | 0,5 | 11,74157 | 0→1 |
| help | 0,5 | 12,54989 | 1→2 |
| hold | 0,5 | 11,89157 | 2→3 |
| hook | 0,5 | 14,64986 | 3→4 |
| wedge | 0,5 | 3,908357 | 4→5 |
| after | 0,5 | 9,083281 | 5→6 |

Baseline kiểm runtime7cfcd7ca cùng công cụ mới khi còn untracked; công cụ sau đó được commit tại b09c7790. Không coi checkout7cfcd7ca tự nó đã chứa tool.

UI baseline: R 0,698→4,580s; Esc 0,624→4,519s; header Trở lại 0,736→4,417s trong world. P: 0,783→11,742s ở dossier, record0→1; quay lại thấy ca đã hoàn tất. Nút Hồ sơ cũng thực sự rời trang và timeline tiếp tục. Timestamp UTC, gate/report/count trước/sau và snapshots rút gọn nằm trong ui-summary; ảnh/log/input raw tra theo manifest. Sau sửa, P-left03:37:59.579Z và P-hidden03:38:12.310Z cùng TrialTime0,603055/record0; quay lại hoàn tất lúc03:39:12.569Z, TrialTime11,741573/record1. Sau save/load và chơi tiếp, record2 lúc03:39:33.422Z.

Lượt API đầu chưa chạy test vì lỗi compile **công cụ mới** thiếu defaultValue của SessionState.GetString. Đã sửa riêng tool rồi chạy baseline retry; giữ log lần đầu, không tính là FAIL runtime hay xóa lịch sử. Baseline/API sau sửa và GUI hoàn tất không có compile error sản phẩm; các cảnh báo compiler legacy, Unity AI/licensing/cloud ngoài task không được lấy làm bằng chứng lỗi gameplay.

## Chạy lại đúng phạm vi khi cần

Đóng mọi Editor đang ghi worktree, kiểm status/diff và dành desktop trước UI. Dùng profile tuyệt đối **chưa tồn tại**, không trỏ profile gốc. Từ project clone của mình, thay `<PROJECT>`, `<PROFILE_MOI>`, `<OUTPUT>` bằng đường dẫn thực; output nằm trong Test/Output để không track raw:

```powershell
& '<UNITY_EDITOR>\Unity.exe' -batchmode -nographics -projectPath '<PROJECT>' -executeMethod ConRoi.Gameplay.EditorChecks.TrialPlaybackContextChecks.RunBatch -logFile '<OUTPUT>\Editor.log' --investigation-save-dir '<PROFILE_MOI>' --trial-context-output '<OUTPUT>' --trial-source b09c7790
```

GUI: cùng Editor/project/profile mới, bỏ batchmode/nographics và dùng `-executeMethod ConRoi.Gameplay.EditorChecks.TrialContextControllerChecks.StartGui --trial-controller-output '<OUTPUT>'`. Bridge chỉ snapshot/lifecycle; không gửi lệnh cấp dữ liệu/trang chơi. Tool/controller không thay thế kiểm scene tích hợp. Baseline `--trial-baseline yes` chỉ nên chạy ở nguồn baseline riêng, không đảo/reset project hiện tại.

## Lưu giữ và giới hạn bàn giao

- [evidence-manifest.json](evidence-manifest.json): path/bytes/SHA256 raw, source hash và mốc kiểm. Raw tại `Test/Output/TrialContext01/` trên worktree tác giả; profile mới tại `Test/PlaytestProfiles/TrialContext01_*`, giữ ngoài Git. Không copy snapshot/full save/log thô/capture vào commit. Bản Git giữ báo cáo, summary và kết quả API nhỏ để review.
- Đã dừng Play/quit đúng phiên sở hữu, không save scene, sceneDirty=false. Settings Unity sinh chỉ đổi newline; copy bytes sau phiên rồi đối chiếu normalized với HEAD trước trả newline về dạng checkout. Không discard semantic diff. Untracked `.vscode/` và `Test/KetQua/W_route_Unity_v30_20261016/` cũ giữ nguyên.
- Không chạy lại bộ save43/54 ca vì không đổi luồng save. Chỉ kiểm roundtrip chịu ảnh hưởng trial như trên. Save gốc, checkpoint, archive và4.183 legacy giữ nguyên.
- UI-LOAD-FEEDBACK/hiển thị lý do Load và P-close của các gói trước vẫn **OPEN riêng**; không đóng bằng ca P của task này. UI1080p/người mới **NOT_RUN**. Chưa kiểm trực tiếp scene tích hợp, chưa push/merge.

---

# Audit/đề xuất lịch sử tại 7cfcd7ca

**Trạng thái: ĐỀ XUẤT, CHƯA ĐƯỢC GIAO TRIỂN KHAI.** Chủ đề xuất/nguồn dự kiến: Gameplay & Dữ liệu. UI/UX & Tích hợp review nhịp quay lại bàn và nhận gói. Đối chiếu source ở `a954a6a6ca270c69a1216c235e0465bfc4df53fe`, nhánh `work/runtime-contracts-baseline`. Chỉ đọc source/tài liệu; chưa chạy Unity, API fixture hoặc controller để tái hiện task này. Không sửa runtime, cơ cấu, UI hoặc scene.

## 1. Căn cứ và hạn chế người chơi

Người chơi chọn một diễn biến rồi rời bàn để khám/đối chiếu. Diễn biến vẫn chạy khi trang bàn dựng không còn mở; lúc quay lại có thể đã kết thúc, hiện báo cáo cuối và thêm `TrialRecord` dù phần diễn biến đó không được trình bày trong thời gian người chơi ở ngoài trang. Họ mất đoạn cần quan sát và phải chạy lại thủ công để hiểu báo cáo. Đây là khoảng thiếu về lifecycle mô phỏng quan sát, ngoài hệ save; không đề xuất thay luật hoặc lời giải.

Căn cứ GDD hiện hành [v3.6](../../../../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md):

- Mục7.1: mô hình là công cụ thử giả thuyết; phản hồi phải là diễn biến cánh/dây/hình nộm và khác biệt với hiện trường, không tự xác nhận danh tính.
- Mục7.2: “Chưa xem nội dung thì sổ không tiết lộ nó.”
- Mục6: khám/đối chiếu/thử giả thuyết/cập nhật suy luận là các bước có thể qua lại.
- Mục14: lifecycle không lộ nội dung chưa xem. Giữ hợp đồng hiện hành: seek không cấp quan sát; sáu diễn biến dùng cùng luật hồi hữu hạn.

**GDD không quy định nguyên văn “đóng trang phải pause trial”.** Tạm ngưng đồng hồ ngoài trang là phương án tối thiểu đề xuất để tránh kết quả chạy nền, cần review trước khi sửa. Chỉ ràng buộc ngữ cảnh trang đang mở, không tuyên bố đo được người chơi thực sự nhìn màn hình.

## 2. Đối chiếu tuyến runtime hiện có

| Bước | Producer/consumer thực tế đã đọc |
|---|---|
| Khám | `Interact()` → `Acquire()`/`OpenEvidence()`; definition/ID tại InvestigationEvidence.cs |
| Ghi quan sát | `RecordVisibleObservations()` → `Observe()`; clip có điều kiện exposure riêng |
| Mục tiêu | `CurrentObjective` đọc firstFloorRecorded/firstNotesReviewed, phone, clip, mechanism, visitedHa/returnedToWorkshop |
| Mật mã | `Assign()` → `ReadCipher()` → `RecordReading()`; bảng gán và cách đọc đã thử |
| Đối chiếu | `AddRelation()` thêm suy nghĩ người chơi, không chấm thành sự thật |
| Dựng lại | `StartTrial()` → `Update()`/`AdvanceTrial()` → `TrialTimeline.At()` → `TrialReport`/`data.trials` |
| Kết luận | Bốn `ConclusionRecord` với claim/grounds/choice; màn hiện có cho chọn “Chưa đủ chứng minh” và xác nhận nhận định prototype |

Đây là bản đồ đọc code, không phải chứng nhận tuyến đầu-cuối đã chơi đạt. Chỉ chọn khoảng thiếu lifecycle bàn dựng dưới đây; không mở rộng các bước khác.

## 3. Dấu vết source cụ thể

- [InvestigationSession.cs](../../../../../Assets/Scripts/InvestigationV36R107/InvestigationSession.cs), `Update()` dòng105: `if(TrialRunning&&!trialScrubbing)AdvanceTrial(Time.deltaTime);` chỉ nằm trong `!Paused`, **không kiểm Page**. Trong cùng block, clip đã có điều kiện `Page=="clip"`.
- `ClosePage()` dòng142 và `OpenPage()` dòng141 đổi Page/modal/focus, không hủy hoặc dừng TrialRunning. `SetModalState()` chỉ bật stageCamera khi `Page=="reconstruction"`.
- `StartTrial()` dòng195 đặt TrialRunning=true; `AdvanceTrial()` dòng200–215 khi timeline kết thúc/settled đặt TrialReport và tự thêm TrialRecord, không kiểm trang có đang trình bày bàn dựng không.
- [InvestigationReviewUI.cs](../../../../../Assets/Scripts/InvestigationV36R107/InvestigationReviewUI.cs), `ReconstructionUI()` hiển thị trạng thái và TrialReport ở bàn dựng. `SeekTrial()` dòng61 đã tạm chặn trialRecorded khi tua, nên **không đề xuất sửa lại seek như một lỗi mới**.
- `data.trials` là dữ liệu record, hiện không được DossierUI liệt kê như một trang kết quả riêng. Không tuyên bố người chơi đã thấy TrialRecord trong hồ sơ; hạn chế thấy được là TrialReport/thời điểm mô hình khi quay lại bàn.

Đường đi suy ra từ source, **chưa chạy tái hiện**: mở bàn → chọn release → đóng trang/P sang hồ sơ → Update tiếp tục AdvanceTrial trong khi stageCamera tắt → hết timeline, settled → report/record được tạo → quay lại bàn thấy mô hình ở cuối. Không cần giả định lỗi asset hoặc thay luật physics để có đường đi này.

## 4. Hành vi mong muốn đề xuất

Khi đang phát và Page rời `reconstruction`, ngừng bước đồng hồ/tiến trạng thái mô hình; không tự hoàn tất hoặc thêm record ngoài trang. Quay lại cùng trang tiếp tục từ thời điểm đã tạm ngưng nếu trước đó đang phát. Nếu người chơi đã bấm tạm dừng trước khi rời thì vẫn tạm dừng khi quay lại. Global pause tiếp tục dừng như hiện tại.

Giữ sáu ID và TrialTimeline/ReturnGateMechanism không đổi; không thêm chứng cứ, chấm giả thuyết, khóa kết luận hoặc buộc xem đủ mọi diễn biến. Không làm mất TrialRecord đã có. Không thay đổi schema/ID/GUID hay cơ chế đọc/ghi save. Không yêu cầu viết lại controls/view hoặc thêm nút mới.

## 5. File và phụ thuộc khi được giao

| File/phạm vi | Công việc dự kiến |
|---|---|
| `Assets/Scripts/InvestigationV36R107/InvestigationSession.cs` | Gameplay & Dữ liệu sửa tối thiểu điều kiện bước playback theo Page; giữ API hiện có và toàn bộ partial cùng chủ |
| `Assets/ConRoi/Modules/Gameplay/Editor/TrialPlaybackContextChecks.cs` + meta | **Đề xuất tạo mới khi được giao**, kiểm lifecycle/record riêng; hiện chưa có tool/type này |
| `Docs/QuanLy/HOP_DONG_MODULE.md` | Chỉ bổ sung contract playback đã thống nhất sau review; hiện chưa thay contract |
| `InvestigationReviewUI.cs`, `TrialTimeline.cs`, `ReturnGateMechanism.cs` | Đọc để hồi quy, không dự kiến đổi presentation/seek hoặc luật cơ cấu |

Phụ thuộc UI/UX & Tích hợp: chấp thuận quy tắc quay lại tiếp tục/tạm dừng và cung cấp fixture bàn dựng/binding stageCamera/modelGate hiện hành cho scene Gameplay thử. Nếu cần view/binding mới, gửi yêu cầu đúng chủ; không chép scene tích hợp đè scene thử. Map & Asset không phải sản xuất asset mới; tái dùng module cơ cấu đã có. Chưa thêm event/API; nếu kiểm phát hiện cần API chung, review contract trước. GDD và registry chung không sửa trong gói này.

## 6. Acceptance và cách kiểm sau khi được giao

1. **Tái hiện baseline trước sửa** bằng fixture/API có ghi rõ: bật một trial, rời trang, đi thời gian qua điểm kết thúc; ghi TrialTime/TrialRunning/Page, trạng thái GateState, TrialReport và số record. Sau đó dùng UI/controller tái hiện một ca trên scene thử nếu fixture trình bày đủ. Không lấy đọc code lần này làm FAIL đã chạy.
2. Với cả sáu ID `release/help/hold/hook/wedge/after`: đang phát → rời sang world/dossier/compare/inspect → qua thời gian lớn hơn phần còn lại. TrialTime, trạng thái mô hình, report và số record giữ nguyên ngoài trang. Không hoàn tất giả thuyết khi view tắt.
3. Quay lại: cùng ID/thời điểm/trạng thái; nếu trước đó đang phát thì tiếp tục tới settled và chỉ thêm đúng một record. Nếu đã tạm dừng tay thì không tự chạy. Không xóa record cũ khi chuyển trang nhiều lần.
4. Global pause/resume, P/R/Esc và di chuyển/modal vẫn đúng; đóng/đổi trang khi đang drag dọn lifecycle như trước. Đây là kiểm chịu ảnh hưởng, không mở lại các task UI/P-close còn riêng.
5. Seek tới giữa/cuối không tự thêm record hoặc mở quan sát. Đổi góc xem không đổi thời điểm. Clear holds sau khi quay lại vẫn hoạt động với cùng điều kiện và cùng kết quả trạng thái đã có.
6. Chạy visible playback cùng đầu vào so với baseline: thời gian mô phỏng theo timeline, cánh/chốt/dây/lực/móc/chèn và report không đổi; chỉ thời gian ngoài view được loại khỏi playback. Không san terrain hoặc đổi geometry để làm kiểm đạt.
7. Bằng chứng tách rõ: đọc source; fixture/API; UI/controller bằng input tổng hợp. Scene `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`, profile thử riêng nếu cần khởi tạo session; không chạy lại bộ save, không dùng save gốc. Không gọi đây là playtest người mới hoặc kiểm vật lý thực địa.

## 7. Quyết định bàn giao

Tiến độ Gameplay & Dữ liệu chưa giao nhiệm vụ gameplay mới tương ứng; đề xuất này **chờ review/giao triển khai**. Không sửa runtime, chạy Unity hoặc thêm tool thực thi trong lượt chuẩn bị. Khi được giao, ưu tiên baseline reproduction và gói sửa một điểm playback, bàn giao commit nguồn/kiểm/tài liệu cho UI/UX & Tích hợp; không triển khai thêm nội dung vụ án hoặc luật kết luận.
