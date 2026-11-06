from pathlib import Path
p=Path('Test/Tam/r107-onboarding-route.cs.txt');s=p.read_text()
s=s.replace('var map=(InputActionMap)typeof(DieuKhienDiChuyen).GetField("gameplay",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(s.movement);var devices=map.devices;map.devices=new InputDevice[]{kb,mouse};','InputActionMap map=null;UnityEngine.InputSystem.Utilities.ReadOnlyArray<InputDevice>? devices=null;')
s=s.replace('if(now>hold&&s.Page==""&&leg<goals.Length){','if(!s.Modal&&map==null){map=InputSystem.ListEnabledActions().First(a=>a.name=="Move"&&a.actionMap.name=="Gameplay").actionMap;devices=map.devices;map.devices=new InputDevice[]{kb,mouse};}\nif(now>hold&&s.Page==""&&leg<goals.Length){')
s=s.replace('map.devices=devices;','if(map!=null)map.devices=devices;')
p.write_text(s)
