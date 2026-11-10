#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Gameplay.TestOnly
{
    [DefaultExecutionOrder(-200)]
    public sealed class FullCasePlayHost : MonoBehaviour
    {
        public FullInvestigationRuntime Runtime {get;private set;}
        public DieuKhienDiChuyen Movement;public DieuKhienGocNhin Look;public Camera Eye;
        public readonly Dictionary<string,GameObject> Zones=new Dictionary<string,GameObject>();
        public readonly Dictionary<string,Vector3> Spawns=new Dictionary<string,Vector3>();
        public readonly Dictionary<string,FullCasePlayTarget> Targets=new Dictionary<string,FullCasePlayTarget>();
        public Func<bool> TextInputFocused;public Action<Rect> DrawPanel;public Func<string,FullActionResult> Inspect,Talk;
        public Action<FullActionResult> Feedback;public Func<bool> Visible;
        public FullCasePlayTarget Focused {get;private set;}
        public string LastWorldKey {get;private set;}="";
        public int RayInteractions {get;private set;}
        public Vector2 LastPointer {get;private set;}public int PointerEvents {get;private set;}public int KeyEvents {get;private set;}
        object context;string location="";bool initialized;
        public void Initialize(FullInvestigationRuntime runtime){Runtime=runtime;Runtime.Changed+=Refresh;initialized=true;Refresh();}
        void Refresh(){if(!initialized)return;bool reset=context!=Runtime.DataContext||location!=Runtime.Location;context=Runtime.DataContext;location=Runtime.Location;
            foreach(var z in Zones)z.Value.SetActive(z.Key==location);
            var local=new HashSet<string>(Runtime.GetLocalEvidence().Select(e=>e.id));
            foreach(var t in Targets.Values)if(t.Action==FullPlayAction.Evidence)t.gameObject.SetActive(t.LocationId==location&&local.Contains(t.StableId));
            if(reset&&Spawns.TryGetValue(location,out var point)){Movement.controller.enabled=false;Movement.transform.position=point;Movement.transform.rotation=Quaternion.identity;Eye.transform.localRotation=Quaternion.identity;Look.RestorePitch(0);Movement.ResetVerticalMotion();Movement.controller.enabled=true;}
            OwnInput();
        }
        void OwnInput(){bool world=Runtime.CanMove&&(Visible==null||Visible());Movement.enabled=world;Look.enabled=world;Cursor.lockState=world?CursorLockMode.Locked:CursorLockMode.None;Cursor.visible=!world;}
        void Update(){if(!initialized)return;var k=Keyboard.current;
            if(k!=null&&(Visible==null||Visible())){if(k.escapeKey.wasPressedThisFrame){if(Runtime.Modal!=FullCaseModal.None)Runtime.Cancel();else Runtime.SetPaused(!Runtime.Paused);}
                bool typing=Runtime.Modal!=FullCaseModal.None&&(TextInputFocused?.Invoke()??false);
                if(!typing){if(k.f5Key.wasPressedThisFrame)Feedback(Runtime.TrySave(Profile));if(k.f6Key.wasPressedThisFrame)Feedback(Runtime.TryLoad(Profile));}
                if(k.pKey.wasPressedThisFrame&&!Runtime.Paused&&!(Runtime.Modal!=FullCaseModal.None&&(TextInputFocused?.Invoke()??false))){if(Runtime.Modal!=FullCaseModal.None)Runtime.Cancel();else Runtime.SetModal(FullCaseModal.Inspect);}}
            OwnInput();Focused=null;
            if(Runtime.CanMove&&(Visible==null||Visible())&&!(k!=null&&(k.escapeKey.wasPressedThisFrame||k.pKey.wasPressedThisFrame))&&InvestigationSession.TryInteractionHit(Eye,Movement.transform,out var hit))Focused=hit.collider.GetComponentInParent<FullCasePlayTarget>();
            if(Focused&&k!=null&&k.eKey.wasPressedThisFrame){LastWorldKey=Focused.Key;RayInteractions++;Feedback(Interact(Focused));}
            if(Visible==null||Visible())Runtime.Advance(Mathf.Min(.1f,Time.unscaledDeltaTime));
        }
        public string Profile {get;set;}
        FullActionResult Interact(FullCasePlayTarget target){if(target.LocationId!=Runtime.Location)return FullActionResult.Fail("location","TEST ONLY: nguồn không ở khu hiện tại.");
            switch(target.Action){case FullPlayAction.Evidence:var r=Runtime.TryCollect(target.StableId);return r.Success?Inspect(target.StableId):r;
                case FullPlayAction.Actor:return Talk(target.StableId);
                case FullPlayAction.Travel:return Runtime.TryTravel(target.StableId);
                case FullPlayAction.Page:Runtime.SetModal(target.Page);return FullActionResult.Ok("TEST ONLY: bàn thao tác nối runtime thật.");
                case FullPlayAction.Appointment:return Runtime.TryOpenEndingSummary();default:return FullActionResult.Fail("target","TEST ONLY: binding chưa có.");}
        }
        void OnGUI(){if(!initialized)return;if(Event.current.type==EventType.KeyDown||Event.current.type==EventType.KeyUp)KeyEvents++;if(Event.current.type==EventType.MouseDown||Event.current.type==EventType.MouseUp){LastPointer=Event.current.mousePosition;PointerEvents++;}GUI.Label(new Rect(12,8,Screen.width-24,45),"TEST ONLY • Play Mode / controller thật + world proxy • WASD, chuột, E; P hồ sơ, Esc đóng/pause; F5/F6 profile riêng");
            if(Runtime.Modal!=FullCaseModal.None||Runtime.Paused){var panel=new Rect(12,55,Mathf.Min(Screen.width-24,1000),Screen.height-70);GUI.Box(panel,GUIContent.none);GUI.BeginGroup(panel);DrawPanel?.Invoke(panel);GUI.EndGroup();}
            else {GUI.Label(new Rect(Screen.width/2-8,Screen.height/2-12,25,25),"+");GUI.Label(new Rect(12,55,Screen.width-24,65),"Khu: "+Runtime.Location+" • "+(Focused?"E • "+Focused.Label:"Đến bảng/vật/NPC, nhìn tâm vào collider trong 3.2 m")+"\n"+Runtime.LastMessage);}
        }
        void OnDestroy(){if(Runtime!=null)Runtime.Changed-=Refresh;Cursor.lockState=CursorLockMode.None;Cursor.visible=true;}
    }
}
#endif
