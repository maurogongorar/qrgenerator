#!/usr/bin/env pwsh
# Publica los artefactos portables de qrgen para un runtime concreto.
#
# Script multiplataforma (PowerShell 7+, funciona en Windows, macOS y Linux).
# Genera el ejecutable en 'artifacts/<Runtime>[-fx]'.
#
# Por defecto la publicacion es self-contained (no requiere tener .NET instalado
# en la maquina destino). Use -FrameworkDependent para producir un artefacto
# liviano que dependa del runtime .NET 10 instalado.
#
# Ejemplos:
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

Write-Host "Publicando qrgen" -ForegroundColor Cyan
Write-Host "  Runtime       : $Runtime"
Write-Host "  Configuracion : $Configuration"
Write-Host "  Self-contained: $selfContained"
Write-Host "  Salida        : $OutputDirectory"

dotnet publish $project --configuration $Configuration --runtime $Runtime --self-contained $selfContained --output $OutputDirectory

if ($LASTEXITCODE -ne 0) {
	throw "dotnet publish fallo con codigo $LASTEXITCODE."
}

# En Unix el ejecutable debe conservar el bit de ejecucion.
if (-not $IsWindows) {
	$exe = Join-Path $OutputDirectory 'qrgen'
	if (Test-Path $exe) {
		chmod +x $exe
	}
}

Write-Host "Artefactos listos en: $OutputDirectory" -ForegroundColor Green
