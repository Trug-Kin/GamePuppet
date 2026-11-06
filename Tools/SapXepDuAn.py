"""Tổ chức mã nguồn theo bảng ánh xạ; mỗi nhóm được áp dụng riêng trước kiểm Unity."""
from pathlib import Path
import json, re, sys, uuid, subprocess

ROOT=Path(__file__).resolve().parents[1]
OUT=ROOT/'Docs/Reviews/journey_v48/restructure-20261102'
# Tên được chọn theo trách nhiệm; hậu tố revision giữ liên hệ với checkpoint.
GROUPS={
'nen': {
'ConRoiAI':('ConRoi','DieuKhienConRoi'),
'ConRoiGameManager':('KhoiDong','QuanLyTrangThaiGame'),
'GameProgress':('LuuTai','TienTrinhTroChoi'),
'ComplianceSystem':('NguoiChoi','NhichCameraKhiDungYen'),
'PlayerMovement':('NguoiChoi','DieuKhienDiChuyen'),
'MouseMovement':('NguoiChoi','DieuKhienGocNhin'),
'PlayerInteraction':('TuongTac','TuongTacNguoiChoi'),
'RegionalSurfaceLighting':('MoiTruong','AnhSangBeMatTheoKhu'),
'RegionalWalkHelp':('NguoiChoi','HoTroDiChuyenTheoKhu'),
'BedInteractable':('TuongTac','TuongTacGiuong'),
'PhoneInteractable':('TuongTac','TuongTacDienThoai'),
'DreamRegionController':('KhoiDong','DieuKhienKhuMo'),
'DreamStateVisuals':('MoiTruong','HinhAnhTrangThaiMo'),
'DreamWakeTrigger':('TuongTac','VungKichHoatTinhDay'),
'SleepFallback':('KhoiDong','DuPhongNguTinh'),
'TriggerVungDauVet':('TuongTac','VungKichHoatDauVet'),
'EndingPresenter':('GiaoDien','TrinhBayKetThuc'),
'EndingTrigger':('TuongTac','KichHoatKetThuc'),
},
'luong': {
'LakeMarketStory':('KhoiDong','DieuPhoiHanhTrinh'),
'ConRoiJourney':('KhoiDong','HanhTrinhConRoi'),
'JourneyPresentationV39':('GiaoDien','TrinhBayHanhTrinhV39'),
'JourneyPolishV40':('GiaoDien','HoanThienTrinhBayV40'),
'JourneyCompletionV41':('KhoiDong','HoanTatHanhTrinhV41'),
'JourneyExperienceV42':('KhoiDong','TraiNghiemHanhTrinhV42'),
'JourneyRevisionV43':('KhoiDong','DieuChinhHanhTrinhV43'),
'JourneyExperienceR22':('KhoiDong','TraiNghiemHanhTrinhR22'),
'JourneyExperienceR23':('KhoiDong','TraiNghiemHanhTrinhR23'),
'JourneyGuideV48':('GiaoDien','HuongDanHanhTrinhV48'),
'JourneyLoadingV44':('GiaoDien','ManHinhTaiV44'),
'JourneyUiR42':('GiaoDien','DieuPhoiGiaoDienR42'),
'PlayerEntryV47':('GiaoDien','MenuVaoGameV47'),
'DreamRoomV47':('KhoiDong','PhongTrongMoV47'),
'R21PlaytestProfile':('LuuTai','HoSoChoiThuR21'),
'R22SceneProfile':('LuuTai','HoSoSceneR22'),
'R23SceneProfile':('LuuTai','HoSoSceneR23'),
'R24SceneProfile':('LuuTai','HoSoSceneR24'),
'R25SceneProfile':('LuuTai','HoSoSceneR25'),
'R26SceneProfile':('LuuTai','HoSoSceneR26'),
'R30SceneProfile':('LuuTai','HoSoSceneR30'),
'R32SceneProfile':('LuuTai','HoSoSceneR32'),
'R33SceneProfile':('LuuTai','HoSoSceneR33'),
'R34SceneProfile':('LuuTai','HoSoSceneR34'),
'R35SceneProfile':('LuuTai','HoSoLuuTaiR35'),
},
'tuongtac': {
'JourneyInteractable':('TuongTac','DoiTuongHanhTrinh'),
'LakeMarketInteraction':('TuongTac','TuongTacHoCho'),
'Class203PapersR32':('TuongTac','TuongTacGiayLop203R32'),
'Class203PaperTargetR32':('TuongTac','DichTuongTacGiayR32'),
'LakeNotebookR37':('TuongTac','TuongTacSoTayHoR37'),
'LakeNotebookTargetR37':('TuongTac','DichTuongTacSoTayR37'),
'MarketPackagesR35':('TuongTac','QuanLyGoiHangChoR35'),
'MarketParcelR35':('TuongTac','GoiHangChoR35'),
'MarketScarfR35':('TuongTac','KhanTrongGoiHangR35'),
'MarketTargetR35':('TuongTac','DichTuongTacChoR35'),
'MarketPreparationR88':('TuongTac','ChuanBiGoiHangR88'),
'GardenInteractionV41':('TuongTac','TuongTacVuonV41'),
'NarrativePropV40':('TuongTac','DaoCuKeChuyenV40'),
'R24SlopeSpool':('TuongTac','CuonChiDocR24'),
'R25SlopeExperience':('TuongTac','TraiNghiemDocR25'),
'R28SchoolExperience':('TuongTac','TraiNghiemTruongR28'),
'R28SchoolInteraction':('TuongTac','TuongTacTruongR28'),
'SchoolChapterV37':('KhoiDong','ChuongTruongHocV37'),
'SchoolInteractionV37':('TuongTac','TuongTacTruongV37'),
'SlopeChapterV39':('KhoiDong','ChuongDocV39'),
'SlopeInteractionV39':('TuongTac','TuongTacDocV39'),
'SlopePreparationsR41':('TuongTac','ChuanBiHanhLyR41'),
'SlopePreparationTargetR41':('TuongTac','DichTuongTacHanhLyR41'),
},
'trinhbay': {
'ActorMicroPerformanceV41':('NhanVat','BieuCamNhoNhanVatV41'),
'ActorPerformanceV40':('NhanVat','DienXuatNhanVatV40'),
'BaoBridgeV48':('NhanVat','TuTheBaoBenCauV48'),
'HumanSurfaceV46':('NhanVat','BeMatNhanVatV46'),
'JourneyActingV45':('NhanVat','DienXuatHanhTrinhV45'),
'RegionalFaceProjection':('NhanVat','HienGuongMatTheoKhu'),
'PurposefulApproachR89':('ConRoi','TiepCanCoChuDichR89'),
'PuppetGroundingV41':('ConRoi','TiepDatConRoiV41'),
'PuppetPerformanceV40':('ConRoi','DienXuatConRoiV40'),
'PuppetStrideR23':('ConRoi','BuocChanConRoiR23'),
'PuppetThreadsR45':('ConRoi','DayConRoiR45'),
'PuppetThreadsR46':('ConRoi','DayConRoiR46'),
'LakeOpeningR23':('ConRoi','MoDauBenHoR23'),
'DalatMistV48':('MoiTruong','SuongDaLatV48'),
'SpatialMistFeatureR22':('MoiTruong','TinhNangSuongKhongGianR22'),
'SpatialMistR22':('MoiTruong','SuongKhongGianR22'),
'LakePlanarReflection':('MoiTruong','PhanChieuMatHo'),
'MarketSceneryV48':('MoiTruong','CanhQuanChoV48'),
'WorldSignFacesV48':('MoiTruong','MatBienHieuV48'),
'RouteGradeR73':('MoiTruong','KhoiPhucCaoDoSauTaiR73'),
'BoundaryRecoveryR21':('MoiTruong','PhucHoiTaiBienR21'),
}}

GROUPS['chandoan']={'ExperienceRecorderV40': ('ChanDoan', 'GhiNhanTraiNghiemV40'), 'FrameHitchV45': ('ChanDoan', 'GhiNhanKhungHinhTreV45'), 'ProfilePlayerRenderV47': ('ChanDoan', 'DoHieuNangRenderV47'), 'RuntimeControlProbeV43': ('ChanDoan', 'ChanDoanDieuKhienV43'), 'TaskAttributionV46': ('ChanDoan', 'DoChiPhiTacVuV46'), 'TaskAttributionV47': ('ChanDoan', 'DoChiPhiTacVuV47'), 'VisibleEvidenceV47': ('ChanDoan', 'GhiBangChungHienThiV47'), 'VisualStateV48': ('ChanDoan', 'GhiTrangThaiHinhAnhV48'), 'VisualIsolationV48': ('ChanDoan', 'CoLapHinhAnhV48')}

# Lexer bảo toàn chuỗi, comment và tên object; chỉ đổi token định danh C#.
TOKEN=re.compile(r'//[^\n]*|/\*[\s\S]*?\*/|@"(?:""|[^"])*"|\$?"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|\b[A-Za-z_][A-Za-z_0-9]*\b')
def rewrite(text,mapping):
    return TOKEN.sub(lambda m:mapping.get(m.group(),m.group()),text)

def plan():
    entries=[]
    for group,items in GROUPS.items():
        for old,(folder,new) in items.items():
            for p in (ROOT/'Assets/Scripts').rglob(old+'*.cs'):
                if p.stem!=old and not p.name.startswith(old+'.'): continue
                suffix=p.name[len(old):]
                entries.append({'group':group,'old_type':old,'new_type':new,'old':p.relative_to(ROOT).as_posix(),'new':f'Assets/Scripts/{folder}/{new}{suffix}'})
    (OUT/'script-moves.json').write_text(json.dumps(entries,ensure_ascii=False,indent=2),encoding='utf-8')
    return entries

def apply(group):
    entries=json.loads((OUT/'script-moves.json').read_text(encoding='utf-8'))
    chosen=[e for e in entries if e['group']==group]
    mapping={e['old_type']:e['new_type'] for e in chosen}
    # Mọi consumer đang compile, kể cả fixture cũ, phải dùng tên kiểu hiện hành.
    for p in (ROOT/'Assets').rglob('*.cs'):
        before=p.read_text(encoding='utf-8-sig'); after=rewrite(before,mapping)
        if before!=after: p.write_text(after,encoding='utf-8')
    for e in chosen:
        p=ROOT/e['old']; dest=ROOT/e['new']
        if not p.exists(): raise RuntimeError('Đường nguồn không tồn tại: '+str(p))
        dest.parent.mkdir(parents=True,exist_ok=True)
        meta=Path(str(dest.parent)+'.meta')
        if not meta.exists(): meta.write_text('fileFormatVersion: 2\nguid: '+uuid.uuid4().hex+'\nfolderAsset: yes\nDefaultImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n',encoding='utf-8')
        txt=p.read_text(encoding='utf-8')
        # MovedFrom bảo toàn tên kiểu serialized cũ; field/schema save không đổi.
        if p.name==e['old_type']+'.cs':
            pattern=r'(public\s+(?:(?:sealed|partial|abstract)\s+)*class\s+'+e['new_type']+r'\b)'
            txt=re.sub(pattern,'[UnityEngine.Scripting.APIUpdating.MovedFrom(true, null, "Assembly-CSharp", "'+e['old_type']+'")]\n'+r'\1',txt,count=1)
        p.write_text(txt,encoding='utf-8')
        if dest.exists(): raise RuntimeError('Đích đã tồn tại')
        p.rename(dest); Path(str(p)+'.meta').rename(Path(str(dest)+'.meta'))
    # Chỉ đọc YAML chứa tên kiểu cần đổi; không quét lại mesh binary hàng loạt.
    pattern=r'(m_EditorClassIdentifier:|m_TargetAssemblyTypeName:).*\b('+ '|'.join(mapping) + r')\b'
    found=subprocess.run(['rg','-l','-g','*.unity','-g','*.prefab','-g','*.asset',pattern,str(ROOT/'Assets')],capture_output=True,text=True,encoding='utf-8')
    for name in found.stdout.splitlines():
        p=Path(name); raw=p.read_bytes()
        if not raw.startswith(b'%YAML'):continue
        txt=raw.decode('utf-8'); after=txt
        for old,new in mapping.items():
            after=re.sub(r'(m_EditorClassIdentifier:.*?::)'+old+r'\b',r'\g<1>'+new,after)
            after=re.sub(r'(m_TargetAssemblyTypeName:\s*)'+old+r'(,)',r'\g<1>'+new+r'\2',after)
        if after!=txt:p.write_bytes(after.encode('utf-8'))
    log=OUT/('applied-'+group+'.json'); log.write_text(json.dumps(chosen,indent=2),encoding='utf-8')
    print('Applied',group,len(chosen))

if __name__=='__main__':
    if sys.argv[1]=='plan': print('Planned',len(plan()),'files')
    else: apply(sys.argv[1])
