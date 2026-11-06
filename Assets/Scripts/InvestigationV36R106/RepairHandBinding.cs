using UnityEngine;
namespace ConRoi.Investigation.V36.R106
{
    [DefaultExecutionOrder(200)]
    public sealed class RepairHandBinding : MonoBehaviour
    {
        public Transform rightHand,repairAttachment; public InvestigationSession session;
        public LineRenderer repairString;
        public Transform clipSpace;
        public Vector3 releasePoint = new Vector3(.28f, 4.15f, 1.35f);
        // The source animation releases at 4.8s. Evaluate from time so reverse seeking
        // and load never depend on whether a transition frame happened to run.
        void LateUpdate()
        {
            if(!rightHand||!repairAttachment||!repairString)return;
            float time=session?session.ClipTime:0;
            Vector3 start=rightHand.position;
            if(time>4.8f&&clipSpace){
                float u=Mathf.SmoothStep(0,1,Mathf.Clamp01((time-4.8f)/.35f));
                start=Vector3.Lerp(clipSpace.TransformPoint(releasePoint),repairAttachment.position+Vector3.down*.25f,u);
            }
            repairString.useWorldSpace=true;repairString.positionCount=3;
            repairString.SetPosition(0,start);
            repairString.SetPosition(1,(start+repairAttachment.position)*.5f+Vector3.down*.02f);
            repairString.SetPosition(2,repairAttachment.position);
        }
    }
}
