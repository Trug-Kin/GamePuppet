using UnityEngine;

namespace ConRoi.Investigation.V36.R107
{
    public sealed partial class InvestigationSession
    {
        readonly Vector3[] tutorialPoints={new Vector3(774,6,286),new Vector3(770.1f,6,285.7f),new Vector3(770.1f,8.5f,292.7f),new Vector3(774,8.5f,293)};
        public string CurrentObjective {
            get {
                if(!data.tutorialSkipped){
                    if(!data.firstFloorRecorded)return "Vào xưởng, lên gác và kiểm tra dấu trên sàn gần lan can.";
                    if(!data.firstNotesReviewed)return "P · Xem ghi chép vừa tạo.";
                }
                if(!Has("phone"))return "Kiểm tra điện thoại trên giá quay cạnh chỗ sửa rối.";
                if(!data.clipBeforeSeen||!data.clipEventSeen)return "Xem đoạn quay sửa rối trong điện thoại.";
                if(!Has("mechanism"))return "Khám cánh, chốt và dây nhả cạnh chỗ sửa rối.";
                if(!data.visitedHa)return "Hỏi Hà tại quầy ngoài cửa xưởng.";
                if(!data.returnedToWorkshop)return "Quay lại xưởng để đối chiếu ghi chép.";
                return "Thử diễn biến tại bàn dựng; ghi nhận định và điều còn thiếu.";
            }
        }
        void BeginIntroduction(bool skip){data.introAccepted=true;data.tutorialSkipped=skip;ClosePage();}
        void UpdateOnboarding(){
            if(Modal||data.tutorialSkipped||data.firstFloorRecorded)return;
            var player=movement.transform.position;
            if(player.x>768&&player.x<780&&player.z>285&&player.z<297)data.tutorialWaypoint=Mathf.Max(data.tutorialWaypoint,1);
            if(player.x<771.5f&&player.x>768&&player.z>286&&player.z<295)data.tutorialWaypoint=Mathf.Max(data.tutorialWaypoint,2);
            if(player.y>8&&player.z>291&&player.x>769&&player.x<779)data.tutorialWaypoint=Mathf.Max(data.tutorialWaypoint,3);
            int i=Mathf.Clamp(data.tutorialWaypoint,0,tutorialPoints.Length);
            if(i<tutorialPoints.Length&&Vector2.Distance(new Vector2(movement.transform.position.x,movement.transform.position.z),new Vector2(tutorialPoints[i].x,tutorialPoints[i].z))<1.1f)data.tutorialWaypoint=i+1;
        }
        void DrawObjectiveMarker(){
            if(data.tutorialSkipped||data.firstFloorRecorded||data.tutorialWaypoint>=tutorialPoints.Length)return;
            var p=eye.WorldToViewportPoint(tutorialPoints[Mathf.Clamp(data.tutorialWaypoint,0,tutorialPoints.Length-1)]+Vector3.up*.5f);
            if(p.z<=0||p.x<.02f||p.x>.98f||p.y<.05f||p.y>.90f)return;
            var at=new Rect(p.x*1200-65,(1-p.y)*800-18,130,38);
            Fill(at,new Color(.06f,.12f,.13f,.85f));GUI.Label(at,data.tutorialWaypoint==0?"  Lối vào xưởng":"  Lối lên gác",smallStyle);
        }
        void RecordVisibleObservations(string id){
            var e=InvestigationContent.Get(id);if(e==null||!Has(id))return;
            if(id=="clip"&&(!data.clipBeforeSeen||!data.clipEventSeen)){Message="Hãy xem cả trước và trong sự cố trước khi ghi quan sát clip.";return;}
            bool wasRecorded=data.firstFloorRecorded;
            for(int i=0;i<e.observations.Length;i++)Observe(id,i);
            if(!wasRecorded&&data.firstFloorRecorded){if(!data.tutorialSkipped)Message="Đã ghi quan sát. P · Xem ghi chép";}
            else Message="Đã ghi quan sát vào hồ sơ.";
        }
    }
}
