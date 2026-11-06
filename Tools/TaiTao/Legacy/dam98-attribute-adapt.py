from pathlib import Path
p=Path('Test/Tam/dam98-make-integration.py');s=p.read_text();needle="(R/'Test/Tam/dam98-integrate.cs.txt').write_text(s,encoding='utf-8')";insert='''s=s.replace('var srcIndices=new Vector3Int[v.Length];','bool attributes=legacy&&!path.StartsWith("R97_Bridgehead_GateSide_Bank/");var srcIndices=new Vector3Int[v.Length];')
s=s.replace('legacy?prior', 'attributes?prior').replace('legacy?previous.', 'attributes?previous.')
s=s.replace('if(legacy)', 'if(attributes)')
# The declaration is later than the priorV arrays: define attributes before them.
s=s.replace('bool attributes=legacy&&!path.StartsWith("R97_Bridgehead_GateSide_Bank/");var srcIndices', 'var srcIndices')
s=s.replace('Vector3[] priorV=', 'bool attributes=legacy&&!path.StartsWith("R97_Bridgehead_GateSide_Bank/");Vector3[] priorV=')
''';s=s.replace(needle,insert+needle);p.write_text(s)
