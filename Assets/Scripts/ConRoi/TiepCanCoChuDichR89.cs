// Rối tiếp cận điểm hành động khi có cơ hội; không ghi đè lựa chọn giấy/sổ của Minh.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story, puppet, destination, focus, head, stride, support, notebook, staging, papers.
// Code tự tìm/tạo: MoDauBenHoR23.
// Thứ tự chạy khai báo: 20950; giữ quan hệ khởi tạo/pose với các owner khác.
// Start kiểm reference bắt buộc; memory/SuccessfulLoads phối hợp restore, không chọn hộ người chơi.
using System;
using UnityEngine;

// Scene opt-in: preparation never changes Minh's paper/notebook choices or scoring.
[DefaultExecutionOrder(20950)]
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "PurposefulApproachR89")]
public sealed class TiepCanCoChuDichR89 : MonoBehaviour
{
    [Serializable] public sealed class Memory {
        public int version=1,phase; // waiting, walking, settling, acting, complete, skipped
        public float elapsed,progress;public Vector3 position;public string plants="";
    }
    public bool school;
    public DieuPhoiHanhTrinh story;
    public Transform puppet,destination,focus,head;
    public BuocChanConRoiR23 stride;
    public Collider support;
    public TuongTacSoTayHoR37 notebook;
    public TraiNghiemTruongR28 staging;
    public TuongTacGiayLop203R32 papers;
    public float speed=.38f,triggerDistance=5;
    public Memory memory=new Memory();
    public bool Blocked {get;private set;}
    public bool OwnsMotion=>memory.phase<4;
    public bool AtDesk=>memory.phase==3||memory.phase==4;
    public bool Skipped=>memory.phase==5;
    public bool Active=>story.data.region==(school?2:0)&&!story.DreamHub;
    public Vector3 NotebookOffset=>school?Vector3.zero:new Vector3(.035f,0,-.025f)*memory.progress;
    public Quaternion NotebookRotation=>school?Quaternion.identity:Quaternion.Euler(0,8*memory.progress,0);
    Vector3 origin,feetOffset,handAxis;Quaternion headRest,armRest;float groundOffset;
    DieuPhoiHanhTrinh.State owner;int loads=-1;bool ready,wasActive,restoredPlants;
    string Saved {get=>school?story.data.schoolApproachR89:story.data.lakeApproachR89;set{if(school)story.data.schoolApproachR89=value;else story.data.lakeApproachR89=value;}}
    void Start(){
        if(!story||!puppet||!destination||!focus||!head||!stride||!support||(!school&&!notebook)||(school&&(!staging||!papers))){enabled=false;Debug.LogError("R89 approach references incomplete",this);return;}
        origin=puppet.position;headRest=head.localRotation;
        if(notebook){armRest=notebook.gesture.localRotation;handAxis=notebook.gesture.InverseTransformDirection(Vector3.down);}
        var legs=puppet.GetComponentsInChildren<Renderer>(true);Vector3 feet=Vector3.zero;int count=0;
        foreach(var leg in legs)if(leg.name=="ConRoi_Leg_L"||leg.name=="ConRoi_Leg_R"){feet+=new Vector3(leg.bounds.center.x,leg.bounds.min.y,leg.bounds.center.z);count++;}
        if(count!=2){enabled=false;Debug.LogError("R89 approach requires paired legs",this);return;}
        feetOffset=feet/count-origin;groundOffset=origin.y-feet.y/count;
        ready=true;Restore();
    }
    bool Touched=>school?(papers.memory.viewed!=0||papers.memory.choice!=0||papers.memory.abandoned||papers.memory.places[0]!=0||papers.memory.places[1]!=0):notebook.memory.touched||notebook.Held||notebook.memory.abandoned;
    public bool ClearStep(Vector3 next){
        if(!ready)return false;var feet=next+feetOffset;var player=story.player.transform.position;player.y=feet.y;
        if(Vector3.Distance(player,feet)<.75f)return false;
        if(!support.Raycast(new Ray(feet+Vector3.up,Vector3.down),out _,2))return false;
        foreach(var c in Physics.OverlapCapsule(feet+Vector3.up*.32f,feet+Vector3.up*1.48f,.20f,~0,QueryTriggerInteraction.Ignore)){
            if(c==support||c.transform.IsChildOf(puppet)||c.transform.IsChildOf(story.player.transform))continue;
            var point=c.ClosestPoint(feet+Vector3.up*.8f);
            if(point.y>feet.y+.2f&&Vector3.Distance(point,feet+Vector3.up*.8f)<.22f)return false;
        }return true;
    }
    void Restore(){
        owner=story.data;loads=story.SuccessfulLoads;memory=new Memory{position=origin};
        // Default migration must inspect the loaded activity, never the previous region/save's memory.
        if(school)papers.Restore();else notebook.Restore();
        if(!string.IsNullOrEmpty(Saved)){
            try{var m=JsonUtility.FromJson<Memory>(Saved);if(m!=null&&m.version==1&&m.phase>=0&&m.phase<=5&&float.IsFinite(m.elapsed)&&float.IsFinite(m.progress)&&float.IsFinite(m.position.x)&&float.IsFinite(m.position.y)&&float.IsFinite(m.position.z)&&Vector3.Distance(m.position,origin)<6)memory=m;}catch(ArgumentException){}
        }else if(Touched||story.data.nextRegion>(school?2:0)||(school&&(staging.ActionFinished||story.data.schoolLeading))){memory.phase=5;if(school)memory.position=destination.position;}
        memory.progress=Mathf.Clamp01(memory.progress);memory.elapsed=Mathf.Max(0,memory.elapsed);
        puppet.position=memory.position;stride.ResetPose();stride.RestorePlantState(memory.plants);restoredPlants=stride.Initialized;
        if(school&&Active)staging.ReapplyPoseR89();
        head.localRotation=headRest;ApplyPose();wasActive=Active;
        if(!school&&!OwnsMotion)Reanchor();
    }
    public void Capture(){
        if(!ready)return;if(owner!=story.data)Restore();
        if(!Active&&memory.phase>0&&memory.phase<4){memory.phase=5;memory.elapsed=0;}
        memory.position=puppet.position;memory.plants=stride.CapturePlantState();Saved=JsonUtility.ToJson(memory);
    }
    void Phase(int phase){memory.phase=phase;memory.elapsed=0;if(!school&&phase>=4)Reanchor();story.Save();}
    void Reanchor(){var o=story.GetComponent<MoDauBenHoR23>();o.lakeFootAnchor=head.position-Vector3.up*1.58f;o.stripHalfWidth=.45f;} // Match the legacy clamp's reference point, not the contact sampler.
    void Update(){
        if(!ready)return;if(owner!=story.data||loads!=story.SuccessfulLoads)Restore();
        if(!restoredPlants&&stride.Initialized){stride.RestorePlantState(memory.plants);restoredPlants=true;}
        if(wasActive&&!Active&&memory.phase>0&&memory.phase<4)Phase(5);wasActive=Active;
        if(!Active||story.Loading||story.MenuOpen||story.Paused||story.PhoneOpen||story.SupplementInspection||story.polish.IsReading)return;
        if(OwnsMotion&&Touched){Phase(5);return;}
        if(memory.phase==0){if(Vector3.Distance(story.player.transform.position,focus.position)<triggerDistance)Phase(1);return;}
        float dt=Time.deltaTime;Blocked=false;
        if(memory.phase==1){
            var next=Vector3.MoveTowards(puppet.position,destination.position,speed*dt);var feet=next+feetOffset;
            if(!ClearStep(next)){Blocked=true;return;}
            if(!support.Raycast(new Ray(feet+Vector3.up,Vector3.down),out var hit,2)){Blocked=true;return;}
            next.y=hit.point.y+groundOffset;puppet.position=next;
            var d=destination.position-next;d.y=0;if(d.sqrMagnitude<.00001f)Phase(2);
        }else if(memory.phase==2){memory.elapsed+=dt;if(memory.elapsed>=.75f)Phase(3);}
        else if(memory.phase==3){
            if(school){if(staging.ActionFinished)Phase(4);}
            else{memory.elapsed+=dt;memory.progress=Mathf.SmoothStep(0,1,(memory.elapsed-.4f)/.85f);if(memory.elapsed>=1.8f){memory.progress=1;Phase(4);}}
        }
    }
    void LateUpdate(){if(ready&&Active)ApplyPose();}
    void ApplyPose(){
        if(memory.phase==2||memory.phase==3){float t=memory.phase==2?Mathf.Clamp01(memory.elapsed/.65f):1;head.localRotation=headRest*Quaternion.Euler(0,0,-12*t);}
        else head.localRotation=headRest;
        if(!school&&memory.phase==3){float reach=Mathf.Clamp01(memory.elapsed/.4f)*(1-Mathf.Clamp01((memory.elapsed-1.25f)/.55f));var d=notebook.book.position-notebook.gesture.position;var rest=notebook.gesture.parent.rotation*armRest;notebook.gesture.rotation=Quaternion.Slerp(rest,Quaternion.FromToRotation(rest*handAxis,d.normalized)*rest,reach);}
        else if(!school)notebook.gesture.localRotation=armRest;
    }
}
