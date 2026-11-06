from pathlib import Path
R=Path('D:/ConRoi')
history='''## 2026-11-02 — r93: kiểm nền diện rộng; chưa chốt sửa nền, chưa tích hợp Unity

**Trạng thái bàn giao: CHƯA HOÀN TẤT đợt sửa nền.** Giữ `Assets/Scenes/JourneyPresentation_v48_r93.unity`, nguồn L’angfarm, checkpoint r92/r93 và asset sản phẩm. Không tạo scene phiên bản mới, FBX/prefab thay thế, runner, build hay ZIP. Các bản Blender I2 dưới đây là nghiên cứu **chưa nghiệm thu**, không được dùng thay nền hiện hành. Không tiếp tục polish mặt tiền.

### Phạm vi, căn cứ và cách đọc số đo

[Mặt bằng có Bắc và dải cây M1](../Test/BangChung/journey_v48/ground_r93/scope-north-up-with-median.png) đánh dấu ba vị trí đảo tròn **đã nhận diện trong model**: I1 đầu cầu đối diện cổng `(610,67;563,99)`, I2 phía cổng `(714,94;459,48)`, I3 tượng chợ `(783,948;191,167)` theo Unity XZ. Không gọi I1 là đảo thực địa đã xác minh: native ghi `Island_Manual_Estimated_R6.9`. **M1** là cửa sổ đo dải cây giữa hai nhánh Nguyễn Thị Minh Khai; không phải footprint đảo đã đo thực địa. Còn đảo chia nhánh nhỏ trong P02 chưa ánh xạ object chắc chắn, giữ OPEN. Không kết luận mọi đảo/dải phân cách đã được định vị chỉ vì tìm được ba vòng tròn.

Đã mở ảnh gốc P02/P03/P13/P14/P36/P37/P40, ảnh cầu Tuổi Trẻ lịch sử 2011 trong hồ sơ r91, và ảnh lỗi người dùng để đối chiếu. P02 xác nhận quan hệ đảo phía cổng–nhánh phố–dải cây–bờ hồ, P03 là đảo tượng chợ; không nhầm hai vị trí. P36/P37 ghi Street View Oct2020; ảnh cầu 2011 chỉ là bằng chứng thời kỳ đó. Ảnh không cung cấp datum cao độ địa phương hoặc mặt cắt mét. Dốc/bậc/vỉa cục bộ không được xác minh bằng DSM30m. Căn cứ ảnh mặt tiền chính thức r93 tiếp tục giữ như mục lịch sử kế tiếp, không có lần khớp camera mặt tiền mới trong đợt nền này.

Tọa độ native: `Unity=(-BlenderX, BlenderZ, -BlenderY)`; metadata UTM xác nhận Bắc=−UnityZ, Đông=−UnityX. Các tên nội bộ `I1_SW`/`I2_NE` là mã cũ trong JSON, **không dùng chúng làm hướng địa lý**. Đơn vị dưới đây là mét authoring/Unity, không phải kích thước đã khảo sát.

Dump render và collider độc lập, gồm collider không có MeshFilter; đo riêng từng lớp thay vì lấy mặt cao nhất: 106 mặt cắt chính, 54 mặt cắt qua 9 cửa nhánh nhận từ road-mask, bổ sung 23 mặt cắt qua dải cây, sân hai bên và ngang hai đầu cầu. Grid nền 1 đơn vị trong X450..870/Z40..610; dump mở rộng còn có mesh ngoài grid. Lát tuyến chủ yếu 0,5, bổ sung 0,25, vòng góc 2°. **9 cửa road-mask không phải danh sách hoàn chỉnh nhánh thực địa**: đầu cầu có deck riêng, sân có thể chỉ nằm trên terrain. Vẫn còn khoảng giữa lát/grid và mặt chưa phân loại; không ghi toàn map đã đo/đã phẳng.

### Kết luận từng vùng — sản phẩm trước=sau vì chưa thay geometry

| Vùng | Đã đo/xác nhận | Kết luận và phần còn mở |
|---|---|---|
| I1, đầu cầu đối diện | Road128 fit rộng 0,089%; residual −0,257..+0,049; lớp local terrain và các nhánh được đo riêng | Fit thấp không loại trừ mối nối cục bộ. Đảo native là ước lượng; controller vòng/nhánh đợt này NOT_RUN |
| I2 phía cổng | Road128 và collider Context110 cùng tăng **0,291962 trên 0,5 = 58,3924%**, từ XZ `(714,94;473,48)` y0,127461 tới `(714,94;473,98)` y0,419423 | **Lỗi gãy dốc hình học đã xác nhận, chưa sửa trong sản phẩm.** Không phải kết luận từ grounded hay lỗi controller. Các tia60°/120° cũng có đoạn35,46%/39,54% |
| I2, mặt đảo và chuyển tiếp | Mặt đảo trong bán kính12,5 fit `y=0,743942−0,009000*(x−714,94)−0,043000*(z−459,48)`, tilt4,393% | Đảo nghiêng khác plane đường r88. Không nâng đường theo plane ngoài rồi vùi mép đảo; vị trí/cao độ đảo thực địa chưa đo. Giữ hình học đảo và cầu trong nghiên cứu |
| I3, vòng tượng chợ | RoadLoop fit **2,5553%**, residual khoảng −0,047..+0,056; road và terrain/sân được tách | Số fit chỉ mô tả. Mặt cắt terrain ngang có đoạn43 đơn vị giảm7,044 (−16,38%), phía kia43,5 tăng6,984 (+16,05%); không gộp thành dốc thoát nước của mặt vòng. Chưa phân loại hết sân đi được/bờ dốc có chủ đích |
| Cầu Ông Đạo và hai đầu | Collider deck0/1/3 y≈1,14006; đoạn43,53 của deck0 trên lát giữa có Δ≈0. Ramp collider5/7/9 y≈0,95006..1,14006, fit khoảng0,87% | Dùng collider deck để đo, không lấy đỉnh render chứa lan can/vòm. Tiếp giáp road/local terrain được đo riêng; đi hai chiều, chéo và dừng từng nối NOT_RUN trong đợt này |
| TTC–L’angfarm | Trên **bare terrain/apron candidate**, hướng ra trước `n=(0,97334367;0,22935149)`, d8→15 dài7, Δ giảm0,760..0,775, grade−10,86..−11,08%; s10,4/d7,5→23 dài15,5 giảm1,710, −11,029% | Gần một mặt nghiêng dài, không phải chỉ một ổ gồ. Chưa chứng minh vùng mẫu là mặt đường, vỉa hay sân có ranh cụ thể. **Giữ nguyên, cao độ thực địa OPEN**, không san về0 |
| M1 dải cây NTMK | Cửa sổ X724..768/Z370..435; nền dưới cây ParkV19/ParkV22 thuộc terrain chung. Lát dọc X746 dài65, Δ−0,487 (−0,75%) nhưng có biến thiên cục bộ −23,8..+14,3% | Chưa có footprint/cao độ thực địa. Không phải cả một nhóm Garden; còn phải phân loại mặt cây/cỏ/vỉa và mặt dốc trước khi sửa |
| Sân/nhánh ngoài vòng, mép hồ và Garden tiếp giáp | Đã có lát riêng trên road, terrain, footway, collider; `Surface_125` có phần bị terrain cao hơn che, không tự gọi là lỗ nền hở | Chưa kiểm kín khe/chồng của mọi lớp, chưa đi controller toàn phạm vi. Garden/r51_trial không bị sửa/gỡ theo tên |

**Truy nguồn 11%:** 121 điểm r93 so native v33 `Core_Terrain_UTM49N_Graded40m` sai khác tối đa **0,0000381** đơn vị. Độ nghiêng kế thừa terrain v33 có nguồn DSM/formation ước lượng, không phát sinh khi dựng mặt tiền r93. Vùng san terrain r90 đã truy là X807,8..816/Z197,3..204, không phải trước L’angfarm. Điều này xác minh lineage, **không xác minh terrain v33 đúng thực địa**. [Đối chiếu số liệu](../Test/KetQua/journey_v48/ground_r93/langfarm-native-v33-comparison.json).

**Truy I2:** road và mặt đảo dùng grading khác nhau; r88 có vùng loại trừ/attenuation quanh đảo, cầu và điểm kê. Kiểm r87 cho thấy chỗ tăng dốc đã có trước, r88 tiếp tục ảnh hưởng; không quy toàn bộ nguyên nhân cho một lần r88. `i2-r88-field-lineage.json` là đối chiếu xấp xỉ theo field; subdivision rồi displacement không cho phép cộng mẫu điểm để tuyên bố tái dựng chính xác từng vertex.

### Blender: nguồn làm việc và gate chưa đạt

Mở [GroundSurvey_R93_Editable.blend](../Source/Blender/JourneyV48/R93/GroundSurvey_R93_Editable.blend), chọn **GROUND_NATIVE_MATCHED_R93**: 13 render mesh native + 7 collider trong **COLLIDERS_NOT_FOR_RENDER** ẩn. Đây là baseline authoring thật lấy từ nguồn r90/r91, không thay bằng REVIEW nhập Unity. `source-workspace.json` ghi 20 mesh matched, còn10 đường chưa match trong tập ứng viên authoring này; không gọi nguồn đã phủ mọi object. So bidirectional khoảng cách tới **mặt tam giác** với tolerance0,005, không so vertex gần nhất vì FBX đổi seam/retriangulation; tolerance chỉ phục vụ provenance.

Các `Ground_I2_*_Study/Trial_R93.blend` trong cùng thư mục là bản **UNACCEPTED/REJECTED**, có BEFORE ẩn, không được export/integrate. Lần thử đầu giảm dốc road nhưng làm tăng diện tích terrain176 >20% từ0,799 lên15,362 đơn vị² trong vành khảo sát, nên bị bác bỏ. Các nghiên cứu phía Nam/rộng32/mép trong12 giữ đảo/cầu và giới hạn ngoài; đã giảm nhiều đoạn đường nhưng vẫn cần giải quyết mặt mảnh và tiếp giáp toàn bề rộng. Ví dụ bản rộng giảm max grade trên tia90 từ58,392 xuống4,499%, nhưng đó không chứng minh các lớp và nhánh cùng đạt. Offset mép0,04, footway0,06 là **giả định thiết kế của nghiên cứu**, không phải số đo thực địa. Không chọn một lát đẹp để nghiệm thu.

Đã save–thoát process–mở lại bằng Blender5.2.0LTS; audit không có linked library/image bắt buộc thiếu. Tách collider khỏi render review để tránh dùng mesh chồng chẩn đoán làm kết luận hiển thị sản phẩm. Trong bản nghiên cứu rộng đã sửa lỗi ánh xạ do recipe nghiên cứu: **Context136 đi cùng Surface153**, offset nghiên cứu0,06 cho cả hai; trước đó nhầm Context116 nên Context136 nhận0,015, chênh0,045 tại full weight. Đây là lỗi của **bản thử chưa tích hợp**, không phải kết luận collider sản phẩm đang lệch0,045. Giữ báo cáo trước sửa dưới tên `historical-before-collider-map-fix-*`; recipe hiện hành `Test/Tam/ground-r93-wide-study.py` duyệt cả collider trong collection con. Còn gate topology/chuyển tiếp/tiếp đất/đồng nhất collider; **chưa xuất FBX**, nên Unity và source sản phẩm chưa đổi. Ảnh Workbench thiếu một số context đảo/đạo cụ và ảnh camera Edit không phải video gameplay.

### Bằng chứng, kiểm chưa chạy và bước tiếp

- [Mặt bằng](../Test/BangChung/journey_v48/ground_r93/scope-north-up-with-median.png); [lát I2 tách lớp](../Test/BangChung/journey_v48/ground_r93/I2_NE-ray_90-ground-only.png); [lát L’angfarm dài15,5](../Test/BangChung/journey_v48/ground_r93/LANGFARM-cross_s10.4-ground-only.png). Các ảnh `*-ground-only.png` còn lại là lát nền hiện hành, không phải after sửa sản phẩm.
- Số liệu trong `Test/KetQua/journey_v48/ground_r93`: `geometry-inventory.json`, `collider-inventory.json`, `regions.json`, `sections.json`, `layer-sections.json`, `branches.json`, `branch-sections.json`, `branch-layers.json`, `supplement-sections.json`, `supplement-layers.json`, `topology-candidates.json`, `atlas-sections.json`. Face mảnh/downward là **ứng viên cần phân loại**, không mặc định mặt đáy/bó vỉa là lỗi; JSON đã thay NaN bằng null khi không có overlap.
- Các ảnh `*-edit-before.png` là camera chẩn đoán Edit/daylight có restore ánh sáng, đã xem trực tiếp; **không phải góc người chơi đang đi**. Không có video controller trước/sau của đợt nền này. Controller vòng hai chiều/mọi nhánh/cầu/đi chéo, pause–save cũ/load và scene dẫn xuất: **NOT_RUN**. Không mượn kết quả r93 mặt tiền/r90 lịch sử làm PASS cho phạm vi nền mới.
- Kiểm producer mới chỉ đọc các owner lưu/load/phục hồi biên; chưa xác nhận đầy đủ mọi producer qua Play/chuyển khu/load. Query xác nhận Editor ở r93, Edit, dirty=False. `protected-product-final.json` đối chiếu287 mục sản phẩm/nguồn được theo dõi:0 thay đổi. Công cụ bảo toàn xác nhận17 file save gốc được theo dõi,3 ZIP,36 nguồn editable cũ và checkpoint không đổi; không diễn giải17 thành kiểm hash toàn bộ523 save. [Kết quả và giới hạn máy đọc được](../Test/KetQua/journey_v48/ground_r93/final-state.json), [CSV331 đoạn đo trước/sau sản phẩm giữ nguyên](../Test/KetQua/journey_v48/ground_r93/sections-selected-before-after.csv).

**Việc tiếp theo trong chính phạm vi này:** hoàn thiện topology và ranh chuyển tiếp I2 theo các lớp đã đo, kiểm lại mặt cắt cả bề rộng và tiếp đất trước khi xuất; không mở cụm model mới. Nếu chưa đạt gate Blender thì tiếp tục giữ scene r93. Nền11%/sân lớn cần thêm mốc cao độ hoặc ảnh đúng hướng trước khi quyết định hình dạng cuối. Giữ URP Inspector OPEN, V32 UNVERIFIED, tai nghe/phần cứng/người chơi mới NOT_RUN; không tuyên bố map đúng thực địa.

---

'''
workflow='''## 2026-11-02 — kiểm nền r93: đường nguồn và mốc dừng trước export

**Chưa có sửa nền sản phẩm được chấp nhận.** Scene vẫn `Assets/Scenes/JourneyPresentation_v48_r93.unity`; không FBX/prefab/scene/build mới. Kết quả, số đo, lỗi đã xác nhận và phần chưa chạy ở mục kiểm nền đầu PROJECT_GUIDE_AND_HISTORY. Đây là quy trình tái lập chẩn đoán/nghiên cứu, không phải quy trình xuất một asset đã nghiệm thu.

### Nguồn đang dùng và ánh xạ thực tế

Mở `Source/Blender/JourneyV48/R93/GroundSurvey_R93_Editable.blend`, chọn **GROUND_NATIVE_MATCHED_R93**, mở child **COLLIDERS_NOT_FOR_RENDER** khi kiểm collider. 13 render +7 collider được append từ nguồn native r90/r91 sau đối chiếu mặt tam giác hai chiều; còn10 path chưa match trong tập nguồn ứng viên. Không gọi baseline này là nguồn đầy đủ cả map. `MEASUREMENTS_DO_NOT_EXPORT` là đường đo; file có `READ_ME_GROUND_R93_HANDOFF`. Không xuất các BEFORE/camera/measurement hoặc collider như render. `Langfarm_R93_Editable.blend` giữ nguyên, không phải nguồn của nền đã san lại.

| Phạm vi | Authoring native → bản làm việc r93 | Asset/Hierarchy hiện hành, chưa thay |
|---|---|---|
| Road I1/I2 và nhánh | `Source/Blender/JourneyV48/R91/north-bank-r91-extra-final.blend` / `BankR91_Grade_052` → GroundSurvey | `BankR91/BankR91_Grade_052_Bound.asset` → `R20_Whole_Source_Export/Surface_128`; collider riêng `LakeGateMarketV32/Context_110` / Grade015 nguồn `north-bank-r91-source-final.blend` |
| Nhánh thứ cấp I2 | Native Grade065 / Grade006 | `Surface_129` và `Context_111`; Context111 còn chứa road khác, **không thay toàn collider bằng riêng render129** |
| Local terrain/bờ | `north-bank-r91-source-final.blend` / BankR91_Grade017 | `BankR91_Grade_017_Bound.asset` → `R20_Whole_Source_Export/Surface_142`, kiểm MeshCollider cùng object |
| Promenade | `north-bank-r91-source-final.blend` / BankR91_Grade057, collider Grade007 | `Surface_153` và `Context_136`; không nhầm Context116 là toàn collider của153 |
| Terrain chính, sân trước TTC/chợ | `Docs/Reviews/journey_v48/r90_review/terrain-stage-r90-source.blend` / `TerrainJoin_R90`, `TerrainColliderJoin_R90` | `MarketBranchesR90/TerrainJoin_R90_Bound.asset` → `Surface_176`; `TerrainColliderJoin_R90_Bound.asset` → `LakeGateMarketV32/Context_124` |
| Nối sân/vỉa chợ | `.../r90_review/market-branches-r90-source.blend` | `MarketBranchesR90/EastFootwayJoin_R90_Bound.asset` → `Surface_104`; các Restored/footway còn đối chiếu độc lập |
| Đảo/đường native, dải cây M1 | `Docs/Reviews/lake_gate_market_current/ntmk_full_map_route_materials_v33.blend`; tra `ExportPolicyR20/surfaces.json` theo trường `mesh` | I1 Surface045/046/048/049; I3 `MarketIsland_R62/*` + `RoadLoop_125314010_R65`; ParkV19/ParkV22 cây là Surface095..103, nền là terrain chung, không tự gán tất cả về Garden |
| Cầu và mặt đảo I2 | `Docs/Reviews/journey_v48/r48/ong-dao-r48-source.blend` và nguồn kế thừa; không import Unity REVIEW làm nguồn | `R11_Local_Bridge_Supports/OD35_Surface_*`; collider thật `OngDao_V35_Photo_Batch/OD35_COLLIDER_*`, `R22_Island_Deck_Collider`; render chứa lan can/vòm không dùng làm độ cao mặt đi |

Tiền tố asset rút gọn trong bảng nằm dưới `Assets/Art/Environment/JourneyV48/`, trừ các asset LakeGateMarketV32/V33 gốc. Exact path/material/layer/matrix/collider tham chiếu ở `Test/KetQua/journey_v48/ground_r93/{inventory.json,collider-inventory.json,geometry-inventory.json,source-workspace.json}`. Không thay GUID, transform, material hay layer dựa vào tên bảng rút gọn.

### Tái lập chẩn đoán và Blender

1. Mở scene r93 sạch Edit. Dùng bridge Unity_RunCommand **hiện có**, không thêm script kiểm vào Assets/player. Snippet `Test/Tam/ground-r93-inventory.cs.txt`, bản mở rộng và collider dump đọc MeshFilter/MeshCollider độc lập; lưu matrix/world vertices. Connection/PID cũ trong `r93-pipe.py` chỉ là phiên đã chạy; khi tái lập phải chọn connection sống và kiểm scene/dirty trước. `GetInstanceID()` không phù hợp API Unity6000.6; không sửa sản phẩm chỉ để snippet compile.
2. `ground-r93-survey.py` → `ground-r93-measure.py` → `ground-r93-branches.py` → `ground-r93-supplement.py` tạo các lớp/lát số. Mỗi thao tác đọc JSON/meshbin nguồn trong Test; không lấy topmost chung làm mặt duy nhất. `atlas.py` tạo bản đồ Bắc từ metadata UTM, `supplement.py` đánh dấu M1 nhưng không suy footprint từ tán cây. Các file exact world cache là **bằng chứng chẩn đoán**, không thay nguồn editable.
3. `ground-r93-native-trace.py`, `ground-r93-findings.py`, `ground-r93-lineage-check.py` đối chiếu native v33 và field r88. Độ dốc11% trước TTC kế thừa native v33; không coi lineage đúng là cao độ đúng. Mặt đường I2/collider cùng gãy0,292 trên0,5 là lỗi kỹ thuật có bằng chứng độc lập.
4. `ground-r93-workspace.py` append native r90/r91 với `unity_path`; kiểm khoảng cách tới mặt tam giác hai chiều thay cho nearest vertex. Tolerance0,005 chỉ dùng truy nguồn, không phải độ chính xác khảo sát. `ground-r93-transition-trial.py` và các south/wide/inner study là nghiên cứu chưa đạt; `ground-r93-handoff-state.py` tách collider và gắn nhãn chặn xuất. **Recipe bản rộng hiện hành là `ground-r93-wide-study.py`**: duyệt `col.all_objects` để không bỏ collider trong collection con; Context136 phải cùng offset0,06 với Surface153, không nhầm Context116. Các báo cáo `historical-before-collider-map-fix-*` là trước sửa recipe nghiên cứu. Không chạy recipe nghiên cứu rồi tự cập nhật asset Unity.
5. Headless Blender5.2.0LTS: `C:/blender.exe -b --factory-startup --python-exit-code 1 -P <recipe>`; một process mỗi lượt. Nguồn mới ở Source/Blender/JourneyV48/R93, log/npz/json ở Test/KetQua/journey_v48/ground_r93, ảnh ở Test/BangChung/journey_v48/ground_r93. Đã lưu/thoát/mở lại; không missing linked library/image. Workbench shadow ở bản diagnostic đầu có sọc, nên không dùng ảnh đó để kết luận mesh sản phẩm lỗi hay đã sửa; ảnh flat/lát tách lớp hỗ trợ đọc hình học. Gate normals/UV/scale không thay bằng đánh giá khớp ảnh.

### Mốc phải đạt trước Unity — hiện còn OPEN

Giải quyết chuyển tiếp toàn bề rộng, mặt mảnh, lớp chồng/khe và quan hệ road–footway–terrain/collider; đối chiếu cùng lát trước/sau, wireframe và render có context. Giữ tâm đảo, footprint, shoreline và cấu trúc cầu; bảo đảm chân biển/cây/đèn/NPC bị tác động. Không áp một displacement theo road lên terrain một cách máy móc: bản thử đầu đã tạo gồ terrain lớn hơn. Không dùng height guard phụ thuộc y để khóa nền mà làm gãy theo lớp. Các offset0,04/0,06 của nghiên cứu phải ghi là giả định, chưa phải chuẩn vỉa thực địa.

**Chưa chạy export/import/integration cho đợt nền.** Sau khi gate trên đạt mới dùng FBX preset Unity (−Z forward/Y up, unit scale, không animation), roundtrip đo world geometry/UV/material; giữ mapping gốc của UV nhiều kênh và vertex color khi bind vào transform legacy. Không sửa bù scale/rotation ở Unity hoặc bỏ UV bổ sung của shader terrain. Tạo scene/prefab dẫn xuất và thay đúng mesh render/collider, không để hai nền chồng, không làm mất các road khác trong collider gộp.

Sau tích hợp phải kiểm Inspector thực: sharedMesh, collider cooking/convex/trigger, material, layer và reference; producer qua Play/chuyển khu/load; đi controller từng vòng/nhánh/cầu hai chiều, chéo, dừng; save thử cao độ cũ, pause/load, lưu–đóng–mở lại. Các ca đó **NOT_RUN trong đợt nền này**. Script `KhoiPhucCaoDoSauTaiR73` là reseat một lần sau load, `PhucHoiTaiBienR21` là phục hồi biên; không sửa controller/grounded để bù lỗi. Chưa đủ producer audit để kết luận không thể sinh lại geometry. Giữ mọi OPEN/UNVERIFIED/NOT_RUN lịch sử và checkpoint.

---

'''
for path,prefix in [('Docs/PROJECT_GUIDE_AND_HISTORY.md',history),('Docs/BLENDER_UNITY_WORKFLOW.md',workflow)]:
 p=R/path;raw=p.read_bytes();marker=prefix.split('\n',1)[0].encode('utf-8')
 if raw.startswith(marker):raw=raw.split(b'\n---\n\n',1)[1]
 p.write_bytes(prefix.encode('utf-8')+raw)
print('Updated two existing documents; prior body retained byte-for-byte.')
