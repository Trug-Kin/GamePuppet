// Điều phối diễn xuất trường/lớp 203; partial R30 dùng chung owner staging và trạng thái lưu.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): approachR89.
// Code tự tìm/tạo: TuongTacGiayLop203R32.
using System;
using UnityEngine;

// Scene opt-in: performance timing only, no geometry or controller changes.
public sealed partial class TraiNghiemTruongR28
{
 public bool stagedSchoolR30;
 // Scene opt-in restoration; r30/r31/r32 checkpoints retain their recorded behavior.
 public bool restoreChairTraceR33;
 public bool paperOwnsResponseR34;
 public TiepCanCoChuDichR89 approachR89;
 public void ReapplyPoseR89(){Pose();}
 [Serializable] sealed class StagingR30 {
  public int version=1;
  public float initial,lead=-1,seen,response=1;
  public bool cue,contact,result,finished,initialSeen;
 }
 StagingR30 r30=new StagingR30();
 public float InitialSeconds=>stagedSchoolR30?r30.initial:0;
 public bool InitialSeen=>stagedSchoolR30&&r30.initialSeen;
 public float ResponseProgress=>responseBlend;
 public bool ActionFinished=>stagedSchoolR30&&r30.finished;
 public string PaperPrompt=>((story.data.schoolReadMask&64)!=0)?(responseBlend<1?"Tờ giấy đang dịch chuyển…":"Bài của Minh · đã ở khay phải"):CanRespond?"E · Dịch bài của Minh sang khay phải (tùy chọn)":"Bài của Minh · còn trên bàn";
 void RestoreR30(bool enabled){
  active=enabled;loads=story.SuccessfulLoads;
  if(!enabled){elapsed=-1;return;}
  try{r30=string.IsNullOrEmpty(story.data.schoolStagingR30)?new StagingR30():JsonUtility.FromJson<StagingR30>(story.data.schoolStagingR30);}catch(ArgumentException){r30=new StagingR30();}
  if(r30==null||r30.version!=1||!Finite(r30.lead)||!Finite(r30.seen)||!Finite(r30.initial)||!Finite(r30.response))r30=new StagingR30();
  if(string.IsNullOrEmpty(story.data.schoolStagingR30)&&story.data.schoolLeading){
   // r29 stored timing, but no observation duration. Preserve its paper phase;
   // never invent the missing observation or replay already passed sound cues.
   float old=Mathf.Max(0,story.data.schoolLeadElapsed);
   r30.lead=old<.65f?old/.65f*.60f+.30f:.90f+(old-.65f)/.70f*.90f;
   r30.cue=true;r30.contact=old>=.65f;r30.result=old>=1.35f;
  }
  r30.initial=Mathf.Clamp(r30.initial,0,1.4f);
  responded=(story.data.schoolReadMask&64)!=0;
  responseBlend=responded?Mathf.Clamp01(r30.response):1;
  elapsed=Mathf.Clamp(r30.lead,-1,120);ObservedSeconds=Mathf.Clamp(r30.seen,0,1.8f);
  Pose();
 }
 static bool Finite(float v)=>!float.IsNaN(v)&&!float.IsInfinity(v);
 void CaptureR30(){
  if(story.data.region!=2)return;
  r30.lead=elapsed;r30.seen=ObservedSeconds;r30.response=responseBlend;
  story.data.schoolStagingR30=JsonUtility.ToJson(r30);
  story.data.schoolLeading=Leading;story.data.schoolLeadElapsed=Leading?Mathf.Min(elapsed,8):0;
 }
 bool ReadableR30(){
  var h=eye.WorldToViewportPoint(story.schoolChapter.puppet.dauTransform.position);
  var p=eye.WorldToViewportPoint(puppetPaper.position);
  return Vector3.Distance(eye.transform.position,puppetPaper.position)<4.8f&&h.y-p.y>.13f&&Visible(puppetPaper)&&Visible(story.schoolChapter.puppet.dauTransform)&&Visible(leadHand)&&(paperOwnsResponseR34||Visible(responsePaper));
 }
 void UpdateR30(){
  if(!initialized)Init();if(story.data.region!=2){if(active){active=false;elapsed=-1;}return;}
  if(!active||loads!=story.SuccessfulLoads)RestoreR30(true);
  if(story.Paused||story.Loading||story.MenuOpen||story.PhoneOpen||story.SupplementInspection||story.polish.IsReading)return;
  if(story.schoolChapter.clockHand)story.schoolChapter.clockHand.Rotate(0,0,-Time.deltaTime*6,Space.Self);
  bool moving=story.player.LastMovementInput.sqrMagnitude>.01f;
  bool readable=ReadableR30();
  if(!r30.cue&&Vector3.Distance(story.player.transform.position,responseDesk.position)<5){r30.cue=true;story.PlayEffectAt(story.wood,puppetDesk.position);}
  if(approachR89&&approachR89.OwnsMotion&&!approachR89.AtDesk){UpdateChairR89();Pose();CaptureR30();return;}
  if(!Leading&&!r30.finished&&(!responded||GetComponent<TuongTacGiayLop203R32>())&&(!approachR89||!approachR89.Skipped)){
   r30.initial=readable&&!moving?r30.initial+Time.deltaTime:0;
   if(r30.initial>=1.4f||(approachR89&&approachR89.AtDesk)){r30.initialSeen=readable;elapsed=0;ObservedSeconds=0;r30.contact=r30.result=false;Cycles++;}
  }else if(Leading){
   elapsed=Mathf.Min(120,elapsed+Time.deltaTime);
   if(elapsed<=1.8f&&readable)ObservedSeconds+=Time.deltaTime;
   if(!r30.contact&&elapsed>=.90f){r30.contact=true;story.PlayEffectAt(story.wood,puppetPaper.position);}
   if(!r30.result&&elapsed>=1.8f){r30.result=true;story.PlayEffectAt(story.wood,puppetPaper.position);}
   if(paperOwnsResponseR34&&elapsed>=1.8f)r30.finished=true;
   if(moving&&(!paperOwnsResponseR34||elapsed>=1.8f)){
    r30.finished=elapsed>=1.8f;
    elapsed=-1;r30.initial=0;
    if(!r30.finished){ObservedSeconds=0;r30.initialSeen=false;}
   }
  }
  if(responded)responseBlend=Mathf.Min(1,responseBlend+Time.deltaTime/.8f);
  // Original R28 chair event. Independent of Mai, papers, notebook and puppet staging.
  if(restoreChairTraceR33&&!story.data.schoolTrace&&Vector3.Distance(story.player.transform.position,traceChair.position)<2.4f){story.data.schoolTrace=true;story.data.awareness++;story.PlayEffect(story.wood);}
  traceChair.rotation=story.data.schoolTrace?chairBoard:chairWindow;
  Pose();CaptureR30();
 }
 void RespondR30(){
  if(story.data.region!=2||story.Paused||story.Loading||story.MenuOpen||story.PhoneOpen||!CanRespond||(story.data.schoolReadMask&64)!=0)return;
  // E never starts or advances the puppet event.
  Responses++;responded=true;responseBlend=0;story.data.schoolReadMask|=64;
  if((Leading&&elapsed>=1.8f||r30.finished)&&r30.initialSeen&&ObservedSeconds>=1.1f){story.data.schoolWitnessed=true;LastResponseDelay=Leading?elapsed:1.8f;}
  story.PlayEffectAt(story.wood,responsePaper.position);CaptureR30();
 }
 void UpdateChairR89(){
  // Approaching the puppet must never gate the independent, existing chair trace.
  if(restoreChairTraceR33&&!story.data.schoolTrace&&Vector3.Distance(story.player.transform.position,traceChair.position)<2.4f){story.data.schoolTrace=true;story.data.awareness++;story.PlayEffect(story.wood);}
  traceChair.rotation=story.data.schoolTrace?chairBoard:chairWindow;
 }
 // New paper placement never starts the event. Early choices remain valid but
 // permanently exclude this attempt from credit for a puppet-first response.
 public void RegisterPaperChoiceR32(bool early){
  if(!GetComponent<TuongTacGiayLop203R32>()||story.data.region!=2)return;
  Responses++;
  if(!early&&(r30.finished||Leading&&elapsed>=1.8f)&&r30.initialSeen&&ObservedSeconds>=1.1f){
   story.data.schoolReadMask|=64;story.data.schoolWitnessed=true;LastResponseDelay=Leading?elapsed:1.8f;
  }
  CaptureR30();
 }
}
