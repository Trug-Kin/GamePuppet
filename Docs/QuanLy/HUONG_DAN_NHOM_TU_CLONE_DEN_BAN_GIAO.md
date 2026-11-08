# Con Rối — hướng dẫn nhóm từ clone đến bàn giao

Ngày đối chiếu: **08/11/2026**. Đây là hướng dẫn chính cho thành viên mới; các báo cáo QA và tài liệu phân công được dẫn tới vẫn là nguồn chi tiết. Các lệnh dưới đây dành cho **PowerShell trên máy của từng người**, chạy tại root checkout của mình trừ khi có chỉ dẫn khác. Không cần tạo đường dẫn giống máy tác giả.

## Mục lục

1. [Game và trạng thái bàn giao](#trang-thai)
2. [Chuẩn bị máy và tài khoản](#chuan-bi)
3. [Clone đúng nhánh](#clone)
4. [Mở Unity và chạy profile riêng](#unity)
5. [Quyền sở hữu và điểm bắt đầu trong source](#phan-cong)
6. [Gói việc đầu tiên của từng người](#viec-dau)
7. [Các giai đoạn phát triển tiếp theo](#giai-doan)
8. [Quy trình hằng ngày](#hang-ngay)
9. [Phối hợp và tích hợp](#tich-hop)
10. [Mẫu tiến độ, yêu cầu và bàn giao](#mau)
11. [Khắc phục lỗi thường gặp](#loi)
12. [Dữ liệu ngoài Git](#ngoai-git)
13. [Checklist nhập nhóm và thông tin cần điền](#checklist)

<a id="trang-thai"></a>
## 1. Game và trạng thái bàn giao

### 1.1. Hướng game đang có hiệu lực

Đọc [GDD v3.6 chốt triển khai](../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md). **Con Rối là game PC một người chơi, góc nhìn thứ nhất, điều tra và giải đố**, bối cảnh Đà Lạt đời thường. Khang điều tra cái chết của ông Tư tại xưởng rối và mối liên hệ với những mật thư bằng tư thế rối, việc chuyển bán rối cổ cùng quá khứ của gánh rối.

Vòng chơi: khám hiện trường → ghi đặc điểm nhìn thấy → hỏi nhân vật/đọc tài liệu → giải mật mã tư thế → đối chiếu vật chứng → thử giả thuyết trên bàn dựng → ghi kết luận và căn cứ. Phải phân biệt người gửi thư, người có mặt/dàn dựng và người thao tác cơ cấu. Tương tự màu vải, dấu giày hoặc cơ hội có mặt không tự chứng minh thời điểm hay ý định. Người chơi được chọn **“Chưa đủ chứng minh”**.

Cơ cấu cánh lan can dùng chốt, dây nhả và hồi hữu hạn; lực người, móc hoặc vật chèn có thể duy trì trạng thái khác nhau. Sáu diễn biến dùng chung luật xác định. Clip sự kiện cho phép quan sát trạng thái, không tự nhận diện người kéo. Clip và hình nộm bàn dựng là **animation minh họa**, cần đối chiếu cả hình và trạng thái. Không thêm siêu nhiên, giấc mơ, rối có ý thức hoặc hệ bắt chước của hướng cũ. GDD v2.x/v3.0–3.5 và `GDD_IMPLEMENTATION_CURRENT.md` lịch sử không thay thế v3.6.

### 1.2. Mốc đã kiểm và giới hạn

| Nội dung | Trạng thái có căn cứ |
|---|---|
| Baseline nhóm ban đầu | `0d878e3b7163c4d50e3976cd5c46118403b19240` |
| Root trên máy tác giả | `D:/ConRoi`; máy thành viên dùng root riêng |
| Unity / entry | **6000.6.2f1** / `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity` |
| Phần prototype | **Xưởng → Hà → xưởng**, có Khải và mẫu đối chiếu; mở đầu, khám/ghi/P, điện thoại/clip, mật mã, đối chiếu, sáu diễn biến, nhận định và lưu/tải |
| Nguồn Unity đã kiểm độc lập | `074caf3dd3da4107b4fc9ac1c089b20216b4705b`; product tree tới baseline 0d878e3b không đổi, các commit sau là bàn giao/audit |
| Checkout / NAV-01 | Compile/reference đạt, 0 Missing Script và 0 reference hỏng trong entry được audit; xem [NAV-01](NavFix20261108/README.md), [checkout trước sửa](CheckoutFinal20261107/README.md) và giới hạn ở đó |
| Loại bằng chứng | Input chuột/phím tổng hợp; NAV-01 có seed API thư/vật/quan sát trong fixture riêng. Không coi fixture là người chơi tự tìm được chứng cứ |
| Playtest người mới r107 | **NOT_RUN** theo báo cáo hiện có. Phản hồi người dùng r106 đã được nhận; không tự nâng thành phiên người mới r107 đạt |
| Map | Winding vùng hở ảnh 105627 đã sửa có phạm vi; ba góc ảnh chưa khớp chính xác pose, topology kế thừa và 8 tam giác cực mảnh còn mở; không tuyên bố toàn map PASS |
| Art | Khải/Hà/rối dùng bộ Quaternius r104 đã tích hợp; Tư/hình nộm/diễn xuất vẫn có giới hạn prototype. Không polish chung khi chưa có lỗi cụ thể |
| Legacy | **4.183 file chưa xác minh** giữ tại máy tác giả; không tự dọn hoặc copy bù vào checkout |

Đã push bốn nhánh bàn giao lên [Trug-Kin/GamePuppet](https://github.com/Trug-Kin/GamePuppet). Đọc refs remote ngày 08/11 xác nhận cả bốn ở 0d878e3b; `main` vẫn ở `6261710cfcf41184eca0630527f79281f5621998`, **không chứa baseline nhóm**. Đây là ảnh chụp trạng thái lúc viết, không yêu cầu các nhánh luôn bằng nhau sau khi có tiến độ mới.

Đợt push trước đã tải lại độc lập **797 object LFS / 6.143.855.590 byte**, SHA-256 toàn bộ khớp. Biên nhận tại máy tác giả: `C:/ConRoi_LFSRemoteVerify_20261108/verification.json`; file này không nằm trong clone. Thành viên có thể yêu cầu người tổ chức cung cấp biên nhận. Đợt viết hướng dẫn này **không chạy lại Unity, clone hoặc tải lại LFS**, chưa xác nhận trên cả ba máy.

### 1.3. Cách đọc tài liệu cũ có trạng thái khác

Nguồn vận hành: [BAT_DAU_TAI_DAY](../../BAT_DAU_TAI_DAY.md), [HANDOFF](HANDOFF.md), [ownership](OWNERSHIP.md), [tích hợp](TICH_HOP.md), [contracts](HOP_DONG_MODULE.md), [registry vật chứng](EVIDENCE_REGISTRY.md), [workflow Blender/Unity](../BLENDER_UNITY_WORKFLOW.md).

Các dòng “chưa remote/push”, task ở a209c0e/fcbb7e19 và BLOCKED dung lượng/UI trong ghi chép cũ là **lịch sử**. Sau đó có sửa NAV-01, fast-forward ba nhánh tới 0d878e3b và push thành công. Giữ nguyên FAIL cũ và đọc mốc mới thay thế nó; không sửa FAIL thành PASS.

Ownership cũ phân Editor/test cho hai người; **bảng “File giao nhau” đầu OWNERSHIP là quy tắc hiện hành**: người 2 giữ source runtime, Editor/test và Tools, chỉ chuyển từng file cho người 1 sau khi ghi nhận. Registry ghi đồng tác giả nội dung/hình không có nghĩa hai người cùng sửa file. Khi còn mâu thuẫn chưa giải được, dừng task tại file đó và nhờ người 3 điều phối một chủ trước khi sửa.

<a id="chuan-bi"></a>
## 2. Chuẩn bị máy và tài khoản

1. Cài **Git for Windows**, **Git LFS**, **Unity Hub**, Editor **6000.6.2f1**, và trình sửa C# hỗ trợ Unity (Visual Studio, Rider hoặc VS Code đã cấu hình C#). Không cần module build nền tảng chỉ để chạy Play trong Editor.
2. Kiểm trong PowerShell mới:

   ```powershell
   git --version
   git lfs version
   ```

   Kết quả mong đợi: cả hai có số phiên bản, không báo command not found. Khởi động lại terminal sau khi cài để nhận PATH.
3. Người 1 cần **Blender 5.2.0 LTS** cho nguồn terrain r107 đã được xác minh trong [audit nguồn](../QA/investigation_r107/source-final-audit.json). Người 2/3 chỉ cần Unity nếu không chỉnh nguồn 3D. Các nguồn art/animation có recipe riêng; kiểm phiên bản/recipe trước khi mở và không ghi đè file bằng bản Blender cũ. Không giả định một lần kiểm terrain đã chứng nhận mọi `.blend` trong lịch sử.
4. Mỗi người dùng một checkout local riêng, có Library riêng. Không mở cùng working directory trên hai máy, không chia sẻ Library, không clone vào thư mục project có sẵn. Trên máy tác giả Library D là junction tới cache cũ; **không sao chép junction đó** sang máy thành viên.
5. Dùng tài khoản GitHub của **chính thành viên đó**. Người chủ repo cấp quyền push/collaborator cho từng tài khoản và thành viên chấp nhận lời mời. Repo đọc được không đồng nghĩa được push. Đăng nhập HTTPS bằng Git Credential Manager/trình duyệt; không gửi token/mật khẩu qua chat, không dùng chung tài khoản `Trug-Kin`. Đợt upload do chủ repo thực hiện không phải quy định dùng tài khoản chủ cho cả nhóm. `git user.name/email` là tác giả commit, không chứng minh danh tính đăng nhập; giữ cấu hình global riêng của mỗi người, không viết lại tác giả cũ.

### Dung lượng phải chuẩn bị

Đo bằng tập file tracked đã materialize tại baseline ngày 08/11, không đi xuyên cache/junction:

| Thành phần trên một máy | Số đo / dự trù |
|---|---|
| Working tree tracked: Assets, Source, docs, packages/settings… | **9.811.783.050 byte**, 24.976 file, khoảng 9,81 GB; đã bao gồm bản asset LFS trong working tree |
| Kho `.git/lfs/objects` riêng | **6.143.855.590 byte**, khoảng 6,14 GB; là bản lưu object thêm ngoài working tree |
| Git objects thường | `git count-objects -v` tại máy tác giả: size-pack 480.401 KiB + loose size 519 KiB, khoảng 0,49 GB; pack máy clone có thể khác |
| Library độc lập đã đo trong checkout C | **6.273.778.145 byte**, khoảng 6,27 GB tại lần ghi nhận; không phải trần tăng trưởng |
| Dự trù Library/import | Khoảng **12,38 GB** theo kiểm kê cache/ước tính bảo thủ trước đó; có thể tăng khi mở thêm nội dung |
| Dự phòng thao tác/import | Ít nhất khoảng **4,3 GB** ngoài các phần trên |

Tổng theo Library đã đo khoảng **27 GB** có dự phòng; theo dự trù Library khoảng **33,1 GB**. Nên có **ít nhất 36 GB trống** trên vị trí checkout trước clone/import, chưa tính bộ cài Unity, Blender, IDE hoặc dữ liệu cá nhân. 6,14 GB LFS **không phải toàn project**. Nếu máy không đủ, chọn một ổ local phù hợp trước khi tải; không xóa dữ liệu để lấy chỗ. Di chuyển từ D sang thư mục khác trên D không tăng dung lượng trống ổ D.

<a id="clone"></a>
## 3. Clone đúng nhánh cho từng người

### 3.1. Chọn đúng một lệnh theo vai

Mở terminal tại thư mục cha muốn chứa project. Tên đích dưới đây phải **chưa tồn tại**. Nếu đã tồn tại, kiểm đó là checkout nào; không clone đè.

**Người 1:**

```powershell
git clone --branch work/map-asset-baseline https://github.com/Trug-Kin/GamePuppet.git ConRoi_Nguoi1
cd ConRoi_Nguoi1
```

**Người 2:**

```powershell
git clone --branch work/runtime-contracts-baseline https://github.com/Trug-Kin/GamePuppet.git ConRoi_Nguoi2
cd ConRoi_Nguoi2
```

**Người 3:**

```powershell
git clone --branch work/ui-onboarding-review https://github.com/Trug-Kin/GamePuppet.git ConRoi_Nguoi3
cd ConRoi_Nguoi3
```

Không dùng clone mặc định rồi coi nội dung `main` là project đầy đủ. Không dùng `--single-branch` trong hướng dẫn này để còn đọc các nhánh phối hợp.

### 3.2. Kiểm nhánh và baseline

Chạy trong checkout vừa clone:

```powershell
git lfs install --local
git lfs pull
git status --short
git branch --show-current
git rev-parse HEAD
git merge-base --is-ancestor 0d878e3b7163c4d50e3976cd5c46118403b19240 HEAD
$LASTEXITCODE
```

Kết quả mong đợi: nhánh đúng vai; working tree mới sạch; lệnh ancestry trả **0**. Ở mốc bàn giao ban đầu HEAD là 0d878e3b; khi nhóm đã làm tiếp, HEAD có thể mới hơn. **Không reset về baseline** chỉ để khớp con số. Nếu ancestry khác 0, báo SHA/nhánh đang dùng cho người 3 trước khi mở Unity hoặc sửa file.

Nếu clone đã tự tải LFS qua smudge, `git lfs pull` chỉ bổ sung phần còn thiếu. Lần tải đầu có thể lâu và cần đủ băng thông/quota. Không tắt LFS rồi tiếp tục coi asset pointer là FBX/ảnh thật.

### 3.3. Kiểm LFS đã materialize

```powershell
git lfs ls-files
git lfs fsck
```

Trong danh sách `git lfs ls-files`, `*` biểu thị nội dung đầy đủ ở working tree; `-` biểu thị pointer. `fsck` kiểm object local, **không thay cho xác nhận working file đã materialize**. Có thể kiểm thêm size của từng working file so với metadata LFS:

```powershell
$lfsFiles = (git lfs ls-files --json | ConvertFrom-Json).files
$badLfs = @($lfsFiles | Where-Object {
    -not (Test-Path -LiteralPath $_.name -PathType Leaf) -or
    (Get-Item -LiteralPath $_.name -ErrorAction SilentlyContinue).Length -ne $_.size
})
$badLfs | Select-Object name, size
if ($badLfs.Count -gt 0) { throw 'LFS working files chưa đủ; chưa mở Unity.' }
```

Mong đợi không có file lỗi, fsck OK. Baseline có 879 working file LFS tham chiếu 797 object; con số có thể đổi sau task hợp lệ. Không áp số cũ để xóa asset mới. Nếu còn pointer: giữ diff, kiểm quyền mạng/quota, chạy lại `git lfs pull`; chỉ dùng `git lfs checkout` khi object đã có và đã bảo toàn thay đổi local liên quan. Không dùng copy file untracked từ máy tác giả để che một clone thiếu nguồn.

<a id="unity"></a>
## 4. Mở Unity và chạy profile riêng

### 4.1. Mở project

1. Unity Hub → **Add project / Add project from disk** → chọn root checkout chứa cùng lúc `Assets`, `Packages`, `ProjectSettings` (không chọn thư mục Assets).
2. Chọn **6000.6.2f1** đúng theo `ProjectSettings/ProjectVersion.txt`. Nếu Hub hỏi nâng phiên bản, dừng và cài đúng Editor. Không tự nâng packages hoặc đổi pipeline.
3. Lần đầu Unity tạo Library và import source; chờ import/compile xong. Lâu là bình thường, **lỗi Console không phải bình thường cần bỏ qua**. Mở Window → General → Console, lọc Error và xem lỗi đầu. Editor log Windows: `%LOCALAPPDATA%/Unity/Editor/Editor.log`.
4. Mở đúng scene:

   ```text
   Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity
   ```

5. Không chạy author/derive/bind/isolate/integrate một lần để “khởi tạo project”. Scene đã lưu có component/binding. Không dùng scene thử rỗng hoặc checkpoint r92 để đánh giá prototype hiện hành.

### 4.2. Chọn profile sạch bằng cơ chế thật

`InvestigationSession.Awake()` trong `Assets/Scripts/InvestigationV36R107/InvestigationSession.cs` đọc **`--investigation-save-dir`**. Nếu bỏ tham số, code dùng `Application.persistentDataPath`; vì vậy phải mở Editor đúng CLI trước phiên thử. Profile được chọn lúc khởi động process, không đổi được bằng cách gõ tham số vào Play.

1. Bảo toàn việc chưa lưu, dừng Play rồi đóng **Editor của checkout mình**, không đóng Unity project của người khác. Nếu Unity đang mở sẵn, Hub có thể đưa bạn về process cũ và CLI mới không áp dụng.
2. Tại root checkout, chạy đoạn sau. Dán đường dẫn Editor thực tế trên máy mình khi được hỏi (Hub → Installs giúp tìm vị trí):

   ```powershell
   $projectPath = (Get-Location).Path
   $unityExe = Read-Host 'Đường dẫn đầy đủ tới Editor/Unity.exe phiên bản 6000.6.2f1'
   if (-not (Test-Path -LiteralPath $unityExe -PathType Leaf)) { throw 'Không tìm thấy Unity.exe' }
   $sessionId = 'PT_' + (Get-Date -Format 'yyyyMMdd_HHmmss') + '_' + [guid]::NewGuid().ToString('N').Substring(0,8)
   $profilePath = Join-Path $env:LOCALAPPDATA ('ConRoi/PlaytestProfiles/' + $sessionId)
   if (Test-Path -LiteralPath $profilePath) { throw 'Profile đã tồn tại; chọn phiên mới.' }
   Write-Host "Project: $projectPath"
   Write-Host "Profile phiên này: $profilePath"
   & $unityExe -projectPath $projectPath --investigation-save-dir $profilePath
   ```

3. Ghi **đường dẫn profile thực tế** và mã phiên vào biên bản. Mở entry, Play. Mong đợi intro **“Ông Tư được phát hiện tử vong tại xưởng rối…”**, chưa có chứng cứ/quan sát, mapping chưa gán, nhận định chưa khẳng định. Các mục kết luận mặc định có thể đã tồn tại, không cần danh sách kết luận có số phần tử bằng 0.
4. Không tự nhấn F6 để nạp một save có sẵn từ phiên kỹ thuật. Khi cần kiểm path, F5 ở trạng thái ban đầu sẽ tạo:

   ```powershell
   $savePath = Join-Path $profilePath 'InvestigationV36/r107/prototype.json'
   Get-Item -LiteralPath $savePath
   Get-Content -LiteralPath $savePath -Encoding UTF8
   ```

   Kiểm schema1, namespace `conroi.investigation.v36.r107` và dữ liệu trống phù hợp; F6 sau đó chỉ tải lại save **của phiên này**. Không chỉnh JSON để cấp chứng cứ rồi gọi đó là New Game sạch. Nếu chưa có file sau F5, dừng kiểm và xác minh process/CLI/log; không đi tìm save gốc rồi dùng thay.
5. Dùng cùng profile để kiểm tiếp giữa phiên. Muốn một New Game khác: đóng Editor an toàn, mở lại với **thư mục mới**, không xóa profile cũ. Không migration save r106 hoặc save gameplay cũ.

### 4.3. Bắt đầu chơi và thao tác hiện hành

1. Ở intro bấm **Bắt đầu**, đọc một mục tiêu: **“Vào xưởng, lên gác và kiểm tra dấu trên sàn gần lan can.”** Spawn sạch nhìn về lối vào. Theo biển/lối vào và marker nhẹ, không tự biết hướng bằng tọa độ debug.
2. Tới dấu sàn, nhìn để có prompt **“E · Kiểm tra dấu trên sàn”**, nhấn E. Trong màn khám, giữ chuột trái để xoay, cuộn để phóng; bấm **Ghi quan sát**, nhấn **R**, rồi **P** xem ghi chép vừa ghi.
3. Mục tiêu tiếp theo chuyển sang **điện thoại trên giá quay cạnh chỗ sửa rối**. Theo mục tiêu hiện hành, sau đó clip/cơ cấu, Hà rồi quay lại xưởng. Không cấp sẵn toàn bộ chức năng bằng fixture khi đang kiểm người mới.

| Thao tác | Phím/nút hiện hành |
|---|---|
| Đi / nhìn | WASD / chuột |
| Tương tác | E khi raycast có prompt |
| Xoay / phóng vật đang khám | Giữ chuột trái / cuộn |
| Ghi | Nút Ghi quan sát |
| Trở lại | R; không chiếm phím đang nhập chữ |
| Mở/đóng hồ sơ | P khi không đang nhập trong ô chữ hoặc pause |
| Đóng trang / pause ở world / resume | Esc theo ngữ cảnh |
| Lưu / tải profile hiện hành | F5 / F6; cũng có nút trong pause/hồ sơ |
| Bỏ qua tutorial | Bỏ qua hướng dẫn ở intro hoặc pause/hồ sơ; không có nghĩa cấp chứng cứ |

Khi modal mở, di chuyển và camera world phải bị chặn. Load mô hình đang chạy trở về trạng thái an toàn theo đặc tả, không bắt tiếp đúng một frame animation chưa lưu. Chi tiết onboarding do `InvestigationOnboarding.cs` điều phối; không gán mọi chuyển trạng thái intro không rõ nguồn input thành lỗi gameplay.

### 4.4. Dừng Play và kiểm thay đổi

1. Ghi lại lỗi/ảnh/state cần thiết trước khi dừng; không tranh input nếu người khác đang thử.
2. Dừng Play. Không lưu runtime scene vào checkpoint hoặc scene chính. Nếu scene dirty, xác định thay đổi Edit có chủ ý hay Unity tự sinh trước khi Save/Discard.
3. Chạy:

   ```powershell
   git status --short
   git diff --stat
   git diff -- Assets Packages ProjectSettings
   ```

4. Unity có thể tự cập nhật settings khi import/thoát; giữ snapshot riêng và hỏi chủ file nếu chưa rõ. Không `git restore .`, reset hoặc discard hàng loạt để làm sạch. Chỉ commit thay đổi có căn cứ thuộc task mình.

<a id="phan-cong"></a>
## 5. Quyền sở hữu và điểm bắt đầu trong source

### 5.1. Bảng phân công

Các tên `Map`, `Gameplay`… trong bảng dưới thuộc `Assets/ConRoi/Modules/`. Module hiện là **ranh giới trách nhiệm**, chưa phải ba assembly hay ba bản game độc lập. Báo cáo task riêng dự kiến đặt dưới `Docs/QA/Tasks/N1`, `N2`, `N3`; các thư mục này được tạo khi có báo cáo thật, không phải source cần copy bù.

| Vai | Nhánh | Thư mục/file sở hữu | Scene thử riêng | Đầu ra | Không tự sửa | Người nhận bàn giao |
|---|---|---|---|---|---|---|
| 1 — Map/asset | `work/map-asset-baseline` | `Map/`, `NgheThuat/`; legacy `Assets/Art`, `Assets/Model`, `Assets/Blender`, `Source`; meta đi cùng | `Assets/ConRoi/Modules/Map/Scenes/Map_ThuNghiem.unity` | Nguồn editable, FBX/mesh/material/rig/collider đã kiểm, báo cáo đúng góc/đối tượng | Luật cơ cấu, điều tra, save/ID; scene tích hợp/checkpoint; GDD; source Tools chưa được chuyển quyền | Người 3 nhận scene wiring; người 2 review contract tương tác |
| 2 — Gameplay/dữ liệu | `work/runtime-contracts-baseline` | `Gameplay/`, `Contracts/`, toàn bộ `Assets/Scripts`, Editor/test, `Tools`; `HOP_DONG_MODULE.md`; cấu hình compile/asmdef | `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity` | API/state/contract mô tả thật, code task có acceptance, dữ liệu mẫu/fixture cách ly | Scene tích hợp/checkpoint, geometry/art, nội dung GDD, schema/ID khi chưa thống nhất | Người 3 nhận code/API và yêu cầu wiring; người 1 nhận yêu cầu collider/binding |
| 3 — UI/UX/tích hợp | `work/ui-onboarding-review` | `GiaoDien/`, `NoiDung/`, `TichHop/`; scene tích hợp, tài liệu chung/GDD/QA; quản lý `Assets/Scenes` nhưng checkpoint bất biến | `Assets/ConRoi/Modules/GiaoDien/Scenes/GiaoDien_ThuNghiem.unity` | View/UI thuộc phạm vi, phiên người mới thật, tài liệu và bản tích hợp đã kiểm từng gói | Geometry và runtime người khác; **mọi partial session**, kể cả UI/onboarding | Người 1/2 review phần liên quan; người 3 điều phối nhận từng gói lên nhánh tích hợp tạm |

Cả ba scene thử trên hiện là **template rỗng**. Muốn kiểm điều tra đầy đủ, có thể mở entry để chạy nhưng người 1/2 không lưu thay đổi vào entry. Cần fixture/sandbox có wiring: ghi task và để đúng chủ chuẩn bị, không tự copy đè scene chính.

### 5.2. File dùng chung và nguồn thực tế

| Phạm vi thực | Chủ sửa duy nhất / cách phối hợp |
|---|---|
| `Assets/Scripts/InvestigationV36R107/InvestigationSession.cs`, `InvestigationSessionUI.cs`, `InvestigationReviewUI.cs`, `InvestigationOnboarding.cs` | **Người 2 giữ trọn** vì partial dùng private state chung; người 3 gửi bug/yêu cầu API/patch đề xuất, không đồng thời sửa trực tiếp |
| `InvestigationEvidence.cs`, `InvestigationInteractable.cs`, `InvestigationActor.cs` cùng thư mục | Người 2 sửa mã/state/ID; người 3 duyệt nội dung, người 1 cung cấp hình/binding. Không đổi ID theo tên mesh mới |
| `ReturnGateMechanism.cs`, `TrialTimeline.cs`, `MechanismPresentation.cs`, `RepairHandBinding.cs`, `PresentationHands.cs` cùng thư mục | Người 2 giữ code/luật; người 1 giữ source art/animation, người 3 trình bày qua contract |
| `Assets/Scripts/NguoiChoi/DieuKhienDiChuyen.cs`, `DieuKhienGocNhin.cs` | Người 2; không đổi controller/Input Actions để thuận tiện công cụ kiểm |
| `Tools/` kể cả `Tools/TaiTao/Legacy`, Editor/test trong Assets/Scripts | Người 2 mặc định. Tool asset muốn giao người 1 phải ghi chuyển quyền từng file, chủ cũ ngừng sửa; không archive type còn consumer compile |
| Entry `ConRoi_Chinh.unity`, GDD, BAT_DAU, lịch sử/workflow, HANDOFF/OWNERSHIP/registry chung | Người 3 quản lý file; thay hướng GDD phải được nhóm thống nhất. Không tự thêm nội dung vì có quyền sửa file |
| Packages/ProjectSettings, `.gitignore`, `.gitattributes`, cấu hình nhóm | Người 2, có review người 3; không tự nâng pipeline/package hoặc reserialize toàn asset |
| `DANG_LAM_NGUOI_1/2/3.md` và báo cáo riêng của mỗi người | Người tương ứng cập nhật; chỉ người 3 tổng hợp lịch sử chung |

Namespace runtime hiện hành: **`ConRoi.Investigation.V36.R107`**. Namespace save là chuỗi **`conroi.investigation.v36.r107`**, schema1; không nhầm hai thứ này. Xem [hợp đồng mô tả source](HOP_DONG_MODULE.md), [registry](EVIDENCE_REGISTRY.md); chưa có một bộ interface/event module độc lập mới. Không tự đặt API rồi xem như đã triển khai.

Một số asset **đã tồn tại** để tra wiring, không phải lệnh chạy author:

- Nguồn map `Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend` → `Assets/Art/Environment/JourneyV48/TerrainR107/TerrainR107_Final.fbx` và các bound mesh theo [MAP_REPAIR](../QA/investigation_r107/MAP_REPAIR.md).
- Nguồn clip `Source/Blender/JourneyV48/R105/Repair_R12_R105_Animation.blend` → `Assets/Art/InvestigationV36/R105/RepairEventActor_R105.fbx`.
- Prefab dấu sàn `Assets/Art/InvestigationV36/R104/TraceCrossing.prefab`. Không phải mọi actor/cơ cấu hiện đã đóng gói thành prefab trong module mới.
- `Assets/ConRoi/Modules/Contracts/README.md` chỉ là điểm đặt trách nhiệm; phần hợp đồng hiện hành ở tài liệu và legacy source.

<a id="viec-dau"></a>
## 6. Gói việc đầu tiên của từng người

### 6.1. Người 1 — triage topology và góc ảnh còn mở

Đọc [DANG_LAM_NGUOI_1](DANG_LAM_NGUOI_1.md), [MAP_REPAIR](../QA/investigation_r107/MAP_REPAIR.md), [nguồn r107](../QA/investigation_r107/ASSET_SOURCES.md). **Mục tiêu:** xác định đúng lỗi còn ảnh hưởng người chơi trước khi đề xuất sửa, tránh san/weld toàn map chỉ vì generic gate FAIL.

1. Ghi HEAD, file dự kiến chạm và phạm vi triage trong file tiến độ riêng. Bắt đầu từ nguồn terrain/FBX ở mục 5.2 và scene Map_ThuNghiem để phân tích asset; mở entry read-only khi cần đối chiếu vị trí thật.
2. Yêu cầu người tổ chức cung cấp đúng bốn ảnh `Screenshot 2026-11-06 105506.png`, `105520.png`, `105534.png`, `105627.png` từ `Test/BugModel` trên máy tác giả. Chúng **chưa tracked**, không hứa có trong clone. Chưa nhận ảnh thì đánh dấu phụ thuộc, không gọi góc khác là matched before/after.
3. Ưu tiên ba pose chưa khớp 105506/105520/105534; ghi player/camera, object/GUID, mesh render/collider, nguồn và mức ảnh hưởng. Góc 105627 đã sửa có phạm vi, dùng làm mốc đối chiếu chứ không suy toàn vùng an toàn.
4. Phân tích topology rời/coplanar/loose và 8 tam giác cực mảnh trong báo cáo hiện hành. Tách cảnh báo kỹ thuật khỏi lỗi nhìn thấy/đi lại được; chưa có lỗi xác nhận thì chỉ báo cáo và đề xuất task tiếp.
5. File được sửa trong gói đầu: `DANG_LAM_NGUOI_1.md`, báo cáo/bằng chứng nhỏ của N1; phân tích nguồn trong sandbox, không ghi đè nguồn chính chỉ để audit. Thay asset sản phẩm chỉ ở task sửa riêng được thống nhất dựa trên lỗi xác nhận, trong vùng ownership N1.

**Bàn giao/kiểm đạt:** bảng từng lỗi hoặc góc chưa tái hiện, ảnh/camera/source/GUID/collider, severity, đề xuất sửa có phạm vi và phần chưa kiểm. Người 3 nhận báo cáo, người 2 review điều kiện tương tác/controller nếu ảnh hưởng. Không đạt bằng một log topology đơn lẻ hoặc tuyên toàn map PASS.

Ở task sửa asset tiếp theo: **Blender editable → lưu/mở lại → xuất FBX/asset → Unity material/rig/collider → controller**; giữ GUID và checkpoint. Cảnh quan tiếp tục theo nguồn tự dựng/ảnh tham chiếu có căn cứ hiện hành, không tự thay bằng pack landscape tải mạng. Ảnh tham chiếu chưa có quyền phát hành không dùng làm texture phân phối. Asset online được đề xuất phải có tác giả, URL gói chính thức, phiên bản, license đúng gói và attribution trong bảng nguồn; không dùng asset rip hoặc OBJ không rõ giấy phép. Nguồn Quaternius đang dùng xem [r104](../QA/investigation_r104/ASSET_SOURCES.md). Không tự đổi luật, save hoặc scene tích hợp để làm hình đẹp.

### 6.2. Người 2 — xác minh contract và chọn một task nhỏ theo GDD

Đọc [DANG_LAM_NGUOI_2](DANG_LAM_NGUOI_2.md), GDD v3.6 và [HOP_DONG_MODULE](HOP_DONG_MODULE.md). **Mục tiêu:** để UI/asset có biên dùng thật, tránh gọi ba folder là ba module độc lập khi session vẫn chung state.

1. Bắt đầu tại `Assets/Scripts/InvestigationV36R107/`, namespace và schema ở mục 5; đọc mọi partial liên quan trước khi đề xuất API. Ghi file sẽ sửa trong tiến độ riêng.
2. Đối chiếu mã hiện có: `Acquire`/`Observe` và trạng thái quan sát; `OpenEvidence`/`OpenPage`/`ClosePage`/`SetPaused`; `ReadCipher`/`Assign`/`AddRelation`; `StartTrial`/`ClearTrialHolds`; `Save`/`Load`/`SavePath`. Đây là tên API có thật, không phải yêu cầu gọi chúng từ fixture cho người mới.
3. Bổ sung hợp đồng bằng producer/consumer, type/ID, điều kiện gọi, đầu ra/state, lỗi và acceptance thực tế. Chọn **một** khoảng thiếu được xác minh trong tuyến xưởng → Hà → xưởng, gửi người 3 review trước khi triển khai. Không nhận nhiệm vụ “làm toàn bộ vụ án”.
4. File được sửa trong gói đầu: tiến độ N2, `HOP_DONG_MODULE.md`, báo cáo riêng. Mã runtime chỉ thay sau khi task nhỏ và biên đã thống nhất; scene Gameplay_ThuNghiem làm sandbox, không lưu vào entry.
5. Giữ ID vật chứng, schema1/namespace save, luật hồi hữu hạn, sáu ca chung cơ cấu và seek không cấp quan sát. Giữ NAV-01; không tự đổi GDD hoặc loại các lớp Editor/test còn được compile/tool gọi.

**Bàn giao/kiểm đạt:** contract khớp source, task được review có đầu vào/đầu ra và ca kiểm. Khi có code, giao commit SHA, chữ ký API/event **thực sự đã thêm hoặc đổi**, thời điểm gọi và cleanup khi đóng/pause/load, hướng dẫn wiring cho N3. Nếu chưa có event thì ghi chưa có, không bịa tên. Dữ liệu mẫu/fixture phải mô tả ID, trạng thái seed, profile riêng và cách tái tạo; không đổi schema hay đưa fixture vào New Game. N1 cung cấp asset/binding khi task cần; N3 xác nhận navigation/view tiêu thụ được contract.

### 6.3. Người 3 — tổ chức người mới trước mở rộng onboarding

Đọc [DANG_LAM_NGUOI_3](DANG_LAM_NGUOI_3.md), [phiếu người tổ chức](../QA/investigation_r107/PLAYTEST_NGUOI_MOI.md), [hướng dẫn người chơi](../QA/investigation_r107/HUONG_DAN_NGUOI_CHOI.md), [mẫu ghi phiên](../QA/investigation_r106/MAU_GHI_NHAN_PHIEN.md), [mẫu vấn đề](../QA/investigation_r106/MAU_TONG_HOP_VAN_DE.md). **Mục tiêu:** xác định người chưa biết đáp án mắc ở đâu trước khi thêm hướng dẫn/công cụ.

1. Dùng entry **ConRoi_Chinh.unity** của mục 4, không dùng đường scene/checkpoint hay đường D tuyệt đối trong phiếu lịch sử. Quy trình phiếu trung tính giữ nguyên; mở bằng profile mới theo CLI đã có.
2. Ghi mã người thử, commit, scene/profile thực, độ phân giải và thời điểm. Chỉ đưa trang dành cho người chơi; không đưa GDD, hướng dẫn kỹ thuật này hoặc thời điểm then chốt clip.
3. Để họ tự đi tuyến; chỉ hỗ trợ thao tác khi yêu cầu, ghi nguyên văn câu hỏi và gợi ý. Tách điều nhìn thấy, điều suy ra/căn cứ và diễn giải của người tổ chức. Ghi riêng không hiểu cơ cấu, không đọc được hình/chữ, không biết thao tác, lỗi chức năng, hoặc hiểu nhưng thấy chứng cứ chưa đủ. Không coi phản biện là không hiểu.
4. Tổng hợp theo mức ảnh hưởng: chặn chơi/mất dữ liệu → hiểu sai chứng cứ/cơ cấu → khó thao tác/đọc → góp ý hình nhỏ. Chỉ đề xuất sửa có tình huống/vị trí/nhận xét cụ thể, không chấm bằng đoán đúng Khải.
5. File được sửa trong gói đầu: tiến độ N3, biên bản thật/báo cáo riêng; tài liệu chung khi tổng hợp. UI thử trong GiaoDien_ThuNghiem chỉ khi có task cụ thể; không trực tiếp sửa session partial để mở rộng onboarding.

**Bàn giao/kiểm đạt:** biên bản trung thực + danh sách vấn đề có căn cứ/đề nghị đúng chủ file. Chưa có người thật thì **NOT_RUN**, bàn giao phiếu/cách khởi động sẵn sàng và chờ lịch, không dùng agent biết GDD thay người mới, không phát sinh polish. Phụ thuộc: nhóm cung cấp người thử chưa đọc GDD và máy đủ điều kiện; N2 xử lý lỗi runtime được xác minh, N1 xử lý lỗi hình/geometry được giao.

<a id="giai-doan"></a>
## 7. Các giai đoạn phát triển tiếp theo

Đây là **thứ tự công việc tiếp theo**, không phải xác nhận đã triển khai đầy đủ hoặc lịch hẹn. Chỉ mở rộng sau gate; không thêm khu/thư/ending trong gói đầu.

| Giai đoạn | Đầu ra | Chủ trì | Phụ thuộc | Điều kiện chuyển bước |
|---|---|---|---|---|
| 1. Nhận project | Mỗi máy đúng nhánh/LFS/Unity/entry, profile riêng | Từng người; N3 tổng hợp | Quyền repo, dung lượng, công cụ | Checklist nhập nhóm có bằng chứng máy đó; không mượn PASS máy tác giả |
| 2. Gói đầu | N1 triage map; N2 contract/task nhỏ; N3 phiên người mới hoặc NOT_RUN có chuẩn bị | Theo vai | Ảnh gốc, contract, lịch người thử | Báo cáo/commit có phạm vi, người nhận review; phần chờ ghi rõ |
| 3. Tích hợp prototype | Nhận từng task đã duyệt, compile/reference, tuyến xưởng → Hà → xưởng | N3; N2 review runtime, N1 review asset | Module/asset và hướng dẫn wiring đầy đủ | Đúng chức năng chịu ảnh hưởng, save riêng không mất trạng thái, không mở chứng cứ bằng seek |
| 4. Playtest và sửa theo phản hồi | Người mới tự mô tả quan sát/suy luận, issue có bước tái hiện | N3 tổ chức; chủ file sửa | Bản tích hợp ổn định, người thử thật | Phân tích phản hồi, sửa lỗi cụ thể rồi kiểm lại phạm vi; chưa có người giữ NOT_RUN |
| 5. Lát cắt và mở rộng theo GDD | Theo thứ tự GDD mục 12: lát cắt văn phòng/xưởng/hồ-chợ/homestay/khu hẹn tối thiểu, sau đó trường/dốc/vườn/hậu đài và nội dung tương ứng | Nhóm thống nhất, N3 điều phối | Gate cơ cấu/suy luận/người mới và quyết định scope | Đầu ra từng gói/chứng cứ có nơi thật, hồi quy đạt; không đưa tài liệu khu chưa dựng làm khóa ẩn |

Mã ending A/C và các diễn biến A-treo/A-sai thuộc **thiết kế tương lai theo GDD**, không phải chứng nhận toàn game/ending đã chơi xuyên suốt ở prototype hiện tại. Không hứa ngày khi nhóm chưa có lịch, không làm build/ZIP/runner trong công việc bàn giao này.

<a id="hang-ngay"></a>
## 8. Quy trình hằng ngày

1. **Trước làm**, tại checkout mình:

   ```powershell
   git branch --show-current
   git status --short
   git rev-parse HEAD
   git fetch origin
   git log --oneline --left-right 'HEAD...@{upstream}'
   ```

   Ref có `@{...}` được quote để PowerShell không phân tích như cú pháp riêng. `fetch` chỉ cập nhật remote-tracking refs, không thay working files. Nếu mạng lỗi, ghi đang dùng mốc offline nào; không giả định đã nhận tiến độ mới.
2. Nếu có thay đổi chưa commit, xem diff và xác định của ai. Bảo toàn bằng commit đúng phạm vi hoặc snapshot/stash có tên khi thích hợp. `git stash` mặc định không giữ untracked/ignored; không dùng nó làm bằng chứng đã giữ save/asset nguồn. Chưa rõ thì dừng pull/switch, không overwrite.
3. **Chỉ khi working tree đã xử lý an toàn** và đúng nhánh có upstream:

   ```powershell
   git pull --ff-only
   ```

   Nếu không fast-forward được, không đổi thành force/rebase/reset tùy tiện; xem mục lỗi. Không cần ép các nhánh vai trò giống nhau.
4. Đọc task/ownership, cập nhật DANG_LAM của mình bằng mẫu mục 10; ghi file đang giữ trước khi sửa. Làm trong phạm vi, kiểm bằng scene thử riêng; entry chỉ N3 lưu.
5. Trước commit, thoát Play, xem:

   ```powershell
   git status --short
   git diff --stat
   git diff --check
   git diff
   ```

   Asset đi cùng `.meta`, nguồn editable và license nếu task thay asset. Kiểm file tự sinh; không add Library/Temp/Obj/Logs/UserSettings/cache/profile/save cá nhân/capture/archive. Không ignore mọi `.asset`, `.meta`, video hoặc toàn Source để làm status ngắn.
6. Stage **từng file cụ thể**, xem staged diff rồi commit nhỏ. Ví dụ hợp lệ nếu người 1 thực sự cập nhật tiến độ đã nhận:

   ```powershell
   git add -- Docs/QuanLy/DANG_LAM_NGUOI_1.md
   git diff --cached --stat
   git diff --cached --check
   git diff --cached
   git commit -m "docs: record N1 topology triage progress"
   ```

   Người 2/3 thay đúng file của mình. Không chạy `git add .` hoặc tạo commit rỗng chỉ để thử.
7. Khi task sẵn sàng và được phép publish, push **đúng nhánh mình**, không `--all`, `--mirror`, `--force`:

   ```powershell
   # Chỉ chạy dòng ứng với vai của mình, sau khi kiểm branch/status.
   git push origin work/map-asset-baseline
   # Người 2 dùng: git push origin work/runtime-contracts-baseline
   # Người 3 dùng: git push origin work/ui-onboarding-review
   ```

   Git LFS hook upload object cùng thay đổi; quota lỗi thì báo chủ repo, không tự mua hoặc bỏ LFS. **Đợt tạo hướng dẫn này chưa push**; lệnh là quy trình cho task phát triển tiếp được giao.
8. Bàn giao SHA + file đổi + cách kiểm/kết quả/giới hạn theo mẫu; chỉ N3 tổng hợp tài liệu chung. Không reset --hard, git clean hoặc xóa dữ liệu như cách xử lý lỗi thường ngày.

<a id="tich-hop"></a>
## 9. Phối hợp và tích hợp

### 9.1. Nhánh và PR hiện hành

Nhánh tích hợp **tạm đã được TICH_HOP quy định là `chore/prepare-r107`**, do N3 điều phối. Đây không phải việc chọn nhánh của N1/N2 làm tích hợp. `work/ui-onboarding-review` vẫn là nhánh task người 3. Đợt viết tài liệu không merge, đổi default branch hoặc xử lý compare/main.

Sau khi có task thật và được review, tạo PR trên GitHub bằng cách **chọn base thủ công**, không nhận `main` mặc định:

| PR | Base | Head |
|---|---|---|
| Map/asset | `chore/prepare-r107` | `work/map-asset-baseline` |
| Gameplay/contracts | `chore/prepare-r107` | `work/runtime-contracts-baseline` |
| UI/docs | `chore/prepare-r107` | `work/ui-onboarding-review` |

Nếu giao diện báo lịch sử không liên quan/không compare được, xác nhận base/head và báo N3; không tự merge main để chữa. Khi các nhánh task có tiến độ riêng, **không fast-forward/reset tất cả về một SHA chung**. Chỉ nhận thay đổi cần thiết sau bảo toàn và review. Chính sách đổi nhánh tích hợp/main tương lai cần quyết định riêng, chưa triển khai trong hướng dẫn này.

### 9.2. Khi chạm file người khác

1. Gửi phiếu yêu cầu mục 10: file/API, lý do, bước tái hiện hoặc contract cần dùng, acceptance, ai đang giữ file.
2. Chủ file xác nhận và tự sửa, hoặc ghi chuyển quyền đúng từng file/commit trong DANG_LAM; chủ cũ ngừng sửa. Không hai người cùng sửa partial UI/session hoặc scene chính.
3. Đổi contract phải thống nhất producer/consumer, schema/ID và tương thích trước; **N2 giữ source contract**, N3 cập nhật view theo API đã có, N1 cập nhật binding được giao. Không tự tạo event mà caller chưa biết.

### 9.3. N3 nhận từng gói

1. Kiểm SHA, file ownership, nguồn/license/meta, hướng dẫn wiring và bằng chứng. Thiếu input thì trả yêu cầu cụ thể, chưa merge gói chưa hiểu.
2. Bảo toàn working tree của checkout tích hợp. Review một PR/gói trước, không nhận đồng thời ba bản scene chính. N1/N2 giao asset/prefab/script và hướng dẫn vị trí/binding; **không chép nguyên scene nhánh khác đè entry**.
3. N2 review compile/API/state, N1 review mesh/material/collider. N3 là người duy nhất sửa wiring entry và tổng hợp nội dung. Không đổi serialized fields để làm mất binding cũ.
4. Sau tích hợp, kiểm import/compile, Missing Script/reference trong phạm vi rõ và chức năng bị tác động: input/modal, onboarding/khám/ghi/P, navigation/save-load nếu đổi UI/state; controller/collider nếu đổi map/rig. Chỉ chạy lại toàn tuyến khi phần thay có ảnh hưởng; không lặp toàn bộ ca PASS cho thay docs.
5. Ghi commit tích hợp, ca đạt/chưa chạy, profile và evidence; rồi mới nhận gói tiếp. N3 cập nhật HANDOFF/lịch sử chung, giữ FAIL/NOT_RUN đúng nguồn.

**Conflict scene/prefab/FBX/Blend:** dừng xử lý tự động, giữ nguyên trạng thái conflict và báo chủ file. Xem `git status`, liệt kê hai phía/nguồn chỉnh sửa, quyết định hợp nhất ở công cụ thích hợp. Không chọn ours/theirs hàng loạt, không xóa meta hoặc tự sinh component mới để hết lỗi. Nếu cần hủy merge, chỉ làm sau khi snapshot thay đổi từ trước và trong merge; không dùng abort/reset khi chưa biết sẽ mất gì.

<a id="mau"></a>
## 10. Mẫu tiến độ, yêu cầu và bàn giao

Copy mẫu tiến độ vào **DANG_LAM_NGUOI_1.md / 2.md / 3.md của chính mình**, giữ nội dung nhiệm vụ/hướng dẫn đã có. Chỉ N3 tổng hợp tài liệu chung; không để cả ba commit vào cùng lịch sử/GDD.

```markdown
### Cập nhật nhiệm vụ
- Mã nhiệm vụ / ngày:
- Người / vai / nhánh:
- Commit xuất phát (SHA đầy đủ):
- Mục tiêu và acceptance đã thống nhất:
- Đang làm bước:
- File đang sửa (kể cả meta/nguồn):
- File giao nhau / chủ đang giữ / xác nhận chuyển quyền nếu có:
- Đã làm:
- Đã kiểm (scene, Unity, profile, loại input và kết quả):
- Chưa kiểm / lỗi còn mở / phạm vi không kết luận:
- Đang chờ ai / cần gì:
- Bước tiếp theo:
```

```markdown
### Yêu cầu chủ file khác
- Người gửi → người nhận / task liên quan:
- File/API/state hoặc asset binding cần dùng:
- Tình huống / bước tái hiện / hình mong đợi và thực tế:
- Đề nghị / contract đầu vào-đầu ra cần thống nhất:
- Acceptance và kiểm hồi quy dự kiến:
- Đang chặn gì / dữ liệu mẫu (nếu có, ghi fixture cách ly):
- Quyết định chủ sửa và file chuyển quyền (chỉ sau xác nhận):
```

```markdown
### Bàn giao gói việc
- Mã nhiệm vụ / người / head branch / base dự kiến:
- Commit xuất phát / commit bàn giao (SHA đầy đủ) / PR nếu đã có:
- Danh sách file đổi và chủ file; GUID/meta có đổi không:
- Nguồn editable / phiên bản công cụ / license & attribution:
- API/event đã có, thay đổi contract và cách wiring (không tự đặt tên):
- Dữ liệu mẫu / trạng thái seed / profile riêng / cách tái tạo:
- Cách kiểm từng bước / kết quả mong đợi / thực tế:
- Bằng chứng: UI chuột/phím thật hoặc tổng hợp / API fixture / render minh họa / người thật:
- Bằng chứng/path/manifest/SHA256 cần tra:
- Chưa kiểm / OPEN / NOT_RUN / rủi ro tương thích:
- Người nhận / phần cần N1/N2/N3 review / điều kiện tích hợp:
```

Không ghi dữ liệu cá nhân không liên quan. Phiên người thử dùng mã phiên, nguyên văn nhận xét/gợi ý và profile path; không ghi PASS nếu chỉ có agent/fixture biết đáp án.

<a id="loi"></a>
## 11. Khắc phục lỗi thường gặp

| Dấu hiệu | Kiểm | Cách xử lý bảo toàn |
|---|---|---|
| Clone chỉ thấy README, không có Assets | `git branch --show-current`, `git branch -a`, HEAD | Có thể đang ở main. Giữ file mới/diff trước, fetch rồi switch nhánh vai đã tồn tại; nếu chưa có local dùng `git switch --track origin/work/ui-onboarding-review` cho N3, thay đúng nhánh N1/N2. Không reset hoặc clone đè |
| Git LFS thiếu / FBX/ảnh là text pointer | `git lfs version`, `git lfs ls-files`, size working files, fsck | Cài LFS, mở terminal mới, `git lfs install --local`, `git lfs pull`; xác minh quyền/quota. Không import pointer, đổi đuôi file hoặc lấy Library máy khác để bù |
| Hub báo project khác version/nâng cấp | `ProjectSettings/ProjectVersion.txt`, Editor chọn trong Hub | Cài/chọn 6000.6.2f1; chưa Save bằng version khác. Nếu đã sinh diff, giữ snapshot và hỏi N2/N3, không discard hàng loạt |
| Compile error / Missing Script / reference null | Console lỗi đầu, Editor.log, đúng SHA/entry, LFS/meta/package lock; so `git status` | N2 xử lý compile/type, N3 xử lý wiring scene, N1 xử lý asset. Không xóa component/meta, tạo type giả hoặc chạy author một lần để “sửa” |
| P không mở hồ sơ | Đang pause/ô nhập focus hay world? Có tổ tiên sửa NAV-01 074caf3 không? Ghi Page/ngữ cảnh/chuỗi thao tác | NAV-01 đã sửa focus sót sau đóng UI; P trong ô nhập vẫn cần bị chặn đúng. Xem ancestry bên dưới; lỗi khác phải tái hiện riêng. Esc → Ghi chép có thể là đường tạm, không thay PASS. Gửi N2, không ép OpenPage mỗi frame |
| Scene dirty/settings đổi sau Play/import | Play đã dừng chưa, diff text/scene, thay Edit có chủ ý không? | Snapshot state/diff, hỏi chủ file trước Save/Discard. Không save runtime vào checkpoint hoặc dùng restore/reset toàn repo |
| Push bị từ chối / non-fast-forward | Đúng tài khoản có quyền? `git fetch origin`, xem ahead/behind/diff với upstream | Bảo toàn local commits/files; báo N3 và chủ nhánh để review merge phù hợp. Không force-push/reset hoặc tự gộp lịch sử không liên quan |
| LFS upload/download báo quota/billing | Giữ thông báo HTTP/quota và số object còn thiếu; không in token | Báo chủ repo để quyết định dung lượng/quota. Không mua thêm, bỏ LFS, push pointer thiếu object hoặc báo clone đạt |
| Conflict scene/prefab/FBX/Blend hoặc thiếu meta | `git status`, chủ file, GUID trong asset reference và meta cũ | Giữ hai phía có nguồn, thống nhất chủ hợp nhất. Asset move bằng Unity hoặc cặp asset/meta; không chọn ours/theirs cả cây, không tự tạo GUID thay cho meta bị mất |
| Hết dung lượng khi clone/import | Dung lượng ổ checkout, working tree + Git/LFS + Library + dự phòng | Dừng lô/import, ghi đã tải gì, chọn vị trí local đủ chỗ được phép; không xóa cache/save/nguồn để chữa. Move cùng ổ không tăng free space |
| Không có D:/Game, D:/Photo hoặc D:/data_unity trên máy mình | Link đó là archive/ảnh tham chiếu/Editor tác giả hay dependency runtime? | Editor dùng đường cài của mình, project dùng root clone. Tra manifest và yêu cầu dữ liệu đúng file nếu task cần. Nếu runtime/compile đòi archive tuyệt đối, báo lỗi N2/N3; không tạo đường giả hoặc copy toàn kho |
| Play mở hồ sơ đã giải, không thấy intro sạch | Process Unity có nhận CLI mới không, thư mục profile đã dùng chưa, có nhấn F6/fixture không? | Bảo toàn phiên, đóng đúng Editor rồi mở profile mới chưa tồn tại. Không xóa save cũ, không gọi API reset và coi đó là khởi động sạch |

Kiểm tổ tiên bản sửa NAV-01:

```powershell
git merge-base --is-ancestor 074caf3dd3da4107b4fc9ac1c089b20216b4705b HEAD
$LASTEXITCODE
```

0 chỉ xác nhận lịch sử có commit sửa, không chứng minh một thay đổi mới chưa gây lỗi khác. Trong PowerShell luôn quote ref có `@{...}` khi copy lệnh. Nếu chưa rõ quyền/file/state, giữ nguyên và báo chính xác bước đang chặn thay vì thử lệnh phá trạng thái.

<a id="ngoai-git"></a>
## 12. Dữ liệu ngoài Git

1. **D:/Game là kho archive trên máy tác giả**, không phải thư mục bắt buộc của mỗi thành viên. Hai lô đã ghi nhận: `ConRoi_LuuTru/git-team-20261106-153912-ee41c6` và `ConRoi_LuuTru/git-review-20261107-001945-00b993`. Dữ liệu không được xóa.
2. Tra [archive-manifest](archive-manifest.jsonl), [archive-result](archive-result.json), [profiles-final](profiles-final.json), [bảng đường dẫn](OLD_NEW_PATHS.md), [review](Review20261107/README.md) và [index bằng chứng NAV-01](NavFix20261108/evidence-index.json). Chỉ mục nhỏ ở Git; capture/log dài/profile/snapshot có thể nằm ngoài repo. Một số đường cũ `Test/KetQua` đã thành `Docs/QA`; tra mapping, không tự copy lại toàn Test vào Assets.
3. Khi cần file ngoài repo, gửi người tổ chức đường cũ/mới từ manifest, nhóm dữ liệu, bytes/SHA256 và lý do task cần. Nhận bản copy đúng phạm vi qua kênh nhóm thống nhất; kiểm hash, không ghi đè file đang có. Không yêu cầu toàn archive hoặc save cá nhân chỉ để mở project.
4. **Nguồn thiết yếu hiện hành** (Assets/Packages/ProjectSettings, source Blender cần phát triển và license) phải truy cập được từ Git/LFS hoặc cơ chế bàn giao nguồn được xác nhận. Runtime/compile của entry đã kiểm không cần D:/Game. Nếu phát hiện source thiết yếu chỉ còn ở archive, giữ lỗi BLOCKED tại task đó, thống nhất cách đưa nguồn vào bàn giao; không lặng lẽ thêm absolute path runtime.
5. Bộ Quaternius nguồn/license giữ trong `Source/Downloads/Quaternius_R104`; xem bảng nguồn r104/r105/r107, không tải lại một bộ nhân vật khác để giải quyết tài liệu.
6. **4.183 legacy chưa xác minh** tại project D vẫn phải giữ nguyên. Chúng chưa thuộc baseline tracked đã kiểm; không xóa/move vì tên revision cũ, không add toàn bộ vào nhánh mình. Clone thiếu nhóm D không đồng nghĩa phải copy bù: entry/compile closure đã kiểm có nguồn riêng. Nếu task chứng minh một file D cần thiết, báo bằng dependency/consumer và giao một task bổ sung có review.
7. Library/Temp/Logs/cache, UserSettings, profile/save cá nhân, capture và kho archive không commit. Script kiểm/author cần tái tạo đã có trong Tools theo phạm vi sở hữu N2; không di chuyển/dọn tiếp 852 script lịch sử hoặc chạy chúng hàng loạt. Không tạo build/ZIP/runner để thay quy trình source.

<a id="checklist"></a>
## 13. Checklist nhập nhóm và thông tin cần điền

Mỗi người copy checklist vào file tiến độ riêng và đánh dấu **theo máy mình**, có ngày/HEAD/profile. Chưa thực hiện để NOT_RUN, không lấy kiểm kỹ thuật máy tác giả thay cho kết quả máy mới.

- [ ] Có tài khoản GitHub riêng, quyền repo phù hợp; không dùng chung credential.
- [ ] Clone đúng nhánh vai vào thư mục riêng; ghi HEAD và baseline ancestry.
- [ ] Git LFS hoạt động, working file materialized và fsck đạt.
- [ ] Đủ dung lượng cho source/Git/LFS/Library và dự phòng.
- [ ] Unity 6000.6.2f1; import/compile xong, không Error chưa giải quyết.
- [ ] Mở đúng `ConRoi_Chinh.unity`; compile/reference kiểm trong phạm vi ghi rõ.
- [ ] Chạy profile mới qua CLI, biết path F5/F6 thực tế; không dùng save đã giải.
- [ ] Đã thử đầu vào → dấu sàn → E → xoay/phóng → ghi → R → P trên máy mình, hoặc ghi ca chưa chạy.
- [ ] Đọc GDD v3.6, ownership, contracts, registry và DANG_LAM đúng vai.
- [ ] Biết scene thử hiện rỗng, file nào được sửa/không sửa và chủ partial session.
- [ ] Ghi nhiệm vụ/file đang giữ; biết gửi gói bàn giao cho ai và base/head PR.
- [ ] Xem diff/meta/nguồn/license trước commit; không stage cache/save/archive.
- [ ] Commit/push một thay đổi hợp lệ **khi thực sự có việc**, không commit rỗng để thử quyền.
- [ ] Người 3 ghi kết quả từng máy và người mới đúng loại bằng chứng; chưa chơi thật giữ NOT_RUN.

### Thông tin còn cần nhóm xác nhận

| Thông tin | Ai cung cấp / nơi ghi |
|---|---|
| Tên/username GitHub của từng người và lời mời collaborator đã nhận | Chủ repo và từng người; ghi DANG_LAM, sau đó N3 tổng hợp. CODEOWNERS chưa có danh tính đầy đủ |
| Root checkout, đường Editor, dung lượng, kết quả nhập nhóm từng máy | Từng người; không bắt dùng D:/ConRoi |
| Ai đang giữ task/file, acceptance và thời điểm chuyển quyền | Chủ task trong tiến độ riêng; N3 điều phối file giao nhau |
| Bốn ảnh gốc BugModel và nguồn tham chiếu cho N1 | Người tổ chức cung cấp theo index/hash khi N1 nhận task |
| API/event/fixture cụ thể cần thêm sau review | N2 thống nhất với N3/N1; hiện chỉ có hợp đồng mô tả source, không bộ API module mới |
| Người mới, lịch phiên và kênh lưu bằng chứng thật | N3 tổ chức cùng nhóm; chưa có lịch thì không hứa ngày hoặc tự tạo kết quả |
| Người review, quyền merge/quy tắc bảo vệ nhánh và quota LFS cho task tiếp | Chủ repo/N3 xác nhận; hướng dẫn không tự thay cài đặt repository |

### Phạm vi rà soát của tài liệu này

Đã đọc nguồn bàn giao/ownership/ba nhiệm vụ/GDD/registry/contracts/checkout/NAV-01, kiểm đường dẫn source/scene/prefab nêu trên, đo size file tracked và đọc SHA remote. Không chạy lại Unity/LFS, không sửa code/scene/GDD/save, không giải quyết compare/main hoặc tự merge/push. Hướng dẫn phải được thành viên thực hiện và ghi kết quả riêng trước khi tuyên bố cả nhóm đã clone/chạy đạt.
