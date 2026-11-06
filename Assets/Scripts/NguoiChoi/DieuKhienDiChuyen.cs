// Đọc Input System và dịch chuyển CharacterController; hook input thử chỉ hoạt động trong scene kiểm riêng.
// Component trong scene r92: Minh_RegionalWalk_180cm.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): controller, groundCheck.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PlayerMovement")]
public class DieuKhienDiChuyen : MonoBehaviour
{
    InputActionMap gameplay;
    InputAction movement, look, jump;
    public Vector2 LookInput => look != null && look.enabled ? look.ReadValue<Vector2>() : Vector2.zero;
    public bool GameplayActionsEnabled => gameplay != null && gameplay.enabled;
    public Vector2 LastMovementInput { get; private set; }
    public Vector3 RequestedDisplacement { get; private set; }
    public Vector3 ActualDisplacement { get; private set; }
    public int MovementFrames { get; private set; }
    public int JumpCount { get; private set; }
    public float VerticalSpeed => velocity.y;
    public CollisionFlags HorizontalFlags { get; private set; }
    public CollisionFlags VerticalFlags { get; private set; }
    public bool GroundedAfterHorizontal { get; private set; }
    public bool MovementAppliedThisFrame { get; private set; }

    void OnEnable()
    {
        if (gameplay == null)
        {
            gameplay = new InputActionMap("Gameplay");
            movement = gameplay.AddAction("Move", InputActionType.Value);
            movement.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/w").With("Down", "<Keyboard>/s")
                .With("Left", "<Keyboard>/a").With("Right", "<Keyboard>/d");
            movement.AddCompositeBinding("2DVector")
                .With("Up", "<Keyboard>/upArrow").With("Down", "<Keyboard>/downArrow")
                .With("Left", "<Keyboard>/leftArrow").With("Right", "<Keyboard>/rightArrow");
            look = gameplay.AddAction("Look", InputActionType.Value, "<Mouse>/delta");
            jump = gameplay.AddAction("Jump", InputActionType.Button, "<Keyboard>/space");
        }
        gameplay.Enable();
    }
    void OnDisable() { gameplay?.Disable(); DisableTestInputHook(); }
    void OnDestroy() { gameplay?.Dispose(); }
    public CharacterController controller;
 
    public float speed = 12f;
    public float gravity = -9.81f * 2;
    public float jumpHeight = 3f;
 
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    [Tooltip("Use raw movement input in the dedicated movement test scene; leave off for normal gameplay.")]
    public bool useImmediateMovementInput = false;

    // Runtime-only input provider for the dedicated automated movement scene.
    internal System.Func<Vector2> movementInputForTesting;
    public System.Func<Vector2,Vector2> MovementFilter {get;set;}

    [Tooltip("Explicit opt-in for automated test input; off for manual tests and gameplay.")]
    public bool enableAutomatedTestInput = false;

    public bool TestInputHookActive => Application.isPlaying && enableAutomatedTestInput
        && movementInputForTesting != null && gameObject.scene.name == "DaLatMovementTest";

    public void DisableTestInputHook()
    {
        enableAutomatedTestInput = false;
        movementInputForTesting = null;
    }

    public Vector2 ReadMovementInput()
    {
        if (TestInputHookActive)
            return movementInputForTesting();
        var raw=movement != null && movement.enabled ? movement.ReadValue<Vector2>() : Vector2.zero;
        return MovementFilter!=null?MovementFilter(raw):raw;
    }
 
    Vector3 velocity;
    Vector3 pendingDisplacement;
    Vector3 supportNormal = Vector3.up, sweepNormal;
    bool sweepHasSupport;
    public void ResetVerticalMotion() { velocity=Vector3.zero; pendingDisplacement=Vector3.zero; supportNormal=Vector3.up; }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float minimumY = Mathf.Cos(controller.slopeLimit * Mathf.Deg2Rad);
        if (hit.normal.y >= minimumY && hit.point.y <= controller.bounds.min.y + controller.radius + controller.skinWidth
            && (!sweepHasSupport || hit.normal.y > sweepNormal.y))
        { sweepNormal=hit.normal; sweepHasSupport=true; }
    }
 
    bool isGrounded;
 
    static readonly Unity.Profiling.ProfilerMarker taskMarkerV46 = new Unity.Profiling.ProfilerMarker("V46.Player.Movement");
    void Update(){using(taskMarkerV46.Auto()) UpdateMeasuredV46();}
    void UpdateMeasuredV46()
    {
        // Dung han khi da vao ending (truoc day nguoi choi van di lai duoc sau ending).
        if (QuanLyTrangThaiGame.Instance != null && QuanLyTrangThaiGame.Instance.DaKetThuc) return;

        //checking if we hit the ground to reset our falling velocity, otherwise we will fall faster the next time
        // A proximity sphere can overlap a slope or wall while the capsule is airborne.
        // Only the previous collision result may reset falling motion or permit a jump.
        isGrounded = controller.isGrounded;
 
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }
 
        Vector2 input = ReadMovementInput();
        LastMovementInput = input;
        MovementFrames++;
        Vector3 before = transform.position;
        float x = input.x;
        float z = input.y;
 
        //right is the red Axis, foward is the blue axis
        Vector3 move = transform.right * x + transform.forward * z;
 
        RequestedDisplacement = move * speed * Time.deltaTime;
 
        //check if the player is on the ground so he can jump
        bool jumping = jump != null && jump.WasPressedThisFrame() && isGrounded;
        if (jumping)
        {
            JumpCount++;
            //the equation for jumping
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
 
        velocity.y += gravity * Time.deltaTime;

        // Walk tangent to the support actually contacted by the last sweep.
        // Horizontal speed is unchanged; gravity remains independent. This is
        // locomotion on a slope, not a ground snap or an assigned grounded flag.
        // A jump or an airborne capsule always uses free vertical motion.
        if (isGrounded && !jumping && supportNormal.y > .001f)
        {
            float downhillTravel = -(supportNormal.x * RequestedDisplacement.x + supportNormal.z * RequestedDisplacement.z) / supportNormal.y;
            // Upward travel is resolved by the controller's existing slope/step
            // solver. Adding a positive contact tangent can launch the capsule
            // when a rounded edge contact overestimates the surface incline.
            if (downhillTravel < 0)
                RequestedDisplacement = new Vector3(RequestedDisplacement.x, downhillTravel, RequestedDisplacement.z);
        }
 
        // Resolve horizontal and vertical travel together. Two separate Move calls
        // discard the first call's contact state and lose the downhill sweep.
        pendingDisplacement += RequestedDisplacement + velocity * Time.deltaTime;
        MovementAppliedThisFrame = pendingDisplacement.sqrMagnitude >= controller.minMoveDistance * controller.minMoveDistance
            && (velocity.y >= 0 || Mathf.Abs(pendingDisplacement.y) >= controller.minMoveDistance);
        // PhysX skips movement below minMoveDistance, returning no contact even
        // when standing on a floor. Accumulate sub-threshold travel instead of
        // issuing a sweep that cannot execute. Grounded remains the engine's
        // result from the last actual sweep, never a probe or assigned value.
        // PhysX also applies minMoveDistance to the residual slide after hitting
        // a wall. At very high render rates, horizontal travel could pass this
        // threshold while gravity's component could not: the wall sweep then
        // discarded the downward residual and returned Sides only on a floor.
        // Retain that real downward travel until its sweep can execute too.
        if (MovementAppliedThisFrame)
        {
            sweepHasSupport=false;
            VerticalFlags = controller.Move(pendingDisplacement);
            pendingDisplacement = Vector3.zero;
            HorizontalFlags = VerticalFlags;
            GroundedAfterHorizontal = controller.isGrounded;
            supportNormal = controller.isGrounded && sweepHasSupport ? sweepNormal : Vector3.up;
        }
        ActualDisplacement = transform.position - before;
    }
}
