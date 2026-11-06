// Đổi màu/volume và âm drone theo trạng thái thức/mơ; tìm API volume bằng reflection để tương thích pipeline.
// Không gắn trực tiếp trong r92; checkpoint ConRoi.unity: "CON R\u1ED0I \u2014 GDD v2.1".
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): droneMo.
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "DreamStateVisuals")]
public class HinhAnhTrangThaiMo : MonoBehaviour
{
    public Color mauThuc = Color.white;
    public Color mauMo = new Color(0.7f, 0.7f, 0.7f, 1f);
    public float doDoiMau = 0.5f;
    public AudioSource droneMo;

    [Header("Mo - giam bao hoa (conroi.txt 4.6)")]
    [Range(-100f, 0f)] public float doBaoHoaMo = -30f;

    private ScriptableObject profileMo;
    private Component volumeMo;
    private bool volumeSupported;

    private void Start()
    {
        // Scene dang o ambient mode Skybox voi skybox bi thieu (guid chet), nen ambient
        // hien tai khong xac dinh. Chuyen sang Flat de mau ambient thuc su hien ra.
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
        TaoVolumeMo();

        if (QuanLyTrangThaiGame.Instance != null)
        {
            QuanLyTrangThaiGame.Instance.TrangThaiChanged += DoiTrangThai;
            DoiTrangThai(QuanLyTrangThaiGame.Instance.TrangThaiHienTai);
        }
    }

    private void OnDestroy()
    {
        if (QuanLyTrangThaiGame.Instance != null) QuanLyTrangThaiGame.Instance.TrangThaiChanged -= DoiTrangThai;
        if (profileMo != null) Destroy(profileMo);
        RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Skybox;
    }

    // URP/Volume la optional dependency. Neu package khong ton tai thi script van chay
    // ma khong crash scene, nhung khong ap dung post-process cho dream state.
    private void TaoVolumeMo()
    {
        Type volumeType = Type.GetType("UnityEngine.Rendering.Volume, Unity.RenderPipelines.Core.Runtime");
        Type profileType = Type.GetType("UnityEngine.Rendering.VolumeProfile, Unity.RenderPipelines.Core.Runtime");
        Type colorAdjustmentsType = Type.GetType("UnityEngine.Rendering.Universal.ColorAdjustments, Unity.RenderPipelines.Universal.Runtime");

        if (volumeType == null || profileType == null || colorAdjustmentsType == null)
        {
            volumeSupported = false;
            Debug.LogWarning("DreamStateVisuals: Volume/URP not available; skip post-processing for dream state.");
            return;
        }

        GameObject volumeObject = new GameObject("DreamAtmosphere_Runtime");
        volumeObject.transform.SetParent(transform, false);

        volumeMo = volumeObject.AddComponent(volumeType);
        volumeSupported = volumeMo != null;

        if (!volumeSupported) return;

        SetPropertyValue(volumeMo, "isGlobal", true);
        SetPropertyValue(volumeMo, "priority", 100f);
        SetPropertyValue(volumeMo, "weight", 0f);

        profileMo = ScriptableObject.CreateInstance(profileType) as ScriptableObject;
        if (profileMo == null) return;

        MethodInfo addMethod = profileType
            .GetMethods(BindingFlags.Public | BindingFlags.Instance)
            .FirstOrDefault(m => m.Name == "Add" && m.IsGenericMethodDefinition);

        if (addMethod == null) return;

        object colorAdjustments = addMethod.MakeGenericMethod(colorAdjustmentsType).Invoke(profileMo, null);
        if (colorAdjustments == null) return;

        object saturation = GetPropertyValue(colorAdjustments, "saturation");
        if (saturation == null) return;

        SetPropertyValue(saturation, "overrideState", true);
        SetPropertyValue(saturation, "value", doBaoHoaMo);

        SetPropertyValue(volumeMo, "profile", profileMo);
    }

    private void DoiTrangThai(QuanLyTrangThaiGame.TrangThai trangThai)
    {
        bool dangMo = trangThai == QuanLyTrangThaiGame.TrangThai.Mo;

        RenderSettings.ambientLight = dangMo ? mauMo : mauThuc;

        if (volumeSupported && volumeMo != null)
        {
            SetPropertyValue(volumeMo, "weight", dangMo ? 1f : 0f);
        }

        if (droneMo == null) return;
        if (dangMo && !droneMo.isPlaying) droneMo.Play();
        if (!dangMo && droneMo.isPlaying) droneMo.Stop();
    }

    private static void SetPropertyValue(object target, string propertyName, object value)
    {
        if (target == null) return;

        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (property != null && property.CanWrite)
        {
            property.SetValue(target, value);
            return;
        }

        FieldInfo field = target.GetType().GetField(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (field != null) field.SetValue(target, value);
    }

    private static object GetPropertyValue(object target, string propertyName)
    {
        if (target == null) return null;

        PropertyInfo property = target.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        if (property != null) return property.GetValue(target);

        FieldInfo field = target.GetType().GetField(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
        return field != null ? field.GetValue(target) : null;
    }
}