from pathlib import Path
p=Path('Test/Tam/ground-r93-inventory.cs.txt').read_text()
p=p.replace('var rows=new List<Row>();var pick=new List<MeshFilter>();','var rows=new List<Row>();var pick=new List<MeshFilter>();')
p=p.replace('if(wb.max.x<580||wb.min.x>1000||wb.max.z<70||wb.min.z>620)continue;','if(wb.max.x<450||wb.min.x>870||wb.max.z<40||wb.min.z>610)continue;')
p=p.replace('if(env&&ground&&broad&&wb.min.y<25&&m.isReadable)pick.Add(f);','if(env&&ground&&broad&&wb.min.y<25&&m.isReadable&&(wb.max.x<580||wb.min.z>620||wb.max.z<70))pick.Add(f);')
p=p.replace('inventory.json','expanded-inventory.json').replace('scene-ground.meshbin','expanded-ground.meshbin').replace('scene-components.txt','expanded-components.txt')
Path('Test/Tam/ground-r93-expanded.cs.txt').write_text(p)
