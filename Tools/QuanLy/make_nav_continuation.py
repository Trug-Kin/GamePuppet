from pathlib import Path
p=Path('Tools/QuanLy/Verification/checkout-final-nav.cs.txt')
s=p.read_text(encoding='utf-8-sig')
a=s.index('q.Enqueue(');z=s.index('double next=',a)
q='''
q.Enqueue(()=>click(285,484));
q.Enqueue(()=>{w.SendEvent(new Event{type=EventType.KeyDown,character='A',keyCode=KeyCode.A});w.SendEvent(new Event{type=EventType.KeyUp,keyCode=KeyCode.A});});
q.Enqueue(()=>{check(s.data.mapping.Count==1,"Cipher field accepts a typed letter through UI");shot("cipher-typed-1080");});
q.Enqueue(()=>click(130,103));q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="dossier","Back then P returns to dossier");});
q.Enqueue(()=>click(130,182));q.Enqueue(()=>{check(s.Page=="compare","Compare reached by dossier navigation");shot("compare-1080");});
q.Enqueue(()=>click(500,180));q.Enqueue(()=>click(950,220));q.Enqueue(()=>shot("compare-selected-1080"));
q.Enqueue(()=>click(130,103));q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="dossier","Dossier still reachable after compare");});
q.Enqueue(()=>click(700,200));q.Enqueue(()=>{check(s.Page=="conclusions","Conclusions reached from dossier main action");shot("conclusions-1080");});
'''
s=s[:a]+q+s[z:]
s=s.replace('navigation-ui.txt','navigation-continuation-ui.txt')
Path('Tools/QuanLy/Verification/checkout-final-nav-continuation.cs.txt').write_text(s,encoding='utf-8')
