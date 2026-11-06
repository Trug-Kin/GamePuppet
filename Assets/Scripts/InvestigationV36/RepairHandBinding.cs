using UnityEngine;
namespace ConRoi.Investigation.V36
{
    [DefaultExecutionOrder(200)]
    public sealed class RepairHandBinding : MonoBehaviour
    {
        public Transform rightHand,repairAttachment;
        public LineRenderer repairString;
        void LateUpdate(){if(!rightHand||!repairAttachment||!repairString)return;repairString.useWorldSpace=true;repairString.positionCount=3;repairString.SetPosition(0,rightHand.position);repairString.SetPosition(1,(rightHand.position+repairAttachment.position)*.5f+Vector3.down*.025f);repairString.SetPosition(2,repairAttachment.position);}
    }
}
