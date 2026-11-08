# Chốt nhận việc 08/11/2026 — fcbb7e19 / biên nhận 55d7f04

Kết quả hiện hành: [bàn giao nhóm](TeamHandoff20261108/README.md), [audit Git/LFS](TeamHandoff20261108/audit.json).
Ba nhánh task đều chứa baseline fcbb7e19, không có commit riêng tại lúc kiểm; không đổi ref trong đợt này. Những thông báo BLOCKED/a209c0e phía dưới thuộc lịch sử, đã được biên nhận NAV-01 thay thế.

## Quy trình hiện hành

1. Nhận đúng nhánh trong DANG_LAM_NGUOI_1/2/3; ghi HEAD/file dự định chạm trước sửa. Nhóm bắt đầu từ fcbb7e19, không phải a209c0e.
2. Mỗi người dùng checkout riêng. N1/N2 dùng scene thử và đề nghị wiring; chỉ N3 sửa `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`.
3. Thứ tự đầu: N1 triage topology/góc ảnh; N2 xác minh hợp đồng/GDD và chọn task nhỏ có review; N3 tổ chức người mới chơi trước mở rộng onboarding. Có thể làm song song ở file đã giao riêng.
4. Bàn giao commit + diff/file/GUID + ca kiểm chịu ảnh hưởng + giới hạn. N2 review runtime/contract; N3 review luồng/nội dung và tích hợp lên chore/prepare-r107. Không tự cherry-pick scene khác đè scene chính hoặc force/reset nhánh có việc riêng.
5. Docs điều phối mới nằm trên nhánh chore; task branch vẫn fcbb7e19. Trước bắt đầu sửa, từng người bảo toàn working tree rồi merge commit tài liệu chốt từ chore bằng Git (sau khi nhận ref đã publish hoặc từ repo local được phép). Không copy bù asset untracked. Không force cập nhật ba nhánh chỉ để nhận docs.
6. Không lặp toàn bộ kiểm Unity đã PASS nếu không thay sản phẩm. Sau thay đổi, compile và kiểm đúng luồng chịu ảnh hưởng; ghi fixture/API riêng. Người mới chưa chạy giữ NOT_RUN.

<!-- nav01-fixed-20261108 -->
> 08/11: [NAV-01 kiểm đạt trên nguồn 074caf3](NavFix20261108/README.md). Lấy baseline nhánh/task và commit báo cáo thực tế từ HANDOFF/biên nhận mới nhất; trạng thái BLOCKED cũ là lịch sử. Ownership/CHUA_NHAN giữ nguyên, người mới NOT_RUN; không push.

<!-- checkout-final-a789222-20261107 -->
> Kết quả mới nhất: [kiểm a789222](CheckoutFinal20261107/README.md), Git/LFS/import/reference đạt; tổng thể BLOCKED_UI_REGRESSION (P sau nhập mật mã). Ba task branch giữ a209c0e, ownership không đổi; không push. Các trạng thái checkout cũ bên dưới là lịch sử.

> 07/11/2026: [Checkout C thực và giới hạn hiện hành](CheckoutTest20261107/README.md). Source a789222; checkout mở ban đầu 8ea91d4. Compile/reference entry đạt; UI và baseline sửa chưa kiểm xong, task branch chưa tiến.

> 07/11/2026: [Rà soát mới](Review20261107/README.md). Baseline rà `90147d9`; checkout sạch BLOCKED_CAPACITY, chưa nâng task branch. Không kế thừa kiểm máy gốc thành PASS checkout.

# Tích hợp Git cho ba người

Unity **6000.6.2f1**, URP/package lock giữ nguyên. Không nâng package hoặc đổi schema/save IDs. Scene checkpoint `Assets/Scenes/JourneyPresentation_v48_r107.unity` giữ nguyên. Xem README đợt để biết scene entry thực tế sau chuẩn bị.

Mỗi người clone riêng, không dùng chung thư mục làm việc hoặc Library. Người3 sở hữu scene tích hợp; người1/2 không commit scene chính. Dùng nhánh theo task, commit nhỏ tách asset/meta, code và tài liệu. Không force-push/rewrite lịch sử.

Ba nhánh đầu (dự kiến; xem kết quả Git để biết đã tạo hay chưa):

- `work/map-asset-baseline`: kiểm dependency/GUID của vùng sửa, không polish terrain.
- `work/runtime-contracts-baseline`: kiểm interface hiện hành và save namespace, không refactor gameplay.
- `work/ui-onboarding-review`: review mở đầu trên profile sạch, ghi vấn đề có bước tái hiện, chưa đổi UI.

Trước sửa: cập nhật `DANG_LAM_NGUOI_N.md`, ghi file sẽ chạm, scene thử, ID vật chứng, branch và commit gốc. Nếu trùng file thì thống nhất một chủ. Trước merge: đóng scene đang sửa, kiểm diff text/GUID, compile và kiểm đúng hành vi chịu ảnh hưởng. Không dùng UnityYAMLMerge để tự chấp nhận mọi conflict; nếu scene conflict, chủ scene xử lý từ thay đổi đã review.

Không tạo remote hoặc push trong đợt này. Repository không được coi sẵn sàng push nếu clean checkout chưa được mở/kiểm, LFS thiếu object, hoặc còn dữ liệu lớn chưa phân loại.
