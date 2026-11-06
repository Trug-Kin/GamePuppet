## 06/11/2026 — hiện hành r107: sửa nền và mở đầu từ phản hồi thực

Mở **[JourneyPresentation_v48_r107.unity](Assets/Scenes/JourneyPresentation_v48_r107.unity)** bằng Unity **6000.6.2f1**. Giữ r106 làm checkpoint. [Bàn giao và giới hạn](Test/KetQua/investigation_r107/README.md) · [Người chơi](Test/KetQua/investigation_r107/HUONG_DAN_NGUOI_CHOI.md) · [Người tổ chức/profile sạch](Test/KetQua/investigation_r107/PLAYTEST_NGUOI_MOI.md).

Người dùng đã báo nền hở và UI quá nhiều công cụ. R107 sửa winding trong bốn mesh riêng từ Blender→FBX→Unity, không đổi cao độ/nước/nền chợ. Mở đầu chỉ một việc: vào xưởng, lên gác, kiểm dấu sàn; E khám, xoay/phóng, ghi, R trở lại, P xem lại. Công cụ hiện theo ngữ cảnh; save riêng `InvestigationV36/r107`, không migration save gốc. Đã có video input tổng hợp và đường controller qua vùng sửa. Chưa coi toàn map sạch topology hoặc người mới đã hiểu: phiên r107 **NOT_RUN**, báo cáo ghi các kiểm còn thiếu. Không chạy lại author/derive/bind, không build/ZIP/runner. Các mục dưới là lịch sử.

## 06/11/2026 — bàn giao r106 sẵn sàng, người thật NOT_RUN

[Giao máy](Test/KetQua/investigation_r106/playtest_handoff/README.md) · [Người tổ chức](Test/KetQua/investigation_r106/NGUOI_TO_CHUC.md) · [Chỉ giao cho người chơi](Test/KetQua/investigation_r106/HUONG_DAN_NGUOI_CHOI.md). Slot PT001 chưa gán cho người thật; đã mở Editor 6000.6.2f1 với profile mới và kiểm F5/F6 hồ sơ trống đúng thư mục. Sản phẩm r106 giữ nguyên, không chạy lại cơ cấu/tuyến hoặc polish. Chờ phiên người thật; ghi nhận trung thực theo mẫu trước khi đề nghị sửa.

## 06/11/2026 — hiện hành r106: chu kỳ cơ cấu đã kiểm, chờ người mới

Mở **[JourneyPresentation_v48_r106.unity](Assets/Scenes/JourneyPresentation_v48_r106.unity)** bằng Unity **6000.6.2f1**. Giữ map/art/checkpoint r101–r105. R106 sửa timeline dừng sớm 10 giây, giữ luật hồi; sáu ca chạy đủ tới đóng/cài hoặc trạng thái giữ có lý do. Có nút ngừng đè/nhả móc/rút chèn để xem phần hồi tiếp theo.

[Bàn giao](Test/KetQua/investigation_r106/README.md) · [Bảng sáu ca](Test/KetQua/investigation_r106/SIX_CASES.md) · [Video](Test/KetQua/investigation_r106/VIDEO_INDEX.md) · [Chuẩn bị profile và phiếu người mới](Test/KetQua/investigation_r106/PLAYTEST_NGUOI_MOI.md).

Khởi động phiên người thử bằng Editor đã đóng rồi mở lại với `--investigation-save-dir` tới thư mục mới chưa dùng; xem lệnh chính xác trong phiếu. Đã kiểm tham số khởi động thật và F5/F6 vào thư mục thử, không tự load. **Người thật NOT_RUN**. Bước kế tiếp là lấy phản hồi; không tiếp tục polish chung khi chưa có lỗi cụ thể. Các mục dưới là lịch sử.

## 05/11/2026 — hiện hành: r105, chuẩn bị playtest điều tra

Mở **[JourneyPresentation_v48_r105.unity](Assets/Scenes/JourneyPresentation_v48_r105.unity)** bằng Unity **6000.6.2f1**, Play từ trước xưởng. WASD/chuột, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 hồ sơ riêng. Mục tiêu: khám xưởng → hỏi Hà → trở lại bàn dựng. Scene lưu hồ sơ trống, không tự load. Namespace `conroi.investigation.v36.r105`, không dùng save gameplay cũ.

[Bàn giao r105](Test/KetQua/investigation_r105/README.md) · [Video](Test/KetQua/investigation_r105/VIDEO_INDEX.md) · [Checklist](Test/KetQua/investigation_r105/CHECKLIST.md) · [Playtest người mới — NOT_RUN](Test/KetQua/investigation_r105/PLAYTEST_NGUOI_MOI.md).

Sửa tay/dụng cụ/dây sửa Tư, hiện dây treo R-12, làm rõ vai giả thuyết và prompt bàn dựng. Giữ Quaternius, map, luật hồi và checkpoint r101–r104. Tư/diễn xuất còn minh họa; chưa có người mới chơi. Các mốc bên dưới là lịch sử.

# Bắt đầu tại đây — Con Rối

## 05/11/2026 — prototype hiện hành r104

Mở **[JourneyPresentation_v48_r104.unity](Assets/Scenes/JourneyPresentation_v48_r104.unity)** bằng Unity **6000.6.2f1**, Play từ trước xưởng. WASD/chuột, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 hồ sơ riêng. Đi xưởng → quầy Hà → quay lại bàn dựng. GDD đúng: Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md.

Khải/Hà/rối Quaternius đã nhập thật; clip/bàn dựng có khung lớn, góc cận và slider kéo được. Clip/diễn xuất vẫn bản dựng thử im tiếng; người mới **NOT_RUN**. Save namespace conroi.investigation.v36.r104 riêng, không đọc save cũ. Giữ map r101 và checkpoint r101–r103; không build/ZIP/runner mới.

[Bàn giao r104](Test/KetQua/investigation_r104/README.md) · [Nguồn/giấy phép](Test/KetQua/investigation_r104/ASSET_SOURCES.md) · [Video](Test/KetQua/investigation_r104/VIDEO_INDEX.md) · [Hướng dẫn/phiếu playtest](Test/KetQua/investigation_r104/PLAYTEST_NGUOI_MOI.md). Các mục phía dưới là lịch sử, không dùng save/menu gameplay cũ để bắt đầu prototype mới.


## 05/11/2026 — prototype điều tra hiện hành r103

**Mở [JourneyPresentation_v48_r103.unity](Assets/Scenes/JourneyPresentation_v48_r103.unity) bằng Unity6000.6.2f1, nhấn Play.** Root `Investigation_V36_R103`, player `Khang_FirstPerson_R103`, bắt đầu trước xưởng. Đi khám xưởng → quầy Hà ngoài cửa → quay lại bàn dựng. WASD/chuột, Space, E, R trả/đóng, P hồ sơ, Esc pause, F5/F6 save riêng. Xin mẫu Khải qua mục liên hệ Phương sau khi gặp Khải. Đoạn quay R12 có ngay khi khám điện thoại.

Save riêng namespace `conroi.investigation.v36.r103`, schema1, `InvestigationV36/r103` dưới persistentDataPath; không nạp/migration save cũ. **R101 là map đóng băng; r102 là checkpoint prototype**, không ghi đè. Không build/ZIP/runner mới. GDD đúng: `Docs/CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md`.

[Bàn giao r103 và giới hạn](Test/KetQua/investigation_r103/README.md) · [Nguồn/giấy phép](Test/KetQua/investigation_r103/ASSET_SOURCES.md) · [Playtest người mới — NOT_RUN](Test/KetQua/investigation_r103/PLAYTEST_NGUOI_MOI.md). Vật chứng có geometry để so; clip12s và art nhân vật vẫn bản dựng thử, chưa final. Không suy giờ kéo/ý định từ vải/giày. Hướng dẫn bên dưới là **lịch sử map/gameplay cũ**, không áp dụng profile/menu đó cho r103. Lịch sử r102 giữ trong hai tài liệu chung.

**Project: `D:\ConRoi`. Scene phát triển map hiện hành: [JourneyPresentation_v48_r101.unity](Assets/Scenes/JourneyPresentation_v48_r101.unity), dẫn xuất từ r100: khép terrain X643.5/Z414 và đầu nối r97 ngày 04/11/2026. Giữ checkpoint r94–r100.** Build Settings vẫn bật r92 lịch sử; chưa có build/ZIP r101. Không chọn baseline chỉ theo số lớn nhất; xem mục r101 trong hai tài liệu chung. Các khu gameplay cũ vẫn trong cùng scene, GDDv3.6 xưởng→quầy Hà→xưởng là hướng prototype tiếp theo, chưa triển khai.

1. Mở Unity Hub → Add project from disk → `D:\ConRoi`, dùng **Unity 6000.6.2f1 (770e33f6875c)**. Máy này có Editor tại `D:\data_unity\6000.6.2f1\Editor\Unity.exe`. Mở scene r101 ở trên để tiếp tục map; chờ import/compile xong. Workspace code: [ConRoi.code-workspace](ConRoi.code-workspace).
2. Tìm các root: **`Lake_Market_Story_GDD21`** (điều phối, UI, save/chuyển khu), **`Minh_RegionalWalk_180cm`** → `Main Camera`/`GroundCheck` (Player), **`StoryProps`** → `CR_Hub`, `CR_Lake`, `CR_Market`; **`School_V37_Classroom_Courtyard`**, **`DaLat_Slope_V39`**. Map dùng thêm `LakeGateMarketV32`, `RouteSurfaces`, `R20_Whole_Source_Export`; ánh sáng tại `DaLat_Daylight`. UI phần lớn chỉ xuất hiện khi Play.
3. Play mở menu: Enter/**New Game** bắt đầu ở phòng; L/**Load Game** là Continue nếu profile có save hợp lệ. H mở trợ giúp; WASD/chuột, Space nhảy, E tương tác, P điện thoại, Esc pause, F5/F6 lưu/tải. Xem mô tả đầy đủ trong menu H và [MenuVaoGameV47.cs](Assets/Scripts/GiaoDien/MenuVaoGameV47.cs). New Game ghi lại profile hiện tại: **không dùng profile người dùng để thử**.
4. Khi kiểm riêng, đóng Editor rồi mở với `--r30-review-output D:\ConRoi\Test\KetQua\<phien-moi>\save-rieng` và `--verification-output D:\ConRoi\Test\KetQua\<phien-moi>\telemetry` (tạo thư mục telemetry trước). Đối số đầu đã kiểm thực tế với r92; đối số thứ hai được xác nhận từ code V40/V41. Một số telemetry V42/V48 vẫn mặc định ghi LocalLow, xem giới hạn trong tài liệu chung. `Tools/KiemUnity.ps1 -Mode Open` chưa tự truyền profile save riêng. Mở chỉ để đọc scene thì không cần Play.
5. Nguồn Blender phát triển: [Source/Blender/JourneyV48](Source/Blender/JourneyV48/) và các editable còn dùng trong `Docs/Reviews`; tra đúng model theo workflow, không chọn bản REVIEW thay nguồn gốc. Bộ báo cáo mở [ConRoi_TongHop_TrinhBay.blend](TrinhBay/Blender/ConRoi_TongHop_TrinhBay.blend), xem [HUONG_DAN](TrinhBay/Blender/HUONG_DAN.md). Bộ này là bản sao trình bày, không tự thay nguồn Unity.
6. Dữ liệu kiểm theo đợt: [Test/BanChoiThu](Test/BanChoiThu/) ZIP/player; [Test/BangChung](Test/BangChung/) ảnh/video; [Test/KetQua](Test/KetQua/) log/telemetry/report; [Test/Tam](Test/Tam/) dữ liệu tái tạo. [Test/README](Test/README.md) chỉ cách tra đường cũ. ZIP hiện hành: `Test/BanChoiThu/journey_v48/restructure-20261102/ConRoi-r92-Reorganized-20261102-Windows-playtest.zip`.

[Hướng dẫn và lịch sử — scene/Hierarchy/kết quả kiểm](Docs/PROJECT_GUIDE_AND_HISTORY.md) · [Workflow Blender–Unity — nguồn model/reference](Docs/BLENDER_UNITY_WORKFLOW.md) · [Danh mục đủ 117 scene](Docs/Inventory/20261102/SCENES.md).

Đợt này đã đọc trực tiếp Hierarchy/serialized Inspector trong Editor, kiểm 0 Missing Script/0 reference hỏng và Play khởi tạo + handler điện thoại trên save riêng. Chưa kiểm lại toàn tuyến, mọi scene lịch sử, phím E/raycast hay toàn bộ hình ảnh; không tạo build mới.
