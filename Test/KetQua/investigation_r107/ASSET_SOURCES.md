# Nguồn và giấy phép r107

Không tải bộ art mới. Quaternius nhân vật/rối, nguồn/giấy phép và attribution giữ theo [bảng r104](../investigation_r104/ASSET_SOURCES.md); clip Tư/hình nộm và giới hạn minh họa giữ theo [r105](../investigation_r105/README.md). Không đổi model, rig, material hoặc pipeline.

| Asset thay đổi | Nguồn / người tạo | Cách tái tạo và giới hạn |
|---|---|---|
| TerrainR107_Final.fbx và4mesh bound riêng r107 | Nguồn tự dựng Con Rối R101, sửa winding trong Blender5.2.0LTS; không có asset mạng mới | `Source/Blender/JourneyV48/R107/Terrain_R107_Editable.blend`, collection TERRAIN_R107_WINDING_REPAIR; [mapping](MAP_REPAIR.md), báo cáo `source-export-final.json`, `map-integration-final.txt` |
| Onboarding và menu theo ngữ cảnh | Script tự viết, `Assets/Scripts/InvestigationV36R107` | Scene r107 remap11GUID; `InvestigationOnboarding.cs` bổ sung. Không chạy lại script derive/author/bind |
| Biển xưởng, vệt sàn, marker | Biển/vật chứng sẵn từ checkpoint; marker và UI code-native | Không thêm bitmap/model hoặc yêu cầu attribution mới |

Không thay hoặc xóa asset mà checkpoint dùng. Generic gate nguồn terrain vẫn FAIL với hạn chế ghi trong [MAP_REPAIR.md](MAP_REPAIR.md), không ghi asset là final topology.
