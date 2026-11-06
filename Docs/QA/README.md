> 07/11/2026: [Rà soát mới](../QuanLy/Review20261107/README.md). Baseline rà `90147d9`; checkout sạch BLOCKED_CAPACITY, chưa nâng task branch. Không kế thừa kiểm máy gốc thành PASS checkout.

# QA của nhóm Con Rối

Entry: `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`, bản copy r107 có GUID scene riêng; checkpoint r107 giữ nguyên. Unity6000.6.2f1. Người mới sau r107 **NOT_RUN**, không dùng kiểm tổng hợp thay người thật.

- [Hướng dẫn người chơi](investigation_r107/HUONG_DAN_NGUOI_CHOI.md)
- [Hướng dẫn người tổ chức](investigation_r107/PLAYTEST_NGUOI_MOI.md): đổi scene entry sang ConRoi_Chinh cho phiên nhóm; save vẫn namespace r107.
- [Kiểm controller/E/raycast scene tích hợp](GitPreparation/onboarding-route.txt)
- [Kiểm xoay/phóng, ghi/P và F5/F6](GitPreparation/onboarding-ui-complete.txt)
- [Reference/compile trong phạm vi scene đang mở](GitPreparation/reference-audit.txt)
- [Nguồn Blender mở lại](GitPreparation/blender-source-open.json)
- [Archive và mapping từng file](../QuanLy/OLD_NEW_PATHS.md)

Các bản báo cáo lịch sử sao nguyên byte sang `investigation_r102`..`r107`, có hash trong `../QuanLy/qa-copies.json`. Giữ nguyên FAIL/superseded/NOT_RUN. Link capture lịch sử có thể trỏ vị trí cũ; không hiểu thiếu capture là kiểm đã thất bại hoặc đã PASS: tra `../QuanLy/archive-manifest.jsonl` theo old path. Capture đã chuyển có `new` tuyệt đối, staging được giữ. Nội dung chưa chuyển do thiếu chỗ còn tại root cũ và được ignore/unclassified theo báo cáo, không xóa.

Không đưa profile kỹ thuật `GitPreparation_20261106` cho người mới: profile này đã có một ghi chép từ lượt kiểm tổng hợp. Chọn một thư mục mới chưa dùng theo hướng dẫn nhóm.
