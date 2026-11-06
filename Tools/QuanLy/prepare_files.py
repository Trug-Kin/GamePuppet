from pathlib import Path
import json,hashlib,shutil,re
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy';B=D/'Before';B.mkdir(exist_ok=True)
for name in ['.gitignore','README.md','BAT_DAU_TAI_DAY.md']:
 p=B/name
 if not p.exists():shutil.copy2(R/name,p)
ignore='''# Unity per-machine data (Library is a junction on the original machine)
/[Ll]ibrary/
/[Tt]emp/
/[Oo]bj/
/[Ll]ogs/
/[Uu]ser[Ss]ettings/
/.build-cache/
/.build-temp/
/.tmp/
/.vs/
**/__pycache__/
/*.csproj
/*.sln
/*.slnx
*.user
*.userprefs
*.suo
*.pidb
*.opendb
*.VC.db
.DS_Store
Thumbs.db
# Specific technical output roots, never blanket-ignore game media or Assets
/Test/BangChung/
/Test/BanChoiThu/
/Test/Output/
/Test/PlaytestProfiles/
/Test/Tam/
/Build/
/Builds/*
!/Builds/README.md
# Packages, Source, all game media, scene/prefab text and .meta are kept.
'''
(R/'.gitignore').write_text(ignore,encoding='utf-8')
attrs='''# Review Unity serialization as text; never wildcard LFS every .asset.
* text=auto
*.meta text eol=lf
*.unity text eol=lf
*.prefab text eol=lf
*.cs text eol=lf
*.md text eol=lf
*.json text eol=lf
*.blend filter=lfs diff=lfs merge=lfs -text
*.fbx filter=lfs diff=lfs merge=lfs -text
*.FBX filter=lfs diff=lfs merge=lfs -text
*.png filter=lfs diff=lfs merge=lfs -text
*.jpg filter=lfs diff=lfs merge=lfs -text
*.jpeg filter=lfs diff=lfs merge=lfs -text
*.tga filter=lfs diff=lfs merge=lfs -text
*.exr filter=lfs diff=lfs merge=lfs -text
*.hdr filter=lfs diff=lfs merge=lfs -text
*.psd filter=lfs diff=lfs merge=lfs -text
*.wav filter=lfs diff=lfs merge=lfs -text
*.ogg filter=lfs diff=lfs merge=lfs -text
*.mp3 filter=lfs diff=lfs merge=lfs -text
*.mp4 filter=lfs diff=lfs merge=lfs -text
*.glb filter=lfs diff=lfs merge=lfs -text
*.dll filter=lfs diff=lfs merge=lfs -text
*.zip filter=lfs diff=lfs merge=lfs -text
'''
large=[]
for p in (R/'Assets').rglob('*.asset'):
 if p.stat().st_size>50*1024**2:
  path=p.relative_to(R).as_posix();attrs+='"'+path+'" filter=lfs diff=lfs merge=lfs -text\n';large.append(path)
(R/'.gitattributes').write_text(attrs,encoding='utf-8')
for mod,owner in [('Map',1),('NgheThuat',1),('Gameplay',2),('Contracts',2),('GiaoDien',3),('NoiDung',3),('TichHop',3)]:
 p=R/'Assets/ConRoi/Modules'/mod;p.mkdir(parents=True,exist_ok=True)
 (p/'README.md').write_text(f'# {mod}\n\nChủ: người {owner}, tra Docs/QuanLy/OWNERSHIP.md. Thư mục dành cho công việc mới; legacy chưa move vì dependency/path. Scene thử nếu có là template rỗng, không đại diện cho prototype chạy độc lập. Không sửa checkpoint r107.\n',encoding='utf-8')
for n,branch,prompt in [(1,'work/map-asset-baseline','Kiểm dependency và GUID map/art hiện hành; chỉ báo cáo vấn đề có căn cứ, chưa sửa terrain/art hoặc scene chính.'),(2,'work/runtime-contracts-baseline','Đọc toàn bộ partial r107, ghi contract hiện hành và ownership; không đổi cơ cấu, controller hoặc schema.'),(3,'work/ui-onboarding-review','Dùng scene tích hợp và profile thử mới để review mở đầu; ghi vấn đề cụ thể, không sửa partial chung khi người2 đang làm.')]:
 (D/f'DANG_LAM_NGUOI_{n}.md').write_text(f'# Người {n} — mẫu nhận việc\n\nTrạng thái: CHUA_NHAN. Nhánh đề xuất `{branch}`; commit gốc: ghi từ `git rev-parse HEAD` sau clone.\n\nPrompt đầu: {prompt}\n\n- File sẽ chạm:\n- Scene thử:\n- Vật chứng/ID ảnh hưởng:\n- Kiểm cần chạy:\n- Người đang cùng chạm file:\n- Kết quả/commit/giới hạn:\n\nKhông cập nhật lịch sử chung hoặc scene chính nếu không phải người3.\n',encoding='utf-8')
# Small historic reports copied verbatim with original hash, not regraded.
qa=R/'Docs/QA';qa.mkdir(exist_ok=True);copies=[]
for rev in range(102,108):
 src=R/f'Test/KetQua/investigation_r{rev}';dst=qa/f'investigation_r{rev}'
 if not src.exists():continue
 dst.mkdir(exist_ok=True)
 for p in src.iterdir():
  if p.is_file() and (p.suffix=='.md' or (p.suffix in ['.json','.txt'] and p.stat().st_size<200_000)) and 'profile' not in p.name and 'state' not in p.name:
   shutil.copy2(p,dst/p.name);copies.append(dict(old=p.relative_to(R).as_posix(),new=(dst/p.name).relative_to(R).as_posix(),sha256=hashlib.sha256(p.read_bytes()).hexdigest()))
(D/'qa-copies.json').write_text(json.dumps(copies,ensure_ascii=False,indent=2),encoding='utf-8')
# Reuse existing targeted verification fixtures; change outputs only, not gameplay/input actions.
verify=R/'Tools/QuanLy/Verification';verify.mkdir(exist_ok=True)
(R/'Docs/QA/GitPreparation').mkdir(parents=True,exist_ok=True);(R/'Test/Output/GitPreparation').mkdir(parents=True,exist_ok=True)
for name,new in [('r107-onboarding-route.cs.txt','route.cs.txt'),('r107-onboarding-ui.cs.txt','ui.cs.txt')]:
 s=(R/'Test/Tam'/name).read_text(encoding='utf-8').replace('Test/KetQua/investigation_r107','Docs/QA/GitPreparation').replace('Test/BangChung/investigation_r107','Test/Output/GitPreparation')
 (verify/new).write_text(s,encoding='utf-8')
(D/'file-setup.json').write_text(json.dumps(dict(qa_copies=len(copies),large_asset_lfs=large,product_refactor=False),indent=2),encoding='utf-8')
print('Prepared ignore/LFS policy, ownership folders, QA copies and reused targeted checks; QA copies:',len(copies),'large mesh LFS:',len(large))
