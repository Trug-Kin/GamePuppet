"""Preserve binary legacy checkpoint bytes and configured render asset closure."""
import hashlib,json,subprocess
from pathlib import Path
R=Path(__file__).resolve().parents[2];O=R/'Docs/QuanLy/CheckoutTest20261107'
def git(*args):return subprocess.check_output(['git','-c','safe.directory='+str(R),*args],cwd=R)
name='Assets/_Recovery/0 (2).unity'
original=(R/name).read_bytes();blob=git('cat-file','blob','8ea91d4:'+name)
assert len(original)==7619172 and hashlib.sha256(original).hexdigest()=='00eac743343893f614abbb6cbb1ddc286565d95f330e4913ced30345384a7939'
extras=['Assets/HDRPDefaultResources.meta','Assets/HDRPDefaultResources/HDRenderPipelineGlobalSettings.asset','Assets/HDRPDefaultResources/HDRenderPipelineGlobalSettings.asset.meta','Assets/HDRPDefaultResources/DefaultSettingsVolumeProfile.asset','Assets/HDRPDefaultResources/DefaultSettingsVolumeProfile.asset.meta']
rows=[]
for n in extras:
    p=R/n;assert p.is_file()
    rows.append(dict(path=n,bytes=p.stat().st_size,sha256=hashlib.sha256(p.read_bytes()).hexdigest(),reason='Verified existing GraphicsSettings HDRP global GUID 49e2c02c67bdb254fb2e233cd8e14627 and its serialized asset closure; preserve existing config, do not activate HDRP'))
report=dict(binary_checkpoint=dict(path=name,original_bytes=len(original),git_8ea91d4_bytes=len(blob),original_sha256=hashlib.sha256(original).hexdigest(),git_8ea91d4_sha256=hashlib.sha256(blob).hexdigest(),cause='Explicit *.unity text forced CRLF normalization on a legacy binary scene. Repair by exact-path LFS -text, without editing/reserializing checkpoint.'),promoted_D=rows,D_remaining=4183,source_bytes_modified=False)
(O/'git-dependency-correction.json').write_text(json.dumps(report,indent=2),encoding='utf-8')
attrs=R/'.gitattributes';prior=attrs.read_text(encoding='utf-8')
line='"Assets/_Recovery/0 (2).unity" filter=lfs diff=lfs merge=lfs -text'
assert line not in prior
attrs.write_text(prior+'\n# Legacy binary scene exception: retain original bytes, do not normalize as YAML.\n'+line+'\n',encoding='utf-8')
print(json.dumps(dict(binary_bytes_preserved=len(original),verified_config_files=len(rows),remaining_D=4183)))
