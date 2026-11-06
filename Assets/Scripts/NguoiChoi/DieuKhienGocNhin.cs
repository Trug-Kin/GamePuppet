// Xoay người/camera từ input nhìn; hành trình sở hữu pause và cursor, tránh mở khóa lại sau resume.
// Component trong scene r92: Minh_RegionalWalk_180cm.
// Code tự tìm/tạo: DieuKhienDiChuyen, DieuPhoiHanhTrinh, Camera.
using UnityEngine;
using UnityEngine.InputSystem;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "MouseMovement")]
public class DieuKhienGocNhin : MonoBehaviour
{
 
    public float mouseSensitivity = 100f;
 
    float xRotation = 0f;
    DieuKhienDiChuyen player;
    Transform eye;
    DieuPhoiHanhTrinh journey;
    public void RestorePitch(float pitch){xRotation=Mathf.Clamp(Mathf.DeltaAngle(0,pitch),-90,90);}
 
    void Start()
    {
      player = GetComponentInParent<DieuKhienDiChuyen>();
      if (!player) player = GetComponent<DieuKhienDiChuyen>();
      journey = FindAnyObjectByType<DieuPhoiHanhTrinh>();
      var camera = player ? player.GetComponentInChildren<Camera>() : null;
      eye = camera ? camera.transform : transform;
      xRotation = Mathf.DeltaAngle(0, eye.localEulerAngles.x);
      //Locking the cursor to the middle of the screen and making it invisible
      Cursor.lockState = CursorLockMode.Locked;
      Cursor.visible = false;
    }
 
    void Update()
    {
       // Dung han khi da vao ending.
       if (QuanLyTrangThaiGame.Instance != null && QuanLyTrangThaiGame.Instance.DaKetThuc) return;

       if (!player) return;
       // Journey owns Esc pause/resume and cursor capture together. Unlocking
       // again here after its resume left live movement with an unlocked mouse.
       if (!journey && Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
       { Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
       if (Mouse.current != null && (Mouse.current.leftButton.wasPressedThisFrame || Mouse.current.rightButton.wasPressedThisFrame))
       { Cursor.lockState = CursorLockMode.Locked; Cursor.visible = false; }
       if (Cursor.lockState != CursorLockMode.Locked) return;
       float mouseX = player.LookInput.x * mouseSensitivity * .0002f;
       float mouseY = player.LookInput.y * mouseSensitivity * .0002f;
 
       //control rotation around x axis (Look up and down)
       xRotation -= mouseY;
 
       //we clamp the rotation so we cant Over-rotate (like in real life)
       xRotation = Mathf.Clamp(xRotation, -90f, 90f);
 
       //control rotation around y axis (Look up and down)
       player.transform.Rotate(Vector3.up, mouseX, Space.World);
 
       //applying both rotations
       eye.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
 
    }
}
