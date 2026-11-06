from pathlib import Path
p=Path('Test/Tam/r107-onboarding-route.cs.txt');s=p.read_text()
s=s.replace('var map=InputSystem.ListEnabledActions().First(a=>a.name=="Move"&&a.actionMap.name=="Gameplay").actionMap;','kb.MakeCurrent();mouse.MakeCurrent();var map=(InputActionMap)typeof(DieuKhienDiChuyen).GetField("gameplay",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(s.movement);')
p.write_text(s)
