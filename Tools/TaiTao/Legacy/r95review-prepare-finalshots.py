from pathlib import Path
s=Path('Test/Tam/r95review-comparison.cs.txt').read_text().replace('new[]{"before","after"}','new[]{"fixed"}').replace('"Assets/Scenes/JourneyPresentation_v48_"+(state=="before"?"r94":"r95")+".unity"','"Assets/Scenes/JourneyPresentation_v48_r96.unity"').replace('comparison-camera-notes.txt','fixed-comparison-camera-notes.txt')
Path('Test/Tam/r95review-fixedshots.cs.txt').write_text(s)
s=Path('Test/Tam/r95review-route-support.cs.txt').read_text().replace('route-support.tsv','route-support-fixed.tsv')
Path('Test/Tam/r95review-route-support-fixed.cs.txt').write_text(s)
