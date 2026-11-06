from pathlib import Path
p=Path('Test/Tam/dam98-evidence.py');s=p.read_text();s=s.replace("dr.polygon([p(q) for q in water],fill='#afd5df')", "dr.polygon([p(q) for q in water],fill='#afd5df');dr.rectangle((0,0,1400,95),fill='#f3f5f6');dr.text((28,18),'R98 — Dam / end stairs / R97 tie',font=font(30),fill='#152c39');dr.text((28,60),'Plan: estimated authoring metres; north=-Unity Z, east=-Unity X',font=font(18),fill='#425667')")
s=s.replace("base(1500,1150,", "base(1500,1250,").replace("140+j*178", "140+j*195").replace("(90,1060)","(90,1175)")
p.write_text(s)
