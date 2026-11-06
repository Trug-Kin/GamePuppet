from pathlib import Path
import json,hashlib
R=Path('D:/ConRoi');D=R/'Test/KetQua/investigation_r104'
rows=json.loads((D/'protected-start.json').read_text());paths={Path(x['path']) for x in rows}
manifest=json.loads((R/'Test/KetQua/investigation_r103/delivery-manifest.json').read_text())
for row in manifest:
 if row['path'].replace('\\','/').startswith(('Assets/','Source/')):paths.add(R/row['path'])
def sha(p):
 with p.open('rb') as f:return hashlib.file_digest(f,'sha256').hexdigest()
old={x['path']:x['sha256'] for x in rows}
for p in paths:
 if str(p) in old:assert sha(p)==old[str(p)]
(D/'protected-start.json').write_text(json.dumps([dict(path=str(p),sha256=sha(p)) for p in sorted(paths)],indent=2))
p=R/'Assets/Scripts/InvestigationV36R104/InvestigationSession.cs';s=p.read_text(encoding='utf-8')
s=s.replace('if(TrialRunning)AdvanceTrial(Time.deltaTime);','if(TrialRunning&&!trialScrubbing)AdvanceTrial(Time.deltaTime);')
s=s.replace('if(Page=="clip"&&ClipPlaying){ClipTime=Mathf.Min(MechanismPresentation.ClipLength,ClipTime+Time.deltaTime);if(ClipTime<3.8f)data.clipBeforeSeen=true;if(ClipTime>4.2f&&ClipTime<7.3f)data.clipEventSeen=true;UpdateClip();if(ClipTime>=MechanismPresentation.ClipLength)ClipPlaying=false;}', 'if(Page=="clip"&&ClipPlaying&&!clipScrubbing){float previous=ClipTime;ClipTime=Mathf.Min(MechanismPresentation.ClipLength,ClipTime+Time.deltaTime);RegisterClipExposure(previous,ClipTime);UpdateClip();if(ClipTime>=MechanismPresentation.ClipLength)ClipPlaying=false;}')
s=s.replace('public void ClosePage(){Page="";', 'public void ClosePage(){CancelReviewDrag();Page="";')
s=s.replace('public void SetPaused(bool value){Paused=value;', 'public void SetPaused(bool value){CancelReviewDrag();Paused=value;')
s=s.replace('public void StartTrial(string id)\n        {','public void StartTrial(string id)\n        {\n            CancelReviewDrag();trialRecorded=false;')
s=s.replace('public void AdvanceTrial(float dt)\n        {','public void AdvanceTrial(float dt)\n        {\n            if(dt<0)TrialTime=0;')
s=s.replace('data.trials.Add(new TrialRecord{id=TrialId,report=TrialReport,returnSeconds=TrialReturnSeconds});','if(!trialRecorded){data.trials.Add(new TrialRecord{id=TrialId,report=TrialReport,returnSeconds=TrialReturnSeconds});trialRecorded=true;}')
s=s.replace('public void ResetTrial(){TrialRunning=false;', 'public void ResetTrial(){CancelReviewDrag();TrialRunning=false;TrialId="";trialRecorded=false;')
p.write_text(s,encoding='utf-8')
p=R/'Assets/Scripts/InvestigationV36R104/InvestigationEvidence.cs';s=p.read_text(encoding='utf-8').replace('public bool clipBeforeSeen,clipEventSeen;','public bool clipBeforeSeen,clipEventSeen; public float clipBeforeExposure,clipEventExposure;');p.write_text(s,encoding='utf-8')
p=R/'Assets/Scripts/InvestigationV36R104/InvestigationSessionUI.cs';s=p.read_text(encoding='utf-8')
s=s.replace('                GUILayout.BeginArea(new Rect(22,84,218,660));','                bool focusedReview=!Paused&&(Page=="clip"||Page=="reconstruction");\n                if(focusedReview){\n                    if(GUI.Button(new Rect(850,20,150,42),"Trở lại [R/Esc]",buttonStyle))ClosePage();\n                    if(GUI.Button(new Rect(1010,20,165,42),"Hồ sơ",buttonStyle))OpenPage("dossier");\n                    GUILayout.BeginArea(new Rect(24,76,1152,696));if(Page=="clip")ClipUI();else ReconstructionUI();GUILayout.EndArea();\n                }else{\n                GUILayout.BeginArea(new Rect(22,84,218,660));')
s=s.replace('                if(diagnostics)GUI.Label(new Rect(267,766,905,28)', '                }\n                if(diagnostics)GUI.Label(new Rect(267,766,905,28)')
a=s.index('        void ReconstructionUI()');b=s.index('        public RenderTexture clipTexture;',a)
s=s[:a]+s[b:];p.write_text(s,encoding='utf-8')
print('r104 isolated observation/UI edits; protected snapshot extended to',len(paths))
