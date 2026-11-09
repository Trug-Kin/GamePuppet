# PROTOTYPE-FLOW-01 — Hoàn thiện tuyến runtime hiện hành

Gameplay & Dữ liệu, 09/11/2026. **Hoàn thành hai gói runtime có phạm vi; chờ kiểm tích hợp.** Không gọi toàn game, map hoặc ending đầy đủ đã hoàn tất.

## 1. Phạm vi và căn cứ

Nhận baseline kỹ thuật HANDOFF c93cb4f1e và tài liệu baf7335b5 bằng fast-forward từ a374a93a sau kiểm không có commit riêng; không reset. Checklist đủ tương tác/khám → ghi → mục tiêu → mật mã → đối chiếu → bàn dựng → kết luận → save/load nằm trong [tiến độ Gameplay & Dữ liệu](../../../../QuanLy/DANG_LAM_NGUOI_2.md). GDD hiện hành là [v3.6 chốt triển khai](../../../../CON_ROI_GDD_v3.6_CHOT_TRIEN_KHAI.md), mục7.2/7.3/11/12.1/13. Registry ID giữ nguyên, source InvestigationEvidence.cs là dữ liệu thực.

Hai thiếu sót đã có căn cứ và thực hiện:

- [CLIP-DISCLOSURE-01](../CLIP-DISCLOSURE-01/README.md): bản chép clip không tiết lộ trước khi xem đủ hai vùng exposure hiện có; ghi bị chặn có phản hồi đúng.
- [CONCLUSION-CHECKPOINT-01](../CONCLUSION-CHECKPOINT-01/README.md): xác nhận gắn nội dung, ghi checkpoint trước chốt và không báo chốt thành công khi ghi thất bại.

Mật mã tập rút gọn, đối chiếu ghi giả thuyết, sáu diễn biến và mục tiêu prototype đã có; không thiết kế lại. Bảng26 tư thế, đủ thư/khu và chấm ending C/A thuộc lát cắt/giai đoạn sau; không tự triển khai từ thiếu đầu vào nội dung.

## 2. Chuỗi nhận và nguồn thực kiểm

Nhận baseline baf7335b5 và toàn bộ lịch sử trước nó. Nhận **đủ**:

1. `e0544b740fa127897fd6d6c746dc7b15aab68fbd` — nguồn clip.
2. `d7f1c93b7db1c1a878c0763733e6f14c56750cc5` — nguồn checkpoint/kết luận.
3. `71b20f71b9732980053b063c0b8a04f61f86a2c3` — hai công cụ kiểm + meta.
4. `df2a1a83f767d206cf1991225a616e6cbf4ed498` — công cụ kiểm Save thành công trước khi dùng file cho Load; thêm context assertion, không đổi runtime.
5. Commit tài liệu chứa báo cáo này và hợp đồng; lấy tip nhánh thực tế, không cherry-pick riêng báo cáo.

**Nguồn chạy API cuối: df2a1a83; nguồn chạy controller/UI: d7f1c93b.** Git blob của năm file runtime/meta thay đổi bằng nhau giữa hai commit, ghi trong [verification.json](verification.json). Các commit công cụ/tài liệu không được mô tả là chạy lại UI sản phẩm. Không sửa scene Gameplay lưu trên đĩa, scene tích hợp, asset/GDD/schema/ID hoặc writer/validator cũ.

## 3. Bằng chứng và phân biệt cách kiểm

Unity6000.6.2f1, scene `Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity`, Library riêng hiện có. Công cụ tạo môi trường proxy lúc Play rồi hủy, không lưu scene; profile mới tách biệt bằng tham số thật `--investigation-save-dir`.

| Lượt | Kết quả | Ý nghĩa |
|---|---:|---|
| ClipBaseline | 5/8 | API/source consumer: lộ full transcript ở ba trạng thái chưa đủ exposure; không phải UI baseline |
| ClipFixed | 43/43 | Bốn tổ hợp cờ, không cấp quan sát, ghi/lặp/quay lại/exposure và roundtrip |
| ConclusionBaseline | 2/6 | API tái hiện handler cũ: báo chốt giả khi lỗi ghi, không checkpoint, xác nhận cũ; không phải click UI baseline |
| ConclusionFixed | 24/25 | Một lần ghi sau resume không hoàn tất; giữ FAIL lịch sử |
| ConclusionRecheck / ConclusionIoTrace | 25/25 mỗi lượt | Cùng runtime; khôi phục lỗi, chữ ký, pause, checkpoint và tiếp tục |
| FinalAffectedApi | 80/86 | Save không hoàn tất nhưng công cụ tiếp tục Load file cũ; downstream assert sai. Không đổi thành PASS |
| FinalIoDiagnosis | 85/86 | Một lần chốt không ghi được; không có exception chẩn đoán thu được |
| AllIoCatchTrace | 86/86 | Có log catch tạm để chẩn đoán; **không phải nguồn cuối**, đã phục hồi bytes trước khi kiểm cuối |
| **FinalCommittedApi** | **86/86** | Đúng df2a1a83, không log tạm: clip43 + kết luận25 + tuyếnAPI18 |
| **ControllerRouteTargeted** | **37/37** | Đúng d7f1c93b; IMGUI event gửi vào GameView và bàn phím ảo Input System |

API/fixture tuyến18: E qua API tương tác → ghi/lặp → mục tiêu → Hà → mật mã trống/sai/đủ → ghi bản đọc → đối chiếu sai/hợp lệ → một diễn biến/record → chưa đủ chứng minh → chốt → Load → checkpoint lỗi/restore tường minh → tiếp tục lưu/tải. Có gọi exposure/Assign/AdvanceTrial bằng API, không phải tự chơi tự nhiên.

Controller/UI37: bắt đầu intro không skip/không chứng cứ → di chuyển thật bằng controller → E dấu sàn, ghi và P → phone → clip dừng sớm/bản chép bị chặn → phát đủ/xem bản chép/ghi lặp → E Hà và cơ cấu → mở bàn dựng/rời/quay lại → F5, di chuyển, F6 → trang kết luận/xác nhận/nút chốt/checkpoint → F6 → P/Esc/pause. Không gọi Acquire/Observe/Save/Load/OpenPage hoặc dịch chuyển live bằng API trong route; dữ liệu đến từ thao tác UI/controller trên các proxy tạo bởi fixture môi trường. Phần giải mật mã/ghi quan hệ của tuyến này chỉ kiểm API, chưa thao tác hết bằng UI.

Không gửi input Windows toàn desktop, không nhận được xác nhận desktop độc quyền trong đợt này. Event IMGUI/virtual keyboard giới hạn Editor batch, không giành input người dùng. Render1600×1099, **không phải1080p**. Proxy không có camera/media clip thật: kiểm guard/timeline exposure, chưa chứng minh người chơi nhìn rõ nội dung clip sản phẩm. Không gọi là playtest người mới.

Hai lượt controller đầu (ControllerRoute10 assertion/2 lỗi, ControllerRouteDpi9/2) dừng ở precondition do tọa độ/DPI và chọn nhầm trang. Đã sửa ánh xạ event theo pixelsPerPoint và tìm navigation đúng, giữ lịch sử; không suy lỗi runtime từ click chưa tới nút. UiProbe chỉ2 kiểm intro/F5, không thay cho tuyến37.

Các lần IO không ổn định trước đó chưa xác định process giữ file hoặc nguyên nhân. Lượt log tạm chỉ thấy ba exception đúng các lỗi khóa đích/tạm/khóa checkpoint được công cụ chủ động tạo; không giải thích được lần IO ngoài dự kiến. **Không tự quy cho antivirus, không retry che kết quả hoặc đổi writer đã bàn giao.** FinalCommittedApi và controller thành công, các lỗi ghi được trả thất bại và giữ bản tốt theo acceptance; nguy cơ IO môi trường vẫn ghi nhận để theo dõi nếu tái hiện. Không chứng minh chịu mất điện/crash.

## 4. Chạy lại đúng phần khi cần

Từ clone có Git/LFS đủ, dùng đúng Unity, đóng Editor của chính working directory trước khi chạy. Hai thư mục output/profile phải riêng; profile **chưa tồn tại**. Đường dẫn Editor thay theo máy; không trỏ save gốc.

```powershell
$unity = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe'
$project = (Get-Location).Path
& $unity -batchmode -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.InvestigationFlowChecks.RunBatch --flow-suite all --flow-source (git rev-parse HEAD) --flow-output "$project/Test/Output/FlowApiNew" --investigation-save-dir "$project/Test/PlaytestProfiles/FlowApiNew" -logFile "$project/Test/Output/FlowApiNew.log"
& $unity -batchmode -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.InvestigationControllerRouteChecks.RunBatch --flow-source (git rev-parse HEAD) --flow-ui-output "$project/Test/Output/FlowUiNew" --investigation-save-dir "$project/Test/PlaytestProfiles/FlowUiNew" -logFile "$project/Test/Output/FlowUiNew.log"
```

Không chạy hai lệnh đồng thời. Chờ Editor đầu thoát hẳn trước lệnh thứ hai. Có thể dùng `--flow-suite clip` hoặc `conclusion` để thu hẹp. Chỉ chạy khi thay đổi/căn cứ mới cần kiểm; không yêu cầu chạy lại bộ save/IO/trial cũ.

## 5. Bàn giao cho UI/UX & Tích hợp

Không cần wiring mới cho bản chép và nút chốt hiện có. Review toàn chuỗi, nhận an toàn trên nhánh tích hợp đã được nhóm xác nhận; không tự đưa main hoặc ép ba nhánh giống nhau. [Hợp đồng API/state](../../../../QuanLy/HOP_DONG_MODULE.md), mục hai task cuối.

Kiểm lại trong scene chung/profile sạch riêng:

1. Trước/chỉ một vùng exposure: inspect/compare clip không lộ full transcript, ghi không báo thành công; sau xem đủ giữ bản chép cũ, không tự ghi/lặp quan sát.
2. Khám→ghi→mục tiêu→Hà→mật mã→đối chiếu→bàn dựng→rời/quay lại→lưu/tải→kết luận. Không suy tuyến UI đầy đủ PASS từ API cấp exposure.
3. Nút xác nhận trung tính, sửa nhận định/ground/choice/Load/pause không giữ xác nhận cũ. Chọn “Chưa đủ chứng minh” vẫn ghi được, không tự chấm ending.
4. Chốt tạo checkpoint chưa finalized trước bản chính finalized; thử lỗi chỉ profile riêng, không báo thành công sai; khắc phục rồi chốt/load/tiếp tục.
5. Restore trước chốt: API đã có/kiểm, **đường truy cập người chơi chưa được thiết kế/giao**. Không yêu cầu thêm nút mới. Nếu đưa nhu cầu thử lại trước chốt của GDD8 vào tuyến người chơi, UI/UX & Tích hợp chốt cách truy cập/xác nhận thay tiến trình rồi giao chủ partial nối `LoadConclusionCheckpoint()`. F6 không tự fallback. Checkpoint chứa draft lần thử gần nhất, không lịch sử nhiều bản.

**Riêng:** scene chung trên nguồn mới NOT_RUN; UI1080p NOT_RUN; người mới NOT_RUN; P-close OPEN/chưa tái hiện/nguyên nhân chưa xác định. Kiểm P ở route37 không đóng mục lịch sử. Probe cũ chưa chạy giữ NOT_RUN. Không đổi baseline nhóm trong gói này.

## 6. Bảo toàn và dữ liệu local

Không đổi4.183 legacy chưa xác minh, archive/save gốc hoặc worktree UI. Hai thư mục untracked `.vscode/` và `Test/KetQua/W_route_Unity_v30_20261016/` giữ nguyên. Snapshot/status/refs/file bytes, profile, screenshots/log thô ở local `Test/Output/GameplayScope20261109` và `Test/PlaytestProfiles`, không đưa vào commit. Năm file Unity đã chụp sau kiểm, đối chiếu nội dung chỉ đổi xuống dòng và phục hồi đúng bytes trước kiểm, không lưu scene runtime. `verification.json` chỉ giữ assertion, SHA bằng chứng và blob nguồn; không chứa save/log thô. Kết quả trước sửa/FAIL/gián đoạn giữ riêng, không ghi đè để hợp thức PASS.


## 7. Tiếp tục đóng checklist sau15d15da2

Đã đối chiếu từng hàng GDD/code, không mở chức năng ngoài checklist. [Bảng hiện hành trong tiến độ](../../../../QuanLy/DANG_LAM_NGUOI_2.md), mục “Checklist đóng phạm vi chức năng”, ghi riêng runtime đã có/đã kiểm, phụ thuộc và ngoài giai đoạn; mỗi phần còn lại có vai trò nhận và điều kiện đạt. Các báo cáo/chưa kiểm ở mục3 giữ nguyên như lịch sử, kết quả dưới đây bổ sung chứ không viết đè.

**Không phát hiện thiếu logic mới có yêu cầu rõ trong prototype sau e0544b74/d7f1c93b. Không sửa runtime trong lượt closure này.** Đã làm tiếp phần acceptance chưa đủ bằng chứng: sửa kết luận qua UI thực và chốt dưới lỗi IO qua UI, thay vì dừng sau push15d15da2.

Chuỗi công cụ bổ sung phải nhận cùng nguồn cũ:

- `a53a85123a101f4674d21b37bdc3f538a437edbc`: suite closure cho mutation, toàn state/bytes khi lỗi ghi và sáu chuỗi resume; UI kiểm sửa nội dung/lỗi ghi.
- `45e5a2b436c51201fca03f7133a9aeeb875445f1`: dispose coroutine khi kiểm gián đoạn để finally nhả file lock và giữ thư mục lỗi, không để công cụ giữ handle tới lúc process thoát.
- `c8fc4552277d28b27a78a9264a1290216309fd8e`: tìm điều khiển bằng thay đổi quan sát được trong UI thay tọa độ một điểm. Không gán data hoặc gọi API thay thao tác UI.
- Commit báo cáo chứa mục này; không tạo vòng commit ghi SHA của chính báo cáo.

### Kết quả mới

| Lượt / nguồn thực chạy | Kết quả | Phạm vi |
|---|---:|---|
| ClosureApi /45e5a2b4 | **36/36** | API Play Mode: choice/grounds/record khác hủy xác nhận; ba lỗi tạm/đích/checkpoint giữ full data, page/pause, xác nhận, pose/gate, trial/clip, component enabled và bytes chính; Load bản tốt, khắc phục và chốt; sáu lượt resume bình thường có kiểm kết quả từng lần |
| ClosureController /45e5a2b4 | **29 đạt/31 dòng,2 lỗi** | Dừng vì click một điểm chưa làm thay choice; chưa tới kiểm hủy xác nhận/lỗi ghi. Là precondition công cụ chưa đạt, không kết luận lỗi gameplay hoặc PASS phần chưa chạy |
| ClosureControllerTargeted /c8fc4552 | **55/55** | Tuyến Gameplay proxy + sửa choice/grounds/claim bằng event GUI; đổi grounds về cũ không khôi phục xác nhận; lỗi tạo checkpoint và ghi chính qua nút chốt giữ state/bytes, báo từ chối; gỡ lỗi/chốt/F6 giữ bản mới; P/Esc/pause tiếp tục |

[Assertion + SHA bằng chứng và tree nguồn](closure-verification.json). Toàn thư mục source runtime r107 có tree hash bằng nhau giữa d7f1c93b,15d15da2,45e5a2b4,c8fc4552; không dùng commit công cụ để ngụ ý runtime mới. API36 và UI55 là tập acceptance bổ sung, có kiểm lặp lại tuyến chịu ảnh hưởng; không cộng với86/37 thành số ca độc lập hoặc coi đã chạy lại toàn bộ save/trial.

UI là **GameView IMGUI events + virtual keyboard của Unity Input System**. Không input Windows, không cấp sẵn chứng cứ trong route hoặc gọi API thay click/di chuyển; môi trường proxy sinh trong scene Gameplay_ThuNghiem, profile mới, render1600×1099. Không có footage/camera clip thật; chưa thao tác giải hết mật mã/ghi quan hệ bằng UI, các phần đó vẫn API. Không có kiểm scene chung/1080p/người mới trên nguồn mới. File lock/directory-block là lỗi được mô phỏng an toàn trong profile thử, finally khôi phục; không chứng minh mất điện/crash.

Có thể chạy đúng phần bổ sung bằng các lệnh mục4: API thay `--flow-suite all` bằng `--flow-suite closure`; controller thêm `--flow-ui-closure yes`. Đổi cả output và profile sang thư mục mới, không chạy hai Editor cùng working directory. Không yêu cầu chạy lại bộ86 hoặc các bộ save/IO/trial cũ.

## 8. IO-OPEN-01 — rủi ro ghi bình thường chưa giải thích

**OPEN, chủ nhận Gameplay & Dữ liệu. Không đóng vì lượt cuối hoặc sáu lượt resume đạt.** Đây là rủi ro trong acceptance ghi/chốt của gói hiện tại, không phải nhiệm vụ audit toàn hệ save mới.

### Bằng chứng đã có và ảnh hưởng

1. `ConclusionFixed` trên d7f1c93b và `FinalIoDiagnosis` trên71b20f71 đều trả false/Message “Không chốt…” tại `resume_can_confirm_and_commit`; state ghi trong assertion có Page=conclusions, Paused=false, confirmed=true, finalized=false. Không có exception gốc/operation path được thu ở hai lần đó. **Có ảnh hưởng acceptance ghi bình thường:** thao tác chốt có lúc không hoàn tất. Không có bằng chứng báo chốt thành công giả ở các lượt này.
2. `FinalAffectedApi` trên71b20f71: Save không hoàn tất nhưng fixture tiếp tục Load; clip roundtrip tải bản tốt cũ và chuỗi kết luận đọc lại bản cũ, làm sáu assertion downstream thất bại. Công cụ df2a1a83 đã buộc kiểm kết quả Save trước Load. Sửa công cụ chỉ ngăn kết luận roundtrip sai, **không sửa hay giải thích nguyên nhân IO**.
3. Chưa thu đủ hash/state ngay thời điểm các lỗi ngoài dự kiến để kết luận mọi bytes/state ở các lượt lịch sử được giữ. Không dùng kết quả lỗi mô phỏng thay cho bằng chứng thiếu đó. Các ca được kiểm có chủ đích (API25/closure36, UI55) chứng minh guard giữ state/bytes khi lỗi mô phỏng; không chứng minh tất cả nguyên nhân IO thật.
4. `AllIoCatchTrace` có log tạm chỉ ghi ba exception tương ứng ba điều kiện lỗi được công cụ chủ động tạo: khóa đích, thư mục chặn tmp, khóa checkpoint. Không tìm ra process hoặc exception của lỗi bình thường; log tạm đã phục hồi khỏi nguồn bàn giao.
5. Phạm vi nguồn: lỗi lộ ở d7f1c93b/71b20f71, **cũng là runtime đang bàn giao**, tree không đổi ởc8fc4552. Writer từ SAVE-WRITE-03 không bị sửa. Chưa đủ bằng chứng quy lỗi cho nguồn mới hay môi trường/antivirus, cũng chưa đủ bằng chứng nói nguồn mới đã hết rủi ro.

Ảnh hưởng cụ thể: lượt Save/chốt có thể bị từ chối, bản hiện tại chưa được cam kết. Nếu sau thông báo ghi thất bại người dùng F6 ngay, Load có thể trở về bản chính cũ và bỏ thay đổi chưa lưu; đây không phải bằng chứng Load âm thầm làm hỏng save. Checkpoint có thể đã cập nhật trước khi ghi chính lỗi theo contract hai file. Không tuyên “không thể mất tiến trình”, giao dịch hai file nguyên tử hoặc chịu được crash.

### Bước tái hiện và điều kiện khép mục

- Dùng Unity6000.6.2f1, scene Gameplay_ThuNghiem, profile mới; chạy riêng `InvestigationFlowChecks.RunBatch --flow-suite conclusion` như mục4 để lặp chuỗi gốc có fault/recovery, sửa claim → đóng/mở trang → pause/resume → xác nhận → chốt. Các assertion lịch sử chỉ vị trí `resume_can_confirm_and_commit`; tái hiện không ổn định.
- `--flow-suite closure` kiểm sáu chuỗi resume và snapshot trước/sau; mỗi thất bại bình thường là một assertion FAIL, không tự retry để biến thành PASS. **Lượt36 hiện tại không tái hiện lỗi**, không đổi trạng thái OPEN.
- Khi lỗi thật tái hiện: giữ profile/file `.tmp`, SHA trước/sau của main/checkpoint, full live snapshot và Message, source SHA/time/process; phân biệt bước ghi checkpoint với ghi chính. Chỉ thu exception/handle trong lượt chẩn đoán có phạm vi và giữ riêng, không lộ qua UI hoặc bỏ guard. Không làm đầy ổ/đổi quyền save người dùng/kill Unity.
- **Gameplay & Dữ liệu** xác định nguyên nhân cụ thể từ lượt tái hiện; nếu lỗi nguồn thì sửa tối thiểu và kiểm chính ca đó cùng giữ state/bytes/recovery. Nếu phụ thuộc file lock/môi trường, cần bằng chứng process/handle và kiểm sau gỡ điều kiện. **UI/UX & Tích hợp** nhận mức ảnh hưởng, kiểm thông báo từ chối không thành công giả và không coi F6 sau Save thất bại là đã lưu bản mới. Không chốt acceptance độ tin cậy ghi bình thường hoặc baseline mới chỉ từ các lượt hiện tại.

## 9. Bàn giao cuối theo chức năng và phụ thuộc

Nhận toàn chuỗi **e0544b74 → d7f1c93b →71b20f71 →df2a1a83 →15d15da2**, sau đó ba commit công cụ closure ở mục7 và báo cáo. Tiền đề baf7335b5/c93cb4f1e đã có trên nhánh; không cherry-pick riêng commit báo cáo. Chưa merge/chốt baseline nhóm.

| Bàn giao | Bằng chứng Gameplay hiện có | Người nhận / acceptance còn lại |
|---|---|---|
| Clip chưa xem đủ không lộ bản chép | API43, route37 và route55: inspect trung tính trước, có bản chép sau; compare dùng cùng accessor kiểm API | **UI/UX & Tích hợp:** inspect/compare với footage thật, ba trạng thái chưa đủ và đủ hai vùng; **Map & Asset** phối hợp media nếu sai binding/hình |
| Ghi bị chặn không báo thành công | RecordVisibleObservations trả Message từ chối ở API; UI chưa đủ không đưa nút Ghi | **UI/UX & Tích hợp:** phản hồi/nút đúng ngữ cảnh scene chung, không buộc test click nút chưa tồn tại |
| Sửa kết luận hủy xác nhận | API: cả record khác/choice/grounds; UI55: choice/claim/ground, toggle ground về cũ vẫn chưa xác nhận | **UI/UX & Tích hợp:** kiểm cùng các trường trong scene chung/1080p, sau Load/pause và quay lại; không sửa partial song song |
| Checkpoint/ghi chính lỗi không chốt giả | API36 giữ full live/bytes; UI55 từ nút chốt giữ state/bytes và gỡ lỗi/commit/F6 được | **UI/UX & Tích hợp:** kiểm feedback scene chung, label không che; IO-OPEN-01 vẫn do **Gameplay & Dữ liệu** theo dõi |
| Khôi phục trước chốt | API `LoadConclusionCheckpoint()` đã kiểm; schema1/validator cũ, không tự fallback | [Contract, cách gọi, điều kiện, tác dụng và lỗi](../../../../QuanLy/HOP_DONG_MODULE.md), mục “Checkpoint trước chốt — contract bàn giao đường truy cập”. **UI/UX & Tích hợp** chốt đường truy cập nếu đưa nhu cầu thử lại trước chốt vào luồng người chơi; không tự yêu cầu thêm nút. **Gameplay & Dữ liệu** nhận nối partial khi thiết kế rõ |
| Toàn tuyến gameplay hiện hành | API18 xuyên suốt; controller/UI55 tuyến đại diện trên proxy, mật mã/quan hệ chỉ API | **UI/UX & Tích hợp:** scene chung từ nguồn nhận thực tế, giải mật mã/ghi quan hệ qua UI,1080p và tổ chức người mới; **Map & Asset:** collider/góc nhìn/hình học/media thật. Không suy PASS từ proxy |

**Kết thúc phần triển khai runtime rõ trong checklist:** không còn logic được giao rõ chưa làm sau hai nguồn trên. Chưa gọi toàn tuyến người chơi hoàn tất: IO-OPEN-01, restore chưa có đường truy cập được thiết kế, scene chung/media/mật mã–đối chiếu bằng UI/1080p/người mới là phần chưa khép với owner và acceptance đã ghi. P-close vẫn OPEN/chưa tái hiện/nguyên nhân chưa rõ, probe cũ NOT_RUN. Ending/khu/full26 ngoài giai đoạn; không chuyển chúng thành việc đang chặn prototype.


## 10. IO-OPEN-01 — kết luận phiên chẩn đoán hữu hạn sau777a59de

**OPEN / CHƯA TÁI HIỆN LỖI NGOÀI DỰ KIẾN / CHƯA XÁC ĐỊNH NGUYÊN NHÂN.** Chủ nhận vẫn Gameplay & Dữ liệu. Phiên này chỉ xử lý IO-OPEN-01, không mở chức năng/audit khác. Không sửa runtime hoặc đóng acceptance độ tin cậy ghi bình thường theo một lượt không gặp lỗi.

### 10.1. Nhận và phân loại bằng chứng

HEAD nhận thực777a59de, đúng work/runtime-contracts-baseline, tracked sạch, hai untracked cũ giữ nguyên. Đọc lại mục8, writer và luồng chốt; nguồn thực chạy lỗi cũ là d7f1c93b/71b20f71, cùng runtime r107 với nguồn hiện tại. Hồ sơ cũ nằm dưới `C:/ConRoi_Nguoi2/Test/PlaytestProfiles/<profile>/InvestigationV36/r107/`:

| Profile cũ | Hiện tượng | Kết luận có căn cứ |
|---|---|---|
| ConclusionFixed20261109 | Resume chốt trả false,24/25 | Lỗi thao tác bình thường có thật trong báo cáo, chưa có exception/operation gốc hoặc hash tại thời điểm lỗi |
| FinalAffectedApi20261109 | Save chưa hoàn tất rồi fixture vẫn Load bản cũ,80/86 | **Lỗi fixture đã xác nhận** ở tiền điều kiện roundtrip; df2a1a83 sửa kiểm Save trước Load. Điều này không giải thích vì sao thao tác ghi chưa hoàn tất |
| FinalIoDiagnosis20261109 | Resume chốt trả false/Message chung,85/86 | Có cùng rủi ro trên nguồn bàn giao; không có exception gốc |

Đã đọc/hash/parse **bảy file thử còn giữ**, gồm `.tmp` của FinalAffectedApi; cả bảy hiện là JSON parse được. `.tmp` còn chín vật chứng trong khi bản chính profile FinalAffectedApi còn một vật chứng, phù hợp việc Load lại bản cũ sau lần ghi chưa hoàn tất. Đây là file **sau toàn phiên kiểm**, không snapshot ngay khi lỗi xảy ra; không suy rằng bytes/state ở thời điểm lỗi đã được chứng minh nguyên vẹn. Không sửa hoặc đưa các file save đó vào Git. Hash/size và phạm vi quan sát ghi trong [io-diagnosis.json](io-diagnosis.json).

**Điểm chưa được chứng minh:** `TryFinalizeConclusions` bắt chung Exception quanh clone/capture, hai lần ghi và áp dụng live; Message “Không chốt…Không ghi được…” không đủ xác định exception thật là IOException hoặc vị trí File.Replace. Không được quy lỗi lịch sử cho antivirus/file lock hay kết luận lỗi runtime cụ thể chỉ từ câu thông báo. Lượt API cũ kết thúc có report, lỗi không phải lượt click sai/DPI chưa tới nút; chưa có bằng chứng quy chúng cho mất focus/desktop gián đoạn. Ảnh hưởng của proxy/Editor hoặc môi trường filesystem vẫn chưa được loại trừ hoặc xác nhận.

### 10.2. Điều kiện đã thử — đúng một phiên hữu hạn

Công cụ mới **7d4a7868dc38e4caacbde061b11f580b94d38f4f**, `Assets/ConRoi/Modules/Gameplay/Editor/IoOpenChecks.cs` +meta, Unity6000.6.2f1, source tree runtime nguyên d7f1c93b. Scene Gameplay_ThuNghiem được mở, dựng proxy lúc Play, tắt movement/look trong fixture API; không input desktop/UI/controller, không scene tích hợp. Windows11/NTFS ổC, trống13.709.086.720 bytes trước /13.703.643.136 bytes sau phiên; không làm đầy ổ.

Profile mới `C:/ConRoi_Nguoi2/Test/PlaytestProfiles/IoOpenFinite20261109/`, ba thư mục con tách biệt:

1. **no-injection:** seed save rồi sáu lượt draft → sửa claim → rời/quay lại → pause/resume → xác nhận/chốt → Load; không tạo file lock hoặc lỗi chủ động.
2. **mixed-original-sequence:** thứ tự lỗi/recovery của ca cũ: khóa độc quyền file chính → Load bản tốt/explicit commit; thư mục chặn checkpoint.tmp → gỡ/explicit commit; khóa checkpoint → gỡ/explicit commit; sửa claim/rời/quay lại/pause/resume/chốt. Mỗi handle nằm trong using, thư mục lỗi chuyển sang output trong finally, không xóa. Các lỗi đều có nhãn injection riêng.
3. **direct-writer:** gọi **đúng writer production hai tham số bằng reflection**, không thay delegate runtime hoặc cài fault hook. Seed +tám cặp ghi checkpoint/chính; riêng một lần khóa chính để hiệu chuẩn thu exception và một ghi phục hồi tường minh. Đây là kiểm writer trực tiếp, không phải UI/Load session hoặc chốt thành công của người chơi. Session.Message ở phase này không phải kết quả writer, không dùng nó để đánh giá.

Mỗi thao tác lưu hash main/checkpoint/cả hai `.tmp`, live snapshot trước/sau và kết quả, rồi ghi report incremental. Writer trực tiếp thu type/HResult/stack nếu có exception; session chỉ có Message chung theo contract hiện tại. Không tự retry thao tác lỗi bình thường để biến thành PASS, không bỏ kết quả thất bại. Chỉ một phiên, không chạy lại bộ86/API/IO/trial/UI cũ.

### 10.3. Kết quả và mức chứng minh

- **36 thao tác ghi:**32 thành công; ba lượt TryFinalizeConclusions trả false do lỗi chủ động và một direct-writer ném exception do khóa chủ động. **0 lỗi ngoài dự kiến,0 vi phạm guard được kiểm, không fixture exception.** Đây không phải36 ca chứng minh đã hết lỗi lịch sử.
- Bốn lượt lỗi mô phỏng giữ main hash/live snapshot, phục hồi tường minh ghi được.11 lượt Load qua Session thành công, không ghi lại main/checkpoint/tmp. Checkpoint có thể thay đổi khi ghi chính lỗi theo contract hai file, không coi đó là rollback toàn giao dịch.
- Calibration direct-writer thu **System.IO.IOException /0x80070020 tại File.Replace** đúng lúc công cụ giữ FileShare.None của file đích. Handle được nhả rồi ghi lại thành công. Đây chỉ chứng minh chẩn đoán và lỗi khóa **chủ động của fixture** hoạt động; **không phải nguyên nhân của lỗi lịch sử**.
- Không tái hiện tự nhiên sau fault/recovery hoặc trong sáu lượt resume không fault. Không kéo dài vòng lặp vô hạn. Không sửa runtime theo giả thuyết, không bảo đảm chịu được mất điện/crash.

Raw state/stack/report/log và profile giữ local ở `Test/Output/IoOpen20261109/FiniteProbe` và profile mới nêu trên, không commit. JSON bàn giao chỉ giữ hash, kết quả, loại lỗi/HResult và các trường đã lược state/stack. Năm file Unity đã snapshot trước/sau, đối chiếu nội dung chỉ đổi xuống dòng rồi phục hồi bytes trước kiểm; scene không được lưu, untracked cũ/save gốc/archive/legacy không đổi.

### 10.4. Phát hiện, giảm rủi ro và điều kiện nhận tiếp

**Ảnh hưởng còn OPEN:** thao tác ghi/chốt bình thường có thể không hoàn tất; chưa biết lỗi lịch sử xảy ra trước/sau bước nào, thiếu hash tại lỗi nên không khẳng định mọi bản cũ đều được giữ. Lượt mô phỏng xác nhận cơ chế giữ bản tốt/feedback lỗi trong các điều kiện đã chèn, không thay cho bằng chứng lịch sử.

- Khi F5 hoặc chốt báo lỗi: **không coi bản mới đã lưu và không F6 ngay để “xác minh”**; Load có thể hợp lệ nhưng quay về bản cũ, làm mất thay đổi chưa lưu. Giữ phiên/draft, profile và `.tmp`; chỉ thử ghi lại tường minh sau xử lý điều kiện khóa/đường dẫn đã xác minh. Không tự xóa/rename `.tmp`, tự repair/fallback hoặc ghi đè save gốc để khắc phục.
- Phát hiện kỹ thuật: kiểm bool TryFinalizeConclusions và Message; với Save(void), kiểm thông báo hoàn tất trước Load như df2a1a83. Khi kiểm thử, phải chụp SHA/state trước thao tác và sau khi lỗi trước mọi Load/recovery. Direct-writer có diagnostic nhưng không thay cho phép thử session.
- **Điều kiện nhận/tái mở điều tra cụ thể:** một lỗi ghi/chốt **không có nhãn injection**, dù chỉ một lần, kèm source/time, profile/main/checkpoint/tmp và SHA trước/sau, snapshot live, Message/exception nếu thu được, hoặc hiện tượng báo thành công mà file không khớp. IO-OPEN-01 hiện vẫn OPEN; “mở lại” ở đây là bắt đầu lượt chẩn đoán mới khi có bằng chứng mới, không đổi OPEN thành CLOSED rồi mở lại.
- **Gameplay & Dữ liệu** nhận bằng chứng đó, xác định bước clone/capture/ghi checkpoint/ghi chính/apply và exception gốc. Chỉ nếu xác nhận lỗi nguồn mới sửa tối thiểu, kiểm chính ca đó và recovery; nếu nguyên nhân file handle/môi trường, cần xác minh process/handle và gỡ điều kiện rồi kiểm. Không tự đoán và không chạy full suite để tìm may rủi.
- **UI/UX & Tích hợp** tiếp tục nhận trạng thái ghi/tải và kiểm feedback scene chung; không diễn giải một lần Load bản cũ thành bản mới đã lưu. Không giao thêm nút hoặc sửa UI trong gói IO này. Baseline/nghiệm thu độ tin cậy ghi bình thường chưa được khép bởi phiên này.

Cách dùng công cụ khi có căn cứ mới: từ clone đúng Unity và đã đóng Editor của working directory, dùng profile/output **chưa tồn tại**:

```powershell
$project = (Get-Location).Path
$unity = 'D:\data_unity\6000.6.2f1\Editor\Unity.exe' # thay theo máy
& $unity -batchmode -projectPath $project -executeMethod ConRoi.Gameplay.EditorChecks.IoOpenChecks.RunBatch --io-source (git rev-parse HEAD) --io-output "$project/Test/Output/IoOpenNew" --investigation-save-dir "$project/Test/PlaytestProfiles/IoOpenNew" -logFile "$project/Test/Output/IoOpenNew.log"
```

**Trạng thái nhiệm vụ Gameplay & Dữ liệu:** logic prototype rõ đã triển khai/bàn giao; phần chẩn đoán IO được giao ở lượt này đã hoàn thành có giới hạn, **IO-OPEN-01 chưa đóng nguyên nhân/rủi ro** và vẫn có chủ nhận ở trên. Các phụ thuộc scene chung/media/mật mã–đối chiếu bằng UI/1080p/người mới và đường truy cập restore giữ đúng owner tại mục9; P-close OPEN. Không mở chức năng mới hoặc tuyên toàn tuyến/game đã xong.
