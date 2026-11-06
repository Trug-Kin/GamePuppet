from pathlib import Path
p=Path('D:/ConRoi/Assets/Scripts/InvestigationV36R103/InvestigationSession.cs');s=p.read_text(encoding='utf-8');a=s.index('            if(modelKhai){');b=s.index('\n            if(modelTu)',a)
s=s[:a]+'''            if(modelKhai){var near=new Vector3(1.54f,0,-4.5f);var far=new Vector3(-3.9f,0,-3.5f);modelKhai.localPosition=TrialId=="after"?Vector3.Lerp(far,near,Mathf.Clamp01((t-5)/4)):Vector3.Lerp(near,far,Mathf.Clamp01((t-(TrialId=="release"||TrialId=="help"?2.5f:TrialId=="hook"?1.2f:5))/4));}'''+s[b:]
s=s.replace('if(fall){p=FallThroughOpening(t-fallAt);p.x=.35f;}modelTu.localPosition=p;', 'if(fall){p=FallThroughOpening(t-fallAt);p.x=.35f;}if(TrialId=="after")p=new Vector3(.8f,.2f,1.05f);modelTu.localPosition=p;')
s=s.replace('fall?Mathf.Min(85,(t-fallAt)*100):0','TrialId=="after"?85:fall?Mathf.Min(85,(t-fallAt)*100):0');p.write_text(s,encoding='utf-8')
