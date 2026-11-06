import ctypes,json
u=ctypes.windll.user32;rows=[]
@ctypes.WINFUNCTYPE(ctypes.c_bool,ctypes.c_void_p,ctypes.c_void_p)
def cb(h,l):
 b=ctypes.create_unicode_buffer(1000);u.GetWindowTextW(h,b,1000)
 if u.IsWindowVisible(h) and b.value:rows.append(dict(handle=h,title=b.value))
 return True
u.EnumWindows(cb,0);print(json.dumps(rows,ensure_ascii=False))
