import json,uuid,sys,time
from pathlib import Path
connection=Path('C:/Users/kaito/.unity/mcp/connections/bridge-7122e2e3-13860.json')
end=time.monotonic()+40
while not connection.exists() and time.monotonic()<end:time.sleep(.25)
c=json.loads(connection.read_text())
p=open(c['connection_path'],'r+b',buffering=0);p.readline();rid=uuid.uuid4().hex
p.write((json.dumps({'type':'Unity_RunCommand','params':{'Code':Path(sys.argv[1]).read_text(encoding='utf-8-sig'),'Title':'R93 scoped map review'},'requestId':rid})+'\n').encode())
while True:
 line=p.readline()
 if not line:break
 obj=json.loads(line)
 if obj.get('requestId')==rid and 'command_in_progress' not in line.decode():
  Path(sys.argv[2]).write_bytes(line);print(line.decode()[:6000]);break





