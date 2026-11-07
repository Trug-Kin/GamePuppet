from pathlib import Path
s=Path('Tools/QuanLy/Verification/checkout-final-nav.cs.txt').read_text(encoding='utf-8-sig')
a=s.index('q.Enqueue(');z=s.index('double next=',a)
q='''
q.Enqueue(()=>{check(s.Page=="conclusions","Conclusions current page from actual navigation");});
q.Enqueue(()=>click(500,256));q.Enqueue(()=>click(700,487));q.Enqueue(()=>click(700,527));
q.Enqueue(()=>{check(s.data.finalized&&File.Exists(s.SavePath),"Confirm and save conclusions through UI");check(s.data.conclusions.TrueForAll(c=>c.choice==0),"All conclusion choices remain insufficient proof; no automatic culprit");shot("conclusions-saved-1080");});
q.Enqueue(()=>key(Key.F6));q.Enqueue(()=>{release();check(s.Page=="conclusions"&&s.data.finalized&&s.data.mapping.Count==1&&s.Record("slide").observations.Count==2,"F6 preserves conclusion choice, typed mapping and actual observations in fixture profile");});
q.Enqueue(()=>click(130,225));q.Enqueue(()=>{check(s.Page=="compare","Compare accessible from conclusion navigation");});
q.Enqueue(()=>click(700,575));q.Enqueue(()=>{foreach(char c in "test")w.SendEvent(new Event{type=EventType.KeyDown,character=c});});q.Enqueue(()=>click(700,627));
q.Enqueue(()=>{check(s.data.relations.Count==1,"Compare relation entered and recorded through UI");shot("compare-relation-1080");});
q.Enqueue(()=>key(Key.F5));q.Enqueue(()=>{release();check(File.Exists(s.SavePath),"F5 while comparing writes fixture profile");});q.Enqueue(()=>key(Key.F6));q.Enqueue(()=>{release();check(s.Page=="compare"&&s.data.relations.Count==1,"F6 restores comparison and recorded relation");});
'''
s=s[:a]+q+s[z:]
s=s.replace('navigation-ui.txt','navigation-save-ui.txt')
Path('Tools/QuanLy/Verification/checkout-final-nav-save.cs.txt').write_text(s,encoding='utf-8')
