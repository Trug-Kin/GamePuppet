using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

// Regenerate imported artifacts only; never edit mesh geometry or source files.
public static class ConRoiR30AssetCache
{
 static JObject Fingerprint(string[] paths){
  var result=new JObject();
  foreach(string path in paths){
   var meshes=new JObject();
   foreach(var m in AssetDatabase.LoadAllAssetsAtPath(path).OfType<Mesh>()){
    AssetDatabase.TryGetGUIDAndLocalFileIdentifier(m,out string guid,out long id);
    using(var stream=new MemoryStream())using(var b=new BinaryWriter(stream)){
     foreach(var v in m.vertices){b.Write(v.x);b.Write(v.y);b.Write(v.z);}foreach(var v in m.normals){b.Write(v.x);b.Write(v.y);b.Write(v.z);}foreach(var uv in m.uv){b.Write(uv.x);b.Write(uv.y);}for(int i=0;i<m.subMeshCount;i++)foreach(int index in m.GetTriangles(i))b.Write(index);
     meshes[id.ToString()]=new JObject{["name"]=m.name,["vertices"]=m.vertexCount,["submeshes"]=m.subMeshCount,["sha256"]=BitConverter.ToString(SHA256.Create().ComputeHash(stream.ToArray())).Replace("-","")};
    }
   }
   result[path]=new JObject{["guid"]=AssetDatabase.AssetPathToGUID(path),["source_file_sha256"]=BitConverter.ToString(SHA256.Create().ComputeHash(File.ReadAllBytes(path))).Replace("-",""),["meshes"]=meshes};
  }
  return result;
 }
 public static void Run(){
  EditorSceneManager.OpenScene(ConRoiR30.Scene);
  var paths=AssetDatabase.GetDependencies(ConRoiR30.Scene,true).Where(p=>p.StartsWith("Assets/")&&new[]{".asset",".fbx",".blend"}.Contains(Path.GetExtension(p).ToLowerInvariant())).Where(p=>AssetDatabase.LoadAllAssetsAtPath(p).Any(o=>o is Mesh)).ToArray();
  var before=Fingerprint(paths);File.WriteAllText(ConRoiR30.Out+"/asset-cache-before.json",before.ToString());
  foreach(var p in paths){AssetDatabase.ImportAsset(p,ImportAssetOptions.ForceUpdate|ImportAssetOptions.ForceSynchronousImport);Debug.Log("R30_REIMPORT "+p);}
  var after=Fingerprint(paths);bool same=JToken.DeepEquals(before,after);
  File.WriteAllText(ConRoiR30.Out+"/asset-cache-reimport.json",new JObject{["paths"]=paths.Length,["same_geometry_uv_normals_guid_and_source_bytes"]=same,["after"]=after,["method"]="Force synchronous reimport of actual scene mesh dependencies; source files and geometry not edited."}.ToString());
  if(!same)throw new Exception("Reimport changed mesh/source fingerprint; inspect before build");
  ConRoiR30.Build();
 }
}
