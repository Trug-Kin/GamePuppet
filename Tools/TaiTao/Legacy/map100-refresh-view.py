from pathlib import Path
s=Path('Test/Tam/map100-bed-views.cs.txt').read_text()
s=s.replace('string dir="','var ff=GameObject.Find("StoryProps/R22_Detailed_Route_Flowers").GetComponentsInChildren<MeshFilter>(true);var originals=ff.Select(f=>f.sharedMesh).ToArray();var clones=ff.Select(f=>Object.Instantiate(f.sharedMesh)).ToArray();for(int k=0;k<ff.Length;k++){clones[k].vertices=originals[k].vertices;clones[k].UploadMeshData(false);ff[k].sharedMesh=clones[k];}string dir="')
s=s.replace('"gate-bed","gate-overhead","gate-lamp-side","far-lamp-side"','"refresh-gate-bed","refresh-gate-overhead","refresh-gate-lamp-side","refresh-far-lamp-side"').replace('sun.intensity=power;','for(int k=0;k<ff.Length;k++){ff[k].sharedMesh=originals[k];Object.DestroyImmediate(clones[k]);}sun.intensity=power;').replace('bed-layer-rays.txt','refresh-layer-rays.txt')
Path('Test/Tam/map100-refresh-view.cs.txt').write_text(s)
