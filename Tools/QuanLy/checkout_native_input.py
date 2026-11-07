"""Scoped native Game-window input for the authorized independent checkout test."""
import ctypes, json, sys, time
from ctypes import wintypes as W
from pathlib import Path
from PIL import ImageGrab
u=ctypes.windll.user32
u.SetProcessDPIAware()
pid=29104
wins=[]
CB=ctypes.WINFUNCTYPE(W.BOOL,W.HWND,W.LPARAM)
def visit(h,l):
    p=W.DWORD();u.GetWindowThreadProcessId(h,ctypes.byref(p))
    b=ctypes.create_unicode_buffer(512);u.GetWindowTextW(h,b,512)
    if p.value==pid and b.value=='Game' and u.IsWindowVisible(h): wins.append(h)
    return True
u.EnumWindows(CB(visit),0)
assert len(wins)==1, wins
h=wins[0];u.ShowWindow(h,5);u.SetForegroundWindow(h);time.sleep(.5)
assert u.GetForegroundWindow()==h, 'Refuse input: Game not foreground'
r=W.RECT();u.GetWindowRect(h,ctypes.byref(r))
print(json.dumps(dict(pid=pid,hwnd=h,rect=[r.left,r.top,r.right,r.bottom])))
if len(sys.argv)>2:
    x,y=int(sys.argv[1]),int(sys.argv[2]);assert r.left<x<r.right and r.top<y<r.bottom
    u.SetCursorPos(x,y);u.mouse_event(2,0,0,0,0);time.sleep(.12);u.mouse_event(4,0,0,0,0);time.sleep(.5)
out=Path('C:/ConRoi_CheckoutTest/Test/Output/CleanCheckout/native-game.png')
ImageGrab.grab(bbox=(r.left,r.top,r.right,r.bottom)).save(out)
print(out)
