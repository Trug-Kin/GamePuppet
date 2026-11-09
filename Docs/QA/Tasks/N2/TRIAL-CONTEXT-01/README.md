# TRIAL-CONTEXT-01 — Tạm ngưng diễn biến khi rời bàn dựng

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
