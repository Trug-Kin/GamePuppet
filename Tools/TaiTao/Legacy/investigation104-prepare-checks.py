from pathlib import Path
R=Path('D:/ConRoi')
for src,dst in [('investigation103-ready.cs.txt','investigation104-ready.cs.txt'),('investigation103-route.cs.txt','investigation104-route.cs.txt'),('investigation103-mechanism-check.cs.txt','investigation104-mechanism-check.cs.txt')]:
 s=(R/'Test/Tam'/src).read_text(encoding='utf-8').replace('R103','R104').replace('r103','r104').replace('Investigation103','Investigation104')
 (R/'Test/Tam'/dst).write_text(s,encoding='utf-8')
# Log the table raycast focus before E opens the page; screenshot exposes the actual prompt.
p=R/'Test/Tam/investigation104-route.cs.txt';s=p.read_text(encoding='utf-8')
s=s.replace('r103 normal','r104 normal')
p.write_text(s,encoding='utf-8')
