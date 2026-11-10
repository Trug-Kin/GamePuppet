using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using ConRoi.Gameplay.TestOnly;
using ConRoi.Investigation.V36.R107;
namespace ConRoi.Gameplay.EditorChecks
{
    [InitializeOnLoad] public static class FullCasePlayLauncher
    {
        public const string Scene="Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        const string Armed="ConRoi.FullCase.Play.Armed";
        static EditorWindow cursorView;
        static FullCaseSandboxWindow panel;public static FullCasePlayHost Active {get;private set;}
        internal static FullCaseSandboxWindow Panel=>panel;
        static FullCasePlayLauncher(){EditorApplication.playModeStateChanged+=OnState;}
        [MenuItem("Con Roi/Gameplay/Choi the gioi TEST ONLY (Play Mode)")]
        public static void Open(){if(EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Dừng Play trước khi tạo phiên thử sạch.");if(UnityEngine.SceneManagement.SceneManager.GetActiveScene().path!=Scene||UnityEngine.SceneManagement.SceneManager.GetActiveScene().isDirty)throw new InvalidOperationException("Mở scene Gameplay_ThuNghiem sạch trước; menu không ghi/đổi scene đang làm.");SessionState.SetBool(Armed,true);EditorApplication.EnterPlaymode();}
        static void OnState(PlayModeStateChange state){if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool(Armed,false)){SessionState.SetBool(Armed,false);Build();}
            if(state==PlayModeStateChange.ExitingPlayMode){if(panel)UnityEngine.Object.DestroyImmediate(panel);panel=null;Active=null;cursorView=null;}}
        // Editor keeps a separate cursor permission after Esc; runtime Cursor setters
        // alone can report Locked while Windows still shows the pointer. Test host only.
        static void CursorContext(bool world){
            var focused=EditorWindow.focusedWindow;
            if(focused&&focused.GetType().Name=="GameView")cursorView=focused;
            if(!cursorView)return;
            var allow=cursorView.GetType().GetMethod("AllowCursorLockAndHide",BindingFlags.Instance|BindingFlags.Static|BindingFlags.Public|BindingFlags.NonPublic|BindingFlags.FlattenHierarchy,null,new[]{typeof(bool)},null);
            if(allow==null){Debug.LogWarning("TEST ONLY: Unity Editor cursor permission API unavailable; native cursor requires recheck.");return;}
            allow.Invoke(allow.IsStatic?null:cursorView,new object[]{world});
        }
        static GameObject Cube(string name,Transform parent,Vector3 point,Vector3 scale){var g=GameObject.CreatePrimitive(PrimitiveType.Cube);g.name="TEST ONLY • "+name;g.transform.SetParent(parent,false);g.transform.localPosition=point;g.transform.localScale=scale;return g;}
        static void Target(FullCasePlayHost host,Transform zone,string location,string key,string id,FullPlayAction action,float x,FullCaseModal page=FullCaseModal.None){var g=Cube(key,zone,new Vector3(x,1.6f,0),new Vector3(.65f,.8f,.3f));var t=g.AddComponent<FullCasePlayTarget>();t.Key=key;t.StableId=id;t.LocationId=location;t.Label="TEST ONLY • "+id;t.Action=action;t.Page=page;host.Targets.Add(key,t);
            var sign=new GameObject("TEST ONLY label");sign.transform.SetParent(g.transform,false);sign.transform.localPosition=new Vector3(0,.8f,-.8f);sign.transform.localScale=new Vector3(1/.65f,1/.8f,1/.3f);var text=sign.AddComponent<TextMesh>();text.text=id;text.characterSize=.13f;text.anchor=TextAnchor.MiddleCenter;text.color=Color.white;}
        public static void Build(){if(!EditorApplication.isPlaying||UnityEngine.SceneManagement.SceneManager.GetActiveScene().path!=Scene||Active)throw new InvalidOperationException("Only explicit Gameplay Play session");
            panel=ScriptableObject.CreateInstance<FullCaseSandboxWindow>();panel.PlayHostMode=true;panel.hideFlags=HideFlags.HideAndDontSave;
            var root=new GameObject("TEST ONLY • Full-case controller environment");root.tag="EditorOnly";var host=root.AddComponent<FullCasePlayHost>();Active=host;
            var player=new GameObject("TEST ONLY player");player.transform.SetParent(root.transform,false);player.SetActive(false);var cc=player.AddComponent<CharacterController>();cc.height=2;cc.radius=.32f;cc.center=Vector3.up;cc.minMoveDistance=0;var movement=player.AddComponent<DieuKhienDiChuyen>();movement.controller=cc;movement.speed=4;movement.jumpHeight=.8f;
            var eye=new GameObject("TEST ONLY eye");eye.transform.SetParent(player.transform,false);eye.transform.localPosition=Vector3.up*1.6f;var camera=eye.AddComponent<Camera>();camera.nearClipPlane=.03f;camera.farClipPlane=150;var look=player.AddComponent<DieuKhienGocNhin>();player.SetActive(true);host.Movement=movement;host.Look=look;host.Eye=camera;
            var lamp=new GameObject("TEST ONLY light");lamp.transform.SetParent(root.transform,false);lamp.transform.rotation=Quaternion.Euler(45,-25,0);lamp.AddComponent<Light>().type=LightType.Directional;
            int zoneIndex=0;var adapter=panel.ControllerForChecks;
            foreach(var location in adapter.Runtime.GetLocations()){
                var zone=new GameObject("TEST ONLY zone "+location.id);zone.transform.SetParent(root.transform,false);zone.transform.position=new Vector3(zoneIndex++*160,0,0);host.Zones.Add(location.id,zone);host.Spawns.Add(location.id,zone.transform.position+new Vector3(location.id=="office"?-4:0,.06f,-2.7f));
                Cube("floor "+location.id,zone.transform,new Vector3(0,-.15f,0),new Vector3(140,.3f,12));int i=0;
                foreach(var evidence in adapter.TestCatalog.evidence.Where(e=>e.location==location.id&&e.id!="decoded"))Target(host,zone.transform,location.id,"evidence."+evidence.id,evidence.id,FullPlayAction.Evidence,i++*2.2f);
                if(location.id=="market")Target(host,zone.transform,location.id,"evidence.receipt.channel","receipt.channel",FullPlayAction.Evidence,i++*2.2f);
                string actor=location.id=="office"?"phuong":location.id=="market"?"ha":location.id=="school"?"mai":location.id=="lake"?"bao":location.id=="slope"?"tung":location.id=="garden"?"lan":location.id=="homestay"?"host":"khai";
                Target(host,zone.transform,location.id,"actor."+location.id,"npc."+actor,FullPlayAction.Actor,-4);
                Target(host,zone.transform,location.id,"table."+location.id,"Bàn dựng",FullPlayAction.Page,-7,FullCaseModal.Reconstruction);
                if(location.id=="station")Target(host,zone.transform,location.id,"appointment","Cuộc hẹn",FullPlayAction.Appointment,-10);
                i=0;foreach(var destination in adapter.Runtime.GetLocations())Target(host,zone.transform,location.id,"travel."+location.id+"."+destination.id,destination.id,FullPlayAction.Travel,-14-i++*2.2f);
            }
            host.CursorContextChanged=CursorContext;host.TextInputFocused=()=>panel.TextInputFocused;host.DrawPanel=panel.DrawPlayUI;host.Inspect=panel.InspectFromWorld;host.Talk=panel.TalkFromWorld;host.Feedback=panel.ResultFromWorld;host.Profile=adapter.Profile;host.Visible=()=>EditorWindow.focusedWindow!=null&&EditorWindow.focusedWindow.GetType().Name=="GameView";host.Initialize(adapter.Runtime);
            var view=EditorWindow.GetWindow(typeof(Editor).Assembly.GetType("UnityEditor.GameView"));view.Show();view.Focus();
        }
    }
}
