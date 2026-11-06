from pathlib import Path
R=Path('D:/ConRoi');p=R/'Docs/PROJECT_GUIDE_AND_HISTORY.md';s=p.read_text(encoding='utf-8');needle='**Còn riêng:** ánh xạ M2 OPEN;'
insert='''**Chốt clip sau sửa đã xem trực tiếp:** nhánh Tây11/11 điểm, clip67.07s; biên Bắc X845/Z85..138 đi–về4/4 điểm, clip34.57s. Cả hai capture1600×984, fps trung bình29.34/29.88 (nguồn yêu cầu30fps VFR), input mô phỏng và im tiếng. Hai clip là fixture cục bộ riêng. Các lượt Bắc gặp công trình tại Z137/Z161 giữ riêng trong `north-motion.txt`, `north-obstruction-motion.txt`, `north-return-motion.txt`; không dùng chúng làm PASS trọn đường dự kiến. Kiểm phase1/3/4 của rối qua memory cũ cho Ydelta0, phase/elapsed/slide và điểm không đổi (`r88-phase-load.txt`); đây là kiểm restore khi menu mở, **Paused=false**, không thay kết quả pause Escape thực tế trong `lifecycle.txt`.

'''
if insert not in s:s=s.replace(needle,insert+needle,1)
p.write_text(s,encoding='utf-8');print('Added exact final video coverage/metadata and phase-test limitation')
