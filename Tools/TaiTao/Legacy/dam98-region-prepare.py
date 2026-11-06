from pathlib import Path
p=Path('Test/Tam/dam98-region-return.cs.txt');p.write_text(Path('Test/Tam/bank97-region-return.cs.txt').read_text(encoding='utf-8-sig').replace('bank_r97','dam_r98').replace('BankR97/','DamR98/').replace('R97_Bridgehead_GateSide_Bank','R98_OngDao_Concrete_Dam'))
