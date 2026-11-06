from pathlib import Path
p=Path('Test/Tam/i2-motion.cs.txt');s=p.read_text(encoding='utf-8');s=s.replace('714.94f,8,486','714.94f,8,475').replace('*.25f,486)','*.25f,475)')
a=s.index('if(t>=3&&');b=s.index('InputSystem.QueueStateEvent(kb,k);',a)
s=s[:a]+'''if(t>=2&&t<2.5)k=new KeyboardState(Key.W);else if(t>=3.5&&t<4)k=new KeyboardState(Key.S);else if(t>=5&&t<6)k=new KeyboardState(Key.W,Key.A);else if(t>=7&&t<8)k=new KeyboardState(Key.S,Key.D);else if(t>=9&&t<10)k=new KeyboardState(Key.W,Key.D);else if(t>=11&&t<12)k=new KeyboardState(Key.S,Key.A);else if(t>=13&&t<14)k=new KeyboardState(Key.A);else if(t>=15&&t<17)k=new KeyboardState(Key.D);else if(t>=18&&t<19)k=new KeyboardState(Key.A);'''+s[b:]
s=s.replace('if(t<51)','if(t<21)').replace('controller-motion.txt','seam-motion.txt').replace('51s controller route','21s seam controller route')
Path('Test/Tam/i2-seam-motion.cs.txt').write_text(s,encoding='utf-8')
p=Path('Test/Tam/i2-record.py');s=p.read_text();s=s.replace("probe='--probe' in sys.argv","probe='--probe' in sys.argv\nseam='--seam' in sys.argv");s=s.replace("'Test/Tam/i2-motion.cs.txt'","'Test/Tam/i2-seam-motion.cs.txt' if seam else 'Test/Tam/i2-motion.cs.txt'").replace("'motion-response.json'","'seam-motion-response.json' if seam else 'motion-response.json'").replace("else '53'","else ('23' if seam else '53')").replace("else 'i2-controller.mp4'","else ('i2-seam-controller.mp4' if seam else 'i2-controller.mp4')").replace("else 'controller'","else ('seam-controller' if seam else 'controller')")
p.write_text(s,encoding='utf-8')
