from pathlib import Path
s=Path('Test/Tam/dam99-flower-capture.cs.txt').read_text(encoding='utf-8-sig').replace('Test/KetQua/journey_v48/dam_r99/flower-sources/','Test/KetQua/journey_v48/map_r99_closeout/actual-flower-sources/')
Path('Test/Tam/map100-flower-capture.cs.txt').write_text(s)
