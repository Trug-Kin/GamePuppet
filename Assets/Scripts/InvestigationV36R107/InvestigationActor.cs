using UnityEngine;
namespace ConRoi.Investigation.V36.R107
{
    // Sole pose owner. No Animator, legacy Animation or dream behaviour on this rig.
    public sealed class InvestigationActor : MonoBehaviour
    {
        public Transform head,leftUpper,rightUpper,leftFore,rightFore;
        public bool puppet; public int pose=-1; public bool repairHands;
        public AnimationClip[] poseClips;
        public AnimationClip repairClip;
        public Transform nativeRoot; public AnimationClip nativeIdle; public float greetingUntil;
        public void Greet(){greetingUntil=Time.time+1.2f;}
        Quaternion h,l,r,lf,rf;
        void Awake(){h=Rest(head);l=Rest(leftUpper);r=Rest(rightUpper);lf=Rest(leftFore);rf=Rest(rightFore);}
        static Quaternion Rest(Transform t)=>t?t.localRotation:Quaternion.identity;
        void LateUpdate()
        {
            var target=nativeRoot?nativeRoot.gameObject:gameObject;
            if(repairHands&&repairClip){repairClip.SampleAnimation(target,0);return;}
            if(pose>=0&&poseClips!=null&&pose<poseClips.Length&&poseClips[pose]){poseClips[pose].SampleAnimation(target,0);return;}
            if(nativeIdle){nativeIdle.SampleAnimation(target,Time.time%nativeIdle.length);if(Time.time<greetingUntil&&rightFore)rightFore.localRotation*=Quaternion.Euler(-20,0,0);return;}
            float t=Time.time;
            if(head)head.localRotation=h*Quaternion.Euler(0,puppet?0:Mathf.Sin(t*.6f)*3,0);
            float a=repairHands?-65:(pose>=0?(-80+(pose%5)*40):Mathf.Sin(t)*2);
            float b=repairHands?65:(pose>=0?(-70+(pose/5)*55):-Mathf.Sin(t)*2);
            if(leftUpper)leftUpper.localRotation=l*Quaternion.Euler(a,0,pose>=0?-18:0);
            if(rightUpper)rightUpper.localRotation=r*Quaternion.Euler(b,0,pose>=0?18:0);
            if(leftFore)leftFore.localRotation=lf*Quaternion.Euler(repairHands?-50:(pose>=0&&(pose%2==0)?-60:0),0,0);
            if(rightFore)rightFore.localRotation=rf*Quaternion.Euler(repairHands?50:(pose>=0&&(pose%3==0)?60:0),0,0);
        }
    }
}
