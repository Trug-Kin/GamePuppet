# Component và Inspector r92 — đọc trực tiếp Editor

Owner dưới đây là đường Hierarchy thực trong scene, không tự thêm component. `prefab` rỗng trong báo cáo nghĩa là object/component lưu trực tiếp trong scene; mesh có thể vẫn lấy từ FBX/.asset. Các reference null hợp lệ được bỏ khỏi bảng ngắn, báo cáo live giữ đầy đủ để phân biệt null tùy chọn với reference hỏng. Missing Script = 0, broken object references = 0.

## `Minh_RegionalWalk_180cm` → `DieuKhienDiChuyen`

Script: `Assets/Scripts/NguoiChoi/DieuKhienDiChuyen.cs` — tên cũ `PlayerMovement`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `controller` | `Minh_RegionalWalk_180cm` (CharacterController) |
| `groundCheck` | `Minh_RegionalWalk_180cm/GroundCheck` (Transform) |

## `Minh_RegionalWalk_180cm` → `DieuKhienGocNhin`

Script: `Assets/Scripts/NguoiChoi/DieuKhienGocNhin.cs` — tên cũ `MouseMovement`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `Minh_RegionalWalk_180cm` → `NhichCameraKhiDungYen`

Script: `Assets/Scripts/NguoiChoi/NhichCameraKhiDungYen.cs` — tên cũ `ComplianceSystem`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `camTransform` | `Minh_RegionalWalk_180cm/Main Camera` (Transform) |

## `Minh_RegionalWalk_180cm` → `HoTroDiChuyenTheoKhu`

Script: `Assets/Scripts/NguoiChoi/HoTroDiChuyenTheoKhu.cs` — tên cũ `RegionalWalkHelp`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `Minh_RegionalWalk_180cm` → `TuongTacNguoiChoi`

Script: `Assets/Scripts/TuongTac/TuongTacNguoiChoi.cs` — tên cũ `PlayerInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerCamera` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |

## `Minh_RegionalWalk_180cm/Main Camera` → `GhiNhanTraiNghiemV40`

Script: `Assets/Scripts/ChanDoan/GhiNhanTraiNghiemV40.cs` — tên cũ `ExperienceRecorderV40`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `Minh_RegionalWalk_180cm/Main Camera` → `DoHieuNangRenderV47`

Script: `Assets/Scripts/ChanDoan/DoHieuNangRenderV47.cs` — tên cũ `ProfilePlayerRenderV47`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `Regional_Surface_Lighting` → `AnhSangBeMatTheoKhu`

Script: `Assets/Scripts/MoiTruong/AnhSangBeMatTheoKhu.cs` — tên cũ `RegionalSurfaceLighting`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `StoryProps/CR_Hub/CR_Bed` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Hub/CR_Bed/CR_Bed_Interaction` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Hub/CR_Phone` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Hub/CR_Phone/CR_Phone_Interaction` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings` → `DayConRoiR46`

Script: `Assets/Scripts/ConRoi/DayConRoiR46.cs` — tên cũ `PuppetThreadsR46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `sourceBindings` | `Assets/Art/Characters/R46/ThreadBindings.json` (TextAsset) |
| `fittedRoutingR91` | `Assets/Art/Characters/R91/ThreadRoutingR91.json` (TextAsset) |
| `attachments.Array.data[0]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_Leg_R` (MeshFilter) |
| `segments.Array.data[0]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_1_0` (Transform) |
| `segments.Array.data[1]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_1_1` (Transform) |
| `segments.Array.data[2]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_2_0` (Transform) |
| `segments.Array.data[3]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_2_1` (Transform) |
| `segments.Array.data[4]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_3_0` (Transform) |
| `segments.Array.data[5]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_3_1` (Transform) |
| `segments.Array.data[6]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_4_0` (Transform) |
| `segments.Array.data[7]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_4_1` (Transform) |
| `segments.Array.data[8]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_5_0` (Transform) |
| `segments.Array.data[9]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_5_1` (Transform) |

## `StoryProps/CR_Hub/Document_TỜ GIẤY CỦA MẸ` → `DaoCuKeChuyenV40`

Script: `Assets/Scripts/TuongTac/DaoCuKeChuyenV40.cs` — tên cũ `NarrativePropV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Hub/Window_Garden_Latch` → `TuongTacVuonV41`

Script: `Assets/Scripts/TuongTac/TuongTacVuonV41.cs` — tên cũ `GardenInteractionV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (HoanTatHanhTrinhV41) |

## `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig` → `BieuCamNhoNhanVatV41`

Script: `Assets/Scripts/NhanVat/BieuCamNhoNhanVatV41.cs` — tên cũ `ActorMicroPerformanceV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Lake/CR_Bao` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Lake/CR_Bao` → `HienGuongMatTheoKhu`

Script: `Assets/Scripts/NhanVat/HienGuongMatTheoKhu.cs` — tên cũ `RegionalFaceProjection`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `ghostHead` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot` (Transform) |
| `minhFace` | `Assets/Art/Models/Model.blend` (Mesh) |
| `minhMaterial` | `Assets/Art/Gameplay/LakeMarketV34/MinhFace.mat` (Material) |
| `minhHeadSource` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/Minh_Head_Pivot` (Transform) |
| `fittedPortraitR91` | `Assets/Art/Characters/R92/R92_Portrait_CanonicalBound.asset` (Mesh) |
| `portraitMaterialsR91.Array.data[0]` | `Assets/Art/Characters/R47/R47_Warm_Wood.mat` (Material) |
| `portraitMaterialsR91.Array.data[1]` | `Assets/Art/Characters/R47/R47_Dark_Endgrain.mat` (Material) |
| `portraitMaterialsR91.Array.data[2]` | `Assets/Art/Characters/R47/R47_Knot_Dark.mat` (Material) |
| `portraitMaterialsR91.Array.data[3]` | `Assets/Art/Characters/R47/R47_Knot_Heart.mat` (Material) |

## `StoryProps/CR_Lake/CR_Bao` → `DienXuatNhanVatV40`

Script: `Assets/Scripts/NhanVat/DienXuatNhanVatV40.cs` — tên cũ `ActorPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `gaze` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot` (Transform) |
| `body` | `StoryProps/CR_Lake/CR_Bao` (Animation) |

## `StoryProps/CR_Lake/CR_Bao` → `BieuCamNhoNhanVatV41`

Script: `Assets/Scripts/NhanVat/BieuCamNhoNhanVatV41.cs` — tên cũ `ActorMicroPerformanceV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Lake/CR_Bao/CR_Bao_Interaction` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Lake/CR_Lake_Wake` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Lake/CR_Lake_Wake/CR_Lake_Wake_Interaction` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Lake/ConRoi_Minh_0_0` → `DieuKhienConRoi`

Script: `Assets/Scripts/ConRoi/DieuKhienConRoi.cs` — tên cũ `ConRoiAI`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerTransform` | `Minh_RegionalWalk_180cm` (Transform) |
| `dauTransform` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/Minh_Head_Pivot` (Transform) |

## `StoryProps/CR_Lake/ConRoi_Minh_0_0` → `DienXuatConRoiV40`

Script: `Assets/Scripts/ConRoi/DienXuatConRoiV40.cs` — tên cũ `PuppetPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Lake/ConRoi_Minh_0_0` (DieuKhienConRoi) |
| `articulation` | `StoryProps/CR_Lake/ConRoi_Minh_0_0` (Animation) |

## `StoryProps/CR_Lake/ConRoi_Minh_0_0` → `TiepDatConRoiV41`

Script: `Assets/Scripts/ConRoi/TiepDatConRoiV41.cs` — tên cũ `PuppetGroundingV41`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `StoryProps/CR_Lake/ConRoi_Minh_0_0` → `BuocChanConRoiR23`

Script: `Assets/Scripts/ConRoi/BuocChanConRoiR23.cs` — tên cũ `PuppetStrideR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Lake/ConRoi_Minh_0_0` (DieuKhienConRoi) |
| `trajectory` | `Assets/Art/Characters/R23/WoodenStep.json` (TextAsset) |
| `supportR46` | `OngDao_V35_Photo_Batch/OD35_COLLIDER_3` (MeshCollider) |
| `fittedLegsR91.Array.data[0]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R91_Fitted_Leg_L` (Transform) |
| `fittedLegsR91.Array.data[1]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R91_Fitted_Leg_R` (Transform) |

## `StoryProps/CR_Lake/ConRoi_Minh_0_0` → `DayConRoiR45`

Script: `Assets/Scripts/ConRoi/DayConRoiR45.cs` — tên cũ `PuppetThreadsR45`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `attachments.Array.data[0]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_L/ConRoi_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_R/ConRoi_Leg_R` (MeshFilter) |
| `threads.Array.data[0]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_1_NoiTam` (Transform) |
| `threads.Array.data[1]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_2_XaHoi` (Transform) |
| `threads.Array.data[2]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_3_HeThong` (Transform) |
| `threads.Array.data[3]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_4_CaNhan` (Transform) |
| `threads.Array.data[4]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_5_HienSinh` (Transform) |
| `head` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshRenderer) |

## `StoryProps/CR_Lake/ConRoi_Minh_0_0` → `DayConRoiR46`

Script: `Assets/Scripts/ConRoi/DayConRoiR46.cs` — tên cũ `PuppetThreadsR46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `sourceBindings` | `Assets/Art/Characters/R46/ThreadBindings.json` (TextAsset) |
| `fittedRoutingR91` | `Assets/Art/Characters/R91/ThreadRoutingR91.json` (TextAsset) |
| `attachments.Array.data[0]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R91_Fitted_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R91_Fitted_Leg_R` (MeshFilter) |
| `segments.Array.data[0]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_1_Segment_1` (Transform) |
| `segments.Array.data[1]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_1_Segment_2` (Transform) |
| `segments.Array.data[2]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_2_Segment_1` (Transform) |
| `segments.Array.data[3]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_2_Segment_2` (Transform) |
| `segments.Array.data[4]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_3_Segment_1` (Transform) |
| `segments.Array.data[5]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_3_Segment_2` (Transform) |
| `segments.Array.data[6]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_4_Segment_1` (Transform) |
| `segments.Array.data[7]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_4_Segment_2` (Transform) |
| `segments.Array.data[8]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_5_Segment_1` (Transform) |
| `segments.Array.data[9]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/R46_String_5_Segment_2` (Transform) |

## `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/Mat_ke_an_toan` → `DichTuongTacSoTayR37`

Script: `Assets/Scripts/TuongTac/DichTuongTacSoTayR37.cs` — tên cũ `LakeNotebookTargetR37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (TuongTacSoTayHoR37) |

## `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/So_cua_Minh` → `DichTuongTacSoTayR37`

Script: `Assets/Scripts/TuongTac/DichTuongTacSoTayR37.cs` — tên cũ `LakeNotebookTargetR37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (TuongTacSoTayHoR37) |

## `StoryProps/CR_Market/CR_Ha` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Market/CR_Ha` → `HienGuongMatTheoKhu`

Script: `Assets/Scripts/NhanVat/HienGuongMatTheoKhu.cs` — tên cũ `RegionalFaceProjection`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `ghostHead` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot` (Transform) |
| `minhFace` | `Assets/Art/Models/Model.blend` (Mesh) |
| `minhMaterial` | `Assets/Art/Gameplay/LakeMarketV34/MinhFace.mat` (Material) |
| `minhHeadSource` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/Minh_Head_Pivot` (Transform) |
| `fittedPortraitR91` | `Assets/Art/Characters/R92/R92_Portrait_CanonicalBound.asset` (Mesh) |
| `portraitMaterialsR91.Array.data[0]` | `Assets/Art/Characters/R47/R47_Warm_Wood.mat` (Material) |
| `portraitMaterialsR91.Array.data[1]` | `Assets/Art/Characters/R47/R47_Dark_Endgrain.mat` (Material) |
| `portraitMaterialsR91.Array.data[2]` | `Assets/Art/Characters/R47/R47_Knot_Dark.mat` (Material) |
| `portraitMaterialsR91.Array.data[3]` | `Assets/Art/Characters/R47/R47_Knot_Heart.mat` (Material) |

## `StoryProps/CR_Market/CR_Ha` → `DienXuatNhanVatV40`

Script: `Assets/Scripts/NhanVat/DienXuatNhanVatV40.cs` — tên cũ `ActorPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `gaze` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot` (Transform) |
| `body` | `StoryProps/CR_Market/CR_Ha` (Animation) |

## `StoryProps/CR_Market/CR_Ha` → `BieuCamNhoNhanVatV41`

Script: `Assets/Scripts/NhanVat/BieuCamNhoNhanVatV41.cs` — tên cũ `ActorMicroPerformanceV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Market/CR_Ha/CR_Ha_Interaction` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Market/CR_Market_Wake` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Market/CR_Market_Wake/CR_Market_Wake_Interaction` → `TuongTacHoCho`

Script: `Assets/Scripts/TuongTac/TuongTacHoCho.cs` — tên cũ `LakeMarketInteraction`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `StoryProps/CR_Market/ConRoi_Minh_1_0` → `DieuKhienConRoi`

Script: `Assets/Scripts/ConRoi/DieuKhienConRoi.cs` — tên cũ `ConRoiAI`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerTransform` | `Minh_RegionalWalk_180cm` (Transform) |
| `dauTransform` | `StoryProps/CR_Market/ConRoi_Minh_1_0/Minh_Head_Pivot` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_0` → `DienXuatConRoiV40`

Script: `Assets/Scripts/ConRoi/DienXuatConRoiV40.cs` — tên cũ `PuppetPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Market/ConRoi_Minh_1_0` (DieuKhienConRoi) |
| `articulation` | `StoryProps/CR_Market/ConRoi_Minh_1_0` (Animation) |

## `StoryProps/CR_Market/ConRoi_Minh_1_0` → `TiepDatConRoiV41`

Script: `Assets/Scripts/ConRoi/TiepDatConRoiV41.cs` — tên cũ `PuppetGroundingV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `supportR46` | `LakeGateMarketV32/Context_112` (MeshCollider) |

## `StoryProps/CR_Market/ConRoi_Minh_1_0` → `BuocChanConRoiR23`

Script: `Assets/Scripts/ConRoi/BuocChanConRoiR23.cs` — tên cũ `PuppetStrideR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Market/ConRoi_Minh_1_0` (DieuKhienConRoi) |
| `trajectory` | `Assets/Art/Characters/R23/WoodenStep.json` (TextAsset) |
| `supportR46` | `LakeGateMarketV32/Context_112` (MeshCollider) |
| `fittedLegsR91.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R91_Fitted_Leg_L` (Transform) |
| `fittedLegsR91.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R91_Fitted_Leg_R` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_0` → `DayConRoiR45`

Script: `Assets/Scripts/ConRoi/DayConRoiR45.cs` — tên cũ `PuppetThreadsR45`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `attachments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_L/ConRoi_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_R/ConRoi_Leg_R` (MeshFilter) |
| `threads.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_1_NoiTam` (Transform) |
| `threads.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_2_XaHoi` (Transform) |
| `threads.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_3_HeThong` (Transform) |
| `threads.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_4_CaNhan` (Transform) |
| `threads.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_5_HienSinh` (Transform) |
| `head` | `StoryProps/CR_Market/ConRoi_Minh_1_0/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshRenderer) |

## `StoryProps/CR_Market/ConRoi_Minh_1_0` → `DayConRoiR46`

Script: `Assets/Scripts/ConRoi/DayConRoiR46.cs` — tên cũ `PuppetThreadsR46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `sourceBindings` | `Assets/Art/Characters/R46/ThreadBindings.json` (TextAsset) |
| `fittedRoutingR91` | `Assets/Art/Characters/R91/ThreadRoutingR91.json` (TextAsset) |
| `attachments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R91_Fitted_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R91_Fitted_Leg_R` (MeshFilter) |
| `segments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_1_Segment_1` (Transform) |
| `segments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_1_Segment_2` (Transform) |
| `segments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_2_Segment_1` (Transform) |
| `segments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_2_Segment_2` (Transform) |
| `segments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_3_Segment_1` (Transform) |
| `segments.Array.data[5]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_3_Segment_2` (Transform) |
| `segments.Array.data[6]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_4_Segment_1` (Transform) |
| `segments.Array.data[7]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_4_Segment_2` (Transform) |
| `segments.Array.data[8]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_5_Segment_1` (Transform) |
| `segments.Array.data[9]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/R46_String_5_Segment_2` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_1` → `DieuKhienConRoi`

Script: `Assets/Scripts/ConRoi/DieuKhienConRoi.cs` — tên cũ `ConRoiAI`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerTransform` | `Minh_RegionalWalk_180cm` (Transform) |
| `dauTransform` | `StoryProps/CR_Market/ConRoi_Minh_1_1/Minh_Head_Pivot` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_1` → `DienXuatConRoiV40`

Script: `Assets/Scripts/ConRoi/DienXuatConRoiV40.cs` — tên cũ `PuppetPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Market/ConRoi_Minh_1_1` (DieuKhienConRoi) |
| `articulation` | `StoryProps/CR_Market/ConRoi_Minh_1_1` (Animation) |

## `StoryProps/CR_Market/ConRoi_Minh_1_1` → `TiepDatConRoiV41`

Script: `Assets/Scripts/ConRoi/TiepDatConRoiV41.cs` — tên cũ `PuppetGroundingV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `supportR46` | `LakeGateMarketV32/Context_112` (MeshCollider) |

## `StoryProps/CR_Market/ConRoi_Minh_1_1` → `BuocChanConRoiR23`

Script: `Assets/Scripts/ConRoi/BuocChanConRoiR23.cs` — tên cũ `PuppetStrideR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Market/ConRoi_Minh_1_1` (DieuKhienConRoi) |
| `trajectory` | `Assets/Art/Characters/R23/WoodenStep.json` (TextAsset) |
| `supportR46` | `LakeGateMarketV32/Context_112` (MeshCollider) |
| `fittedLegsR91.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R91_Fitted_Leg_L` (Transform) |
| `fittedLegsR91.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R91_Fitted_Leg_R` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_1` → `DayConRoiR45`

Script: `Assets/Scripts/ConRoi/DayConRoiR45.cs` — tên cũ `PuppetThreadsR45`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `attachments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_L/ConRoi_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_R/ConRoi_Leg_R` (MeshFilter) |
| `threads.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_1_NoiTam` (Transform) |
| `threads.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_2_XaHoi` (Transform) |
| `threads.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_3_HeThong` (Transform) |
| `threads.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_4_CaNhan` (Transform) |
| `threads.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_5_HienSinh` (Transform) |
| `head` | `StoryProps/CR_Market/ConRoi_Minh_1_1/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshRenderer) |

## `StoryProps/CR_Market/ConRoi_Minh_1_1` → `DayConRoiR46`

Script: `Assets/Scripts/ConRoi/DayConRoiR46.cs` — tên cũ `PuppetThreadsR46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `sourceBindings` | `Assets/Art/Characters/R46/ThreadBindings.json` (TextAsset) |
| `fittedRoutingR91` | `Assets/Art/Characters/R91/ThreadRoutingR91.json` (TextAsset) |
| `attachments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R91_Fitted_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R91_Fitted_Leg_R` (MeshFilter) |
| `segments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_1_Segment_1` (Transform) |
| `segments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_1_Segment_2` (Transform) |
| `segments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_2_Segment_1` (Transform) |
| `segments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_2_Segment_2` (Transform) |
| `segments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_3_Segment_1` (Transform) |
| `segments.Array.data[5]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_3_Segment_2` (Transform) |
| `segments.Array.data[6]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_4_Segment_1` (Transform) |
| `segments.Array.data[7]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_4_Segment_2` (Transform) |
| `segments.Array.data[8]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_5_Segment_1` (Transform) |
| `segments.Array.data[9]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/R46_String_5_Segment_2` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_2` → `DieuKhienConRoi`

Script: `Assets/Scripts/ConRoi/DieuKhienConRoi.cs` — tên cũ `ConRoiAI`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerTransform` | `Minh_RegionalWalk_180cm` (Transform) |
| `dauTransform` | `StoryProps/CR_Market/ConRoi_Minh_1_2/Minh_Head_Pivot` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_2` → `DienXuatConRoiV40`

Script: `Assets/Scripts/ConRoi/DienXuatConRoiV40.cs` — tên cũ `PuppetPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Market/ConRoi_Minh_1_2` (DieuKhienConRoi) |
| `articulation` | `StoryProps/CR_Market/ConRoi_Minh_1_2` (Animation) |

## `StoryProps/CR_Market/ConRoi_Minh_1_2` → `TiepDatConRoiV41`

Script: `Assets/Scripts/ConRoi/TiepDatConRoiV41.cs` — tên cũ `PuppetGroundingV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `supportR46` | `LakeGateMarketV32/Context_124` (MeshCollider) |

## `StoryProps/CR_Market/ConRoi_Minh_1_2` → `BuocChanConRoiR23`

Script: `Assets/Scripts/ConRoi/BuocChanConRoiR23.cs` — tên cũ `PuppetStrideR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Market/ConRoi_Minh_1_2` (DieuKhienConRoi) |
| `trajectory` | `Assets/Art/Characters/R23/WoodenStep.json` (TextAsset) |
| `supportR46` | `LakeGateMarketV32/Context_124` (MeshCollider) |
| `fittedLegsR91.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R91_Fitted_Leg_L` (Transform) |
| `fittedLegsR91.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R91_Fitted_Leg_R` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_2` → `DayConRoiR45`

Script: `Assets/Scripts/ConRoi/DayConRoiR45.cs` — tên cũ `PuppetThreadsR45`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `attachments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_L/ConRoi_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_R/ConRoi_Leg_R` (MeshFilter) |
| `threads.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_1_NoiTam` (Transform) |
| `threads.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_2_XaHoi` (Transform) |
| `threads.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_3_HeThong` (Transform) |
| `threads.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_4_CaNhan` (Transform) |
| `threads.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_5_HienSinh` (Transform) |
| `head` | `StoryProps/CR_Market/ConRoi_Minh_1_2/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshRenderer) |

## `StoryProps/CR_Market/ConRoi_Minh_1_2` → `DayConRoiR46`

Script: `Assets/Scripts/ConRoi/DayConRoiR46.cs` — tên cũ `PuppetThreadsR46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `sourceBindings` | `Assets/Art/Characters/R46/ThreadBindings.json` (TextAsset) |
| `fittedRoutingR91` | `Assets/Art/Characters/R91/ThreadRoutingR91.json` (TextAsset) |
| `attachments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R91_Fitted_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R91_Fitted_Leg_R` (MeshFilter) |
| `segments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_1_Segment_1` (Transform) |
| `segments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_1_Segment_2` (Transform) |
| `segments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_2_Segment_1` (Transform) |
| `segments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_2_Segment_2` (Transform) |
| `segments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_3_Segment_1` (Transform) |
| `segments.Array.data[5]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_3_Segment_2` (Transform) |
| `segments.Array.data[6]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_4_Segment_1` (Transform) |
| `segments.Array.data[7]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_4_Segment_2` (Transform) |
| `segments.Array.data[8]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_5_Segment_1` (Transform) |
| `segments.Array.data[9]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/R46_String_5_Segment_2` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_3` → `DieuKhienConRoi`

Script: `Assets/Scripts/ConRoi/DieuKhienConRoi.cs` — tên cũ `ConRoiAI`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerTransform` | `Minh_RegionalWalk_180cm` (Transform) |
| `dauTransform` | `StoryProps/CR_Market/ConRoi_Minh_1_3/Minh_Head_Pivot` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_3` → `DienXuatConRoiV40`

Script: `Assets/Scripts/ConRoi/DienXuatConRoiV40.cs` — tên cũ `PuppetPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Market/ConRoi_Minh_1_3` (DieuKhienConRoi) |
| `articulation` | `StoryProps/CR_Market/ConRoi_Minh_1_3` (Animation) |

## `StoryProps/CR_Market/ConRoi_Minh_1_3` → `TiepDatConRoiV41`

Script: `Assets/Scripts/ConRoi/TiepDatConRoiV41.cs` — tên cũ `PuppetGroundingV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `supportR46` | `LakeGateMarketV32/Context_124` (MeshCollider) |

## `StoryProps/CR_Market/ConRoi_Minh_1_3` → `BuocChanConRoiR23`

Script: `Assets/Scripts/ConRoi/BuocChanConRoiR23.cs` — tên cũ `PuppetStrideR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `puppet` | `StoryProps/CR_Market/ConRoi_Minh_1_3` (DieuKhienConRoi) |
| `trajectory` | `Assets/Art/Characters/R23/WoodenStep.json` (TextAsset) |
| `supportR46` | `LakeGateMarketV32/Context_124` (MeshCollider) |
| `fittedLegsR91.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R91_Fitted_Leg_L` (Transform) |
| `fittedLegsR91.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R91_Fitted_Leg_R` (Transform) |

## `StoryProps/CR_Market/ConRoi_Minh_1_3` → `DayConRoiR45`

Script: `Assets/Scripts/ConRoi/DayConRoiR45.cs` — tên cũ `PuppetThreadsR45`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `attachments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_L/ConRoi_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_R/ConRoi_Leg_R` (MeshFilter) |
| `threads.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_1_NoiTam` (Transform) |
| `threads.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_2_XaHoi` (Transform) |
| `threads.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_3_HeThong` (Transform) |
| `threads.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_4_CaNhan` (Transform) |
| `threads.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_5_HienSinh` (Transform) |
| `head` | `StoryProps/CR_Market/ConRoi_Minh_1_3/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshRenderer) |

## `StoryProps/CR_Market/ConRoi_Minh_1_3` → `DayConRoiR46`

Script: `Assets/Scripts/ConRoi/DayConRoiR46.cs` — tên cũ `PuppetThreadsR46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `sourceBindings` | `Assets/Art/Characters/R46/ThreadBindings.json` (TextAsset) |
| `fittedRoutingR91` | `Assets/Art/Characters/R91/ThreadRoutingR91.json` (TextAsset) |
| `attachments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R91_Fitted_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R91_Fitted_Leg_R` (MeshFilter) |
| `segments.Array.data[0]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_1_Segment_1` (Transform) |
| `segments.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_1_Segment_2` (Transform) |
| `segments.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_2_Segment_1` (Transform) |
| `segments.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_2_Segment_2` (Transform) |
| `segments.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_3_Segment_1` (Transform) |
| `segments.Array.data[5]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_3_Segment_2` (Transform) |
| `segments.Array.data[6]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_4_Segment_1` (Transform) |
| `segments.Array.data[7]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_4_Segment_2` (Transform) |
| `segments.Array.data[8]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_5_Segment_1` (Transform) |
| `segments.Array.data[9]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/R46_String_5_Segment_2` (Transform) |

## `StoryProps/CR_Market/R35_Roi_tai_quay` → `DayConRoiR45`

Script: `Assets/Scripts/ConRoi/DayConRoiR45.cs` — tên cũ `PuppetThreadsR45`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `attachments.Array.data[0]` | `StoryProps/CR_Market/R35_Roi_tai_quay/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/R35_Roi_tai_quay/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/R35_Roi_tai_quay/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/R35_Roi_tai_quay/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_L/ConRoi_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/R35_Roi_tai_quay/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Leg_R/ConRoi_Leg_R` (MeshFilter) |
| `threads.Array.data[0]` | `StoryProps/CR_Market/R35_Roi_tai_quay/ConRoi_String_1_NoiTam` (Transform) |
| `threads.Array.data[1]` | `StoryProps/CR_Market/R35_Roi_tai_quay/ConRoi_String_2_XaHoi` (Transform) |
| `threads.Array.data[2]` | `StoryProps/CR_Market/R35_Roi_tai_quay/ConRoi_String_3_HeThong` (Transform) |
| `threads.Array.data[3]` | `StoryProps/CR_Market/R35_Roi_tai_quay/ConRoi_String_4_CaNhan` (Transform) |
| `threads.Array.data[4]` | `StoryProps/CR_Market/R35_Roi_tai_quay/ConRoi_String_5_HienSinh` (Transform) |
| `head` | `StoryProps/CR_Market/R35_Roi_tai_quay/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshRenderer) |

## `StoryProps/CR_Market/R35_Roi_tai_quay` → `DayConRoiR46`

Script: `Assets/Scripts/ConRoi/DayConRoiR46.cs` — tên cũ `PuppetThreadsR46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `sourceBindings` | `Assets/Art/Characters/R46/ThreadBindings.json` (TextAsset) |
| `fittedRoutingR91` | `Assets/Art/Characters/R91/ThreadRoutingR91.json` (TextAsset) |
| `attachments.Array.data[0]` | `StoryProps/CR_Market/R35_Roi_tai_quay/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `StoryProps/CR_Market/R35_Roi_tai_quay/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_L/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `StoryProps/CR_Market/R35_Roi_tai_quay/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R/ConRoi_Arm_R` (MeshFilter) |
| `attachments.Array.data[3]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R91_Fitted_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R91_Fitted_Leg_R` (MeshFilter) |
| `segments.Array.data[0]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_1_Segment_1` (Transform) |
| `segments.Array.data[1]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_1_Segment_2` (Transform) |
| `segments.Array.data[2]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_2_Segment_1` (Transform) |
| `segments.Array.data[3]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_2_Segment_2` (Transform) |
| `segments.Array.data[4]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_3_Segment_1` (Transform) |
| `segments.Array.data[5]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_3_Segment_2` (Transform) |
| `segments.Array.data[6]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_4_Segment_1` (Transform) |
| `segments.Array.data[7]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_4_Segment_2` (Transform) |
| `segments.Array.data[8]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_5_Segment_1` (Transform) |
| `segments.Array.data[9]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R46_String_5_Segment_2` (Transform) |

## `StoryProps/CR_Market/R35_Roi_tai_quay` → `BuocChanConRoiR23`

Script: `Assets/Scripts/ConRoi/BuocChanConRoiR23.cs` — tên cũ `PuppetStrideR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `trajectory` | `Assets/Art/Characters/R23/WoodenStep.json` (TextAsset) |
| `supportR46` | `LakeGateMarketV32/Context_112` (MeshCollider) |
| `fittedLegsR91.Array.data[0]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R91_Fitted_Leg_L` (Transform) |
| `fittedLegsR91.Array.data[1]` | `StoryProps/CR_Market/R35_Roi_tai_quay/R91_Fitted_Leg_R` (Transform) |

## `StoryProps/CR_Market/R35_Roi_tai_quay/Mimic_package_socket/Goi_canh_phieu/Khan_that` → `KhanTrongGoiHangR35`

Script: `Assets/Scripts/TuongTac/KhanTrongGoiHangR35.cs` — tên cũ `MarketScarfR35`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_canh_phieu` → `GoiHangChoR35`

Script: `Assets/Scripts/TuongTac/GoiHangChoR35.cs` — tên cũ `MarketParcelR35`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (QuanLyGoiHangChoR35) |
| `lid` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_canh_phieu/Hinged_lid` (Transform) |
| `scarf` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_canh_phieu/Khan_that` (KhanTrongGoiHangR35) |
| `label` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_canh_phieu/Hinged_lid/Cung_nhan` (TextMesh) |
| `inside` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_canh_phieu/Khan_that/Mau_khan` (TextMesh) |

## `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_canh_phieu/Khan_that` → `KhanTrongGoiHangR35`

Script: `Assets/Scripts/TuongTac/KhanTrongGoiHangR35.cs` — tên cũ `MarketScarfR35`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_khoang_ben` → `GoiHangChoR35`

Script: `Assets/Scripts/TuongTac/GoiHangChoR35.cs` — tên cũ `MarketParcelR35`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (QuanLyGoiHangChoR35) |
| `lid` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_khoang_ben/Hinged_lid` (Transform) |
| `scarf` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_khoang_ben/Khan_that` (KhanTrongGoiHangR35) |
| `label` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_khoang_ben/Hinged_lid/Cung_nhan` (TextMesh) |
| `inside` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_khoang_ben/Khan_that/Mau_khan` (TextMesh) |

## `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_khoang_ben/Khan_that` → `KhanTrongGoiHangR35`

Script: `Assets/Scripts/TuongTac/KhanTrongGoiHangR35.cs` — tên cũ `MarketScarfR35`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/O_giao_hang` → `DichTuongTacChoR35`

Script: `Assets/Scripts/TuongTac/DichTuongTacChoR35.cs` — tên cũ `MarketTargetR35`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (QuanLyGoiHangChoR35) |

## `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Phieu_ghi_hang` → `DichTuongTacChoR35`

Script: `Assets/Scripts/TuongTac/DichTuongTacChoR35.cs` — tên cũ `MarketTargetR35`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (QuanLyGoiHangChoR35) |

## `Lake_Market_Story_GDD21` → `DieuPhoiHanhTrinh`

Script: `Assets/Scripts/KhoiDong/DieuPhoiHanhTrinh.cs` — tên cũ `LakeMarketStory`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `player` | `Minh_RegionalWalk_180cm` (DieuKhienDiChuyen) |
| `hub` | `StoryProps/CR_Hub` (GameObject) |
| `lake` | `StoryProps/CR_Lake` (GameObject) |
| `market` | `StoryProps/CR_Market` (GameObject) |
| `schoolChapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |
| `slopeChapter` | `Lake_Market_Story_GDD21` (ChuongDocV39) |
| `presentation` | `Lake_Market_Story_GDD21` (TrinhBayHanhTrinhV39) |
| `polish` | `Lake_Market_Story_GDD21` (HoanThienTrinhBayV40) |
| `completion` | `Lake_Market_Story_GDD21` (HoanTatHanhTrinhV41) |
| `hubSpawn` | `Hub_Story_Spawn` (Transform) |
| `lakeSpawn` | `Lake_Story_Spawn` (Transform) |
| `marketSpawn` | `Market_Story_Spawn` (Transform) |
| `traces.Array.data[0]` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/R11_Calendar_Complete_Assembly/CR_Calendar/CR_Calendar_Page` (Transform) |
| `traces.Array.data[1]` | `StoryProps/CR_Market/CR_Books_Unopened` (Transform) |
| `ghosts.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao` (Transform) |
| `ghosts.Array.data[1]` | `StoryProps/CR_Market/CR_Ha` (Transform) |
| `puppets.Array.data[0]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0` (DieuKhienConRoi) |
| `puppets.Array.data[1]` | `StoryProps/CR_Market/ConRoi_Minh_1_0` (DieuKhienConRoi) |
| `puppets.Array.data[2]` | `StoryProps/CR_Market/ConRoi_Minh_1_1` (DieuKhienConRoi) |
| `puppets.Array.data[3]` | `StoryProps/CR_Market/ConRoi_Minh_1_2` (DieuKhienConRoi) |
| `puppets.Array.data[4]` | `StoryProps/CR_Market/ConRoi_Minh_1_3` (DieuKhienConRoi) |
| `ambience.Array.data[0]` | `Assets/ConRoiGame/Audio/Phòng trọ.wav` (AudioClip) |
| `ambience.Array.data[1]` | `Assets/ConRoiGame/Audio/Gió mặt hồ.wav` (AudioClip) |
| `ambience.Array.data[2]` | `Assets/ConRoiGame/Audio/Chợ vắng.wav` (AudioClip) |
| `ambience.Array.data[3]` | `Assets/ConRoiGame/Audio/Đồng hồ lớp.wav` (AudioClip) |
| `ambience.Array.data[4]` | `Assets/ConRoiGame/Audio/Gió trên dốc.wav` (AudioClip) |
| `wood` | `Assets/ConRoiGame/Audio/Gỗ va.wav` (AudioClip) |
| `notification` | `Assets/ConRoiGame/Audio/Thông báo.wav` (AudioClip) |
| `drone` | `Assets/ConRoiGame/Audio/Drone.wav` (AudioClip) |
| `clockHand` | `StoryProps/CR_Hub/CR_Clock/CR_Clock_Hand` (Transform) |

## `Lake_Market_Story_GDD21` → `PhanChieuMatHo`

Script: `Assets/Scripts/MoiTruong/PhanChieuMatHo.cs` — tên cũ `LakePlanarReflection`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerCamera` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `water` | `R20_Whole_Source_Export/Surface_051` (MeshRenderer) |
| `notebook` | `Lake_Market_Story_GDD21` (TuongTacSoTayHoR37) |

## `Lake_Market_Story_GDD21` → `ChuongTruongHocV37`

Script: `Assets/Scripts/KhoiDong/ChuongTruongHocV37.cs` — tên cũ `SchoolChapterV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `environment` | `School_V37_Classroom_Courtyard` (GameObject) |
| `spawn` | `School_V37_Classroom_Courtyard/School_Spawn` (Transform) |
| `chair` | `School_V37_Classroom_Courtyard/R28_School/Ghe_Giao_Vien_R28` (Transform) |
| `ghost` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher` (Transform) |
| `clockHand` | `School_V37_Classroom_Courtyard/R28_School/203_ClockHand` (Transform) |
| `puppet` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3` (DieuKhienConRoi) |
| `face` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher` (HienGuongMatTheoKhu) |
| `tick` | `Assets/ConRoiGame/Audio/Đồng hồ lớp.wav` (AudioClip) |

## `Lake_Market_Story_GDD21` → `ChuongDocV39`

Script: `Assets/Scripts/KhoiDong/ChuongDocV39.cs` — tên cũ `SlopeChapterV39`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `environment` | `DaLat_Slope_V39` (GameObject) |
| `spawn` | `DaLat_Slope_V39/Slope_Spawn` (Transform) |
| `suitcase` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase` (Transform) |
| `ghost` | `DaLat_Slope_V39/AnhTung_Cousin` (Transform) |
| `face` | `DaLat_Slope_V39/AnhTung_Cousin` (HienGuongMatTheoKhu) |
| `puppet` | `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly` (DieuKhienConRoi) |
| `rollingWood` | `DaLat_Slope_V39/Distant_Wooden_Spool` (Transform) |
| `rollingSound` | `Assets/ConRoiGame/Audio/Gỗ va.wav` (AudioClip) |

## `Lake_Market_Story_GDD21` → `TrinhBayHanhTrinhV39`

Script: `Assets/Scripts/GiaoDien/TrinhBayHanhTrinhV39.cs` — tên cũ `JourneyPresentationV39`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `wire` | `Assets/ConRoiGame/Audio/V39_Wire.wav` (AudioClip) |
| `step` | `Assets/ConRoiGame/Audio/V39_Step.wav` (AudioClip) |
| `strings.Array.data[0]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_1_NoiTam` (MeshRenderer) |
| `strings.Array.data[1]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_2_XaHoi` (MeshRenderer) |
| `strings.Array.data[2]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_3_HeThong` (MeshRenderer) |
| `strings.Array.data[3]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_4_CaNhan` (MeshRenderer) |
| `strings.Array.data[4]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_5_HienSinh` (MeshRenderer) |
| `strings.Array.data[5]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_1_NoiTam` (MeshRenderer) |
| `strings.Array.data[6]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_2_XaHoi` (MeshRenderer) |
| `strings.Array.data[7]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_3_HeThong` (MeshRenderer) |
| `strings.Array.data[8]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_4_CaNhan` (MeshRenderer) |
| `strings.Array.data[9]` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/ConRoi_String_5_HienSinh` (MeshRenderer) |
| `strings.Array.data[10]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_1_NoiTam` (MeshRenderer) |
| `strings.Array.data[11]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_2_XaHoi` (MeshRenderer) |
| `strings.Array.data[12]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_3_HeThong` (MeshRenderer) |
| `strings.Array.data[13]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_4_CaNhan` (MeshRenderer) |
| `strings.Array.data[14]` | `StoryProps/CR_Market/ConRoi_Minh_1_0/ConRoi_String_5_HienSinh` (MeshRenderer) |
| `strings.Array.data[15]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_1_NoiTam` (MeshRenderer) |
| `strings.Array.data[16]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_2_XaHoi` (MeshRenderer) |
| `strings.Array.data[17]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_3_HeThong` (MeshRenderer) |
| `strings.Array.data[18]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_4_CaNhan` (MeshRenderer) |
| `strings.Array.data[19]` | `StoryProps/CR_Market/ConRoi_Minh_1_1/ConRoi_String_5_HienSinh` (MeshRenderer) |
| `strings.Array.data[20]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_1_NoiTam` (MeshRenderer) |
| `strings.Array.data[21]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_2_XaHoi` (MeshRenderer) |
| `strings.Array.data[22]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_3_HeThong` (MeshRenderer) |
| `strings.Array.data[23]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_4_CaNhan` (MeshRenderer) |
| `strings.Array.data[24]` | `StoryProps/CR_Market/ConRoi_Minh_1_2/ConRoi_String_5_HienSinh` (MeshRenderer) |
| `strings.Array.data[25]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_1_NoiTam` (MeshRenderer) |
| `strings.Array.data[26]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_2_XaHoi` (MeshRenderer) |
| `strings.Array.data[27]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_3_HeThong` (MeshRenderer) |
| `strings.Array.data[28]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_4_CaNhan` (MeshRenderer) |
| `strings.Array.data[29]` | `StoryProps/CR_Market/ConRoi_Minh_1_3/ConRoi_String_5_HienSinh` (MeshRenderer) |
| `strings.Array.data[30]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/ConRoi_String_1_NoiTam` (MeshRenderer) |
| `strings.Array.data[31]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/ConRoi_String_2_XaHoi` (MeshRenderer) |
| `strings.Array.data[32]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/ConRoi_String_3_HeThong` (MeshRenderer) |
| `strings.Array.data[33]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/ConRoi_String_4_CaNhan` (MeshRenderer) |
| `strings.Array.data[34]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/ConRoi_String_5_HienSinh` (MeshRenderer) |
| `strings.Array.data[35]` | `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly/ConRoi_String_1_NoiTam` (MeshRenderer) |
| `strings.Array.data[36]` | `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly/ConRoi_String_2_XaHoi` (MeshRenderer) |
| `strings.Array.data[37]` | `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly/ConRoi_String_3_HeThong` (MeshRenderer) |
| `strings.Array.data[38]` | `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly/ConRoi_String_4_CaNhan` (MeshRenderer) |
| `strings.Array.data[39]` | `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly/ConRoi_String_5_HienSinh` (MeshRenderer) |
| `strings.Array.data[40]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_1_NoiTam_Dream_Repaired` (MeshRenderer) |
| `strings.Array.data[41]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_2_XaHoi_Dream_Repaired` (MeshRenderer) |
| `strings.Array.data[42]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_3_HeThong_Dream_Repaired` (MeshRenderer) |
| `strings.Array.data[43]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_4_CaNhan_Dream_Repaired` (MeshRenderer) |
| `strings.Array.data[44]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/ConRoi_String_5_HienSinh_Dream_Repaired` (MeshRenderer) |
| `strings.Array.data[45]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_1_0` (MeshRenderer) |
| `strings.Array.data[46]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_1_1` (MeshRenderer) |
| `strings.Array.data[47]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_2_0` (MeshRenderer) |
| `strings.Array.data[48]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_2_1` (MeshRenderer) |
| `strings.Array.data[49]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_3_0` (MeshRenderer) |
| `strings.Array.data[50]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_3_1` (MeshRenderer) |
| `strings.Array.data[51]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_4_0` (MeshRenderer) |
| `strings.Array.data[52]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_4_1` (MeshRenderer) |
| `strings.Array.data[53]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_5_0` (MeshRenderer) |
| `strings.Array.data[54]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_5_1` (MeshRenderer) |
| `endingPhone` | `StoryProps/CR_Hub/CR_Phone` (Transform) |
| `endingJournal` | `StoryProps/CR_Hub/Ending_Open_Journal` (Transform) |

## `Lake_Market_Story_GDD21` → `HoanThienTrinhBayV40`

Script: `Assets/Scripts/GiaoDien/HoanThienTrinhBayV40.cs` — tên cũ `JourneyPolishV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `endingFocus.Array.data[0]` | `StoryProps/CR_Hub/Ending_Composition_0` (Transform) |
| `endingFocus.Array.data[1]` | `StoryProps/CR_Hub/Ending_Composition_1` (Transform) |
| `endingFocus.Array.data[2]` | `StoryProps/CR_Hub/Ending_Composition_2` (Transform) |
| `endingFocus.Array.data[3]` | `StoryProps/CR_Hub/Ending_Composition_3` (Transform) |
| `gardenFenceStages.Array.data[0]` | `StoryProps/CR_Hub/Lan_Outside_Garden/Lan_Fence_Added_0` (GameObject) |
| `gardenFenceStages.Array.data[1]` | `StoryProps/CR_Hub/Lan_Outside_Garden/Lan_Fence_Added_1` (GameObject) |
| `gardenFenceStages.Array.data[2]` | `StoryProps/CR_Hub/Lan_Outside_Garden/Lan_Fence_Added_2` (GameObject) |
| `gardenFenceStages.Array.data[3]` | `StoryProps/CR_Hub/Lan_Outside_Garden/Lan_Fence_Added_3` (GameObject) |
| `pageTurn` | `Assets/Art/Environment/JourneyV40/PageTurn.wav` (AudioClip) |
| `receiver` | `Assets/Art/Environment/JourneyV40/Receiver.wav` (AudioClip) |

## `Lake_Market_Story_GDD21` → `HoanTatHanhTrinhV41`

Script: `Assets/Scripts/KhoiDong/HoanTatHanhTrinhV41.cs` — tên cũ `JourneyCompletionV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `vy` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig` (Transform) |
| `lan` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig` (Transform) |
| `lanArm` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Elbow_Pivot/Ha_Sorting_Shoulder/Lan_Watering_Shoulder` (Transform) |
| `baoArm` | `StoryProps/CR_Lake/CR_Bao/V40_Elbow_Pivot/Bao_Rod_Shoulder` (Transform) |
| `haArm` | `StoryProps/CR_Market/CR_Ha/V40_Elbow_Pivot/Ha_Sorting_Shoulder` (Transform) |
| `paperRustle` | `Assets/Art/Environment/JourneyV41/PaperRustle.wav` (AudioClip) |
| `cloth` | `Assets/Art/Environment/JourneyV41/ParcelCloth.wav` (AudioClip) |
| `watering` | `Assets/Art/Environment/JourneyV41/Watering.wav` (AudioClip) |
| `lakeReflection` | `StoryProps/CR_Lake/V41_Lake_Details/V41_Practical` (Light) |
| `bookLight` | `V41_Market_Craft/V41_Practical` (Light) |
| `traceDetails.Array.data[0]` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/R11_Calendar_Complete_Assembly/CR_Calendar/CR_Calendar_Page` (Transform) |
| `traceDetails.Array.data[1]` | `StoryProps/CR_Market/CR_Books_Unopened` (Transform) |
| `traceDetails.Array.data[2]` | `School_V37_Classroom_Courtyard/R28_School/Ghe_Giao_Vien_R28` (Transform) |
| `traceDetails.Array.data[3]` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase` (Transform) |

## `Lake_Market_Story_GDD21` → `TraiNghiemHanhTrinhV42`

Script: `Assets/Scripts/KhoiDong/TraiNghiemHanhTrinhV42.cs` — tên cũ `JourneyExperienceV42`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `sortedParcel` | `StoryProps/CR_Market/Ha_Same_Wrapped_Parcel` (Transform) |

## `Lake_Market_Story_GDD21` → `DieuChinhHanhTrinhV43`

Script: `Assets/Scripts/KhoiDong/DieuChinhHanhTrinhV43.cs` — tên cũ `JourneyRevisionV43`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `upper.Array.data[0]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_UpperSleeve_0` (Transform) |
| `upper.Array.data[1]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_UpperSleeve_1` (Transform) |
| `lower.Array.data[0]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_ForeSleeve_0` (Transform) |
| `lower.Array.data[1]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_ForeSleeve_1` (Transform) |
| `hands.Array.data[0]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_Palm_0` (Transform) |
| `hands.Array.data[1]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_Palm_1` (Transform) |
| `haHead` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot` (Transform) |
| `parcel` | `StoryProps/CR_Market/Ha_Same_Wrapped_Parcel` (Transform) |

## `Lake_Market_Story_GDD21` → `ChanDoanDieuKhienV43`

Script: `Assets/Scripts/ChanDoan/ChanDoanDieuKhienV43.cs` — tên cũ `RuntimeControlProbeV43`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `ManHinhTaiV44`

Script: `Assets/Scripts/GiaoDien/ManHinhTaiV44.cs` — tên cũ `JourneyLoadingV44`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `DienXuatHanhTrinhV45`

Script: `Assets/Scripts/NhanVat/DienXuatHanhTrinhV45.cs` — tên cũ `JourneyActingV45`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `rigs.Array.data[0].actor` | `StoryProps/CR_Lake/CR_Bao` (Transform) |
| `rigs.Array.data[0].frame` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame` (Transform) |
| `rigs.Array.data[0].head` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot` (Transform) |
| `rigs.Array.data[0].torso` | `StoryProps/CR_Lake/CR_Bao/V40_Tailored_Silhouette/Tailored_Jacket` (Transform) |
| `rigs.Array.data[0].upper.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Upper_0` (Transform) |
| `rigs.Array.data[0].upper.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Upper_1` (Transform) |
| `rigs.Array.data[0].fore.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Fore_0` (Transform) |
| `rigs.Array.data[0].fore.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Fore_1` (Transform) |
| `rigs.Array.data[0].palm.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Palm_0` (Transform) |
| `rigs.Array.data[0].palm.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Palm_1` (Transform) |
| `rigs.Array.data[0].eyes.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead/Bao_Eye-1` (Transform) |
| `rigs.Array.data[0].eyes.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead/Bao_Eye1` (Transform) |
| `rigs.Array.data[0].brows.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead/Bao_Brow-1` (Transform) |
| `rigs.Array.data[0].brows.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead/Bao_Brow1` (Transform) |
| `rigs.Array.data[1].actor` | `StoryProps/CR_Market/CR_Ha` (Transform) |
| `rigs.Array.data[1].frame` | `StoryProps/CR_Market/CR_Ha/V45_Ha_Frame` (Transform) |
| `rigs.Array.data[1].head` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot` (Transform) |
| `rigs.Array.data[1].eyes.Array.data[0]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead/Ha_Eye-1` (Transform) |
| `rigs.Array.data[1].eyes.Array.data[1]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead/Ha_Eye1` (Transform) |
| `rigs.Array.data[1].brows.Array.data[0]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead/Ha_Brow-1` (Transform) |
| `rigs.Array.data[1].brows.Array.data[1]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead/Ha_Brow1` (Transform) |
| `rigs.Array.data[2].actor` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher` (Transform) |
| `rigs.Array.data[2].frame` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body` (Transform) |
| `rigs.Array.data[2].head` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot` (Transform) |
| `rigs.Array.data[2].torso` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/Mai_Fitted_Garment` (Transform) |
| `rigs.Array.data[2].upper.Array.data[0]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Upper_0` (Transform) |
| `rigs.Array.data[2].upper.Array.data[1]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Upper_1` (Transform) |
| `rigs.Array.data[2].fore.Array.data[0]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Fore_0` (Transform) |
| `rigs.Array.data[2].fore.Array.data[1]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Fore_1` (Transform) |
| `rigs.Array.data[2].palm.Array.data[0]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Palm_0` (Transform) |
| `rigs.Array.data[2].palm.Array.data[1]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Palm_1` (Transform) |
| `rigs.Array.data[2].eyes.Array.data[0]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead/Mai_Eye-1` (Transform) |
| `rigs.Array.data[2].eyes.Array.data[1]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead/Mai_Eye1` (Transform) |
| `rigs.Array.data[2].brows.Array.data[0]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead/Mai_Brow-1` (Transform) |
| `rigs.Array.data[2].brows.Array.data[1]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead/Mai_Brow1` (Transform) |
| `rigs.Array.data[3].actor` | `DaLat_Slope_V39/AnhTung_Cousin` (Transform) |
| `rigs.Array.data[3].frame` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body` (Transform) |
| `rigs.Array.data[3].head` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot` (Transform) |
| `rigs.Array.data[3].torso` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/Tung_Fitted_Garment` (Transform) |
| `rigs.Array.data[3].upper.Array.data[0]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Upper_0` (Transform) |
| `rigs.Array.data[3].upper.Array.data[1]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Upper_1` (Transform) |
| `rigs.Array.data[3].fore.Array.data[0]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Fore_0` (Transform) |
| `rigs.Array.data[3].fore.Array.data[1]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Fore_1` (Transform) |
| `rigs.Array.data[3].palm.Array.data[0]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Palm_0` (Transform) |
| `rigs.Array.data[3].palm.Array.data[1]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Palm_1` (Transform) |
| `rigs.Array.data[3].eyes.Array.data[0]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead/Tung_Eye-1` (Transform) |
| `rigs.Array.data[3].eyes.Array.data[1]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead/Tung_Eye1` (Transform) |
| `rigs.Array.data[3].brows.Array.data[0]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead/Tung_Brow-1` (Transform) |
| `rigs.Array.data[3].brows.Array.data[1]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead/Tung_Brow1` (Transform) |
| `rigs.Array.data[4].actor` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig` (Transform) |
| `rigs.Array.data[4].frame` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body` (Transform) |
| `rigs.Array.data[4].head` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead` (Transform) |
| `rigs.Array.data[4].torso` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/Lan_Fitted_Garment` (Transform) |
| `rigs.Array.data[4].upper.Array.data[0]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Upper_0` (Transform) |
| `rigs.Array.data[4].upper.Array.data[1]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Upper_1` (Transform) |
| `rigs.Array.data[4].fore.Array.data[0]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Fore_0` (Transform) |
| `rigs.Array.data[4].fore.Array.data[1]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Fore_1` (Transform) |
| `rigs.Array.data[4].palm.Array.data[0]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Palm_0` (Transform) |
| `rigs.Array.data[4].palm.Array.data[1]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Palm_1` (Transform) |
| `rigs.Array.data[4].eyes.Array.data[0]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead/Lan_Eye-1` (Transform) |
| `rigs.Array.data[4].eyes.Array.data[1]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead/Lan_Eye1` (Transform) |
| `rigs.Array.data[4].brows.Array.data[0]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead/Lan_Brow-1` (Transform) |
| `rigs.Array.data[4].brows.Array.data[1]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead/Lan_Brow1` (Transform) |
| `rigs.Array.data[5].actor` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig` (Transform) |
| `rigs.Array.data[5].frame` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body` (Transform) |
| `rigs.Array.data[5].head` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead` (Transform) |
| `rigs.Array.data[5].torso` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/Vy_Fitted_Garment` (Transform) |
| `rigs.Array.data[5].upper.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Upper_0` (Transform) |
| `rigs.Array.data[5].upper.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Upper_1` (Transform) |
| `rigs.Array.data[5].fore.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Fore_0` (Transform) |
| `rigs.Array.data[5].fore.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Fore_1` (Transform) |
| `rigs.Array.data[5].palm.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Palm_0` (Transform) |
| `rigs.Array.data[5].palm.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Palm_1` (Transform) |
| `rigs.Array.data[5].eyes.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead/Vy_Eye-1` (Transform) |
| `rigs.Array.data[5].eyes.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead/Vy_Eye1` (Transform) |
| `rigs.Array.data[5].brows.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead/Vy_Brow-1` (Transform) |
| `rigs.Array.data[5].brows.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead/Vy_Brow1` (Transform) |
| `rigs.Array.data[5].thighs.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Thigh_0` (Transform) |
| `rigs.Array.data[5].thighs.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Thigh_1` (Transform) |
| `rigs.Array.data[5].shins.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Shin_0` (Transform) |
| `rigs.Array.data[5].shins.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Shin_1` (Transform) |
| `rigs.Array.data[5].shoes.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/Vy_Leather_Shoe-1` (Transform) |
| `rigs.Array.data[5].shoes.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/Vy_Leather_Shoe1` (Transform) |
| `driven.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao` (Transform) |
| `driven.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot` (Transform) |
| `driven.Array.data[2]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead/Bao_Eye-1` (Transform) |
| `driven.Array.data[3]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead/Bao_Eye1` (Transform) |
| `driven.Array.data[4]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead/Bao_Brow-1` (Transform) |
| `driven.Array.data[5]` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead/Bao_Brow1` (Transform) |
| `driven.Array.data[6]` | `StoryProps/CR_Lake/CR_Bao/V40_Tailored_Silhouette/Tailored_Jacket` (Transform) |
| `driven.Array.data[7]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Upper_0` (Transform) |
| `driven.Array.data[8]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Fore_0` (Transform) |
| `driven.Array.data[9]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Palm_0` (Transform) |
| `driven.Array.data[10]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Upper_1` (Transform) |
| `driven.Array.data[11]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Fore_1` (Transform) |
| `driven.Array.data[12]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Palm_1` (Transform) |
| `driven.Array.data[13]` | `StoryProps/CR_Market/CR_Ha` (Transform) |
| `driven.Array.data[14]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot` (Transform) |
| `driven.Array.data[15]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead/Ha_Eye-1` (Transform) |
| `driven.Array.data[16]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead/Ha_Eye1` (Transform) |
| `driven.Array.data[17]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead/Ha_Brow-1` (Transform) |
| `driven.Array.data[18]` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead/Ha_Brow1` (Transform) |
| `driven.Array.data[19]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher` (Transform) |
| `driven.Array.data[20]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot` (Transform) |
| `driven.Array.data[21]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead/Mai_Eye-1` (Transform) |
| `driven.Array.data[22]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead/Mai_Eye1` (Transform) |
| `driven.Array.data[23]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead/Mai_Brow-1` (Transform) |
| `driven.Array.data[24]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead/Mai_Brow1` (Transform) |
| `driven.Array.data[25]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/Mai_Fitted_Garment` (Transform) |
| `driven.Array.data[26]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Upper_0` (Transform) |
| `driven.Array.data[27]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Fore_0` (Transform) |
| `driven.Array.data[28]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Palm_0` (Transform) |
| `driven.Array.data[29]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Upper_1` (Transform) |
| `driven.Array.data[30]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Fore_1` (Transform) |
| `driven.Array.data[31]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Palm_1` (Transform) |
| `driven.Array.data[32]` | `DaLat_Slope_V39/AnhTung_Cousin` (Transform) |
| `driven.Array.data[33]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot` (Transform) |
| `driven.Array.data[34]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead/Tung_Eye-1` (Transform) |
| `driven.Array.data[35]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead/Tung_Eye1` (Transform) |
| `driven.Array.data[36]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead/Tung_Brow-1` (Transform) |
| `driven.Array.data[37]` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead/Tung_Brow1` (Transform) |
| `driven.Array.data[38]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/Tung_Fitted_Garment` (Transform) |
| `driven.Array.data[39]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Upper_0` (Transform) |
| `driven.Array.data[40]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Fore_0` (Transform) |
| `driven.Array.data[41]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Palm_0` (Transform) |
| `driven.Array.data[42]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Upper_1` (Transform) |
| `driven.Array.data[43]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Fore_1` (Transform) |
| `driven.Array.data[44]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Palm_1` (Transform) |
| `driven.Array.data[45]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig` (Transform) |
| `driven.Array.data[46]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead` (Transform) |
| `driven.Array.data[47]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead/Lan_Eye-1` (Transform) |
| `driven.Array.data[48]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead/Lan_Eye1` (Transform) |
| `driven.Array.data[49]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead/Lan_Brow-1` (Transform) |
| `driven.Array.data[50]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead/Lan_Brow1` (Transform) |
| `driven.Array.data[51]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/Lan_Fitted_Garment` (Transform) |
| `driven.Array.data[52]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Upper_0` (Transform) |
| `driven.Array.data[53]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Fore_0` (Transform) |
| `driven.Array.data[54]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Palm_0` (Transform) |
| `driven.Array.data[55]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Upper_1` (Transform) |
| `driven.Array.data[56]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Fore_1` (Transform) |
| `driven.Array.data[57]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Palm_1` (Transform) |
| `driven.Array.data[58]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig` (Transform) |
| `driven.Array.data[59]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead` (Transform) |
| `driven.Array.data[60]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead/Vy_Eye-1` (Transform) |
| `driven.Array.data[61]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead/Vy_Eye1` (Transform) |
| `driven.Array.data[62]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead/Vy_Brow-1` (Transform) |
| `driven.Array.data[63]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead/Vy_Brow1` (Transform) |
| `driven.Array.data[64]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/Vy_Fitted_Garment` (Transform) |
| `driven.Array.data[65]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Upper_0` (Transform) |
| `driven.Array.data[66]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Fore_0` (Transform) |
| `driven.Array.data[67]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Palm_0` (Transform) |
| `driven.Array.data[68]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Upper_1` (Transform) |
| `driven.Array.data[69]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Fore_1` (Transform) |
| `driven.Array.data[70]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Palm_1` (Transform) |
| `driven.Array.data[71]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Thigh_0` (Transform) |
| `driven.Array.data[72]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Shin_0` (Transform) |
| `driven.Array.data[73]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/Vy_Leather_Shoe-1` (Transform) |
| `driven.Array.data[74]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Thigh_1` (Transform) |
| `driven.Array.data[75]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Shin_1` (Transform) |
| `driven.Array.data[76]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/Vy_Leather_Shoe1` (Transform) |
| `driven.Array.data[77]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V45_Lan_Handheld_Can` (Transform) |
| `driven.Array.data[78]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_UpperSleeve_0` (Transform) |
| `driven.Array.data[79]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_UpperSleeve_1` (Transform) |
| `driven.Array.data[80]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_ForeSleeve_0` (Transform) |
| `driven.Array.data[81]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_ForeSleeve_1` (Transform) |
| `driven.Array.data[82]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_Palm_0` (Transform) |
| `driven.Array.data[83]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_Palm_1` (Transform) |
| `driven.Array.data[84]` | `StoryProps/CR_Market/Ha_Same_Wrapped_Parcel` (Transform) |
| `driven.Array.data[85]` | `StoryProps/CR_Lake/CR_Bao/V40_Elbow_Pivot/Bao_Rod_Shoulder` (Transform) |
| `footClip` | `Assets/ConRoiGame/Audio/V39_Step.wav` (AudioClip) |
| `can` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V45_Lan_Handheld_Can` (Transform) |
| `spout` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V45_Lan_Handheld_Can/V45_Can_Spout` (Transform) |
| `rod` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/Bamboo_Fishing_Rod` (Transform) |

## `Lake_Market_Story_GDD21` → `GhiNhanKhungHinhTreV45`

Script: `Assets/Scripts/ChanDoan/GhiNhanKhungHinhTreV45.cs` — tên cũ `FrameHitchV45`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `BeMatNhanVatV46`

Script: `Assets/Scripts/NhanVat/BeMatNhanVatV46.cs` — tên cũ `HumanSurfaceV46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `acting` | `Lake_Market_Story_GDD21` (DienXuatHanhTrinhV45) |
| `bodies.Array.data[0].frame` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame` (Transform) |
| `bodies.Array.data[0].head` | `StoryProps/CR_Lake/CR_Bao/V40_Head_Gaze_Pivot/V44_Bao_SculptedHead` (Transform) |
| `bodies.Array.data[0].torso` | `StoryProps/CR_Lake/CR_Bao/V40_Tailored_Silhouette/Tailored_Jacket` (Transform) |
| `bodies.Array.data[0].upper.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Upper_0` (Transform) |
| `bodies.Array.data[0].upper.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Upper_1` (Transform) |
| `bodies.Array.data[0].lower.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Fore_0` (Transform) |
| `bodies.Array.data[0].lower.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Fore_1` (Transform) |
| `bodies.Array.data[0].hands.Array.data[0]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Palm_0` (Transform) |
| `bodies.Array.data[0].hands.Array.data[1]` | `StoryProps/CR_Lake/CR_Bao/V45_Bao_Frame/V45_Bao_Palm_1` (Transform) |
| `bodies.Array.data[0].cloth` | `Assets/Art/Environment/JourneyV42/Pine_Needle_Mid.mat` (Material) |
| `bodies.Array.data[0].skin` | `Assets/Art/Environment/JourneyV44/Bao_Skin.mat` (Material) |
| `bodies.Array.data[0].trousers` | `Assets/Art/Environment/JourneyV42/Pine_Needle_Mid.mat` (Material) |
| `bodies.Array.data[1].frame` | `StoryProps/CR_Market/CR_Ha/V45_Ha_Frame` (Transform) |
| `bodies.Array.data[1].head` | `StoryProps/CR_Market/CR_Ha/V40_Head_Gaze_Pivot/V44_Ha_SculptedHead` (Transform) |
| `bodies.Array.data[1].upper.Array.data[0]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_UpperSleeve_0` (Transform) |
| `bodies.Array.data[1].upper.Array.data[1]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_UpperSleeve_1` (Transform) |
| `bodies.Array.data[1].lower.Array.data[0]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_ForeSleeve_0` (Transform) |
| `bodies.Array.data[1].lower.Array.data[1]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_ForeSleeve_1` (Transform) |
| `bodies.Array.data[1].hands.Array.data[0]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_Palm_0` (Transform) |
| `bodies.Array.data[1].hands.Array.data[1]` | `StoryProps/CR_Market/CR_Ha/V43_Ha_Palm_1` (Transform) |
| `bodies.Array.data[1].cloth` | `Assets/Art/Environment/JourneyV40/Cloth_Chị Hà.mat` (Material) |
| `bodies.Array.data[1].skin` | `Assets/Art/Environment/JourneyV44/Ha_Skin.mat` (Material) |
| `bodies.Array.data[1].trousers` | `Assets/Art/Environment/JourneyV40/Cloth_Chị Hà.mat` (Material) |
| `bodies.Array.data[2].frame` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body` (Transform) |
| `bodies.Array.data[2].head` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot/V44_Mai_SculptedHead` (Transform) |
| `bodies.Array.data[2].torso` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/Mai_Fitted_Garment` (Transform) |
| `bodies.Array.data[2].upper.Array.data[0]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Upper_0` (Transform) |
| `bodies.Array.data[2].upper.Array.data[1]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Upper_1` (Transform) |
| `bodies.Array.data[2].lower.Array.data[0]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Fore_0` (Transform) |
| `bodies.Array.data[2].lower.Array.data[1]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Fore_1` (Transform) |
| `bodies.Array.data[2].hands.Array.data[0]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Palm_0` (Transform) |
| `bodies.Array.data[2].hands.Array.data[1]` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V44_Mai_Body/V45_Mai_Palm_1` (Transform) |
| `bodies.Array.data[2].cloth` | `Assets/Art/Environment/JourneyV44/Mai_Cloth.mat` (Material) |
| `bodies.Array.data[2].skin` | `Assets/Art/Environment/JourneyV44/Mai_Skin.mat` (Material) |
| `bodies.Array.data[2].trousers` | `Assets/Art/Environment/JourneyV44/Mai_Cloth.mat` (Material) |
| `bodies.Array.data[3].frame` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body` (Transform) |
| `bodies.Array.data[3].head` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot/V44_Tung_SculptedHead` (Transform) |
| `bodies.Array.data[3].torso` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/Tung_Fitted_Garment` (Transform) |
| `bodies.Array.data[3].upper.Array.data[0]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Upper_0` (Transform) |
| `bodies.Array.data[3].upper.Array.data[1]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Upper_1` (Transform) |
| `bodies.Array.data[3].lower.Array.data[0]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Fore_0` (Transform) |
| `bodies.Array.data[3].lower.Array.data[1]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Fore_1` (Transform) |
| `bodies.Array.data[3].hands.Array.data[0]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Palm_0` (Transform) |
| `bodies.Array.data[3].hands.Array.data[1]` | `DaLat_Slope_V39/AnhTung_Cousin/V44_Tung_Body/V45_Tung_Palm_1` (Transform) |
| `bodies.Array.data[3].cloth` | `Assets/Art/Environment/JourneyV44/Tung_Cloth.mat` (Material) |
| `bodies.Array.data[3].skin` | `Assets/Art/Environment/JourneyV44/Tung_Skin.mat` (Material) |
| `bodies.Array.data[3].trousers` | `Assets/Art/Environment/JourneyV44/Tung_Cloth.mat` (Material) |
| `bodies.Array.data[4].frame` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body` (Transform) |
| `bodies.Array.data[4].head` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/Lan_Garden/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Lan_SculptedHead` (Transform) |
| `bodies.Array.data[4].torso` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/Lan_Fitted_Garment` (Transform) |
| `bodies.Array.data[4].upper.Array.data[0]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Upper_0` (Transform) |
| `bodies.Array.data[4].upper.Array.data[1]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Upper_1` (Transform) |
| `bodies.Array.data[4].lower.Array.data[0]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Fore_0` (Transform) |
| `bodies.Array.data[4].lower.Array.data[1]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Fore_1` (Transform) |
| `bodies.Array.data[4].hands.Array.data[0]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Palm_0` (Transform) |
| `bodies.Array.data[4].hands.Array.data[1]` | `StoryProps/CR_Hub/V41_Lan_Garden_Performance/Lan_Garden_World_Rig/V44_Lan_Body/V45_Lan_Palm_1` (Transform) |
| `bodies.Array.data[4].cloth` | `Assets/Art/Environment/JourneyV44/Lan_Cloth.mat` (Material) |
| `bodies.Array.data[4].skin` | `Assets/Art/Environment/JourneyV44/Lan_Skin.mat` (Material) |
| `bodies.Array.data[4].trousers` | `Assets/Art/Environment/JourneyV44/Lan_Cloth.mat` (Material) |
| `bodies.Array.data[5].frame` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body` (Transform) |
| `bodies.Array.data[5].head` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/Vy_Ordinary_Classmate/V40_Head_Gaze_Pivot/CR_Ha_Head/V44_Vy_SculptedHead` (Transform) |
| `bodies.Array.data[5].torso` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/Vy_Fitted_Garment` (Transform) |
| `bodies.Array.data[5].upper.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Upper_0` (Transform) |
| `bodies.Array.data[5].upper.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Upper_1` (Transform) |
| `bodies.Array.data[5].lower.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Fore_0` (Transform) |
| `bodies.Array.data[5].lower.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Fore_1` (Transform) |
| `bodies.Array.data[5].hands.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Palm_0` (Transform) |
| `bodies.Array.data[5].hands.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Palm_1` (Transform) |
| `bodies.Array.data[5].thighs.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Thigh_0` (Transform) |
| `bodies.Array.data[5].thighs.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Thigh_1` (Transform) |
| `bodies.Array.data[5].shins.Array.data[0]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Shin_0` (Transform) |
| `bodies.Array.data[5].shins.Array.data[1]` | `V41_Market_Craft/Vy_Ordinary_Classmate_World_Rig/V44_Vy_Body/V45_Vy_Shin_1` (Transform) |
| `bodies.Array.data[5].cloth` | `Assets/Art/Environment/JourneyV44/Vy_Cloth.mat` (Material) |
| `bodies.Array.data[5].skin` | `Assets/Art/Environment/JourneyV44/Vy_Skin.mat` (Material) |
| `bodies.Array.data[5].trousers` | `Assets/Art/Environment/JourneyV44/Vy_LowerCloth.mat` (Material) |

## `Lake_Market_Story_GDD21` → `DoChiPhiTacVuV46`

Script: `Assets/Scripts/ChanDoan/DoChiPhiTacVuV46.cs` — tên cũ `TaskAttributionV46`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `Lake_Market_Story_GDD21` → `DoChiPhiTacVuV47`

Script: `Assets/Scripts/ChanDoan/DoChiPhiTacVuV47.cs` — tên cũ `TaskAttributionV47`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `Lake_Market_Story_GDD21` → `PhongTrongMoV47`

Script: `Assets/Scripts/KhoiDong/PhongTrongMoV47.cs` — tên cũ `DreamRoomV47`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `brokenStrings.Array.data[0]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Five_Broken_Cords` (MeshRenderer) |
| `repairedStrings.Array.data[0]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_1_0` (MeshRenderer) |
| `repairedStrings.Array.data[1]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_1_1` (MeshRenderer) |
| `repairedStrings.Array.data[2]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_2_0` (MeshRenderer) |
| `repairedStrings.Array.data[3]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_2_1` (MeshRenderer) |
| `repairedStrings.Array.data[4]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_3_0` (MeshRenderer) |
| `repairedStrings.Array.data[5]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_3_1` (MeshRenderer) |
| `repairedStrings.Array.data[6]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_4_0` (MeshRenderer) |
| `repairedStrings.Array.data[7]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_4_1` (MeshRenderer) |
| `repairedStrings.Array.data[8]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_5_0` (MeshRenderer) |
| `repairedStrings.Array.data[9]` | `StoryProps/CR_Hub/Desk_Puppet_Broken_Strings/R46_Dream_String_5_1` (MeshRenderer) |

## `Lake_Market_Story_GDD21` → `MenuVaoGameV47`

Script: `Assets/Scripts/GiaoDien/MenuVaoGameV47.cs` — tên cũ `PlayerEntryV47`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `GhiBangChungHienThiV47`

Script: `Assets/Scripts/ChanDoan/GhiBangChungHienThiV47.cs` — tên cũ `VisibleEvidenceV47`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `HuongDanHanhTrinhV48`

Script: `Assets/Scripts/GiaoDien/HuongDanHanhTrinhV48.cs` — tên cũ `JourneyGuideV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `CanhQuanChoV48`

Script: `Assets/Scripts/MoiTruong/CanhQuanChoV48.cs` — tên cũ `MarketSceneryV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `scenery.Array.data[0]` | `V41_Market_Craft` (GameObject) |
| `scenery.Array.data[1]` | `V41_Entrance_Lamp` (GameObject) |
| `scenery.Array.data[2]` | `V48_Closed_Outer_Streets` (GameObject) |
| `scenery.Array.data[3]` | `R11_Restored_Source_Market` (GameObject) |
| `scenery.Array.data[4]` | `R11_Local_Bridge_Supports` (GameObject) |
| `scenery.Array.data[5]` | `R11_Route_Wayfinding` (GameObject) |
| `scenery.Array.data[6]` | `R15_Local_Plinth_Foundations` (GameObject) |
| `scenery.Array.data[7]` | `R15_Completed_Source_Models` (GameObject) |
| `scenery.Array.data[8]` | `R20_Whole_Source_Export` (GameObject) |
| `scenery.Array.data[9]` | `R21_Planter_Sidewalk_Connections` (GameObject) |
| `scenery.Array.data[10]` | `R21_Paving_Construction_Edges` (GameObject) |
| `scenery.Array.data[11]` | `R21_Conformed_Authored_Bridge_Links` (GameObject) |
| `scenery.Array.data[12]` | `R21_SE_Promenade_Construction_Edges` (GameObject) |
| `scenery.Array.data[13]` | `R21_LakeMarket_Measured_Perimeter` (GameObject) |
| `scenery.Array.data[14]` | `R21_RealPlanterFlowerSample` (GameObject) |
| `scenery.Array.data[15]` | `R21_Boundary_Landscape_Finish` (GameObject) |

## `Lake_Market_Story_GDD21` → `GhiTrangThaiHinhAnhV48`

Script: `Assets/Scripts/ChanDoan/GhiTrangThaiHinhAnhV48.cs` — tên cũ `VisualStateV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `TuTheBaoBenCauV48`

Script: `Assets/Scripts/NhanVat/TuTheBaoBenCauV48.cs` — tên cũ `BaoBridgeV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `acting` | `Lake_Market_Story_GDD21` (DienXuatHanhTrinhV45) |
| `actor` | `StoryProps/CR_Lake/CR_Bao` (Transform) |
| `rod` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/Bamboo_Fishing_Rod` (Transform) |
| `line` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/Fishing_Line` (Transform) |
| `bag` | `StoryProps/CR_Lake/V41_Lake_Details/Bao_Fishing_Bag` (Transform) |
| `calendar` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/R11_Calendar_Complete_Assembly/CR_Calendar/CR_Calendar_Page` (Transform) |
| `reflectionLight` | `StoryProps/CR_Lake/V41_Lake_Details/V41_Practical` (Transform) |
| `leftElbowTarget` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/LeftForearmOnRail` (Transform) |
| `leftHandTarget` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/LeftHandOverWater` (Transform) |
| `rightElbowTarget` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/RightForearmOnRail` (Transform) |
| `rightHandTarget` | `StoryProps/CR_Lake/R11_Bao_Rail_Contact_Targets/RightHandRestingOverRail` (Transform) |

## `Lake_Market_Story_GDD21` → `SuongDaLatV48`

Script: `Assets/Scripts/MoiTruong/SuongDaLatV48.cs` — tên cũ `DalatMistV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `PhucHoiTaiBienR21`

Script: `Assets/Scripts/MoiTruong/PhucHoiTaiBienR21.cs` — tên cũ `BoundaryRecoveryR21`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `SuongKhongGianR22`

Script: `Assets/Scripts/MoiTruong/SuongKhongGianR22.cs` — tên cũ `SpatialMistR22`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `TraiNghiemHanhTrinhR23`

Script: `Assets/Scripts/KhoiDong/TraiNghiemHanhTrinhR23.cs` — tên cũ `JourneyExperienceR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `readableUi` | `Assets/Art/UI/R23/ReadableUI.mat` (Material) |

## `Lake_Market_Story_GDD21` → `MoDauBenHoR23`

Script: `Assets/Scripts/ConRoi/MoDauBenHoR23.cs` — tên cũ `LakeOpeningR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `preparationR89` | `Lake_Market_Story_GDD21` (TiepCanCoChuDichR89) |

## `Lake_Market_Story_GDD21` → `CuonChiDocR24`

Script: `Assets/Scripts/TuongTac/CuonChiDocR24.cs` — tên cũ `R24SlopeSpool`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongDocV39) |

## `Lake_Market_Story_GDD21` → `TraiNghiemDocR25`

Script: `Assets/Scripts/TuongTac/TraiNghiemDocR25.cs` — tên cũ `R25SlopeExperience`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `HoSoSceneR26`

Script: `Assets/Scripts/LuuTai/HoSoSceneR26.cs` — tên cũ `R26SceneProfile`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `pipeline` | `Assets/Art/Environment/JourneyV48/IntegrationR30/Pipeline.asset` (UniversalRenderPipelineAsset) |

## `Lake_Market_Story_GDD21` → `TraiNghiemTruongR28`

Script: `Assets/Scripts/TuongTac/TraiNghiemTruongR28.cs` — tên cũ `R28SchoolExperience`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `approachR89` | `Lake_Market_Story_GDD21` (TiepCanCoChuDichR89) |
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `puppetPaper` | `School_V37_Classroom_Courtyard/R28_School/Puppet_Paper` (Transform) |
| `responsePaper` | `School_V37_Classroom_Courtyard/R28_School/Player_Paper` (Transform) |
| `responseDesk` | `School_V37_Classroom_Courtyard/R28_School/Ban_Sinh_Vien_Response_R28` (Transform) |
| `puppetDesk` | `School_V37_Classroom_Courtyard/R28_School/Ban_Sinh_Vien_Puppet_R28` (Transform) |
| `traceChair` | `School_V37_Classroom_Courtyard/R28_School/Ghe_Giao_Vien_R28` (Transform) |
| `leadUpper` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/GD3_UpperArm` (Transform) |
| `leadFore` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/GD3_ForeArm` (Transform) |
| `leadHand` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/GD3_Hand` (Transform) |
| `leadShoulder` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/GD3_RightShoulder` (Transform) |
| `pushPivotR91` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean` (Transform) |
| `pushSourceR91` | `Assets/Art/Characters/R91/SchoolPushPoseR91.json` (TextAsset) |

## `Lake_Market_Story_GDD21` → `TuongTacGiayLop203R32`

Script: `Assets/Scripts/TuongTac/TuongTacGiayLop203R32.cs` — tên cũ `Class203PapersR32`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `staging` | `Lake_Market_Story_GDD21` (TraiNghiemTruongR28) |
| `papers.Array.data[0]` | `School_V37_Classroom_Courtyard/R28_School/Player_Paper` (Transform) |
| `papers.Array.data[1]` | `School_V37_Classroom_Courtyard/R28_School/Model_Answer_Paper_R32` (Transform) |
| `pages.Array.data[0]` | `Assets/Art/Gameplay/Class203R32/Minh.png` (Texture2D) |
| `pages.Array.data[1]` | `Assets/Art/Gameplay/Class203R32/Model.png` (Texture2D) |
| `tray` | `School_V37_Classroom_Courtyard/R28_School/R32_Minh_Tray_SafePoint` (Transform) |
| `outside` | `School_V37_Classroom_Courtyard/R28_School/R32_Outside_Tray_SafePoint` (Transform) |

## `Lake_Market_Story_GDD21` → `QuanLyGoiHangChoR35`

Script: `Assets/Scripts/TuongTac/QuanLyGoiHangChoR35.cs` — tên cũ `MarketPackagesR35`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `root` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau` (Transform) |
| `delivery` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/O_giao_hang` (Transform) |
| `slip` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Phieu_ghi_hang` (Transform) |
| `puppet` | `StoryProps/CR_Market/R35_Roi_tai_quay` (Transform) |
| `gesture` | `StoryProps/CR_Market/R35_Roi_tai_quay/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R` (Transform) |
| `mimicPackage` | `StoryProps/CR_Market/R35_Roi_tai_quay/Mimic_package_socket/Goi_canh_phieu` (Transform) |
| `mimicLid` | `StoryProps/CR_Market/R35_Roi_tai_quay/Mimic_package_socket/Goi_canh_phieu/Hinged_lid` (Transform) |
| `parcels.Array.data[0]` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_canh_phieu` (GoiHangChoR35) |
| `parcels.Array.data[1]` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Goi_khoang_ben` (GoiHangChoR35) |
| `placementPreview` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/Placement_preview` (GameObject) |

## `Lake_Market_Story_GDD21` → `HoSoLuuTaiR35`

Script: `Assets/Scripts/LuuTai/HoSoLuuTaiR35.cs` — tên cũ `R35SceneProfile`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `TuongTacSoTayHoR37`

Script: `Assets/Scripts/TuongTac/TuongTacSoTayHoR37.cs` — tên cũ `LakeNotebookR37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `root` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong` (Transform) |
| `book` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/So_cua_Minh` (Transform) |
| `leaf` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/So_cua_Minh/Bia_gap_quanh_gay` (Transform) |
| `leftPage` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/So_cua_Minh/Trang_trong` (Transform) |
| `socket` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/Book_safe_socket` (Transform) |
| `gesture` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/V40_Neutral_Articulation_Rig/V40_Wooden_Joint_ConRoi_Arm_R` (Transform) |
| `planText` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/So_cua_Minh/Dong_ke_hoach` (TextMesh) |
| `blankText` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/So_cua_Minh/Trang_trong/Tieu_de_trang_trong` (TextMesh) |
| `coverText` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/So_cua_Minh/Bia_gap_quanh_gay/Ten_Minh` (TextMesh) |
| `preview` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/Preview_dat_so` (GameObject) |
| `bookCollider` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/So_cua_Minh` (BoxCollider) |
| `paperSound` | `Assets/Art/Gameplay/LakeR37/Paper.wav` (AudioClip) |
| `closeSound` | `Assets/Art/Gameplay/LakeR37/Close.wav` (AudioClip) |
| `placeSound` | `Assets/Art/Gameplay/LakeR37/Place.wav` (AudioClip) |
| `preparationR89` | `Lake_Market_Story_GDD21` (TiepCanCoChuDichR89) |

## `Lake_Market_Story_GDD21` → `ChuanBiHanhLyR41`

Script: `Assets/Scripts/TuongTac/ChuanBiHanhLyR41.cs` — tên cũ `SlopePreparationsR41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `root` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau` (Transform) |
| `tag` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/The_hanh_ly_cu` (Transform) |
| `paper` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/To_chuan_bi_gap` (Transform) |
| `flap` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/To_chuan_bi_gap/Checklist_hinged_flap` (Transform) |
| `tagRest` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/Tag_original_front` (Transform) |
| `tagArranged` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/Tag_next_to_handle` (Transform) |
| `paperRest` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/Checklist_on_original_case` (Transform) |
| `tagCollider` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/The_hanh_ly_cu` (BoxCollider) |
| `paperCollider` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/To_chuan_bi_gap` (BoxCollider) |
| `tagFront` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/The_hanh_ly_cu/Tag_name` (TextMesh) |
| `tagBack` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/The_hanh_ly_cu/Tag_reverse` (TextMesh) |
| `paperInside` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/To_chuan_bi_gap/Repeated_revisions` (TextMesh) |
| `paperCover` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/To_chuan_bi_gap/Checklist_hinged_flap/Folded_title` (TextMesh) |
| `preview` | `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/Preview_tag_arranged` (GameObject) |
| `paperSound` | `Assets/Art/Gameplay/LakeR37/Paper.wav` (AudioClip) |
| `placeSound` | `Assets/Art/Gameplay/LakeR37/Place.wav` (AudioClip) |

## `Lake_Market_Story_GDD21` → `DieuPhoiGiaoDienR42`

Script: `Assets/Scripts/GiaoDien/DieuPhoiGiaoDienR42.cs` — tên cũ `JourneyUiR42`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `Lake_Market_Story_GDD21` → `KhoiPhucCaoDoSauTaiR73`

Script: `Assets/Scripts/MoiTruong/KhoiPhucCaoDoSauTaiR73.cs` — tên cũ `RouteGradeR73`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `previousSupports.Array.data[0]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/PreviousSupport_0.asset` (Mesh) |
| `previousSupports.Array.data[1]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/PreviousSupport_1.asset` (Mesh) |
| `previousSupports.Array.data[2]` | `Assets/Art/Environment/JourneyV48/LandscapeR21/R21_ProjectedDense_Context_136.asset` (Mesh) |
| `previousSupports.Array.data[3]` | `Assets/Art/Environment/JourneyV48/CollisionJoinR58/Context_132_R58.asset` (Mesh) |
| `previousSupports.Array.data[4]` | `Assets/Art/Environment/JourneyV48/ForecourtR60/Context_106_R60.asset` (Mesh) |
| `previousSupports.Array.data[5]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/PreviousSupport_5.asset` (Mesh) |
| `previousSupports.Array.data[6]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/PreviousSupport_6.asset` (Mesh) |
| `previousSupports.Array.data[7]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/PreviousSupport_7.asset` (Mesh) |
| `previousSupports.Array.data[8]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/PreviousSupport_8.asset` (Mesh) |
| `previousSupports.Array.data[9]` | `Assets/Art/Environment/JourneyV48/RouteGradeR73/Context_132_R73.asset` (Mesh) |
| `previousSupports.Array.data[10]` | `Assets/Art/Environment/JourneyV48/RouteGradeR73/Context_106_R73.asset` (Mesh) |
| `previousSupports.Array.data[11]` | `Assets/Art/Environment/JourneyV48/RouteGradeR73/Context_136_R73.asset` (Mesh) |
| `previousSupports.Array.data[12]` | `Assets/Art/Environment/JourneyV48/ShoreRepairR50/Surface_142_R50.asset` (Mesh) |
| `previousSupports.Array.data[13]` | `Assets/Art/Environment/JourneyV48/LandscapeR21/R21_Conformed_Authored_Bridge_Links.asset` (Mesh) |
| `previousSupports.Array.data[14]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Surface_142_R74.asset` (Mesh) |
| `previousSupports.Array.data[15]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Context_124_R74.asset` (Mesh) |
| `previousSupports.Array.data[16]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Context_110_R74.asset` (Mesh) |
| `previousSupports.Array.data[17]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Context_111_R74.asset` (Mesh) |
| `previousSupports.Array.data[18]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR75/Previous_Context_116_CPUOnly.asset` (Mesh) |
| `previousSupports.Array.data[19]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/R21_Conformed_Authored_Bridge_Links_R74.asset` (Mesh) |
| `previousSupports.Array.data[20]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Context_132_R74.asset` (Mesh) |
| `previousSupports.Array.data[21]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR75/Context_110_R75.asset` (Mesh) |
| `previousSupports.Array.data[22]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Context_124_R76.asset` (Mesh) |
| `previousSupports.Array.data[23]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Context_136_R74.asset` (Mesh) |
| `previousSupports.Array.data[24]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Context_132_R76.asset` (Mesh) |
| `previousSupports.Array.data[25]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Context_106_R74.asset` (Mesh) |
| `previousSupports.Array.data[26]` | `Assets/Art/Environment/JourneyV48/MapRestoredR11/RestoredMarket.fbx` (Mesh) |
| `previousSupports.Array.data[27]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[28]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Context_133_R76.asset` (Mesh) |
| `previousSupports.Array.data[29]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Context_111_R76.asset` (Mesh) |
| `previousSupports.Array.data[30]` | `Assets/Art/Environment/JourneyV48/PlazaGradeR77/Context_136_R77_Bound.asset` (Mesh) |
| `previousSupports.Array.data[31]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[32]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Context_130_R76.asset` (Mesh) |
| `previousSupports.Array.data[33]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[34]` | `Assets/Art/Environment/JourneyV48/ContactFinishR21/R21_PavingEdge_OD35_Approach_Sidewalk_FaceNormals.asset` (Mesh) |
| `previousSupports.Array.data[35]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Context_110_R76.asset` (Mesh) |
| `previousSupports.Array.data[36]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[37]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Surface_142_R76.asset` (Mesh) |
| `previousSupports.Array.data[38]` | `Assets/Art/Environment/JourneyV48/ContactFinishR21/R21_PavingEdge_OD35_Approach_Sidewalk.002_FaceNormals.asset` (Mesh) |
| `previousSupports.Array.data[39]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Context_131_R76.asset` (Mesh) |
| `previousSupports.Array.data[40]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[41]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[42]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Context_108_R74.asset` (Mesh) |
| `previousSupports.Array.data[43]` | `Assets/Art/Environment/JourneyV48/PlazaGradeR77/Context_124_R77_Bound.asset` (Mesh) |
| `previousSupports.Array.data[44]` | `Assets/Art/Environment/JourneyV48/PlazaGradeR77/Context_132_R77_Bound.asset` (Mesh) |
| `previousSupports.Array.data[45]` | `Assets/Art/Environment/JourneyV48/PlazaGradeR77/Context_106_R77_Bound.asset` (Mesh) |
| `previousSupports.Array.data[46]` | `Assets/Art/Environment/JourneyV48/MapRestoredR11/RestoredMarket.fbx` (Mesh) |
| `previousSupports.Array.data[47]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[48]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[49]` | `Assets/Art/Environment/JourneyV48/ContactFinishR21/R21_PavingEdge_OD35_Main_Route_Side_Link_122_FaceNormals.asset` (Mesh) |
| `previousSupports.Array.data[50]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Context_116_R76.asset` (Mesh) |
| `previousSupports.Array.data[51]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/R21_Conformed_Authored_Bridge_Links_R76.asset` (Mesh) |
| `previousSupports.Array.data[52]` | `Assets/Art/Environment/JourneyV48/PlazaGradeR77/Restored_029_R77_Bound.asset` (Mesh) |
| `previousSupports.Array.data[53]` | `Assets/Art/Environment/JourneyV48/ContactFinishR21/R21_PavingEdge_OD35_Approach_Road_NW_FaceNormals.asset` (Mesh) |
| `previousSupports.Array.data[54]` | `Assets/Art/Environment/JourneyV48/ContactFinishR21/R21_PavingEdge_PlanterWalkConnection_2_FaceNormals.asset` (Mesh) |
| `previousSupports.Array.data[55]` | `Assets/Art/Environment/JourneyV48/MarketIslandR62/Island125314010_R62.fbx` (Mesh) |
| `previousSupports.Array.data[56]` | `Assets/Art/Environment/JourneyV48/LandscapeR21/PlanterWalkConnection_2.asset` (Mesh) |
| `previousSupports.Array.data[57]` | `Assets/Art/Environment/JourneyV48/RoundaboutsR88/Grade_036_R88_Bound.asset` (Mesh) |
| `previousSupports.Array.data[58]` | `Assets/Art/Environment/JourneyV48/RouteGradeR74/Surface_125_R74.asset` (Mesh) |
| `previousSupports.Array.data[59]` | `Assets/Art/Environment/JourneyV48/PlazaGradeR77/Surface_149_R77_Bound.asset` (Mesh) |
| `previousSupports.Array.data[60]` | `Assets/Art/Environment/JourneyV48/RouteSeamsR76/Surface_128_R76.asset` (Mesh) |
| `previousSupports.Array.data[61]` | `Assets/Art/Environment/JourneyV48/PlazaGradeR77/Surface_153_R77_Bound.asset` (Mesh) |

## `Lake_Market_Story_GDD21` → `ChuanBiGoiHangR88`

Script: `Assets/Scripts/TuongTac/ChuanBiGoiHangR88.cs` — tên cũ `MarketPreparationR88`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `activity` | `Lake_Market_Story_GDD21` (QuanLyGoiHangChoR35) |
| `stride` | `StoryProps/CR_Market/R35_Roi_tai_quay` (BuocChanConRoiR23) |
| `support` | `LakeGateMarketV32/Context_112` (MeshCollider) |
| `destination` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/R88_Prepare_Stop_Outside_ThroughRoute` (Transform) |
| `packageDestination` | `StoryProps/CR_Market/R35_Nhung_goi_hang_giong_nhau/R88_Package_Prepared_Home_Not_Delivery` (Transform) |
| `head` | `StoryProps/CR_Market/R35_Roi_tai_quay/Minh_Head_Pivot` (Transform) |

## `Lake_Market_Story_GDD21` → `TiepCanCoChuDichR89`

Script: `Assets/Scripts/ConRoi/TiepCanCoChuDichR89.cs` — tên cũ `PurposefulApproachR89`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `puppet` | `StoryProps/CR_Lake/ConRoi_Minh_0_0` (Transform) |
| `destination` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/R89_Lake_Approach_Destination` (Transform) |
| `focus` | `StoryProps/CR_Lake/R37_Dieu_minh_dat_xuong/Book_safe_socket` (Transform) |
| `head` | `StoryProps/CR_Lake/ConRoi_Minh_0_0/Minh_Head_Pivot` (Transform) |
| `stride` | `StoryProps/CR_Lake/ConRoi_Minh_0_0` (BuocChanConRoiR23) |
| `support` | `OngDao_V35_Photo_Batch/OD35_COLLIDER_3` (MeshCollider) |
| `notebook` | `Lake_Market_Story_GDD21` (TuongTacSoTayHoR37) |

## `Lake_Market_Story_GDD21` → `TiepCanCoChuDichR89`

Script: `Assets/Scripts/ConRoi/TiepCanCoChuDichR89.cs` — tên cũ `PurposefulApproachR89`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `puppet` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3` (Transform) |
| `destination` | `School_V37_Classroom_Courtyard/R28_School/R89_School_Approach_Destination` (Transform) |
| `focus` | `School_V37_Classroom_Courtyard/R28_School/Puppet_Paper` (Transform) |
| `head` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/Minh_Head_Pivot` (Transform) |
| `stride` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3` (BuocChanConRoiR23) |
| `support` | `School_V37_Classroom_Courtyard/R28_School/Rear_Classrooms_Slab` (BoxCollider) |
| `staging` | `Lake_Market_Story_GDD21` (TraiNghiemTruongR28) |
| `papers` | `Lake_Market_Story_GDD21` (TuongTacGiayLop203R32) |

## `School_V37_Classroom_Courtyard/CoMai_Former_Teacher` → `HienGuongMatTheoKhu`

Script: `Assets/Scripts/NhanVat/HienGuongMatTheoKhu.cs` — tên cũ `RegionalFaceProjection`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `ghostHead` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot` (Transform) |
| `minhFace` | `Assets/Art/Models/Model.blend` (Mesh) |
| `minhMaterial` | `Assets/Art/Gameplay/LakeMarketV34/MinhFace.mat` (Material) |
| `minhHeadSource` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/Minh_Head_Pivot` (Transform) |
| `fittedPortraitR91` | `Assets/Art/Characters/R92/R92_Portrait_CanonicalBound.asset` (Mesh) |
| `portraitMaterialsR91.Array.data[0]` | `Assets/Art/Characters/R47/R47_Warm_Wood.mat` (Material) |
| `portraitMaterialsR91.Array.data[1]` | `Assets/Art/Characters/R47/R47_Dark_Endgrain.mat` (Material) |
| `portraitMaterialsR91.Array.data[2]` | `Assets/Art/Characters/R47/R47_Knot_Dark.mat` (Material) |
| `portraitMaterialsR91.Array.data[3]` | `Assets/Art/Characters/R47/R47_Knot_Heart.mat` (Material) |

## `School_V37_Classroom_Courtyard/CoMai_Former_Teacher` → `DienXuatNhanVatV40`

Script: `Assets/Scripts/NhanVat/DienXuatNhanVatV40.cs` — tên cũ `ActorPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `gaze` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher/V40_Head_Gaze_Pivot` (Transform) |
| `body` | `School_V37_Classroom_Courtyard/CoMai_Former_Teacher` (Animation) |

## `School_V37_Classroom_Courtyard/CoMai_Former_Teacher` → `BieuCamNhoNhanVatV41`

Script: `Assets/Scripts/NhanVat/BieuCamNhoNhanVatV41.cs` — tên cũ `ActorMicroPerformanceV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3` → `DieuKhienConRoi`

Script: `Assets/Scripts/ConRoi/DieuKhienConRoi.cs` — tên cũ `ConRoiAI`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerTransform` | `Minh_RegionalWalk_180cm` (Transform) |
| `dauTransform` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/Minh_Head_Pivot` (Transform) |
| `gesturePart` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/V41_Leading_Shoulder` (Transform) |

## `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3` → `TiepDatConRoiV41`

Script: `Assets/Scripts/ConRoi/TiepDatConRoiV41.cs` — tên cũ `PuppetGroundingV41`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3` → `BuocChanConRoiR23`

Script: `Assets/Scripts/ConRoi/BuocChanConRoiR23.cs` — tên cũ `PuppetStrideR23`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `trajectory` | `Assets/Art/Characters/R23/WoodenStep.json` (TextAsset) |
| `supportR46` | `School_V37_Classroom_Courtyard/R28_School/Rear_Classrooms_Slab` (BoxCollider) |
| `fittedLegsR91.Array.data[0]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_Fitted_Leg_L` (Transform) |
| `fittedLegsR91.Array.data[1]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_Fitted_Leg_R` (Transform) |

## `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3` → `DayConRoiR46`

Script: `Assets/Scripts/ConRoi/DayConRoiR46.cs` — tên cũ `PuppetThreadsR46`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `sourceBindings` | `Assets/Art/Characters/R46/ThreadBindings.json` (TextAsset) |
| `fittedRoutingR91` | `Assets/Art/Characters/R91/ThreadRoutingR91.json` (TextAsset) |
| `attachments.Array.data[0]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/Minh_Head_Pivot/ConRoi_Head_SameAsMinh` (MeshFilter) |
| `attachments.Array.data[1]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/ConRoi_Arm_L` (MeshFilter) |
| `attachments.Array.data[2]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/GD3_Hand` (MeshFilter) |
| `attachments.Array.data[3]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_Fitted_Leg_L` (MeshFilter) |
| `attachments.Array.data[4]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_Fitted_Leg_R` (MeshFilter) |
| `segments.Array.data[0]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_1_0` (Transform) |
| `segments.Array.data[1]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_1_1` (Transform) |
| `segments.Array.data[2]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_2_0` (Transform) |
| `segments.Array.data[3]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_2_1` (Transform) |
| `segments.Array.data[4]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_3_0` (Transform) |
| `segments.Array.data[5]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_3_1` (Transform) |
| `segments.Array.data[6]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_4_0` (Transform) |
| `segments.Array.data[7]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_4_1` (Transform) |
| `segments.Array.data[8]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_5_0` (Transform) |
| `segments.Array.data[9]` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_String_5_1` (Transform) |

## `School_V37_Classroom_Courtyard/School_Return_Clock` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/Mai_Interaction` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/LỊCH HỌC` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/SỔ NHẬN XÉT` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/TRANG VIẾT DỞ` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/BÀI LÀM CŨ` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/THÔNG BÁO` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/GIÁO ÁN` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/Document_THƯ CHƯA GỬI` → `DaoCuKeChuyenV40`

Script: `Assets/Scripts/TuongTac/DaoCuKeChuyenV40.cs` — tên cũ `NarrativePropV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `School_V37_Classroom_Courtyard/Document_BIÊN BẢN HỌP KHOA` → `DaoCuKeChuyenV40`

Script: `Assets/Scripts/TuongTac/DaoCuKeChuyenV40.cs` — tên cũ `NarrativePropV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `School_V37_Classroom_Courtyard/Document_NHẬT KÝ THỰC TẬP` → `DaoCuKeChuyenV40`

Script: `Assets/Scripts/TuongTac/DaoCuKeChuyenV40.cs` — tên cũ `NarrativePropV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `School_V37_Classroom_Courtyard/R28_School/Ban_Sinh_Vien_Response_R28/Desk_Top` → `DichTuongTacGiayR32`

Script: `Assets/Scripts/TuongTac/DichTuongTacGiayR32.cs` — tên cũ `Class203PaperTargetR32`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (TuongTacGiayLop203R32) |

## `School_V37_Classroom_Courtyard/R28_School/Player_Paper` → `DichTuongTacGiayR32`

Script: `Assets/Scripts/TuongTac/DichTuongTacGiayR32.cs` — tên cũ `Class203PaperTargetR32`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (TuongTacGiayLop203R32) |

## `School_V37_Classroom_Courtyard/R28_School/Right_PaperTray_Base` → `DichTuongTacGiayR32`

Script: `Assets/Scripts/TuongTac/DichTuongTacGiayR32.cs` — tên cũ `Class203PaperTargetR32`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (TuongTacGiayLop203R32) |

## `School_V37_Classroom_Courtyard/R28_School/Mai_R28_Interaction` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/R28_School/Document_THƯ CHƯA GỬI(Clone)` → `DaoCuKeChuyenV40`

Script: `Assets/Scripts/TuongTac/DaoCuKeChuyenV40.cs` — tên cũ `NarrativePropV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `School_V37_Classroom_Courtyard/R28_School/Document_BIÊN BẢN HỌP KHOA(Clone)` → `DaoCuKeChuyenV40`

Script: `Assets/Scripts/TuongTac/DaoCuKeChuyenV40.cs` — tên cũ `NarrativePropV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `School_V37_Classroom_Courtyard/R28_School/Document_NHẬT KÝ THỰC TẬP(Clone)` → `DaoCuKeChuyenV40`

Script: `Assets/Scripts/TuongTac/DaoCuKeChuyenV40.cs` — tên cũ `NarrativePropV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `School_V37_Classroom_Courtyard/R28_School/Gate_Return` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/R28_School/203_Return` → `TuongTacTruongV37`

Script: `Assets/Scripts/TuongTac/TuongTacTruongV37.cs` — tên cũ `SchoolInteractionV37`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongTruongHocV37) |

## `School_V37_Classroom_Courtyard/R28_School/Model_Answer_Paper_R32` → `DichTuongTacGiayR32`

Script: `Assets/Scripts/TuongTac/DichTuongTacGiayR32.cs` — tên cũ `Class203PaperTargetR32`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (TuongTacGiayLop203R32) |

## `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/The_hanh_ly_cu` → `DichTuongTacHanhLyR41`

Script: `Assets/Scripts/TuongTac/DichTuongTacHanhLyR41.cs` — tên cũ `SlopePreparationTargetR41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (ChuanBiHanhLyR41) |

## `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/To_chuan_bi_gap` → `DichTuongTacHanhLyR41`

Script: `Assets/Scripts/TuongTac/DichTuongTacHanhLyR41.cs` — tên cũ `SlopePreparationTargetR41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (ChuanBiHanhLyR41) |

## `DaLat_Slope_V39/Tung_Packed_Dusty_Suitcase/R41_Thu_da_chuan_bi_tu_lau/Dat_the_canh_quai` → `DichTuongTacHanhLyR41`

Script: `Assets/Scripts/TuongTac/DichTuongTacHanhLyR41.cs` — tên cũ `SlopePreparationTargetR41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `activity` | `Lake_Market_Story_GDD21` (ChuanBiHanhLyR41) |

## `DaLat_Slope_V39/VÉ XE CŨ` → `TuongTacDocV39`

Script: `Assets/Scripts/TuongTac/TuongTacDocV39.cs` — tên cũ `SlopeInteractionV39`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongDocV39) |

## `DaLat_Slope_V39/ĐỊA CHỈ VIẾT TAY` → `TuongTacDocV39`

Script: `Assets/Scripts/TuongTac/TuongTacDocV39.cs` — tên cũ `SlopeInteractionV39`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongDocV39) |

## `DaLat_Slope_V39/AnhTung_Cousin` → `HienGuongMatTheoKhu`

Script: `Assets/Scripts/NhanVat/HienGuongMatTheoKhu.cs` — tên cũ `RegionalFaceProjection`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `ghostHead` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot` (Transform) |
| `minhFace` | `Assets/Art/Models/Model.blend` (Mesh) |
| `minhMaterial` | `Assets/Art/Gameplay/LakeMarketV34/MinhFace.mat` (Material) |
| `minhHeadSource` | `School_V37_Classroom_Courtyard/ConRoi_Minh_GD3/R91_PushLean/Minh_Head_Pivot` (Transform) |
| `fittedPortraitR91` | `Assets/Art/Characters/R92/R92_Portrait_CanonicalBound.asset` (Mesh) |
| `portraitMaterialsR91.Array.data[0]` | `Assets/Art/Characters/R47/R47_Warm_Wood.mat` (Material) |
| `portraitMaterialsR91.Array.data[1]` | `Assets/Art/Characters/R47/R47_Dark_Endgrain.mat` (Material) |
| `portraitMaterialsR91.Array.data[2]` | `Assets/Art/Characters/R47/R47_Knot_Dark.mat` (Material) |
| `portraitMaterialsR91.Array.data[3]` | `Assets/Art/Characters/R47/R47_Knot_Heart.mat` (Material) |

## `DaLat_Slope_V39/AnhTung_Cousin` → `DienXuatNhanVatV40`

Script: `Assets/Scripts/NhanVat/DienXuatNhanVatV40.cs` — tên cũ `ActorPerformanceV40`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |
| `gaze` | `DaLat_Slope_V39/AnhTung_Cousin/V40_Head_Gaze_Pivot` (Transform) |
| `body` | `DaLat_Slope_V39/AnhTung_Cousin` (Animation) |

## `DaLat_Slope_V39/AnhTung_Cousin` → `BieuCamNhoNhanVatV41`

Script: `Assets/Scripts/NhanVat/BieuCamNhoNhanVatV41.cs` — tên cũ `ActorMicroPerformanceV41`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `story` | `Lake_Market_Story_GDD21` (DieuPhoiHanhTrinh) |

## `DaLat_Slope_V39/Tung_Interaction` → `TuongTacDocV39`

Script: `Assets/Scripts/TuongTac/TuongTacDocV39.cs` — tên cũ `SlopeInteractionV39`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongDocV39) |

## `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly` → `DieuKhienConRoi`

Script: `Assets/Scripts/ConRoi/DieuKhienConRoi.cs` — tên cũ `ConRoiAI`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `playerTransform` | `Minh_RegionalWalk_180cm` (Transform) |
| `dauTransform` | `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly/Minh_Head_Pivot` (Transform) |
| `gesturePart` | `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly/V41_Leading_Shoulder` (Transform) |

## `DaLat_Slope_V39/ConRoi_Minh_GD4_FinalOnly` → `TiepDatConRoiV41`

Script: `Assets/Scripts/ConRoi/TiepDatConRoiV41.cs` — tên cũ `PuppetGroundingV41`. Prefab component: lưu trực tiếp trong scene.

Không có reference object tùy chỉnh được serialize; xem code cho thành phần tự tìm/tạo.

## `DaLat_Slope_V39/Last_Door` → `TuongTacDocV39`

Script: `Assets/Scripts/TuongTac/TuongTacDocV39.cs` — tên cũ `SlopeInteractionV39`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `chapter` | `Lake_Market_Story_GDD21` (ChuongDocV39) |

## `R11_Route_Wayfinding/CẦU ÔNG ĐẠO →
Ông Bảo bên lan can` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/CẦU ÔNG ĐẠO →
Ông Bảo bên lan can/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/CẦU ÔNG ĐẠO →
Ông Bảo bên lan can/Lettering_Reverse_Face` (MeshRenderer) |

## `R11_Route_Wayfinding/← CẦU ÔNG ĐẠO
LỐI VEN HỒ` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/← CẦU ÔNG ĐẠO
LỐI VEN HỒ/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/← CẦU ÔNG ĐẠO
LỐI VEN HỒ/Lettering_Reverse_Face` (MeshRenderer) |

## `R11_Route_Wayfinding/VÒNG XOAY / CHỢ →` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/VÒNG XOAY / CHỢ →/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/VÒNG XOAY / CHỢ →/Lettering_Reverse_Face` (MeshRenderer) |

## `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ/Lettering_Reverse_Face` (MeshRenderer) |

## `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ/Lettering_Reverse_Face` (MeshRenderer) |

## `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ/Lettering_Reverse_Face` (MeshRenderer) |

## `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/CHỢ ĐÀ LẠT ↑
Theo phố tới mặt chợ/Lettering_Reverse_Face` (MeshRenderer) |

## `R11_Route_Wayfinding/MẶT CHỢ ↑
Có thể đi tiếp rồi quay lại` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/MẶT CHỢ ↑
Có thể đi tiếp rồi quay lại/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/MẶT CHỢ ↑
Có thể đi tiếp rồi quay lại/Lettering_Reverse_Face` (MeshRenderer) |

## `R11_Route_Wayfinding/TỈNH DẬY
E · Về phòng trọ` → `MatBienHieuV48`

Script: `Assets/Scripts/MoiTruong/MatBienHieuV48.cs` — tên cũ `WorldSignFacesV48`. Prefab component: lưu trực tiếp trong scene.

| Field Inspector/serialized | Đối tượng/asset thực |
|---|---|
| `eye` | `Minh_RegionalWalk_180cm/Main Camera` (Camera) |
| `front` | `R11_Route_Wayfinding/TỈNH DẬY
E · Về phòng trọ/Lettering` (MeshRenderer) |
| `back` | `R11_Route_Wayfinding/TỈNH DẬY
E · Về phòng trọ/Lettering_Reverse_Face` (MeshRenderer) |

Các field số/bool/enum và trạng thái enabled gốc xem scene YAML, scene-serialized.json và live-systems.json; bảng này không biểu diễn toàn bộ Inspector tùy biến. Không có kiểm bằng ảnh panel Inspector trong phiên này.
