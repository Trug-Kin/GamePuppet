from pathlib import Path
p=Path('Test/Tam/bank97-route.cs.txt');s=p.read_text();needle='new Vector2(689.23783f,471.00000f),new Vector2(690.24009f,476.00000f),new Vector2(689.81167f,480.00000f),new Vector2(690.00000f,489.00000f)'
assert needle in s
s=s.replace(needle,'new Vector2(689.23783f,471.00000f),new Vector2(690.24009f,476.00000f),new Vector2(689.81167f,480.00000f),new Vector2(682.5f,480.0f),new Vector2(678.5f,483.0f),new Vector2(679.0f,486.0f),new Vector2(685.0f,487.0f),new Vector2(690.00000f,489.00000f)')
# Actual gameplay lighting begins after the far stair descent, before return.
s=s.replace('leg==61','leg==32');p.write_text(s)
print('68 waypoints; fixed gameplay lighting switch on return at waypoint32')
