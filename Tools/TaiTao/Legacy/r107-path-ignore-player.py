from pathlib import Path
p=Path('Test/Tam/r107-path-plan.cs.txt');s=p.read_text()
s=s.replace('.Where(h=>h.point.y>.3f&&h.normal.y>.5f)', '.Where(h=>!h.collider.GetComponentInParent<DieuKhienDiChuyen>()&&h.point.y>.3f&&h.normal.y>.5f)')
s=s.replace('if(overlaps.Length>0)continue;','if(overlaps.Any(c=>!c.GetComponentInParent<DieuKhienDiChuyen>()))continue;')
s=s.replace('if(Physics.SphereCast(a,.25f,(b-a).normalized,out var hit,(b-a).magnitude,mask,QueryTriggerInteraction.Ignore))continue;','if(Physics.SphereCastAll(a,.25f,(b-a).normalized,(b-a).magnitude,mask,QueryTriggerInteraction.Ignore).Any(h=>!h.collider.GetComponentInParent<DieuKhienDiChuyen>()))continue;')
p.write_text(s)
