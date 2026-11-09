# OBS-PROGRESS-01 — Đồng bộ ghi quan sát dấu sàn và tiến trình

**HOÀN TẤT PHẠM VI, CHỜ REVIEW/TÍCH HỢP — 09/11/2026.** Gameplay & Dữ liệu. Nhánh work/runtime-contracts-baseline, worktree C:/ConRoi_Nguoi2. Nguồn `1bd2ebd7f7143ba549eab889642cf1e191f7333e`; commit thực sự kiểm `45917774b301f59073f8233287575783ebaf0414`, gồm nguồn và công cụ. Chưa push/merge. Phần acceptance phía dưới được ghi trước baseline/sửa, giữ để đối chiếu.

## Cải thiện và nguyên nhân

API Observe từng ghi quan sát slide/shoeprint thành công nhưng để firstFloorRecorded=false; consumer đọc CurrentObjective vẫn yêu cầu kiểm dấu sàn. Ngược lại, batch wrapper RecordVisibleObservations tự đặt cờ kể cả khi Observe không ghi vì chưa Acquire. Nút UI bình thường vốn dùng batch với vật đã thu nên không nhận trường hợp đó là lỗi người chơi đã gặp qua UI.

Sửa tại biên ghi hợp lệ: sau guard ID đã thu, index hợp lệ và guard clip, Observe ghi observation duy nhất rồi đặt firstFloorRecorded cho hai ID dấu sàn hiện hành. Batch wrapper từ chối vật chưa thu và dùng chuyển trạng thái từ Observe để giữ nhắc P lần đầu. Đã kiểm cả quan sát index0 và index1; không buộc ghi đủ mọi vật hoặc mở toàn bộ lời giải.

- `InvestigationSession.cs`: một dòng đồng bộ cờ sau ghi hợp lệ.
- `InvestigationOnboarding.cs`: chỉ phần runtime RecordVisibleObservations; bỏ setter cờ vô điều kiện, dùng trạng thái trước/sau để giữ Message hiện có. Không sửa CurrentObjective, BeginIntroduction, marker, view hoặc text UI.
- Công cụ mới `Assets/ConRoi/Modules/Gameplay/Editor/ObservationProgressChecks.cs` + meta; không lưu scene.
- Contracts/tiến độ và kết quả nhỏ của task. Không đổi GDD/cốt truyện, ID/schema/serialized field/GUID cũ, luật cơ cấu, map/asset, scene tích hợp hoặc save gốc.

## Baseline đã tái hiện

Unity6000.6.2f1, scene Gameplay_ThuNghiem, profile mới; runtime00c17147 và công cụ mới khi còn untracked (được commit sau tại45917774). [baseline-api.json](baseline-api.json): **10 assertions, 3 không đạt** đúng khoảng thiếu:

| Ca | Trạng thái thực baseline |
|---|---|
| Acquire/OpenEvidence slide hoặc shoeprint, chưa ghi | observations=[], firstFloorRecorded=false; đạt đối chứng |
| Observe slide,0 | observations=[0], firstFloorRecorded=false, mục tiêu vẫn yêu cầu kiểm dấu sàn |
| Observe shoeprint,0 | observations=[0], firstFloorRecorded=false, mục tiêu vẫn yêu cầu kiểm dấu sàn |
| Batch slide chưa Acquire | không có EvidenceRecord nhưng firstFloorRecorded=true, nhắc P dù chưa ghi được |

Các thao tác này là **producer/API fixture**, không phải tái hiện lỗi qua nút UI hoặc tuyến người mới. Dữ liệu/flag/objective thực được ghi ở từng assertion.

## Sau sửa: acceptance và kết quả

**35/35 API đạt, 0 lỗi** ([fixed-api.json](fixed-api.json)). CLI source label là HEAD, được cố định tại `45917774b301f59073f8233287575783ebaf0414` xuyên suốt API/UI; manifest ghi SHA thực và hash nguồn, không dùng chữ HEAD làm baseline bàn giao.

| Acceptance | Bằng chứng |
|---|---|
| Khám không đồng nghĩa ghi; hồ sơ mở trước ghi không tính xem ghi chép | Hai ID API; UI slide |
| Ghi hợp lệ cập nhật bước dấu sàn; ghi trùng không nhân record/observation | API direct Observe và batch lặp; UI ghi/lặp |
| Chưa Acquire/index ngoài phạm vi/ID lạ không mở tiến trình; vật khác không hoàn tất dấu sàn | API; clip chưa đủ exposure vẫn bị chặn như trước |
| Ghi lần đầu nhắc P; sau mở dossier mới đến mục tiêu điện thoại | API và UI, không sửa chuỗi mục tiêu |
| Thứ tự slide/shoeprint linh hoạt, quan sát thứ hai không reset xem sổ; skip được giữ | API cả hai thứ tự và skip; không ép ghi đủ hai vật |
| Không cấp phone/decoded/trial/finalized hoặc quan hệ chưa có | API và UI đối chứng; không phải kiểm toàn bộ luật kết luận |
| Lưu/tải khám-chưa-ghi, ghi-chưa-xem-sổ, đã-xem-sổ; tiếp tục ghi/lưu/tải | Ba roundtrip API và UI F5/F6 theo tiến trình; không chạy lại các bộ save cũ |

**14/14 kiểm UI/controller đạt bằng input Windows tổng hợp** ([ui-summary.json](ui-summary.json)). Dùng lại bridge TrialContextControllerChecks chỉ dựng môi trường và snapshot/lifecycle, không cấp evidence/observation hoặc mở trang bằng API. Tuyến: intro sạch → Bắt đầu → đi/nhìn → E khám slide (chưa ghi) → P trước ghi không tính review → trở lại khám → F5 → ghi → F6 phục hồi chưa ghi → ghi/lặp không nhân đôi → R thấy nhắc P → F5 → P → F6 phục hồi đang chờ xem sổ → P lại tới mục tiêu điện thoại → F5 → R/E → F6 giữ bước đã xem sổ → mở lại vật vẫn giữ tiến trình. Modal khóa di chuyển. Đã xem ảnh UI có nhắc P và mục tiêu điện thoại cùng hai quan sát đã ghi.

Môi trường proxy tạm trong scene `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`; tiến trình UI tự tạo qua E/nút. Không save scene. Compile không có errorCS hoặc exception sản phẩm trong log task; không rà toàn project/scene tích hợp. Cảnh báo Unity AI/cloud hoặc compiler legacy không được suy thành lỗi task. Input kiểm PID foreground trước mỗi thao tác; không có lượt gián đoạn được ghi nhận.

## Contract và cách nhận

Nhận đủ **1bd2ebd7 nguồn → 45917774 công cụ/commit đã kiểm → commit báo cáo**, trên nền00c17147. Không chỉ lấy báo cáo. UI/UX & Tích hợp review hai đoạn runtime và contract, nhận bằng quy trình nhánh tích hợp của nhóm. **Không API/event/wiring mới**; consumer vẫn dùng Observe/Observation, firstFloorRecorded, firstNotesReviewed, CurrentObjective.

Một Observe hợp lệ của slide/shoeprint ghi nhận lần ghi dấu sàn đầu tiên. Không reset firstNotesReviewed; Acquire/OpenEvidence/index bị từ chối không tính. API gọi lặp không thêm index trùng. Chỉ setter tại biên Observe được chuẩn hóa; caller không tự đặt flag để giả dữ kiện. Batch UI vẫn ghi tất cả mô tả hiện có và dùng Message cũ. Save/schema1 và Load không đổi; không tự migration/repair save cũ khi load. Với dữ liệu cũ có observation nhưng flagfalse, thao tác Observe hợp lệ tiếp theo cập nhật cờ; không âm thầm rewrite save trên Load.

Phía tích hợp cần review hành vi gọi Observe của producer riêng và kiểm một tuyến ghi→P trên scene chung thuộc quyền mình; không chỉnh scene trong gói này. Không thêm yêu cầu khóa mọi bước sau theo số vật chứng. Những thứ tự lớn hơn mà GDD chưa quy định chi tiết không được áp thêm trong đợt này.

## Chạy lại đúng phạm vi và lưu giữ

Đóng Editor đang ghi worktree trước batch, bảo toàn status/diff, dùng Unity6000.6.2f1 và profile tuyệt đối **chưa tồn tại**. Thay placeholder bằng đường dẫn clone của mình, không dùng save người chơi:

```powershell
& '<UNITY_EDITOR>\Unity.exe' -batchmode -nographics -projectPath '<PROJECT>' -executeMethod ConRoi.Gameplay.EditorChecks.ObservationProgressChecks.RunBatch -logFile '<OUTPUT>\Editor.log' --investigation-save-dir '<PROFILE_MOI>' --observation-output '<OUTPUT>' --observation-source 45917774
```

Baseline `--observation-baseline yes` chỉ dùng với runtimebaseline riêng, không reset worktree đang làm. GUI tái dùng StartGui của TrialContextControllerChecks với --trial-controller-output và profile mới; không có lệnh seed/trang chơi trong bridge.

[evidence-manifest.json](evidence-manifest.json) chỉ mục local raw path/bytes/SHA256 và nguồn. Raw ảnh/log/input/snapshot ở `Test/Output/ObservationProgress01/`; profile riêng `Test/PlaytestProfiles/ObservationProgress01_*`, không đưa save/cache/capture/log thô vào Git. Kết quả API chứa dữ liệu fixture kiểm có chủ đích, không chứa save người dùng. Settings sau Unity được copy riêng rồi so newline-normalized với snapshot trước task trước trả lại đúng bytes ban đầu; không discard semantic diff. Đã stop/quit phiên sở hữu, sceneDirty=false. Hai thư mục untracked cũ giữ nguyên.

## Giới hạn còn lại

- Direct API gap được tái hiện bằng fixture, UI kiểm hồi quy đường nút hiện có; không gọi đó là lỗi UI baseline hoặc tuyến chơi tự nhiên trong xưởng thật. Proxy không kiểm chất lượng hình dấu sàn hoặc đường đi map.
- Game1536×746/desktop1920×1080; UI1080p, người mới và scene tích hợp **NOT_RUN**. UI chỉ slide; shoeprint/skip/invalid/đổi thứ tự chi tiết là API.
- Không chạy lại bốn gói save/trial cũ; chỉ các roundtrip chịu ảnh hưởng tiến trình. Load feedback/P-close vẫn OPEN riêng, không đóng bằng task này. Giữ archive/checkpoint/save gốc và4.183 legacy.
- Chưa push/merge, không build/ZIP. Không mở nhiệm vụ nội dung khác.

---

# Acceptance và căn cứ trước sửa

Gameplay & Dữ liệu nhận triển khai theo yêu cầu09/11/2026. Xuất phát00c17147953237eab8b52ed8347cae9a267965b3 trên work/runtime-contracts-baseline, C:/ConRoi_Nguoi2; tracked sạch, untracked cũ giữ nguyên. Chưa push/merge.

## Căn cứ và phạm vi trước sửa

GDD v3.6 mục6: khám/xem→thử/cập nhật suy luận; mục7.2 tách quan sát, lời kể, giả thuyết và không tiết lộ điều chưa xem; mục11 lưu các trạng thái này riêng. Contracts hiện hành công bố Acquire/Observe/Observation cho producer/consumer. R107 đã có firstFloorRecorded/firstNotesReviewed và mục tiêu đầu; không thêm luật GDD mới hoặc yêu cầu nhặt đủ đồ.

Source Observe thêm observation nhưng không cập nhật firstFloorRecorded; RecordVisibleObservations mới đặt cờ, kể cả khi Observe từ chối ID chưa Acquire. Khoảng thiếu là API producer ghi thật nhưng tiến trình vẫn yêu cầu dấu sàn. Không tuyên bố nút UI hiện hành bị kẹt: nút gọi batch wrapper vốn đã đặt cờ. Chọn đồng bộ tại biên ghi hợp lệ; một observation dấu sàn hợp lệ là lần ghi đầu, không chờ tất cả vật chứng. Không sửa CurrentObjective, UI/render/scene, luật cơ cấu, ID/schema/GUID, clip exposure hoặc Load validation.

## Acceptance ghi trước sửa

1. Baseline trong scene Gameplay/profile mới: Acquire/OpenEvidence slide nhưng chưa Observe giữ observations rỗng và firstFloorRecorded=false. Observe hợp lệ thêm observation nhưng baseline chưa chuyển mục tiêu; ghi trạng thái thực, không gọi API fixture là tuyến chơi tự nhiên.
2. Sau sửa, chỉ Observe được chấp nhận của slide hoặc shoeprint mới ghi nhận firstFloorRecorded; mục tiêu chuyển sang xem ghi chép. Observe thiếu Acquire/index lỗi/ID lạ không đổi tiến trình hoặc mở nội dung. Vật khác không hoàn tất bước dấu sàn.
3. Ghi trùng/khám lại cùng quan sát không nhân đôi record/observation, không reset firstNotesReviewed; thứ tự slide/shoeprint linh hoạt. Chưa mở hồ sơ sau ghi thì vẫn nhắc P; mở trước ghi không được tính xem lại ghi chép. Sau mở hồ sơ đúng lúc mới chuyển mục tiêu điện thoại.
4. Giữ nút Ghi quan sát hiện hành, prompt lần đầu và skip tutorial; batch wrapper chỉ nhận kết quả từ Observe, không tự đặt cờ khi không ghi được. Không thêm API/event hoặc presentation mới.
5. Roundtrip Save/Load các trạng thái khám-chưa-ghi, ghi-chưa-xem-sổ, đã-xem-sổ; tiếp tục ghi/lặp được. Giữ tương thích schema1, không tự migration/rewrite save cũ hoặc cấp observation trên Load. Chỉ kiểm roundtrip chịu ảnh hưởng, không chạy bộ save cũ.
6. API/fixture baseline trước sửa rồi kiểm sau sửa; UI/controller đại diện nếu phiên desktop an toàn. Ghi rõ loại bằng chứng, profile riêng, không save scene. Luồng UI vốn đạt được kiểm giữ hành vi, không dùng fixture cấp dữ kiện để nhận là chơi tự nhiên.

## File và phụ thuộc

Nguồn dự kiến InvestigationSession.cs/Observe và phần runtime RecordVisibleObservations trong InvestigationOnboarding.cs. Không đổi các hàm UI/marker/nội dung. Tool kiểm mới trong Assets/ConRoi/Modules/Gameplay/Editor; báo cáo N2 và HOP_DONG_MODULE. UI/UX & Tích hợp nhận contract/state qua API cũ; không wiring scene chung. Các quyết định GDD khác về thứ tự evidence/clip/các bước kết luận không thuộc gói này.
