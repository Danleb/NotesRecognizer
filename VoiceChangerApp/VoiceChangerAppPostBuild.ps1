# Copies Suprecessor native library to the managed project folder.

Write-Output "`r`nSTART VoiceChangerApp post build event script =============================================================================================================================="

$solutionDir = $args[0]
$targetDir = $args[1]
$buildType = $args[2]

Write-Output "Solution directory:" $solutionDir
Write-Output "Target directory:" $targetDir

$suprecessorLibPath = "$($solutionDir)Suprecessor\out\build\x64-$($buildType)\SuprecessorApi.dll"
Write-Output "Suprecessor lib path: "$suprecessorLibPath

if (Test-Path -Path $suprecessorLibPath -PathType Leaf)
{
    Copy-Item $suprecessorLibPath -Destination $targetDir
    Write-Host "SuprecessorApi library copied successfully."
}
 else
 {
    Write-Host "Cannot copy SuprecessorApi library, because file doesn't exist."
    exit 1
 }

Write-Output "FINISH VoiceChangerApp post build event script ==============================================================================================================================`r`n"