from pathlib import Path
import json
p=Path('Test/Tam/bank97-final-manifest.py');s=p.read_text().replace('bank_r97','dam_r98').replace('R97','R98').replace('BankR98','DamR98').replace('bank97-*','dam98-*').replace('r97.unity','r98.unity').replace('New r97','New r98');Path('Test/Tam/dam98-final-manifest.py').write_text(s)
