# Playtest người mới — baseline kỹ thuật 09/11/2026

**NOT_RUN: chưa có người chơi thật.** UI/UX & Tích hợp tổ chức. Không dùng fixture, cấp chứng cứ, tool auto-play hoặc input tổng hợp cho phiên người thật.

Baseline: `c93cb4f1e47acd14fb4dc9502b8a0b77c2d2a7f3`; nguồn thực kiểm Unity: `f242b60a2fb48c45b37caabd8a55836e4d7db53b`. Unity **6000.6.2f1**, scene `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`. Nhánh phân phối: `review/runtime-integration-20261109`. Commit tài liệu vận hành sau baseline không được gọi là chạy lại sản phẩm. [HANDOFF](../../../QuanLy/HANDOFF.md).

## 1. Chỉ đưa phần này cho người chơi

> Bạn đang điều tra một sự việc tại xưởng rối. Hãy bắt đầu với hướng dẫn trên màn hình, tự khám phá và nói thành lời điều bạn đang nghĩ. Không cần biết trước cốt truyện hay lời giải. Bạn có thể hỏi khi thao tác không rõ, dừng hoặc nghỉ bất cứ lúc nào.
>
> WASD để đi, chuột để nhìn. E kiểm tra khi có lời nhắc; ghi bằng nút “Ghi quan sát”. R trở lại, P xem ghi chép, Esc đóng trang hoặc tạm dừng khi đang đi. F5 lưu, F6 tải phiên thử. Khi xem vật: giữ chuột trái để xoay, cuộn để phóng. Nếu không nhớ phím, hãy dùng các nút và hướng dẫn trên màn hình.
>
> Hãy tự quyết định nên kiểm tra gì và điều gì còn thiếu. Người tổ chức không chỉ đáp án.

Không đưa GDD, registry, báo cáo kỹ thuật/fixture hoặc phần ghi nhận của người tổ chức cho người chơi trước phiên. Không hướng dẫn nghiệm mật mã, thời điểm clip, diễn biến hoặc kết luận cần chọn.

## 2. Người tổ chức chuẩn bị baseline và máy

1. Hỏi người thử từng xem/chơi/đọc lời giải chưa, ghi trung thực. Chỉ gắn nhãn “người mới” cho người mới thật; người đã biết là kiểm sử dụng.
2. Dùng checkout riêng, Library riêng, không đổi nhánh/pull trên working directory người khác đang sửa. Nếu cần clone, kiểm ổ local đủ cho Git/source/LFS **cùng Library/import và dự phòng**; không coi dung lượng LFS là tổng project, không xóa dữ liệu lấy chỗ.

```powershell
# Chỉ clone nếu thư mục chưa có; chọn vị trí local phù hợp trước.
if (Test-Path '.\ConRoi_Playtest') { throw 'Chọn thư mục mới, không ghi đè.' }
git lfs install
git clone --single-branch --branch review/runtime-integration-20261109 https://github.com/Trug-Kin/GamePuppet.git ConRoi_Playtest
Set-Location .\ConRoi_Playtest
git lfs pull
git status --short
git branch --show-current
git rev-parse HEAD
git merge-base --is-ancestor c93cb4f1e47acd14fb4dc9502b8a0b77c2d2a7f3 HEAD
if ($LASTEXITCODE -ne 0) { throw 'Không chứa baseline; dừng và hỏi UI/UX & Tích hợp.' }
```

3. Ghi SHA thực chạy. HEAD mới hơn baseline: kiểm diff Assets/Packages/ProjectSettings/Tools với baseline; nếu đổi sản phẩm, ghi rõ phiên bản và điều kiện kiểm, không tự gọi cùng baseline. Không reset nhánh có tiến độ; cần SHA cũ thì phối hợp dùng checkout riêng.
4. Xác minh LFS materialize, không asset nào chỉ còn pointer. Thiếu object hoặc dung lượng thì BLOCKED trước import. Unity đúng version, đợi compile/import, xem Console. Không chạy tool author/check có cấp dữ liệu; không chạy lại toàn bộ API/UI đã đạt để chuẩn bị phiên.

## 3. Profile sạch, không fixture

1. Đóng Editor của **checkout chơi thử** sau khi bảo toàn mọi thay đổi chưa lưu; không taskkill. Không mở hai Editor cùng project.
2. Từ root checkout riêng, chọn Unity.exe thật trên máy. Tạo profile chưa từng dùng rồi launch với tham số thật `--investigation-save-dir` được Awake đọc:

```powershell
$projectPath = (Get-Location).Path
$unityEditor = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe'
if (-not (Test-Path -LiteralPath $unityEditor)) { throw 'Chọn Unity.exe 6000.6.2f1 thực trên máy.' }
$profilePath = Join-Path $projectPath ('Test\PlaytestProfiles\Newcomer_' + (Get-Date -Format 'yyyyMMdd_HHmmss') + '_' + [guid]::NewGuid().ToString('N').Substring(0,8))
if (Test-Path -LiteralPath $profilePath) { throw 'Profile đã có; chọn profile mới.' }
New-Item -ItemType Directory -Path $profilePath | Out-Null
# Ghi lại profilePath, không copy save/fixture vào đây.
& $unityEditor -projectPath $projectPath --investigation-save-dir $profilePath
```

Đường dẫn Editor trên là ví dụ máy tổ chức, không bắt máy khác có ổ D hoặc D:\Game. Mở đúng cửa sổ Unity được khởi chạy, không launch lần hai từ Hub làm mất tham số profile.

3. Mở scene tích hợp nêu trên, Game **1920×1080**, đợi Console ổn trước Play. Chờ người chơi sẵn sàng. Profile phải rỗng trước phiên; file save mới nếu có chỉ ở `<profile>\InvestigationV36\r107\prototype.json`. Không bỏ tham số rồi mặc định LocalLow là profile riêng.
4. Không Load profile kỹ thuật, không chép save bốn/năm vật chứng, mapping hoặc TrialRecord, không dùng fixture/API để mở khóa. Trước tương tác phải là màn mở đầu, chưa ghi quan sát. Có tiến trình sẵn: dừng, ghi phiên chuẩn bị không hợp lệ, chọn **profile mới**, giữ profile cũ để tra, không xóa.
5. Profile local đã chuẩn bị: **`C:\ConRoi_CheckoutTest\Test\PlaytestProfiles\Newcomer_20261109_9d509a9f`**. Đây chỉ là thư mục rỗng, chưa mở Editor/chạy người chơi. Máy khác hoặc phiên sau phải tạo profile mới; không bắt dùng cùng đường dẫn.

## 4. Mục tiêu quan sát — không giao đáp án

| Chặng tự nhiên nếu người chơi đi tới | Điều cần quan sát |
| --- | --- |
| Bắt đầu / định hướng | Hiểu việc trước mắt, biết đi/nhìn, tự tìm đường? |
| Khám / ghi | Phân biệt đã xem với đã ghi, tìm được nút ghi, hiểu mục tiêu sau ghi? |
| Ghi chép / điện thoại | Tự tìm lại thông tin, hiểu mục tiêu tiếp và thao tác xem? |
| Mật mã / đối chiếu | Hiểu thao tác, chữ/hình đọc được, biết giới hạn suy luận? |
| Bàn dựng / rời-quay lại | Hiểu chạy/tạm dừng, rời/quay lại không mất định hướng? |
| Lưu / tải / tiếp tục | Khi chủ động hoặc tới điểm nghỉ tự nhiên, hiểu thông báo và tiếp tục được? Không cố làm hỏng file trong phiên người mới. |
| Kết luận nếu tự tới | Nêu căn cứ và điều chưa chắc, không bị người tổ chức dẫn chọn? |

Không ép tới mọi chặng: chưa tới là NOT_RUN, không FAIL. Hỏi trung tính: “Bạn đang muốn làm gì?”, “Điều gì khiến bạn nghĩ vậy?”, “Bạn còn cần kiểm tra gì?”. Không chỉ lời giải. Ghi riêng trợ giúp: thời điểm, nguyên văn câu hỏi/lời hỗ trợ, ai đã thao tác.

## 5. Mẫu ghi nhận có thể copy

```text
Mã phiên:
Vai trò tổ chức: UI/UX & Tích hợp
Ngày/giờ, thời gian thực chơi:
Người thử (mã ẩn danh), đã biết game/lời giải trước chưa:
Baseline: c93cb4f1e47acd14fb4dc9502b8a0b77c2d2a7f3
Commit thực chạy:
Unity / scene / Game resolution / OS:
Profile local riêng (không commit save):
Profile rỗng trước phiên: YES / NO / UNKNOWN
Compile/import trước phiên:
Input: người thật / hỗ trợ (ghi rõ)

Thời điểm | Ý định người chơi | Thao tác thật | Kết quả thấy trực tiếp
          | Lời nói nguyên văn | Trợ giúp đã đưa | Bằng chứng local

Các chặng đã tới / chưa tới:
Điểm mắc và thời gian mắc:
UI khó đọc / che khuất (màn cụ thể):
P-close bất thường nếu có: page trước/sau, ô focus, click/phím, pause/Load,
foreground trước/sau, snapshot/video; không suy nguyên nhân:
Mất focus / người khác thao tác / gián đoạn (thời điểm):
Cách khôi phục không mất dữ liệu:
Lỗi có bước tái hiện / chỉ quan sát một lần:
Điều thấy trực tiếp / suy đoán của người tổ chức (tách riêng):
Mức tác động: chặn chơi-mất dữ liệu / hiểu sai / khó thao tác-đọc / góp ý
Chủ cần nhận: Map & Asset / Gameplay & Dữ liệu / UI/UX & Tích hợp
Giới hạn / NOT_RUN:
Kết quả phiên: NOT_RUN / INTERRUPTED / COMPLETED_WITH_OBSERVATIONS
```

Không điền kết quả thay người thật. Xin phép trước ghi hình/âm thanh; mặc định ghi chú với mã ẩn danh. Capture/save/log thô để ngoài Git (`Test/Output`, profile riêng hoặc kho thống nhất); chỉ báo cáo nhỏ đã rà thông tin cá nhân vào Git. Khi mất focus/người khác thao tác: ghi INTERRUPTED cho phần ảnh hưởng, không coi là lỗi gameplay hoặc PASS.

## 6. Kết thúc và bàn giao

Dừng Play sau phiên, kiểm scene dirty và git diff, không lưu state runtime vào scene hoặc discard/overwrite thay đổi chưa rõ nguồn. Giữ profile/bằng chứng, không dùng lại làm phiên sạch. Tổng hợp riêng kết quả, chặng chưa chạy và lỗi cần bàn giao; gửi lỗi runtime cho Gameplay & Dữ liệu, không sửa session partial song song.

P-close vẫn **OPEN/chưa tái hiện/nguyên nhân chưa rõ** theo kiểm kỹ thuật gần nhất. Người mới **NOT_RUN** tới khi có phiên thật; kiểm tổng hợp đã đạt không thay kết quả người thật. Probe bổ sung cũ giữ riêng NOT_RUN.
