from pathlib import Path
s=Path('Test/Tam/ground-r93-south-study.py').read_text().replace('Ground_I2_South_Transition_Study_R93.blend','Ground_I2_Inner_Transition_Study_R93.blend').replace('r<=13.2','r<=12').replace('(r-13.2)/.8','(r-12)/1.2').replace('>13.2)','>12)').replace('south-transition-study.json','inner-transition-study.json').replace('I2-south-study-plan.png','I2-inner-study-plan.png').replace('inner_radius=13.2','inner_radius=12')
s=s.replace('13.2..24','12..24; visible road boundary at13.2; below deck at inner edge')
Path('Test/Tam/ground-r93-inner-study.py').write_text(s)
for n in ['reopen','quality','measure']:
 s=Path('Test/Tam/ground-r93-south-'+n+'.py').read_text().replace('Ground_I2_South_Transition_Study_R93.blend','Ground_I2_Inner_Transition_Study_R93.blend').replace('south-transition-study.json','inner-transition-study.json').replace('south-study','inner-study')
 Path('Test/Tam/ground-r93-inner-'+n+'.py').write_text(s)
