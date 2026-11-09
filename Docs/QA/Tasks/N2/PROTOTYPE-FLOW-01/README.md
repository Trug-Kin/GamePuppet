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
5. Nếu cần nút restore trước chốt: **chưa có UI mới**; thống nhất vị trí và xác nhận thao tác rồi gửi chủ partial Gameplay & Dữ liệu nối `LoadConclusionCheckpoint()`. API đã có/kiểm, F6 không tự fallback. Checkpoint chứa draft lần thử gần nhất, không lịch sử nhiều bản.

**Riêng:** scene chung trên nguồn mới NOT_RUN; UI1080p NOT_RUN; người mới NOT_RUN; P-close OPEN/chưa tái hiện/nguyên nhân chưa xác định. Kiểm P ở route37 không đóng mục lịch sử. Probe cũ chưa chạy giữ NOT_RUN. Không đổi baseline nhóm trong gói này.

## 6. Bảo toàn và dữ liệu local

Không đổi4.183 legacy chưa xác minh, archive/save gốc hoặc worktree UI. Hai thư mục untracked `.vscode/` và `Test/KetQua/W_route_Unity_v30_20261016/` giữ nguyên. Snapshot/status/refs/file bytes, profile, screenshots/log thô ở local `Test/Output/GameplayScope20261109` và `Test/PlaytestProfiles`, không đưa vào commit. Năm file Unity đã chụp sau kiểm, đối chiếu nội dung chỉ đổi xuống dòng và phục hồi đúng bytes trước kiểm, không lưu scene runtime. `verification.json` chỉ giữ assertion, SHA bằng chứng và blob nguồn; không chứa save/log thô. Kết quả trước sửa/FAIL/gián đoạn giữ riêng, không ghi đè để hợp thức PASS.
