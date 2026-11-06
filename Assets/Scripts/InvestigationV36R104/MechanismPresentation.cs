using UnityEngine;
namespace ConRoi.Investigation.V36.R104
{
    // All six cases and the event clip use Advance with the same spring/contact law.
    // Only the inputs and their timing differ. Replaying from zero makes seeking deterministic.
    public static class MechanismPresentation
    {
        public const float ClipLength=12f;
        public static GateState At(string id,float time)
        {
            var s=new GateState();float at=0;
            while(at<time){float dt=Mathf.Min(1f/120,time-at);Inputs(id,at,s,out bool contact);ReturnGateMechanism.Advance(s,dt,contact);at+=dt;}
            return s;
        }
        public static void Inputs(string id,float t,GateState s,out bool contact)
        {
            bool clip=id=="event";
            s.rope=clip?t>=4&&t<5.5f:(id=="release"||id=="help"||id=="wedge")?t<2.5f:id=="hold"&&t>2&&t<5;
            s.hook=id=="hook"&&t>1;
            s.bodyForce=clip?t<4.8f:id=="release"?(t<2.5f||t>8.5f):id=="wedge"?t<2.5f:id=="help"?t<5:id=="hold"?t<4.5f:id=="hook"&&t>2&&t<5;
            s.wedge=id=="wedge"&&t>2;
            contact=id=="help"&&t>=2.5f&&t<5.1f;
        }
        public static float FallAt(string id)=>id=="event"?4.8f:id=="help"?5.1f:id=="hold"?4.5f:id=="hook"?5f:float.PositiveInfinity;
        public static string StateWords(GateState s)=>
            (s.rope?"Đang giữ dây":s.hook?"Móc đang giữ dây":"Dây đã thả")+" • "+(s.latched?"chốt đã cài":"chốt đã nhả")+" • "+(s.wedge?"vật chèn đang giữ cánh":s.bodyForce?"có lực đẩy từ gác":s.velocity<-.05f?"cánh đang hồi":s.angle<.35f?"cánh đã đóng":"cánh còn mở");
    }
}
