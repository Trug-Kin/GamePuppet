// Đích tương tác hồ/chợ; kiểm vùng và trạng thái trước khi chuyển thao tác cho story.
// Component trong scene r92: CR_Ha_Interaction, CR_Bao_Interaction, CR_Lake_Wake_Interaction, CR_Phone_Interaction, CR_Bed_Interaction, CR_Market_Wake_Interaction (các instance khác xem bảng tra cứu).
// Reference public để đối chiếu Inspector (có nhánh tùy chọn): story.
// Code tự tìm/tạo: QuanLyGoiHangChoR35.
using UnityEngine;
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "LakeMarketInteraction")]
public sealed class TuongTacHoCho : MonoBehaviour,IPlayerInteractable
{
    public DieuPhoiHanhTrinh story;
    public enum Kind {Bed,Phone,Wake,Bao,Ha,Puppet}
    public Kind kind;
    public void Interact()
    {
        if(kind==Kind.Ha&&story.TryGetComponent<QuanLyGoiHangChoR35>(out var packages)){packages.Ha();return;}
        switch(kind){case Kind.Bed:story.Sleep();break;case Kind.Phone:story.OpenPhone();break;case Kind.Wake:story.Wake();break;case Kind.Bao:story.Talk(0);break;case Kind.Ha:story.Talk(1);break;case Kind.Puppet:break;}
    }
}
