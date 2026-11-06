from pathlib import Path
from collections import Counter
import re,json
p=Path('Assets/Model/Characters');files=[x for x in p.rglob('*') if x.is_file() and x.suffix!='.meta']
print('Characters count',len(files),'extensions',dict(Counter(x.suffix for x in files)))
print('\n'.join(x.name for x in files))
print('License candidates', [str(x) for x in Path('Assets/Model').rglob('*') if x.is_file() and any(t in x.name.lower() for t in ['license','readme','credit'])])
s=Path('Source/Downloads/Quaternius_R104/official-drive-folder.html').read_text(encoding='utf-8')
print('DRIVE', '\n'.join(re.findall(r'data-id="[^"]+".{0,200}',s)))
print('LABELS', '\n'.join(re.findall(r'aria-label="[^"]+"',s)[-65:]))
