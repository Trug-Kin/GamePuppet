exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());out=[]
for row in g['objects']:
 if row['path'].split('/')[-1] not in ['Surface_176','Surface_142','Context_124','Restored_029']:continue
 fp=D/(row['key']+'-graded.npz');a=np.load(fp);v=a['v'].copy();before=a['before'];t=a['tri'];uv=a['uv'];slots=a['slots'];flags=a['is_floor']
 # Interior construction skirts are not intentional kerbs/bank walls. Old
 # non-floor faces retained their high tips after their neighbouring floor was
 # relevelled, visible as yellow triangles in the object attribution render.
 # Remove old internal construction skirts rather than collapse a second
 # sheet onto the remeshed floor (which would recreate coplanar overlap).
 centres=v[t].mean(1);obsolete=(~flags)&(centres[:,0]>704)&(centres[:,0]<870)&(centres[:,2]<320)
 t=t[~obsolete];uv=uv[~obsolete];slots=slots[~obsolete];flags=flags[~obsolete];moved=np.zeros(len(v),bool)
 # Retain the original TOP provenance when an old skirt collapses onto a floor.
 order=np.argsort(-before[:,1],kind='stable');unique,first,inv=np.unique(np.round(v[order],6),axis=0,return_index=True,return_inverse=True);remap=np.empty(len(v),int);remap[order]=inv;v=v[order[first]];bb=before[order[first]];t=remap[t]
 q=v[t];n=np.cross(q[:,1]-q[:,0],q[:,2]-q[:,0]);good=np.linalg.norm(n,axis=1)>1e-9;t=t[good];uv=uv[good];slots=slots[good];flags=flags[good];_,keep=np.unique(np.sort(t,axis=1),axis=0,return_index=True);keep.sort();t=t[keep];uv=uv[keep];slots=slots[keep];flags=flags[keep]
 np.savez_compressed(fp,v=v,tri=t,before=bb,uv=uv,slots=slots,is_floor=flags)
 out.append(dict(path=row['path'],removed_obsolete_internal_skirt_faces=int(obsolete.sum()),relevelled_skirt_vertices=int(moved.sum()),removed_degenerate=int((~good).sum()),duplicate_faces_removed=int(good.sum()-len(keep))))
(D/'interior-construction-skirts.json').write_text(json.dumps(out,indent=2));print(out,flush=True)
