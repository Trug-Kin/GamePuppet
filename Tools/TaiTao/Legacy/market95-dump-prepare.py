from pathlib import Path
p=Path('Test/Tam/ground-r93-inventory.cs.txt').read_text()
p=p.replace('r93.unity','r94.unity').replace('clean r93','clean r94').replace('Test/KetQua/journey_v48/ground_r93/','Test/KetQua/journey_v48/market_ground_r95/')
p=p.replace('if(env&&ground&&broad&&wb.min.y<25&&m.isReadable)pick.Add(f);','if(m.isReadable&&File.ReadAllText(d+"native-input.json").Contains("\\\"path\\\": \\\""+row.path+"\\\""))pick.Add(f);')
Path('Test/Tam/market95-dump.cs.txt').write_text(p,encoding='utf-8')
