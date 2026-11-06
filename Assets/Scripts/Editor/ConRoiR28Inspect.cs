using System.IO;
using System.Linq;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
public static class ConRoiR28Inspect {
 static JArray V(Vector3 p)=>new JArray(p.x,p.y,p.z);
 public static void Run(){EditorSceneManager.OpenScene(ConRoiR27Slope.Scene);var s=Object.FindAnyObjectByType<DieuPhoiHanhTrinh>();var ch=s.schoolChapter;
 var all=new JArray();foreach(var t in ch.environment.GetComponentsInChildren<Transform>(true)){
 if(t.parent!=ch.environment.transform&&!t.GetComponent<TuongTacTruongV37>()&&!t.GetComponent<DaoCuKeChuyenV40>())continue;
 var r=t.GetComponent<Renderer>();all.Add(new JObject{["name"]=t.name,["position"]=V(t.position),["rotation"]=V(t.eulerAngles),["active"]=t.gameObject.activeSelf,["bounds"]=r?new JArray(V(r.bounds.min),V(r.bounds.max)):null,["text"]=t.GetComponent<TextMesh>()?t.GetComponent<TextMesh>().text:null});}
 Directory.CreateDirectory("Docs/Reviews/journey_v48/r28");File.WriteAllText("Docs/Reviews/journey_v48/r28/inspect.json",new JObject{["environment"]=V(ch.environment.transform.position),["spawn"]=V(ch.spawn.position),["chair"]=V(ch.chair.position),["puppet"]=V(ch.puppet.transform.position),["ghost"]=V(ch.ghost.position),["objects"]=all,["rigs"]=new JArray(s.GetComponent<DienXuatHanhTrinhV45>().rigs.Select(r=>new JObject{["role"]=r.role,["actor"]=r.actor.name,["position"]=V(r.actor.position),["frame"]=r.frame.name}))}.ToString());
 }
}
