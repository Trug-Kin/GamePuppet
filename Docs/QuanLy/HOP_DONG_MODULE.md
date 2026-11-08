# Hợp đồng module hiện hành — baseline fcbb7e19

Chủ file: **người 2**; người 3 review cách dùng UI, người 1 review asset binding. Đây là tài liệu mô tả mã có sẵn, chưa phải interface/assembly mới. Nguồn chuẩn: `Assets/Scripts/InvestigationV36R107/InvestigationSession.cs`, các partial cùng thư mục, `InvestigationEvidence.cs` và GDD v3.6. Không dùng GDD_IMPLEMENTATION_CURRENT v47 làm yêu cầu mới.

| Biên hiện hành | Producer / chủ sửa | Consumer / trách nhiệm |
|---|---|---|
| Asset, collider, rig, material và GUID | Người 1 | Người 3 wire scene; người 2 kiểm contract tương tác, không đổi ID để khớp tên art |
| `Acquire`, `Observe`, `Has`, `Record`, `Observation` | Session / người 2 | UI và interaction đọc trạng thái; acquire không đồng nghĩa đã nhìn thấy mọi quan sát |
| `OpenEvidence`, `OpenPage`, `ClosePage`, `SetPaused`, `Modal` | Session partial / người 2 | UI gửi ý định qua luồng hiện có; đóng trang phải dọn focus/drag và khôi phục input đúng ngữ cảnh, giữ NAV-01 |
| `ReadCipher`, `Assign`, `AddRelation` | Session / người 2 | UI trình bày dữ kiện và suy nghĩ; không tự gán danh tính/hung thủ vì chạy một ca |
| `StartTrial`, `ClearTrialHolds`, `TrialSettled` | Session + TrialTimeline + cơ cấu / người 2 | Người 3 wire trang bàn dựng; sáu ca cùng luật, giữ hồi hữu hạn, hiển thị đúng lực/móc/chèn; seek không cấp quan sát |
| `Save`, `Load`, `SaveDirectoryOverride`, `SavePath` | Session / người 2 | Người 3 chuẩn bị profile thử riêng và kiểm đường dẫn; không tự load profile kỹ thuật |

Save hiện hành schema1, namespace `conroi.investigation.v36.r107`; đường dẫn `<profile>/InvestigationV36/r107/prototype.json`. ID theo mã hiện hành, không đổi serialized field/schema/GUID trong handoff. CLI `--investigation-save-dir` được Awake đọc; đổi CLI cần mở lại Editor của checkout đó. F5/F6 chỉ dùng profile thử đã xác nhận.

Người 1/3 không gọi API seed dữ kiện để chứng minh người chơi tìm thấy chúng. Bằng chứng phải tách UI/input tổng hợp, API/fixture, render minh họa và người mới. Ca đã PASS kỹ thuật xem NavFix20261108; không suy toàn map PASS hoặc người mới PASS.

Nhiệm vụ đầu của người 2 là bổ sung chi tiết type/consumer/acceptance theo source thực tế trước đề xuất thay đổi. Mọi biên còn dùng chung file phải giữ một chủ; chưa tuyên bố ba module độc lập.
