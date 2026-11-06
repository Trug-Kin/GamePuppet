from pathlib import Path
import json
R=Path('D:/ConRoi');D=R/'Test/KetQua/journey_v48/dam_r98'
s=(R/'Test/Tam/bank97-integrate.cs.txt').read_text(encoding='utf-8-sig')
s=s.replace('Clean r96','Clean r97').replace('EndsWith("r96.unity")','EndsWith("r97.unity")')
s=s.replace('r97.unity','r98.unity').replace('EndsWith("r98.unity")','EndsWith("r97.unity")')
s=s.replace('bank_r97','dam_r98').replace('BankR97','DamR98').replace('BoundR97','BoundR98').replace('R97_Bridgehead_GateSide_Bank','R98_OngDao_Concrete_Dam').replace('BridgeheadBankR97','OngDaoDamR98').replace('R97_','R98_').replace('R97 derivative','R98 derivative')
s=s.replace('"Grass","Paving","Concrete","Quay","Rail"','"Concrete","Ridge","Toe","Paving","Rail"')
s=s.replace('new Color(.28f,.36f,.13f),new Color(.52f,.48f,.40f),new Color(.48f,.46f,.40f),new Color(.28f,.29f,.26f),new Color(.22f,.34f,.31f)','new Color(.62f,.59f,.51f),new Color(.46f,.44f,.38f),new Color(.25f,.28f,.26f),new Color(.49f,.46f,.39f),new Color(.24f,.34f,.31f)')
s=s.replace('bool legacy=path!="";','bool legacy=path!="",colliderOnly=e.Length>3&&e[3]=="1";')
s=s.replace('var rr=g.GetComponent<MeshRenderer>();UnityEngine.Mesh previous=legacy?Readable(f.sharedMesh):null;','var rr=colliderOnly?null:g.GetComponent<MeshRenderer>();UnityEngine.Mesh previous=legacy?Readable(colliderOnly?co.sharedMesh:f.sharedMesh):null;')
s=s.replace('f.sharedMesh=mesh;if(co&&co.enabled)','if(!colliderOnly)f.sharedMesh=mesh;if(co&&co.enabled)')
s=s.replace('if(rr&&rr.enabled&&mesh.HasVertexAttribute','if(rr&&rr.enabled&&mesh.HasVertexAttribute')
s=s.replace('var srcIndices=new Vector3Int[v.Length];','bool attributes=legacy&&!path.StartsWith("R97_Bridgehead_GateSide_Bank/");var srcIndices=new Vector3Int[v.Length];')
s=s.replace('legacy?prior', 'attributes?prior').replace('legacy?previous.', 'attributes?previous.')
s=s.replace('if(legacy)', 'if(attributes)')
# The declaration is later than the priorV arrays: define attributes before them.
s=s.replace('bool attributes=legacy&&!path.StartsWith("R97_Bridgehead_GateSide_Bank/");var srcIndices', 'var srcIndices')
s=s.replace('Vector3[] priorV=', 'bool attributes=legacy&&!path.StartsWith("R97_Bridgehead_GateSide_Bank/");Vector3[] priorV=')
(R/'Test/Tam/dam98-integrate.cs.txt').write_text(s,encoding='utf-8')
rows=json.loads((D/'export-reopen.json').read_text())['rows']
(D/'integration.tsv').write_text('\n'.join(r['key']+'\t'+r['path']+'\t'+(';'.join(r['material_names']) if not r['legacy'] else '')+'\t'+('1' if r['collider_only'] else '0') for r in rows),encoding='utf-8')
print('Prepared',len(rows),'exact bindings')
