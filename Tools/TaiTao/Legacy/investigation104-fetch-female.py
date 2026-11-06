import urllib.request,re
from pathlib import Path
R=Path('Source/Downloads/Quaternius_R104')
u='https://drive.google.com/drive/folders/1X0armnO9EePmnQwP5EJHxpCiuucwG_uQ'
b=urllib.request.urlopen(u,timeout=35).read();(R/'animated-blends.html').write_bytes(b)
print(re.findall(r'data-id="([^\"]+)" jsname="vtaz5c".{0,150}?data-tooltip="([^\"]+)"',b.decode(errors='replace')))
b=urllib.request.urlopen('https://drive.google.com/uc?export=download&id=1eooYt7zHAMOPEU6d3QHMzR2p0gk9ZJF8',timeout=30).read();(R/'animated-license.txt').write_bytes(b);print(b.decode(errors='replace')[:1200])
