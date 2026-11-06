from pathlib import Path
import json
p=Path('Test/KetQua/journey_v48/dam_r98/fbx-roundtrip.json');q=json.loads(p.read_text());p.write_text(json.dumps(list({r['key']:r for r in q}.values()),indent=2))
