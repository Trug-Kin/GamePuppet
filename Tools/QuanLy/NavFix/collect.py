"""Assert the tested source and collect small reports; raw captures remain outside Git."""
from pathlib import Path
import datetime,hashlib,json,re,shutil,subprocess
R=Path(__file__).resolve().parents[3];C=Path('C:/ConRoi_CheckoutTest');O=R/'Docs/QuanLy/NavFix20261108';commit='074caf3dd3da4107b4fc9ac1c089b20216b4705b'
def git(root,*a):return subprocess.check_output(['git','-c','safe.directory='+str(root),'-C',str(root),*a]).decode('utf-8').strip()
def sha(p):return hashlib.sha256(p.read_bytes()).hexdigest()
assert git(C,'rev-parse','HEAD')==commit
assert not git(C,'diff','--name-only')
assert not git(R,'diff','--name-only','--','Assets','Source','Packages','ProjectSettings')
allowed={'Assets/Scripts/InvestigationV36R107/InvestigationSession.cs','Assets/Scripts/InvestigationV36R107/InvestigationSessionUI.cs'}
assert set(git(R,'diff','--name-only','a789222',commit,'--','Assets','Source','Packages','ProjectSettings').splitlines())==allowed
proof=[]
for phase in ['repro','fixed']:
    Q=C/('Test/Output/NavFix_'+phase+'_20261108')
    names=['suite.txt','close.txt']+(['tail.txt','controller-E.txt'] if phase=='fixed' else [])
    for name in names:
        t=(Q/name).read_text(encoding='utf-8');assert 'FAIL' not in t,(phase,name)
        if name=='suite.txt':assert ('REPRODUCED blocked P after correct' if phase=='repro' else 'P after world load') in t
        shutil.copyfile(Q/name,O/(phase+'-'+name))
    for p in Q.iterdir():
        if p.is_file() and p.suffix in {'.png','.txt','.json'}:proof.append(dict(path=str(p),bytes=p.stat().st_size,sha256=sha(p)))
shutil.copyfile(C/'Test/Output/CleanCheckout/reference-audit.txt',O/'reference-audit.txt')
audit=(O/'reference-audit.txt').read_text(encoding='utf-8');assert 'MissingScript=0 brokenSerializedReferences=0' in audit and 'compiling=False' in audit
metas=git(C,'ls-files','Assets').splitlines();bad=[];total=0
for n in metas:
    if not n.endswith('.meta'):continue
    total+=1;a=re.search(rb'^guid: (\S+)',(R/n).read_bytes(),re.M);b=re.search(rb'^guid: (\S+)',(C/n).read_bytes(),re.M)
    if (a.group(1) if a else None)!=(b.group(1) if b else None):bad.append(n)
assert not bad
profile=json.loads((R/'Docs/QuanLy/profiles-final.json').read_text(encoding='utf-8'))['rows'][0];assert sha(Path(profile['new']))==profile['sha256']
raw=json.loads(git(C,'lfs','ls-files','--json'));rows=raw.get('files',[]) if isinstance(raw,dict) else raw
for row in rows:assert sha(C/row['name'])==row['oid'],row['name']
fsck=git(C,'lfs','fsck','--objects')
legacy=len(git(R,'ls-files','--others','--exclude-standard','Assets').splitlines());assert legacy==4183
branches={x.split()[0]:x.split()[1] for x in git(R,'for-each-ref','--format=%(refname:short) %(objectname)','refs/heads/work/').splitlines()}
for branch,old in branches.items():assert old=='a209c0e3b41a8b55e438aeabf1025b5ef6e7d940';git(R,'merge-base','--is-ancestor',old,commit)
report=dict(utc=datetime.datetime.now(datetime.timezone.utc).isoformat(),tested_source=commit,checkout=str(C),unity='6000.6.2f1',entry='Assets/ConRoi/Modules/TichHop/Scenes/ConRoi_Chinh.unity',status='PASS_TECHNICAL_NAV01',newcomer='NOT_RUN',lfs_files=len(rows),lfs_unique_objects=len(set(x['oid'] for x in rows)),lfs_bad=[],lfs_fsck=fsck,meta_guids_compared=total,meta_guid_mismatch=bad,changed_product_paths=sorted(allowed),legacy_unclassified_held=legacy,original_profile_sha256=profile['sha256'],task_branches_before=branches,worktrees_before=git(R,'worktree','list','--porcelain'),stash_commits_preserved=git(R,'stash','list','--format=%H').splitlines(),source_remotes=git(R,'remote').splitlines(),checkout_remotes=git(C,'remote').splitlines(),C_free=shutil.disk_usage('C:/').free,source_branch=git(R,'branch','--show-current'),library='Independent C Library created originally; incremental import of fixed commit, never copied/shared from D')
(O/'audit.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8');(O/'evidence-index.json').write_text(json.dumps(proof,indent=2),encoding='utf-8')
print(json.dumps(report,ensure_ascii=False,indent=2))
