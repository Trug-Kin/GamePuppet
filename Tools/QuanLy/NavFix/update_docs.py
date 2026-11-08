from pathlib import Path
R=Path(__file__).resolve().parents[3];marker='<!-- nav01-fixed-20261108 -->'
for rel in ['Docs/QuanLy/HANDOFF.md','BAT_DAU_TAI_DAY.md','README.md']:
    p=R/rel;prior=p.read_text(encoding='utf-8-sig');assert marker not in prior
    link=('' if rel.startswith('Docs/QuanLy/') else 'Docs/QuanLy/')+'NavFix20261108/README.md'
    note=f'''{marker}
## NAV-01 đã kiểm đạt — 08/11/2026

**Commit nguồn đã kiểm: `074caf3dd3da4107b4fc9ac1c089b20216b4705b`.** Sửa tối thiểu vòng đời focus trong hai script r107. Trên checkout độc lập `C:/ConRoi_CheckoutTest`, Unity **6000.6.2f1**, entry `Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity`: compile/reference đạt, NAV-01 và hồi quy UI/input/save-load **PASS kỹ thuật**. P hoạt động sau nhập đúng/sai/dở, còn được chặn đúng khi ô nhập vẫn focus. Không thay map, luật, schema, GUID hoặc save gốc.

[Báo cáo, fixture và giới hạn]({link}): input tổng hợp; letter/slide/phone và 2 quan sát floor được seed API có ghi rõ, không phải người mới. **Người mới NOT_RUN; 4.183 file legacy chưa xác minh giữ nguyên.** 879 file LFS/797 object materialized đủ, 11.458 GUID meta khớp; runtime không phụ thuộc cache/archive ngoài repo. Library C độc lập, import tăng dần commit sửa. State/diff/profile được snapshot, Editor đã đóng ở Edit sạch không lưu scene.

NAV-01 thay thế trạng thái BLOCKED_UI_REGRESSION 07/11; các FAIL cũ bên dưới được giữ như lịch sử. Commit báo cáo và biên nhận fast-forward ba task branch được ghi riêng sau khi có hash thực tế. Ownership vẫn giữ: người 2 sở hữu toàn bộ partial session, người 3 không đồng thời sửa các file đó. Không push/remote/build/ZIP; chưa tuyên bố toàn map/legacy PASS.

---

'''
    p.write_text(note+prior,encoding='utf-8');assert p.read_text(encoding='utf-8')==note+prior
for name in ['OWNERSHIP.md','TICH_HOP.md','CLONE_VA_BAT_DAU.md','DANG_LAM_NGUOI_1.md','DANG_LAM_NGUOI_2.md','DANG_LAM_NGUOI_3.md']:
    p=R/'Docs/QuanLy'/name;t=p.read_text(encoding='utf-8-sig');assert marker not in t
    note=marker+'\n> 08/11: [NAV-01 kiểm đạt trên nguồn 074caf3](NavFix20261108/README.md). Lấy baseline nhánh/task và commit báo cáo thực tế từ HANDOFF/biên nhận mới nhất; trạng thái BLOCKED cũ là lịch sử. Ownership/CHUA_NHAN giữ nguyên, người mới NOT_RUN; không push.\n\n'
    p.write_text(note+t,encoding='utf-8')
print('Updated nine current pointers, preserving all historical records and task ownership.')
