from pathlib import Path
p=Path('Test/Tam/r107-integrate-map.cs.txt');s=p.read_text()
s=s.replace('TerrainR107.fbx','TerrainR107_Final.fbx').replace('"LakeGateMarketV32/Context_124"};','"LakeGateMarketV32/Context_124","R20_Whole_Source_Export/Surface_142"};')
s=s.replace('var src=srcs["T107_"+k]','var src=srcs["T107_"+(k==3?4:k)]')
s=s.replace('_WindingR107"','_FinalWindingR107"').replace('p.z>418','p.z>460').replace('map-integration.txt','map-integration-final.txt')
Path('Test/Tam/r107-integrate-map-final.cs.txt').write_text(s)
