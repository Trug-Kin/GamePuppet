from pathlib import Path
P=Path(__file__).parent
s=(P/'fixed-suite.cs.txt').read_text(encoding='utf-8')
s=s.replace('s.Page!="intro"||s.InspectionCount!=0','s.Page!="inspect"')
a=s.index('q.Enqueue(');z=s.index('double next=',a)
q=r'''
q.Enqueue(()=>{check(s.Page=="inspect","E controller route opened floor");});
q.Enqueue(()=>key(Key.F5));q.Enqueue(()=>{release();check(File.Exists(s.SavePath),"F5 in inspected object");});q.Enqueue(()=>key(Key.R));q.Enqueue(()=>{release();check(s.Page=="","R leaves inspected object");});q.Enqueue(()=>key(Key.F6));q.Enqueue(()=>{release();check(s.Page=="inspect"&&!s.Paused,"F6 restores inspection without stuck pause");});
q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="dossier","P works after inspection load");});q.Enqueue(()=>click(130,138));q.Enqueue(()=>{check(s.Page=="cipher","Cipher for typing guard");click(285,484);});
q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="cipher","P remains a text input key while a field is focused, does not open dossier");});
q.Enqueue(()=>click(130,103));q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="dossier","P works again immediately after closing focused field");});q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="","P closes dossier after typing guard");map=InputSystem.ListEnabledActions().First(a=>a.name=="Move"&&a.actionMap.name=="Gameplay").actionMap;oldDevices=map.devices;map.devices=new InputDevice[]{kb,mouse};});
foreach(var k in new[]{Key.W,Key.S,Key.A,Key.D}){Key direction=k;q.Enqueue(()=>{worldPosition=s.movement.transform.position;key(direction);});q.Enqueue(()=>{release();check(Vector2.Distance(new Vector2(worldPosition.x,worldPosition.z),new Vector2(s.movement.transform.position.x,s.movement.transform.position.z))>.05f,"Controller direction "+direction+" moves after UI");});}
q.Enqueue(()=>{check(s.movement.controller.isGrounded,"Controller remains grounded");shot("tail-world-1080");});
'''
s=s[:a]+q+s[z:]
s=s.replace('/suite.txt','/tail.txt').replace('/progress.txt','/tail-progress.txt').replace('/live.json','/tail-live.json')
(P/'fixed-tail.cs.txt').write_text(s,encoding='utf-8')
