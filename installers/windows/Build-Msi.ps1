#!/usr/bin/env pwsh
# Builds the qrgen MSI installer (wizard) for Windows.
#
# It first publishes the portable artifacts and then builds the WiX v6 package.
# The resulting MSI shows a wizard that lets the user choose between installing
# "just for me" (no elevation) or "for all users" (elevates with UAC), and
# registers the installation folder in the matching PATH scope.
#
# Requirements: .NET SDK 10 and NuGet connectivity (the WiX SDK is restored).
#
# Examples:
#   ./installers/windows/Build-Msi.ps1
#   ./installers/windows/Build-Msi.ps1 -Runtime win-arm64 -ProductVersion 1.2.0
#   ./installers/windows/Build-Msi.ps1 -FrameworkDependent

[CmdletBinding()]
param(
	[ValidateSet('win-x64', 'win-arm64')]
	[string]$Runtime = 'win-x64',

	[string]$ProductVersion = '1.0.0',

	[string]$Manufacturer = 'QRGenerator',

	[string]$Configuration = 'Release',

	[switch]$FrameworkDependent,

	[switch]$SkipPublish
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)
$folder = if ($FrameworkDependent) { "$Runtime-fx" } else { $Runtime }
$publishDir = Join-Path $repoRoot 'artifacts' $folder
$outputDir = Join-Path $repoRoot 'artifacts' 'installers'
$wixProject = Join-Path $PSScriptRoot 'QRGenerator.Installer.wixproj'

# Keep the wizard license page in sync with the repository LICENSE (GNU GPL v3).
& (Join-Path $PSScriptRoot 'New-LicenseRtf.ps1')

if (-not $SkipPublish) {
	$publishScript = Join-Path $repoRoot 'build' 'Publish-QrGen.ps1'
	& $publishScript -Runtime $Runtime -Configuration $Configuration -FrameworkDependent:$FrameworkDependent
}

if (-not (Test-Path (Join-Path $publishDir 'qrgen.exe'))) {
	throw "'qrgen.exe' was not found in '$publishDir'. Run the publish step first."
}

$platform = if ($Runtime -eq 'win-arm64') { 'arm64' } else { 'x64' }

New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

Write-Host "Building MSI installer" -ForegroundColor Cyan
Write-Host "  Runtime : $Runtime"
Write-Host "  Version : $ProductVersion"
Write-Host "  Source  : $publishDir"
Write-Host "  Output  : $outputDir"

dotnet build $wixProject -c $Configuration -p:InstallerPlatform=$platform -p:ProductVersion=$ProductVersion -p:Manufacturer=$Manufacturer -p:PublishDir=$publishDir -p:OutputPath=$outputDir -p:OutputName="qrgen-setup-$ProductVersion-$Runtime"

if ($LASTEXITCODE -ne 0) {
	throw "Installer build failed with exit code $LASTEXITCODE."
}

Write-Host "Installer generated at: $outputDir" -ForegroundColor Green
