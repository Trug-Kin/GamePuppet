from pathlib import Path
p=Path('Test/Tam/market95-grade.py');s=p.read_text();s=s.replace('ss((p[:,0]-450)/190)','ss((p[:,0]-450)/254)').replace('ss((p[:,1]-370)/20)','ss((p[:,1]-300)/90)').replace('ss((p[:,1]-110)/45)','ss((p[:,1]-70)/95)').replace('1-ss((p[:,1]-435)/110)','1-ss((p[:,1]-405)/40)');p.write_text(s)
p=Path('Test/Tam/market95-preserve-boundary.py');s=p.read_text().replace('box(450,110,1000,545)','box(450,70,1000,445)');p.write_text(s)
