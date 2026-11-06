param(
 [ValidateSet("Probe","Build","Open")][string]$Mode="Probe",
 [string]$Tag=("kiem-"+(Get-Date -Format "yyyyMMdd-HHmmss")),
 [string]$OutputDirectory="",
 [string]$BuildOutput="",
 [string]$TestRoot=$env:CONROI_TEST_ROOT,
 [switch]$ReadOnly
)
# Output kiem rieng; project va cache khong phu thuoc thu muc Test.
$ErrorActionPreference="Stop"
$projectRoot=Split-Path -Parent $PSScriptRoot
if([string]::IsNullOrWhiteSpace($TestRoot)){$TestRoot=Join-Path $projectRoot "Test"}
elseif(-not [System.IO.Path]::IsPathRooted($TestRoot)){$TestRoot=Join-Path $projectRoot $TestRoot}
$env:CONROI_TEST_ROOT=$TestRoot
if([string]::IsNullOrWhiteSpace($OutputDirectory)){$OutputDirectory=Join-Path $TestRoot ("KetQua/Unity/"+$Tag)}
elseif(-not [System.IO.Path]::IsPathRooted($OutputDirectory)){$OutputDirectory=Join-Path $projectRoot $OutputDirectory}
if(Get-Process Unity -ErrorAction SilentlyContinue){throw "Close the active Editor first"}
$log=Join-Path $OutputDirectory ($Tag+".log")
$report=Join-Path $OutputDirectory ($Tag+".json")
if((Test-Path -LiteralPath $log) -or (Test-Path -LiteralPath $report)){throw "Preserve existing evidence; choose another Tag/output"}
New-Item -ItemType Directory -Path $OutputDirectory -Force | Out-Null
$taskTemp=Join-Path $TestRoot ("Tam/Unity/"+$Tag)
New-Item -ItemType Directory -Path $taskTemp -Force | Out-Null
$env:TEMP=$taskTemp
$env:TMP=$taskTemp
$argsList=@("-projectPath",('"'+$projectRoot+'"'),"-logFile",('"'+$log+'"'))
if($Mode -ne "Open") {
 $entry=if($Mode -eq "Build"){"BuildDelayed"}else{"ProbeOpenDelayed"}
 $argsList+=@("-batchmode","-executeMethod",("KiemTraNguonVaBuild."+$entry),"-trial-report",('"'+$report+'"'))
 if($ReadOnly){$argsList+="-trial-read-only"}
 if($Mode -eq "Build"){
  if([string]::IsNullOrWhiteSpace($BuildOutput)){$BuildOutput=Join-Path $TestRoot ("BanChoiThu/"+$Tag)}
  elseif(-not [System.IO.Path]::IsPathRooted($BuildOutput)){$BuildOutput=Join-Path $projectRoot $BuildOutput}
  if(Test-Path -LiteralPath $BuildOutput){throw "Refuse existing player output"}
  $argsList+=@("-trial-output",('"'+$BuildOutput+'"'))
 }
}
$p=Start-Process -FilePath "D:/data_unity/6000.6.2f1/Editor/Unity.exe" -ArgumentList $argsList -WorkingDirectory $projectRoot -WindowStyle Hidden -PassThru
@{pid=$p.Id;project=$projectRoot;mode=$Mode;tag=$Tag;testRoot=$TestRoot;report=$report;utc=(Get-Date).ToUniversalTime().ToString("o")} | ConvertTo-Json | Set-Content -LiteralPath (Join-Path $OutputDirectory ($Tag+"-launch.json")) -Encoding UTF8
Write-Output ("Unity PID "+$p.Id)
