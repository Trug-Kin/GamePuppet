from pathlib import Path
p=Path('Test/Tam/investigation103-input-edges.cs.txt');s=p.read_text()
s=s.replace('var kb=InputSystem.AddDevice<Keyboard>', 'var settings=InputSystem.settings;if(AssetDatabase.Contains(settings))throw new Exception("Runtime input settings required");string prior=JsonUtility.ToJson(settings);settings.editorInputBehaviorInPlayMode=InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;settings.backgroundBehavior=InputSettings.BackgroundBehavior.IgnoreFocus;var kb=InputSystem.AddDevice<Keyboard>')
s=s.replace('InputSystem.RemoveDevice(kb);InputSystem.RemoveDevice(mouse);','InputSystem.RemoveDevice(kb);InputSystem.RemoveDevice(mouse);JsonUtility.FromJsonOverwrite(prior,settings);')
p.write_text(s)
