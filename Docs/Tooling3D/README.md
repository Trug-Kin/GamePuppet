# Chuẩn bị công cụ 3D cho CON RỐI — 20/10/2026

Ưu tiên pipeline `bpy` sẵn có. Đã kiểm chứng đọc scene thật → sửa mesh mẫu trong file riêng → render → lưu → mở lại bằng tiến trình Blender khác → xuất FBX → nhập Unity → render ở độ cao mắt người chơi. Không cần cài Blender MCP để đáp ứng công việc hiện tại.

## Môi trường và nguồn đã đọc

| Thành phần | Kết quả |
|---|---|
| Blender | `C:/blender.exe`, 5.2.0 LTS, build `fbe6228777e7` |
| Pipeline | Script `Tools/*.py` chạy bằng Blender background; ví dụ `survey_v48_r21_shore.py`, `repair_v48_bridge_support.py`, `convert_route_materials_v33.py` |
| Node / npm | 24.21.0 / 11.19.0. Dùng `npm.cmd` vì PowerShell chặn npm.ps1; không đổi execution policy |
| Python / RAM | Python hệ thống 3.11.9; RAM 15,21 GiB, khoảng 2,52 GiB khả dụng tại lần đo đầu. Không cài venv |
| GPU | RTX 2050, VRAM 4.096 MiB; driver 592.27 |
| Dung lượng lúc khảo sát | C còn khoảng 21,29 GiB; D còn khoảng 20,12 GiB. PSDrive báo 0 trong sandbox là số sai; đã đo lại bằng `shutil.disk_usage` |
| Unity | 6000.6.2f1, `D:/data_unity/6000.6.2f1/Editor/Unity.exe` |
| Codex/MCP | Không thấy `mcp_servers` trong config Codex hiện có, cấu hình MCP cấp project hoặc addon Blender MCP. Không thay config |

README ba repo, cây tệp và commit đã lưu tại `sources/`. Ghim phiên bản đã khảo sát:

- [Meshy 3D Agent](https://github.com/meshy-dev/meshy-3d-agent): `92ffd747638284f6ccdf419d764e6f14753ec298`, repo release 0.7.0; CLI 0.4.3.
- [Hunyuan3D-2.1](https://github.com/Tencent-Hunyuan/Hunyuan3D-2.1): `82920d643c0dc2f7bfd7255f45f62d386edfe60c`.
- [MCP for Blender](https://github.com/ahujasid/mcp-for-blender): `98b991c8daf91ab9d5451ee6b923d89367f733a5`; package 2.2.1; addon bl_info 1.8, protocol 14. Đã đọc README, addon, server và pyproject. Server dùng socket localhost:9876, hỗ trợ thực thi Python. Chưa cài, chưa tuyên bố kết nối MCP hoạt động. Nếu cần bổ sung sau: ghim phiên bản, sao lưu và thêm riêng entry `blender`, giữ các entry khác; tắt telemetry; kiểm handshake và thao tác scene thật trước khi công nhận.

## Meshy

Đã cài đầy đủ thư mục `.agents/skills/meshy-3d-generation` từ commit trên bằng skill-installer, gồm SKILL.md và bốn tài liệu references. Không cài skill in 3D. Vị trí phù hợp [tài liệu chính thức Codex](https://developers.openai.com/codex/skills); Codex có thể nhận skill mới từ lượt tiếp theo. Đã đọc trực tiếp trong lượt thiết lập.

CLI chạy thực tế qua `npm.cmd exec --yes --package=meshy-cli@0.4.3 -- meshy ...`. `doctor` báo local_ready=true, các origin đúng Meshy production. Kiểm tra ban đầu không có profile hay MESHY_API_KEY. Đã khởi động một phiên đăng nhập device flow; trạng thái cuối xem `meshy-status.md`. Không đọc hoặc sao chép credentials. Đăng nhập thực hiện trên trình duyệt, không gửi token/key vào chat.

Ước tính CLI `make --dry-run`: text → preview 20 + refine 10 = **30 credit**; ảnh → textured model = **30 credit**. File `meshy-image-estimate.json` ghi `requests_made: 0`. Đây là giá ước tính cho các bước mặc định này, không phải báo giá mọi cấu hình Smart Topology/PBR/remesh. Bảng giá tham chiếu: https://docs.meshy.ai/en/api/pricing, bản HTML lưu ngày khảo sát.

Phương án đề xuất cho hoa: **Smart Topology meshy-t2.5, mục tiêu 2.500 tam giác, texture 2K, một GLB: ước tính 15 credit/lượt**, theo bảng giá chính thức đọc 20/10/2026 (image-to-3d meshy-t2/t2.5 với texture 2K). Đây là báo giá từ bảng công bố, không phải dry-run cấu hình tùy chỉnh. CLI help đã xác nhận tham số. Không cần bước remesh trả phí mặc định. Mesh-only cùng model là 5 credit nếu quyết định tự làm vật liệu; không tự chuyển phương án. Bảng giá còn báo model_type lowpoly cũ ngừng 30/10/2026; dùng smart-topology, không dùng legacy lowpoly.

**Ngân sách chi tiêu hiện được phép: 0 credit.** Chưa gửi create, regenerate, remesh tính phí, hoặc mua credit. Chưa quy đổi USD/VND vì chưa xác minh gói và mức giá credit của tài khoản. Sau khi được chốt ngân sách, kiểm lại balance và cấu hình; bất kỳ tạo lại hoặc bước trả phí bổ sung nào cũng phải nằm trong hạn mức đã duyệt. Chi phí thực tế phải lấy từ consumed_credits của task.

## Hunyuan

README yêu cầu 10 GB VRAM cho shape, 21 GB cho texture và 29 GB cho cả hai; môi trường tác giả thử là Python 3.10, PyTorch 2.5.1+cu124, kèm rasterizer và renderer biên dịch. RTX 2050 4 GB không phù hợp pipeline đầy đủ. Cờ low_vram_mode có trong repo nhưng không có chứng cứ bảo đảm 4 GB đáp ứng; không coi đây là cách chạy đã xác minh.

Không tải weights, PyTorch, môi trường Python hoặc CUDA mới. Nhánh local để tùy chọn khi có máy phù hợp. Local có chi phí điện, dung lượng và thời gian GPU; không dùng credit Meshy. Demo/website Tencent hoặc thuê GPU là dịch vụ bên ngoài, có điều khoản, quota và giá riêng chưa xác minh; không mặc định miễn phí và chưa gửi ảnh/tác vụ.

## Luồng asset và phạm vi r21

| Công việc | Luồng chọn |
|---|---|
| Cầu, bờ, nước, tiếp đất, vị trí/kích thước kiến trúc | Đo bằng mét, sửa cục bộ trong Blender trên nguồn hiện hữu; đối chiếu render/collider Unity và chuyển hệ tọa độ sẵn có |
| Ông Bảo tựa tay lan can | Chỉnh rig/pose hoặc chuỗi điều khiển pose hiện hữu; kiểm áo/cẳng tay với lan can, chân với sàn, nghỉ/hội thoại/save-load. Giữ controller và vị trí NPC |
| Hoa, chậu, giỏ, đồ len, đạo cụ | Ưu tiên asset thủ tục/nguồn hiện hữu; Meshy sau ngân sách; Hunyuan là tùy chọn. Luôn qua Blender để sửa và chuẩn hóa |
| Mất mặt khi xuất | Sửa exporter và kiểm nguồn/xuất/Unity; không tạo lại kiến trúc để che lỗi. Giữ kết quả exporter r20 |
| Sương và biên vùng chơi | Tiếp tục phạm vi r21 trong Unity: sương theo vùng; biên theo địa hình/kiến trúc đo được, giữ lối quay lại và khám phá. Sương không thay collider |

Giữ map hiện hữu, tọa độ, controller, gameplay, checkpoint và save. Không tạo toàn map từ ảnh. Scene thử không thêm vào Build Settings. Đợt chuẩn bị không chứng nhận các lỗi r21 đã hoàn tất; tiếp tục từ `Docs/Reviews/journey_v48/r21/CURRENT-REVIEW.md`. Gom kiểm thử sau từng phần hoàn chỉnh, không yêu cầu người dùng chơi từng thao tác.

## Cụm hoa thử

- Đích: một cụm hoa trang trí bồn cầu, chưa nhân rộng. Bối cảnh hiện có: `../Reviews/journey_v48/r21/internal-se-perimeter-clear/se-planter-oblique.png`. Đây là render nội bộ, không phải ảnh hiện trường. Bảy ảnh mới được nhắc trong r21 vẫn thiếu; không lấy ảnh cũ thay thế.
- Ảnh đầu vào Meshy đề xuất: `trial/blender-preview.png`, do chính pipeline thủ tục tạo, không phát sinh chi phí tạo ảnh. Đã xem ảnh. Mẫu cúc kem/vàng này là concept kỹ thuật, chưa xác nhận loài/màu đúng hiện trường; không coi nó là phục dựng cụm hoa đỏ/vàng đang có trong map.
- Kích thước mục tiêu: footprint khoảng 0,34 × 0,30 m, cao 0,23 m; thiết kế tạm, cần đối chiếu bồn thực trước tích hợp. Phong cách low-poly dịu màu, bề mặt nhám, không đèn/emission, không kèm chậu hay kiến trúc.
- Bản thủ tục đã làm: 7 bông, 2.016 tam giác, 1.134 vertex, 3 vật liệu màu, một UV set. Mức mục tiêu ≤3.000 tam giác/cụm; không texture ảnh, không cần texture 4K. Cụm lặp hàng loạt phải bổ sung LOD hoặc giảm thêm mật độ trước khi nhân rộng.
- Pivot (0,0,0) ở đáy, scale (1,1,1); normal tính lại hướng ngoài từng thành phần kín; 0 cạnh non-manifold, 0 mặt suy biến, UV hữu hạn. Các phần thân/lá/cánh là các shell rời, chưa phải lưới liền dùng cho biến dạng. UV smart project chưa được chứng nhận packing/overlap cho texture bake.
- Không thêm collider cho hoa trang trí; bồn chịu va chạm. Bản thử có proxy bệ riêng, không phải hình học bồn thật.
- Đã mở lại `.blend` trong tiến trình Blender khác, xuất FBX, nhập Unity, xác minh kích thước tương đương sau đổi trục. Đã xem ảnh Unity tại mắt 1,65 m, cách cụm khoảng 1,5 m; silhouette và vật liệu hiển thị được. Đây là render editor với bệ mẫu; còn cần xem trong khu vực thật trước khi nhân rộng.
- File: `trial/flower_trial.blend`, `Assets/Tooling3DTrial/FlowerClump.fbx`, scene riêng `Assets/Tooling3DTrial/FlowerReview.unity`. Kết quả đo: `trial/verification.json`, `trial/unity-verification.json`; ảnh: `trial/blender-preview.png`, `trial/unity-player-eye.png`.
- Bản thủ tục: **0 credit dịch vụ**. Nhánh Meshy đề xuất: **ước tính 15 credit/một lượt Smart Topology + 2K**; luồng mặc định CLI dry-run là 30 credit. Chưa thực hiện nhánh nào, không gồm tạo lại hay các bước trả phí khác.

## Chạy lại và bước tiếp theo

1. `C:/blender.exe --background --factory-startup --python-exit-code 1 --python Tools/prepare_3d_trial.py`
2. Chạy cùng lệnh với `-- --verify` để mở lại/kiểm/xuất. Script chỉ ghi khu thử; chạy lại sẽ thay bản thử này.
3. Unity batchmode: `-projectPath D:/Game/GameHorror/Game -executeMethod Tooling3DTrial.Prepare -quit -logFile D:/Game/GameHorror/Game/Docs/Tooling3D/unity-trial.log` để dựng lại scene thử.
4. `python Tools/audit_3d_preparation.py` kiểm hash checkpoint, save, FBX exporter và blend nguồn. Kết quả `preservation.json`.

Tiếp theo tiếp tục sửa r21 bằng pipeline hiện có; kiểm tổng hợp một phần việc hoàn chỉnh. Nhánh tạo Meshy chỉ chạy khi đăng nhập được xác minh và người dùng chốt ngân sách. Giữ mẫu thủ tục để không chặn công việc. Khi có ảnh nguồn đúng cụm hoa, chốt lại loài/màu/độ dày rồi kiểm bồn thật trong Unity trước nhân rộng.

Giới hạn chẩn đoán: Blender không ghi được cache thumbnail tại đường dẫn ngoài workspace trong sandbox; render PNG và save/reopen thật đều thành công, tiến trình Blender xác minh thoát 0. Unity batchmode thoát 0; log có cảnh báo deprecation sẵn có, không có lỗi biên dịch mới. Chưa chạy nghiệm thu gameplay r21 trong bước chuẩn bị này.
