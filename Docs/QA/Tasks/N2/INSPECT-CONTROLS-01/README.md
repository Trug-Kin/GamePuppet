# INSPECT-CONTROLS-01 — Xoay, phóng và đặt lại vật đang xem

Gameplay & Dữ liệu. **RUNTIME ĐÃ KIỂM → CHỜ UI/TÍCH HỢP.** Chưa có action đặt lại trong UI sản phẩm; cần wiring, không gọi đây là chức năng đã dùng được trong scene chung.

## 1. Người chơi và căn cứ

[GDD9](../../../../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md): xem vật phải xoay, phóng, đặt lại và chặn đi/nhìn; GDD6.5: phóng to/mô tả hỗ trợ so mẫu, không kéo khớp pixel. Baseline b3c1bb06 có viewer600×600, mouse trực tiếp và preset góc, chưa có command đặt lại hay state/result cho adapter khác. Acceptance ghi trước sửa trong [tiến độ](../../../../QuanLy/DANG_LAM_NGUOI_2.md), mục “Hai gói viewer”. Không chấm chứng cứ hoặc thay hướng truyện.

Đã cung cấp command/state/event cho viewer một vật, dùng clamp/default cũ; mouse cũ đi cùng biên command và không đổi góc khi pause. UI chỉ cần chuyển action người dùng thành command, không tìm object bằng tên nút/Hierarchy.

## 2. Nguồn và API thực

Namespace `ConRoi.Investigation.V36.R107`; chủ toàn bộ partial vẫn Gameplay & Dữ liệu.

- Nguồn **b8cc108f1d96cede3cefa71b3d61db89eb5c02dd**: InvestigationInspectionControls.cs +meta, các hook runtime trong InvestigationSession.cs. Không sửa SessionUI.cs/scene/art/save/validator.
- Công cụ:4c2ef425 →0720006e →**61f26ad08af4e79c3c132da57b7d6c29e7d52dec**, commit thực chạy cuối. Nhận đủ nguồn/công cụ/báo cáo; không chỉ cherry-pick tài liệu.

| API | Tham số / kết quả |
|---|---|
| `InspectionView` | Snapshot `InspectionViewState`: EvidenceId, Pose, Availability, Preview và CanControl. Chỉ đọc, không cấp vật/quan sát |
| `TryAdjustInspection(float yawDegrees, float pitchDegrees, float distanceDelta, out string error)` | Delta góc theo độ, khoảng cách theo đơn vị viewer; dương distance=xa ra. true khi nhận, error rỗng; false có lý do tiếng Việt, không đổi pose/data |
| `TryResetInspection(out string error)` | Về default hiện hành của ID đang mở; true cả khi đã mặc định, không phát event thừa khi không đổi |
| `InspectionViewChanged` | `event Action<InspectionViewState>`: snapshot sau command có đổi pose và khi refresh ngữ cảnh/open/pause/close. Đọc snapshot ban đầu ngay khi subscribe |

`EvidenceViewPose` có Yaw/Pitch/Distance chỉ đọc. Yaw chuẩn hóa0–360; pitch clamp−80…80; distance0,5…4. NaN/Infinity bị từ chối, finite lớn không làm yaw NaN. Default yaw25; shoe/shoecontrol pitch65; slide/shoeprint/fabric/coat pitch−65; còn lại10. slide/shoeprint distance1,65, còn lại2. Không đổi ID/GUID cũ hoặc hệ tọa độ nguồn.

## 3. State, event, lỗi và dữ liệu mẫu

- `WrongContext`: không ở Page==inspect; ẩn/khóa controls, Preview=null.
- `NoEvidence`: trang inspect chưa có ID đã thu hợp lệ.
- `MissingVisual`: đã có vật nhưng chưa resolve được mô hình/camera. Preview=null, mô tả vẫn có thể dùng qua EvidenceTranscript, không hiển thị texture vật cũ.
- `Paused`: giữ ảnh/pose nhưng CanControl=false; command trả lý do tiếp tục trước.
- `Ready`: Preview do session sở hữu, CanControl=true. Adapter **không Destroy/Release/ghi** texture.

Lỗi command qua `out error`, không tự đổi Message Load/feedback cũ, không tự chuyển trang. Đăng ký/hủy trên cùng session ở OnEnable/OnDisable; callback chỉ cập nhật view, không gọi command đệ quy. Callback ném exception được cô lập/log Warning loại exception, không làm Load hoặc command trả thành công giả. Không phát event cho thất bại/command không đổi pose. Open có thể phát MissingVisual rồi Ready; context refresh có thể có snapshot lặp.

**Giới hạn tương thích:** preset legacy trong SessionUI vẫn ghi private pose trực tiếp, chưa phát event này; getter luôn đọc pose hiện tại. Khi thay renderer/preset, UI/UX & Tích hợp gửi action wiring cho chủ partial, không sửa song song. Mouse native đã gọi command và phát event khi đổi; không feed cùng delta lần nữa từ adapter.

Dữ liệu mẫu fixture: `shoe` đã Acquire, cube có InvestigationInteractable.evidenceId=shoe; `phone` đã Acquire nhưng không có model là MissingVisual. Đây là fixture, không phải tuyến nhận vật tự nhiên. Asset sản phẩm phải dùng inspectionPrefab/world source đúng ID như OpenEvidence hiện có; không phụ thuộc D:/Game hoặc asset giả.

## 4. Wiring từng bước — chưa thực hiện trong sản phẩm

1. UI/UX & Tích hợp nhận đúng nguồn và reference **InvestigationSession thật** từ cấu hình scene/adapter của mình, không Find theo tên UI. Giữ một chủ partial, không sửa SessionUI trực tiếp song song.
2. Trong lifecycle adapter đọc `session.InspectionView`, subscribe event; gỡ subscription khi adapter tắt/đổi session. Renderer vẽ Preview với tỷ lệ đúng, khóa controls khi !CanControl, cung cấp empty/model-missing bằng Availability.
3. Action đặt lại của thiết kế UI gọi TryResetInspection một lần. Input controller rời rạc có thể gọi TryAdjustInspection với delta hữu hạn. Giữ native mouse hiện có hoặc thống nhất thay driver với Gameplay, không feed cùng mouse delta hai lần; không tự ép view mỗi frame.
4. Hiển thị `error` khi command false; không tự Acquire/Observe hoặc sửa fields. Điều khiển của adapter phải xử lý pointer/focus theo thiết kế, không cho click nút thành di chuyển thế giới.
5. Việc đặt tên/đặt vị trí action chưa được làm. UI/UX & Tích hợp chốt renderer/action, nếu cần thay preset/consumer trong partial thì gửi patch/yêu cầu cho Gameplay & Dữ liệu nối; Map & Asset nhận vấn đề model/bounds/ánh sáng nguồn.

Ví dụ lệnh thực (biến session là reference do adapter nhận):

```csharp
var view = session.InspectionView;
bool ok = session.TryAdjustInspection(15f, 0f, -0.2f, out string error);
bool reset = session.TryResetInspection(out error);
// Chỉ hiển thị kết quả thực; không đổi evidence/progress thay command.
```

Ví dụ lifecycle adapter **chưa thêm vào sản phẩm**:

```csharp
void OnEnable() { session.InspectionViewChanged += Changed; Changed(session.InspectionView); }
void OnDisable() { if (session) session.InspectionViewChanged -= Changed; }
void Changed(InspectionViewState view) { /* cập nhật state của renderer thuộc UI */ }
```

## 5. Input/modal, pause và save/load

Chỉ command khi trang inspect, có evidence/model và không pause. Đi/nhìn vẫn khóa qua SetModalState cũ; ClosePage trả input như cũ. Pause giữ pose và khóa cả command/mouse branch. API không tự chiếm focus hoặc mở trang.

Pose/texture/event không thêm vào schema1 và không tự lưu. Save/Load evidence/progress như cũ; Load thành công/reopen tạo viewer và default pose hiện hành, xóa nguồn preview cũ trước khi tạo mới. Save/Load lỗi dùng guards hiện có; gói này không sửa writer hoặc IO-OPEN-01. Không gọi đây là lưu góc camera giữa các phiên.

## 6. Kiểm đã chạy và cách kiểm lại

Unity6000.6.2f1, Gameplay_ThuNghiem, profile thử riêng. **27/27** trên61f26ad0: API clamp/NaN/Infinity/extreme/reset/repeat/context/pause, state/data không grant, event/unsubscribe/isolate subscriber, source transform không đổi, model thiếu không dùng ảnh cũ, roundtrip và input branch bằng mouse ảo. Mouse được bơm bằng InputSystem.Update() rồi gọi polling Update trực tiếp qua fixture; **không phải UI/controller tự nhiên hoặc input Windows**. Một Warning do listener cố ý ném được giữ riêng.

Lịch sử không ghi đè: Inspection trên4c2ef425 đạt22/24 dòng rồi dừng do frame mouse không đổi góc; không kết luận lỗi runtime từ input chưa tới. InspectionFixture trên0720006e không vào Play do công cụ gọi overload Update không public (CS1501); đã sửa bằng entry public ở61f26ad0. [Bằng chứng assertion](verification.json). Không chạy lại bộ save/trial/IO cũ.

Chạy công cụ từ root clone, đúng đường dẫn Unity của máy, cả profile/output phải mới, đóng Editor cùng working directory trước:

```powershell
$project = (Get-Location).Path
$unity = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe'
& $unity -batchmode -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.EvidenceViewChecks.RunBatch --view-source (git rev-parse HEAD) --view-output "$project/Test/Output/InspectNew" --investigation-save-dir "$project/Test/PlaytestProfiles/InspectNew" -logFile "$project/Test/Output/InspectNew.log"
```

**UI/UX & Tích hợp kiểm lại sau wiring:** thu/mở vật bằng tuyến thật → xoay/phóng/reset lặp → pause/tiếp tục → hủy/R/P/Esc → mở vật khác/model thiếu → Save/Load hợp lệ và thao tác tiếp. Không grant chứng cứ sẵn rồi coi là chơi tự nhiên. Kiểm native mouse/preset/focus trên GameView thật, không suy từ polling fixture. Model/góc/texture-readability/1080p, scene chung và người mới **NOT_RUN** với gói này. IO-OPEN-01/P-close giữ OPEN độc lập. Không chốt baseline nhóm.
