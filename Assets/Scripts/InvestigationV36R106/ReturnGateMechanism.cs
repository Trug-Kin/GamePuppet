using System;
using UnityEngine;

namespace ConRoi.Investigation.V36.R106
{
    [Serializable] public class GateState
    {
        public float angle,velocity; public bool rope,hook,bodyForce,wedge,latched=true;
    }
    // Deterministic demonstration with spring, damping, finite angular speed and contact.
    // This is an explicitly bounded game mechanism, not a forensic physics solver.
    public sealed class ReturnGateMechanism : MonoBehaviour
    {
        public Transform leaf,latch,handle,rope;
        public CharacterController player;
        public bool checkPlayerContact=true;
        public LineRenderer releaseLine;
        public GameObject wedgeVisual;
        public GateState state=new GateState();
        public bool ContactHeld {get;private set;}
        public float maxSpeed=55f,spring=2.5f,damping=4f;
        Quaternion leafRest=Quaternion.identity,latchRest=Quaternion.identity;
        void Awake(){ if(leaf)leafRest=leaf.localRotation;if(latch)latchRest=latch.localRotation;Apply(); }
        public static void Advance(GateState s,float dt,bool contact=false,float speed=55,float spring=2.5f,float damping=4)
        {
            bool released=s.rope||s.hook;
            if(released)s.latched=false;
            if(s.latched){s.angle=0;s.velocity=0;return;}
            float acceleration=(s.bodyForce?230f:0)-spring*s.angle-damping*s.velocity;
            s.velocity=Mathf.Clamp(s.velocity+acceleration*dt,-speed,speed);
            float next=Mathf.Clamp(s.angle+s.velocity*dt,0,82);
            if(s.wedge&&next<35){next=Mathf.Min(35,Mathf.Max(s.angle,0));s.velocity=0;}
            if(contact&&next<s.angle){next=s.angle;s.velocity=0;}
            if(next==0||next==82)s.velocity=0;
            s.angle=next;
            if(s.angle<.35f&&!released&&!s.bodyForce&&!s.wedge&&!contact){s.angle=0;s.latched=true;s.velocity=0;}
        }
        void Update()
        {
            ContactHeld=checkPlayerContact&&player&&state.angle>5&&Vector3.Distance(player.bounds.center,transform.TransformPoint(new Vector3(.75f,.55f,0)))<1.25f;
            float remaining=Mathf.Min(Time.deltaTime,.1f);while(remaining>0){float dt=Mathf.Min(remaining,1f/120);Advance(state,dt,ContactHeld,maxSpeed,spring,damping);remaining-=dt;}Apply();
        }
        public void Apply()
        {
            if(leaf)leaf.localRotation=leafRest*Quaternion.Euler(0,state.angle,0);
            if(latch)latch.localRotation=latchRest*Quaternion.Euler(0,0,state.latched?0:65);
            if(handle)handle.localScale=Vector3.one*(state.rope||state.hook?1.08f:1f);
            if(wedgeVisual)wedgeVisual.SetActive(state.wedge);
            if(releaseLine){releaseLine.positionCount=13;releaseLine.useWorldSpace=false;for(int i=0;i<13;i++){float t=i/12f;var p=Vector3.Lerp(new Vector3(1.54f,.92f,.03f),new Vector3(1.54f,-1.8f,-6),t);p.y-=Mathf.Sin(t*Mathf.PI)*(state.rope||state.hook?.02f:.22f);releaseLine.SetPosition(i,p);}}
        }
        public void Restore(GateState source){state=JsonUtility.FromJson<GateState>(JsonUtility.ToJson(source));Apply();}
        public void ResetGate(){state=new GateState();Apply();}
    }
}
