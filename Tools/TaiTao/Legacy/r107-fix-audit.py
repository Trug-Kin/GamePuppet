from pathlib import Path
p=Path('Test/Tam/r107-readonly-final-audit.cs.txt')
s=p.read_text(encoding='utf-8')
start=s.index('{down++;b.AppendLine(')
end=s.index('}}b.AppendLine(paths[k]',start)
s=s[:start]+'{down++;b.AppendLine("DOWN tri="+i/3+" at="+p.ToString("F6")+" normal="+n.ToString("F6"));'+s[end:]
p.write_text(s,encoding='utf-8')
