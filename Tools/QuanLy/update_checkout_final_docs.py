from pathlib import Path
R=Path(__file__).resolve().parents[2]
marker='<!-- checkout-final-a789222-20261107 -->'
overview='''{marker}
## Kiểm baseline a789222 — kết quả hiện hành 07/11/2026

**Đã kiểm thực tế `a789222286f1a56db5b6cc21e1ffb5a0e7542be4` tại `C:/ConRoi_CheckoutTest`, Unity 6000.6.2f1.** Git/LFS/import/reference PASS: 878 file LFS materialized, 0 Missing Script/0 reference hỏng, 4.582 dependency. Library C độc lập tạo mới ở lần checkout đầu, import tăng dần baseline sửa; không dùng chung Library D. Entry `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`.

**Tổng thể `BLOCKED_UI_REGRESSION`**, không còn chặn vì dung lượng/quyền điều khiển. Controller Start → dấu sàn → khám/ghi/R/P → điện thoại, 1080p, save/load và skip/revisit đạt bằng input tổng hợp. Navigation mật mã/đối chiếu/kết luận được kiểm với thư cấp API trong profile fixture riêng. Ca **nhập mật mã → Trở lại → P không mở hồ sơ** chưa đạt; Esc → Ghi chép là đường truy cập được, không thay cho PASS. Chưa xác nhận nguyên nhân cuối, không sửa sản phẩm trong đợt này.

Đã bảo toàn diff/state trước Play stop/sync, quarantine 11 file Unity tự sinh, sync từ 8ea91d4 qua Git/LFS tới a789222; không ghi đè snapshot. Editor đã dừng/đóng khi Edit sạch, không save runtime scene. ProjectSettings ghi lúc thoát có snapshot và diff nội dung rỗng; source D không đổi sản phẩm. Lần intro tự đổi ở phiên cũ vẫn **chưa rõ nguồn input**, không gọi là lỗi gameplay.

Ba task branch **giữ a209c0e**, chưa nâng vì còn NAV-01; ownership không đổi, người 2 giữ toàn bộ partial session r107. **4.183 file D chưa xác minh giữ nguyên**. Save gốc/archive/GUID bảo toàn, profile kỹ thuật riêng ở C. Người mới **NOT_RUN**. Không sửa map/gameplay, không remote/push/build/ZIP/runner runtime mới.

Xem [báo cáo cuối]({report}), [audit]({audit}) và [manifest/index bằng chứng]({index}). Commit sau a789222 chỉ là tài liệu/công cụ kiểm; không gọi commit tài liệu là đã chạy sản phẩm. Các phần bên dưới là lịch sử, không thay kết quả hiện hành này.

---

'''
for rel in ['Docs/QuanLy/HANDOFF.md','BAT_DAU_TAI_DAY.md','README.md']:
    p=R/rel;old=p.read_text(encoding='utf-8-sig');assert marker not in old
    prefix='' if rel.startswith('Docs/QuanLy/') else 'Docs/QuanLy/'
    text=overview.format(marker=marker,report=prefix+'CheckoutFinal20261107/README.md',audit=prefix+'CheckoutFinal20261107/final-audit.json',index=prefix+'CheckoutFinal20261107/evidence-index.json')+old
    p.write_text(text,encoding='utf-8');assert p.read_text(encoding='utf-8')==text
for rel in ['OWNERSHIP.md','TICH_HOP.md','CLONE_VA_BAT_DAU.md']:
    p=R/'Docs/QuanLy'/rel;old=p.read_text(encoding='utf-8-sig');assert marker not in old
    note=marker+'\n> Kết quả mới nhất: [kiểm a789222](CheckoutFinal20261107/README.md), Git/LFS/import/reference đạt; tổng thể BLOCKED_UI_REGRESSION (P sau nhập mật mã). Ba task branch giữ a209c0e, ownership không đổi; không push. Các trạng thái checkout cũ bên dưới là lịch sử.\n\n'
    p.write_text(note+old,encoding='utf-8')
print('Updated current pointers in six existing UTF-8 documents; history retained.')
