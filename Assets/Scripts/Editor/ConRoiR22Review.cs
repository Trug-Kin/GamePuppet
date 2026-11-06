using System;
using System.IO;
using System.Linq;
using System.Collections;
using Newtonsoft.Json.Linq;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public static class ConRoiR22Review
{
    const string Out=ConRoiR22Scenery.Out;
    static IEnumerator routine;static double deadline;static JArray checks=new JArray(),errors=new JArray();
    static ConRoiR22Review(){EditorApplication.playModeStateChanged+=state=>{if(state==PlayModeStateChange.EnteredPlayMode&&SessionState.GetBool("r22.review",false)){routine=Run();deadline=EditorApplication.timeSinceStartup+700;Application.logMessageReceived+=Log;EditorApplication.update+=Tick;}};}
    static void Log(string message,string trace,LogType type){if(type==LogType.Error||type==LogType.Exception||type==LogType.Assert)errors.Add(new JObject{["message"]=message,["trace"]=trace});}
    public static void Before(){Begin(true);}
    public static void After(){Begin(false);}
    public static void Pictures(){SessionState.SetBool("r22.pictures",true);Begin(false);}
    static void Begin(bool before){EditorSceneManager.OpenScene(before?ConRoiR22Scenery.Source:ConRoiR22Scenery.Scene);SessionState.SetBool("r22.before",before);SessionState.SetBool("r22.review",true);EditorApplication.EnterPlaymode();}
    static void Check(string name,bool pass,JToken detail=null){Debug.Log("R22_CHECK "+name+" "+pass);checks.Add(new JObject{["name"]=name,["pass"]=pass,["details"]=detail});if(!pass)throw new Exception(name);}
    static void Tick(){try{EditorApplication.QueuePlayerLoopUpdate();if(EditorApplication.timeSinceStartup>deadline)throw new Exception("Review timeout");if(routine.MoveNext())return;Finish(null);}catch(Exception e){Finish(e.ToString());}}
    static void Finish(string error){bool before=SessionState.GetBool("r22.before",false);File.WriteAllText(Out+"/"+(before?"before":"after")+"-playmode.json",new JObject{["complete"]=error==null&&errors.Count==0,["method"]="Automated Editor Play Mode; direct story APIs, normal PlayerMovement filter and URP render/readback. Batch initial loading gate released by fixture after first render. OS input and shipped loading gate NOT certified.",["checks"]=checks,["errors"]=errors,["failure"]=error}.ToString());SessionState.SetBool("r22.review",false);EditorApplication.update-=Tick;Application.logMessageReceived-=Log;Time.captureDeltaTime=0;EditorApplication.Exit(error==null&&errors.Count==0?0:1);}
    static IEnumerator Frames(int n){for(int i=0;i<n;i++)yield return null;}
    static IEnumerator Wait(double seconds){double end=EditorApplication.timeSinceStartup+seconds;while(EditorApplication.timeSinceStartup<end)yield return null;}
    static void Shot(Camera eye,string folder,string name){ConRoiR22Scenery.Capture(eye,Out+"/"+folder+"/"+name+".png");}
    static void InspectPuppet(Camera source,Transform puppet,string folder,string name)
    {
        var c=UnityEngine.Object.Instantiate(source);c.transform.SetParent(null);c.enabled=false;c.fieldOfView=60;c.aspect=1.6f;
        var ai=puppet.GetComponent<DieuKhienConRoi>();var head=ai&&ai.dauTransform?ai.dauTransform.position:puppet.position+Vector3.up*1.5f;
        var anchor=head-Vector3.up*1.5f;var p=head-Vector3.up*.55f;var forward=puppet.forward;forward.y=0;if(forward.sqrMagnitude<.01f)forward=Vector3.forward;
        Vector3 from=anchor+forward.normalized*3.5f+Vector3.up*1.55f;int best=-1;
        for(int i=0;i<8;i++){
            var candidate=anchor+Quaternion.Euler(0,i*45,0)*Vector3.forward*3.5f+Vector3.up*1.55f;int score=0;
            foreach(float h in new[]{.45f,.9f,1.4f}){var delta=anchor+Vector3.up*h-candidate;bool blocked=Physics.RaycastAll(candidate,delta.normalized,delta.magnitude,~0,QueryTriggerInteraction.Ignore).Any(hit=>!hit.transform.IsChildOf(puppet));if(!blocked)score++;}
            if(score>best){best=score;from=candidate;}
        }
        // The slope entrance is approached from the south. The northern view
        // is inside the closed private courtyard and is not a player view.
        if(name=="gd4-standing")from=anchor+Vector3.back*3.5f+Vector3.up*1.55f;
        c.transform.SetPositionAndRotation(from,Quaternion.LookRotation(p-from));c.ResetWorldToCameraMatrix();c.ResetCullingMatrix();Shot(c,folder,name);
        File.WriteAllText(Out+"/"+folder+"/"+name+"-camera.json",new JObject{["method"]="Separate stationary inspection camera near encounter, not walking footage or forced gameplay camera",["eye"]=new JArray(from.x,from.y,from.z),["target"]=new JArray(p.x,p.y,p.z)}.ToString());
        UnityEngine.Object.DestroyImmediate(c.gameObject);
    }
    static IEnumerator Run()
    {
        bool before=SessionState.GetBool("r22.before",false);bool pictures=SessionState.GetBool("r22.pictures",false);SessionState.SetBool("r22.pictures",false);string stage=before?"play-before":"play-after";
        var s=UnityEngine.Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var eye=s.player.GetComponentInChildren<Camera>();var mouse=s.player.GetComponentInChildren<DieuKhienGocNhin>();
        UnityEngine.InputSystem.InputSystem.settings.updateMode=UnityEngine.InputSystem.InputSettings.UpdateMode.ProcessEventsManually;Application.runInBackground=true;
        Check("isolated_profile_before_commands",s.SavePathOverride.Contains(before?"r21-playtest":"r22-playtest"),new JValue(s.SavePathOverride));
        s.SavePathOverride=System.IO.Path.GetFullPath(Out+"/"+(before?"before":"after")+"-fixture-save.json");s.SetMenu(false);s.Restart();
        var wait=Wait(1);while(wait.MoveNext())yield return null;
        var loader=s.GetComponent<ManHinhTaiV44>();loader.StopAllCoroutines();Shot(eye,stage,"opening");s.SetLoading(false);loader.enabled=false;if(mouse)mouse.enabled=false;
        var experience=s.GetComponent<TraiNghiemHanhTrinhR22>();
        if(!before&&!pictures){
            var scores=new Vector2(s.data.awareness,s.data.interactions);eye.transform.localRotation*=Quaternion.Euler(0,16,0);yield return null;
            Check("look_is_optional_presentation_memory",(s.data.experienceR22Flags&1)!=0&&scores==new Vector2(s.data.awareness,s.data.interactions));
            s.player.MovementFilter=input=>new Vector2(.25f,0);wait=Wait(1.5);while(wait.MoveNext())yield return null;s.player.MovementFilter=null;
            s.OpenPhone();yield return null;Shot(eye,stage,"opening-phone");Check("phone_replays_context_without_gate",experience.PhoneText.Contains("VIỆC TRƯỚC MẮT"));s.PhoneChoice(2);s.OpenPhone();
            // Restart skips the optional tutorial entirely. Sleep still advances.
            s.Restart();
        }
        s.Sleep();wait=Wait(4.5);while(wait.MoveNext())yield return null;Check("lake_without_tutorial_or_optional_NPC",s.data.region==0);
        yield return null;Shot(eye,stage,"lake-context");
        if(experience){var textUI=experience.GetComponentsInChildren<UnityEngine.UI.Text>(true).Single(t=>t.transform.parent&&t.transform.parent.name=="Hint"&&t.transform.parent.parent.name=="R22_ContextHint");Check("context_hint_fits_panel",textUI.preferredHeight<=textUI.rectTransform.rect.height+.1f,new JObject{["needed"]=textUI.preferredHeight,["available"]=textUI.rectTransform.rect.height});}
        if(mouse)mouse.enabled=false;s.player.enabled=false;
        var mist=s.GetComponent<SuongKhongGianR22>();var legacy=s.GetComponent<SuongDaLatV48>();
        foreach(var canvas in UnityEngine.Object.FindObjectsByType<Canvas>(FindObjectsInactive.Include))canvas.enabled=false;
        var localPosition=eye.transform.localPosition;var localRotation=eye.transform.localRotation;
        var reviewEye=UnityEngine.Object.Instantiate(eye);reviewEye.transform.SetParent(null);reviewEye.gameObject.name="R22_Fixed_Review_Camera";reviewEye.enabled=false;reviewEye.fieldOfView=60;reviewEye.aspect=1.6f;reviewEye.farClipPlane=1000;
        var fixedViews=new JArray();
        foreach(bool clear in new[]{false,true}){
            if(mist)mist.clearReview=clear;else legacy.clearGeometryReview=clear;
            wait=Wait(.3);while(wait.MoveNext())yield return null;
            foreach(var v in ConRoiR22Scenery.Views){reviewEye.transform.SetPositionAndRotation(v.eye,Quaternion.LookRotation(v.target-v.eye));reviewEye.ResetWorldToCameraMatrix();reviewEye.ResetCullingMatrix();Shot(reviewEye,stage,v.name+(clear?"-clear":"-mist"));fixedViews.Add(new JObject{["name"]=v.name,["clear"]=clear,["eye"]=new JArray(reviewEye.transform.position.x,reviewEye.transform.position.y,reviewEye.transform.position.z),["fov"]=reviewEye.fieldOfView});}
        }
        if(mist)mist.clearReview=false;else legacy.clearGeometryReview=false;
        if(pictures){var v=ConRoiR22Scenery.Views.Single(v=>v.name=="market");reviewEye.transform.SetPositionAndRotation(v.eye,Quaternion.LookRotation(v.target-v.eye));mist.debugDepth=true;Shot(reviewEye,stage,"market-depth");mist.debugDepth=false;}
        eye.transform.localPosition=localPosition;eye.transform.localRotation=localRotation;
        UnityEngine.Object.DestroyImmediate(reviewEye.gameObject);File.WriteAllText(Out+"/"+stage+"/cameras.json",fixedViews.ToString());
        var reflection=UnityEngine.Object.FindAnyObjectByType<PhanChieuMatHo>();Check("lake_reflection_renderer_active",reflection&&reflection.water&&reflection.water.gameObject.activeInHierarchy);
        var bridge=s.GetComponent<TuTheBaoBenCauV48>();var contact=BaoContactProbeV48.Sample(bridge);Check("bao_contact_preserved",contact["shoes"].All(p=>p["gap"].Type!=JTokenType.Null&&Mathf.Abs((float)p["gap"])<.025f)&&contact["sleeves"].All(p=>(float)p["triangleToTopRailSurfaceMin"]>=-.003f&&(float)p["triangleToTopRailSurfaceMin"]<.025f),contact);
        if(before||pictures)yield break;
        // The actual movement component supplies ordinary walking, including
        // gravity/support solving. No per-frame transform writes or OS injection.
        s.player.enabled=true;s.player.MovementFilter=input=>Vector2.right*.5f;wait=Wait(.45);while(wait.MoveNext())yield return null;s.player.MovementFilter=null;yield return null;
        var puppet=s.puppets[0];eye.transform.rotation=Quaternion.LookRotation(puppet.transform.position+Vector3.up*.9f-eye.transform.position);wait=Wait(.4);while(wait.MoveNext())yield return null;Shot(eye,stage,"gd1-observation");Check("gd1_motion_available",puppet.HasWitnessedPhase);
        // The preserved water shader exposes reflection as an HLSL uniform,
        // not a ShaderLab material property; GetTexture would log an error.
        var reflected=typeof(PhanChieuMatHo).GetField("texture",System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).GetValue(reflection) as RenderTexture;
        Check("gd1_reflection_has_rendered_texture",reflection.HasRendered&&reflected&&reflected.IsCreated());
        var previousRT=RenderTexture.active;RenderTexture.active=reflected;var pixels=new Texture2D(reflected.width,reflected.height,TextureFormat.RGB24,false);pixels.ReadPixels(new Rect(0,0,reflected.width,reflected.height),0,0);pixels.Apply();File.WriteAllBytes(Out+"/"+stage+"/gd1-reflection-texture.png",pixels.EncodeToPNG());RenderTexture.active=previousRT;UnityEngine.Object.DestroyImmediate(pixels);
        s.player.enabled=false;
        // Capture a continuous 16-second walk on the verified bridge sidewalk.
        var route=JObject.Parse(File.ReadAllText("Docs/Reviews/journey_v48/r21/recovery-fix/authoring.json"))["trails"][0]["route"].Select(p=>new Vector3((float)p[0],(float)p[1],(float)p[2])).ToArray();
        s.Place(route[0]+Vector3.up*.94f,Quaternion.LookRotation(new Vector3(route[1].x-route[0].x,0,route[1].z-route[0].z)));eye.transform.localPosition=localPosition;eye.transform.localRotation=Quaternion.identity;
        s.player.enabled=true;Vector2 steer=Vector2.zero;s.player.MovementFilter=input=>steer;Time.captureDeltaTime=1f/12;int next=1;var walk=new JArray();
        for(int frame=0;frame<192&&next<route.Length;frame++){
            var delta=route[next]-s.player.transform.position;delta.y=0;
            if(delta.magnitude<.45f){next++;if(next>=route.Length)break;delta=route[next]-s.player.transform.position;delta.y=0;}
            var heading=Quaternion.LookRotation(delta);s.player.transform.rotation=Quaternion.RotateTowards(s.player.transform.rotation,heading,8);var local=s.player.transform.InverseTransformDirection(delta.normalized);steer=new Vector2(local.x,local.z);
            yield return null;Shot(eye,"walk-frames","frame-"+frame.ToString("D4"));walk.Add(new JObject{["frame"]=frame,["position"]=new JArray(s.player.transform.position.x,s.player.transform.position.y,s.player.transform.position.z),["target"]=next});
        }
        steer=Vector2.zero;s.player.MovementFilter=null;Time.captureDeltaTime=0;File.WriteAllText(Out+"/walk.json",new JObject{["method"]="Editor Play Mode, PlayerMovement ordinary walking at unchanged 3.6m/s. 12fps fixed simulation, no jump, no per-frame relocation; initial placement only. OS input NOT_RUN.",["frames"]=walk}.ToString());
        Check("walking_video_has_continuous_positions",walk.Count>80&&Enumerable.Range(1,walk.Count-1).All(i=>Vector3.Distance(Read(walk[i]["position"]),Read(walk[i-1]["position"]))<.5f));
        // Save compatibility and optional chapter progression use only fixture files.
        s.data.experienceR22Flags|=32;s.Save();int saved=s.data.experienceR22Flags;s.data.experienceR22Flags=0;s.Load();Check("save_restores_new_presentation_memory",s.data.experienceR22Flags==saved);
        var text=JObject.Parse(File.ReadAllText(s.SavePathOverride));text.Remove("experienceR22Flags");File.WriteAllText(s.SavePathOverride,text.ToString());s.Load();Check("old_version1_save_defaults_new_memory",s.data.experienceR22Flags==0);
        s.Wake();s.Sleep();wait=Wait(4.5);while(wait.MoveNext())yield return null;Check("optional_lake_actions_do_not_block_market",s.data.region==1);
        s.player.MovementFilter=input=>Vector2.right*.5f;wait=Wait(.5);while(wait.MoveNext())yield return null;s.player.MovementFilter=null;wait=Wait(.7);while(wait.MoveNext())yield return null;
        Check("gd2_delayed_motion_available",s.puppets.Skip(1).Any(p=>p.HasWitnessedPhase));s.player.enabled=false;eye.transform.rotation=Quaternion.LookRotation(s.puppets[2].transform.position+Vector3.up*.8f-eye.transform.position);Shot(eye,stage,"gd2-observation");
        eye.transform.SetPositionAndRotation(new Vector3(789,9.2f,218),Quaternion.LookRotation(new Vector3(791,16,154)-new Vector3(789,9.2f,218)));yield return null;Shot(eye,stage,"market-night");
        s.Wake();s.Sleep();wait=Wait(4.5);while(wait.MoveNext())yield return null;Check("school_without_NPC_chain",s.data.region==2);
        var school=s.schoolChapter;if(mouse)mouse.enabled=false;eye.transform.rotation=Quaternion.LookRotation(school.puppet.transform.position+Vector3.up*.9f-eye.transform.position);wait=Wait(2);while(wait.MoveNext())yield return null;InspectPuppet(eye,school.puppet.transform,stage,"gd3-leading");Check("gd3_leads_while_player_can_stand",school.Leading);
        s.Wake();s.Sleep();wait=Wait(4.5);while(wait.MoveNext())yield return null;Check("slope_without_school_optional_tasks",s.data.region==3&&!s.slopeChapter.puppet.gameObject.activeInHierarchy);
        s.slopeChapter.BeginFinal();yield return null;var final=s.slopeChapter.puppet;var pos=final.transform.position;wait=Wait(1);while(wait.MoveNext())yield return null;Check("gd4_stays_stationary",!final.enabled&&Vector3.Distance(pos,final.transform.position)<.001f);
        if(mouse)mouse.enabled=false;InspectPuppet(eye,final.transform,stage,"gd4-standing");
        Check("hidden_formula_absent_from_phone",!experience.PhoneText.Contains("awareness")&&!experience.PhoneText.Contains("compliance"));
    }
    static Vector3 Read(JToken v)=>new Vector3((float)v[0],(float)v[1],(float)v[2]);
}
