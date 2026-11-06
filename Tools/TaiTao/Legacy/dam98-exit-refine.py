from pathlib import Path
p=Path('Test/Tam/dam98-close-stairs.py');s=p.read_text();s=s.replace("beam(vv,ff,pos(s,.3,2.170076),pos(s,end-.3,1.25),.027)\n for d in np.linspace(.3,end-.3,15):", "endRail=end-1.75 if tag.endswith('OUT') else end-.3\n lastTop=2.170076-.920076*(endRail-.3)/(end-.6)\n beam(vv,ff,pos(s,.3,2.170076),pos(s,endRail,lastTop),.027)\n for d in np.linspace(.3,endRail,15):")
p.write_text(s)
p=Path('Test/Tam/dam98-export.py');s=p.read_text();s=s.replace("rows=[];expected={}","selected=sys.argv[sys.argv.index('--')+1:] if '--' in sys.argv else []\nprior=json.loads((D/'export-reopen.json').read_text())['rows'] if selected else []\nprior_round=json.loads((D/'fbx-roundtrip.json').read_text()) if selected else []\nrows=[];expected={}")
s=s.replace("if o.type!='MESH' or not o.get('dam98_export'):continue", "if o.type!='MESH' or not o.get('dam98_export'):continue\n if selected and o.name not in selected:continue")
s=s.replace("(D/'export-reopen.json').write_text", "rows=[r for r in prior if r['name'] not in selected]+rows if selected else rows\n(D/'export-reopen.json').write_text")
s=s.replace("(D/'fbx-roundtrip.json').write_text(json.dumps(out,indent=2))", "out=[r for r in prior_round if r['key'] not in selected]+out if selected else out\n(D/'fbx-roundtrip.json').write_text(json.dumps(out,indent=2))")
p.write_text(s)
