from pathlib import Path
p=Path('Test/Tam/market95-props-dump.cs.txt').read_text().replace('using UnityEngine;','using UnityEngine;using UnityEngine.Rendering;using Unity.Collections;',1)
p=p.replace('if(env&&m.isReadable&&','if(env&&!m.isReadable&&')
p=p.replace('props-inventory.json','props-extra-inventory.json').replace('props-baseline.meshbin','props-extra-baseline.meshbin').replace('props-scene-components.txt','props-extra-components.txt')
p=p.replace('var m=f.sharedMesh;w.Write(P(f.transform));w.Write(AssetDatabase.GetAssetPath(m));','var original=f.sharedMesh;var m=Readable(original);w.Write(P(f.transform));w.Write(AssetDatabase.GetAssetPath(original));')
p=p.replace('foreach(int x in ids)w.Write(x);}}}', 'foreach(int x in ids)w.Write(x);}if(m!=original)UnityEngine.Object.DestroyImmediate(m);}}')
helper='''static UnityEngine.Mesh Readable(UnityEngine.Mesh source){
using(var data=UnityEditor.MeshUtility.AcquireReadOnlyMeshData(source)){
var d=data[0];var m=new UnityEngine.Mesh();m.indexFormat=source.indexFormat;
using(var a=new NativeArray<Vector3>(d.vertexCount,Allocator.Temp)){d.GetVertices(a);m.vertices=a.ToArray();}
if(source.HasVertexAttribute(VertexAttribute.Normal))using(var a=new NativeArray<Vector3>(d.vertexCount,Allocator.Temp)){d.GetNormals(a);m.normals=a.ToArray();}
if(source.HasVertexAttribute(VertexAttribute.TexCoord0))using(var a=new NativeArray<Vector2>(d.vertexCount,Allocator.Temp)){d.GetUVs(0,a);m.uv=a.ToArray();}
if(source.HasVertexAttribute(VertexAttribute.Tangent))using(var a=new NativeArray<Vector4>(d.vertexCount,Allocator.Temp)){d.GetTangents(a);m.tangents=a.ToArray();}
m.subMeshCount=source.subMeshCount;
for(int j=0;j<d.subMeshCount;j++){var s=d.GetSubMesh(j);var ids=new int[s.indexCount];if(source.indexFormat==IndexFormat.UInt16){var raw=d.GetIndexData<ushort>();for(int i=0;i<ids.Length;i++)ids[i]=raw[s.indexStart+i]+s.baseVertex;}else{var raw=d.GetIndexData<int>();for(int i=0;i<ids.Length;i++)ids[i]=raw[s.indexStart+i]+s.baseVertex;}m.SetTriangles(ids,j);}
m.bounds=source.bounds;return m;}}
'''
p=p.replace('static string P(',helper+'static string P(')
Path('Test/Tam/market95-extra-props-dump.cs.txt').write_text(p,encoding='utf-8')
p=Path('Test/Tam/market95-props-cache.py').read_text().replace('props-baseline.meshbin','props-extra-baseline.meshbin').replace('props-unity-','props-extra-unity-').replace('props-geometry-inventory.json','props-extra-geometry-inventory.json')
Path('Test/Tam/market95-extra-props-cache.py').write_text(p,encoding='utf-8')
