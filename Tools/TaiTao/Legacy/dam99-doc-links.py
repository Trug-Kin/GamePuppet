from pathlib import Path
R=Path('D:/ConRoi')
for name in ['PROJECT_GUIDE_AND_HISTORY.md','BLENDER_UNITY_WORKFLOW.md']:
 p=R/'Docs'/name;s=p.read_text(encoding='utf-8');needle='\n## ';i=s.index(needle)
 note='\n**Ảnh chồng mái thực/model:** [181347](../Test/BangChung/journey_v48/dam_r99/overlay-candidate-181347.jpg), [181402](../Test/BangChung/journey_v48/dam_r99/overlay-candidate-181402.jpg); alpha48%, chỉ mesh mái để thấy biên hình học, cùng camera gần đúng đã lưu. Không dùng overlay này để nghiệm thu toàn cấu trúc hoặc cao độ mét. `dam99-overlay-render.py` chỉ render tạm từ file đã lưu, không ghi lại nguồn. `dam99-finalize.py` đóng manifest/hash, không xuất asset.\n'
 if '**Ảnh chồng mái thực/model:**' not in s[:i]:p.write_text(s[:i]+note+s[i:],encoding='utf-8')
print('Overlay links appended to existing sections')
