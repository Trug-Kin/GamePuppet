// Màu khăn đi cùng vật thật; đổi label không được làm đổi phản hồi của Hà.
// Component trong scene r92: Khan_that.
using UnityEngine;
// Content travels with the physical scarf; changing a label cannot change Hà's response.
[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "MarketScarfR35")]
public sealed class KhanTrongGoiHangR35:MonoBehaviour {
 public enum Colour { MossGreen,BrickRed }
 public Colour colour;
 public string Description=>colour==Colour.MossGreen?"XANH RÊU":"ĐỎ GẠCH";
}
