from pathlib import Path
p=Path('Test/Tam/investigation104-route.cs.txt');s=p.read_text(encoding='utf-8')
s=s.replace('if(mode==2){','if(mode==4){if(now>hold+.6){ScreenCapture.CaptureScreenshot("Test/BangChung/investigation_r104/table-prompt-raycast.png");log.AppendLine("PROMPT actual controller ray table; visible label derived from focused action; page="+s.Page);keys=new KeyboardState(Key.E);mode=2;shot=false;hold=now;}}\n    else if(mode==2){')
s=s.replace('keys=new KeyboardState(Key.E);mode=2;shot=false;hold=now;log.AppendLine("E_RAY', 'keys=g.Item2=="@table"?new KeyboardState():new KeyboardState(Key.E);mode=g.Item2=="@table"?4:2;shot=false;hold=now;log.AppendLine("E_RAY')
p.write_text(s,encoding='utf-8')
