from pathlib import Path
import json,re,shutil
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout'
a=(R/'Assets/Scenes/JourneyPresentation_v48_r99.unity').read_text().splitlines();b=(R/'Assets/Scenes/JourneyPresentation_v48_r100.unity').read_text().splitlines();assert len(a)==len(b)
delta=[dict(line=i+1,r99=x,r100=y) for i,(x,y) in enumerate(zip(a,b)) if x!=y];(D/'r99-r100-scene-line-diff.json').write_text(json.dumps(delta,indent=2));assert len(delta)==8 and all(any(x['r99'].strip().startswith(k) and x['r100'].strip().startswith(k) for k in ['m_Mesh:','objectReference:']) for x in delta),delta
shutil.copy2(R/'Test/KetQua/journey_v48/dam_r99/migrated-private.json',D/'migrated-private.json')
print('Only eight mesh reference lines differ (six MeshFilter, two prefab mesh overrides); transforms/MonoBehaviours/colliders unchanged.')
