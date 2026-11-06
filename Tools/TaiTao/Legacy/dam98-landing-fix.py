from pathlib import Path
p=Path('Test/Tam/dam98-close-stairs.py');s=p.read_text();s=s.replace("end=shore(s);start=.7;run=(end-2.3)/8;section=", "end=shore(s);start=4.1 if tag=='GATE' else .7;run=(end-1.6-start)/8;section=")
s=s.replace("def hh(d):return 1.140076-.920076/8*max(0,min(8,math.ceil((d-.7)/((end-2.3)/8)-1e-7)))", "def hh(d):\n  start=4.1 if tag.startswith('GATE') else .7\n  return 1.140076-.920076/8*max(0,min(8,math.ceil((d-start)/((end-1.6-start)/8)-1e-7)))")
p.write_text(s)
