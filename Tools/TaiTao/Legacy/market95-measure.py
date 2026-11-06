import sys,json
from pathlib import Path
sys.path.insert(0,'Tools/python-deps')
import numpy as np,shapely
from shapely import STRtree
from PIL import Image,ImageDraw,ImageFont
code=Path('Test/Tam/market95-grade.py').read_text();exec(code[:code.index('data=')])
M=Path('Test/BangChung/journey_v48/market_ground_r95');g=json.loads((D/'grading.json').read_text());out=[];baseline={r['path']:r for r in json.loads((D/'geometry-inventory.json').read_text())};visibility={r['path']:r['render'] for r in json.loads((D/'inventory.json').read_text())['rows']}
sections=[('longitudinal-branch-east',[746,432],[781,215]),('longitudinal-branch-west',[719,432],[744,215]),('apron-Langfarm-cross',[719,300],[740,305]),('apron-Langfarm-along',[730,282],[730,318]),('plaza-X',[700,191.167],[850,191.167]),('plaza-Z',[783.948,145],[783.948,240])]
for z in [165,220,255,300,345,390,430]:sections.append(('full-width-z'+str(z),[695,z],[850,z]))
for x in [705,750,830]:sections.append(('north-boundary-x'+str(x),[x,58.8],[x,200]))
for z in [70,125]:sections.append(('north-full-width-z'+str(z),[650,z],[860,z]))
sections += [('clock-long',[746,410],[746,435]),('clock-cross',[730,422],[762,422]),('M2-search-east',[696,400],[720,410]),('M2-search-branch',[684,380],[710,430])]
sectiondefs=[]
for name,p,q in sections:
 p,q=np.array(p),np.array(q);length=np.linalg.norm(q-p);n=int(np.ceil(length/.25))+1;sectiondefs.append(dict(name=name,xz=(p+(q-p)*np.linspace(0,1,n)[:,None]).tolist(),length=float(length),direction=(q-p).tolist()))
for rad in [21,24,27]:
 a=np.deg2rad(np.arange(0,361,2));p=np.c_[783.948+rad*np.cos(a),191.167+rad*np.sin(a)];sectiondefs.append(dict(name='market-ring-r'+str(rad),xz=p.tolist(),length=float(2*np.pi*rad),direction='clockwise/counterclockwise circular sampling'))
sectiondata=[]
for row in g['objects']:
 name=row['path'].split('/')[-1]
 if '/MarketIsland_R62/' in row['path'] or row['key'].startswith('M95_F'):continue
 a=np.load(D/(row['key']+'-graded.npz'));v=a['v'];t=a['tri'];q=v[t];nn=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);c=q.mean(1);w=weight(c[:,[0,2]]);grade=np.linalg.norm(nn[:,[0,2]],axis=1)/np.maximum(abs(nn[:,1]),1e-15)*100;floors=a['is_floor'] if 'is_floor' in a else np.ones(len(t),bool);valid=(abs(nn[:,1])>1e-7)&(w>.99)&floors;transition=(abs(nn[:,1])>1e-7)&(w>1e-3)&floors;ix=np.flatnonzero(transition);worst=ix[np.argsort(grade[ix])[-8:]];out.append(dict(path=row['path'],core_face_max_grade=float(grade[valid].max()) if valid.any() else None,transition_max_grade=float(grade[transition].max()) if transition.any() else None,worst=[dict(p=c[i].tolist(),grade=float(grade[i]),weight=float(w[i])) for i in worst],core_faces=int(valid.sum())))
 for state in ['before','after']:
  if state=='before':
   original=np.load(D/(baseline[row['path']]['cache']+'.npz'));sam=Sampler(original['v'],original['tri'],np.zeros((len(original['tri']),3,2)))
  else:sam=Sampler(v,t,a['uv'])
  for s in sectiondefs:
   pp=np.array(s['xz']);hp,_,_,_=sam.sample(pp);h=hp[:,1];valid=sam.last_valid;h[~valid]=np.nan;ds=np.linalg.norm(np.diff(pp,axis=0),axis=1);gr=np.diff(h)/ds*100
   sectiondata.append(dict(layer=row['path'],state=state,section=s['name'],heights=[float(y) if np.isfinite(y) else None for y in h],max_sampled_grade=float(np.nanmax(abs(gr))) if np.isfinite(gr).any() else None))
(D/'face-grade-audit.json').write_text(json.dumps(out,indent=2));(D/'sections.json').write_text(json.dumps(sectiondefs));(D/'section-layers.json').write_text(json.dumps(sectiondata));print('GRADE_AUDIT',[(r['path'].split('/')[-1],r['core_face_max_grade'],r['transition_max_grade']) for r in out],flush=True)
# High level design plan, actual changed vertices, no claim based on controller tracks.
im=Image.new('RGB',(1500,1700),'#f1f3f5');dr=ImageDraw.Draw(im);font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',19)
def px(p):return (100+(870-p[0])*5,90+(p[1]-120)*4)
for row in g['objects']:
 if '/MarketIsland_R62/' in row['path'] or 'Context_' in row['path']:continue
 a=np.load(D/(row['key']+'-graded.npz'));v=a['v'];sel=(v[:,0]>650)&(v[:,0]<870)&(v[:,2]>130)&(v[:,2]<465);changed=sel&(abs(v[:,1]-a['before'][:,1])>.001)
 for p in v[sel][::max(1,int(sel.sum()/20000))]:x,y=px(p[[0,2]]);dr.point((x,y),fill='#9daab2')
 for p in v[changed]:x,y=px(p[[0,2]]);dr.point((x,y),fill='#d4452d')
dr.line([px(p) for p in g['polygon']],fill='#336655',width=3)
for label,p in [('I2 r94 preserved',(714.94,459.48)),('Both gate branches / planted median',(745,390)),('TTC / Langfarm apron',(727,300)),('Market ring / plaza',(783.948,191.167)),('Split / service branches',(775,265))]:dr.text(px(p),label,font=font,fill='black',stroke_width=2,stroke_fill='white')
dr.text((25,10),'R95 GAME GRADE DESIGN - NOT FIELD ELEVATIONS; red = changed vertices',font=font,fill='black');dr.text((1210,70),'N = -Unity Z\nE = -Unity X\n\nRoad/yard/footway:\nfull width design.\n\nBridge, dam, banks\nand structural steps\nexcluded.\n\nController coverage\npending integration.',font=font,fill='black');dr.line((1250,250,1250,130),fill='black',width=4);dr.text((1240,105),'N',font=font,fill='black');im.save(M/'design-scope-and-change.png')
# Same-place section plots retain layer gaps; no highest-layer shortcut.
for s in sectiondefs:
 if s['name'] not in ['apron-Langfarm-cross','plaza-X','longitudinal-branch-east','full-width-z390']:continue
 layers=[r for r in sectiondata if r['section']==s['name']];vals=[y for r in layers for y in r['heights'] if y is not None];lo,hi=min(vals),max(vals);lo-=.2;hi+=.2;im=Image.new('RGB',(1400,650),'white');dr=ImageDraw.Draw(im);dr.text((30,15),s['name']+' length='+str(round(s['length'],3))+' authored m; direction='+str(s['direction']),font=font,fill='black')
 for j,r in enumerate(layers):
  col='#bb4444' if r['state']=='before' else '#176d40';line=[]
  for i,y in enumerate(r['heights']):
   if y is None:
    if len(line)>1:dr.line(line,fill=col,width=2)
    line=[];continue
   line.append((80+1240*i/(len(r['heights'])-1),560-(y-lo)/(hi-lo)*470))
  if len(line)>1:dr.line(line,fill=col,width=2)
 for i in range(6):y=lo+(hi-lo)*i/5;yy=560-i/5*470;dr.text((10,yy),str(round(y,2)),font=font,fill='black');dr.line((75,yy,1320,yy),fill='#dddddd',width=1)
 dr.text((100,590),'red=before / green=after; Y authored m, X distance 0..'+str(round(s['length'],2))+' m; independent layers',font=font,fill='black');im.save(M/(s['name']+'-before-after.png'))
