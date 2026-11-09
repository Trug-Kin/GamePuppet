# SAVE-EVIDENCE-04 — kiểm nhận trong scene chung

UI/UX & Tích hợp, 09/11/2026. **Đã kiểm đạt ba acceptance trong scene chung; giới hạn riêng ở dưới.** Nhận đủ17b1f6e2→f73631ae→597369a5 sau91835c80, không lấy riêng báo cáo. Merge vào review/runtime-integration-20261109 tại C:/ConRoi_CheckoutTest: **22acc9611aa8c6d0f86741798b2108d68f9b85d0**. D:/ConRoi và thay đổi riêng, các nhánh task/chore/main giữ nguyên.

## Review và acceptance trước Play

- Review cả nguồn/công cụ/meta/contract/báo cáo. Guard JSON chạy trước JsonUtility và trước data=incoming; typed validator tiếp tục ID/index/finite. evidence bắt buộc là array, [] hợp lệ; null/missing/sai kiểu/null-record bị chặn. Collection phụ missing giữ default cũ, explicit null và null-record cùng nguyên nhân bị chặn. Không đổi schema/ID/GUID/API công khai hoặc wiring. Newtonsoft đã có trong package lock; không nâng package.
- Công cụ API chỉ kích hoạt khi gọi RunBatch; không gắn game. [Đối chiếu nguồn](source-handoff-comparison.json): giữ SHA raw của bên giao; checkout CRLF khác working-tree LF nên preflight raw dừng trước Unity, sau đó so bytes normalized với cả source đã kiểm và Git blob597369a5 đều khớp. Không thay manifest hash baseline để nhận sai thay đổi.78/78 API,54/54 và sandbox UI là bằng chứng bên giao, không chạy lại hoặc suy PASS scene chung. Probe bổ sung W/chuột NOT_RUN, Load feedback/P-close OPEN riêng.
- Unity6000.6.2f1, scene thật Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity, profile mới Test/PlaytestProfiles/SaveEvidence04Integration20261109. Tool TichHop hiện có chỉ đọc/audit/snapshot và lifecycle, không cấp evidence, dựng proxy hoặc API Save/Load/page/teleport.
- Fixture bắt đầu4 evidence là **bản sao save tạo bằng UI/controller trong RUNTIME-INTEGRATION-01**: slide/phone/clip/mechanism, một TrialRecord và cờ tiến trình đã ghi/xem. Giữ nguyên file nguồn/hash. Load bản sao bằng F6 vào profile mới; không nhận là tuyến chơi tự nhiên mới hoặc kiểm máy thành viên.
- Tạo New Game[] bằng UI/F5 trong profile mới, giữ bytes riêng. Với4 vật chứng, F5 → sửa bản sao evidence:null → F6 phải từ chối, giữ full data/identity/mục tiêu/vật chứng/quan sát/trial record và state Load liên quan, bytes/SHA mẫu giữ nguyên.
- Sau lỗi, P/E/Esc/di chuyển hoặc tương tác đúng ngữ cảnh → F6 save hợp lệ → tiếp tục/F5/đổi tiến trình vị trí/F6 khôi phục được. F6 save New Game[] vẫn hợp lệ; trở lại save có4 vật được.
- Ghi đúng commit22acc961 đã chạy; commit docs sau kiểm không phải chạy lại. Kết quả scene chung, render/UI1080p, người mới và probe cũ tách riêng. Nếu focus/PID hoặc state ngoài script khác dự kiến: snapshot, đánh dấu gián đoạn; không kết luận FAIL gameplay/PASS chưa chạy. Nếu lỗi runtime: bước tái hiện gửi Gameplay & Dữ liệu, không sửa partial.
- Chỉ đề xuất baseline ứng viên sau đạt; không tự cập nhật nhánh task/chore/main, push, build hoặc ZIP.

Snapshot trước merge/state/diff/settings và fixture-source hash: Test/Output/SaveEvidence04Integration20261109/Before, ngoài Git. Library độc lập hiện có tại C dùng import tăng dần; C còn13.659.910.144byte trước import, không tạo Library mới.


## Kết quả thực chạy — 09/11/2026

**Nguồn tích hợp đã chạy: `22acc9611aa8c6d0f86741798b2108d68f9b85d0`**, nhánh `review/runtime-integration-20261109`, Unity **6000.6.2f1**, scene `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`. Commit báo cáo tạo sau phiên này chỉ ghi nhận bằng chứng, không được gọi là một lần chạy mới. Đã nhận toàn bộ chuỗi `17b1f6e2 → f73631ae → 597369a5` bằng merge giữ lịch sử; không cần wiring mới.

| Ca trong scene chung | Kết quả và bằng chứng |
| --- | --- |
| Import/compile/reference | PASS trong phạm vi scene mở: 32.958 component, 0 Missing Script, không reference serialized hỏng; đủ các binding của session. Log không có error CS, NullReferenceException hoặc MissingReferenceException. Có cảnh báo môi trường Burst/WDAC, licensing/cloud; không coi mọi warning đã được xử lý. |
| Bốn vật chứng → F6 `evidence:null` | PASS: vẫn slide/phone/clip/mechanism, cùng identity dữ liệu, toàn bộ data/quan sát/cờ tiến trình/mục tiêu và một TrialRecord. Các trường runtime Load được chụp cũng không đổi: page/paused/modal/position/yaw/pitch/movement/look/gate/trialId/time/running/report/modelGate/clipTime. Chỉ message đổi theo lỗi. |
| Không tự sửa/ghi file lỗi | PASS: bytes và SHA256 trước/sau F6, cả sau thao tác tiếp, không đổi: `2edf8b0ab41eebee4fbfe945bd3ff86608b34051e98efe6a566e2f6ee157d069`. Trả kết quả hiện có: “Không tải: Danh sách evidence trong hồ sơ không hợp lệ; không nhận null hoặc mục null.” |
| Thao tác tiếp sau từ chối | PASS: P mở/đóng hồ sơ; aim chuột và E mở bàn dựng thật; R về world. Esc pause khóa movement/look, W không đổi vị trí; Esc resume và A di chuyển được. Không phát hiện kẹt input trong lượt này. |
| Tải hợp lệ → tiếp tục → F5/F6 | PASS: khôi phục bản sao hợp lệ bốn vật chứng bằng F6; di chuyển bằng controller, F5, thay vị trí rồi F6 khôi phục data đầy đủ và vị trí đã lưu (sai số <0,002). Không cấp evidence/API Load/Save để thay các bước này. |
| `evidence:[]` hợp lệ | PASS: save New Game tạo bằng UI/F5 của profile mới được F6 nhận, evidence rỗng và firstFloorRecorded=false. Sau đó F6 lại save bốn vật chứng, phục hồi toàn bộ data đã lưu. |

[Kết quả có cấu trúc](integration-results.json), [audit scene](reference-audit.json), [bảo toàn](preservation.json), [index hash bằng chứng local](evidence-index.json). Raw save/profile/capture/log và script desktop nằm ngoài Git trong `C:/ConRoi_CheckoutTest/Test/Output/SaveEvidence04Integration20261109` và `Test/PlaytestProfiles/SaveEvidence04Integration20261109`; thành viên khác không phải tạo đường dẫn C này để chạy game. Index giúp yêu cầu đúng bằng chứng từ UI/UX & Tích hợp, không phải dependency runtime.

### Phương pháp, lượt gián đoạn và giới hạn

- Toàn bộ thao tác phím/chuột là **input Windows tổng hợp** trên scene tích hợp thật. Bộ đọc state/audit hiện có chỉ quan sát; không dựng proxy, teleport hoặc cấp chứng cứ. Bốn vật chứng ban đầu là **fixture bản sao save tạo bằng UI/controller trong phiên RUNTIME-INTEGRATION-01**, được F6 vào profile mới. Đây là kiểm Load có dữ liệu chuẩn bị, **không phải** tuyến New Game tự nhiên mới hoặc playtest người mới. Nguồn fixture giữ nguyên SHA256 `7227ad5519a3097239b847e51d61eafc7d8165ca21b960251de5e5d6f5ff1424`.
- Một lượt sau R/snapshot bị mất foreground **trước khi gửi Esc**, helper dừng input. Lượt đó **INTERRUPTED**, không FAIL gameplay; lượt kiểm lại riêng Esc/W/resume/di chuyển đạt. Giữ input log và snapshot, không xóa lịch sử gián đoạn.
- Preflight raw hash lần đầu dừng vì LF/CRLF; so normalized với nguồn đã kiểm và blob Git khớp. Giữ manifest hash gốc và đối chiếu riêng, không sửa baseline hash để biến khác biệt thành PASS.
- **78/78 API và 54/54 hồi quy** là bằng chứng Gameplay & Dữ liệu ở gói nguồn; không chạy lại, không cộng vào các kết quả scene chung ở bảng này. Không lặp bộ IO/trial hoặc toàn tuyến điều tra trong lần nhận bản sửa collection.
- **Probe bổ sung W/chuột của bên giao: NOT_RUN** như báo cáo nguồn; ca di chuyển có phạm vi của lượt integration này không đóng hồi tố probe đó.
- **UI1080p đầy đủ trên bản sửa: NOT_RUN**. Render đo 1920×1080 và có capture các thao tác mục tiêu; không suy thành kiểm toàn bộ trang/layout/navigation. **Người mới: NOT_RUN.**
- **Load feedback: OPEN**: message hiện có trả lý do, nhưng yêu cầu trình bày lý do trong hồ sơ/pause chưa được xử lý. **P-close: OPEN** riêng: P hoạt động trong lượt này không đủ đóng bất thường lịch sử hoặc mọi ngữ cảnh P. Không sửa UI/session partial song song.

### Bảo toàn và đề xuất nhận baseline

- Đã dừng Play, scene không dirty, đóng đúng Editor không lưu scene. Năm file Unity có snapshot Before/After và đối chiếu normalized; giữ/phục hồi đúng bytes trước phiên. Product diff Assets/Packages/ProjectSettings/Source so với commit chạy rỗng; các dấu M xuống dòng có từ trước vẫn được giữ.
- D:/ConRoi giữ nguyên HEAD/branch/diff và toàn bộ danh sách untracked theo snapshot (kể cả deletion tài liệu riêng). Nhánh Gameplay giữ `597369a5`; legacy, save gốc và archive không thay đổi. Không push/main/chore/cập nhật nhánh thành viên.
- **INT-SAVE-NULL-LIST-01 được xác nhận sửa trong scene chung** trong phạm vi nêu trên. Có thể **đề xuất `22acc961` làm nguồn ứng viên baseline tích hợp** để nhóm review/chốt, kèm commit báo cáo sau đó. Chưa tự chốt baseline nhóm; các OPEN/NOT_RUN còn nguyên. Không phát hiện lỗi runtime mới cần gửi Gameplay & Dữ liệu trong lượt này.
