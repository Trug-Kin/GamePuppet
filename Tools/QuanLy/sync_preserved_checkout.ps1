$ErrorActionPreference = 'Stop'
$root = 'C:\ConRoi_CheckoutTest'
if (Get-CimInstance Win32_Process -Filter "name='Unity.exe'" | Where-Object { $_.CommandLine -like '*-projectPath C:\ConRoi_CheckoutTest*' }) { throw 'Checkout Unity still running' }
$manifest = Get-Content -Raw -Encoding UTF8 'D:\ConRoi\Docs\QuanLy\CheckoutFinal20261107\preservation-before-sync.json' | ConvertFrom-Json
$quarantine = Join-Path $root ('Test\Output\CleanCheckout\Quarantine_' + (Get-Date -Format yyyyMMdd_HHmmss))
foreach ($row in $manifest.files) {
  $src = [IO.Path]::GetFullPath((Join-Path $root $row.path))
  if (!$src.StartsWith($root + '\')) { throw 'Outside checkout' }
  if ((Get-FileHash -LiteralPath $src -Algorithm SHA256).Hash.ToLower() -ne $row.sha256) { throw "Changed since snapshot: $src" }
  if ((Get-FileHash -LiteralPath $row.snapshot -Algorithm SHA256).Hash.ToLower() -ne $row.sha256) { throw 'Snapshot hash mismatch' }
}
foreach ($row in $manifest.files) {
  $src = [IO.Path]::GetFullPath((Join-Path $root $row.path))
  $dst = [IO.Path]::GetFullPath((Join-Path $quarantine $row.path))
  if (!$dst.StartsWith($root + '\Test\Output\CleanCheckout\')) { throw 'Outside quarantine' }
  New-Item -ItemType Directory -Force -Path (Split-Path $dst) | Out-Null
  Move-Item -LiteralPath $src -Destination $dst
}
git -c safe.directory=C:/ConRoi_CheckoutTest -C $root restore --worktree --source=HEAD -- Assets ProjectSettings
if ($LASTEXITCODE) { throw 'Restore failed' }
git -c safe.directory=C:/ConRoi_CheckoutTest -C $root fetch D:/ConRoi a789222286f1a56db5b6cc21e1ffb5a0e7542be4
if ($LASTEXITCODE) { throw 'Fetch failed' }
$env:GIT_LFS_SKIP_SMUDGE='1'
git -c safe.directory=C:/ConRoi_CheckoutTest -C $root checkout --detach a789222286f1a56db5b6cc21e1ffb5a0e7542be4
if ($LASTEXITCODE) { throw 'Checkout failed' }
$oid='00eac743343893f614abbb6cbb1ddc286565d95f330e4913ced30345384a7939'
$relative='.git\lfs\objects\00\ea\'+$oid
$objectSource=Join-Path 'D:\ConRoi' $relative
$objectTarget=Join-Path $root $relative
New-Item -ItemType Directory -Force -Path (Split-Path $objectTarget) | Out-Null
if (!(Test-Path -LiteralPath $objectTarget)) { Copy-Item -LiteralPath $objectSource -Destination $objectTarget }
if ((Get-FileHash -LiteralPath $objectTarget).Hash.ToLower() -ne $oid) { throw 'LFS object mismatch' }
git -c safe.directory=C:/ConRoi_CheckoutTest -C $root lfs checkout
if ($LASTEXITCODE) { throw 'LFS checkout failed' }
git -c safe.directory=C:/ConRoi_CheckoutTest -C $root lfs fsck --objects
if ($LASTEXITCODE) { throw 'LFS fsck failed' }
@{commit=(git -c safe.directory=C:/ConRoi_CheckoutTest -C $root rev-parse HEAD); quarantine=$quarantine; preservedFiles=$manifest.files.Count; recoverySHA256=(Get-FileHash -LiteralPath (Join-Path $root 'Assets\_Recovery\0 (2).unity')).Hash; freeC=(Get-CimInstance Win32_LogicalDisk -Filter "DeviceID='C:'").FreeSpace} | ConvertTo-Json | Set-Content -Encoding UTF8 'D:\ConRoi\Docs\QuanLy\CheckoutFinal20261107\sync-result.json'
git -c safe.directory=C:/ConRoi_CheckoutTest -C $root status --short
