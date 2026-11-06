// Điều khiển bốn giai đoạn bắt chước của rối; giữ hàng đợi độ trễ GĐ2 đủ 0,5 giây, bỏ delta chuyển cảnh trên 2 m.
// Component trong scene r92: ConRoi_Minh_1_2, ConRoi_Minh_1_1, ConRoi_Minh_GD3, ConRoi_Minh_1_0, ConRoi_Minh_1_3, ConRoi_Minh_GD4_FinalOnly (các instance khác xem bảng tra cứu).
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): playerTransform, dauTransform, gesturePart.
// Pause khi Time.timeScale bằng 0; teleport chuyển khu không phải hành động bắt chước.
using System.Collections.Generic;
using UnityEngine;

[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "ConRoiAI")]
public class DieuKhienConRoi : MonoBehaviour
{
    public enum GiaiDoan { GD1, GD2, GD3, GD4 }
    public GiaiDoan giaiDoanHienTai;
    public float tocDoDiChuyen=3.5f, doTreGD2=.5f, gd3NguongDungYen=1.5f;
    public Transform playerTransform, dauTransform, gesturePart;
    // Authored stations on r45 market edges; delayed motion stays local.
    public bool situatedR45;
    public Vector3 stationR45;
    public float stationRadiusR45=.28f;
    public bool HasWitnessedPhase { get; private set; }
    struct Motion { public float time; public Vector3 delta; }
    readonly Queue<Motion> history=new Queue<Motion>();
    Vector3 previous;
    float idle;
    float witnessedMotion;
    bool leading;
    Quaternion gestureRest;
    void OnEnable()
    {
        if(playerTransform)previous=playerTransform.position;
        history.Clear(); idle=0;leading=false;witnessedMotion=0;
        if(gesturePart)gestureRest=gesturePart.localRotation;
    }
    void LateUpdate()
    {
        if(!playerTransform || Time.timeScale==0)return;
        Vector3 delta=playerTransform.position-previous;previous=playerTransform.position;delta.y=0;
        // Scene transitions are not an action to mimic.
        if(delta.magnitude>2) {history.Clear();return;}
        if(giaiDoanHienTai==GiaiDoan.GD1)
        {transform.position+=delta;witnessedMotion+=delta.magnitude;if(witnessedMotion>.01f)HasWitnessedPhase=true;}
        else if(giaiDoanHienTai==GiaiDoan.GD2)
        {
            // Keep motion for its full delay. A frame-count cap discarded real
            // actions before .5 s elapsed at high render rates; idle frames do
            // not need queue entries.
            if(delta.sqrMagnitude>0f)history.Enqueue(new Motion{time=Time.time,delta=delta});
            while(history.Count>0 && Time.time-history.Peek().time>=doTreGD2)
            {var motion=history.Dequeue();transform.position+=motion.delta;if(situatedR45){var offset=transform.position-stationR45;offset.y=0;offset=Vector3.ClampMagnitude(offset,stationRadiusR45);transform.position=new Vector3(stationR45.x+offset.x,transform.position.y,stationR45.z+offset.z);}witnessedMotion+=motion.delta.magnitude;if(witnessedMotion>.01f)HasWitnessedPhase=true;}
        }
        else if(giaiDoanHienTai==GiaiDoan.GD3)
        {
            if(delta.sqrMagnitude>.00001f)
            {
                if(leading)HasWitnessedPhase=true;
                idle=0;leading=false;
                if(gesturePart)gesturePart.localRotation=Quaternion.Slerp(gesturePart.localRotation,gestureRest,Time.deltaTime*4);
            }
            else
            {
                idle+=Time.deltaTime;
                if(idle>=gd3NguongDungYen)
                {
                    leading=true;
                    if(gesturePart)gesturePart.localRotation=Quaternion.Slerp(gesturePart.localRotation,gestureRest*Quaternion.Euler(-35,0,15),Time.deltaTime*3);
                    Look();
                }
            }
        }
        else {HasWitnessedPhase=true; Look();}
    }
    void Look()
    {
        if(!dauTransform)return;
        var d=playerTransform.position+Vector3.up*.7f-dauTransform.position;
        if(d.sqrMagnitude>.001f)dauTransform.rotation=Quaternion.Slerp(dauTransform.rotation,Quaternion.LookRotation(d),Time.deltaTime*3);
    }
    public void SetPhase(GiaiDoan phase)
    {
        giaiDoanHienTai=phase;history.Clear();idle=0;leading=false;HasWitnessedPhase=false;witnessedMotion=0;
        if(playerTransform)previous=playerTransform.position;
    }
    public void ChuyenSangGD1()=>SetPhase(GiaiDoan.GD1);
    public void ChuyenSangGD2()=>SetPhase(GiaiDoan.GD2);
    public void ChuyenSangGD3()=>SetPhase(GiaiDoan.GD3);
    public void ChuyenSangGD4()=>SetPhase(GiaiDoan.GD4);
}
