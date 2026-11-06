using UnityEngine;

public class GrayboxPrototypeBootstrap : MonoBehaviour
{
    public Transform player;
    public Camera playerCamera;
    public bool taoKhiBatDau = true;
    public bool xoaGrayboxCu = true;

    private Transform grayboxRoot;
    private int groundLayer;

    [Tooltip("So dau vet phai phat hien trong khu vuc nay. Dung chung cho DreamRegionController va ConRoiGameManager de khong lech nhau.")]
    public int soDauVetTrongKhuVuc = 4;

    private void Start()
    {
        if (!taoKhiBatDau) return;

        TimPlayerVaCamera();
        if (player == null)
        {
            Debug.LogError("GrayboxPrototypeBootstrap: khong tim thay GameObject nao co tag \"Player\". Khong tao graybox (tranh dung sai vi tri goc).");
            return;
        }
        if (groundLayer < 0)
        {
            Debug.LogError("GrayboxPrototypeBootstrap: layer \"Ground\" khong ton tai. Khong tao graybox.");
            return;
        }

        TaoManager();
        TaoDreamState();
        TaoPlayerInteraction();
        TaoGraybox();
    }

    private void TimPlayerVaCamera()
    {
        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null) player = playerObject.transform;
        }

        if (playerCamera == null) playerCamera = Camera.main;
        if (playerCamera == null) Debug.LogWarning("GrayboxPrototypeBootstrap: khong tim thay Camera.main, PlayerInteraction se khong tuong tac duoc.");
        groundLayer = LayerMask.NameToLayer("Ground");
    }

    private void TaoManager()
    {
        if (TienTrinhTroChoi.Instance == null)
        {
            GameObject progressObject = new GameObject("GameProgress_Runtime");
            progressObject.AddComponent<TienTrinhTroChoi>();
        }

        if (QuanLyTrangThaiGame.Instance == null)
        {
            GameObject managerObject = new GameObject("ConRoiGameManager_Runtime");
            managerObject.AddComponent<QuanLyTrangThaiGame>();
        }
    }

    // P0-3: DreamStateVisuals truoc day khong duoc tao o bat ky dau, nen trang thai
    // Thuc va Mo giong het nhau. EndingPresenter thi lang nghe EndingTriggered.
    private void TaoDreamState()
    {
        GameObject dreamStateObject = new GameObject("DreamState_Runtime");
        dreamStateObject.AddComponent<HinhAnhTrangThaiMo>();
        dreamStateObject.AddComponent<TrinhBayKetThuc>();
    }

    private void TaoPlayerInteraction()
    {
        if (player == null) return;

        TuongTacNguoiChoi interaction = player.GetComponent<TuongTacNguoiChoi>();
        if (interaction == null) interaction = player.gameObject.AddComponent<TuongTacNguoiChoi>();
        interaction.playerCamera = playerCamera;
    }

    private void TaoGraybox()
    {
        GameObject oldRoot = GameObject.Find("RuntimeGraybox");
        if (oldRoot != null && xoaGrayboxCu) Destroy(oldRoot);
        if (grayboxRoot != null) return;

        grayboxRoot = new GameObject("RuntimeGraybox").transform;
        Vector3 origin = player == null ? Vector3.zero : player.position;
        origin.y -= 1f;

        // P0-5: san/tuong phai bao het corridor. San cu (dai 42, tam +8) chi toi z+29,
        // trong khi Evidence_04 o +31, Wake_Trigger o +37 va Ending_Interact o +40
        // nam ngoai san, tren terrain tho.
        TaoKhoi("Floor", origin + new Vector3(0f, -0.5f, 14f), new Vector3(18f, 1f, 56f), Color.gray, true);
        TaoKhoi("Wall_Left", origin + new Vector3(-9f, 2f, 14f), new Vector3(1f, 5f, 56f), Color.gray, true);
        TaoKhoi("Wall_Right", origin + new Vector3(9f, 2f, 14f), new Vector3(1f, 5f, 56f), Color.gray, true);
        TaoKhoi("Wall_End", origin + new Vector3(0f, 2f, 42.5f), new Vector3(18f, 5f, 1f), Color.gray, true);

        DatPlayerLenMatSan(origin);

        GameObject bed = TaoKhoi("Bed_Interact", origin + new Vector3(-5f, 0.6f, 2f), new Vector3(3f, 1f, 5f), Color.blue, false);
        bed.AddComponent<TuongTacGiuong>();

        // Ba hanh dong doi thuc (conroi.txt 5.3: +1 dang bai, +3 nghe may, -1 bo qua)
        // nam tren ba doi tuong rieng, vi moi doi tuong chi tinh diem mot lan.
        TaoPhone(origin + new Vector3(4f, 0.7f, 0f), "Phone_DangBai", TuongTacDienThoai.HanhDongDienThoai.DangBai);
        TaoPhone(origin + new Vector3(4f, 0.7f, 3f), "Phone_NgheMay", TuongTacDienThoai.HanhDongDienThoai.NgheMay);
        TaoPhone(origin + new Vector3(4f, 0.7f, 6f), "Phone_BoQua", TuongTacDienThoai.HanhDongDienThoai.BoQua);

        GameObject fallback = new GameObject("Hub_SleepFallback");
        fallback.transform.SetParent(grayboxRoot);
        fallback.AddComponent<DuPhongNguTinh>();

        DieuKhienConRoi puppet = TaoPuppet(origin + new Vector3(0f, 1f, 7f));
        DieuKhienKhuMo region = TaoRegion(origin, soDauVetTrongKhuVuc);

        // P0-4: truoc day TongDauVet luon la hang so 4 trong Inspector, nen nguong
        // "Nhan thuc cao" khong lien quan gi den so dau vet thuc co trong scene.
        QuanLyTrangThaiGame.Instance?.DatTongDauVet(soDauVetTrongKhuVuc);
        TaoEvidence(origin + new Vector3(-5f, 0.5f, 13f), "Evidence_01", puppet, region, VungKichHoatDauVet.PhaseSauKhiPhatHien.GD1);
        TaoEvidence(origin + new Vector3(5f, 0.5f, 19f), "Evidence_02", puppet, region, VungKichHoatDauVet.PhaseSauKhiPhatHien.GD2);
        TaoEvidence(origin + new Vector3(-5f, 0.5f, 25f), "Evidence_03", puppet, region, VungKichHoatDauVet.PhaseSauKhiPhatHien.GD3);
        TaoEvidence(origin + new Vector3(5f, 0.5f, 31f), "Evidence_04", puppet, region, VungKichHoatDauVet.PhaseSauKhiPhatHien.KhongDoi);

        GameObject wake = TaoKhoi("Wake_Trigger", origin + new Vector3(0f, 1f, 37f), new Vector3(8f, 2f, 1f), Color.yellow, false);
        BoxCollider wakeCollider = wake.GetComponent<BoxCollider>();
        wakeCollider.isTrigger = true;
        wake.AddComponent<VungKichHoatTinhDay>();

        GameObject ending = TaoKhoi("Ending_Interact", origin + new Vector3(0f, 1f, 40f), new Vector3(2f, 2f, 1f), Color.red, false);
        KichHoatKetThuc endingTrigger = ending.AddComponent<KichHoatKetThuc>();
        // Graybox hien chi co 1 khu vuc that (corridor nay), nen nguong = 1.
        // Khi du 4 khu vuc (Ho Xuan Huong -> Cho dem -> Truong -> Doc) doi thanh 4.
        endingTrigger.soKhuVucCanHoanTat = 1;
    }

    private void TaoPhone(Vector3 position, string objectName, TuongTacDienThoai.HanhDongDienThoai hanhDong)
    {
        GameObject phone = TaoKhoi(objectName, position, new Vector3(0.8f, 0.2f, 1.4f), Color.green, false);
        TuongTacDienThoai phoneInteractable = phone.AddComponent<TuongTacDienThoai>();
        phoneInteractable.hanhDong = hanhDong;
    }

    // Player khoi tao voi chan o y = 0.65 trong khi mat san la y = 1.5, nen capsule bi
    // nhun vao san ngay frame dau. Dat lai cho chan dung tren mat san.
    private void DatPlayerLenMatSan(Vector3 origin)
    {
        if (player == null) return;

        CharacterController controller = player.GetComponent<CharacterController>();
        float nuaChieuCao = controller != null ? controller.height * 0.5f : 1f;

        Vector3 viTriMoi = player.position;
        viTriMoi.y = origin.y + nuaChieuCao;

        if (controller != null) controller.enabled = false;
        player.position = viTriMoi;
        if (controller != null) controller.enabled = true;
    }

    private DieuKhienConRoi TaoPuppet(Vector3 position)
    {
        GameObject puppetObject = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        puppetObject.name = "Puppet_Graybox";
        puppetObject.transform.SetParent(grayboxRoot);
        puppetObject.transform.position = position;
        puppetObject.transform.localScale = new Vector3(0.8f, 1.2f, 0.8f);
        puppetObject.GetComponent<Renderer>().material.color = Color.black;

        DieuKhienConRoi puppet = puppetObject.AddComponent<DieuKhienConRoi>();
        puppet.playerTransform = player;
        puppet.dauTransform = puppetObject.transform;
        return puppet;
    }

    private DieuKhienKhuMo TaoRegion(Vector3 origin, int soDauVetCanPhatHien)
    {
        GameObject regionObject = new GameObject("DreamRegion_Graybox");
        regionObject.transform.SetParent(grayboxRoot);
        regionObject.transform.position = origin;
        DieuKhienKhuMo region = regionObject.AddComponent<DieuKhienKhuMo>();
        region.soDauVetCanPhatHien = Mathf.Max(1, soDauVetCanPhatHien);

        // P0-1: truoc day autoTinhDay = false VA OnRegionCompleted khong co nguoi nghe,
        // nen phat hien du 4/4 dau vet cung khong co gi xay ra. Gio hoan tat khu vuc se
        // tu tinh day, dong thoi cong KhuVucHienTai (EndingTrigger can KhuVucHienTai >= 1).
        region.tuDongTinhDayKhiHoanTat = true;
        region.OnRegionCompleted.AddListener(HoanTatKhuVuc);
        return region;
    }

    private void HoanTatKhuVuc()
    {
        QuanLyTrangThaiGame.Instance?.ChuyenKhuVucTiepTheo();
    }

    private void TaoEvidence(Vector3 position, string objectName, DieuKhienConRoi puppet, DieuKhienKhuMo region, VungKichHoatDauVet.PhaseSauKhiPhatHien phase)
    {
        GameObject evidence = TaoKhoi(objectName, position, new Vector3(2f, 2f, 2f), Color.magenta, false);
        evidence.GetComponent<BoxCollider>().enabled = false;
        SphereCollider trigger = evidence.AddComponent<SphereCollider>();
        trigger.isTrigger = true;
        trigger.radius = 1.5f;

        VungKichHoatDauVet evidenceTrigger = evidence.AddComponent<VungKichHoatDauVet>();
        evidenceTrigger.conRoi = puppet;
        evidenceTrigger.phaseSauKhiPhatHien = phase;
        evidenceTrigger.OnActivated.AddListener(region.GhiNhanDauVet);
    }

    private GameObject TaoKhoi(string objectName, Vector3 position, Vector3 scale, Color color, bool laMatDat)
    {
        GameObject block = GameObject.CreatePrimitive(PrimitiveType.Cube);
        block.name = objectName;
        block.transform.SetParent(grayboxRoot);
        block.transform.position = position;
        block.transform.localScale = scale;
        block.GetComponent<Renderer>().material.color = color;
        if (laMatDat) block.layer = groundLayer;
        return block;
    }
}