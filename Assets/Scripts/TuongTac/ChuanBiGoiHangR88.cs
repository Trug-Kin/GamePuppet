// Rối chuẩn bị cơ hội giao gói hàng bằng stride hiện hữu; không chọn thay Minh.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, activity, stride, support, destination, packageDestination, head.
// Thứ tự chạy khai báo: 21050; giữ quan hệ khởi tạo/pose với các owner khác.
// Start kiểm reference bắt buộc; memory/SuccessfulLoads phối hợp restore, không chọn hộ người chơi.
using System;
using UnityEngine;

// Scene opt-in: prepare a possibility, never perform Minh's choice.
// Movement uses the existing Blender-authored PuppetStrideR23 contact trajectory.
[DefaultExecutionOrder(21050)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "MarketPreparationR88")]
public sealed class ChuanBiGoiHangR88 : MonoBehaviour
{
    [Serializable] public sealed class Memory
    {
        public int version=1, phase; // 0 waiting, 1 walk, 2 settle, 3 push, 4 wait, 5 skipped
        public float elapsed, slide;
        public Vector3 position;
        public Quaternion rotation=Quaternion.identity;
        public string stridePose="";
    }
    public DieuPhoiHanhTrinh story;
    public QuanLyGoiHangChoR35 activity;
    public BuocChanConRoiR23 stride;
    public Collider support;
    public Transform destination, packageDestination, head;
    public float triggerDistance=5, speed=.32f;
    // Scene opt-in after a ground revision; preserve old save phase/choice and XZ.
    public bool restoreOnRegradedSupport;
    public Memory memory=new Memory();
    public bool Busy=>memory.phase>=1&&memory.phase<=3;
    public Vector3 PackageOffset=>packageDelta*Mathf.Clamp01(memory.slide);
    Vector3 origin, packageDelta, handAxis, footOffset;
    float rootGroundOffset;
    Quaternion rotation, headRest, gestureRest;
    DieuPhoiHanhTrinh.State owner;
    int loads=-1, region=-99;
    bool ready;

    void Start()
    {
        if(!story||!activity||!stride||!support||!destination||!packageDestination||!head){enabled=false;Debug.LogError("Market preparation has incomplete scene references",this);return;}
        origin=activity.puppet.position;rotation=activity.puppet.rotation;
        headRest=head.localRotation;gestureRest=activity.gesture.localRotation;
        packageDelta=packageDestination.position-activity.parcels[0].transform.position;
        var legs=activity.puppet.GetComponentsInChildren<Renderer>(true);
        Vector3 feet=Vector3.zero;int count=0;
        foreach(var leg in legs)if(leg.name=="ConRoi_Leg_L"||leg.name=="ConRoi_Leg_R"){
            feet+=new Vector3(leg.bounds.center.x,leg.bounds.min.y,leg.bounds.center.z);count++;
        }
        if(count!=2||!support.Raycast(new Ray(feet/Mathf.Max(1,count)+Vector3.up,Vector3.down),out var contact,2)){
            enabled=false;Debug.LogError("Market preparation lacks valid paired foot support",this);return;
        }
        footOffset=feet/count-origin;rootGroundOffset=origin.y-contact.point.y;
        handAxis=activity.gesture.InverseTransformDirection(Vector3.down);
        ready=true;Restore();
    }
    void Restore()
    {
        owner=story.data;loads=story.SuccessfulLoads;
        activity.Restore();memory=new Memory{position=origin,rotation=rotation};
        if(!string.IsNullOrEmpty(owner.marketPreparationR88))
        {
            try{
                var saved=JsonUtility.FromJson<Memory>(owner.marketPreparationR88);
                if(saved!=null&&saved.version==1&&saved.phase>=0&&saved.phase<=5&&
                    float.IsFinite(saved.elapsed)&&float.IsFinite(saved.slide)&&
                    float.IsFinite(saved.position.x)&&float.IsFinite(saved.position.y)&&float.IsFinite(saved.position.z)&&
                    Quaternion.Dot(saved.rotation,saved.rotation)>.01f&&
                    Vector3.Distance(saved.position,origin)<2){memory=saved;}
            }catch(ArgumentException){ }
        }
        else if(owner.nextRegion>1||activity.memory.viewed!=0||activity.memory.notChosen||activity.memory.abandoned||activity.Held>=0||activity.Candidate>=0)
            memory.phase=5;
        memory.slide=Mathf.Clamp01(memory.slide);memory.elapsed=Mathf.Max(0,memory.elapsed);
        if(restoreOnRegradedSupport){
            var feet=memory.position+footOffset;
            if(support.Raycast(new Ray(feet+Vector3.up*4,Vector3.down),out var restoredSupport,29))
                memory.position=new Vector3(memory.position.x,restoredSupport.point.y+rootGroundOffset,memory.position.z);
        }
        activity.puppet.SetPositionAndRotation(memory.position,memory.rotation);
        stride.ResetPose();stride.RestorePlantState(memory.stridePose);head.localRotation=headRest;activity.gesture.localRotation=gestureRest;ApplyPose(0);
    }
    public void Capture()
    {
        if(!ready)return;
        if(owner!=story.data){Restore();}
        if(story.data.region!=1&&Busy)memory.phase=5;
        memory.position=activity.puppet.position;memory.rotation=activity.puppet.rotation;
        memory.stridePose=stride.CapturePlantState();
        owner.marketPreparationR88=JsonUtility.ToJson(memory);
    }
    void SavePhase(int phase)
    {
        memory.phase=phase;memory.elapsed=0;story.Save();
    }
    bool PlayerTouched=>activity.Held>=0||activity.Candidate>=0||activity.memory.viewed!=0||activity.memory.notChosen||activity.memory.abandoned||activity.memory.confirmed>=0;
    void Update()
    {
        if(!ready)return;
        if(owner!=story.data||loads!=story.SuccessfulLoads)Restore();
        if(region!=story.data.region){region=story.data.region;if(region!=1&&Busy)SavePhase(5);}
        if(!activity.Ready||story.polish.IsReading)return;
        if((memory.phase==0||Busy)&&PlayerTouched){SavePhase(5);return;}
        if(memory.phase==0){
            if(Vector3.Distance(story.player.transform.position,activity.slip.position)<triggerDistance)SavePhase(1);
            return;
        }
        float dt=Time.deltaTime;
        if(memory.phase==1){
            var next=Vector3.MoveTowards(activity.puppet.position,destination.position,speed*dt);
            // Pause if Minh occupies the destination of the next footstep; never push the player.
            var feet=next+footOffset;
            var player=story.player.transform.position;player.y=feet.y;
            if(Vector3.Distance(feet,player)<.65f)return;
            if(!support.Raycast(new Ray(feet+Vector3.up,Vector3.down),out var hit,2))return;
            next.y=hit.point.y+rootGroundOffset;
            activity.puppet.position=next;
            var d=destination.position-next;d.y=0;
            if(d.sqrMagnitude<.00001f)SavePhase(2);
        }
        else if(memory.phase==2){memory.elapsed+=dt;if(memory.elapsed>=.8f)SavePhase(3);}
        else if(memory.phase==3){
            memory.elapsed+=dt;
            // Reach first, slide on the same table, then release. No delivery state is changed.
            memory.slide=Mathf.SmoothStep(0,1,(memory.elapsed-.65f)/.9f);
            if(memory.elapsed>=2.2f){memory.slide=1;SavePhase(4);}
        }
    }
    void LateUpdate()
    {
        if(!ready||!activity.Available||!activity.Ready)return;
        ApplyPose(Time.deltaTime);
    }
    void ApplyPose(float deltaTime)
    {
        // Head turns to the parcel after walking; root/pivot remains on its measured support.
        if(memory.phase==2||memory.phase==3){
            float turn=memory.phase==2?Mathf.Clamp01(memory.elapsed/.65f):1;
            head.localRotation=headRest*Quaternion.Euler(0,0,-12*turn);
        }
        else head.localRotation=Quaternion.Slerp(head.localRotation,headRest,deltaTime*4);
        if(memory.phase==3){
            float reach=Mathf.Clamp01(memory.elapsed/.65f)*(1-Mathf.Clamp01((memory.elapsed-1.55f)/.65f));
            var contact=activity.parcels[0].transform.position+new Vector3(0,.075f,.10f);
            var rest=activity.gesture.parent.rotation*gestureRest;
            var desired=Quaternion.FromToRotation(rest*handAxis,(contact-activity.gesture.position).normalized)*rest;
            activity.gesture.rotation=Quaternion.Slerp(rest,desired,reach);
        }
        else if(memory.phase==2)activity.gesture.localRotation=gestureRest;
    }
}
