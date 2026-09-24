#!/usr/bin/env pwsh
# Genera el instalador MSI (wizard) de qrgen para Windows.
#
# Publica primero los artefactos portables y luego compila el paquete WiX v6.
# El MSI resultante muestra un wizard que permite elegir entre instalar
# "solo para mi" (sin elevacion) o "para todos los usuarios" (eleva con UAC),
# y registra la carpeta de instalacion en el PATH correspondiente.
#
# Requisitos: .NET SDK 10 y conexion a NuGet (el SDK de WiX se restaura solo).
#
# Ejemplos:
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

if (-not $SkipPublish) {
	$publishScript = Join-Path $repoRoot 'build' 'Publish-QrGen.ps1'
	& $publishScript -Runtime $Runtime -Configuration $Configuration -FrameworkDependent:$FrameworkDependent
}

if (-not (Test-Path (Join-Path $publishDir 'qrgen.exe'))) {
	throw "No se encontro 'qrgen.exe' en '$publishDir'. Ejecute la publicacion primero."
}

$platform = if ($Runtime -eq 'win-arm64') { 'arm64' } else { 'x64' }

New-Item -ItemType Directory -Path $outputDir -Force | Out-Null

Write-Host "Compilando instalador MSI" -ForegroundColor Cyan
Write-Host "  Runtime  : $Runtime"
Write-Host "  Version  : $ProductVersion"
Write-Host "  Origen   : $publishDir"
Write-Host "  Salida   : $outputDir"

dotnet build $wixProject -c $Configuration -p:InstallerPlatform=$platform -p:ProductVersion=$ProductVersion -p:Manufacturer=$Manufacturer -p:PublishDir=$publishDir -p:OutputPath=$outputDir -p:OutputName="qrgen-setup-$ProductVersion-$Runtime"

if ($LASTEXITCODE -ne 0) {
	throw "La compilacion del instalador fallo con codigo $LASTEXITCODE."
}

Write-Host "Instalador generado en: $outputDir" -ForegroundColor Green
