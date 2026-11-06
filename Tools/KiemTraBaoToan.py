"""Đối chiếu checkpoint sau tổ chức: token C#, GUID, UnityEvent và asset nguồn."""
from pathlib import Path
from DuongDanKiem import ket_qua,phien,dau_vao
import zipfile,json,re,hashlib
R=Path(__file__).resolve().parents[1]; O=R/'Docs/Reviews/journey_v48/restructure-20261102'
LEX=re.compile(r'(?P<skip>\s+|//[^\n]*|/\*[\s\S]*?\*/)|@"(?:""|[^"])*"|\$?"(?:\\.|[^"\\])*"|\'(?:\\.|[^\'\\])*\'|[A-Za-z_][A-Za-z_0-9]*|[^\s]')
def tokens(t,m):
    t=re.sub(r'\[UnityEngine\.Scripting\.APIUpdating\.MovedFrom\(true, null, "Assembly-CSharp", "\w+"\)\]\s*','',t)
    return [m.get(x.group(),x.group()) for x in LEX.finditer(t) if x.lastgroup!='skip']
def main():
    output=ket_qua(phien("bao-toan"));output.mkdir(parents=True,exist_ok=False)
    entries=json.loads((O/'script-moves.json').read_text(encoding='utf-8')); move={e['old']:e['new'] for e in entries}; types={e['old_type']:e['new_type'] for e in entries}
    checked=0;bad=[];meta=[];yaml=[]
    with zipfile.ZipFile(O/'checkpoint-before-restructure.zip') as z:
        for name in z.namelist():
            if not name.startswith('Assets/'):continue
            dst=move.get(name,move.get(name[:-5],name[:-5])+'.meta' if name.endswith('.meta') else name); 
            for a,b in json.loads((O/'directory-moves.json').read_text(encoding='utf-8')):
                if dst.startswith(a+'/'):dst=b+dst[len(a):]
            p=R/dst
            if name.endswith('.cs'):
                checked+=1
                if not p.exists() or tokens(z.read(name).decode('utf-8-sig'),types)!=tokens(p.read_text(encoding='utf-8-sig'),{}):bad.append(name)
            elif name.endswith('.cs.meta'):
                if not p.exists() or re.search(rb'guid: (\w+)',z.read(name)).group(1)!=re.search(rb'guid: (\w+)',p.read_bytes()).group(1):meta.append(name)
            elif name.endswith(('.unity','.prefab','.asset')) and p.exists():
                before=z.read(name);after=p.read_bytes()
                if before!=after:
                    if before.startswith(b'%YAML') and after.startswith(b'%YAML'):
                        t=after.decode('utf-8')
                        for old,new in types.items():
                            t=re.sub(r'(m_EditorClassIdentifier:.*?::)'+new+r'\b',r'\g<1>'+old,t)
                            t=re.sub(r'(m_TargetAssemblyTypeName:\s*)'+new+r'(,)',r'\g<1>'+old+r'\2',t)
                        if before.replace(b'\r\n',b'\n')!=t.encode('utf-8').replace(b'\r\n',b'\n'):yaml.append(name)
                    else:yaml.append(name)
    result={'cs_files_checked':checked,'behavior_token_differences':bad,'script_guid_differences':meta,'yaml_changes_beyond_type_names':yaml,'schema_field_and_method_names':'Không đổi; so token toàn bộ source với checkpoint sau ánh xạ tên kiểu.'}
    (output/'static-preservation.json').write_text(json.dumps(result,ensure_ascii=False,indent=2),encoding='utf-8')
    print(json.dumps(result,ensure_ascii=True))
    if bad or meta or yaml:raise SystemExit(1)
if __name__=='__main__':main()
