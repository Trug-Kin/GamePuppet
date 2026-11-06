using UnityEngine;
namespace ConRoi.Investigation.V36.R104
{
    [DefaultExecutionOrder(300)]
    public sealed class PresentationHands:MonoBehaviour
    {
        public InvestigationSession session;
        public InvestigationActor actor;
        public Transform handL,handR;
        public bool operatorAtHandle;
        public float maxReachError;
        void LateUpdate()
        {
            if(!session||!actor||string.IsNullOrEmpty(session.TrialId))return;
            if(operatorAtHandle){if(session.modelGate.state.rope&&session.modelGate.handle)Solve(actor.rightUpper,actor.rightFore,handR,session.modelGate.handle.position);return;}
            if(!session.modelGate.state.bodyForce)return;
            Solve(actor.leftUpper,actor.leftFore,handL,session.modelGate.leaf.TransformPoint(new Vector3(.05f,1.05f,0)));
            Solve(actor.rightUpper,actor.rightFore,handR,session.modelGate.leaf.TransformPoint(new Vector3(.15f,1.05f,0)));
        }
        void Solve(Transform upper,Transform fore,Transform hand,Vector3 target)
        {
            if(!upper||!fore||!hand)return;
            Vector3 start=upper.position;float a=Vector3.Distance(start,fore.position),b=Vector3.Distance(fore.position,hand.position);
            Vector3 direction=target-start;float distance=Mathf.Clamp(direction.magnitude,.01f,a+b-.0001f);direction.Normalize();
            Vector3 pole=Vector3.Cross(direction,Vector3.up).normalized;
            float along=(a*a-b*b+distance*distance)/(2*distance);
            Vector3 elbow=start+direction*along+pole*Mathf.Sqrt(Mathf.Max(0,a*a-along*along));
            upper.rotation=Quaternion.FromToRotation(fore.position-start,elbow-start)*upper.rotation;
            fore.rotation=Quaternion.FromToRotation(hand.position-fore.position,target-fore.position)*fore.rotation;
            maxReachError=Mathf.Max(maxReachError,Vector3.Distance(hand.position,target));
        }
    }
}
