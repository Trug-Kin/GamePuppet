from pathlib import Path
p=Path('Test/Tam/i2-motion.cs.txt');s=p.read_text(encoding='utf-8')
s=s.replace('(InputActionMap)typeof(DieuKhienDiChuyen).GetField("gameplay",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(s.player)','InputSystem.ListEnabledActions().First(a=>a.name=="Move"&&a.actionMap.name=="Gameplay").actionMap')
s=s.replace('var settings=InputSystem.settings;', 's.SavePathOverride=Path.GetFullPath("Test/KetQua/journey_v48/i2_r93_complete/private-local.json");s.Load();s.SetMenu(false);s.SetSupplementInspection(false);s.ClosePhone();var settings=InputSystem.settings;')
p.write_text(s,encoding='utf-8')
