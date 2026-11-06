from pathlib import Path
p=Path('Test/Tam/investigation104-route.cs.txt');s=p.read_text(encoding='utf-8')
s=s.replace('keys=new KeyboardState(Key.E);mode=2;shot=false;hold=now;}}\n    else if(mode==2)', 'mode=5;hold=now;}}\n    else if(mode==5){if(now>hold+.4){keys=new KeyboardState(Key.E);mode=2;shot=false;hold=now;}}\n    else if(mode==2)')
p.write_text(s,encoding='utf-8')
