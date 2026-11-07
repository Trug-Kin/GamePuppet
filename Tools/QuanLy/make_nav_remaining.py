from pathlib import Path
s=Path('Tools/QuanLy/Verification/checkout-final-nav.cs.txt').read_text(encoding='utf-8-sig')
a=s.index('q.Enqueue(');z=s.index('double next=',a)
q='''
q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();b.AppendLine("P retry page="+s.Page);});
q.Enqueue(()=>key(Key.Escape));q.Enqueue(()=>{release();check(s.Paused,"Escape opens pause after cipher Back");shot("cipher-back-pause-1080");});
q.Enqueue(()=>click(130,144));q.Enqueue(()=>{check(s.Page=="dossier"&&!s.Paused,"Dossier accessible through pause button");});
q.Enqueue(()=>click(130,182));q.Enqueue(()=>{check(s.Page=="compare","Compare reached by dossier navigation");shot("compare-1080");});
q.Enqueue(()=>click(500,180));q.Enqueue(()=>click(950,220));q.Enqueue(()=>shot("compare-selected-1080"));
q.Enqueue(()=>click(130,103));q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="dossier","P returns after compare without text entry");});
q.Enqueue(()=>click(700,200));q.Enqueue(()=>{check(s.Page=="conclusions","Conclusions reached by dossier main action");shot("conclusions-1080");});
'''
s=s[:a]+q+s[z:]
s=s.replace('navigation-ui.txt','navigation-remaining-ui.txt')
Path('Tools/QuanLy/Verification/checkout-final-nav-remaining.cs.txt').write_text(s,encoding='utf-8')
