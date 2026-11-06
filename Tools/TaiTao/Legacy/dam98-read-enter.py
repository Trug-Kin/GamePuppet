import json
for name in ['bank97-final-play-response.json','bank97-boundary-stop-response.json']:
 d=json.load(open('Test/Tam/'+name)); print(name,str(d.get('result',{}).get('data',{}).get('localFixedCode',''))[:3200])
