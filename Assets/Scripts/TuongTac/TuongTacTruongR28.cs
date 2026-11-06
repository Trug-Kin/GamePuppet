// Chuyển tương tác trường cho owner trải nghiệm R28.
// Không gắn trực tiếp trong r92; checkpoint JourneyRepair_v48_r30.unity: Player_Paper.
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): school.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "R28SchoolInteraction")]
public sealed class TuongTacTruongR28:MonoBehaviour,IPlayerInteractable
{
 public TraiNghiemTruongR28 school;
 public string Prompt=>school.stagedSchoolR30?school.PaperPrompt:school.blenderSchoolR29?((school.story.data.schoolReadMask&64)!=0?"Giấy đã ở khay phải":school.CanRespond?"E · Dịch giấy sang khay phải":"Tờ giấy"):(school.story.data.schoolReadMask&64)!=0?"Gi?y ?? ? khay ph?i":"E · Dịch tờ giấy sang khay phải (tùy chọn)";
 public void Interact(){school.Respond();}
}
