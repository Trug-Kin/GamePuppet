from pathlib import Path
R=Path('Assets/Scripts/InvestigationV36R106')
p=R/'InvestigationSession.cs';s=p.read_text(encoding='utf-8')
s=s.replace('public float TrialTime {get;private set;}', '''public float TrialTime {get;private set;}
        public float TrialDuration {get;private set;}=16f;
        public float TrialClearAt {get;private set;}=float.PositiveInfinity;
        public bool TrialSettled=>!string.IsNullOrEmpty(TrialId)&&TrialTime>=TrialDuration-.001f&&TrialTimeline.Settled(modelGate.state);
        public bool CanClearTrialHolds=>!string.IsNullOrEmpty(TrialId)&&float.IsPositiveInfinity(TrialClearAt)&&TrialTime>=TrialTimeline.LastAction(TrialId)&&(modelGate.state.bodyForce||modelGate.state.hook||modelGate.state.wedge);
        public void ClearTrialHolds(){if(!CanClearTrialHolds)return;CancelReviewDrag();TrialClearAt=TrialTime;TrialDuration=TrialTimeline.End(TrialId,TrialClearAt);trialRecorded=false;TrialRunning=true;TrialReport="Đã bỏ điều kiện giữ. Tiếp tục quan sát cánh hồi và chốt.";}''')
s=s.replace('TrialId=id;TrialTime=0;releaseAt=', 'TrialId=id;TrialTime=0;TrialClearAt=float.PositiveInfinity;TrialDuration=TrialTimeline.End(id,TrialClearAt);releaseAt=')
s=s.replace('TrialTime+=dt;var s=modelGate.state;', 'TrialTime=Mathf.Min(TrialDuration,TrialTime+dt);var s=modelGate.state;')
s=s.replace('modelGate.state=MechanismPresentation.At(TrialId,t)', 'modelGate.state=TrialTimeline.At(TrialId,t,TrialClearAt)')
s=s.replace('if(t<10)return;', '''if(t<TrialDuration)return;
            if(!TrialTimeline.Settled(s)){TrialRunning=false;TrialReport="Mô hình chưa ổn định; chưa ghi kết quả hoàn tất.";return;}''')
start=s.index('            string detail=TrialId=="release"?')
end=s.index('\n        public void UpdateClip()',start)
# Preserve ResetTrial below the result block, replacing only its reset fields.
reset=s[s.index('        public void ResetTrial()',start):end]
reset=reset.replace('TrialTime=0;TrialReport=', 'TrialTime=0;TrialClearAt=float.PositiveInfinity;TrialDuration=16f;TrialReport=')
s=s[:start]+'''            string detail=s.bodyForce?"Cánh đang được người trên gác giữ mở. Có thể ngừng đè giữ để quan sát hồi.":s.wedge?"Vật chèn còn giữ cánh. Có thể rút chèn để quan sát hồi.":s.hook?"Móc còn giữ dây; cánh về đóng nhưng chốt chưa cài. Có thể nhả móc.":"Cánh đã hồi đóng và chốt đã cài.";
            TrialReport=observed+detail+" Mô hình không xác định người thao tác hay ý định.";
            if(!trialRecorded){data.trials.Add(new TrialRecord{id=TrialId+(float.IsPositiveInfinity(TrialClearAt)?"":"/clear-holds"),report=TrialReport,returnSeconds=TrialReturnSeconds});trialRecorded=true;}
        }
'''+reset+s[end:]
p.write_text(s,encoding='utf-8')
p=R/'InvestigationReviewUI.cs';s=p.read_text(encoding='utf-8')
s=s.replace('AdvanceTrial(Mathf.Clamp(time,0,10))','AdvanceTrial(Mathf.Clamp(time,0,TrialDuration))')
s=s.replace('if(TrialTime>=10)SeekTrial(0)', 'if(TrialTime>=TrialDuration)SeekTrial(0)')
s=s.replace('ReviewSlider(TrialSliderRect,TrialTime,10,false)', 'ReviewSlider(TrialSliderRect,TrialTime,TrialDuration,false)')
s=s.replace('TrialTime.ToString("F1")+" / 10 s"', 'TrialTime.ToString("F1")+" / "+TrialDuration.ToString("F1")+" s"')
s=s.replace('            ReviewImage(new Rect(254,36,898,480),stageTexture);', '''            GUI.enabled=CanClearTrialHolds;
            string clearLabel=modelGate.state.hook?"Nhả dây khỏi móc":modelGate.state.wedge?"Rút vật chèn":"Ngừng đè giữ cánh";
            if(ReviewButton(new Rect(0,524,240,54),clearLabel))ClearTrialHolds();
            GUI.enabled=true;
            ReviewImage(new Rect(254,36,898,480),stageTexture);''')
p.write_text(s,encoding='utf-8')
print('Patched timeline completion/seek and explicit removal control; mechanism law and input schedules untouched.')
