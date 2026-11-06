using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ConRoi.Investigation.V36.R103
{
    [DefaultExecutionOrder(-100)]
    public sealed partial class InvestigationSession : MonoBehaviour
    {
        public DieuKhienDiChuyen movement;
        public DieuKhienGocNhin look;
        public Camera eye;
        public ReturnGateMechanism gate,modelGate,clipGate;
        public Transform modelKhai,modelTu,clipTu,clipLeftHand,clipRightHand,clipRepair;
        public Camera stageCamera;
        public Camera clipCamera;
        public RenderTexture stageTexture;
        public Texture2D[] poseIcons;
        public InvestigationActor cipherPuppet;
        public bool diagnostics;
        public AnimationClip repairEventMotion;
        public Transform repairEventActor;
        public InvestigationSave data=new InvestigationSave();
        public string SaveDirectoryOverride {get;set;}
        public string SavePath => Path.Combine(string.IsNullOrEmpty(SaveDirectoryOverride)?Application.persistentDataPath:SaveDirectoryOverride,"InvestigationV36","r103","prototype.json");
        public string Page {get;private set;}="";
        public string Message {get;private set;}="Khám xưởng, xem điện thoại và cơ cấu; sau đó tìm Hà ở quầy ngoài cửa.";
        public string TrialId {get;private set;}="";
        public float TrialTime {get;private set;}
        public float ClipTime {get;private set;}
        public bool Paused {get;private set;}
        public bool TrialRunning {get;private set;}
        public bool ClipPlaying {get;private set;}
        public string TrialReport {get;private set;}="Chọn một diễn biến để quan sát. Mô hình không nhận diện hung thủ.";
        public float TrialReturnSeconds {get;private set;}=-1;
        float releaseAt,closeAt; bool sawOpen,trialContact;
        InvestigationInteractable focused;
        Transform viewer; Camera viewerCamera; RenderTexture viewerTexture;
        float viewDistance=2,viewYaw=25,viewPitch=10;
        string inspectId="",selectedA="",selectedB="",relationNote="",feedback="";
        int hintLevel;
        bool textEditing;
        Vector2 scroll;
        public int InspectionCount=>data.evidence.Count;
        public bool Has(string id)=>data.evidence.Any(e=>e.id==id);
        public EvidenceRecord Record(string id)=>data.evidence.FirstOrDefault(e=>e.id==id);
        public bool Observation(string id,int index)=>Record(id)?.observations.Contains(index)==true;
        public bool Modal=>Paused||Page!="";
        void Awake()
        {
            data.namespaceId="conroi.investigation.v36.r103";
            foreach(var arg in Environment.GetCommandLineArgs().Select((value,index)=>new{value,index}))
                if(arg.value=="--investigation-save-dir"){
                    var args=Environment.GetCommandLineArgs();if(arg.index+1<args.Length)SaveDirectoryOverride=args[arg.index+1];
                }
            Time.timeScale=1;Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;
            EnsureConclusions();
        }
        void EnsureConclusions(){while(data.conclusions.Count<4)data.conclusions.Add(new ConclusionRecord());}
        void Start(){if(modelGate)modelGate.enabled=false;SetModalState();}
        void OnDestroy(){Time.timeScale=1;if(viewerTexture)Destroy(viewerTexture);if(viewerCamera)Destroy(viewerCamera.gameObject);}
        public void Acquire(string id)
        {
            if(InvestigationContent.Get(id)==null)return;
            if(!Has(id))data.evidence.Add(new EvidenceRecord{id=id,collected=true});
        }
        public void Observe(string id,int index)
        {
            if(!Has(id))return;var definition=InvestigationContent.Get(id);
            if(id=="clip"&&(!data.clipBeforeSeen||!data.clipEventSeen)){feedback="Hãy xem cả trước và trong sự cố trước khi ghi quan sát clip.";return;}
            if(index<0||index>=definition.observations.Length)return;
            var record=Record(id);if(!record.observations.Contains(index))record.observations.Add(index);
        }
        void Update()
        {
            var k=Keyboard.current;
            if(k!=null)
            {
                if(k.escapeKey.wasPressedThisFrame){if(Page!="")ClosePage();else SetPaused(!Paused);}
                if(k.f5Key.wasPressedThisFrame)Save();
                if(k.f6Key.wasPressedThisFrame)Load();
                if(!Paused&&!textEditing&&k.pKey.wasPressedThisFrame){if(Page=="dossier")ClosePage();else OpenPage("dossier");}
                if(!Paused&&!textEditing&&k.rKey.wasPressedThisFrame&&Page!="")ClosePage();
            }
            if(!Modal && !(k!=null&&(k.escapeKey.wasPressedThisFrame||k.rKey.wasPressedThisFrame)))
            {
                focused=null;
                if(TryInteractionHit(out var hit))
                    focused=hit.collider.GetComponentInParent<InvestigationInteractable>();
                if(focused&&k!=null&&k.eKey.wasPressedThisFrame)Interact(focused);
                if(data.visitedHa&&!data.returnedToWorkshop&&Vector3.Distance(movement.transform.position,new Vector3(774,6,290))<6){data.returnedToWorkshop=true;Message="Đã trở lại xưởng. Đối chiếu các mục đã xem và thử diễn biến tại bàn.";}
            }
            if(!Paused)
            {
                if(TrialRunning)AdvanceTrial(Time.deltaTime);
                if(Page=="clip"&&ClipPlaying){ClipTime=Mathf.Min(MechanismPresentation.ClipLength,ClipTime+Time.deltaTime);if(ClipTime<3.8f)data.clipBeforeSeen=true;if(ClipTime>4.2f&&ClipTime<7.3f)data.clipEventSeen=true;UpdateClip();if(ClipTime>=MechanismPresentation.ClipLength)ClipPlaying=false;}
            }
            if(Page=="inspect"&&viewer&&Mouse.current!=null){if(Mouse.current.leftButton.isPressed){var d=Mouse.current.delta.ReadValue();viewYaw+=d.x*.25f;viewPitch=Mathf.Clamp(viewPitch-d.y*.25f,-80,80);}viewDistance=Mathf.Clamp(viewDistance-Mouse.current.scroll.ReadValue().y*.002f,.5f,4);UpdateViewer();}
        }
        public bool TryInteractionHit(out RaycastHit hit)
        {
            // The camera can be above the capsule when looking down. Ignore only
            // Khang's own body; the nearest scene obstruction still wins.
            foreach(var candidate in Physics.RaycastAll(eye.transform.position,eye.transform.forward,3.2f,~(1<<30|1<<29),QueryTriggerInteraction.Ignore).OrderBy(h=>h.distance))
                if(!candidate.collider.transform.IsChildOf(movement.transform)) { hit=candidate;return true; }
            hit=default;return false;
        }
        public void Interact(InvestigationInteractable target)
        {
            if(target.action=="table"){OpenPage("reconstruction");return;}
            if(target.action=="rope"){gate.state.rope=!gate.state.rope;Message=gate.state.rope?"Đang giữ D: chốt nhả. Dây không tự đẩy cánh. E lần nữa để thả.":"Đã thả D; cánh hồi khi không có lực giữ/vật cản.";return;}
            if(target.action=="gateforce"){gate.state.bodyForce=!gate.state.bodyForce;Message=gate.state.bodyForce?"Mô phỏng lực dựa từ gác đang giữ cánh; E để bỏ lực.":"Đã bỏ lực dựa.";return;}
            if((target.evidenceId=="coat"||target.evidenceId=="shoe")&&!data.samplesAllowed){Message="Hãy gặp Khải, rồi liên hệ Phương qua điện thoại P để nhận quyền xem mẫu.";return;}
            Acquire(target.evidenceId);
            if(target.evidenceId=="ha"){
                data.visitedHa=true;Acquire("letter");Acquire("receipt");Acquire("draft");
                Message="Hà giao thư, phiếu và giấy mẫu. Các mục đã nhận nằm trong hồ sơ; cần tự mở xem.";
            }
            OpenEvidence(target.evidenceId,target.transform);
        }
        public void OpenEvidence(string id,Transform visual=null)
        {
            if(!Has(id))return;
            inspectId=id;data.inspecting=id;OpenPage("inspect");
            if(!visual){var all=FindObjectsByType<InvestigationInteractable>(FindObjectsInactive.Exclude,FindObjectsSortMode.None);visual=all.FirstOrDefault(x=>x.evidenceId==id)?.transform;}
            if(visual){var item=visual.GetComponent<InvestigationInteractable>();if(item&&item.inspectionPrefab)visual=item.inspectionPrefab.transform;}
            if(id=="mechanism")visual=gate.transform;
            CreateViewer(visual);
        }
        public void OpenPage(string page){Page=page;data.page=page;scroll=Vector2.zero;SetModalState();}
        public void ClosePage(){Page="";data.page="";data.inspecting="";inspectId="";if(viewer)Destroy(viewer.gameObject);SetModalState();}
        public void SetPaused(bool value){Paused=value;Time.timeScale=value?0:1;SetModalState();}
        void SetModalState()
        {
            if(movement)movement.enabled=!Modal;
            if(look)look.enabled=!Modal;
            Cursor.lockState=Modal?CursorLockMode.None:CursorLockMode.Locked;Cursor.visible=Modal;
            if(stageCamera)stageCamera.enabled=Page=="reconstruction";
            if(clipCamera)clipCamera.enabled=Page=="clip";
        }
        void CreateViewer(Transform visual)
        {
            if(viewer)Destroy(viewer.gameObject);if(!visual)return;
            if(!viewerCamera){var go=new GameObject("Evidence preview camera");viewerCamera=go.AddComponent<Camera>();viewerCamera.cullingMask=1<<30;viewerCamera.clearFlags=CameraClearFlags.SolidColor;viewerCamera.backgroundColor=new Color(.11f,.14f,.15f);viewerTexture=new RenderTexture(600,600,24);viewerCamera.targetTexture=viewerTexture;viewerCamera.fieldOfView=35;var light=go.AddComponent<Light>();light.type=LightType.Point;light.range=12;light.intensity=3;light.cullingMask=1<<30;}
            viewer=Instantiate(visual.gameObject).transform;viewer.name="Evidence preview only";
            // Freeze world-space lines into the clone before moving/scaling the preview.
            foreach(var line in viewer.GetComponentsInChildren<LineRenderer>(true))if(line.useWorldSpace){var points=new Vector3[line.positionCount];line.GetPositions(points);for(int i=0;i<points.Length;i++)points[i]=line.transform.InverseTransformPoint(points[i]);line.useWorldSpace=false;line.SetPositions(points);}
            foreach(var c in viewer.GetComponentsInChildren<Collider>(true))Destroy(c);
            foreach(var b in viewer.GetComponentsInChildren<MonoBehaviour>(true))Destroy(b);
            foreach(var t in viewer.GetComponentsInChildren<Transform>(true))t.gameObject.layer=30;
            viewer.position=new Vector3(8000,1000,0);viewer.rotation=Quaternion.identity;viewer.localScale=Vector3.one;
            var bounds=new Bounds(viewer.position,Vector3.zero);foreach(var r in viewer.GetComponentsInChildren<Renderer>())bounds.Encapsulate(r.bounds);
            float size=Mathf.Max(.05f,bounds.size.magnitude);viewer.localScale*=1.2f/size;
            // Re-center using transformed bounds after normalized display scale.
            bounds=new Bounds(viewer.position,Vector3.zero);foreach(var r in viewer.GetComponentsInChildren<Renderer>())bounds.Encapsulate(r.bounds);
            viewer.position+=new Vector3(8000,1000,0)-bounds.center;viewYaw=25;viewPitch=(inspectId=="shoe"||inspectId=="shoecontrol")?65:(inspectId=="slide"||inspectId=="shoeprint"||inspectId=="fabric"||inspectId=="coat")?-65:10;viewDistance=2;UpdateViewer();
        }
        void UpdateViewer(){if(!viewerCamera)return;var centre=new Vector3(8000,1000,0);if(inspectId=="mechanism"&&viewer&&viewDistance<1){var part=viewer.GetComponentsInChildren<Transform>().FirstOrDefault(t=>t.name=="C_latch");if(part)centre=part.position;}viewerCamera.transform.position=centre+Quaternion.Euler(viewPitch,viewYaw,0)*new Vector3(0,0,viewDistance);viewerCamera.transform.LookAt(centre);}
        public string ReadCipher()
        {
            return string.Concat(InvestigationContent.HaPlain.Select(c=>{if(c==' ')return " • ";string value=data.mapping.FirstOrDefault(x=>x.pose==c.ToString())?.letter;return string.IsNullOrEmpty(value)?"_":value;}));
        }
        public void Assign(string pose,string letter)
        {
            if(pose.Length!=1||!InvestigationContent.Alphabet.Contains(pose))return;
            letter=letter.ToUpperInvariant();if(letter.Length>1||letter.Any(c=>c<'A'||c>'Z'))return;
            var item=data.mapping.FirstOrDefault(x=>x.pose==pose);if(item==null)data.mapping.Add(new CharacterAssignment{pose=pose,letter=letter});else item.letter=letter;
        }
        public void RecordReading()
        {
            if(!Has("letter")){feedback="Chưa nhận dải thư từ Hà.";return;}
            string text=ReadCipher().Replace(" • "," ");
            if(text.Contains("THREE CRATES GO")){Acquire("decoded");feedback="Đã ghi cách đọc: hỏi ba thùng đi đâu. Có thể đối chiếu phiếu; chưa biết hàng bên trong hay người gửi.";}
            else feedback="Đã giữ bảng gán và bản đọc thử. Hãy ghi phần bạn đọc được; hồ sơ chưa thêm một lời giải.";
        }
        public void AddRelation(string first,string second,string note)
        {
            if(first==second||!Has(first)||!Has(second)||string.IsNullOrWhiteSpace(note))return;
            data.relations.Add(new EvidenceRelation{first=first,second=second,note=note.Trim()});feedback="Đã ghi liên hệ của bạn; không tự xác nhận là sự thật.";
        }
        public void StartTrial(string id)
        {
            TrialId=id;TrialTime=0;releaseAt=2.5f;closeAt=-1;TrialReturnSeconds=-1;sawOpen=false;trialContact=false;TrialRunning=true;modelGate.ResetGate();modelGate.enabled=false;TrialReport="Đang chạy: hãy quan sát dây, chốt, lực giữ và cánh.";
        }
        public void AdvanceTrial(float dt)
        {
            TrialTime+=dt;var s=modelGate.state;float t=TrialTime;
            modelGate.state=MechanismPresentation.At(TrialId,t);s=modelGate.state;modelGate.Apply();var hook=modelGate.transform.parent.Find("M_visible_hook");if(hook)hook.gameObject.SetActive(s.hook);
            if(s.angle>20)sawOpen=true;
            if(t>releaseAt&&sawOpen&&closeAt<0&&s.latched){closeAt=t;TrialReturnSeconds=t-releaseAt;}
            if(modelKhai){var near=new Vector3(1.54f,0,-4.5f);var far=new Vector3(-3.9f,0,-3.5f);modelKhai.localPosition=TrialId=="after"?Vector3.Lerp(far,near,Mathf.Clamp01((t-5)/4)):Vector3.Lerp(near,far,Mathf.Clamp01((t-(TrialId=="release"||TrialId=="help"?2.5f:TrialId=="hook"?1.2f:5))/4));}
            if(modelTu){float fallAt=MechanismPresentation.FallAt(TrialId);bool fall=(TrialId=="hold"||TrialId=="hook"||TrialId=="help")&&t>fallAt;Vector3 p=new Vector3(.1f,3,1.67f);if(TrialId=="release"&&t>2.5f){float u=t<5.5f?(t-2.5f)/3:1-(t-5.5f)/3;p=Vector3.Lerp(p,new Vector3(2.2f,3,3.6f),Mathf.Clamp01(u));}if(fall){float elapsed=t-fallAt;p=new Vector3(.1f,Mathf.Max(.2f,3-Mathf.Max(0,elapsed-.35f)*2),Mathf.Lerp(1.67f,1.05f,Mathf.Clamp01(elapsed/.35f)));}if(TrialId=="after")p=new Vector3(.8f,.2f,1.05f);modelTu.localPosition=p;modelTu.localRotation=Quaternion.Euler(TrialId=="after"?85:fall?Mathf.Min(85,(t-fallAt)*100):0,180,0);}
            if(t<10)return;
            TrialRunning=false;modelGate.enabled=false;
            string observed=MechanismPresentation.StateWords(s)+". ";
            string detail=TrialId=="release"?"Người rời mép rồi quay lại khi chốt chưa bắt. Hãy so với trạng thái trước sự cố trong clip.":TrialId=="help"?"Dây đã thả nhưng Tư vẫn giữ lực; cánh chưa hồi qua người. So với khoảng cánh đóng trước sự cố.":TrialId=="hold"?"Quan sát thời điểm giữ dây và người mất chỗ tựa. Chưa biết người kéo hoặc ý định.":TrialId=="hook"?"Móc giữ dây thay người. So bề mặt móc tại xưởng.":TrialId=="wedge"?"Vật chèn giữ cánh. So khe cánh tại xưởng.":"Người tới sau có thể để dấu. Diễn biến này chưa giải thích cú ngã trước đó.";
            TrialReport=observed+detail;
            data.trials.Add(new TrialRecord{id=TrialId,report=TrialReport,returnSeconds=TrialReturnSeconds});
        }
        public void UpdateClip()
        {
            if(!clipGate)return;float t=ClipTime;
            clipGate.state=MechanismPresentation.At("event",t);clipGate.Apply();
            if(repairEventMotion&&repairEventActor)repairEventMotion.SampleAnimation(repairEventActor.gameObject,t);
            else if(clipTu){clipTu.localPosition=t>6.2f?FallThroughOpening(t-6.2f):new Vector3(.8f,3,1.9f);clipTu.localRotation=Quaternion.Euler(t>6.2f?Mathf.Min(75,(t-6.2f)*80):0,180,0);}
        }
        static Vector3 FallThroughOpening(float elapsed)
        {
            // Cross the loft edge at Z=1.5 before descending; never sink through its floor.
            return new Vector3(.8f,Mathf.Max(.2f,3-Mathf.Max(0,elapsed-.35f)*2),Mathf.Lerp(1.9f,1.05f,Mathf.Clamp01(elapsed/.35f)));
        }
        public void ResetTrial(){TrialRunning=false;modelGate.enabled=false;modelGate.ResetGate();TrialTime=0;TrialReport="Đã đặt lại mô hình.";if(modelTu){modelTu.localPosition=new Vector3(.8f,3,1.9f);modelTu.localRotation=Quaternion.Euler(0,180,0);}}
        public void Save()
        {
            try{data.position=movement.transform.position;data.yaw=movement.transform.eulerAngles.y;data.pitch=eye.transform.localEulerAngles.x;data.gate=JsonUtility.FromJson<GateState>(JsonUtility.ToJson(gate.state));data.page=Page;data.inspecting=inspectId;
                Directory.CreateDirectory(Path.GetDirectoryName(SavePath));string temp=SavePath+".tmp";File.WriteAllText(temp,JsonUtility.ToJson(data,true));File.Copy(temp,SavePath,true);File.Delete(temp);Message="Đã lưu hồ sơ prototype riêng.";
            }catch(Exception e){Message="Không lưu được: "+e.Message;}
        }
        public bool Load()
        {
            try{
                if(!File.Exists(SavePath)){Message="Chưa có save điều tra ở thư mục riêng.";return false;}
                var incoming=JsonUtility.FromJson<InvestigationSave>(File.ReadAllText(SavePath));
                if(incoming==null||incoming.schema!=1||incoming.namespaceId!="conroi.investigation.v36.r103"||!float.IsFinite(incoming.position.x)||!float.IsFinite(incoming.position.y)||!float.IsFinite(incoming.position.z))throw new Exception("Schema/namespace/vị trí không hợp lệ; không migration save cũ.");
                if(incoming.evidence.Any(e=>InvestigationContent.Get(e.id)==null))throw new Exception("ID vật chứng không hợp lệ");
                data=incoming;EnsureConclusions();bool cc=movement.controller.enabled;movement.controller.enabled=false;movement.transform.SetPositionAndRotation(data.position,Quaternion.Euler(0,data.yaw,0));movement.controller.enabled=cc;movement.ResetVerticalMotion();look.RestorePitch(data.pitch);gate.Restore(data.gate);
                ResetTrial();ClipPlaying=false;ClipTime=0;UpdateClip();SetPaused(false);Page="";
                if(data.page=="inspect"&&Has(data.inspecting))OpenEvidence(data.inspecting);else if(new[]{"cipher","dossier","compare","reconstruction","clip","conclusions"}.Contains(data.page))OpenPage(data.page);else ClosePage();Message="Đã tải hồ sơ riêng; thử nghiệm đang chạy trở về trạng thái an toàn.";return true;
            }catch(Exception e){Message="Không tải: "+e.Message;return false;}
        }
    }
}
