param([Parameter(Mandatory=$true)][string]$HandleFile)
Add-Type -AssemblyName System.Windows.Forms
$probeForm = New-Object System.Windows.Forms.Form
$probeForm.Text = "ConRoi TEST ONLY external focus probe"
$probeForm.Width = 420
$probeForm.Height = 160
$probeForm.StartPosition = "CenterScreen"
$probeLabel = New-Object System.Windows.Forms.Label
$probeLabel.Text = "TEST ONLY - input focus isolation. No gameplay commands."
$probeLabel.Dock = "Fill"
$probeForm.Controls.Add($probeLabel)
$probeForm.Add_Shown({[IO.File]::WriteAllText($HandleFile, $probeForm.Handle.ToInt64().ToString())})
$probeTimer = New-Object System.Windows.Forms.Timer
$probeTimer.Interval = 180000
$probeTimer.Add_Tick({$probeForm.Close()})
$probeTimer.Start()
[System.Windows.Forms.Application]::Run($probeForm)
$probeTimer.Dispose()
$probeForm.Dispose()
