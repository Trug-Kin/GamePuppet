param([switch]$Apply)
$ErrorActionPreference='Stop'
$projectRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..')).TrimEnd('\')
$reportRoot=Join-Path $projectRoot 'Docs/QuanLy/Review20261107'
if(Test-Path -LiteralPath (Join-Path $reportRoot 'archive-result.json')){throw 'Preserve existing result; never reapply'}
$live=@(Get-CimInstance Win32_Process -Filter "Name='Unity.exe'" | Where-Object {$_.CommandLine -match '(?i)-projectPath\s+"?D:[/\\]ConRoi(?:"|\s|$)'})
if($live.Count){throw 'ConRoi process active; no archive'}
$items=@()
$captureDir=Join-Path $projectRoot 'Test/Output/GitPreparation'
if(Test-Path -LiteralPath $captureDir){$items+=@(Get-ChildItem -LiteralPath $captureDir -File | Where-Object {$_.Extension -eq '.png'})}
$logPath=Join-Path $projectRoot 'Logs/team-prepare-editor.log'
if(Test-Path -LiteralPath $logPath){$items+=Get-Item -LiteralPath $logPath}
if(-not $items.Count){throw 'No verified previous technical capture/log inputs'}
$bytes=($items | Measure-Object Length -Sum).Sum
if($bytes -gt 32MB){throw 'Exceeds bounded 32MiB staging; review required'}
if((New-Object IO.DriveInfo('D:\')).AvailableFreeSpace -lt (2GB+$bytes)){throw 'Insufficient 2GiB operating reserve plus staging; no move'}
$batch='git-review-20261107-'+(Get-Date -Format 'HHmmss')+'-'+[Guid]::NewGuid().ToString('N').Substring(0,6)
$archive=[IO.Path]::GetFullPath((Join-Path 'D:\Game\ConRoi_LuuTru' $batch)).TrimEnd('\')
if(-not $archive.StartsWith('D:\Game\ConRoi_LuuTru\',[StringComparison]::OrdinalIgnoreCase)){throw 'Archive path escaped'}
if(Test-Path -LiteralPath $archive){throw 'Destination exists'}
foreach($item in $items){
 $src=[IO.Path]::GetFullPath($item.FullName)
 if(-not $src.StartsWith($projectRoot+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Source escaped root'}
 $node=$item
 while($node -and $node.FullName.Length -ge $projectRoot.Length){if($node.Attributes -band [IO.FileAttributes]::ReparsePoint){throw 'Reparse ancestor; do not follow'};$node=$node.Parent}
}
if(-not $Apply){Write-Output "Verified C technical evidence candidates $($items.Count) files / $bytes bytes; moving same D will not free D";exit}
New-Item -ItemType Directory -Path $archive | Out-Null
$rows=@();$utf8=New-Object Text.UTF8Encoding($false)
foreach($item in $items){
 $rel=$item.FullName.Substring($projectRoot.Length+1).Replace('\','/')
 $staging=Join-Path $archive ('Staging\'+$rel);$dest=Join-Path $archive ('DuLieu\'+$rel)
 New-Item -ItemType Directory -Path (Split-Path -Parent $staging) -Force | Out-Null
 $hash=(Get-FileHash -LiteralPath $item.FullName -Algorithm SHA256).Hash
 Copy-Item -LiteralPath $item.FullName -Destination $staging
 if((Get-FileHash -LiteralPath $staging -Algorithm SHA256).Hash -ne $hash){throw 'Staging mismatch; no source move yet'}
 $rows+=[pscustomobject]@{old=$rel;new=$dest;staging=$staging;bytes=$item.Length;sha256=$hash;category='C';reason='Known previous GitPreparation verification screenshot or stopped Editor log; not game media/source; not Unity Assets';status='VERIFIED_COPY'}
}
[IO.File]::WriteAllText((Join-Path $archive 'verified-before-move.json'),($rows|ConvertTo-Json -Depth 5),$utf8)
foreach($row in $rows){
 $src=Join-Path $projectRoot $row.old
 if((Get-FileHash -LiteralPath $src -Algorithm SHA256).Hash -ne $row.sha256){throw 'Source changed after copy; stop'}
 New-Item -ItemType Directory -Path (Split-Path -Parent $row.new) -Force | Out-Null
 Move-Item -LiteralPath $src -Destination $row.new
 if((Get-FileHash -LiteralPath $row.new -Algorithm SHA256).Hash -ne $row.sha256){throw 'Moved hash mismatch'}
 $row.status='MOVED_VERIFIED';$line=($row|ConvertTo-Json -Compress)+[Environment]::NewLine
 [IO.File]::AppendAllText((Join-Path $archive 'MANIFEST.jsonl'),$line,$utf8)
 [IO.File]::AppendAllText((Join-Path $reportRoot 'archive-manifest.jsonl'),$line,$utf8)
}
$result=@{batch=$batch;archive=$archive;files=$rows.Count;bytes=$bytes;stagingRetained=$true;deletedFiles=0;D_free_space_gain=0;additional_staging_bytes=$bytes;sourceGuidChanges=0;runtimeMediaMoved=$false;restore='Use per-file old/new/SHA256; verify then copy back only if old absent, never overwrite conflicts'}
[IO.File]::WriteAllText((Join-Path $reportRoot 'archive-result.json'),($result|ConvertTo-Json),$utf8)
Write-Output ($result|ConvertTo-Json)
