import urllib.request,json,hashlib,re
from pathlib import Path
R=Path('Source/Downloads/Quaternius_R104')
for name,fileid in [('Casual_Hoodie.blend','1b1OSUM6eYVsHshEPo_6B86MZjcUHxE30'),('Casual_2.blend','1X-enmpVGyhUwjx8nCLUCdr-zXCq5eKoq')]:
 p=R/name
 if not p.exists():
  req=urllib.request.Request('https://drive.google.com/uc?export=download&id='+fileid,headers={'User-Agent':'Mozilla/5.0'})
  with urllib.request.urlopen(req,timeout=50) as res:p.write_bytes(res.read())
 print(name,p.stat().st_size,hashlib.sha256(p.read_bytes()).hexdigest(),p.read_bytes()[:12])
u='https://drive.google.com/drive/folders/1sNi1AfenfPRrvRt5yfaj5QMMd6KKcUJ5'
b=urllib.request.urlopen(u,timeout=30).read();(R/'animated-folder.html').write_bytes(b)
print('ANIMATED',re.findall(r'data-id="([^\"]+)" jsname="vtaz5c".{0,150}?data-tooltip="([^\"]+)"',b.decode(errors='replace')))
