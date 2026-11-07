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
