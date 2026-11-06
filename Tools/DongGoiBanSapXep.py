"""Dong goi player co san; output cau hinh duoc, khong tao build moi."""
from pathlib import Path
import argparse,json,hashlib,zipfile,os
from DuongDanKiem import R,goc_test,ket_qua,phien,dau_vao
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
def main():
 p=argparse.ArgumentParser();p.add_argument('--build',default='Builds/ConRoi-r92-Reorganized-20261102');p.add_argument('--output-root');p.add_argument('--episode',default=phien('dong-goi'));a=p.parse_args()
 if a.output_root:os.environ['CONROI_TEST_ROOT']=a.output_root
 B=dau_vao(a.build);assert (B/'ConRoi.exe').is_file(),'Can player da build'
 out=goc_test()/'BanChoiThu'/a.episode;Z=out/'ConRoi-Windows-playtest.zip';E=out/'GiaiNenKiem';O=ket_qua(a.episode)
 assert not out.exists() and not O.exists(),'Khong ghi de output';out.mkdir(parents=True);O.mkdir(parents=True)
 files=[p for p in B.rglob('*') if p.is_file() and not any('DontShip' in q for q in p.relative_to(B).parts)]
 manifest={p.relative_to(B).as_posix():sha(p) for p in files}
 with zipfile.ZipFile(Z,'x',zipfile.ZIP_DEFLATED,compresslevel=6) as z:
  for p in files:z.write(p,p.relative_to(B).as_posix())
 E.mkdir()
 with zipfile.ZipFile(Z) as z:
  assert z.testzip() is None;z.extractall(E)
 differences=[n for n,h in manifest.items() if sha(E/n)!=h]
 report={'zip':str(Z),'sha256':sha(Z),'bytes':Z.stat().st_size,'input_player':str(B),'runtime_files':len(files),'extracted':str(E),'extraction_hash_differences':differences,'startup':'NOT_RUN','excluded':'DontShip retained with raw player'}
 (O/'runtime-hashes.json').write_text(json.dumps(manifest,indent=2),encoding='utf-8');(O/'package.json').write_text(json.dumps(report,indent=2),encoding='utf-8');print(json.dumps(report));assert not differences
if __name__=='__main__':main()
