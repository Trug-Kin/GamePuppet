# Danh mục 117 scene — kiểm kê 02/11/2026

Scene sản phẩm và scene tiếp tục phát triển: **Assets/Scenes/JourneyPresentation_v48_r92.unity**. Đây là scene duy nhất được bật trong EditorBuildSettings; truy vấn Editor cho active Build Profile = null. Không chọn theo số r; kết luận dựa cấu hình build, luồng khởi động và manifest nguồn.

Mở scene bằng double-click đường dẫn trong Project của Unity 6000.6.2f1. Các scene ngoài build chỉ mở để đọc/tái lập đúng phiên; không dùng New Game trên profile người dùng. Không chạy các lệnh Assemble/Setup/Review lịch sử chỉ để xem scene: nhiều lệnh có SaveScene hoặc đổi Build Settings.

Trong r92, phòng/hồ/chợ/trường/dốc chung một scene; không có scene phụ sản phẩm load động được tìm thấy trong code runtime đã rà. Các LoadSceneAsync tìm được thuộc fixture kiểm lịch sử, dùng reload cùng scene. Các tham chiếu tên scene trong hook di chuyển giới hạn ở DaLatMovementTest. Không suy luận “không còn consumer” từ không thấy literal C#: danh sách dưới chỉ là consumer literal/GUID tìm được, chưa bao phủ reflection ghép chuỗi, lệnh bên ngoài hay mọi checkpoint bên ngoài Assets. Không xóa scene nào.

r44 được lịch sử dự án ghi rõ giữ checkpoint khi các bản presentation kế thừa tiếp tục; không phải workspace phát triển hiện hành riêng. Các bản Experience/Integration/Repair/Presentation trước r92 là nguồn checkpoint/authoring lịch sử, dù tool Editor còn đọc chúng. `_trial`, `_review` được đánh dấu theo lịch sử/fixture và phạm vi ngoài build; r66_review chưa đủ bằng chứng để mô tả mục đích chi tiết. SampleScene là đích setup của tool movement cũ, chưa xác nhận cần tiếp tục giữ với vai trò nào. Bốn _Recovery chỉ biết là bản recovery, không đoán bản chuẩn.

| Đường dẫn chính xác | Trạng thái / cách dùng | Consumer C#/serialized đã tìm được |
|---|---|---|
| `Assets/_Recovery/0 (1).unity` | Ngoài build; vai trò còn cần xác minh | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/_Recovery/0 (2).unity` | Ngoài build; vai trò còn cần xác minh | `Assets/Scripts/Editor/KiemTraNguonVaBuild.cs:134` |
| `Assets/_Recovery/0 (3).unity` | Ngoài build; vai trò còn cần xác minh | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/_Recovery/0.unity` | Ngoài build; vai trò còn cần xác minh | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/CanopyAccessTest_20261014/DaLatMovementTest.unity` | Kiểm thử/fixture lịch sử, ngoài build | `Assets/CanopyAccessTest_20261014/Editor/CanopyAccessSetup.cs:9`; `Assets/DaLatMovementTest_20261013/DaLatMovementRun.cs:64`; `Assets/DaLatMovementTest_20261013/Editor/DaLatRequestedSceneSetup.cs:62`; `Assets/Scripts/NguoiChoi/DieuKhienDiChuyen.cs:67` |
| `Assets/ConRoiGame/ConRoi.unity` | Nguyên mẫu cũ; Editor assembly/bridge còn consumer | `Assets/Scripts/Editor/ConRoiSceneAssembly.cs:42`; `Assets/WRouteC3Test_v30_20261016/Editor/C3LiveEditorBridge.cs:212` |
| `Assets/DaLatMovementTest_20261013/MovementTest.unity` | Kiểm thử/fixture lịch sử, ngoài build | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/DaLatMovementTest_20261013/MovementTest_ImmediateInput.unity` | Kiểm thử/fixture lịch sử, ngoài build | `Assets/DaLatMovementTest_20261013/DaLatMovementTestProfile.cs:10`; `Assets/DaLatMovementTest_20261013/Editor/DaLatRequestedSceneSetup.cs:14` |
| `Assets/Scenes/JourneyAcceptance_v47.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiReviewV47.cs:11` |
| `Assets/Scenes/JourneyAcceptance_v47_rc3.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiHandoffV47.cs:11`; `Assets/Scripts/Editor/ConRoiRepairV48.cs:16,93,106,107,112`; `Assets/Scripts/Editor/ConRoiV48StateAudit.cs:20` |
| `Assets/Scenes/JourneyDevelopment_v48_r44.unity` | Scene phát triển r44 được giữ làm checkpoint | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyExperience_v40.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiFinishV40.cs:12`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75,77` |
| `Assets/Scenes/JourneyExperience_v41.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiCompletionV41.cs:14`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75,77` |
| `Assets/Scenes/JourneyExperience_v42.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiExperienceV42.cs:14`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75,77` |
| `Assets/Scenes/JourneyExperience_v43.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiReviewV43.cs:14` |
| `Assets/Scenes/JourneyExperience_v44.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiReviewV44.cs:13` |
| `Assets/Scenes/JourneyExperience_v45.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiReviewV45.cs:14` |
| `Assets/Scenes/JourneyExperience_v46.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiReviewV46.cs:12` |
| `Assets/Scenes/JourneyIntegration_v39.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiSlopeV39.cs:11`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75` |
| `Assets/Scenes/JourneyPresentation_v48_r45.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r46.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r47.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r48.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r49.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r50.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r51_trial.unity` | Kiểm thử/fixture lịch sử, ngoài build | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r52.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r53.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r54.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r55.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r56.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r57.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r58.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r59.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r60.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r61.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r62.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r63.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r64.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r65.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r66_review.unity` | Bản review ngoài build; vai trò chi tiết chưa rõ | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r67.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r68.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r69.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r70.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r71.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r72.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r73.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r74.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r75.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r76.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r77.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r78.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r79.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r80.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r81.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r82.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r83.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r84.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r85.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r86.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r87.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r88.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r89.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyPresentation_v48_r90.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/KiemTraNguonVaBuild.cs:63` |
| `Assets/Scenes/JourneyPresentation_v48_r91.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/KiemTraNguonVaBuild.cs:63` |
| `Assets/Scenes/JourneyPresentation_v48_r92.unity` | Sản phẩm hiện hành; phát triển trực tiếp | `Assets/Scripts/Editor/KiemTraNguonVaBuild.cs:19`; `ProjectSettings/EditorBuildSettings.asset` |
| `Assets/Scenes/JourneyRepair_v48.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiRepairV48.cs:12` |
| `Assets/Scenes/JourneyRepair_v48_r10.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48AcceptanceRepair.cs:25`; `Assets/Scripts/Editor/ConRoiV48MapAudit.cs:16`; `Assets/Scripts/Editor/ConRoiV48StateAudit.cs:35` |
| `Assets/Scenes/JourneyRepair_v48_r11.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48AcceptanceRepair.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r12.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48R14.cs:16`; `Assets/Scripts/Editor/ConRoiV48SupportFacesRepair.cs:19,44` |
| `Assets/Scenes/JourneyRepair_v48_r14.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48R14.cs:34`; `Assets/Scripts/Editor/ConRoiV48R15.cs:16` |
| `Assets/Scenes/JourneyRepair_v48_r15.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48R15.cs:51`; `Assets/Scripts/Editor/ConRoiV48R16.cs:25` |
| `Assets/Scenes/JourneyRepair_v48_r16.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyRepair_v48_r17.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48R19.cs:23,130`; `Assets/Scripts/Editor/ConRoiV48R20.cs:24,131` |
| `Assets/Scenes/JourneyRepair_v48_r19.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48R19.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r20.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48R20.cs:13`; `Assets/Scripts/Editor/ConRoiV48R20PhotoAudit.cs:15,23,64`; `Assets/Scripts/Editor/ConRoiV48R21.cs:21` |
| `Assets/Scenes/JourneyRepair_v48_r21.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48R21.cs:15`; `Assets/Scripts/Editor/ConRoiV48R21ApproachLayers.cs:15,42`; `Assets/Scripts/Editor/ConRoiV48R21AssetPass.cs:14`; `Assets/Scripts/Editor/ConRoiV48R21Boundaries.cs:16`; `Assets/Scripts/Editor/ConRoiV48R21BoundaryAudit.cs:13`; `Assets/Scripts/Editor/ConRoiV48R21BoundaryCapsule.cs:15,28`; `Assets/Scripts/Editor/ConRoiV48R21BoundaryFinish.cs:13`; `Assets/Scripts/Editor/ConRoiV48R21BridgeLinks.cs:17,75`; `Assets/Scripts/Editor/ConRoiV48R21ConstructionNormals.cs:16`; `Assets/Scripts/Editor/ConRoiV48R21ContactFinish.cs:15,41`; `Assets/Scripts/Editor/ConRoiV48R21Footpaths.cs:51`; `Assets/Scripts/Editor/ConRoiV48R21GeometryAudit.cs:13`; `Assets/Scripts/Editor/ConRoiV48R21LakeMarketBoundary.cs:17,84,92,115`; `Assets/Scripts/Editor/ConRoiV48R21Landscape.cs:61,154`; `Assets/Scripts/Editor/ConRoiV48R21NativeFollowup.cs:17,30,50,65,75`; `Assets/Scripts/Editor/ConRoiV48R21PavingEdges.cs:16`; `Assets/Scripts/Editor/ConRoiV48R21PromenadeEdges.cs:24`; `Assets/Scripts/Editor/ConRoiV48R21Reachability.cs:15`; `Assets/Scripts/Editor/ConRoiV48R21RecoveryFix.cs:12`; `Assets/Scripts/Editor/ConRoiV48R21RegroundBoundary.cs:15`; `Assets/Scripts/Editor/ConRoiV48R21SampleRefinement.cs:11`; `Assets/Scripts/Editor/ConRoiV48R21Shore.cs:17`; `Assets/Scripts/Editor/ConRoiV48R21TerrainBoundary.cs:15`; `Assets/Scripts/Editor/ConRoiV48R21WaterSupportAudit.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r21_playtest.unity` | Kiểm thử/fixture lịch sử, ngoài build | `Assets/Scripts/Editor/ConRoiR21ProductReview.cs:19`; `Assets/Scripts/Editor/ConRoiR22Scenery.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r21_recoveryfix.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48R21RecoveryFix.cs:12`; `Assets/Scripts/Editor/ConRoiV48R21RecoveryGeometry.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r21_recoveryprep.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR21HedgeContinuity.cs:13`; `Assets/Scripts/Editor/ConRoiR21Preflight.cs:16`; `Assets/Scripts/Editor/ConRoiR21ProductReview.cs:18`; `Assets/Scripts/Editor/ConRoiR21RecoveryPrep.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r22.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR22Scenery.cs:14` |
| `Assets/Scenes/JourneyRepair_v48_r23.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR23Opening.cs:11` |
| `Assets/Scenes/JourneyRepair_v48_r24.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR24Reference.cs:12` |
| `Assets/Scenes/JourneyRepair_v48_r25.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR25Slope.cs:12` |
| `Assets/Scenes/JourneyRepair_v48_r26.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR26Slope.cs:12` |
| `Assets/Scenes/JourneyRepair_v48_r27.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR27Slope.cs:12` |
| `Assets/Scenes/JourneyRepair_v48_r28.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiModelLocations.cs:18,29`; `Assets/Scripts/Editor/ConRoiR28School.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r29.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR29Inventory.cs:18,29`; `Assets/Scripts/Editor/ConRoiR29School.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r30.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR30.cs:12`; `Assets/Scripts/Editor/ConRoiR31Review.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r32.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR32.cs:11` |
| `Assets/Scenes/JourneyRepair_v48_r33.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR33Chair.cs:7` |
| `Assets/Scenes/JourneyRepair_v48_r34.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR34.cs:11` |
| `Assets/Scenes/JourneyRepair_v48_r35.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR35.cs:10` |
| `Assets/Scenes/JourneyRepair_v48_r36.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR36.cs:13` |
| `Assets/Scenes/JourneyRepair_v48_r37.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR37.cs:10` |
| `Assets/Scenes/JourneyRepair_v48_r38.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR38.cs:3` |
| `Assets/Scenes/JourneyRepair_v48_r39.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR39.cs:10`; `Assets/Scripts/KiemThuLichSu/R40LakeAudit.cs:22` |
| `Assets/Scenes/JourneyRepair_v48_r40.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR40.cs:3`; `Assets/Scripts/KiemThuLichSu/R40LakeAudit.cs:23,33` |
| `Assets/Scenes/JourneyRepair_v48_r41.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR41.cs:3` |
| `Assets/Scenes/JourneyRepair_v48_r42.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiR42.cs:14`; `Assets/Scripts/Editor/ConRoiR43.cs:14` |
| `Assets/Scenes/JourneyRepair_v48_r5.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyRepair_v48_r6.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyRepair_v48_r7_isolation.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/JourneyRepair_v48_r8_isolation.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiV48StateAudit.cs:35` |
| `Assets/Scenes/JourneyRepair_v48_r9.unity` | Checkpoint/nguồn authoring lịch sử | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/Scenes/LakeGateMarket_v32.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiRouteV32.cs:13`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75` |
| `Assets/Scenes/LakeGateMarket_v33.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiRouteV33.cs:14`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75,79,153` |
| `Assets/Scenes/LakeMarketIntegration_v36.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiStabilizationV36.cs:9`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75` |
| `Assets/Scenes/LakeMarketStory_v34.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiLakeMarketV34.cs:12`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75,79` |
| `Assets/Scenes/LakeMarketStory_v35.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiOngDaoV35.cs:12`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75` |
| `Assets/Scenes/SampleScene.unity` | Ngoài build; vai trò còn cần xác minh | `Assets/DaLatMovementTest_20261013/Editor/DaLatMovementSetup.cs:127` |
| `Assets/Scenes/SchoolContinuation_v37.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiSchoolV37.cs:11` |
| `Assets/Scenes/SchoolIntegration_v38.unity` | Checkpoint/nguồn authoring lịch sử | `Assets/Scripts/Editor/ConRoiSchoolV38.cs:12`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.Region.cs:75` |
| `Assets/Scenes/Tests/DaLatMovementTest.unity` | Kiểm thử/fixture lịch sử, ngoài build | `Assets/DaLatMovementTest_20261013/DaLatMovementRun.cs:64`; `Assets/DaLatMovementTest_20261013/Editor/DaLatFollowupSetup.cs:12`; `Assets/DaLatMovementTest_20261013/Editor/DaLatRequestedSceneSetup.cs:15,62`; `Assets/Scripts/NguoiChoi/DieuKhienDiChuyen.cs:67` |
| `Assets/Tooling3DTrial/FlowerReview.unity` | Kiểm thử/fixture lịch sử, ngoài build | Chưa tìm được literal/GUID trong phạm vi đã quét; không chứng minh không dùng |
| `Assets/WRouteC3Test_v30_20261016/DaLatMovementTest.unity` | Kiểm thử/fixture lịch sử, ngoài build | `Assets/DaLatMovementTest_20261013/DaLatMovementRun.cs:64`; `Assets/DaLatMovementTest_20261013/Editor/DaLatRequestedSceneSetup.cs:62`; `Assets/Scripts/Editor/ConRoiRouteV32.cs:66`; `Assets/Scripts/NguoiChoi/DieuKhienDiChuyen.cs:67`; `Assets/WRouteC3Test_v30_20261016/C3ActualPlayRun.cs:54`; `Assets/WRouteC3Test_v30_20261016/C3ManualWalk.cs:100`; `Assets/WRouteC3Test_v30_20261016/C3RouteFixture.cs:35,50`; `Assets/WRouteC3Test_v30_20261016/Editor/C3LiveEditorBridge.cs:14` |

Bằng chứng cấu hình/runtime: [hồ sơ kiểm kê](../../../Test/KetQua/kiem-ke-du-an-20261102/). Danh sách JSON chứa toàn bộ số dòng consumer; không mở/chạy lại toàn bộ scene lịch sử trong đợt tài liệu này.
