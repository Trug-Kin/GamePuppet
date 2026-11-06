"""Cong cu nhap lieu OS, ke thua native-input.py r90; khong them runner gameplay.
Small OS keyboard/mouse helper. A human/model chooses every direction from
the presented Game view; no route data, test driver, seeded saves or teleports.
"""
import argparse,ctypes as c,hashlib,json,math,os,subprocess,time,sys
from pathlib import Path
from DuongDanKiem import ket_qua,bang_chung,phien,dau_vao
R=Path(__file__).resolve().parents[1];sys.path.insert(0,str(R/'Tools/python-deps'))
from PIL import ImageGrab
p=argparse.ArgumentParser();p.add_argument('--folder',default=str(ket_qua(phien('player'))));p.add_argument('--media-folder');p.add_argument('--boot');p.add_argument('--enter',action='store_true');p.add_argument('--turn',type=float,default=0);p.add_argument('--pitch',type=float,default=0);p.add_argument('--walk',type=float,default=0);p.add_argument('--strafe',type=float,default=0);p.add_argument('--key');p.add_argument('--wait',type=float,default=.3);p.add_argument('--name',default='view');p.add_argument('--close',action='store_true');p.add_argument('--yaw',type=float);p.add_argument('--look-pitch',type=float);a=p.parse_args()
out=R/a.folder;out.mkdir(parents=True,exist_ok=True);media=Path(a.media_folder) if a.media_folder else bang_chung(out.relative_to(ket_qua('')).as_posix() if out.is_relative_to(ket_qua('')) else out.name);media=media if media.is_absolute() else R/media;media.mkdir(parents=True,exist_ok=True);save=out/'private-profile/journey-v48.json'
u=c.windll.user32;u.SetProcessDPIAware();u.GetForegroundWindow.restype=c.c_void_p
for n in ['SetForegroundWindow','IsWindowVisible','ShowWindow','GetClientRect','ClientToScreen','PostMessageW','GetWindowThreadProcessId']:getattr(u,n).argtypes=[c.c_void_p]+{'SetForegroundWindow':[],'IsWindowVisible':[],'ShowWindow':[c.c_int],'GetClientRect':[c.c_void_p],'ClientToScreen':[c.c_void_p],'PostMessageW':[c.c_uint,c.c_void_p,c.c_void_p],'GetWindowThreadProcessId':[c.POINTER(c.c_ulong)]}[n]
def key(k,down):u.keybd_event(k,u.MapVirtualKeyW(k,0),0 if down else 2,0)
def tap(k):key(k,True);time.sleep(.12);key(k,False);time.sleep(.2)
if a.boot:
 assert not (out/'launch.json').exists(),'Preserve each observed run'
 if save.exists():(out/'pre-test-save.json').write_bytes(save.read_bytes())
 exe=dau_vao(R/a.boot)/'ConRoi.exe'
 launchargs=[str(exe),'-screen-fullscreen','0','-screen-width','1280','-screen-height','720','-logFile',str(out/'Player.log')]
 # Finite final-startup check: DEFAULT_BOOT leaves the built-in product profile intact.
 if a.key!='DEFAULT_BOOT':launchargs+=['--r30-review-output',str(out/'private-profile')]
 else:a.key=None
 proc=subprocess.Popen(launchargs,cwd=exe.parent)
 launch={'pid':proc.pid,'build':a.boot,'binary':{str(f):hashlib.file_digest((exe.parent/f).open('rb'),'sha256').hexdigest() for f in ['ConRoi.exe','ConRoi_Data/Managed/Assembly-CSharp.dll','ConRoi_Data/level0']},'method':'Ordinary binary; native OS input, directions chosen from presented scene and in-game guidance; no engine test arguments/fixtures/teleports'}
 launch['arguments']=launchargs;launch['profile']='default product profile' if '--r30-review-output' not in launchargs else 'private test profile'
 (out/'launch.json').write_text(json.dumps(launch,indent=2));time.sleep(25)
launch=json.loads((out/'launch.json').read_text(encoding='utf-8-sig'));pid=launch['pid'];handles=[]
@c.WINFUNCTYPE(c.c_bool,c.c_void_p,c.c_void_p)
def enum(h,l):
 q=c.c_ulong();u.GetWindowThreadProcessId(h,c.byref(q));r=(c.c_long*4)();u.GetClientRect(h,c.byref(r))
 if q.value==pid:
  title=c.create_unicode_buffer(256);u.GetClassNameW(c.c_void_p(h),title,256)
  if title.value=='UnityWndClass' and r[2]>0 and r[3]>0:
   u.ShowWindow(h,9);handles.append(h)
 return True
for attempt in range(160 if a.boot else 1):
 u.EnumWindows(enum,0)
 if handles:break
 time.sleep(.5)
assert handles,'No presented game window';h=handles[-1]
u.SetWindowPos.argtypes=[c.c_void_p,c.c_void_p,c.c_int,c.c_int,c.c_int,c.c_int,c.c_uint]
u.ShowWindow(h,9);u.SetWindowPos(h,-1,20,20,0,0,1);time.sleep(.4)
def focused():
 q=c.c_ulong();u.GetWindowThreadProcessId(u.GetForegroundWindow(),c.byref(q));return q.value==pid
if not focused():
 u.ShowWindow(h,9);subprocess.run(['powershell','-NoProfile','-Command',f'$taskShell=New-Object -ComObject WScript.Shell; $taskShell.AppActivate({pid})'],capture_output=True)
 u.SetForegroundWindow(h);time.sleep(.6)
 if not focused():
  owner=u.GetWindowThreadProcessId(u.GetForegroundWindow(),None);caller=c.windll.kernel32.GetCurrentThreadId();attached=u.AttachThreadInput(caller,owner,True)
  try:u.SetForegroundWindow(h)
  finally:
   if attached:u.AttachThreadInput(caller,owner,False)
  time.sleep(.4)
 if not focused():
  r=(c.c_long*4)();o=(c.c_long*2)();u.GetClientRect(h,c.byref(r));u.ClientToScreen(h,c.byref(o));u.SetCursorPos(o[0]+r[2]//2,o[1]+r[3]//2);u.mouse_event(2,0,0,0,0);u.mouse_event(4,0,0,0,0);time.sleep(.3)
assert focused(),'Foreground unavailable; no movement sent'
if a.close:
 u.PostMessageW(h,0x10,0,0);time.sleep(3)
 if (out/'pre-test-save.json').exists():save.write_bytes((out/'pre-test-save.json').read_bytes())
 print('Closed own run and restored previous save');sys.exit(0)
if a.enter:tap(0x0D);time.sleep(4)
if a.yaw is not None or a.look_pitch is not None:
 tap(0x74);time.sleep(.3);aimstate=json.loads(save.read_text(encoding='utf-8'));aq=aimstate['rotation'];oldyaw=math.degrees(math.atan2(2*(aq['w']*aq['y']+aq['x']*aq['z']),1-2*(aq['y']**2+aq['x']**2)));cq=aimstate['cameraRotation'];oldpitch=math.degrees(2*math.atan2(cq['x'],cq['w']));a.turn=((a.yaw-oldyaw+180)%360-180) if a.yaw is not None else 0;a.pitch=(a.look_pitch-oldpitch) if a.look_pitch is not None else 0
if a.turn or a.pitch:u.mouse_event(1,round(a.turn/.1),round(a.pitch/.1),0,0);time.sleep(.25)
if a.key=='CLICK_QUIT':
 r=(c.c_long*4)();o=(c.c_long*2)();u.GetClientRect(h,c.byref(r));u.ClientToScreen(h,c.byref(o))
 u.SetCursorPos(o[0]+round(r[2]*795/1280),o[1]+round(r[3]*563/720));time.sleep(.15)
 u.mouse_event(2,0,0,0,0);time.sleep(.1);u.mouse_event(4,0,0,0,0);time.sleep(3)
 row={'utc':__import__('datetime').datetime.now(__import__('datetime').timezone.utc).isoformat(),'action':'OS click visible menu Quit at client 795,563 / 1280,720','pid':pid,'window_closed':not bool(u.IsWindowVisible(h))}
 with (out/'actions.jsonl').open('a',encoding='utf-8') as f:f.write(json.dumps(row)+'\n')
 print(json.dumps(row));sys.exit(0 if row['window_closed'] else 1)
if a.key:tap({'E':0x45,'P':0x50,'F5':0x74,'F6':0x75,'ESC':0x1B,'F8':0x77,'TAB':0x09,'SPACE':0x20,'ENTER':0x0D}.get(a.key,ord(a.key[0])))
for seconds,k in [(a.walk,0x57 if a.walk>0 else 0x53),(a.strafe,0x44 if a.strafe>0 else 0x41)]:
 if not seconds:continue
 assert abs(seconds)<=12,'Choose a direction again from Game view after at most 12 seconds'
 try:
  key(k,True);end=time.monotonic()+abs(seconds)
  while time.monotonic()<end:
   if not focused():raise RuntimeError('Focus lost during movement; released key')
   time.sleep(.05)
 finally:key(k,False)
time.sleep(a.wait);tap(0x74);time.sleep(.2)
d=json.loads(save.read_text(encoding='utf-8')) if save.exists() else {}
q=d.get('rotation',{});yaw=math.degrees(math.atan2(2*(q.get('w',1)*q.get('y',0)+q.get('x',0)*q.get('z',0)),1-2*(q.get('y',0)**2+q.get('x',0)**2)))
r=(c.c_long*4)();o=(c.c_long*2)();u.GetClientRect(h,c.byref(r));u.ClientToScreen(h,c.byref(o));ImageGrab.grab(bbox=(o[0],o[1],o[0]+r[2],o[1]+r[3])).save(media/(a.name+'.png'))
row={'utc':__import__('datetime').datetime.now(__import__('datetime').timezone.utc).isoformat(),'actions':vars(a),'foreground':focused(),'position':d.get('position'),'yaw':yaw,'cameraRotation':d.get('cameraRotation'),'region':d.get('region'),'nextRegion':d.get('nextRegion'),'mapRevision':d.get('mapLayoutRevision'),'screenshot':str(media/(a.name+'.png'))}
with (out/'actions.jsonl').open('a',encoding='utf-8') as f:f.write(json.dumps(row)+'\n')
print(json.dumps(row))
