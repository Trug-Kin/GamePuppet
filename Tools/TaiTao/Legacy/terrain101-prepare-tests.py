from pathlib import Path
P=Path('Test/Tam')
def adapt(name,out):
 s=(P/name).read_text(encoding='utf-8-sig').replace('map_r99_closeout','terrain_r101').replace('r100.unity','r101.unity');(P/out).write_text(s,encoding='utf8');return s
adapt('map100-play.cs.txt','terrain101-play.cs.txt')
s=adapt('map100-fixture.cs.txt','terrain101-fixture.cs.txt').replace('new Vector3(678,3.9f,502)','new Vector3(654,2.44f,414)');(P/'terrain101-fixture.cs.txt').write_text(s,encoding='utf8')
s=adapt('map100-route.cs.txt','terrain101-route.cs.txt').replace('new Vector3(678,15,502)','new Vector3(654,15,414)').replace('new Vector3(678,hit.point.y-s.player.transform.TransformVector(cc0.center).y+cc0.height*.5f+cc0.skinWidth*.25f,502)','new Vector3(654,hit.point.y-s.player.transform.TransformVector(cc0.center).y+cc0.height*.5f+cc0.skinWidth*.25f,414)')
a=s.index('Vector2[] path=');b=s.index(';bool gameplayLight',a)
s=s[:a]+'Vector2[] path={new Vector2(650,414),new Vector2(646,414),new Vector2(644.6f,414),new Vector2(646,416),new Vector2(648,418),new Vector2(649.4f,420),new Vector2(648,418),new Vector2(646,416),new Vector2(644.6f,414),new Vector2(646,414),new Vector2(650,414),new Vector2(654,414)}'+s[b:]
s=s.replace('leg==22','leg==6').replace('t<280','t<100').replace('max280s','max100s');(P/'terrain101-route.cs.txt').write_text(s,encoding='utf8')
adapt('map100-pause.cs.txt','terrain101-pause.cs.txt')
adapt('map100-close-play.cs.txt','terrain101-close-play.cs.txt')
adapt('map100-game-window.cs.txt','terrain101-window.cs.txt')
s=(P/'map100-record.py').read_text().replace('map_r99_closeout','terrain_r101').replace('Test/Tam/map100-route.cs.txt','Test/Tam/terrain101-route.cs.txt').replace("'285'","'105'").replace('dam-end-stairs-controller.mp4','terrain-controller.mp4').replace('one ground-review fixture on bridge','one adjacent-good-ground fixture at X654/Z414')
(P/'terrain101-record.py').write_text(s)
