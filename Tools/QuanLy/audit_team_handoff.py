"""Read-only Git/LFS audit; writes a small report under Docs/QuanLy only."""
import datetime
import json
import pathlib
import subprocess

ROOT = pathlib.Path(__file__).resolve().parents[2]
BASE = 'fcbb7e19c094a2a52d99237d03587df16679d579'
BRANCHES = ['work/map-asset-baseline', 'work/runtime-contracts-baseline', 'work/ui-onboarding-review']

def git(*args):
    return subprocess.check_output(['git', '-c', 'safe.directory=' + ROOT.as_posix(), *args], cwd=ROOT).decode('utf-8').strip()

report = {'utc': datetime.datetime.now(datetime.timezone.utc).isoformat(), 'baseline': BASE,
          'head_before_documentation': git('rev-parse', 'HEAD'), 'branch': git('branch', '--show-current'),
          'tracked_status': git('status', '--short', '--untracked-files=no'),
          'remote_names_only': git('remote').splitlines(), 'worktrees': git('worktree', 'list', '--porcelain'),
          'stash_commits': git('stash', 'list', '--format=%H').splitlines(), 'tasks': {}}
for branch in BRANCHES:
    counts = git('rev-list', '--left-right', '--count', BASE + '...' + branch).split()
    report['tasks'][branch] = {'commit': git('rev-parse', branch), 'baseline_missing_commits': int(counts[0]),
                               'private_commits': int(counts[1])}
report['untracked_assets_held'] = len(git('ls-files', '--others', '--exclude-standard', 'Assets').splitlines())
report['ignored_but_tracked'] = git('ls-files', '-ci', '--exclude-standard').splitlines()
report['product_diff_from_tested_source'] = git('diff', '--name-only', '074caf3', 'HEAD', '--', 'Assets', 'Packages', 'ProjectSettings', 'Source').splitlines()
lfs = json.loads(git('lfs', 'ls-files', '--json'))['files']
unique = {}
bad = []
for item in lfs:
    oid, size = item['oid'], item['size']
    unique[oid] = size
    obj = ROOT / '.git/lfs/objects' / oid[:2] / oid[2:4] / oid
    path = ROOT / item['name']
    if not obj.is_file() or obj.stat().st_size != size or not path.is_file() or path.stat().st_size != size:
        bad.append(item['name'])
    elif path.open('rb').read(64).startswith(b'version https://git-lfs.github.com/spec/v1'):
        bad.append(item['name'])
report['lfs'] = {'files': len(lfs), 'unique_objects': len(unique), 'unique_bytes': sum(unique.values()),
                 'object_size_or_materialization_errors': bad, 'hash_audit_reused': 'NavFix20261108/audit.json',
                 'current_check': 'object existence/size and working-file size/pointer; no repeated full hash audit'}
objects = git('rev-list', '--objects', '--all')
proc = subprocess.run(['git', '-c', 'safe.directory=' + ROOT.as_posix(), 'cat-file', '--batch-check=%(objecttype) %(objectname) %(objectsize) %(rest)'], input=objects+'\n', text=True, encoding='utf-8', capture_output=True, cwd=ROOT, check=True)
blobs = [line.split(' ', 3) for line in proc.stdout.splitlines() if line.startswith('blob ')]
report['largest_regular_history_blob_bytes'] = max(int(row[2]) for row in blobs)
report['regular_history_blobs_over_100MiB'] = [row for row in blobs if int(row[2]) > 100*1024*1024]
report['publish_candidates_only'] = [report['branch'], *BRANCHES]
report['mutations'] = 'No refs, stash, assets, archives or profiles changed; no remote/push.'
dest = ROOT / 'Docs/QuanLy/TeamHandoff20261108/audit.json'
dest.parent.mkdir(parents=True, exist_ok=True)
dest.write_text(json.dumps(report, ensure_ascii=False, indent=2)+'\n', encoding='utf-8')
print(json.dumps(report, ensure_ascii=False, indent=2))
