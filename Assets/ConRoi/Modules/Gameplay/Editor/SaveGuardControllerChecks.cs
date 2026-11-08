using System;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using ConRoi.Investigation.V36.R107;

namespace ConRoi.Gameplay.EditorChecks
{
    // Environment fixture only: gameplay progress must come from OS/controller/UI input.
    // No evidence grants, Save/Load calls, page opening or movement overrides in this tool.
    [InitializeOnLoad]
    public static class SaveGuardControllerChecks
    {
        const string Active = "ConRoi.SaveGuardController.Active";
        const string OutputKey = Active + ".Output";
        const string Scene = "Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
        static double next;
        static SaveGuardControllerChecks() { EditorApplication.update += Tick; }
        static string Arg(string key)
        {
            var args=Environment.GetCommandLineArgs();int at=Array.IndexOf(args,key);
            return at>=0&&at+1<args.Length?args[at+1]:"";
        }
        public static void StartGui()
        {
            if(Application.unityVersion!="6000.6.2f1"||Application.isBatchMode||EditorApplication.isPlayingOrWillChangePlaymode)
                throw new InvalidOperationException("Need GUI Unity 6000.6.2f1 outside Play.");
            string profile=Arg("--investigation-save-dir"), output=Arg("--save-controller-output");
            if(!Path.IsPathRooted(profile)||Directory.Exists(profile)||!Path.IsPathRooted(output))
                throw new InvalidOperationException("Need a new private absolute profile and absolute output directory.");
            if(SceneManager.GetActiveScene().isDirty)throw new InvalidOperationException("Preserve dirty scene before starting.");
            Directory.CreateDirectory(output);
            EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);
            SessionState.SetString(OutputKey,output);SessionState.SetBool(Active,true);
            SessionState.SetString(Active+".Phase","pending");
            EditorApplication.EnterPlaymode();
        }
        static GameObject Cube(string name,Vector3 position,Vector3 scale)
        {
            var go=GameObject.CreatePrimitive(PrimitiveType.Cube);go.name=name;go.transform.position=position;go.transform.localScale=scale;return go;
        }
        static void Setup()
        {
            var root=new GameObject("SaveGuard_Controller_EnvironmentOnly");root.SetActive(false);
            var floor=Cube("Sandbox floor",new Vector3(0,-.25f,3),new Vector3(20,.5f,20));floor.transform.SetParent(root.transform);
            var player=new GameObject("ControllerPlayer");player.transform.SetParent(root.transform);player.transform.position=new Vector3(0,1.05f,0);
            var cc=player.AddComponent<CharacterController>();cc.height=2;cc.radius=.3f;cc.minMoveDistance=0;
            var movement=player.AddComponent<DieuKhienDiChuyen>();movement.controller=cc;movement.speed=2;
            var eye=new GameObject("ControllerEye");eye.transform.SetParent(player.transform);eye.transform.localPosition=new Vector3(0,.6f,0);
            var camera=eye.AddComponent<Camera>();camera.clearFlags=CameraClearFlags.SolidColor;camera.backgroundColor=new Color(.15f,.2f,.24f);
            var look=eye.AddComponent<DieuKhienGocNhin>();
            var gates=new GameObject("Gates");gates.transform.SetParent(root.transform);
            var gateObject=new GameObject("Gate");gateObject.transform.SetParent(gates.transform);var gate=gateObject.AddComponent<ReturnGateMechanism>();gate.checkPlayerContact=false;gate.enabled=false;
            var modelObject=new GameObject("Model");modelObject.transform.SetParent(gates.transform);var model=modelObject.AddComponent<ReturnGateMechanism>();model.checkPlayerContact=false;model.enabled=false;
            foreach(var pair in new[]{new {id="slide",position=new Vector3(0,1.6f,2.7f)},new{id="shoeprint",position=new Vector3(2,1.6f,2.7f)}})
            {
                var target=Cube("Controller target "+pair.id,pair.position,new Vector3(.55f,.55f,.12f));target.transform.SetParent(root.transform);
                var interactable=target.AddComponent<InvestigationInteractable>();interactable.evidenceId=pair.id;
            }
            var sun=new GameObject("Sandbox light");sun.transform.SetParent(root.transform);var light=sun.AddComponent<Light>();light.type=LightType.Directional;light.intensity=1.4f;sun.transform.rotation=Quaternion.Euler(50,-30,0);
            var session=root.AddComponent<InvestigationSession>();session.movement=movement;session.look=look;session.eye=camera;session.gate=gate;session.modelGate=model;
            root.SetActive(true);Application.runInBackground=true;
            var view=EditorWindow.GetWindow(Type.GetType("UnityEditor.GameView,UnityEditor"));view.Show();view.maximized=true;view.Focus();
        }
        static JObject State()
        {
            var s=UnityEngine.Object.FindAnyObjectByType<InvestigationSession>();
            var state=new JObject{["utc"]=DateTime.UtcNow.ToString("o"),["unity"]=Application.unityVersion,["scene"]=SceneManager.GetActiveScene().path,["sceneDirty"]=SceneManager.GetActiveScene().isDirty,["play"]=EditorApplication.isPlaying,["compiling"]=EditorApplication.isCompiling,["width"]=Screen.width,["height"]=Screen.height};
            if(s){state["data"]=JObject.Parse(JsonUtility.ToJson(s.data));state["dataIdentity"]=RuntimeHelpers.GetHashCode(s.data);state["page"]=s.Page;state["paused"]=s.Paused;state["modal"]=s.Modal;state["message"]=s.Message;state["savePath"]=s.SavePath;state["position"]=JObject.Parse(JsonUtility.ToJson(s.movement.transform.position));state["yaw"]=s.movement.transform.eulerAngles.y;state["pitch"]=s.eye.transform.localEulerAngles.x;state["gate"]=JObject.Parse(JsonUtility.ToJson(s.gate.state));state["movement"]=s.movement.enabled;state["look"]=s.look.enabled;state["actions"]=s.movement.GameplayActionsEnabled;state["testInputHook"]=s.movement.TestInputHookActive;state["timeScale"]=Time.timeScale;state["cursor"]=Cursor.lockState.ToString();}
            return state;
        }
        static void Tick()
        {
            if(!SessionState.GetBool(Active,false)||EditorApplication.isCompiling||EditorApplication.isUpdating)return;
            string output=SessionState.GetString(OutputKey,"");if(output=="")return;
            try{
                string phase=SessionState.GetString(Active+".Phase","");
                if(phase=="pending"&&EditorApplication.isPlaying){SessionState.SetString(Active+".Phase","running");Setup();}
                if(EditorApplication.timeSinceStartup<next)return;next=EditorApplication.timeSinceStartup+.2;
                var state=State();File.WriteAllText(Path.Combine(output,"state.json"),state.ToString());
                string command=Path.Combine(output,"command.json");if(!File.Exists(command))return;
                var c=JObject.Parse(File.ReadAllText(command));string nonce=(string)c["nonce"];
                if(SessionState.GetString(Active+".Nonce","")==nonce)return;SessionState.SetString(Active+".Nonce",nonce);
                string action=(string)c["action"];
                if(action=="snapshot"){string stem="snapshot_"+Guid.Parse(nonce).ToString("N");File.WriteAllText(Path.Combine(output,stem+".json"),state.ToString());if(EditorApplication.isPlaying)ScreenCapture.CaptureScreenshot(Path.Combine(output,stem+".png"));}
                else if(action=="stop"){File.WriteAllText(Path.Combine(output,"before-stop.json"),state.ToString());EditorApplication.ExitPlaymode();SessionState.SetString(Active+".Phase","stopped");}
                else if(action=="start"&&!EditorApplication.isPlayingOrWillChangePlaymode){StartGui();}
                else if(action=="quit"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetBool(Active,false);EditorApplication.Exit(0);}
                else throw new InvalidOperationException("Unsupported command or unsafe phase.");
                File.WriteAllText(Path.Combine(output,"reply.json"),new JObject{["nonce"]=nonce,["action"]=action}.ToString());
            }catch(Exception e){File.WriteAllText(Path.Combine(output,"bridge-error.txt"),e.ToString());Debug.LogException(e);}
        }
    }
}
