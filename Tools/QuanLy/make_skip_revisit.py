from pathlib import Path
s=Path('Tools/QuanLy/Verification/checkout-final-nav.cs.txt').read_text(encoding='utf-8-sig')
s=s.replace('if(!s.SavePath.Contains("NavFixture_a789222")||!s.Has("letter"))throw new Exception("Isolated navigation fixture required");','if(!s.SavePath.Contains("Skip_a789222")||!s.Has("slide")||s.Page!="inspect")throw new Exception("Skip route inspection required");')
a=s.index('q.Enqueue(');z=s.index('double next=',a)
q='''
q.Enqueue(()=>{check(s.data.tutorialSkipped&&!s.data.firstFloorRecorded&&s.Record("slide").observations.Count==0,"Skipped route has only viewed floor, no observations auto-recorded");});
q.Enqueue(()=>key(Key.R));q.Enqueue(()=>{release();check(s.Page=="","R returns from skipped inspection");});
q.Enqueue(()=>key(Key.E));q.Enqueue(()=>{release();check(s.Page=="inspect"&&s.data.inspecting=="slide","E revisits same floor through existing controller raycast");});
q.Enqueue(()=>key(Key.F5));q.Enqueue(()=>{release();check(File.Exists(s.SavePath),"F5 saves unrecorded floor inspection under Skip profile");});
q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="dossier"&&s.Has("slide"),"Existing viewed evidence remains accessible in dossier");});
q.Enqueue(()=>key(Key.F6));q.Enqueue(()=>{release();check(s.Page=="inspect"&&s.data.inspecting=="slide"&&s.data.tutorialSkipped&&!s.data.firstFloorRecorded&&s.Record("slide").observations.Count==0,"F6 restores unrecorded inspection and skip state without awarding observations");shot("skip-revisited-load-1080");});
'''
s=s[:a]+q+s[z:]
s=s.replace('navigation-ui.txt','skip-revisit-ui.txt').replace('UI navigation via synthetic keys/GameView clicks; letter seeded API in separate fixture profile.','Skip revisit via synthetic keys; no evidence seeded or pages opened through API.')
Path('Tools/QuanLy/Verification/checkout-final-skip-revisit.cs.txt').write_text(s,encoding='utf-8')
