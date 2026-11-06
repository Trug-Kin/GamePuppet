// Raycast từ camera để gọi IPlayerInteractable; khóa tương tác theo trạng thái hành trình.
// Component trong scene r92: Minh_RegionalWalk_180cm.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): playerCamera.
// Code tự tìm/tạo: DieuPhoiHanhTrinh, DieuKhienDiChuyen, IPlayerInteractable.
using UnityEngine;
using UnityEngine.InputSystem;

public interface IPlayerInteractable
{
    void Interact();
}

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PlayerInteraction")]
public class TuongTacNguoiChoi : MonoBehaviour
{
    InputAction interact;
    DieuPhoiHanhTrinh regionalStory;
    void Awake(){regionalStory=Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();}
    public IPlayerInteractable Target { get; private set; }
    public int CompletedInteractions { get; private set; }
    void OnEnable()
    {
        interact = new InputAction("Interact", InputActionType.Button, "<Keyboard>/"+interactionKey.ToString().ToLowerInvariant());
        interact.Enable();
    }
    void OnDisable() { interact?.Dispose(); interact=null; Target=null; }
    public Camera playerCamera;
    public float interactionDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;

    private static readonly Unity.Profiling.ProfilerMarker taskMarkerV46 = new Unity.Profiling.ProfilerMarker("V46.Player.InteractionRaycast");
    void Update(){using(taskMarkerV46.Auto()) UpdateMeasuredV46();}
    void UpdateMeasuredV46()
    {
        if(regionalStory&&regionalStory.polish&&regionalStory.polish.IsReading&&interact!=null&&interact.WasPressedThisFrame()){regionalStory.NotifyInteraction();regionalStory.polish.DismissReading();return;}
        Target = null;
        if(regionalStory&&regionalStory.UiInputBlocked)return;
        if (playerCamera == null || Time.timeScale == 0 || (QuanLyTrangThaiGame.Instance != null && QuanLyTrangThaiGame.Instance.DaKetThuc)) return;

        Ray ray = new Ray(playerCamera.transform.position, playerCamera.transform.forward);
        // The view ray may cross the player's capsule when looking down. Ignore only
        // the owning player; environmental occluders still stop the interaction.
        var hits=Physics.RaycastAll(ray,interactionDistance,~0,QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits,(a,b)=>a.distance.CompareTo(b.distance));
        RaycastHit hit=default;bool found=false;
        var owner=GetComponentInParent<DieuKhienDiChuyen>();
        foreach(var candidate in hits)
        {if(owner&&candidate.collider.transform.IsChildOf(owner.transform))continue;hit=candidate;found=true;break;}
        if(!found)return;

        Target = hit.collider.GetComponentInParent<IPlayerInteractable>();
        if (Target != null && interact != null && interact.WasPressedThisFrame())
        { if(regionalStory)regionalStory.NotifyInteraction();Target.Interact(); CompletedInteractions++; }
    }
    void OnGUI()
    {
        var story=GetComponentInParent<DieuPhoiHanhTrinh>();
        if(!story)story=Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();
        if(story&&story.polish)return;
        if (Target != null && Time.timeScale > 0)
            GUI.Box(new Rect(Screen.width/2-70,Screen.height*.75f,140,32),"E · Tương tác");
    }
}
