from pathlib import Path
import json,re,collections
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy';items=[];sources=[]
for top in ['Assets','Tools','Packages']:
 for p in (R/top).rglob('*'):
  if not p.is_file() or p.suffix.lower() not in ['.cs','.asmdef','.asmref','.json','.py','.ps1'] or 'python-deps' in p.parts or p.stat().st_size>2_000_000:continue
  try:s=p.read_text(encoding='utf-8-sig')
  except (UnicodeError,OSError):continue
  flags=[x for x,pattern in [('Resources',r'Resources\.Load'),('StreamingAssets',r'StreamingAssets'),('Addressables',r'Addressables'),('reflection',r'Type\.GetType|Assembly\.Load|GetComponent\("'),('editorAssetPath',r'AssetDatabase\.Load'),('historicalOutputPath',r'Test[/\\]|Docs[/\\]Reviews'),('absolutePath',r'[CD]:[/\\]')] if re.search(pattern,s)]
  if flags:items.append(dict(path=p.relative_to(R).as_posix(),flags=flags))
  if p.suffix in ['.asmdef','.asmref']:sources.append(p.relative_to(R).as_posix())
manifest=json.loads((R/'Packages/manifest.json').read_text(encoding='utf-8'));local={k:v for k,v in manifest['dependencies'].items() if v.startswith(('file:','git+','http'))}
report=dict(scanned=['Assets','Tools','Packages'],findings=items,assembly_definitions=sources,local_or_url_packages=local,policy='Keep all Assets compiled C#/Editor/tests, Resources, StreamingAssets and original paths. Do not infer unused from scene dependency. Historical recipes held PROVISIONAL; no runtime D:/Game archive path introduced.')
(D/'static-dependencies.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print('Static findings',len(items),'asmdef/asmref',len(sources),'local packages',len(local))
