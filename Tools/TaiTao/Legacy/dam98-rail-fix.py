from pathlib import Path
p=Path('Test/Tam/dam98-close-stairs.py');s=p.read_text();s=s.replace("ds=np.linspace(.3,end-.3,24)\n for d,e in zip(ds,ds[1:]):beam(vv,ff,pos(s,d,ramp(d)+1.03),pos(s,e,ramp(e)+1.03),.027)\n for d in np.linspace(.3,end-.3,15):beam(vv,ff,pos(s,d,hh(d)-.012),pos(s,d,ramp(d)+1.05),.023)","beam(vv,ff,pos(s,.3,2.170076),pos(s,end-.3,1.25),.027)\n for d in np.linspace(.3,end-.3,15):\n  top=2.170076-.920076*(d-.3)/(end-.6)\n  beam(vv,ff,pos(s,d,hh(d)-.012),pos(s,d,top+.004),.023)")
p.write_text(s)
