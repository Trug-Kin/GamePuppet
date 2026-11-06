# Gửi lệnh qua bridge GUI Editor hiện có, chờ ACK; không tự mở Unity.
"""Request existing GUI Editor actions, waiting for completion; never starts Unity."""
import json,time,uuid,sys
from DuongDanKiem import ket_qua
from pathlib import Path
root=Path(__file__).resolve().parents[1];folder=ket_qua('W_route_Unity_v30_20261016/editor_live')
def read(name):
 try:return json.loads((folder/name).read_text(encoding='utf-8-sig'))
 except (OSError,ValueError):return {}
def wait(predicate,timeout):
 deadline=time.monotonic()+timeout
 while time.monotonic()<deadline:
  if predicate():return
  time.sleep(.5)
 raise TimeoutError('Editor timeout')
for action in sys.argv[1:]:
 wait(lambda:not read('heartbeat.json').get('is_playing',True),600)
 nonce=str(uuid.uuid4());tmp=folder/'command.next.json';tmp.write_text(json.dumps(dict(action=action,nonce=nonce)),encoding='utf-8');tmp.replace(folder/'command.json')
 wait(lambda:read('ack.json').get('nonce')==nonce,180);ack=read('ack.json')
 if ack.get('status')!='EXECUTED':raise RuntimeError(ack)
 print(action,'ACK',flush=True)
 if action.startswith('run_'):
  wait(lambda:read('heartbeat.json').get('is_playing',False),120)
  wait(lambda:not read('heartbeat.json').get('is_playing',True),900)
  print(action,'PLAY_FINISHED',flush=True)
