from pathlib import Path
import subprocess,json,shutil,hashlib,collections
R=Path(__file__).resolve().parents[2];D=R/'Docs/QuanLy'
def run(*args):
 p=subprocess.run(['git',*args],cwd=R,capture_output=True);return p.returncode,p.stdout.decode('utf-8',errors='replace'),p.stderr.decode('utf-8',errors='replace')
branch=run('branch','--show-current')[1].strip();head=run('rev-parse','HEAD')[1].strip();remotes=run('remote')[1].splitlines()
tracked=run('ls-files','-z')[1].split('\0');tracked=[x for x in tracked if x]
ignored_tracked=run('ls-files','-ci','--exclude-standard')[1].splitlines()
untracked=run('ls-files','--others','--exclude-standard','-z')[1].split('\0');untracked=[x for x in untracked if x]
code,lfs,_=run('lfs','ls-files','--json');lf=json.loads(lfs) if code==0 else {}
code,fsck,err=run('lfs','fsck','--objects');fs=dict(exit=code,output=fsck,error=err)
raw=run('rev-list','--objects','--all')[1]
proc=subprocess.run(['git','cat-file','--batch-check=%(objecttype) %(objectname) %(objectsize) %(rest)'],input=raw,cwd=R,text=True,capture_output=True)
large=[]
for line in proc.stdout.splitlines():
 a=line.split(' ',3)
 if len(a)==4 and a[0]=='blob' and int(a[2])>90*1024**2:large.append(dict(oid=a[1],bytes=int(a[2]),path=a[3]))
stats=collections.defaultdict(lambda:dict(files=0,bytes=0))
for name in untracked:
 p=R/name;top=name.split('/')[0];stats[top]['files']+=1
 if p.is_file():stats[top]['bytes']+=p.stat().st_size
space=shutil.disk_usage(R).free
report=dict(root=str(R),branch=branch,head=head,remote_names=remotes,tracked=len(tracked),tracked_ignored=ignored_tracked,untracked=len(untracked),untracked_groups=dict(stats),large_history_blobs_over90MiB=large,lfs_listing=lf,lfs_fsck=fs,free_bytes=space,clean_checkout=dict(status='BLOCKED',reason='Same D drive has insufficient free space for prepared working tree plus new Library/import; do not reuse original Library as clean-checkout proof',working_tree_estimate=json.loads((D/'git-stage-plan.json').read_text(encoding='utf-8'))['bytes'],no_library_copy=True,opened=False),push=False,public_repo=False)
(D/'git-audit.json').write_text(json.dumps(report,ensure_ascii=False,indent=2),encoding='utf-8')
print(json.dumps({k:v for k,v in report.items() if k not in ['lfs_listing','untracked_groups']},ensure_ascii=False,indent=2))
