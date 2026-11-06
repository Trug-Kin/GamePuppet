from pathlib import Path
s=Path('Test/Tam/bank97-route.cs.txt').read_text();s=s.replace('678,15,502','666.79944f,15,450').replace('678,hit.point','666.79944f,hit.point').replace(',502),Quaternion',',450),Quaternion')
a=s.index('Vector2[] path=');b=s.index(';bool gameplayLight=',a)
coords=[(666.79944,450),(668.09944,450),(670.19944,450),(672.44944,450),(673.89944,450),(675.89944,450),(678.39944,450),(680.39944,450),(678.39944,450),(675.89944,450),(673.89944,450),(672.44944,450),(670.19944,450),(668.09944,450),(666.79944,450)]
s=s[:a]+'Vector2[] path={'+','.join('new Vector2('+str(x)+'f,'+str(z)+'f)' for x,z in coords)+'}'+s[b:]
s=s.replace('route-progress.txt','gameplay-route-progress.txt').replace('route-motion.txt','gameplay-route-motion.txt').replace('One initial local fixture','One initial low-path fixture, actual gameplay lighting throughout; separate from connected bridge route')
Path('Test/Tam/bank97-gameplay-route.cs.txt').write_text(s)
s=Path('Test/Tam/bank97-record.py').read_text().replace('bank97-route.cs.txt','bank97-gameplay-route.cs.txt').replace('route-response.json','gameplay-route-response.json').replace('bridgehead-bank-controller.mp4','bank-gameplay-stairs-controller.mp4').replace('route-motion.txt','gameplay-route-motion.txt').replace('capture-fixed-live.txt','capture-gameplay-live.txt').replace('capture-fixed.json','capture-gameplay.json').replace('one ground-review fixture on bridge','one separate gameplay review fixture on lower path Z450').replace('daylight outbound, gameplay return','actual gameplay lighting throughout this separate local fixture')
Path('Test/Tam/bank97-record-gameplay.py').write_text(s)
