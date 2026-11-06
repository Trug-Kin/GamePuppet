using UnityEngine;

namespace ConRoi.Investigation.V36.R106
{
    // Same six input schedules and same gate law as r105. A user action may
    // explicitly remove remaining holds; that action is part of the seekable timeline.
    public static class TrialTimeline
    {
        public static float LastAction(string id) => id=="release"?8.5f:id=="help"?5.1f:id=="hold"||id=="hook"?5f:id=="wedge"?2.5f:9f;
        public static void Inputs(string id,float time,float clearAt,GateState state,out bool contact)
        {
            MechanismPresentation.Inputs(id,time,state,out contact);
            if(time>=clearAt){state.rope=false;state.hook=false;state.bodyForce=false;state.wedge=false;contact=id=="release"&&time<clearAt+.6f;}
        }
        public static GateState At(string id,float time,float clearAt)
        {
            var state=new GateState();float at=0;
            while(at<time){float dt=Mathf.Min(1f/120,time-at);Inputs(id,at,clearAt,state,out bool contact);ReturnGateMechanism.Advance(state,dt,contact);at+=dt;}
            return state;
        }
        public static bool Settled(GateState state)
        {
            if(state.bodyForce)return state.angle>=81.99f&&Mathf.Abs(state.velocity)<.05f;
            if(state.wedge)return Mathf.Abs(state.angle-35)<.01f&&Mathf.Abs(state.velocity)<.05f;
            // The existing law approaches closed asymptotically while the latch
            // is held released. Use its existing .35 degree latch window, not a snap.
            if(state.rope||state.hook)return state.angle<.35f&&Mathf.Abs(state.velocity)<.05f;
            return state.latched&&state.angle==0;
        }
        public static float End(string id,float clearAt)
        {
            var state=new GateState();float at=0;
            float last=float.IsPositiveInfinity(clearAt)?LastAction(id):Mathf.Max(LastAction(id),clearAt+(id=="release"?.6f:0));
            while(at<60){Inputs(id,at,clearAt,state,out bool contact);ReturnGateMechanism.Advance(state,1f/120,contact);at+=1f/120;if(at>=last+.05f&&Settled(state))return at+.025f;}
            // A guard prevents unbounded computation; it is never a success result.
            return 60;
        }
    }
}
