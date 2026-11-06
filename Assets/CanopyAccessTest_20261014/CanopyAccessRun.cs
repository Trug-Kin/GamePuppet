using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public sealed class CanopyAccessRun : MonoBehaviour
{
    public DieuKhienDiChuyen player;
    public Vector3[] points;
    // Offline support-envelope measurements for this unchanged fixture.
    public Vector3[] spawnPositions;
    public string output;
    Vector2 testInput;
    StreamWriter frames;
    readonly List<RunResult> results = new List<RunResult>();
    string direction = "settle";
    bool complete;
    [Serializable] public class RunResult { public string direction, status; public float travelled, duration, maxLateralError; public int inputFrames; public string[] contacts; }
    [Serializable] public class Report { public string status, scene, input; public float radius,height,skinWidth,stepOffset,slopeLimit,speed,gravity; public Vector3 scale; public RunResult[] runs; }
    IEnumerator Start()
    {
        Directory.CreateDirectory(output);
        var cc=player.controller;
        player.useImmediateMovementInput=true;
        player.enableAutomatedTestInput=true;
        player.movementInputForTesting=()=>testInput;
        if(!player.TestInputHookActive) throw new InvalidOperationException("Existing PlayerMovement test hook is inactive");
        var contacts=player.gameObject.AddComponent<CanopyAccessContacts>();
        frames=new StreamWriter(Path.Combine(output,"controller_frames.csv"));
        frames.WriteLine("direction,time,x,y,z,grounded,input_x,input_y");
        foreach(bool reverse in new[]{false,true})
        {
            Vector3 a=points[reverse?points.Length-1:0],b=points[reverse?0:points.Length-1];
            testInput=Vector2.zero;cc.enabled=false;
            player.transform.position=spawnPositions[reverse?1:0];
            typeof(DieuKhienDiChuyen).GetField("velocity",BindingFlags.NonPublic|BindingFlags.Instance).SetValue(player,Vector3.zero);
            Vector3 heading=b-a;heading.y=0;player.transform.rotation=Quaternion.LookRotation(heading);
            cc.enabled=true;Physics.SyncTransforms();direction="settle";
            yield return new WaitForSeconds(1);
            direction=reverse?"reverse":"forward";contacts.names.Clear();
            float began=Time.realtimeSinceStartup,maxLateral=0;int inputFrames=0;bool reached=false;
            Vector3 axis=heading.normalized;float length=heading.magnitude;
            testInput=Vector2.up;
            while(Time.realtimeSinceStartup-began<8)
            {
                Vector3 delta=player.transform.position-a;delta.y=0;
                float progress=Vector3.Dot(delta,axis);
                maxLateral=Mathf.Max(maxLateral,(delta-axis*progress).magnitude);
                if(player.ReadMovementInput().sqrMagnitude>.5f)inputFrames++;
                if(progress>=length-.2f){reached=true;break;}
                yield return null;
            }
            testInput=Vector2.zero;
            Vector3 moved=player.transform.position-a;moved.y=0;
            results.Add(new RunResult{direction=direction,status=reached?"TRAVERSED_PLAY_MODE":"TIMEOUT_PLAY_MODE",travelled=Vector3.Dot(moved,axis),duration=Time.realtimeSinceStartup-began,maxLateralError=maxLateral,inputFrames=inputFrames,contacts=new List<string>(contacts.names).ToArray()});
            yield return new WaitForSeconds(.3f);
        }
        var report=new Report{status="PLAY_MODE_COMPLETED",scene=gameObject.scene.path,input="existing PlayerMovement.ReadMovementInput automated raw hook; original Move and gravity",radius=cc.radius,height=cc.height,skinWidth=cc.skinWidth,stepOffset=cc.stepOffset,slopeLimit=cc.slopeLimit,speed=player.speed,gravity=player.gravity,scale=player.transform.lossyScale,runs=results.ToArray()};
        File.WriteAllText(Path.Combine(output,"controller_result.json"),JsonUtility.ToJson(report,true));
        frames.Dispose();frames=null;complete=true;player.DisableTestInputHook();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying=false;
#endif
    }
    void LateUpdate()
    {
        if(frames==null)return;var p=player.transform.position;var i=player.ReadMovementInput();
        frames.WriteLine(string.Join(",",direction,Time.realtimeSinceStartup.ToString("R",System.Globalization.CultureInfo.InvariantCulture),p.x.ToString("R",System.Globalization.CultureInfo.InvariantCulture),p.y.ToString("R",System.Globalization.CultureInfo.InvariantCulture),p.z.ToString("R",System.Globalization.CultureInfo.InvariantCulture),player.controller.isGrounded,i.x,i.y));
    }
    void OnDisable(){testInput=Vector2.zero;frames?.Dispose();frames=null;if(player)player.DisableTestInputHook();if(!complete&&Directory.Exists(output))File.WriteAllText(Path.Combine(output,"interrupted.txt"),"Play mode ended before completing both directions; NOT a traversal PASS.");}
}
public sealed class CanopyAccessContacts : MonoBehaviour
{
    public readonly HashSet<string> names=new HashSet<string>();
    void OnControllerColliderHit(ControllerColliderHit hit){names.Add(hit.collider.name);}
}
