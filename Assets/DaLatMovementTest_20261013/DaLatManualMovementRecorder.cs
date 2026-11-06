using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;

[DefaultExecutionOrder(10000)]
public class DaLatManualMovementRecorder : MonoBehaviour
{
    [Serializable] public class Case {public string id,direction;public Vector3[] points;public bool stationary;public float yaw;}
    public DieuKhienDiChuyen player;
    public Case[] cases;
    public bool recordOnPlay=false;
    public string outputRoot=@"D:\DaLatMap\output\night_market_20261013_182234_unity_movement_test\raycast_manual_followup_20261013\manual_runs";
    int selected;bool recording,ended,inputSeen,mouseSeen,walkSeen;
    float started,stillSince=-1;Vector3 stopStart;float maxStopDrift;
    string folder;StreamWriter frames,events;
    readonly HashSet<string> supports=new HashSet<string>();
    string[] statuses;
    FieldInfo velocity=typeof(DieuKhienDiChuyen).GetField("velocity",BindingFlags.Instance|BindingFlags.NonPublic);
    void Start()
    {
        if(!recordOnPlay){enabled=false;return;}
        folder=Path.Combine(outputRoot,DateTime.UtcNow.ToString("yyyyMMdd_HHmmss"));Directory.CreateDirectory(folder);
        statuses=new string[cases.Length];for(int i=0;i<statuses.Length;i++)statuses[i]="NOT_RUN";
        frames=new StreamWriter(folder+"/frames.csv"){AutoFlush=true};events=new StreamWriter(folder+"/events.csv"){AutoFlush=true};
        frames.WriteLine("case,direction,time,x,y,z,input_x,input_z,mouse_x,mouse_y,grounded,hook_active,ground_contacts");
        events.WriteLine("case,direction,status,note");Snapshot();
        if(player.TestInputHookActive||player.enableAutomatedTestInput||!player.GetComponent<DieuKhienGocNhin>().enabled||player.GetComponent<NhichCameraKhiDungYen>().enabled){Stop("Configuration invalid for physical keyboard/mouse test");return;}
        player.movementInputForTesting=null;Place();
        if(!Focus()){Stop("Game View not focused at start");return;}
        File.WriteAllText(folder+"/mode.txt","Physical keyboard/mouse only; no synthetic keys or input provider. NOT_RUN until explicitly recorded by operator; no automatic PASS.");
    }
    bool Focus()
    {
#if UNITY_EDITOR
        return Application.isFocused&&UnityEditor.EditorWindow.focusedWindow!=null&&UnityEditor.EditorWindow.focusedWindow.GetType().Name=="GameView";
#else
        return Application.isFocused;
#endif
    }
    void OnApplicationFocus(bool focused){if(recordOnPlay&&!focused&&frames!=null&&!ended)Stop("Game View lost focus");}
    void Update()
    {
        if(!recordOnPlay||ended||frames==null)return;
        if(!Focus()){Stop("Game View lost focus");return;}
        if(Input.GetKeyDown(KeyCode.F9)){Stop("Operator stopped the manual run");return;}
        if(Input.GetKeyDown(KeyCode.F6))
        {
            if(recording){Event("NOT_RUN","Changed case before completion");recording=false;}
            selected=(selected+1)%cases.Length;Place();Snapshot();
        }
        if(Input.GetKeyDown(KeyCode.F7)){recording=true;started=Time.realtimeSinceStartup;inputSeen=mouseSeen=walkSeen=false;stillSince=-1;maxStopDrift=0;supports.Clear();}
        if(Input.GetKeyDown(KeyCode.F8)&&recording)
        {
            bool sufficient=cases[selected].stationary?walkSeen&&stillSince>=0&&Time.realtimeSinceStartup-stillSince>=10:inputSeen;
            Event(sufficient?"RECORDED_MANUAL_REVIEW":"NOT_RUN",sufficient?"Operator ended physical-input capture; inspect frame log; mouse_seen="+mouseSeen+" stop_drift_m="+maxStopDrift.ToString("R",CultureInfo.InvariantCulture):"No movement or stationary interval shorter than 10 s");
            recording=false;Snapshot();
        }
    }
    void LateUpdate()
    {
        if(!recording||ended||frames==null)return;
        Vector2 axis=player.ReadMovementInput();float mx=Input.GetAxisRaw("Mouse X"),my=Input.GetAxisRaw("Mouse Y");
        bool moving=axis.sqrMagnitude>.0001f;inputSeen|=moving;mouseSeen|=Mathf.Abs(mx)+Mathf.Abs(my)>.001f;walkSeen|=moving;
        if(cases[selected].stationary)
        {
            if(moving)stillSince=-1;
            else if(walkSeen){if(stillSince<0){stillSince=Time.realtimeSinceStartup;stopStart=player.transform.position;}maxStopDrift=Mathf.Max(maxStopDrift,Vector3.Distance(stopStart,player.transform.position));}
        }
        Vector3 p=player.transform.position;
        string F(float value)=>value.ToString("R",CultureInfo.InvariantCulture);
        frames.WriteLine(string.Join(",",cases[selected].id,cases[selected].direction,F(Time.realtimeSinceStartup),F(p.x),F(p.y),F(p.z),F(axis.x),F(axis.y),F(mx),F(my),player.controller.isGrounded,player.TestInputHookActive,string.Join("|",supports)));
        supports.Clear();
    }
    void OnControllerColliderHit(ControllerColliderHit hit){if(recording&&hit.normal.y>.1f&&((1<<hit.collider.gameObject.layer)&player.groundMask.value)!=0)supports.Add(hit.collider.name);}
    void Place()
    {
        var cc=player.controller;cc.enabled=false;player.transform.position=cases[selected].points[0]+Vector3.up*(cc.height/2-cc.center.y+.04f);cc.enabled=true;
        velocity.SetValue(player,Vector3.zero);float yaw=cases[selected].yaw;player.transform.rotation=Quaternion.Euler(0,yaw,0);
        var mouse=player.GetComponent<DieuKhienGocNhin>();typeof(DieuKhienGocNhin).GetField("YRotation",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(mouse,yaw);typeof(DieuKhienGocNhin).GetField("xRotation",BindingFlags.Instance|BindingFlags.NonPublic).SetValue(mouse,0f);
        Physics.SyncTransforms();
    }
    void Event(string status,string note){statuses[selected]=status;events.WriteLine(string.Join(",",cases[selected].id,cases[selected].direction,status,note.Replace(',',';')));Snapshot();}
    void Snapshot()
    {
        using(var writer=new StreamWriter(folder+"/case_status.csv")){writer.WriteLine("case,direction,status");for(int i=0;i<cases.Length;i++)writer.WriteLine(cases[i].id+","+cases[i].direction+","+statuses[i]);}
    }
    void Stop(string reason)
    {
        if(ended)return;if(frames==null)return;
        Event("NOT_RUN",reason);File.WriteAllText(folder+"/stopped.txt",reason+"\nClick Game View and restart the explicit manual-test menu. No synthetic input fallback.");
        recording=false;ended=true;frames.Dispose();events.Dispose();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#endif
    }
    void OnGUI()
    {
        if(!recordOnPlay||ended||cases==null||cases.Length==0)return;
        GUI.Box(new Rect(8,8,640,110),"");
        GUI.Label(new Rect(18,15,620,95),"PHYSICAL INPUT ONLY — "+cases[selected].id+" / "+cases[selected].direction+"\nF6: next/place start   F7: begin capture   F8: finish   F9: stop\nWASD + mouse. For slope: move briefly; release WASD; stand >=10 s.\n"+(recording?"Recording; still seconds="+(stillSince<0?0:Time.realtimeSinceStartup-stillSince).ToString("F1"):"NOT_RUN / waiting for F7"));
    }
    void OnDisable(){if(recordOnPlay&&frames!=null&&!ended)Stop("Play Mode ended before manual capture completion");}
}
