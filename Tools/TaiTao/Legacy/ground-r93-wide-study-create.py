from pathlib import Path
s=Path('Test/Tam/ground-r93-south-study.py').read_text()
s=s.replace("('Surface_153','Context_116')","('Surface_153','Context_136')").replace("('Surface_129','Surface_173','Context_111','Context_136')","('Surface_129','Surface_173','Context_111')").replace('list(col.objects)','list(col.all_objects)').replace('for o in col.objects','for o in col.all_objects')
s=s.replace('Ground_I2_South_Transition_Study_R93.blend','Ground_I2_Wide_Transition_Study_R93.blend').replace('r>=24','r>=32').replace('(24-r)/3','(32-r)/8').replace('(24-13.35)','(32-13.35)').replace('<24)','<32)').replace('outer_radius=24','outer_radius=32').replace('south-transition-study.json','wide-transition-study.json').replace('I2-south-study-plan.png','I2-wide-study-plan.png')
Path('Test/Tam/ground-r93-wide-study.py').write_text(s)
for n in ['reopen','quality','measure']:
 s=Path('Test/Tam/ground-r93-south-'+n+'.py').read_text().replace('Ground_I2_South_Transition_Study_R93.blend','Ground_I2_Wide_Transition_Study_R93.blend').replace('south-transition-study.json','wide-transition-study.json').replace('south-study','wide-study').replace('(r<24)','(r<32)').replace('(rr<24)','(rr<32)').replace('list(col.objects)','list(col.all_objects)').replace('for o in col.objects','for o in col.all_objects')
 Path('Test/Tam/ground-r93-wide-'+n+'.py').write_text(s)
