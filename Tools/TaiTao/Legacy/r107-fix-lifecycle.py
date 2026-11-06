from pathlib import Path
p=Path('Test/Tam/r107-map-lifecycle.cs.txt')
s=p.read_text(encoding='utf-8')
s=s.replace('var kb=InputSystem.AddDevice<Keyboard>();','var settings=InputSystem.settings;string prior=JsonUtility.ToJson(settings);settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;var kb=InputSystem.AddDevice<Keyboard>();')
s=s.replace('InputSystem.RemoveDevice(kb);EditorApplication.update-=f;','InputSystem.RemoveDevice(kb);JsonUtility.FromJsonOverwrite(prior,settings);EditorApplication.update-=f;')
s=s.replace('var p=s.movement.transform.position;var yaw=', 'var origin=s.movement.transform.position;var p=origin;var yaw=')
s=s.replace('check(s.movement.controller.isGrounded,"Grounded', 'check(Vector3.Distance(origin,s.movement.transform.position)>.5f,"Actual diagonal controller displacement");check(s.movement.controller.isGrounded,"Grounded')
Path('Test/KetQua/investigation_r107/map-lifecycle-superseded.txt').write_text(Path('Test/KetQua/investigation_r107/map-lifecycle.txt').read_text(encoding='utf-8'),encoding='utf-8')
p.write_text(s,encoding='utf-8')
