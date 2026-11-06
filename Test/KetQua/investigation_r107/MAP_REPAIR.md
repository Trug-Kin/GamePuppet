# Sửa nền theo phản hồi người dùng — 06/11/2026

Checkpoint r106 giữ nguyên. Scene sửa: `Assets/Scenes/JourneyPresentation_v48_r107.unity`.

## Nguyên nhân đã xác nhận

Ảnh `Screenshot 2026-11-06 105627.png` được tái hiện từ player `(681.10,2.89,413.07)`, eye `(681.10,3.64,413.07)`, pitch10.10/yaw230. Mặt nền vẫn có collider đỡ nhưng nhiều tam giác render quay xuống: normalY khoảng −0,98 tới −0,9986. Backface culling làm mất mặt khi nhìn từ trên. Renderer/reference không mất; không có bằng chứng producer dựng nhầm mesh. Không quy lỗi cho M2 hoặc legacy chỉ vì lịch sử OPEN.

Ba ảnh 105506/105520/105534 đã xem trực tiếp, cho ngữ cảnh các mặt tiền/quảng trường, cổng và khối tường. Chưa xác định chính xác pose của cả ba; các ảnh so sánh view0–2 chỉ là hướng nhìn bổ sung từ vị trí ảnh cuối, **không phải ba góc trước/sau khớp ảnh người dùng**. Không gọi các khối xa chưa hoàn thiện là đã sửa. `comparison-views-final.txt` ghi rõ camera từng ảnh.

Đối chiếu reference checkpoint trong `checkpoint-reference-comparison.json`: các asset có mặt từ r97 và còn tham chiếu ở r99–r106. Đây là mốc sớm nhất đã xác minh, chưa chứng minh thời điểm đầu tiên tạo winding sai.

## Thay đổi có giới hạn

Nguồn editable: `Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend`, collection `TERRAIN_R107_WINDING_REPAIR`, lấy ngữ cảnh editable R101. Blender5.2.0LTS → `TerrainR107_Final.fbx` → mesh riêng r107. Trong cửa sổ X560..720/Z320..460, Y centroid>−0,05, sửa hướng các mặt nền quay xuống; không dịch vertex/cao độ, nước hoặc nền chợ. Phần Z418..460 được bổ sung sau pixel-ray xác nhận vết rách kề ảnh, không mở rộng san map.

| Object scene | Nguồn Blender | Số tam giác đảo tích lũy trên bound mesh |
|---|---|---:|
| `R20_Whole_Source_Export/Surface_176` | TerrainJoin_R90 | 914 |
| `R20_Whole_Source_Export/Surface_104` | EastFootwayJoin_R90 | 2137 |
| `LakeGateMarketV32/Context_124` | TerrainColliderJoin_R90 | 894 |
| `R20_Whole_Source_Export/Surface_142` | BankR91_Grade_017 | 2522 |

Chỉ bốn reference trên thay trong r107. Surface142 dùng cùng mesh sửa cho renderer/collider; Context124 là collider terrain. `Context_109/M95_Service4_NATIVE` đã đúng, giữ nguyên. Không bật Two Sided, fog hoặc tấm che.

FBX giữ nguồn chỉnh sửa và hướng mặt. Khâu tích hợp chuyển hướng từ triangle FBX gần nhất lên topology bound cũ (ngưỡng0,02m) để giữ đúng vertex/UV; tất cả target có nguồn khớp, unmatched0, vertexDelta0, UVEqualTrue, không đổi topology ngoài đảo thứ tự. Các script derive/integrate là thao tác một lần, **không chạy lại**. Khi sửa tiếp, mở scene/Blender hiện hành và chỉnh có mục tiêu; bảng mapping có trong `map-integration-final.txt` và `source-export-final.json`.

## Kết quả và giới hạn

Ảnh cuối `map-final-before-3.png`/`map-final-after-3.png` cùng camera tái hiện vùng hở rộng; view2 và cặp top kiểm vùng nối kề. Các ảnh `map-before-*`/`map-after-*` đời đầu là **superseded**, còn thiếu vết rách kề, không dùng làm kết quả cuối.

Nguồn Blender mở lại, roundtrip FBX5object sai số tối đa dưới0,001m. `source-final-audit.json` không có mặt top quay xuống còn lại theo phép đo nguồn. Audit bound Unity vẫn báo 8 tam giác rất mảnh (gồm bản collider lặp), diện tích khoảng6,16e−11..1,95e−9m², cạnh dài0,00020..0,01182m; có sai khác normal do lượng tử hóa float/triangulation. Xem `residual-faces.txt`; không tuyên bố mọi tam giác đã hoàn hảo hoặc toàn map sạch lỗi.

**Generic `source-delivery-gate.json`: FAIL**, giữ nguyên kết quả. Gate dành cho prop kín báo các mảnh terrain rời, face trùng/coplanar và loose vertex kế thừa, cùng heuristic mặt nhỏ. Audit diện tích thực riêng không thấy mặt có area≤1e−12 trong nguồn. Không xóa hoặc weld toàn terrain chỉ để đổi gate thành PASS. Các cảnh báo inward ngoài X720 không thuộc sửa này.

Đường controller và lifecycle có log riêng trong README/checklist. Không đổi cao độ để che lỗi. Chưa thêm hàng rào giới hạn ngoài prototype; vùng xa chưa hoàn thiện chưa được chứng nhận an toàn. Việc sửa mặt hỏng và việc giới hạn truy cập được báo riêng, không dùng giới hạn thay bằng chứng sửa nền.
