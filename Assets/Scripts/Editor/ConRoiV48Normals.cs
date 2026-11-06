using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;
using Newtonsoft.Json.Linq;

public static class ConRoiV48Normals
{
 static bool Invalid(Vector3 n)=>float.IsNaN(n.x)||float.IsNaN(n.y)||float.IsNaN(n.z)||float.IsInfinity(n.x)||float.IsInfinity(n.y)||float.IsInfinity(n.z)||n.sqrMagnitude<1e-10f;
 public static void Repair(string report)
 {
  const string folder="Assets/Art/Environment/JourneyV48/FinitePineNormals";Directory.CreateDirectory(folder);
  var groups=UnityEngine.Object.FindObjectsByType<MeshFilter>(FindObjectsInactive.Include).Where(f=>f.sharedMesh&&AssetDatabase.GetAssetPath(f.sharedMesh).StartsWith("Assets/Art/Environment/JourneyV42/")&&(f.sharedMesh.name.Contains("Pine")||f.sharedMesh.name.Contains("Foliage"))).GroupBy(f=>f.sharedMesh).ToArray();
  var rows=new JArray();
  foreach(var group in groups){
   var source=group.Key;var positions=source.vertices;var normals=source.normals;if(normals.Length!=positions.Length)continue;
   var triangles=source.triangles;int invalid=triangles.Count(i=>Invalid(normals[i]));if(invalid==0)continue;
   var vertices=positions.ToList();var fixedNormals=normals.Select(n=>Invalid(n)?Vector3.up:n).ToList();var originalColors=source.colors;var colors=originalColors.ToList();
   var uv=new List<Vector4>[8];var originalUv=new List<Vector4>[8];for(int c=0;c<8;c++){originalUv[c]=new List<Vector4>();source.GetUVs(c,originalUv[c]);uv[c]=originalUv[c].ToList();}
   var submeshes=new List<int[]>();
   for(int sub=0;sub<source.subMeshCount;sub++){
    var indices=source.GetTriangles(sub);var result=indices.ToArray();
    for(int i=0;i<indices.Length;i+=3){
     if(!Invalid(normals[indices[i]])&&!Invalid(normals[indices[i+1]])&&!Invalid(normals[indices[i+2]]))continue;
     var face=Vector3.Cross(positions[indices[i+1]]-positions[indices[i]],positions[indices[i+2]]-positions[indices[i]]).normalized;
     if(Invalid(face))face=Vector3.up; // Degenerate geometry has no rasterized area.
     for(int j=0;j<3;j++){
      int old=indices[i+j];result[i+j]=vertices.Count;vertices.Add(positions[old]);fixedNormals.Add(Invalid(normals[old])?face:normals[old]);
      if(originalColors.Length==positions.Length)colors.Add(originalColors[old]);
      for(int c=0;c<8;c++)if(originalUv[c].Count==positions.Length)uv[c].Add(originalUv[c][old]);
     }
    }
    submeshes.Add(result);
   }
   var repaired=new Mesh{name=source.name+"_V48_SeparatedFaceNormals",indexFormat=vertices.Count>65535?IndexFormat.UInt32:IndexFormat.UInt16};
   repaired.SetVertices(vertices);repaired.SetNormals(fixedNormals);if(colors.Count>0)repaired.SetColors(colors);for(int c=0;c<8;c++)if(uv[c].Count>0)repaired.SetUVs(c,uv[c]);
   repaired.subMeshCount=submeshes.Count;for(int sub=0;sub<submeshes.Count;sub++)repaired.SetTriangles(submeshes[sub],sub);repaired.RecalculateBounds();
   bool identical=triangles.Select(i=>positions[i]).SequenceEqual(submeshes.SelectMany(s=>s).Select(i=>vertices[i]));
   if(!identical||fixedNormals.Any(Invalid))throw new Exception("Pine repair changed positions or retained invalid normals");
   string path=folder+"/"+source.name+".asset";var existing=AssetDatabase.LoadAssetAtPath<Mesh>(path);if(existing){EditorUtility.CopySerialized(repaired,existing);UnityEngine.Object.DestroyImmediate(repaired);repaired=existing;}else AssetDatabase.CreateAsset(repaired,path);
   foreach(var filter in group)filter.sharedMesh=repaired;
   rows.Add(new JObject{["source"]=AssetDatabase.GetAssetPath(source),["derived"]=path,["instances"]=group.Count(),["original_vertices"]=positions.Length,["original_invalid_normal_indices"]=invalid,["new_vertices"]=vertices.Count,["new_invalid_normals"]=fixedNormals.Count(Invalid),["triangle_count"]=triangles.Length/3,["same_ordered_triangle_positions"]=identical,["materials_changed"]=false});
  }
  File.WriteAllText(report+"/pine-normal-repair.json",rows.ToString());AssetDatabase.SaveAssets();
 }
}
