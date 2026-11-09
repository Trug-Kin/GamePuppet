using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace ConRoi.Investigation.V36.R107
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
        public string SavePath => Path.Combine(string.IsNullOrEmpty(SaveDirectoryOverride)?Application.persistentDataPath:SaveDirectoryOverride,"InvestigationV36","r107","prototype.json");
        public string Page {get;private set;}="";
        string transientMessage=""; float messageUntil;
        public string Message {get=>Time.unscaledTime<messageUntil?transientMessage:"";private set{transientMessage=value;messageUntil=Time.unscaledTime+6;}}
        public string TrialId {get;private set;}="";
        public float TrialTime {get;private set;}
        public float TrialDuration {get;private set;}=16f;
        public float TrialClearAt {get;private set;}=float.PositiveInfinity;
        public bool TrialSettled=>!string.IsNullOrEmpty(TrialId)&&TrialTime>=TrialDuration-.001f&&TrialTimeline.Settled(modelGate.state);
        public bool CanClearTrialHolds=>!string.IsNullOrEmpty(TrialId)&&float.IsPositiveInfinity(TrialClearAt)&&TrialTime>=TrialTimeline.LastAction(TrialId)&&(modelGate.state.bodyForce||modelGate.state.hook||modelGate.state.wedge);
        public void ClearTrialHolds(){if(!CanClearTrialHolds)return;CancelReviewDrag();TrialClearAt=TrialTime;TrialDuration=TrialTimeline.End(TrialId,TrialClearAt);trialRecorded=false;TrialRunning=true;TrialReport="Đã bỏ điều kiện giữ. Tiếp tục quan sát cánh hồi và chốt.";}
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
        bool textEditing,clearTextFocus;
        Vector2 scroll;
        public int InspectionCount=>data.evidence.Count;
        public bool Has(string id)=>data.evidence.Any(e=>e.id==id);
        public EvidenceRecord Record(string id)=>data.evidence.FirstOrDefault(e=>e.id==id);
        public bool Observation(string id,int index)=>Record(id)?.observations.Contains(index)==true;
        public bool Modal=>Paused||Page!="";
        void Awake()
        {
            data.namespaceId="conroi.investigation.v36.r107";
            foreach(var arg in Environment.GetCommandLineArgs().Select((value,index)=>new{value,index}))
                if(arg.value=="--investigation-save-dir"){
                    var args=Environment.GetCommandLineArgs();if(arg.index+1<args.Length)SaveDirectoryOverride=args[arg.index+1];
                }
            Time.timeScale=1;Cursor.lockState=CursorLockMode.Locked;Cursor.visible=false;
            EnsureConclusions();
        }
        void EnsureConclusions(){while(data.conclusions.Count<4)data.conclusions.Add(new ConclusionRecord());}
        void Start(){if(modelGate)modelGate.enabled=false;if(!data.introAccepted)OpenPage("intro");else SetModalState();}
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
            if(id=="slide"||id=="shoeprint")data.firstFloorRecorded=true;
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
            UpdateOnboarding();
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
                if(Page=="reconstruction"&&TrialRunning&&!trialScrubbing)AdvanceTrial(Time.deltaTime);
                if(Page=="clip"&&ClipPlaying&&!clipScrubbing){float previous=ClipTime;ClipTime=Mathf.Min(MechanismPresentation.ClipLength,ClipTime+Time.deltaTime);RegisterClipExposure(previous,ClipTime);UpdateClip();if(ClipTime>=MechanismPresentation.ClipLength)ClipPlaying=false;}
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
            if(target.action=="table"){data.tableDiscovered=true;OpenPage("reconstruction");return;}
            if(target.action=="rope"){gate.state.rope=!gate.state.rope;Message=gate.state.rope?"Đang giữ D: chốt nhả. Dây không tự đẩy cánh. E lần nữa để thả.":"Đã thả D; cánh hồi khi không có lực giữ/vật cản.";return;}
            if(target.action=="gateforce"){gate.state.bodyForce=!gate.state.bodyForce;Message=gate.state.bodyForce?"Mô phỏng lực dựa từ gác đang giữ cánh; E để bỏ lực.":"Đã bỏ lực dựa.";return;}
            if((target.evidenceId=="coat"||target.evidenceId=="shoe")&&!data.samplesAllowed){Message="Hãy gặp Khải, rồi liên hệ Phương qua điện thoại P để nhận quyền xem mẫu.";return;}
            var actor=target.GetComponent<InvestigationActor>();if(actor)actor.Greet();
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
        public void OpenPage(string page){if(page=="dossier"&&data.firstFloorRecorded)data.firstNotesReviewed=true;Page=page;data.page=page;scroll=Vector2.zero;SetModalState();}
        public void ClosePage(){CancelReviewDrag();Page="";data.page="";data.inspecting="";inspectId="";if(viewer)Destroy(viewer.gameObject);SetModalState();}
        public void SetPaused(bool value){CancelReviewDrag();Paused=value;Time.timeScale=value?0:1;SetModalState();}
        void SetModalState()
        {
            // Text focus belongs to the page being left, never to world input.
            textEditing=false;
            clearTextFocus=true;
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
            viewer.position+=new Vector3(8000,1000,0)-bounds.center;viewYaw=25;viewPitch=(inspectId=="shoe"||inspectId=="shoecontrol")?65:(inspectId=="slide"||inspectId=="shoeprint"||inspectId=="fabric"||inspectId=="coat")?-65:10;viewDistance=(inspectId=="slide"||inspectId=="shoeprint")?1.65f:2;UpdateViewer();
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
            CancelReviewDrag();trialRecorded=false;
            TrialId=id;TrialTime=0;TrialClearAt=float.PositiveInfinity;TrialDuration=TrialTimeline.End(id,TrialClearAt);releaseAt=2.5f;closeAt=-1;TrialReturnSeconds=-1;sawOpen=false;trialContact=false;TrialRunning=true;modelGate.ResetGate();modelGate.enabled=false;TrialReport="Đang chạy: hãy quan sát dây, chốt, lực giữ và cánh.";
        }
        public void AdvanceTrial(float dt)
        {
            if(dt<0)TrialTime=0;
            TrialTime=Mathf.Min(TrialDuration,TrialTime+dt);var s=modelGate.state;float t=TrialTime;
            modelGate.state=TrialTimeline.At(TrialId,t,TrialClearAt);s=modelGate.state;modelGate.Apply();var hook=modelGate.transform.parent.Find("M_visible_hook");if(hook)hook.gameObject.SetActive(s.hook);
            if(s.angle>20)sawOpen=true;
            if(t>releaseAt&&sawOpen&&closeAt<0&&s.latched){closeAt=t;TrialReturnSeconds=t-releaseAt;}
            if(modelKhai){var near=new Vector3(1.1f,0,-4.25f);var far=new Vector3(-3.9f,0,-3.5f);modelKhai.localPosition=TrialId=="after"?Vector3.Lerp(far,near,Mathf.Clamp01((t-5)/4)):Vector3.Lerp(near,far,Mathf.Clamp01((t-(TrialId=="release"||TrialId=="help"?2.5f:TrialId=="hook"?1.2f:5))/4));}
            if(modelTu){float fallAt=MechanismPresentation.FallAt(TrialId);bool fall=(TrialId=="hold"||TrialId=="hook"||TrialId=="help")&&t>fallAt;Vector3 p=new Vector3(.55f,3,1.95f);if(TrialId=="release"&&t>2.5f){float u=t<5.5f?(t-2.5f)/3:1-(t-5.5f)/3;p=Vector3.Lerp(p,new Vector3(2.2f,3,3.6f),Mathf.Clamp01(u));}if(fall){float elapsed=t-fallAt;p=new Vector3(.55f,Mathf.Max(.2f,3-Mathf.Max(0,elapsed-.35f)*2),Mathf.Lerp(1.95f,1.05f,Mathf.Clamp01(elapsed/.35f)));}if(TrialId=="after")p=new Vector3(.8f,.2f,1.05f);if(TrialId=="release"&&t>=TrialClearAt)p=Vector3.Lerp(p,new Vector3(2.2f,3,3.6f),Mathf.Clamp01((t-TrialClearAt)/.6f));modelTu.localPosition=p;modelTu.localRotation=Quaternion.Euler(TrialId=="after"?85:fall?Mathf.Min(85,(t-fallAt)*100):0,180,0);}
            if(t<TrialDuration)return;
            if(!TrialTimeline.Settled(s)){TrialRunning=false;TrialReport="Mô hình chưa ổn định; chưa ghi kết quả hoàn tất.";return;}
            TrialRunning=false;modelGate.enabled=false;
            string observed=MechanismPresentation.StateWords(s)+". ";
            string detail=s.bodyForce?"Cánh đang được người trên gác giữ mở. Có thể ngừng đè giữ để quan sát hồi.":s.wedge?"Vật chèn còn giữ cánh. Có thể rút chèn để quan sát hồi.":s.hook?"Móc còn giữ dây; cánh về đóng nhưng chốt chưa cài. Có thể nhả móc.":"Cánh đã hồi đóng và chốt đã cài.";
            TrialReport=observed+detail+" Mô hình không xác định người thao tác hay ý định.";
            if(!trialRecorded){data.trials.Add(new TrialRecord{id=TrialId+(float.IsPositiveInfinity(TrialClearAt)?"":"/clear-holds"),report=TrialReport,returnSeconds=TrialReturnSeconds});trialRecorded=true;}
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
        public void ResetTrial(){CancelReviewDrag();TrialRunning=false;TrialId="";trialRecorded=false;modelGate.enabled=false;modelGate.ResetGate();TrialTime=0;TrialClearAt=float.PositiveInfinity;TrialDuration=16f;TrialReport="Đã đặt lại mô hình.";if(modelTu){modelTu.localPosition=new Vector3(.8f,3,1.9f);modelTu.localRotation=Quaternion.Euler(0,180,0);}}
        public void Save()
        {
            try{data.position=movement.transform.position;data.yaw=movement.transform.eulerAngles.y;data.pitch=eye.transform.localEulerAngles.x;data.gate=JsonUtility.FromJson<GateState>(JsonUtility.ToJson(gate.state));data.page=Page;data.inspecting=inspectId;
                InvestigationSaveFile.Write(SavePath,JsonUtility.ToJson(data,true));Message="Đã lưu hồ sơ prototype riêng.";
            }catch(Exception e){Message="Không lưu được: "+e.Message;}
        }
        public bool Load()
        {
            try{
                if(!File.Exists(SavePath)){Message="Chưa có save điều tra ở thư mục riêng.";return false;}
                string json=File.ReadAllText(SavePath);
                if(!InvestigationSaveValidation.TryValidateSerializedCollections(json,out string validationError))throw new InvalidDataException(validationError);
                var incoming=JsonUtility.FromJson<InvestigationSave>(json);
                if(!InvestigationSaveValidation.TryValidate(incoming,out validationError))throw new InvalidDataException(validationError);
                data=incoming;EnsureConclusions();bool cc=movement.controller.enabled;movement.controller.enabled=false;movement.transform.SetPositionAndRotation(data.position,Quaternion.Euler(0,data.yaw,0));movement.controller.enabled=cc;movement.ResetVerticalMotion();look.RestorePitch(data.pitch);gate.Restore(data.gate);
                ResetTrial();ClipPlaying=false;ClipTime=0;UpdateClip();SetPaused(false);Page="";
                if(data.page=="inspect"&&Has(data.inspecting))OpenEvidence(data.inspecting);else if(new[]{"cipher","dossier","compare","reconstruction","clip","conclusions","intro"}.Contains(data.page))OpenPage(data.page);else ClosePage();Message="Đã tải hồ sơ riêng; thử nghiệm đang chạy trở về trạng thái an toàn.";return true;
            }catch(InvalidDataException e){Message="Không tải: "+e.Message;return false;}
            catch(Newtonsoft.Json.JsonException){Message="Không tải: Nội dung hồ sơ không đúng định dạng.";return false;}
            catch(UnauthorizedAccessException){Message="Không tải: Không có quyền đọc tệp hồ sơ. Kiểm tra quyền truy cập rồi thử lại.";return false;}
            catch(IOException){Message="Không tải: Không đọc được tệp hồ sơ. Hãy đóng ứng dụng đang dùng tệp rồi thử lại.";return false;}
            catch(Exception){Message="Không tải: Không thể đọc hồ sơ này. Hãy thử lại hoặc dùng bản sao hợp lệ.";return false;}
        }
    }
}
