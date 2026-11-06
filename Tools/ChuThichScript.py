"""Chú thích dựa trên source và GUID trong scene, không sửa logic hoặc serialized field."""
from pathlib import Path
import json,re
R=Path(__file__).resolve().parents[1]; O=R/'Docs/Reviews/journey_v48/restructure-20261102'
ROLE={
'ConRoiAI':'Điều khiển bốn giai đoạn bắt chước của rối; giữ hàng đợi độ trễ GĐ2 đủ 0,5 giây, bỏ delta chuyển cảnh trên 2 m.',
'ConRoiGameManager':'Giữ singleton trạng thái thức/mơ/kết thúc và phát sự kiện đổi khu; tồn tại qua chuyển scene.',
'GameProgress':'Giữ hai trục tiến trình và snapshot; ngưỡng ending đọc số tương tác đời thực, không thay bằng tổng điểm có trọng số.',
'ComplianceSystem':'Nhích camera khi đứng yên theo nhánh legacy; không gộp với owner camera của hành trình.',
'PlayerMovement':'Đọc Input System và dịch chuyển CharacterController; hook input thử chỉ hoạt động trong scene kiểm riêng.',
'MouseMovement':'Xoay người/camera từ input nhìn; hành trình sở hữu pause và cursor, tránh mở khóa lại sau resume.',
'PlayerInteraction':'Raycast từ camera để gọi IPlayerInteractable; khóa tương tác theo trạng thái hành trình.',
'RegionalSurfaceLighting':'Áp dụng ánh sáng bề mặt theo vùng; dùng chung với camera/gameplay hiện có.',
'RegionalWalkHelp':'Hiện hướng dẫn di chuyển cho scene môi trường khi owner hành trình chưa chạy.',
'BedInteractable':'Tương tác giường để chuyển sang nhánh ngủ hiện có.',
'PhoneInteractable':'Tương tác điện thoại theo tiến trình hiện có.',
'DreamRegionController':'Đếm dấu vết của khu mơ và phát sự kiện hoàn tất khu.',
'DreamStateVisuals':'Đổi màu/volume và âm drone theo trạng thái thức/mơ; tìm API volume bằng reflection để tương thích pipeline.',
'DreamWakeTrigger':'Trigger cho Player tỉnh dậy và chuyển khu theo các cờ Inspector.',
'SleepFallback':'Lưới dự phòng thời gian ngủ/mơ của graybox; giữ khoảng chờ để không cướp lượt tỉnh chủ động.',
'TriggerVungDauVet':'Kích hoạt dấu vết một lần, phát UnityEvent và đổi giai đoạn rối nếu được cấu hình.',
'EndingPresenter':'Trình bày kết thúc theo sự kiện từ game manager.',
'EndingTrigger':'Kích hoạt nhánh kết thúc khi đủ điều kiện trigger hiện có.',
'LakeMarketStory':'Điều phối vùng, tương tác, điện thoại, pause và save/load; các chương trường/dốc sở hữu tiến trình cục bộ.',
'ConRoiJourney':'Luồng hành trình legacy dùng reference scene serialized, không sử dụng fixture di chuyển.',
'JourneyPresentationV39':'Trình bày hành trình/chương cuối và các reference rối hiện có.',
'JourneyPolishV40':'Quản lý trình bày thoại/đọc và phản hồi trải nghiệm; dùng chung owner hành trình.',
'JourneyCompletionV41':'Bổ sung hoàn tất/trải nghiệm hành trình theo vùng và telemetry hiện có.',
'JourneyExperienceV42':'Bổ sung lớp trải nghiệm hành trình V42 trên các owner hiện hữu.',
'JourneyRevisionV43':'Điều chỉnh lớp trình bày hành trình V43; không thay controller người chơi.',
'JourneyExperienceR22':'Bổ sung trải nghiệm scene opt-in R22.',
'JourneyExperienceR23':'Bổ sung trải nghiệm R23; các phần partial dùng chung một component/owner.',
'JourneyGuideV48':'Hiện gợi ý hành trình theo trạng thái và vùng hiện tại.',
'JourneyLoadingV44':'Giữ màn hình tải tới khi kiểm GPU readback hoàn tất; không thay bằng chờ thời gian cố định.',
'JourneyUiR42':'Phân xử giao diện hành trình; giữ thứ tự với menu, pause và các màn đọc.',
'PlayerEntryV47':'Menu vào game/tiếp tục/chơi mới; NewGame gọi Restart, Continue chỉ đóng menu sau Load thành công.',
'DreamRoomV47':'Đoạn mơ ngắn trong phòng quen thuộc; không thêm mục tiêu hoặc điểm nhận thức.',
'R21PlaytestProfile':'Chọn cấu hình scene/profile cho lượt chơi thử R21.',
'R22SceneProfile':'Áp profile và cấu hình riêng cho scene R22.',
'R23SceneProfile':'Áp profile và cấu hình riêng cho scene R23.',
'R24SceneProfile':'Áp profile và cấu hình riêng cho scene R24.',
'R25SceneProfile':'Áp profile và cấu hình riêng cho scene R25.',
'R26SceneProfile':'Chọn pipeline và đường save dự phòng sớm trước khi hành trình khởi tạo.',
'R30SceneProfile':'Áp profile/save riêng cho scene R30.',
'R32SceneProfile':'Chọn đường save riêng cho scene R32.',
'R33SceneProfile':'Chọn đường save riêng cho scene R33.',
'R34SceneProfile':'Chọn đường save riêng cho scene R34.',
'R35SceneProfile':'Chọn profile save sớm; seed legacy một lần khi chưa có save và không có đối số review riêng.',
'JourneyInteractable':'Đích tương tác của hành trình legacy; chuyển thao tác cho owner hiện có.',
'LakeMarketInteraction':'Đích tương tác hồ/chợ; kiểm vùng và trạng thái trước khi chuyển thao tác cho story.',
'Class203PapersR32':'Quản lý hai bài giấy lớp 203, cầm/đặt/đối chiếu và memory; không cộng hai trục ending.',
'Class203PaperTargetR32':'Chuyển thao tác trên giấy/khay cho activity theo ID và cờ inTray.',
'LakeNotebookR37':'Quản lý cầm/đặt/mở khép cùng cặp trang sổ; lưu hinge dở, không dùng page giả thay geometry.',
'LakeNotebookTargetR37':'Đích tương tác sổ, chuyển thao tác về activity đã gán.',
'MarketPackagesR35':'Quản lý gói hàng, ô giao và phản hồi Hà; đọc khăn thật thay vì nhãn hoặc ID nút.',
'MarketParcelR35':'Component gói hàng: ID, nắp, khăn, nhãn và thao tác nhặt qua activity.',
'MarketScarfR35':'Màu khăn đi cùng vật thật; đổi label không được làm đổi phản hồi của Hà.',
'MarketTargetR35':'Đích thao tác chợ theo cấu hình của activity.',
'MarketPreparationR88':'Rối chuẩn bị cơ hội giao gói hàng bằng stride hiện hữu; không chọn thay Minh.',
'GardenInteractionV41':'Tương tác đạo cụ vườn theo owner hành trình.',
'NarrativePropV40':'Đạo cụ kể chuyện của lớp trải nghiệm V40.',
'R24SlopeSpool':'Cuộn chỉ trong trải nghiệm dốc R24.',
'R25SlopeExperience':'Lớp trải nghiệm dốc R25, giữ cho checkpoint cũ.',
'R28SchoolExperience':'Điều phối diễn xuất trường/lớp 203; partial R30 dùng chung owner staging và trạng thái lưu.',
'R28SchoolInteraction':'Chuyển tương tác trường cho owner trải nghiệm R28.',
'SchoolChapterV37':'Chương trường tùy chọn; khi không có component, checkpoint cũ chỉ gồm hồ/chợ.',
'SchoolInteractionV37':'Tương tác trường theo vùng 2; chặn khi pause hoặc mở điện thoại.',
'SlopeChapterV39':'Chương dốc; GĐ4 chỉ thuộc cuộc gặp cuối một lần, không thêm AI bắt chước khi khám phá.',
'SlopeInteractionV39':'Tương tác vùng dốc 3: cửa cuối hoặc ký ức theo cấu hình.',
'SlopePreparationsR41':'Cầm/đặt/đọc hành lý opt-in; memory không ghi điểm, witness hoặc ending.',
'SlopePreparationTargetR41':'Chuyển thao tác hành lý về activity theo item/arrange.',
'ActorMicroPerformanceV41':'Chớp mắt/miệng nhỏ sau diễn xuất chính; tìm các phần mắt/miệng trong con của actor.',
'ActorPerformanceV40':'Điều khiển nhìn và clip thân actor; không tranh kênh với crossfade mặt.',
'BaoBridgeV48':'Sở hữu pose tiếp xúc lan can của Bảo ở LateUpdate; thoại/điểm/chương vẫn do owner cũ quản lý.',
'HumanSurfaceV46':'Cấu hình lớp bề mặt nhân vật hiện hữu.',
'JourneyActingV45':'Diễn xuất và bước chân nhân vật theo vùng, phối hợp rig/clip hiện có.',
'RegionalFaceProjection':'Hiện/crossfade gương mặt theo khu; R91 dùng portrait jaw-only gắn với ghostHead/NoseTip.',
'PurposefulApproachR89':'Rối tiếp cận điểm hành động khi có cơ hội; không ghi đè lựa chọn giấy/sổ của Minh.',
'PuppetGroundingV41':'Căn tiếp đất rối theo cấu hình trình bày hiện có.',
'PuppetPerformanceV40':'Diễn xuất rối trên owner hiện hữu.',
'PuppetStrideR23':'Đọc quỹ đạo bước/điểm trụ từ nguồn Blender và fit chân rối; giữ trạng thái plant khi restore.',
'PuppetThreadsR45':'Lớp dây rối R45 giữ cho checkpoint cũ.',
'PuppetThreadsR46':'Dây rối theo anchor mesh chuyển động; giữ đăng ký dây đứt/sửa của phòng mơ.',
'LakeOpeningR23':'Điều phối rối tại dải hồ, sight/LOS và witness theo cơ hội đi/dừng.',
'DalatMistV48':'Nội suy vertex fog URP theo vùng hồ/chợ; không dùng overlay che hình học gần.',
'SpatialMistFeatureR22':'RendererFeature sương không gian; shader gán trong Renderer Data, không gắn GameObject.',
'SpatialMistR22':'Mật độ sương neo thế giới với corridor trong tọa độ map.',
'LakePlanarReflection':'Camera phản chiếu mặt hồ trong URP; giữ nhánh VP/flip và occlusion đã kiểm ở R39.',
'MarketSceneryV48':'Hiện/ẩn cảnh quan chợ theo trạng thái vùng.',
'WorldSignFacesV48':'Chỉ hiện mặt chữ đang hướng về camera vì vật liệu font legacy vẽ hai phía.',
'RouteGradeR73':'Migrate cao độ một lần sau load thành công; không chỉnh grounded/controller mỗi frame.',
'BoundaryRecoveryR21':'Khôi phục tại biên opt-in của scene recovery; không tự ghi save hoặc đổi tiến trình.',
}

ROLE.update({'ExperienceRecorderV40': 'Ghi nhận trải nghiệm theo cấu hình quan sát hiện có; không dùng làm driver thao tác.', 'FrameHitchV45': 'Ghi thời gian frame/GC và vị trí vào CSV khi được bật để điều tra spike.', 'ProfilePlayerRenderV47': 'Đo render player theo cấu hình profiling opt-in hiện có.', 'RuntimeControlProbeV43': 'Quan sát input/control runtime và ghi CSV; không thay controller.', 'TaskAttributionV46': 'Đo chi phí tác vụ V46 khi profiling được bật.', 'TaskAttributionV47': 'Đo chi phí tác vụ V47 khi profiling được bật.', 'VisibleEvidenceV47': 'Quan sát hiển thị/cửa sổ và ghi bằng chứng thụ động; chỉ bật với --verification-output và --visible-validation.', 'VisualStateV48': 'Quan sát trạng thái hình ảnh; không sửa light/material hay ép readback, F8 giữ ảnh và trạng thái.', 'VisualIsolationV48': 'Công cụ ablation hình ảnh cho developer, chỉ bật khi có --visual-isolation; không thuộc lượt chơi thường.'})

def main():
    entries=json.loads((O/'script-moves.json').read_text(encoding='utf-8'))
    bindings=json.loads((O/'script-scene-bindings.json').read_text(encoding='utf-8'))
    type_names={e['new_type'] for e in entries}|{'Transform','GameObject','Camera','Collider','CharacterController','Renderer','Mesh','Material','AudioSource','AudioClip','TextMesh','Animation','Shader','Light'}
    docs=[]
    for e in entries:
        p=R/e['new']
        if not p.exists():continue
        t=p.read_text(encoding='utf-8-sig'); old=e['old_type']
        while t.startswith('//'):t=t.split('\n',1)[1]
        own=bindings.get(old,[]); current=[b for b in own if b['scene'].endswith('_r92.unity')]
        names=list(dict.fromkeys(b['object'] for b in current))
        refs=[]
        for typ,decl in re.findall(r'public\s+([\w]+)(?:\[\])?\s+([^;{}]+);',t):
            if typ in type_names:
                for name in decl.split(','):
                    name=name.split('=')[0].strip()
                    if re.fullmatch(r'\w+',name):refs.append(name)
        lines=[ROLE[old]]
        if names:lines.append('Component trong scene r92: '+', '.join(names[:6])+(' (các instance khác xem bảng tra cứu).' if len(names)>6 else '.'))
        elif own:lines.append('Không gắn trực tiếp trong r92; checkpoint '+own[-1]['scene'].split('/')[-1]+': '+own[-1]['object']+'.')
        elif old!='SpatialMistFeatureR22':lines.append('Không thấy attachment trực tiếp trong YAML scene; xem consumer AddComponent và prefab trong bảng tra cứu.')
        if refs:lines.append('Reference public để đối chiếu Inspector (có nhánh tùy chọn): '+', '.join(refs[:14])+'.')
        auto=list(dict.fromkeys(re.findall(r'(?:GetComponent(?:InChildren|InParent)?|FindAnyObjectByType|AddComponent)<([\w]+)>',t)))
        if auto:lines.append('Code tự tìm/tạo: '+', '.join(auto[:8])+'.')
        order=re.search(r'DefaultExecutionOrder\((-?\d+)\)',t)
        if order:lines.append('Thứ tự chạy khai báo: '+order.group(1)+'; giữ quan hệ khởi tạo/pose với các owner khác.')
        if old=='LakeMarketStory':lines += ['Update bỏ qua khi Loading/MenuOpen; xử lý save/load trước nhánh return khi pause.', 'State JSON version 1 và tên field giữ tương thích; profile chọn trước Awake, test dùng --r30-review-output riêng.']
        if old=='R35SceneProfile':lines += ['Awake -31800 chạy sau profile R26 -31900; không seed khi có --r30-review-output, không ghi đè save có sẵn.']
        if old=='ConRoiAI':lines += ['Pause khi Time.timeScale bằng 0; teleport chuyển khu không phải hành động bắt chước.']
        if old in ['PurposefulApproachR89','MarketPreparationR88']:lines += ['Start kiểm reference bắt buộc; memory/SuccessfulLoads phối hợp restore, không chọn hộ người chơi.']
        if old in ['Class203PapersR32','LakeNotebookR37','MarketPackagesR35','SlopePreparationsR41']:lines += ['Memory giữ tên field/schema; restore phối hợp owner State và bộ đếm SuccessfulLoads.']
        header='// '+ '\n// '.join(lines)+'\n'
        if not t.startswith(header):p.write_text(header+t,encoding='utf-8')
        docs.append(dict(e,inspector_references=refs,r92_objects=names,attachments=own,role=ROLE[old]))
    (O/'script-guide.json').write_text(json.dumps(docs,ensure_ascii=False,indent=2),encoding='utf-8')
    print('Annotated',len(docs),'files')
if __name__=='__main__':main()
