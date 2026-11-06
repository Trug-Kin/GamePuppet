from pathlib import Path
p=Path('Test/Tam/bank97-scene-delta.py');s=p.read_text().replace('bank_r97','dam_r98').replace("a=read('r96');b=read('r97')","a=read('r97');b=read('r98')");Path('Test/Tam/dam98-scene-delta.py').write_text(s)
s=Path('Test/Tam/bank97-preserve.py').read_text().replace('bank_r97','dam_r98').replace('protected-current-start.json','protected-start.json');Path('Test/Tam/dam98-preserve.py').write_text(s)
