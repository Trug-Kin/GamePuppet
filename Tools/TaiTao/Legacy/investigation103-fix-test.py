from pathlib import Path
p=Path('Test/Tam/investigation103-playback.cs.txt')
s=p.read_text().replace('+21.2f','+55f').replace('450,537','450,555').replace('800,537','800,555').replace('580,578','580,581').replace('420,165','420,170').replace('phase*43','phase*41')
s=s.replace('log.AppendLine("completed "+ids[phase]', 'if(s.TrialId!=ids[phase])throw new Exception("UI wrong case "+ids[phase]+" actual="+s.TrialId);log.AppendLine("completed "+ids[phase]')
p.write_text(s)
