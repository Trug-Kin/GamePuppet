# Đường dẫn cũ → mới

Các thay đổi thực sự phải tra `archive-manifest.jsonl` (từng file, SHA256/trạng thái), `archive-result.json` và `unity-changes.json` nếu đã thực hiện. Không suy từ kế hoạch là đã chuyển.

| Cũ | Mới / trạng thái |
|---|---|
| `D:/ConRoi` | Giữ root; kiểm repo trong báo cáo |
| `Assets/Scenes/JourneyPresentation_v48_r107.unity` | Giữ checkpoint, không rename |
| `Assets/Art/InvestigationV36/R104`, nguồn clipR105, terrainR107 | Giữ nguyên GUID/path/dependency |
| `Assets/Scripts/InvestigationV36R107` | Giữ nguyên path/type/namespace/field/save IDs; người2 giữ cả partial tạm |
| Script `.py/.ps1/.cs/.cs.txt` trong `Test/Tam` có trong manifest | `Tools/TaiTao/Legacy/<relative>`; chỉ khi MOVED_VERIFIED |
| Capture/profile/output được chọn | `<archive>/DuLieu/<old-relative>`; bản staging kiểm hash được giữ, không xóa |
| Báo cáo nhỏ r102–r107 | Bản tra trong `Docs/QA`; bản lịch sử giữ trạng thái FAIL/NOT_RUN và hash gốc |

Không replace hàng loạt `D:\` trong recipe lịch sử. Tool historical có path cũ chưa được kiểm lại phải ghi PROVISIONAL, không chạy một lần author/derive/integrate. Không có runtime path trỏ archive để thay game media.
