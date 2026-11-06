# Gửi input OS cho cửa sổ Editor được chỉ định; kiểm foreground trước thao tác.
param([int]$EditorId = 27952, [switch]$RefreshOnly, [string]$TestRoot=$env:CONROI_TEST_ROOT)
$ErrorActionPreference = 'Stop'
Add-Type @'
using System;
using System.Runtime.InteropServices;
public static class ConRoiNativeInput {
 [DllImport("user32.dll")] public static extern bool SetForegroundWindow(IntPtr h);
 [DllImport("user32.dll")] public static extern bool ShowWindow(IntPtr h,int cmd);
 [DllImport("user32.dll")] public static extern IntPtr GetForegroundWindow();
 [DllImport("user32.dll")] public static extern uint GetWindowThreadProcessId(IntPtr h, out uint process);
 [DllImport("kernel32.dll")] public static extern uint GetCurrentThreadId();
 [DllImport("user32.dll")] public static extern bool AttachThreadInput(uint a,uint b,bool attach);
 [DllImport("user32.dll")] public static extern IntPtr SetFocus(IntPtr h);
 [DllImport("user32.dll")] public static extern void keybd_event(byte vk,byte scan,uint flags,UIntPtr extra);
}
'@
$editor = Get-Process -Id $EditorId
$handle = $editor.MainWindowHandle
if ($handle -eq 0) { throw 'No Editor window available' }
[ConRoiNativeInput]::ShowWindow($handle,9) | Out-Null
[ConRoiNativeInput]::ShowWindow($handle,5) | Out-Null
[uint32]$frontProcess=0
$frontThread=[ConRoiNativeInput]::GetWindowThreadProcessId([ConRoiNativeInput]::GetForegroundWindow(),[ref]$frontProcess)
$myThread=[ConRoiNativeInput]::GetCurrentThreadId()
$attached=[ConRoiNativeInput]::AttachThreadInput($myThread,$frontThread,$true)
[ConRoiNativeInput]::SetForegroundWindow($handle) | Out-Null
[ConRoiNativeInput]::SetFocus($handle) | Out-Null
if ($attached) { [ConRoiNativeInput]::AttachThreadInput($myThread,$frontThread,$false) | Out-Null }
Start-Sleep -Milliseconds 500
if ([ConRoiNativeInput]::GetForegroundWindow() -ne $handle) { throw 'Cannot focus Editor OS window' }
if ($RefreshOnly) {
 try {
  [ConRoiNativeInput]::keybd_event(17,0,0,[UIntPtr]::Zero)
  [ConRoiNativeInput]::keybd_event(82,0,0,[UIntPtr]::Zero)
 } finally {
  [ConRoiNativeInput]::keybd_event(82,0,2,[UIntPtr]::Zero)
  [ConRoiNativeInput]::keybd_event(17,0,2,[UIntPtr]::Zero)
 }
 return
}
$projectRoot=Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
if([string]::IsNullOrWhiteSpace($TestRoot)){$TestRoot=Join-Path $projectRoot 'Test'}
elseif(-not [System.IO.Path]::IsPathRooted($TestRoot)){$TestRoot=Join-Path $projectRoot $TestRoot}
$bridge=Join-Path $TestRoot 'KetQua/W_route_Unity_v30_20261016/editor_live'
New-Item -ItemType Directory -Path $bridge -Force | Out-Null
$result = @()
foreach ($key in @(87,83,65,68)) {
 try {
  [ConRoiNativeInput]::keybd_event([byte]$key,0,0,[UIntPtr]::Zero)
  @{action='runtime_inspect';nonce=[guid]::NewGuid().ToString()} | ConvertTo-Json | Set-Content "$bridge/command.json" -Encoding UTF8
  Start-Sleep -Milliseconds 1600
  $state = Get-Content "$bridge/runtime_state.json" -Raw | ConvertFrom-Json
  $result += @{ key=$key; state=$state; source='OS keybd_event; synthetic OS input, not human input' }
 } finally { [ConRoiNativeInput]::keybd_event([byte]$key,0,2,[UIntPtr]::Zero) }
 Start-Sleep -Milliseconds 200
}
$result | ConvertTo-Json -Depth 8 | Set-Content "$bridge/os_input_probe.json" -Encoding UTF8
$result | ConvertTo-Json -Depth 8
