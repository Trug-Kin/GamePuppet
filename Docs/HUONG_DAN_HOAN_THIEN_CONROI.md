# Huong dan bo sung tai nguyen va hoan thien CON ROI trong Unity

Tai lieu nay dua tren `Docs/Design/conroi.txt` v2.1, `Docs/Audits/conroi_code_audit.txt` va kiem ke asset Blender/Unity hien co. Muc tieu la chi ro viec can lam, thu tu lam va cach kiem tra. Cac muc R0 da co trong code khong can lam lai; can Play-test trong Unity de xac nhan hanh vi thuc te.

## 1. Hien trang

### Da co

- `Assets/Art/Models/Model.blend`: nhieu mesh moi truong cho pho, nha, cho dem, truong hoc, doc Da Lat, mot so vat dung va ba bo chi tiet Con Roi (`Roi1`-`Roi3`). Con Roi co cac Empty dieu khien, nhung chua co Armature va animation.
- `Assets/Art/Models/Main_character.blend`: hai mesh va mot material; chua co Armature hay animation Action.
- `Assets/Art/Models/hoodie/source/a pose hoodie.fbx` va bon texture: albedo, metallic, normal, roughness. FBX la mesh ao khoac, chua co Armature/animation.
- `Assets/Scenes/SampleScene.unity`, terrain, URP settings va script gameplay.
- Code da co bootstrap graybox, manager/progress, ngu va tinh day, trigger dau vet, scoring, dream visuals runtime va `EndingPresenter`.
- `EndingPresenter` hien dung `OnGUI`, phu hop prototype; chua phai UI cuoi cung.

### Con thieu hoac chua san sang

- Chua co Unity prefab cho nhan vat, Con Roi, vat dung hay cac khu vuc; `Assets/Prefabs` dang rong.
- Chua co animation clip (`.anim`). `ConRoiAnimator.controller` mot minh chua lam nhan vat chuyen dong.
- Chua co AudioClip: drone, ambience, tieng day keo, tieng go, thong bao dien thoai va buoc chan.
- Chua co model Minh/Vy va cac bong ma Ong Bao, Chi Ha, Co Mai, Anh Tung, Ba Lan.
- Dau vet lich cua Ong Bao chua co; khu Ho Xuan Huong can mat nuoc/phan chieu ro hon. Sach, vali va ghe lop hoc da co trong Model.blend; hang rao co the tai su dung cho vignette Ba Lan.
- Chua co UI Canvas cho dien thoai/tin nhan va ending production.
- Tai thoi diem kiem ke, asset `.blend`, FBX hoodie va texture hoodie chua co `.meta` Unity. Mo Unity va doi import; neu khong tu import, kiem tra Console/Editor.log va Blender external tools.
- `SampleScene` van co tham chieu skybox GUID khong tim thay theo audit. Can gan skybox hop le hoac chon cach ambient khac.

## 2. Thu tu lam de tranh lam lai

### Buoc 1: Import va to chuc asset

1. Sao luu project truoc khi thay doi scene hoac asset.
2. Mo dung project `Game` bang Unity Editor, de Unity import cac file trong `Assets/Art/Models`.
3. Kiem tra Unity tao `.meta` cho `Main_character.blend`, FBX hoodie va tung texture. Xu ly loi import trong Console truoc khi keo asset vao scene.
4. Nen dung FBX lam file trao doi Blender-Unity cho nhan vat va cac model can rig. File `.blend` giu lam source; khong nen dua ca scene Blender hang nghin object vao mot prefab Unity.
5. Tao cac thu muc Unity de quan ly sau khi import: `Assets/Art/Characters`, `Assets/Art/Environment`, `Assets/Art/Props`, `Assets/Audio`, `Assets/Prefabs`, `Assets/UI`. Chi di chuyen asset sau khi da xac nhan Unity import duoc va khong lam mat reference.
6. Tach `Model.blend` thanh cac nhom/collection de export rieng: Hub, Ho, ChoDem, Truong, DocDaLat, Props va Characters. Moi FBX nen co ten, scale va origin ro rang.

**Texture hoodie trong URP:** Albedo vao Base Map; normal texture phai duoc import voi Texture Type = Normal map; roughness can doi thanh smoothness (smoothness = 1 - roughness) hoac pack vao alpha theo shader; metallic map can dung dung kenh ma shader URP Lit mong doi. Kiem tra material trong scene vi map roughness khong tu dong duoc Unity Lit hieu giong Blender.

### Buoc 2: Hoan thien Minh va Con Roi

1. Mo `Main_character.blend`, xac dinh hai mesh la than/ao hay cac bo phan nao, chinh ti le nhan vat khoang 1.7 m va dat origin o chan.
2. Gan hoodie vao nhan vat, chinh form ao de khong xuyen mesh. Hoodie hien chua co rig; can weight/parent theo skeleton cua nhan vat.
3. Tao rig humanoid cho Minh. Co the dung Rigify trong Blender de tao xương va dieu chinh weight paint; khi export sang Unity, kiem tra Avatar duoc nhan dang Humanoid.
4. Tao prefab Minh tu FBX da import. Vi game FPS, uu tien tay/canh tay va dau dung cho reflection; khong can lam body qua chi tiet neu khong len hinh.
5. Tai su dung dung khuon mat Minh cho Con Roi. Tao vat lieu go/son cu cho than, nhung khong thay doi danh tinh khuon mat theo tung bong ma.
6. Tao rig/chuyen dong cho Con Roi. Thiet ke animation clip rieng cho GD1, GD2, GD3 va GD4; kiem tra controller co Motion that, khong chi co state rong.
7. GD3 can co lead time 1-2 giay: Con Roi thuc hien cu chi truoc, sau do moi cho input tuong ung. GD4 la dung im va nhin, chi xuat hien mot lan truoc ending.
8. Export FBX kem rig/animation, import vao Unity, tao Avatar va gan `Animator Controller`. Kiem tra tung clip trong Animation Preview truoc khi gan vao prefab.

**Tieu chi xong:** Minh va Con Roi import duoc, Avatar hop le, clip chay trong preview, mat Con Roi la mat Minh, khong co loi weight/mesh ro rang.

### Buoc 3: Dung prefab va cac khu vuc

1. Bat dau voi pham vi demo cua `Docs/Design/conroi.txt`: phong tro, Ho Xuan Huong va Cho dem. Sau khi luong nay on dinh moi them Truong va Doc Da Lat.
2. Tao prefab rieng cho cac props lap lai: nha, den, cot dien, hang rao, ban ghe, quầy cho, vali, sach, lich va Con Roi.
3. Tao scene hoac prefab environment theo tung khu vuc. Khong de ca bo asset Blender trong mot scene Unity khong co phan lop.
4. Gan collider don gian cho props can va cham; trigger rieng cho dau vet. Kiem tra layer Ground, Player tag va kich thuoc theo don vi Unity.
5. Tao dau vet dung y nghia trong thiet ke:
   - Ong Bao: lich treo tuong dung o mot ngay.
   - Chi Ha: chong giao trinh chua mo.
   - Co Mai: ghe giao vien quay ve cua so.
   - Anh Tung: vali dong, phu bui, dat canh cua.
   - Ba Lan: hang rao khu vuon dan khop kin hon.
6. Them mat nuoc Ho Xuan Huong va phan chieu. Hien model co `Fountain_Base`, nhung chua du de the hien ngon ngu so “phan chieu”.
7. Dung silhouette rieng cho tung bong ma; che mat/cho quay mat di. Khong can lam bong ma thanh quai vat, va khong can lam ro mat Vy.
8. Moi scene/vung can co diem vao, diem ra, trigger khu vuc va thong tin phase Con Roi. Dung dung thu tu: Ho -> Cho Dem -> Truong -> Doc.

**Tieu chi xong:** nguoi choi di duoc tren dia hinh, khong roi khoi map; moi dau vet kich hoat dung mot lan va khong chan tien trinh neu bo qua.

### Buoc 4: Them am thanh

Thu am hoac tao cac AudioClip rieng, dung ban quyen/nguon asset ro rang:

- Drone tan so thap cho trang thai Mo.
- Ambience Ho: gio va mat nuoc yen.
- Ambience Cho dem: khong khi vang bat thuong, han che tieng dam dong.
- Ambience Truong: dong ho lop hoc.
- Ambience Doc: gio manh va tieng go lan tu xa.
- Tieng day keo gan voi Compliance.
- Tieng go ngan cho beat nhoe mat.
- Thong bao dien thoai va buoc chan co the dieu chinh theo trang thai.

Gan AudioSource vao manager/nhan vat/vung phu hop; gan clip vao field trong Inspector. Kiem tra volume, spatial blend, loop va transition. Theo spec, giu toi da hai lop hieu ung cung luc; ambience va drone la lop nen duoc mien tru.

**Tieu chi xong:** khong co AudioSource phat clip null; chuyen Thuc/Mo va doi khu vuc nghe thay doi ro nhung tiet che.

### Buoc 5: Hoan thien presentation/UI

1. Giu `EndingPresenter` OnGUI de test logic. Khi lam ban chinh, thay bang Canvas + TextMeshPro, them nut choi lai/quay ve Hub neu kich ban can.
2. Tao giao dien dien thoai don gian de hien tin nhan Vy va cac hanh dong dang bai/nghe may/bo qua. Khong hien chi so NhanThuc, HanhDong hay Compliance cho nguoi choi.
3. Them prompt tuong tac rat gon (vi du icon/nut E) va phan hoi am thanh/camera cho dau vet; tranh popup giai thich y nghia dau vet.
4. Tao mot hieu ung face-blur cross-fade khoang 0.08 giay, phat tieng go dong bo, moi bong ma mot lan.
5. Tao nam soi day bang Line Renderer hoac mesh don gian. Cap nhat mau/rung khi phat hien dau vet; GD4 lam day cang; soi thu nam doi mau theo ending.

## 3. Cong viec code con lai theo audit

Cac muc duoi day la phan audit danh dau con mo, khong phai yeu cau phai lam het truoc khi import model:

- Sua ConRoiAI dung huong di chuyen world-space cua Player thay vi tu doc input va dung truc rieng cua Con Roi; gioi han lich su input de khong tang vo han.
- Tach hanh vi GD3 va GD4. Hien enum ConRoiAI moi co GD1-GD3; logic GD3 hien tai gan voi dung im/nhin hon la “hanh dong truoc”.
- Tao cac muc Compliance Thap/Trung binh/Cao, delay input 150-250 ms o muc Cao va cap nhat tai checkpoint.
- Gan phan hoi dau vet: camera nhich ve phia dau vet, bien doi moi truong nho va am thanh; khong dung Debug.Log nhu phan hoi gameplay cuoi.
- Tao logic tin nhan Vy theo tan suat dang bai/bo qua trong spec.
- Xoa hoac tat object `ConRoi_Placeholder` va `DauVet_Test` cu neu bootstrap runtime tao ban graybox tuong ung; tranh hai Con Roi/doc lap va trigger trung.
- Kiem tra loi pitch dang ap vao Player body; pitch nen dieu khien camera, yaw dieu khien than.
- Kiem tra tuong tac raycast voi trigger va feedback khi nham/khong co target.
- Giai quyet skybox dang tham chieu GUID khong ton tai trong scene.

## 4. Cach kiem tra tung moc

1. Sau moi dot import, doi Unity import xong va dam bao Console khong co compile/import error.
2. Mo `SampleScene`, nhan Play va thu vong lap: tuong tac giuong -> Mo -> phat hien dau vet -> tinh day -> tuong tac dien thoai -> khu vuc tiep theo.
3. Thu bo qua dau vet de xac nhan khong co fail state.
4. Thu cac to hop NhanThuc/HanhDong de nhan dung A, A2, B, C; chi tinh diem qua hanh vi, khong them lua chon Yes/No.
5. Kiem tra moi clip Con Roi trong Animator va moi audio clip bang preview truoc khi Play-test.
6. Chay lai `Tools/check_dalat_model.py` trong Blender khi thay doi `Model.blend`; checker chi xac nhan nhom object theo ten, khong thay the viec xem viewport.
7. Ghi nhan ket qua Play-test rieng. Build C# thanh cong khong tu dong chung minh scene wiring, animation, audio hay collider hoat dong.

## 5. Phan biet viec bat buoc va viec co the de sau

**Can cho demo co ban:** import model Con Roi co rig, mot bo animation GD1-GD3, mot khu vuc Ho + Cho dem + Hub, dau vet vat ly, mot ending co the xem, am thanh nen/drone toi thieu va prefab co collider.

**Co the lam sau demo:** du 5 bong ma voi model rieng, Vy ba cap do, ca bon khu vuc hoan chinh, bo am thanh day du, UI cuoi cung, toi uu asset/package va test assembly.

**Trang thai xac nhan:** tai lieu nay tong hop static audit va kiem ke asset. Chua co Play-test Unity duoc thuc hien trong qua trinh viet huong dan nay.
