// Nhích camera khi đứng yên theo nhánh legacy; không gộp với owner camera của hành trình.
// Component trong scene r92: Minh_RegionalWalk_180cm.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): camTransform.
using UnityEngine;
using System.Collections;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "ComplianceSystem")]
public class NhichCameraKhiDungYen : MonoBehaviour
{
    public Transform camTransform;
    public float nguongDungYen = 4f;
    public Vector3 offsetNhich = new Vector3(0.05f, 0.02f, 0f);
    public float thoiGianNhich = 1.2f;

    private float thoiGianDungYenHienTai = 0f;
    private bool dangNhich = false;
    private Vector3 viTriGoc;

    void Start()
    {
        if (camTransform != null)
            viTriGoc = camTransform.localPosition;
    }

    void Update()
    {
        if (QuanLyTrangThaiGame.Instance != null && QuanLyTrangThaiGame.Instance.DaKetThuc) return;

        bool dangDiChuyen = Mathf.Abs(Input.GetAxis("Horizontal")) > 0.01f
                          || Mathf.Abs(Input.GetAxis("Vertical")) > 0.01f
                          || Mathf.Abs(Input.GetAxis("Mouse X")) > 0.01f
                          || Mathf.Abs(Input.GetAxis("Mouse Y")) > 0.01f;

        if (dangDiChuyen)
        {
            thoiGianDungYenHienTai = 0f;
        }
        else
        {
            thoiGianDungYenHienTai += Time.deltaTime;
            if (thoiGianDungYenHienTai >= nguongDungYen && !dangNhich)
            {
                dangNhich = true;
                StartCoroutine(NhichCamera());
            }
        }
    }

    private IEnumerator NhichCamera()
    {
        Vector3 viTriMoi = viTriGoc + offsetNhich;

        float t = 0f;
        while (t < thoiGianNhich / 2f)
        {
            t += Time.deltaTime;
            camTransform.localPosition = Vector3.Lerp(viTriGoc, viTriMoi, t / (thoiGianNhich / 2f));
            yield return null;
        }

        t = 0f;
        while (t < thoiGianNhich / 2f)
        {
            t += Time.deltaTime;
            camTransform.localPosition = Vector3.Lerp(viTriMoi, viTriGoc, t / (thoiGianNhich / 2f));
            yield return null;
        }

        camTransform.localPosition = viTriGoc;
        dangNhich = false;
        thoiGianDungYenHienTai = 0f;
    }
}