# Output kiểm hiện hành

Xem [Test/README](../Test/README.md). `CONROI_TEST_ROOT` mặc định project/Test; đặt trước khi mở Editor. KiemUnity.ps1 nhận -TestRoot/-OutputDirectory/-BuildOutput/-ReadOnly; DieuKhienNhapLieu.py nhận --folder/--media-folder; DongGoiBanSapXep.py nhận --build/--output-root/--episode. Report mới từ hai công cụ bảo toàn vào Test/KetQua theo timestamp. GuiLenhUnity.py và Unity/GuiNhapLieuUnity.ps1 dùng cùng root với Editor bridge.

DuongDanKiem.py --resolve <đường-cũ>: tra file bằng chứng/player đã chuyển, không copy/symlink. ToChucDuLieuKiem.py verify: đối chiếu SHA256 journal; không chạy lại plan/apply đã chốt. Các công cụ tổ chức/đổi tên/chú thích đợt trước là hồ sơ hành chính; không chạy lại lên source hiện hành. Các module/command lịch sử chưa được coi là lane kiểm mới: giữ đầu vào Docs, cần cấu hình trước khi tái sử dụng. Audit bảo toàn checkpoint cũ có thể ghi nhận thay đổi Editor output/read-only có chủ đích; đối chiếu protected-final.json của đợt này, không bỏ qua khác biệt gameplay.

---

# Công cụ hiện hành

- KiemUnity.ps1: mở/kiểm/build bằng helper Editor cũ; root tính từ PSScriptRoot, tag/output mới không ghi đè.
- GuiLenhUnity.py và Unity/GuiNhapLieuUnity.ps1: bridge có sẵn/input OS theo PID.
- SapXepDuAn.py, ChuThichScript.py: bảng đổi tên/chú thích đợt này, không áp lại nhóm đã move.
- KiemTraBaoToan.py: đối chiếu token C#, GUID, YAML với checkpoint.
- KiemKeSapXep.py: audit trước chuyển đã chạy, không ghi đè checkpoint.
- python-deps: bên thứ ba, giữ cấu trúc.

Công cụ lịch sử/module giữ trong Docs/Archives/ToolsBeforeRestructure; xem tool-moves.json, không chạy author/repair lịch sử lên r92 mới.

- DieuKhienNhapLieu.py: kế thừa helper OS r90; --boot/--folder profile thử riêng, tự F5 sau mỗi capture; khi kiểm F6 phải phục hồi save thử gốc trước tải.
- DongGoiBanSapXep.py: ZIP và hash/extract, từ chối ghi đè.
- KiemDuLieuCanGiu.py: hash ZIP gốc, 17 save, Blender, checkpoint.
- CapNhatHoSo.py: bổ sung hồ sơ một lần, không chạy lặp.
