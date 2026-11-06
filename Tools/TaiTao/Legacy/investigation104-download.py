import urllib.request,re,json,sys,hashlib
from pathlib import Path
root=Path('D:/ConRoi/Source/Downloads/Quaternius_R104');root.mkdir(parents=True,exist_ok=True)
u='https://drive.google.com/drive/folders/1USAAquX2JJWuA2m6zol0KUkFe3UkZ8zX?usp=sharing'
s=urllib.request.urlopen(u,timeout=30).read();(root/'official-drive-folder.html').write_bytes(s)
print(s.decode(errors='replace')[:1500])
text=s.decode(errors='replace')
print('\n'.join(re.findall(r'.{0,110}(?:\.zip|\.fbx|\.blend|CC0|license).{0,170}',text,re.I)))
