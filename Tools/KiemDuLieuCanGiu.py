# Kiem chung cac du lieu duoc yeu cau giu, chi doc file goc.
from pathlib import Path
from DuongDanKiem import ket_qua,phien,dau_vao
import json,hashlib,os
R=Path(__file__).resolve().parents[1];O=R/'Docs/Reviews/journey_v48/restructure-20261102'
def read(p):return json.loads(p.read_text(encoding='utf-8-sig'))
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
def main():
 output=ket_qua(phien("bao-toan"));output.mkdir(parents=True,exist_ok=False)
 saves=read(O/'user-saves-before.json');zips=read(O/'delivered-zip-moves.json');m=read(R/'Docs/Reviews/journey_v48/r92/source-r92-manifest.json')['editable_sources']
 checks={'user_saves':{'count':len(saves),'differences':[e['path'] for e in saves if not Path(e['path']).is_file() or sha(Path(e['path']))!=e['sha256']]},'delivered_zips':{'count':len(zips),'differences':[e['new'] for e in zips if sha(dau_vao(R/e['new']))!=e['sha256']]},'editable_sources':{'count':len(m),'differences':[p for p,e in m.items() if not (R/p).is_file() or sha(R/p)!=e['sha256']]},'old_root_children':[p.name for p in Path('D:/Game/GameHorror/Game').iterdir()],'library_target':os.readlink(R/'Library'),'agents_preserved':(R/'.agents/skills').is_dir(),'checkpoint_sha256':sha(O/'checkpoint-before-restructure.zip')}
 checks['passed']=all(not checks[k]['differences'] for k in ['user_saves','delivered_zips','editable_sources']) and not checks['old_root_children'] and checks['checkpoint_sha256']=='544f96ae26afbf8e331914b9fda7043e4fbb8e5f37457d2510b56eebea0c8015'
 (output/'protected-data-final.json').write_text(json.dumps(checks,indent=2),encoding='utf-8');print(json.dumps(checks));assert checks['passed']
if __name__=='__main__':main()
