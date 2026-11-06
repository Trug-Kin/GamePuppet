"""Existing Unity bridge client; select live ConRoi connection after Editor reload."""
import json,uuid,sys,time,os
from pathlib import Path
folder=Path('C:/Users/kaito/.unity/mcp/connections')
live=[]
for p in folder.glob('bridge-7122e2e3-*.json'):
 try:
  pid=int(p.stem.split('-')[-1]);import ctypes
  handle=ctypes.windll.kernel32.OpenProcess(0x1000,False,pid)
  if handle:ctypes.windll.kernel32.CloseHandle(handle);live.append(p)
 except Exception:pass
if not live:raise RuntimeError('No live ConRoi Editor bridge')
p=max(live,key=lambda p:p.stat().st_mtime);c=json.loads(p.read_text());print('Bridge',p.name,flush=True)
pipe=open(c['connection_path'],'r+b',buffering=0);pipe.readline();rid=uuid.uuid4().hex
pipe.write((json.dumps({'type':'Unity_RunCommand','params':{'Code':Path(sys.argv[1]).read_text(encoding='utf-8-sig'),'Title':'R104 scoped prototype work'},'requestId':rid})+'\n').encode())
while True:
 line=pipe.readline()
 if not line:raise RuntimeError('Bridge closed before result')
 obj=json.loads(line)
 if obj.get('requestId')==rid and 'command_in_progress' not in line.decode():
  Path(sys.argv[2]).write_bytes(line);result=obj.get('result',{});print(json.dumps(result.get('data',{}),ensure_ascii=False)[:5500]);sys.exit(0 if result.get('success') else 2)
