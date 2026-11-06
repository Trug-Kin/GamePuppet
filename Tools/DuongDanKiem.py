"""Duong dan output kiem thu: CONROI_TEST_ROOT hoac Test; khong dung trong code san pham."""
from pathlib import Path
import os,datetime
R=Path(__file__).resolve().parents[1]
def goc_test():
 p=Path(os.environ.get('CONROI_TEST_ROOT','Test'));return p if p.is_absolute() else R/p
def thu_muc(nhom,dot):return goc_test()/nhom/dot
def ket_qua(dot):return thu_muc('KetQua',dot)
def bang_chung(dot):return thu_muc('BangChung',dot)
def tam(dot):return thu_muc('Tam',dot)
def phien(tien_to):return tien_to+'-'+datetime.datetime.now().strftime('%Y%m%d-%H%M%S')
def dau_vao(path):
 """Tra file da chuyen; nguon/fixture con o Docs luon duoc uu tien, khong tao ban sao."""
 p=Path(path);p=p if p.is_absolute() else R/p
 if p.is_file():return p
 try:rel=p.relative_to(R).as_posix()
 except ValueError:return p
 if rel.startswith('Docs/Reviews/'):
  tail=rel[len('Docs/Reviews/'):]
  for group in ['KetQua','BangChung','BanChoiThu']:
   q=goc_test()/group/tail
   if q.exists():return q
 if rel.startswith('Builds/'):
  from ToChucDuLieuKiem import batch
  tail=rel[7:];top=tail.split('/')[0];group='BangChung' if p.suffix.lower() in {'.mp4','.avi','.webm','.mov'} else 'BanChoiThu'
  sub=('journey_v48/restructure-20261102/'+tail) if top=='Verification' else batch(top)+'/'+tail
  q=goc_test()/group/sub
  if q.exists():return q
 return p
if __name__=='__main__':
 import argparse
 p=argparse.ArgumentParser();p.add_argument('--resolve');a=p.parse_args();print(dau_vao(a.resolve) if a.resolve else goc_test())
