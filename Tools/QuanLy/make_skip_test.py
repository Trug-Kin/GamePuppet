from pathlib import Path
s=Path('Tools/QuanLy/Verification/checkout-final-nav.cs.txt').read_text(encoding='utf-8-sig')
old='if(!s.SavePath.Contains("NavFixture_a789222")||!s.Has("letter"))throw new Exception("Isolated navigation fixture required");'
new='if(s.Page!="intro"||s.InspectionCount!=0||s.data.mapping.Count!=0||s.data.finalized)throw new Exception("Fresh intro; no automatic technical save load required");var profile=Path.GetFullPath("Test/PlaytestProfiles/Skip_a789222_20261107");if(Directory.Exists(profile))throw new Exception("Profile already used");s.SaveDirectoryOverride=profile;'
assert old in s;s=s.replace(old,new)
s=s.replace('w.Focus();var settings=', 'w.minSize=w.maxSize=new Vector2(1920,1101.2f);w.position=new Rect(0,0,1920,1101.2f);w.Focus();var settings=')
a=s.index('q.Enqueue(');z=s.index('double next=',a)
q='''
q.Enqueue(()=>{check(s.Page=="intro"&&s.InspectionCount==0&&s.data.mapping.Count==0&&!s.data.finalized,"Fresh Play does not auto load previous technical save");check(Screen.width==1920&&Screen.height==1080,"Skip UI at 1080p");shot("skip-intro-1080");});
q.Enqueue(()=>click(600,431));q.Enqueue(()=>{check(s.Page==""&&s.data.tutorialSkipped&&s.data.introAccepted&&s.InspectionCount==0,"Skip button keeps case empty and grants no observations");shot("skip-world-1080");});
q.Enqueue(()=>key(Key.F5));q.Enqueue(()=>{release();check(File.Exists(s.SavePath),"F5 creates only new Skip profile");});q.Enqueue(()=>key(Key.P));q.Enqueue(()=>{release();check(s.Page=="dossier"&&s.InspectionCount==0,"P works with empty skipped tutorial");});q.Enqueue(()=>key(Key.F6));q.Enqueue(()=>{release();check(s.Page==""&&s.data.tutorialSkipped&&s.InspectionCount==0,"F6 preserves skipped tutorial before first evidence");});
'''
s=s[:a]+q+s[z:]
s=s.replace('navigation-ui.txt','skip-ui.txt')
Path('Tools/QuanLy/Verification/checkout-final-skip.cs.txt').write_text(s,encoding='utf-8')
