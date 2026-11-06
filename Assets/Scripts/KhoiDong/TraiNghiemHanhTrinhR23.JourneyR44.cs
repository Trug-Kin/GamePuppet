// Bổ sung trải nghiệm R23; các phần partial dùng chung một component/owner.
// Component trong scene r92: Lake_Market_Story_GDD21.
// Code tự tìm/tạo: TuongTacSoTayHoR37, QuanLyGoiHangChoR35, TuongTacGiayLop203R32, ChuanBiHanhLyR41, TraiNghiemTruongR28.
using UnityEngine;

// Presentation only. Opt in on the development scene; r42 keeps its original path.
// Read live restored memories rather than persist another inventory or completion flag.
public sealed partial class TraiNghiemHanhTrinhR23
{
    public bool journeyR44;

    public string HeldR44
    {
        get
        {
            if(story.DreamHub||story.data.region<0)return "Không mang vật trong mơ về phòng.";
            var lake=GetComponent<TuongTacSoTayHoR37>();
            if(story.data.region==0&&lake&&lake.Held)return "Sổ của Minh · "+(lake.memory.open?"đang mở":"đang khép")+". R · Đặt lại.";
            var market=GetComponent<QuanLyGoiHangChoR35>();
            if(story.data.region==1&&market&&market.Held>=0)return "Gói "+(market.Held==0?"cạnh phiếu":"khoang bên")+" · "+(market.memory.opened[market.Held]?"đã mở":"đang đóng")+". R · Đặt về chỗ cũ.";
            var school=GetComponent<TuongTacGiayLop203R32>();
            if(story.data.region==2&&school)return school.HeldNames;
            var slope=GetComponent<ChuanBiHanhLyR41>();
            if(story.data.region==3&&slope&&slope.Held)return (slope.memory.held==0?"Thẻ hành lý cũ":"Tờ chuẩn bị")+". R · Đặt lại.";
            return "Không giữ vật. Những vật đã đặt vẫn ở trong khu.";
        }
    }

    public string JourneyGoalR44
    {
        get
        {
            switch(story.data.region)
            {
                case 0:return "Theo vỉa cầu tới biển TỈNH DẬY phía vòng xoay để về phòng. Có thể hỏi ông Bảo bên lan can vì sao ông vẫn trở lại đây.";
                case 1:return "Theo phố tới quầy Hà, rồi tới biển TỈNH DẬY để về phòng. Có thể hỏi Hà về những tối bán hàng ở chợ.";
                case 2:
                    var school=GetComponent<TraiNghiemTruongR28>();
                    var papers=GetComponent<TuongTacGiayLop203R32>();
                    if(papers)return papers.JourneyObjectiveR44;
                    return school?school.Objective:"Theo cầu thang trái tới lớp 203. Có thể hỏi Mai về lớp cũ; lối TỈNH DẬY ở cửa lớp.";
                case 3:return story.data.finalStarted?"":"Theo con dốc tới cánh cửa cuối đường; nhìn gần và E để tiếp tục. Có thể ghé Tùng bên đường, hỏi về chuyến đi anh đang chuẩn bị.";
                default:return "";
            }
        }
    }

    // Phone-only: optional activity never replaces the main navigation hint on HUD.
    public string LocalThreadR44
    {
        get
        {
            if(story.DreamHub)return "";
            switch(story.data.region)
            {
                case 0:
                    var lake=GetComponent<TuongTacSoTayHoR37>();if(!lake)return "";
                    if(lake.Held)return lake.memory.open?"Sổ đang mở. Có thể khép bằng E khi không ngắm vật khác, hoặc đặt lên mặt kê gần Bảo.":"E · Mở sổ khi không ngắm vật khác; R · Đặt lại. Không cần giữ sổ để về phòng.";
                    if(lake.memory.response>=0)return "Bảo đã nói về cuốn sổ bạn đặt lại. Có thể xem lại hoặc theo biển TỈNH DẬY về phòng.";
                    return lake.memory.viewed!=0?"Đã xem sổ. Có thể để mở hoặc khép trên mặt kê gần Bảo, hoặc tiếp tục tới lối về.":lake.memory.invited?"Bảo chỉ cuốn sổ trên mặt kê. Có thể mở xem rồi để lại cho lần trở lại.":"Bảo đứng bên lan can. Có thể ghé nghe ông kể về những lần trở lại hồ, hoặc đi tiếp.";
                case 1:
                    var market=GetComponent<QuanLyGoiHangChoR35>();if(!market)return "";
                    if(market.Held>=0)return "E · Mở / khép khi không ngắm vật khác. Có thể đối chiếu với phiếu, đặt lên ô giao rồi nói với Hà; R · Trả về chỗ cũ.";
                    if(market.memory.confirmed>=0)return "Hà đã nhận gói trên ô và nói về thứ bên trong. Gói ở lại quầy; ghi nhớ chỉ giữ những gì đã xem. Có thể đi tiếp.";
                    if(market.Candidate>=0)return "Gói mới được đặt thử, chưa giao. Gặp Hà để xác nhận hoặc cầm lại đổi gói; cũng có thể đi tiếp.";
                    if(market.memory.notChosen)return "Bạn đã nói với Hà rằng chưa chọn. Hai gói ở lại quầy; có thể xem tiếp hoặc đi tới lối về.";
                    return (market.memory.viewed&1)!=0?"Đã đọc phiếu. Có thể mở gói xem bên trong trước khi đặt lên ô giao.":market.memory.invited||market.memory.viewed!=0?"Hà muốn kiểm hàng cùng nhãn. Có thể xem phiếu trước, mở gói rồi đặt thử; cũng có thể để lại.":"Có thể ghé quầy Hà, nghe chuyện bán hàng. Lối về vẫn mở nếu bạn muốn đi tiếp.";
                case 2:
                    var papers=GetComponent<TuongTacGiayLop203R32>();if(!papers)return "";
                    return papers.memory.choice!=0?SchoolStateR44+" Có thể nói với Mai hoặc về phòng.":papers.memory.viewed==3?"Đã xem cả hai bài. N · Ghi nhớ để đặt hai nội dung cạnh nhau; có thể chọn cách đặt bài hoặc để lại.":papers.memory.viewed!=0||story.data.schoolTalks>0?"Có thể xem lại bài trên bàn, chọn cách đặt rồi nói với Mai. Không cần nộp bài để về phòng.":"Có thể gặp Mai cạnh bảng trong lớp 203, hỏi về lớp học cũ. Lối TỈNH DẬY ở cửa lớp.";
                case 3:
                    if(story.data.finalStarted)return "";
                    var slope=GetComponent<ChuanBiHanhLyR41>();if(!slope)return "";
                    if(slope.Held)return slope.memory.held==0?"E · Xem mặt kia khi không ngắm vật khác; cuộn để xoay. Có thể xếp thẻ cạnh quai vali hoặc R đặt lại.":"E · Mở / khép khi không ngắm vật khác; cuộn để xoay. R · Đặt lại tờ chuẩn bị.";
                    if(slope.memory.response>=0)return "Tùng đã kể về đồ bạn xem. Có thể hỏi lại sau khi đổi cách xếp, hoặc tiếp tục lên cửa cuối đường.";
                    return slope.memory.viewed!=0?"Đã xem đồ cạnh vali. Có thể hỏi Tùng về chúng hoặc đi tiếp.":slope.memory.invited?"Có thể xem thẻ và tờ chuẩn bị cạnh vali, rồi hỏi Tùng. Cửa cuối dốc vẫn dẫn đi tiếp.":"Tùng đứng bên đường. Có thể ghé hỏi anh về chuyến đi, hoặc tiếp tục lên cửa cuối dốc.";
                default:return "N · Mở những điều đã thấy. Vật trong mơ ở lại khu; giường vẫn dẫn tới chặng tiếp theo.";
            }
        }
    }

    public string LakeReplyR44
    {
        get
        {
            var lake=GetComponent<TuongTacSoTayHoR37>();if(!lake)return "";
            if(lake.memory.viewed==0)return "Ông Bảo: Bác để sổ trên mặt kê cho khỏi ướt. Cháu muốn xem lúc nào cũng được; bác vẫn ở đây.";
            if((lake.memory.viewed&1)==0)return "Ông Bảo: Trang cháu xem còn trống. Bác để sổ trên mặt kê, khi quay lại cháu vẫn tìm được nó.";
            if((lake.memory.viewed&2)==0&&lake.memory.open)return "Ông Bảo: Dòng cháu xem đã gạch rồi. Bác để sổ mở ở đây, cháu muốn viết tiếp lúc nào cũng được.";
            return lake.memory.open?"Ông Bảo: Cháu để mở thế à. Dòng cũ đã gạch rồi, trang bên vẫn còn chỗ. Bác để nó đây, cháu quay lại còn viết tiếp.":"Ông Bảo: Cháu khép sổ rồi. Bác vẫn giữ chỗ khô này cho nó; dòng đã gạch cũng chưa mất đi đâu.";
        }
    }

    public string BaoTalkR44(int turn)
    {
        var lake=GetComponent<TuongTacSoTayHoR37>();
        if(turn==0)return "Ông Bảo: Cháu cũng trở lại đây à. Bác nhìn từ chỗ lan can này là biết mặt hồ hôm nay thế nào.";
        if(turn==1)return lake&&(lake.memory.viewed&2)!=0?"Ông Bảo: Sổ cháu còn trang chưa viết. Bác cũng từng định nghỉ một bữa, rồi đi nửa đường lại quay về đây.":"Ông Bảo: Có lần bác định nghỉ một bữa. Đi được nửa đường rồi lại quay về. Không ai gọi cả.";
        return story.data.traces[0]?"Ông Bảo: Cháu thấy tờ lịch rồi à. Bác vẫn nghĩ mai rồi xé, thành ra ngày ấy còn ở đây.":"Ông Bảo: Có hôm bác chỉ đứng một lát rồi về. Vậy mà sáng sau vẫn ra đúng chỗ này.";
    }

    public string MarketReplyR44(bool matchesSlip)
    {
        var market=GetComponent<QuanLyGoiHangChoR35>();if(!market)return "";
        if(market.Candidate<0)return "Chị Hà: Chưa chọn cũng được. Chị để hàng lại quầy; nhìn mỗi vỏ gói thì chính chị cũng dễ lấy nhầm. Em muốn đi cứ đi nhé.";
        return matchesSlip?"Chị Hà: Đúng khăn xanh rêu trên phiếu. Chị cứ lấy theo vỏ, may em mở ra xem. Chị giữ gói này trên ô giao, không lẫn lại vào hai chỗ cũ.":"Chị Hà: Khăn đỏ gạch, mà phiếu của chị ghi xanh rêu. Chị dán cùng nhãn nên mới lẫn. Chị để riêng gói em mở trên ô này; em không phải sửa hết quầy cho chị.";
    }

    public string SchoolStateR44
    {
        get
        {
            var papers=GetComponent<TuongTacGiayLop203R32>();if(!papers)return "";
            if(papers.memory.places[1]==2)return "Bài mẫu đang trong khay.";
            if(papers.memory.places[0]==1)return "Bạn còn cầm bài của Minh; bài chưa nằm trong khay.";
            if(papers.memory.places[0]==3)return "Bài của Minh đang ngoài khay.";
            if(papers.memory.places[1]==1)return "Bạn còn cầm bài mẫu; bài chưa nằm trong khay.";
            return "Bài đang để lại trên bàn; chưa có bài mẫu trong khay.";
        }
    }

    public string SchoolReplyR44
    {
        get
        {
            var papers=GetComponent<TuongTacGiayLop203R32>();if(!papers)return "";
            if(papers.memory.viewed==0&&papers.memory.places[0]==0&&papers.memory.places[1]==0)return "Cô Mai: Cô còn giữ bài có tên em trên bàn. Nếu muốn biết hồi đó mình viết gì, em cứ xem; hôm nay không phải nộp lại.";
            if(papers.memory.places[1]==2)return (papers.memory.viewed&1)!=0?"Cô Mai: Bài mẫu ở trong khay rồi. Còn ý em muốn thử một tháng vẫn ở bài có tên em; cô chưa viết thêm gì vào đó.":"Cô Mai: Bài mẫu ở trong khay rồi. Cô nhận ra dàn ý này; bài có tên em vẫn có thể lấy ra xem, không cần viết theo ngay.";
            if(papers.memory.places[0]==1)return (papers.memory.viewed&1)!=0?"Cô Mai: Em còn cầm bài của mình. Câu muốn thử một tháng vẫn ở đó, dù cô đã gạch. Em chưa phải đặt nó vào khay.":"Cô Mai: Em đang giữ bài có tên mình. Cứ xem trước đã, khay vẫn để đây.";
            if(papers.memory.places[0]==3)return (papers.memory.viewed&1)!=0?"Cô Mai: Em để bài riêng ngoài khay. Dòng cô gạch vẫn đọc được; hôm nay cô để nguyên, em không cần chép lại.":"Cô Mai: Bài có tên em đang ngoài khay. Em có thể xem lại trước khi quyết định đặt đâu.";
            if(papers.memory.places[1]==1)return "Cô Mai: Em đang giữ bài mẫu, chưa đặt vào khay. Cứ xem trước đã; bài có tên em vẫn ở đây.";
            return (papers.memory.viewed&1)!=0?"Cô Mai: Bài em lại nằm trên bàn. Lời nhận xét của cô vẫn ở cạnh câu em viết; em có thể đọc lại cả hai, hoặc để lần khác.":"Cô Mai: Bài mẫu và bài có tên em vẫn ở bàn. Em muốn xem tiếp hoặc để lần khác đều được.";
        }
    }

    public string SlopeReplyR44
    {
        get
        {
            var slope=GetComponent<ChuanBiHanhLyR41>();if(!slope)return "";
            if(slope.memory.held==0)return "Anh Tùng: Em đang cầm thẻ à. Chữ trên đó phai rồi; anh vẫn giữ nó cho chuyến đi này.";
            if((slope.memory.viewed&2)!=0)return slope.memory.paperOpen?"Anh Tùng: Tờ chuẩn bị còn mở. Áo anh xếp rồi, chỉ dòng mua vé là sửa ngày mấy lần. Anh cứ nghĩ sáng mai sẽ làm.":"Anh Tùng: Anh gấp tờ ấy lại sau mỗi lần sửa ngày mua vé. Việc xếp áo thì xong lâu rồi; vé vẫn chưa mua.";
            if(slope.memory.tagSlot==1)return "Anh Tùng: Thẻ em xếp cạnh quai rồi. Lần trước anh cũng xếp gọn thế để dễ nhấc vali lên; cuối cùng vẫn để đây.";
            return "Anh Tùng: Thẻ ấy anh giữ từ chuyến định đi trước. Ngày trên đó sửa rồi mà vali vẫn ở chỗ này.";
        }
    }

    public string KnownConnectionsR44
    {
        get
        {
            var lake=GetComponent<TuongTacSoTayHoR37>();var school=GetComponent<TuongTacGiayLop203R32>();var slope=GetComponent<ChuanBiHanhLyR41>();
            if(lake&&(lake.memory.viewed&1)!=0&&school&&(school.memory.viewed&1)!=0)return "ĐẶT CẠNH NHAU\nTrong sổ: ‘Tối nay viết một trang’ bị gạch. Trên bài của Minh: ‘tự quyết định’ bị gạch. Hai dòng đều đã xem; có thể mở lại từng ghi nhớ.";
            if(lake&&(lake.memory.viewed&1)!=0&&slope&&(slope.memory.viewed&2)!=0)return "ĐẶT CẠNH NHAU\nTrong sổ có việc viết một trang bị gạch; trên tờ chuẩn bị, ngày mua vé đã đổi nhiều lần. Đây là hai chi tiết đã xem.";
            return "";
        }
    }

    public string ReturnNoteR44
    {
        get
        {
            if(story.data.nextRegion<=0)return "Vy nhắc Minh ghi lại giấc mơ. Khi sẵn sàng, giường dẫn tới hồ.";
            string seen="";
            switch(story.data.nextRegion-1)
            {
                case 0:
                    var lake=GetComponent<TuongTacSoTayHoR37>();
                    if(lake&&lake.memory.viewed!=0)seen=lake.Notes;
                    break;
                case 1:
                    var market=GetComponent<QuanLyGoiHangChoR35>();
                    if(market&&market.memory.viewed!=0)seen=market.Notes;
                    break;
                case 2:
                    var papers=GetComponent<TuongTacGiayLop203R32>();
                    if(papers)for(int i=0;i<2;i++)if((papers.memory.viewed&(1<<i))!=0)seen+=(seen.Length>0?"\n":"")+"Đã xem "+TuongTacGiayLop203R32.Names[i].ToLowerInvariant()+".";
                    break;
                case 3:
                    var slope=GetComponent<ChuanBiHanhLyR41>();
                    if(slope&&slope.memory.viewed!=0)seen=slope.Notes;
                    break;
            }
            int region=story.data.nextRegion-1;
            string trace=region==0&&story.data.traces[0]?"Tờ lịch ở lan can vẫn giữ một ngày.":region==1&&story.data.traces[1]?"Chồng sách ở quầy còn nguyên bọc.":region==2&&story.data.schoolTrace?"Ghế giáo viên quay ra cửa sổ.":region==3&&story.data.slopeTrace?"Vali cạnh đường đã phủ bụi.":"";
            if(trace.Length>0)seen+=(seen.Length>0?"\n":"")+trace;
            return (seen.Length>0?"CÒN NHỚ TỪ GIẤC MƠ\n"+seen:"Đã về phòng. Không có nội dung vật phẩm hoặc dấu vết mới từ khu vừa rời.")+"\nVật trong mơ ở lại khu. Bạn có thể ghi, gọi hoặc để tin nhắn lại rồi ngủ tiếp.";
        }
    }
}
