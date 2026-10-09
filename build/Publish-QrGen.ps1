#!/usr/bin/env pwsh
# Publishes the portable qrgen artifacts for a specific runtime.
#
# Cross-platform script (PowerShell 7+, works on Windows, macOS and Linux).
# It produces the executable in 'artifacts/<Runtime>[-fx]'.
#
# By default the publish is self-contained (the target machine does not need
# .NET installed). Use -FrameworkDependent to produce a lightweight artifact
# that depends on the installed .NET 10 runtime.
#
# Examples:
#   ./build/Publish-QrGen.ps1
#   ./build/Publish-QrGen.ps1 -Runtime osx-arm64
#   ./build/Publish-QrGen.ps1 -Runtime linux-x64 -FrameworkDependent

[CmdletBinding()]
param(
	[ValidateSet('win-x64', 'win-arm64', 'linux-x64', 'linux-arm64', 'osx-x64', 'osx-arm64')]
	[string]$Runtime,

	[string]$Configuration = 'Release',

	[switch]$FrameworkDependent,

	[string]$OutputDirectory
)

$ErrorActionPreference = 'Stop'

function Get-DefaultRuntime {
	$arch = [System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture
	$suffix = if ($arch -eq 'Arm64') { 'arm64' } else { 'x64' }

	if ($IsWindows) { return "win-$suffix" }
	if ($IsMacOS) { return "osx-$suffix" }
	return "linux-$suffix"
}

if (-not $Runtime) {
	$Runtime = Get-DefaultRuntime
}

$repoRoot = Split-Path -Parent $PSScriptRoot
$project = Join-Path $repoRoot (Join-Path 'src' (Join-Path 'Cocosoft.Cli.Tools.QRGenerator' 'Cocosoft.Cli.Tools.QRGenerator.csproj'))

if (-not $OutputDirectory) {
	$folder = if ($FrameworkDependent) { "$Runtime-fx" } else { $Runtime }
	$OutputDirectory = Join-Path $repoRoot 'artifacts' $folder
}

if (Test-Path $OutputDirectory) {
	Remove-Item $OutputDirectory -Recurse -Force
}

$selfContained = (-not $FrameworkDependent).ToString().ToLowerInvariant()

Write-Host "Publishing qrgen" -ForegroundColor Cyan
Write-Host "  Runtime       : $Runtime"
Write-Host "  Configuration : $Configuration"
Write-Host "  Self-contained: $selfContained"
Write-Host "  Output        : $OutputDirectory"

dotnet publish $project --configuration $Configuration --runtime $Runtime --self-contained $selfContained --output $OutputDirectory

if ($LASTEXITCODE -ne 0) {
	throw "dotnet publish failed with exit code $LASTEXITCODE."
}

# On Unix the executable must keep the execution bit.
if (-not $IsWindows) {
	$exe = Join-Path $OutputDirectory 'qrgen'
	if (Test-Path $exe) {
		chmod +x $exe
	}
}

Write-Host "Artifacts ready at: $OutputDirectory" -ForegroundColor Green
