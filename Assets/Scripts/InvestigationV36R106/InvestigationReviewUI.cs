using UnityEngine;
namespace ConRoi.Investigation.V36.R106
{
    public sealed partial class InvestigationSession
    {
        bool clipScrubbing,trialScrubbing,trialRecorded;
        int reviewHotControl;
        public bool ClipScrubbing=>clipScrubbing;
        public bool TrialScrubbing=>trialScrubbing;
        public Rect ClipSliderRect {get;private set;}
        public Rect TrialSliderRect {get;private set;}
        // Visible playback exposure only. Seeking never unlocks unseen observations.
        void RegisterClipExposure(float a,float b)
        {
            float overlap(float lo,float hi)=>Mathf.Max(0,Mathf.Min(b,hi)-Mathf.Max(a,lo));
            data.clipBeforeExposure+=overlap(.5f,3.8f);data.clipEventExposure+=overlap(4,6.2f);
            if(data.clipBeforeExposure>=.6f)data.clipBeforeSeen=true;
            if(data.clipEventExposure>=.8f)data.clipEventSeen=true;
        }
        void CancelReviewDrag(){clipScrubbing=false;trialScrubbing=false;if(reviewHotControl!=0&&GUIUtility.hotControl==reviewHotControl)GUIUtility.hotControl=0;reviewHotControl=0;}
        float ReviewSlider(Rect rect,float value,float end,bool clip)
        {
            int id=GUIUtility.GetControlID((clip?"clip":"trial").GetHashCode(),FocusType.Passive,rect);
            var e=Event.current;bool dragging=clip?clipScrubbing:trialScrubbing;
            if(e.type==EventType.MouseDown&&e.button==0&&rect.Contains(e.mousePosition)){
                GUIUtility.hotControl=id;reviewHotControl=id;dragging=true;if(clip){clipScrubbing=true;ClipPlaying=false;}else{trialScrubbing=true;TrialRunning=false;}e.Use();
            }
            if(dragging&&GUIUtility.hotControl==id){
                if(e.type==EventType.MouseDrag||e.type==EventType.MouseDown||e.type==EventType.Used)value=Mathf.Clamp01((e.mousePosition.x-rect.x-9)/(rect.width-18))*end;
                if(e.type==EventType.MouseDrag)e.Use();
                if(e.type==EventType.MouseUp){value=Mathf.Clamp01((e.mousePosition.x-rect.x-9)/(rect.width-18))*end;GUIUtility.hotControl=0;reviewHotControl=0;if(clip)clipScrubbing=false;else trialScrubbing=false;e.Use();}
            }
            // Draw only: the pointer lifecycle above owns seeking including release outside.
            if(e.type==EventType.Repaint){GUI.skin.horizontalSlider.Draw(new Rect(rect.x,rect.center.y-3,rect.width,6),false,false,false,false);GUI.skin.horizontalSliderThumb.Draw(new Rect(rect.x+9+(rect.width-18)*value/end-7,rect.center.y-11,14,22),false,dragging,false,false);}
            return value;
        }
        void ReviewImage(Rect box,Texture texture){Fill(box,new Color(.025f,.035f,.04f));if(texture)GUI.DrawTexture(box,texture,ScaleMode.ScaleToFit,false);}
        bool ReviewButton(Rect rect,string label)=>GUI.Button(rect,label,buttonStyle);
        void ClipUI()
        {
            GUI.Label(new Rect(0,0,1152,32),"Điện thoại • buổi sửa R-12 • bản dựng thử, không âm thanh",bodyStyle);
            ReviewImage(new Rect(0,36,1152,500),clipTexture);
            ClipSliderRect=new Rect(12,542,1000,32);float next=ReviewSlider(ClipSliderRect,ClipTime,MechanismPresentation.ClipLength,true);
            if(Mathf.Abs(next-ClipTime)>.0001f){ClipTime=next;UpdateClip();}
            GUI.Label(new Rect(1025,544,120,30),ClipTime.ToString("F1")+" / 12 s",smallStyle);
            if(ReviewButton(new Rect(0,584,160,46),ClipPlaying?"Tạm dừng":"Phát tiếp"))ClipPlaying=!ClipPlaying;
            if(ReviewButton(new Rect(170,584,190,46),"Phát lại từ đầu")){CancelReviewDrag();ClipTime=0;ClipPlaying=true;UpdateClip();}
            if(ReviewButton(new Rect(370,584,150,46),"Lùi 1 giây")){ClipTime=Mathf.Max(0,ClipTime-1);ClipPlaying=false;UpdateClip();}
            if(ReviewButton(new Rect(530,584,150,46),"Tiến 1 giây")){ClipTime=Mathf.Min(12,ClipTime+1);ClipPlaying=false;UpdateClip();}
            if(ReviewButton(new Rect(690,584,260,46),"Ghi điều đã quan sát"))OpenEvidence("clip");
            GUI.Label(new Rect(0,642,1152,40),"Kéo thanh để xem lại • tua không tự ghi quan sát • F5 lưu / F6 tải hồ sơ riêng",smallStyle);
        }
        void SetStageView(int view)
        {
            stageView=view;var p=stageCamera.transform.parent;
            Vector3 eye=view==0?new Vector3(4.2f,6.2f,-5.4f):view==1?new Vector3(2.7f,4.7f,-1f):new Vector3(2.6f,1.8f,-2.8f);
            Vector3 target=view==0?new Vector3(0,1.8f,-.8f):view==1?new Vector3(.8f,4.1f,1.55f):new Vector3(1.4f,1.1f,-4.4f);
            stageCamera.transform.localPosition=eye;stageCamera.transform.LookAt(p.TransformPoint(target));stageCamera.fieldOfView=view==0?80:40;
        }
        int stageView;
        void SeekTrial(float time){bool recorded=trialRecorded;trialRecorded=true;TrialTime=0;AdvanceTrial(Mathf.Clamp(time,0,TrialDuration));TrialRunning=false;trialRecorded=recorded;}
        void ReconstructionUI()
        {
            GUI.Label(new Rect(0,0,1152,32),"Bàn dựng lại • vai giả thuyết, chưa xác định người kéo • bản dựng thử",bodyStyle);
            for(int i=0;i<trialIds.Length;i++)if(ReviewButton(new Rect(0,42+i*69,240,61),trialNames[i]))StartTrial(trialIds[i]);
            if(ReviewButton(new Rect(0,464,240,48),"Đặt lại mô hình"))ResetTrial();
            GUI.enabled=CanClearTrialHolds;
            string clearLabel=modelGate.state.hook?"Nhả dây khỏi móc":modelGate.state.wedge?"Rút vật chèn":"Ngừng đè giữ cánh";
            if(ReviewButton(new Rect(0,524,240,54),clearLabel))ClearTrialHolds();
            GUI.enabled=true;
            ReviewImage(new Rect(254,36,898,480),stageTexture);
            if(ReviewButton(new Rect(254,524,190,42),"Toàn bố cục"))SetStageView(0);
            if(ReviewButton(new Rect(453,524,220,42),"Cận cánh / chốt"))SetStageView(1);
            if(ReviewButton(new Rect(682,524,220,42),"Cận tay / dây"))SetStageView(2);
            GUI.enabled=!string.IsNullOrEmpty(TrialId);if(ReviewButton(new Rect(912,524,240,42),TrialRunning?"Tạm dừng mô hình":"Tiếp tục mô hình")){if(TrialTime>=TrialDuration)SeekTrial(0);TrialRunning=!TrialRunning;}GUI.enabled=true;
            TrialSliderRect=new Rect(254,573,770,30);float next=ReviewSlider(TrialSliderRect,TrialTime,TrialDuration,false);if(!string.IsNullOrEmpty(TrialId)&&Mathf.Abs(next-TrialTime)>.0001f)SeekTrial(next);
            GUI.Label(new Rect(1035,575,117,30),TrialTime.ToString("F1")+" / "+TrialDuration.ToString("F1")+" s",smallStyle);
            GUI.Label(new Rect(254,611,898,42),MechanismPresentation.StateWords(modelGate.state),bodyStyle);
            GUI.Label(new Rect(0,656,1152,39),TrialRunning?"Theo dõi người, dây và cánh; đổi góc giữ nguyên thời điểm.":TrialReport,smallStyle);
        }
    }
}
