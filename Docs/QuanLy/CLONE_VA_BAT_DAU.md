<!-- checkout-final-a789222-20261107 -->
> Kết quả mới nhất: [kiểm a789222](CheckoutFinal20261107/README.md), Git/LFS/import/reference đạt; tổng thể BLOCKED_UI_REGRESSION (P sau nhập mật mã). Ba task branch giữ a209c0e, ownership không đổi; không push. Các trạng thái checkout cũ bên dưới là lịch sử.

> 07/11/2026: [Checkout C thực và giới hạn hiện hành](CheckoutTest20261107/README.md). Source a789222; checkout mở ban đầu 8ea91d4. Compile/reference entry đạt; UI và baseline sửa chưa kiểm xong, task branch chưa tiến.

> 07/11/2026: [Rà soát mới](Review20261107/README.md). Baseline rà `90147d9`; checkout sạch BLOCKED_CAPACITY, chưa nâng task branch. Không kế thừa kiểm máy gốc thành PASS checkout.

# Clone/LFS và bắt đầu

Repo local `D:/ConRoi`; nhánh chuẩn bị `chore/prepare-r107`. Không có remote trong đợt này. URL từng được nhắc không tự được thêm; yêu cầu mới nhất là **không push**. Xem `HANDOFF.md` để biết commit thực tế và trạng thái checkout.

**Không coi checkout đã chạy khi báo cáo còn BLOCKED/NOT_RUN.** Git LFS phải được cài; `git lfs pull` cần quyền truy cập remote và toàn bộ object. Không copy Library/UserSettings/profile cá nhân sang máy khác.

Sau khi người tổ chức duyệt remote và publish riêng:

```powershell
git clone <remote-da-duoc-duyet> ConRoi
cd ConRoi
git lfs install --local
git lfs pull
git lfs fsck
git switch -c work/<nguoi>-<task> <commit-tich-hop-da-chot>
```

Dùng Unity **6000.6.2f1**, mở `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`. Build Settings bật entry này; r92 lịch sử giữ disabled. Không dùng scene r92 để đánh giá prototype r107. Ba scene thử Map/Gameplay/GiaoDien là template rỗng; chưa có luồng điều tra riêng.

Đóng Editor của project trước khi truyền profile mới, không đóng project Unity khác:

```powershell
& 'D:/data_unity/6000.6.2f1/Editor/Unity.exe' -projectPath '<checkout>' --investigation-save-dir '<thu-muc-thu-moi-chua-dung>'
```

Đường Editor có thể khác ở máy khác, nhưng đúng phiên bản. Play phải thấy intro và hồ sơ trống; không tự load. F5 ghi `<profile>/InvestigationV36/r107/prototype.json`; kiểm path thực, namespace `conroi.investigation.v36.r107`, schema1. Không dùng save kỹ thuật hoặc migration profile gameplay cũ. P mở ghi chép, E theo prompt, R trở lại, Esc pause. Hướng dẫn chỉ cho người chơi trong Docs/QA.

Library trên máy gốc là junction tới `.build-cache/r92-unity-library`; **không clone junction**, không đi xuyên nó để dọn. Checkout mới tự sinh Library, cần đủ dung lượng cho source + LFS objects + cache/import. Tool historical chưa audit đầy đủ giữ PROVISIONAL; không chạy author/derive/bind/integrate một lần.
