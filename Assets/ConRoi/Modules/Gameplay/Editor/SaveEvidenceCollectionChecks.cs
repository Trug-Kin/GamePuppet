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
namespace ConRoi.Gameplay.EditorChecks {
// Explicit serialized-file/API fixture. Does not run unless RunBatch is requested.
[InitializeOnLoad] public static class SaveEvidenceCollectionChecks {
 const string Key="ConRoi.SaveEvidence04",Scene="Assets/ConRoi/Modules/Gameplay/Scenes/Gameplay_ThuNghiem.unity";
 static SaveEvidenceCollectionChecks(){EditorApplication.update+=Tick;}
 static string Arg(string key){var a=Environment.GetCommandLineArgs();int i=Array.IndexOf(a,key);return i>=0&&i+1<a.Length?a[i+1]:"";}
 public static void RunBatch(){
  if(Application.unityVersion!="6000.6.2f1"||!Application.isBatchMode||EditorApplication.isPlayingOrWillChangePlaymode)throw new InvalidOperationException("Need explicit batch Unity 6000.6.2f1 outside Play");
  string profile=Arg("--investigation-save-dir"),output=Arg("--evidence-check-output");if(!Path.IsPathRooted(profile)||Directory.Exists(profile)||!Path.IsPathRooted(output))throw new InvalidOperationException("Need NEW absolute test profile and output");
  Directory.CreateDirectory(output);EditorSceneManager.OpenScene(Scene,OpenSceneMode.Single);SessionState.SetString(Key,"pending");EditorApplication.EnterPlaymode();
 }
 static void Tick(){if(EditorApplication.isCompiling||EditorApplication.isUpdating)return;string phase=SessionState.GetString(Key,"");if(phase=="pending"&&EditorApplication.isPlaying){SessionState.SetString(Key,"running");Execute();}else if(phase=="finished"&&!EditorApplication.isPlayingOrWillChangePlaymode){SessionState.SetString(Key,"");EditorApplication.Exit(SessionState.GetInt(Key+".exit",2));}}
 static JObject State(InvestigationSession s)=>new JObject{["data"]=JObject.Parse(JsonUtility.ToJson(s.data)),["identity"]=RuntimeHelpers.GetHashCode(s.data),["position"]=JObject.Parse(JsonUtility.ToJson(s.movement.transform.position)),["rotation"]=JObject.Parse(JsonUtility.ToJson(s.movement.transform.rotation)),["eye"]=JObject.Parse(JsonUtility.ToJson(s.eye.transform.localRotation)),["gate"]=JObject.Parse(JsonUtility.ToJson(s.gate.state)),["modelGate"]=JObject.Parse(JsonUtility.ToJson(s.modelGate.state)),["page"]=s.Page,["paused"]=s.Paused,["modal"]=s.Modal,["movement"]=s.movement.enabled,["look"]=s.look.enabled,["controller"]=s.movement.controller.enabled,["timeScale"]=Time.timeScale,["trialId"]=s.TrialId,["trialTime"]=s.TrialTime,["trialRunning"]=s.TrialRunning};
 static void Execute(){
  var rows=new JArray();var audit=new JArray();GameObject root=null;string output=Arg("--evidence-check-output");int failures=0;
  Action<string,bool,string> check=(name,pass,detail)=>{rows.Add(new JObject{["name"]=name,["pass"]=pass,["detail"]=detail});if(!pass)failures++;};
  try{
   check("private_gameplay_scene",SceneManager.GetActiveScene().path==Scene,"No integration scene or source save used");
   root=new GameObject("SaveEvidence04_API_Fixture");root.SetActive(false);var player=new GameObject("Player");player.transform.SetParent(root.transform);var cc=player.AddComponent<CharacterController>();var movement=player.AddComponent<DieuKhienDiChuyen>();movement.controller=cc;var eye=new GameObject("Eye");eye.transform.SetParent(player.transform);var camera=eye.AddComponent<Camera>();var look=eye.AddComponent<DieuKhienGocNhin>();
   var g=new GameObject("Gate");g.transform.SetParent(root.transform);var gate=g.AddComponent<ReturnGateMechanism>();gate.checkPlayerContact=false;gate.enabled=false;var m=new GameObject("Model");m.transform.SetParent(root.transform);var model=m.AddComponent<ReturnGateMechanism>();model.checkPlayerContact=false;model.enabled=false;
   var s=root.AddComponent<InvestigationSession>();s.movement=movement;s.look=look;s.eye=camera;s.gate=gate;s.modelGate=model;root.SetActive(true);s.enabled=false;movement.enabled=false;look.enabled=false;s.ClosePage();
   check("private_profile",s.SaveDirectoryOverride==Arg("--investigation-save-dir"),s.SavePath);s.Save();byte[] empty=File.ReadAllBytes(s.SavePath);check("new_game_empty_save_load",s.Load()&&s.data.evidence.Count==0,"Writer emits explicit evidence:[]");
   s.Acquire("slide");s.Observe("slide",0);s.Acquire("shoeprint");s.Observe("shoeprint",0);s.AddRelation("slide","shoeprint","API fixture only");s.Save();byte[] good=File.ReadAllBytes(s.SavePath);var original=JObject.Parse(File.ReadAllText(s.SavePath));
   Func<JObject,string> serialize=x=>x.ToString();
   Action<string,Action<JObject>,bool> run=(name,mutate,shouldLoad)=>{
    File.WriteAllBytes(s.SavePath,good);if(!s.Load())throw new Exception("Cannot restore valid fixture");s.OpenPage("dossier");s.SetPaused(true);var before=State(s);var input=(JObject)original.DeepClone();mutate(input);byte[] payload=System.Text.Encoding.UTF8.GetBytes(serialize(input));File.WriteAllBytes(s.SavePath,payload);bool loaded=s.Load();var after=State(s);bool bytes=File.ReadAllBytes(s.SavePath).SequenceEqual(payload);var row=new JObject{["case"]=name,["expectedLoad"]=shouldLoad,["loaded"]=loaded,["stateUnchanged"]=JToken.DeepEquals(before,after),["bytesUnchanged"]=bytes,["message"]=s.Message,["beforeEvidence"]=((JArray)before["data"]["evidence"]).Count,["afterEvidence"]=s.data.evidence.Count};audit.Add(row);check(name,loaded==shouldLoad&&bytes&&(shouldLoad||JToken.DeepEquals(before,after)),row.ToString(Newtonsoft.Json.Formatting.None));
    File.WriteAllBytes(s.SavePath,good);bool recovered=s.Load()&&s.Observation("slide",0);s.Observe("slide",1);s.Save();check(name+"_recovery_save_load",recovered&&s.Load()&&s.Observation("slide",1),"Valid fixture restored; Load/continue/Save/Load");
   };
   run("evidence_null",x=>x["evidence"]=JValue.CreateNull(),false);run("evidence_missing",x=>x.Remove("evidence"),false);run("evidence_empty",x=>x["evidence"]=new JArray(),true);run("evidence_valid",x=>{},true);run("evidence_null_record",x=>x["evidence"]=new JArray(JValue.CreateNull()),false);run("evidence_object",x=>x["evidence"]=new JObject(),false);run("evidence_string",x=>x["evidence"]="[]",false);
   run("observations_null",x=>x["evidence"][0]["observations"]=JValue.CreateNull(),false);run("observations_missing_legacy_default",x=>((JObject)x["evidence"][0]).Remove("observations"),true);run("observations_null_item",x=>x["evidence"][0]["observations"]=new JArray(JValue.CreateNull()),false);
   foreach(string field in new[]{"mapping","relations","trials","conclusions"}){string f=field;run(f+"_null",x=>x[f]=JValue.CreateNull(),false);run(f+"_missing_legacy_default",x=>x.Remove(f),true);run(f+"_empty",x=>x[f]=new JArray(),true);run(f+"_null_record",x=>x[f]=new JArray(JValue.CreateNull()),false);}
   run("grounds_null",x=>x["conclusions"][0]["grounds"]=JValue.CreateNull(),false);run("grounds_missing_legacy_default",x=>((JObject)x["conclusions"][0]).Remove("grounds"),true);
   run("out_of_range_observation",x=>x["evidence"][0]["observations"]=new JArray(999),false);run("duplicate_evidence",x=>((JArray)x["evidence"]).Add(x["evidence"][0].DeepClone()),false);run("unknown_evidence",x=>x["evidence"][0]["id"]="not_in_registry",false);run("relation_unknown_endpoint",x=>x["relations"][0]["first"]="not_in_registry",false);run("relation_null_record",x=>x["relations"]=new JArray(JValue.CreateNull()),false);
   serialize=x=>x.ToString().Insert(1,"\"evidence\":null,");run("duplicate_evidence_key_null_first",x=>{},false);
   serialize=x=>x.ToString().Insert(x.ToString().Length-1,",\"evidence\":null");run("duplicate_evidence_key_null_last",x=>{},false);serialize=x=>x.ToString();
   string compat=Arg("--evidence-check-compat");if(compat!=""){byte[] old=File.ReadAllBytes(compat);var schema=JObject.Parse(System.Text.Encoding.UTF8.GetString(old));check("baseline_fixture_has_explicit_evidence_array",schema["evidence"] is JArray&&schema.Value<int>("schema")==1,"Read-only old fixture");File.WriteAllBytes(s.SavePath,old);check("baseline_schema1_load",s.Load()&&s.Observation("slide",0)&&!s.Observation("slide",1)&&s.data.mapping.Count==1&&s.data.relations.Count==1,"Correct baseline API fixture; not UI fixture");s.Observe("slide",1);s.Save();check("baseline_continue_save_load",s.Load()&&s.Observation("slide",1),"");check("baseline_source_bytes_unchanged",old.SequenceEqual(File.ReadAllBytes(compat)),"");}
   File.WriteAllBytes(s.SavePath,empty);check("empty_new_game_load_after_progress",s.Load()&&s.data.evidence.Count==0,"Explicit [] remains legitimate even after a progressed session");
  }catch(Exception e){check("fixture_exception",false,e.ToString());}finally{if(root)UnityEngine.Object.DestroyImmediate(root);Time.timeScale=1;File.WriteAllText(Path.Combine(output,"results.json"),new JObject{["unity"]=Application.unityVersion,["scene"]=Scene,["kind"]="SERIALIZED_FILE_PLAYMODE_API_FIXTURE_NOT_NATURAL_ROUTE",["checks"]=rows,["failures"]=failures,["audit"]=audit}.ToString());SessionState.SetInt(Key+".exit",failures==0?0:1);SessionState.SetString(Key,"finished");EditorApplication.ExitPlaymode();}
 }
}}
