exec(open('Test/Tam/market95-grade.py').read().split('data=')[0])
g=json.loads((D/'grading.json').read_text());g['objects']=list({r['path']:r for r in g['objects']}.values());g.pop('profile_nodes',None);g.pop('profile_distance_from_I2_nodes',None);g['crossfall']=0;g['flat_core_height']=5.04;g['external_transition']={'z':[320,435],'height':[5.04,.72],'end_easing_length':10,'maximum_analytic_grade_percent':4.32/105*100};g['field_coordinates']='Unity XZ; North=-Z. Game design, not surveyed elevations';g['north_boundary']='Flat core continues to actual mesh edge, no arbitrary Z70 ridge'
for row in g['objects']:
 a=np.load(D/(row['key']+'-graded.npz'));row['vertices']=len(a['v']);row['triangles']=len(a['tri'])
(D/'grading.json').write_text(json.dumps(g,indent=2))
