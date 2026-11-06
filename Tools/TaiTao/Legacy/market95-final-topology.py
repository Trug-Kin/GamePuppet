exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());out=[]
for row in g['objects']:
 if '/MarketIsland_R62/' in row['path'] or row['key'].startswith('M95_F'):continue
 fp=D/(row['key']+'-graded.npz');a=np.load(fp)
 if 'is_floor' not in a:continue
 v=a['v'].copy();t=a['tri'];original_count=len(t);before=a['before'];uv=a['uv'];slots=a['slots'];flags=a['is_floor'];floor_vertices=np.unique(t[flags]);v[floor_vertices[:,None],[0,2]]=np.round(v[floor_vertices[:,None],[0,2]],4)
 # Height is authored by the shared continuous field. Do not project a subset
 # using the obsolete weight mask: that would create narrow height seams.
 order=np.argsort(-before[:,1],kind='stable');_,first,inv=np.unique(np.round(v[order],6),axis=0,return_index=True,return_inverse=True);remap=np.empty(len(v),int);remap[order]=inv;v=v[order[first]];before=before[order[first]];t=remap[t];q=v[t];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);edges=np.linalg.norm(np.roll(q[:,:,[0,2]],-1,axis=1)-q[:,:,[0,2]],axis=2);altitude=abs(n[:,1])/np.maximum(edges.max(1),1e-12);thin=flags&(altitude<.0001);area=float(abs(n[thin,1]).sum()/2)
 assert area<.01,(row['path'],area)
 good=(np.linalg.norm(n,axis=1)>1e-9)&~thin;t=t[good];uv=uv[good];slots=slots[good];flags=flags[good];_,keep=np.unique(np.sort(t,axis=1),axis=0,return_index=True);keep.sort();t=t[keep];uv=uv[keep];slots=slots[keep];flags=flags[keep]
 np.savez_compressed(fp,v=v,tri=t,before=before,uv=uv,slots=slots,is_floor=flags)
 out.append(dict(path=row['path'],removed_under_01mm_altitude=int(thin.sum()),removed_projected_area=area,removed_zero_or_duplicate=int(original_count-len(t))))
(D/'topology-finalization.json').write_text(json.dumps(out,indent=2));print('FINAL_TOPOLOGY',sum(r['removed_under_01mm_altitude'] for r in out),'area',sum(r['removed_projected_area'] for r in out),flush=True)
