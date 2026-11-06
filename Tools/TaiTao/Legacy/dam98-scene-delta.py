from pathlib import Path
import re,json
D=Path('Test/KetQua/journey_v48/dam_r98')
def read(rev):
 s=Path(f'Assets/Scenes/JourneyPresentation_v48_{rev}.unity').read_text(encoding='utf-8-sig');parts=re.split(r'(?m)^--- !u!(\d+) &(\d+)(?: stripped)?\n',s);return {parts[i+1]:(int(parts[i]),parts[i+2]) for i in range(1,len(parts),3)}
a=read('r97');b=read('r98');changed=[dict(id=k,type=a[k][0]) for k in a.keys()&b.keys() if a[k]!=b[k]];out=dict(changed_existing_blocks=changed,new_blocks=[dict(id=k,type=b[k][0]) for k in b.keys()-a.keys()],removed_blocks=list(a.keys()-b.keys()),existing_transform_changes=[x for x in changed if x['type'] in [4,224]],existing_mono_changes=[x for x in changed if x['type']==114]);(D/'scene-delta.json').write_text(json.dumps(out,indent=2));print('changes',len(changed),'removed',len(out['removed_blocks']),'existingTransformChanges',out['existing_transform_changes'],'existingMonoChanges',out['existing_mono_changes'])
