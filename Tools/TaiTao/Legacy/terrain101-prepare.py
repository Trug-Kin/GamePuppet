from pathlib import Path
p=Path('Test/Tam');s=(p/'map100-views.cs.txt').read_text(encoding='utf-8-sig')
s=s.replace('map_r99_closeout','terrain_r101').replace('new Vector3(683.53369f,3.12f,490.63019f),new Vector3(624.46228f,1.5f,535.79663f),new Vector3(693,2,507),new Vector3(643.5f,2,414)','new Vector3(643.5f,.26f,414),new Vector3(643.5f,.26f,414),new Vector3(643.5f,.26f,414),new Vector3(643.5f,2,414)').replace('"gate-lamp","far-lamp","historic-693-507","historic-643-414"','"top","underside","section","player"').replace('at[i]+new Vector3(-2,1.15f,-2)','at[i]+(i==0?new Vector3(0,16,-.01f):i==1?new Vector3(0,-8,-.01f):i==2?new Vector3(-8,.4f,0):new Vector3(-2,1.15f,-2))').replace('-r99.png','-before.png')
(p/'terrain101-views.cs.txt').write_text(s,encoding='utf8')
