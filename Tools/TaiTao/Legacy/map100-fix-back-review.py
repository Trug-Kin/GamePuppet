from pathlib import Path
import shutil
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/map_r99_closeout';M=R/'Test/BangChung/journey_v48/map_r99_closeout';A=D/'attempt-back-input-component';A.mkdir(exist_ok=True)
for name in ['back-diagonal-response.json','capture-back-diagonal.json','capture-back-diagonal-live.txt']:
 p=D/name
 if p.exists():shutil.move(str(p),str(A/name))
p=M/'bridge-backward-diagonal.mp4'
if p.exists():shutil.move(str(p),str(A/p.name))
p=R/'Test/Tam/map100-back-diagonal.cs.txt';s=p.read_text();s=s.replace('using System.IO;','using System.IO;using System.Linq;').replace('var kb=InputSystem.AddDevice<Keyboard>("ScopedMapBackDiagonal");var map=s.player.GetComponent<UnityEngine.InputSystem.PlayerInput>().actions.FindActionMap("Gameplay");','foreach(var old in InputSystem.devices.Where(x=>x.name.StartsWith("ScopedMapBackDiagonal")).ToArray())InputSystem.RemoveDevice(old);var map=InputSystem.ListEnabledActions().First(a=>a.name=="Move"&&a.actionMap.name=="Gameplay").actionMap;var kb=InputSystem.AddDevice<Keyboard>("ScopedMapBackDiagonal");');p.write_text(s)
print('Only test input lookup corrected to same route API; failed diagnostic kept, no product edits.')
