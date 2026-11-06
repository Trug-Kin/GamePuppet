using UnityEngine;
namespace ConRoi.Investigation.V36.R104
{
    [DefaultExecutionOrder(200)]
    public sealed class RepairHandBinding : MonoBehaviour
    {
        public Transform rightHand,repairAttachment; public InvestigationSession session;
        public LineRenderer repairString;
        void LateUpdate(){if(!rightHand||!repairAttachment||!repairString)return;repairString.useWorldSpace=true;repairString.positionCount=3;Vector3 start=(!session||session.ClipTime<4.8f)?rightHand.position:repairAttachment.position+Vector3.down*.25f;repairString.SetPosition(0,start);repairString.SetPosition(1,(start+repairAttachment.position)*.5f+Vector3.down*.025f);repairString.SetPosition(2,repairAttachment.position);}
    }
}
