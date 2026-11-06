# Nguồn asset r104

Đã kiểm `Assets/Model/Characters`:191 OBJ, chưa thấy rig/animation hoặc hồ sơ license/credit trong Assets/Model. Không dùng nhóm này để tránh mặc định quyền sử dụng.

## Gói đã tải và tích hợp

**Quaternius — Ultimate Animated Characters**. Trang chính thức: https://quaternius.com/packs/ultimatedanimatedcharacter.html ; folder chính thức https://drive.google.com/drive/folders/1sNi1AfenfPRrvRt5yfaj5QMMd6KKcUJ5 ; Blend folder `1X0armnO9EePmnQwP5EJHxpCiuucwG_uQ`.

Đúng file license tải kèm `Source/Downloads/Quaternius_R104/animated-license.txt`: **CC0 1.0 Universal**, https://creativecommons.org/publicdomain/zero/1.0/ . Cho phép dùng/chỉnh trong game thương mại, không bắt buộc attribution; credit tự nguyện “Character bases by Quaternius (CC0), adapted for Con Rối”. Không khẳng định đây là license của mọi asset Quaternius. Upstream không công bố version cho các file đã chọn; nhận diện bằng SHA256/ngày tải05/11/2026.

| File gốc / Drive ID | Thực tế dùng |
|---|---|
| Casual_Male.blend / 1Mig1cMlBWgQ_H8FxlWkfNeXFkNeclyCl | Khải, tỷ lệ, blue outfit, cuff rách, idle/rig |
| Casual_Female.blend / 17O7eSD7xunnN10MQiQcFOQ_EPvRR9_M3 | Hà, tỷ lệ/tạp dề, idle/rig |
| BaseCharacter.blend / 1lSCK-VuY3SPqP-2T-n8b6W6glUbhRStz | R03/R12/cipher/clip puppet, gỗ/khớp/pose |
| animated-license.txt / 1eooYt7zHAMOPEU6d3QHMzR2p0gk9ZJF8 | License đúng gói trên |

Nguồn sửa Blender trong R104; asset URP trong Art/InvestigationV36/R104. Rối dùng base chỉnh được khớp; không tuyên bố đây là puppet final có kết cấu cơ khí thực.

## Ứng viên không tích hợp / art tạm

Casual_Hoodie.blend, Casual_2.blend từ gói Modular Males và license.txt tải riêng: đánh giá nhưng không dùng trong scene; không nhập cả package. Universal Base Characters itch trảHTTP429, chuyển ứng viên khác từ nguồn chính thức. Kenney chỉ đánh giá cũ, không tích hợp.

Tư sự kiện và hình nộm trên gác bàn dựng: art tự dựng prototype kế thừa r103; operator dưới gác đã dùng Khải Quaternius, sửa layer29 và native rig/idle; Khang góc nhìn thứ nhất dùng controller gốc không có full-body art mới. Map/nhà/cơ cấu/vật chứng tự dựng, không thay map bằng gói mạng. Clip vẫn illustrative silent.

## Hash tải gốc

- `animated-license.txt` · SHA256 `83d8959f9fc56353ed571fbe2dc52e4bcd64508e2399501cd45ac2ce3df0bf8c`
- `BaseCharacter.blend` · SHA256 `bf88b7b60506799c2087aad6a2141834695f4974f94ed16f0ba04c9055f74518`
- `Casual_2.blend` · SHA256 `8b033ab6343c8ef095fb6ed1bb46ea8308f449ae2b7241b3a9b1413314e3d016`
- `Casual_Female.blend` · SHA256 `df3dae4a16a7e331c0b7662f768cb4eabe8fb0863e9ab09cbfdf3031cf8d8955`
- `Casual_Hoodie.blend` · SHA256 `d138d37cfb3f71400455bcbd9363a69d6e3e6f7288aede8b5bfabcf1d3a14816`
- `Casual_Male.blend` · SHA256 `ca9c264a4549eb664c6945db053ea2db1d0809a98ffc55cc2800e5dd09ecf55f`
- `license.txt` · SHA256 `e8dbf915a2b82229913e301a0787696611241bdefec4832bc084f54161db1efe`
