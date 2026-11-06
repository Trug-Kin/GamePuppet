#if UNITY_EDITOR || CON_ROI_BUILD_REVIEW
using System.Collections;
using System.IO;
using Newtonsoft.Json.Linq;
using UnityEngine;

public sealed partial class C3ActualPlayRun
{
 IEnumerator RunHitchesV45()
 {
  yield return PrepareJourneyCheck("hitches");LoadJourneySeed("school_entry");actingPlacements++;
  var origin=school.spawn.position-new Vector3(0,.94f,-7);
  for(int loop=0;loop<3;loop++)
  {
   foreach(var p in new[]{new Vector3(-10,.94f,-5),new Vector3(-22,.94f,-5),new Vector3(-22,.94f,0),new Vector3(-22,.94f,10),new Vector3(-22,.94f,0),new Vector3(-22,.94f,-5),new Vector3(-10,.94f,-5),new Vector3(0,.94f,-5),new Vector3(0,.94f,1),new Vector3(1.2f,.94f,9)})
   {
    phase="repeat_"+loop+"_"+p.x+"_"+p.z;walkingStory.polish.Event("v45_hitch_repeat",phase);yield return SchoolGo(origin+p,phase);WalkCheck(phase,regionalReached);
   }
   yield return SchoolUse(false);yield return new WaitForSeconds(1);yield return SchoolCheckpoint("repeat_Mai_load_"+loop,2,2);
   foreach(var p in new[]{new Vector3(0,.94f,5),new Vector3(0,.94f,1),new Vector3(0,.94f,-5)})yield return SchoolGo(origin+p,"repeat_exit_"+loop);
  }
  File.WriteAllText(Path.Combine(VerificationOutput,"inspection-placements.json"),new JObject{["count"]=actingPlacements,["description"]="One school-entry save fixture, then 3 real keyboard traversals of the same archive/courtyard/classroom and Mai interaction/save/load."}.ToString());
  yield return FinishWalk();
 }
}

#endif
