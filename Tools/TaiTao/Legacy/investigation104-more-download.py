import urllib.request,re
from pathlib import Path
R=Path('Source/Downloads/Quaternius_R104')
urls={'blends':'https://drive.google.com/drive/folders/1Cq6EAKWDjgIVrfSxAbZ4g9mgIGpF9eDd','animated':'https://quaternius.com/packs/ultimatedanimatedcharacter.html'}
for name,u in urls.items():
 try:
  s=urllib.request.urlopen(u,timeout=30).read();(R/(name+'.html')).write_bytes(s);text=s.decode(errors='replace')
  if name=='blends':print(name,re.findall(r'data-id="([^\"]+)" jsname="vtaz5c".{0,150}?data-tooltip="([^\"]+)"',text))
  else:print(name,'\n'.join(re.findall(r'.{0,60}(?:drive.google|mediafire|CC0|February|2020|2021).{0,180}',text)))
 except Exception as e:print(name,e)
