import urllib.request,re,json
from pathlib import Path
R=Path('Source/Downloads/Quaternius_R104')
urls={'humanoid':'https://drive.google.com/drive/folders/1mWUjw1gRmgTer-P0t5Ssu6bgSlIOPNlg','individual':'https://drive.google.com/drive/folders/1_c3YIA1fLwK6ReOXhxzEbdkgnnH891Bl','catalog':'https://quaternius.com/index.html','license':'https://drive.google.com/uc?export=download&id=1TTvylHa1CsiJuHFWWiv6PFGhLM-aAH5z'}
for name,u in urls.items():
 try:
  b=urllib.request.urlopen(u,timeout=35).read();(R/(name+('.txt' if name=='license' else '.html'))).write_bytes(b);s=b.decode(errors='replace')
  if name=='license':print('LICENSE',s[:1500])
  elif name=='catalog':print('CATALOG',re.findall(r'href="([^"]*(?:character|people|human)[^"]*)"',s,re.I))
  else:
   rows=re.findall(r'data-id="([^\"]+)" jsname="vtaz5c".{0,150}?data-tooltip="([^\"]+)"',s);print(name,rows)
 except Exception as e:print(name,str(e))
