// Đích tương tác của hành trình legacy; chuyển thao tác cho owner hiện có.
// Không gắn trực tiếp trong r92; checkpoint ConRoi.unity: "C\xF4 Mai".
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): journey.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "JourneyInteractable")]
public sealed class DoiTuongHanhTrinh : MonoBehaviour, IPlayerInteractable
{
    public enum Kind { Bed, Phone, Wake, Dialogue }
    public HanhTrinhConRoi journey;
    public Kind kind;
    [TextArea] public string dialogue;
    public void Interact()
    {
        if(!journey)return;
        if(kind==Kind.Bed)journey.Sleep();
        else if(kind==Kind.Phone)journey.OpenPhone();
        else if(kind==Kind.Wake)journey.Wake();
        else journey.Speak(dialogue);
    }
}
