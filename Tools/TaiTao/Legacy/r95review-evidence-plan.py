from pathlib import Path
import sys,json,re
sys.path.insert(0,'Tools/python-deps')
import numpy as np
import matplotlib
matplotlib.use('Agg')
import matplotlib.pyplot as plt
from matplotlib.collections import PolyCollection
from matplotlib.patches import Rectangle
D=Path('Test/KetQua/journey_v48/r95_visual_review');M=Path('Test/BangChung/journey_v48/r95_visual_review');B=Path('Test/KetQua/journey_v48/market_ground_r95')
fig,ax=plt.subplots(figsize=(10,14),dpi=135)
for key in ['M95_002','M95_019','M95_020','M95_021','M95_Service0']:
 a=np.load(B/(key+'-source.npz'));q=a['v'][a['tri']][:,:,[0,2]];cent=q.mean(1);sel=(cent[:,0]>610)&(cent[:,0]<840)&(cent[:,1]>140)&(cent[:,1]<565)
 ax.add_collection(PolyCollection(-q[sel],facecolors='#dededb',edgecolors='none',rasterized=True))
route=json.loads((D/'route-plan.json').read_text());print(type(route),str(route)[:120])
text=Path('Test/Tam/r95review-route-fixed.cs.txt').read_text();points=np.array([(float(a),float(b)) for a,b in re.findall(r'new Vector2\(([\d.]+)f,([\d.]+)f\)',text)])
points=np.vstack([[650,528],points]);ax.plot(-points[:,0],-points[:,1],color='#197c66',linewidth=2,label='Planned controller route (see motion log for actual completion)')
ax.add_patch(Rectangle((-755,-505),75,70,fill=False,edgecolor='#ba542a',linewidth=2,label='Winding fix envelope; no height changes'))
ax.add_patch(Rectangle((-720,-425),40,35,fill=False,edgecolor='#b36aa3',linestyle='--',label='M2 attribution window: UNVERIFIED'))
for name,x,z in [('Bridge fixture',650,528),('Bridge-end fall / fixed',701.44,480.44),('I2 fountain',714.94,459.48),('Gate / clock',746,422),('Langfarm',724,306),('Market plaza / I3 (not M2)',783.948,191.167)]:
 ax.scatter(-x,-z,s=25,color='#253a54');ax.annotate(name,(-x,-z),xytext=(7,7),textcoords='offset points',fontsize=8)
ax.annotate('N',xy=(-625,-165),xytext=(-625,-200),arrowprops=dict(arrowstyle='->'),ha='center');ax.set_xlim(-840,-610);ax.set_ylim(-565,-140);ax.set_aspect('equal');ax.set_xlabel('East = -Unity X (authoring m)');ax.set_ylabel('North = -Unity Z (authoring m)');ax.set_title('r95 ground review / r96 winding repair\nSource footprint + route; not a field survey or whole-map approval');ax.legend(loc='lower left',fontsize=7);ax.grid(alpha=.2);fig.tight_layout();fig.savefig(M/'review-route-and-fix-scope.png');plt.close(fig)
# Same route sampling before/after, first bridge-to-I2 approach only.
def samples(file,mode):
 out=[]
 for line in (D/file).read_text().splitlines():
  leg,p,back,hits=line.split('\t')
  if int(leg)>5 or back!=mode:continue
  x,z=map(float,p.strip('()').split(','));height=float(hits.split(';')[0].split(':')[1]) if hits else np.nan;out.append((x,z,height))
 return np.array(out)
old=samples('route-support.tsv','True');new=samples('route-support-fixed.tsv','True');front0=samples('route-support.tsv','False');front1=samples('route-support-fixed.tsv','False');dist=np.r_[0,np.cumsum(np.linalg.norm(np.diff(old[:,:2],axis=0),axis=1))]
fig,axes=plt.subplots(2,1,figsize=(11,6),dpi=135,sharex=True);axes[0].plot(dist,old[:,2],label='r95 geometric surface (two-sided ray)',lw=3,color='#dd9f66');axes[0].plot(dist,new[:,2],label='r96 geometric surface (two-sided ray)',lw=1,color='#165e72');axes[0].set_ylabel('Support elevation (m)');axes[0].legend(fontsize=8)
axes[1].step(dist,np.isfinite(front0[:,2]).astype(int),where='mid',label='r95 top-down front-face support',color='#c24f38');axes[1].step(dist,np.isfinite(front1[:,2]).astype(int),where='mid',label='r96 top-down front-face support',color='#197c66');axes[1].set_yticks([0,1],['missing','present']);axes[1].set_xlabel('Distance along bridge fixture -> I2 -> north approach (m)');axes[1].legend(fontsize=8,loc='lower left')
for a in axes:a.grid(alpha=.3)
fig.suptitle('Winding correction: support orientation restored, grade unchanged\nDiagnostic rays at ~1m spacing; not evidence of visual flatness');fig.tight_layout();fig.savefig(M/'bridge-I2-support-before-after.png');plt.close(fig)
(D/'section-review.json').write_text(json.dumps(dict(sample_count=len(old),length_m=float(dist[-1]),delta_elevation_max_m=float(np.nanmax(np.abs(old[:,2]-new[:,2]))),front_missing_before=int(np.isnan(front0[:,2]).sum()),front_missing_after=int(np.isnan(front1[:,2]).sum())),indent=2))
