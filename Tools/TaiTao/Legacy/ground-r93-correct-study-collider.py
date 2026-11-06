from pathlib import Path
import json
p=Path('Test/Tam/ground-r93-wide-study.py');s=p.read_text()
s=s.replace("('Surface_153','Context_116')","('Surface_153','Context_136')").replace("('Surface_129','Surface_173','Context_111','Context_136')","('Surface_129','Surface_173','Context_111')")
s=s.replace("I2 radius13.2..24","I2 radius13.2..32")
p.write_text(s)
d=Path('Test/KetQua/journey_v48/ground_r93')
for n in ['wide-transition-study.json','wide-study-face-grades.json','wide-study-reopen-audit.json','wide-study-section-comparison.json']:
 f=d/n
 if f.exists():(d/('historical-before-collider-map-fix-'+n)).write_bytes(f.read_bytes())
(d/'study-collider-map-correction.json').write_text(json.dumps(dict(status='AUTHORING_STUDY_ONLY_NOT_PRODUCT_FIX',cause='Context136 is the collider matching Surface153, not Context116. Previous unaccepted generator selected0.015 vs render0.06 estimated layer offset.',change='Use0.06 for both Surface153 and Context136. No product mesh/reference changed.',delta_estimate_at_full_weight=.045),indent=2))
print('Study generator corrected; previous numerical reports retained.')
