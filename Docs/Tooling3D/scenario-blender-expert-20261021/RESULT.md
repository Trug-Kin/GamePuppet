# Blender Expert — cài và thử local cho CON RỐI

Đã cài đủ **15 skill Blender Expert** trong `.agents/skills`, giữ nguyên skill
Meshy có sẵn. Nguồn [scenario-labs/skills](https://github.com/scenario-labs/skills),
commit **`6e7b8c30f3589c57cf515a00820ddef7c81cc11f`**. Không cài toàn repository.
Không sửa hoặc tự cập nhật nội dung upstream đã cài.

Đã đọc INSTALL.md, README gốc, README Blender và router; kiểm nội dung các
chuyên môn dùng trong thử nghiệm, import/dependency và các điểm gọi subprocess
trong scripts trước khi chạy. README yêu cầu family cài cạnh nhau do dùng chung
scripts, nên cài trọn family 15 thay vì chỉ lấy export/UV. Dùng OpenAI
skill-installer với `--method git --ref <SHA> --dest <project>/.agents/skills`;
không dùng lệnh cập nhật main tự động. Lock ghi hash từng file và đối chiếu với
checkout đã đọc: [installation-lock.json](installation-lock.json).
Hướng dẫn/license upstream lưu tại [reviewed-upstream](reviewed-upstream).

## Đã cài

Router `scenario-blender-expert`; hard-surface, retopology, UV-baking,
texturing-shading, lighting-rendering, rigging, animation và export; kèm các
sibling của family: sculpting, geometry-nodes, hair, grease-pencil,
previs-storyboard, blockout. Khả năng kiến trúc dùng hard-surface/geometry-nodes
với đo tiếp xúc và gate, không có skill riêng tên architecture.

Đây là **Expert local**, thực thi bpy bằng Blender đã cài. Khác với
`scenario-blender` (plugin/Scenario MCP/generation) và các skill dịch vụ.
Blockout có hướng dẫn nhánh plugin/Render-to-Real; nhánh đó không được gọi.
Không cài Unity Expert vì kiểm import nhỏ có thể làm bằng API editor sẵn có;
chỉ bổ sung family Unity khi có việc engine cụ thể cần toolkit đó.
`scenario-3d`, `scenario-textures`, `scenario-skyboxes`, `scenario-meshy` giữ tùy
chọn. Không kết nối tài khoản/MCP, không gửi ảnh dự án, không chạy paid jobs.

23 file Python được AST-parse và kiểm các điểm gọi hệ thống: không thấy client
HTTP/socket hay lệnh pip trong family; hai subprocess helper phục vụ kiểm local
bằng chính Blender. Các script bpy có quyền thay scene/file khi được gọi, nên
không coi skill là sandbox; chỉ chạy chúng với đầu vào/bản sao đã chọn.
Không chạy package prepare/husky, pnpm/npm scripts, examples hoặc cài dependency
dev của toàn repo. Không thêm Blender addon hoặc MCP bridge.

## Phiên bản và nhận diện Codex

| Thành phần | Thực tế |
| --- | --- |
| Blender | **5.2.0 LTS**, build `fbe6228777e7` |
| Python của Blender | **3.13.13** |
| NumPy / OpenImageIO | **2.3.4 / 3.1.13.1**, đi kèm Blender |
| openvdb / aud, bpy/bmesh/mathutils | Đã import được từ Blender có sẵn |
| Unity project và Editor kiểm import | **6000.6.2f1**, revision `770e33f6875c` |
| Upstream đã thử nghiệm | Blender **5.2.1**, Unity **6000.3.21f1**; không mặc định tương đương môi trường này |

Không nâng phiên bản, cài pip hay thay config project. Đã đọc Blender-5.2-deltas
và bpy-reliability; probe RNA của FBX exporter thực tế, nhập thành công **18
module bx_*** từ family đã cài. [blender-api-probe.json](blender-api-probe.json)
giữ phiên bản/phụ thuộc/API. Import thành công không chứng minh mọi chức năng
của module chạy tốt.

Đã hỏi **Codex app-server `skills/list`, forceReload=true** trên cwd project:
15 skill nhận diện, **enabled=true, scope=repo**, không lỗi parse:
[codex-skills-list.json](codex-skills-list.json). Chỉ gọi metadata local, không
tạo model turn. Do sandbox không ghi được runtime SQLite của Codex home người
dùng, phép kiểm dùng CODEX_HOME tạm trong project, không sao chép auth/config;
vẫn quét đúng `.agents/skills` project. Skill sẽ có trong lượt Codex tiếp theo.

## Thao tác đã chạy thực tế

1. Blender headless đọc `PlanterBases.fbx` sản xuất, chỉ lấy bản sao của
   `R15_Plinth_Collision_4` làm bài tập khối bệ visual; ghi hash source và tịnh
   tiến bản sao về gốc để đo. Đây vốn là collision proxy kín, không có UV;
   thiếu UV của proxy không được gọi là lỗi gameplay/asset gốc.
2. Audit topology/normal/volume, kiểm min-Z và bốn điểm chạm nền; render Cycles
   CPU ảnh trước. Sửa đúng một chi tiết hình học: vát cạnh 20 mm bằng toolkit
   hard-surface, weighted normal, recalculation outward. UV explicit CONFORMAL,
   pack padding 4 px tại 512 px; material Principled đơn giản và material audit.
3. Render ảnh sau giữ nguyên camera, ánh sáng, AgX, 768×512 và 24 samples. Chạy
   `bx_review` với silhouette/matcap/wire/normals từ 4 hướng, **đã mở xem ảnh**.
4. Gate trước export: không lỗi/warning của deliverable gate sau sửa; mesh
   kín, không mặt diện tích 0, volume dương, scale=1, UV hữu hạn, chạm nền.
   12 → **188 triangles**, 96 vertex ở Blender. FBX xuất bằng toolkit với
   axes -Z forward/Y up, unit scale 1, leaf bones=false, bake animation=false;
   giữ `bake_space_transform=false` theo pipeline đang dùng, không sửa preset
   upstream hay đổi hàng loạt export project.
5. Tiến trình Blender thứ hai nhập lại FBX: **diffs=[]**, 188 triangles,
   một mesh, UV còn nguyên; gate không lỗi/warning, min-Z ≈ −0.00000021 m.
6. Unity **6000.6.2f1 batchmode/nographics** nhập FBX trong project thử hoàn
   toàn riêng dưới Docs, không mở project game hoặc build binary. Kiểm một
   mesh/188 triangles, đủ normal/UV hữu hạn, normal đơn vị, file/global scale=1,
   bounds đúng trong 1 mm, min-Y gần 0; raycast từ trên trúng mặt trên. Collider
   chỉ được tạo tạm trong scene chưa lưu để kiểm raycast, không bật addCollider
   của importer. Unity đã kết thúc và log xác nhận shutdown.

| Bằng chứng | Kết quả |
| --- | --- |
| [Trước](trial/before.png) / [Sau](trial/after.png) | Cùng camera: vát cạnh tạo highlight rõ hơn, bóng tiếp xúc vẫn bám nền |
| [Review sheet](trial/review/review_sheet.png) | Đã xem 4 hướng × 4 chế độ |
| [Blender roundtrip](trial/roundtrip.json) | PASS_STATIC_BLENDER_ROUNDTRIP |
| [Unity import](trial/unity-import/unity-import-result.json) | PASS_STATIC_UNITY_IMPORT |
| [Nguồn, thông số, UV và export](trial/trial.json) | Kết quả build trước bước reimport; kết quả sau nằm trong roundtrip/Unity JSON riêng |

## Giới hạn và điểm phải giữ khi tích hợp

- Đây là thử static nhỏ. **Bake ảnh thực, rig/deformation, skeletal FBX và
  animation chưa chạy**; chỉ module import của các nhánh đó đã kiểm. Không
  tuyên bố toàn pipeline nhân vật đạt.
- Unity tách vertex ở seam/normal: 96 Blender → 384 Unity, tam giác vẫn 188.
  Node gốc Unity có X≈270.02° với export override đang giữ. Bounds và tiếp xúc
  world đúng; rotation=0 và workflow skeletal chưa được xác minh. Không áp
  preset mới cho asset thật trước khi kiểm orientation/rig tại engine.
- Vát cạnh làm bounds XY giảm khoảng **17 mm mỗi trục** của khối bệ xoay.
  Không dùng bản visual thử để thay collision hay kích thước support gameplay.
  Nếu chọn chi tiết này sau, cần giữ footprint/support chuẩn trong bản dẫn xuất.
- Vật liệu thử chỉ màu phẳng/roughness 0.62, chưa texture/bake hoặc xác minh
  look URP trong scene CON RỐI. Material audit có info về roughness hằng;
  không gọi đây là look-dev production đã hoàn tất.
- Blender lưu ảnh/.blend thành công nhưng báo lỗi tạo thumbnail phụ tại
  `\.thumbnails\large` do HOME của runner; không đổi quyền/HOME hệ thống để né.
  Unity có Curl error 7 khi gọi endpoint telemetry/config Unity; compile/import
  và kiểm assertions vẫn hoàn tất. Không có request Scenario hay ảnh upload.
- GUI sculpt/paint, hair/Grease Pencil thao tác tương tác không được chứng minh
  bằng headless import. Các reference test upstream không nằm trong gói và
  không thay thế kiểm thực tế tại máy này.

## Bảo toàn

[preservation.json](preservation.json): recoveryprep đủ **345 file** khớp frozen
manifest; source FBX hash không đổi; save và ledger đúng SHA giữ từ trước;
guard không sửa trong đợt này. Không thay model trong Assets, ProjectSettings
hoặc package manifest game; project Unity thử riêng có manifest rỗng để tránh
kéo package. Không tích hợp asset thử, không rebuild map, không mở foreground
game, không tạo ứng viên. Các ca native NOT_RUN giữ nguyên.

Chỉ sau khi chọn thay đổi cụ thể mới tích hợp vào bản dẫn xuất, rồi kiểm lại
support/collider/UV/export và ảnh native phù hợp; recoveryprep tiếp tục là mốc
foreground độc lập với thử nghiệm tooling này.
