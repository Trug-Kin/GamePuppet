exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
from PIL import Image,ImageDraw,ImageFont
M=Path('Test/BangChung/journey_v48/market_ground_r95');g=json.loads((D/'grading.json').read_text());im=Image.new('RGB',(1500,1200),'#f6f5f0');dr=ImageDraw.Draw(im);font=ImageFont.truetype('C:/Windows/Fonts/arial.ttf',18)
def px(p):return (60+(820-p[0])*5,100+(p[1]-340)*7)
for name,col in [('Surface_176','#dfe8cd'),('Surface_104','#5d6874'),('Surface_173','#777a80'),('Surface_134','#4b6779'),('Surface_153','#c3a574')]:
 r=next(r for r in g['objects'] if r['path'].endswith('/'+name));a=np.load(D/(r['key']+'-graded.npz'));q=a['v'][a['tri']];cc=q.mean(1);pick=(cc[:,0]>640)&(cc[:,0]<820)&(cc[:,2]>340)&(cc[:,2]<485)
 for t in q[pick]:dr.polygon([px(p[[0,2]]) for p in t],fill=col)
for name,p in [('I2 (714.94,459.48)',[714.94,459.48]),('Clock (746,422)',[746,422]),('Gate east foot',[716.7,412.5]),('Gate west foot',[735.7,416.9]),('EastShop frontage',[711,370]),('P02 splitter search area',[701,404])]:
 x,y=px(p);dr.ellipse((x-5,y-5,x+5,y+5),fill='#e6322a');dr.text((x+8,y),name,font=font,fill='black',stroke_fill='white',stroke_width=2)
for x in range(640,821,20):dr.text(px([x,338]),str(x),font=font,fill='black')
for z in range(340,486,20):dr.text((10,px([820,z])[1]),str(z),font=font,fill='black')
dr.text((25,15),'R95 actual road/footway footprints; North=-Z, East=-X. Authoring units.',font=font,fill='black')
dr.text((1040,100),'P02: I2 foreground, clock\nin planted median, EastShop\nat lake-side branch.\n\nSmall P02 splitter is NOT\nI3 market-statue circle.\n\nRed search marker is a\nsearch window, NOT a\nsurveyed coordinate.\n\nNo separate splitter\nobject is identified by\nthis footprint audit alone.',font=font,fill='black')
im.save(M/'m2-actual-footprints.png')
(D/'m2-attribution.json').write_text(json.dumps(dict(status='OPEN_PHOTO_TO_OBJECT_CORRESPONDENCE',photo='P02',excluded_false_equivalences=['I2 foreground island','I3 statue circle','M1 planted median'],locked_landmarks=dict(clock=[746,422],gate_east=[716.7,412.5],gate_west=[735.7,416.9],eastshop=[711,370]),search_window_xz=[680,390,720,425],actual_floor_layers=['Surface_176','Surface_134','Surface_173','Surface_153','Context_124','Context_116','Restored_028','Context_136'],conclusion='No independently bounded splitter mesh is established. These existing layers are graded by footprint; no island selected/flattened from label alone, no island invented.'),indent=2))
