from pathlib import Path
p=Path('Test/Tam/i2-motion.cs.txt');s=p.read_text(encoding='utf-8')
s=s.replace('double begin=EditorApplication.timeSinceStartup,last=0;', '''var mouse=InputSystem.AddDevice<Mouse>("I2ScopedMouse");var map=(InputActionMap)typeof(DieuKhienDiChuyen).GetField("gameplay",System.Reflection.BindingFlags.NonPublic|System.Reflection.BindingFlags.Instance).GetValue(s.player);var originalDevices=map.devices;map.devices=new InputDevice[]{kb,mouse};Physics.SyncTransforms();var hit=Physics.RaycastAll(new Vector3(714.94f,8,486),Vector3.down,20,8,QueryTriggerInteraction.Ignore).Where(h=>h.normal.y>.7f).OrderByDescending(h=>h.point.y).First();var cc0=s.player.controller;s.Place(new Vector3(714.94f,hit.point.y-s.player.transform.TransformVector(cc0.center).y+cc0.height*.5f+cc0.skinWidth*.25f,486),Quaternion.Euler(0,180,0));s.player.GetComponentInChildren<DieuKhienGocNhin>().RestorePitch(8);double begin=EditorApplication.timeSinceStartup,last=0;''')
s=s.replace('using System.Text;','using System.Text;using System.Linq;')
s=s.replace('InputSystem.QueueStateEvent(kb,k);','InputSystem.QueueStateEvent(kb,k);InputSystem.QueueStateEvent(mouse,new MouseState());')
s=s.replace('InputSystem.RemoveDevice(kb);','map.devices=originalDevices;InputSystem.RemoveDevice(kb);InputSystem.RemoveDevice(mouse);')
s=s.replace('flags="+cc.collisionFlags','yaw="+s.player.transform.eulerAngles.y+" input="+s.player.LastMovementInput+" flags="+cc.collisionFlags')
p.write_text(s,encoding='utf-8')
# Keep first, invalidly located route as diagnostic history.
import shutil
for a,b in [('Test/BangChung/journey_v48/i2_r93_complete/i2-controller.mp4','Test/BangChung/journey_v48/i2_r93_complete/historical-off-target-controller.mp4'),('Test/KetQua/journey_v48/i2_r93_complete/controller-motion.txt','Test/KetQua/journey_v48/i2_r93_complete/historical-off-target-motion.txt')]:
 shutil.copy2(a,b)
