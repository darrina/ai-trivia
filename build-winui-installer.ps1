[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Release'
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent $PSCommandPath
$appProject = Join-Path $repoRoot 'AITrivia.WinUI\AITrivia.WinUI.csproj'
$installerProject = Join-Path $repoRoot 'AITrivia.WinUI.Installer\AITrivia.WinUI.Installer.wixproj'
$installerBinDir = Join-Path $repoRoot "AITrivia.WinUI.Installer\bin\$Configuration\x64"
$installerObjDir = Join-Path $repoRoot "AITrivia.WinUI.Installer\obj\x64\$Configuration"
$publishDir = Join-Path $installerBinDir 'app-publish'

foreach ($path in @($installerBinDir, $installerObjDir)) {
    if (Test-Path $path) {
        Remove-Item $path -Recurse -Force
    }
}

dotnet publish $appProject `
    -c $Configuration `
    -p:Platform=x64 `
    -r win-x64 `
    --self-contained true `
    -p:PublishDir=$publishDir `
    -p:SatelliteResourceLanguages=en-US `
    -p:DebugSymbols=false `
    -p:DebugType=None

Get-ChildItem $publishDir -Directory |
    Where-Object {
        $_.Name -ne 'en-us' -and
        (Get-ChildItem $_.FullName -File -Filter '*.mui' -ErrorAction SilentlyContinue)
    } |
    Remove-Item -Recurse -Force

dotnet build $installerProject `
    -c $Configuration `
    -p:Platform=x64 `
    -p:PublishDir=$publishDir
