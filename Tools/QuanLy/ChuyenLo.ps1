param([switch]$Apply)
$ErrorActionPreference='Stop'
$taskRoot=[IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../..')).TrimEnd('\')
$plan=Get-Content -LiteralPath (Join-Path $taskRoot 'Docs/QuanLy/classification.json') -Encoding UTF8 -Raw | ConvertFrom-Json
$archiveRoot=[IO.Path]::GetFullPath($plan.archive).TrimEnd('\')
if(-not $archiveRoot.StartsWith('D:\Game\ConRoi_LuuTru\',[StringComparison]::OrdinalIgnoreCase)){throw 'Invalid archive root'}
if(Test-Path -LiteralPath $archiveRoot){throw 'Archive batch already exists; never overwrite/reapply'}
$live=Get-CimInstance Win32_Process -Filter "Name='Unity.exe'" | Where-Object {$_.CommandLine -match '(?i)-projectPath\s+"?D:[/\\]ConRoi(?:"|\s|$)'}
if($live){throw 'Close ConRoi Editor safely before this bounded move batch; other projects are not touched'}
$entries=@($plan.entries);$bytes=($entries | Measure-Object bytes -Sum).Sum
$drive=New-Object IO.DriveInfo('D:\')
if($drive.AvailableFreeSpace -lt ($bytes+4GB)){throw 'Insufficient free space for staging plus 4GB reserve. Batch not moved.'}
foreach($entry in $entries){
 $src=[IO.Path]::GetFullPath((Join-Path $taskRoot $entry.old));$dst=[IO.Path]::GetFullPath($entry.new)
 if(-not $src.StartsWith($taskRoot+'\',[StringComparison]::OrdinalIgnoreCase)){throw 'Source outside project'}
 if(-not ($dst.StartsWith($archiveRoot+'\',[StringComparison]::OrdinalIgnoreCase) -or $dst.StartsWith($taskRoot+'\Tools\TaiTao\Legacy\',[StringComparison]::OrdinalIgnoreCase))){throw 'Destination outside approved roots'}
 $parent=Get-Item -LiteralPath $src
 while($parent -and $parent.FullName.Length -ge $taskRoot.Length){if($parent.Attributes -band [IO.FileAttributes]::ReparsePoint){throw ('Reparse ancestor: '+$parent.FullName)};$parent=$parent.Parent}
 if(Test-Path -LiteralPath $dst){throw ('Destination exists: '+$dst)}
 if((Get-Item -LiteralPath $src).Length -ne $entry.bytes){throw ('Source changed since inventory: '+$src)}
}
if(-not $Apply){Write-Output "Validated $($entries.Count) files / $bytes bytes; no mutation";exit}
New-Item -ItemType Directory -Path $archiveRoot | Out-Null
$manifest=Join-Path $archiveRoot 'MANIFEST.jsonl';$local=Join-Path $taskRoot 'Docs/QuanLy/archive-manifest.jsonl'
$utf8=New-Object Text.UTF8Encoding($false)
# All files copy and hash-verify before ANY source move. Staging is retained, never deleted.
$verified=@()
foreach($entry in $entries){
 $src=[IO.Path]::GetFullPath((Join-Path $taskRoot $entry.old));$stage=Join-Path $archiveRoot ('Staging\'+$entry.old)
 New-Item -ItemType Directory -Path (Split-Path -Parent $stage) -Force | Out-Null
 $before=(Get-FileHash -LiteralPath $src -Algorithm SHA256).Hash
 Copy-Item -LiteralPath $src -Destination $stage
 $after=(Get-FileHash -LiteralPath $stage -Algorithm SHA256).Hash
 if($before -ne $after -or (Get-Item -LiteralPath $stage).Length -ne $entry.bytes){throw ('Staging mismatch; no remaining sources moved: '+$src)}
 $verified+= [pscustomobject]@{old=$entry.old;new=[IO.Path]::GetFullPath($entry.new);staging=$stage;action=$entry.action;reason=$entry.reason;bytes=$entry.bytes;sha256=$before;status='VERIFIED_COPY'}
}
[IO.File]::WriteAllText((Join-Path $archiveRoot 'verified-before-move.json'),($verified | ConvertTo-Json -Depth 5),$utf8)
foreach($entry in $verified){
 $src=[IO.Path]::GetFullPath((Join-Path $taskRoot $entry.old))
 if((Get-FileHash -LiteralPath $src -Algorithm SHA256).Hash -ne $entry.sha256){throw ('Changed after staging: '+$src)}
 New-Item -ItemType Directory -Path (Split-Path -Parent $entry.new) -Force | Out-Null
 Move-Item -LiteralPath $src -Destination $entry.new
 if((Get-FileHash -LiteralPath $entry.new -Algorithm SHA256).Hash -ne $entry.sha256){throw ('Moved hash mismatch: '+$entry.new)}
 $entry.status='MOVED_VERIFIED';$line=($entry | ConvertTo-Json -Compress)+[Environment]::NewLine
 [IO.File]::AppendAllText($manifest,$line,$utf8);[IO.File]::AppendAllText($local,$line,$utf8)
}
$result=@{batch=$plan.batch;files=$verified.Count;bytes=$bytes;archiveFiles=@($verified | Where-Object action -eq 'ARCHIVE').Count;archiveBytes=($verified | Where-Object action -eq 'ARCHIVE' | Measure-Object bytes -Sum).Sum;stagingRetained=$true;noDelete=$true;archive=$archiveRoot}
[IO.File]::WriteAllText((Join-Path $taskRoot 'Docs/QuanLy/archive-result.json'),($result | ConvertTo-Json),$utf8)
Write-Output ($result | ConvertTo-Json)
