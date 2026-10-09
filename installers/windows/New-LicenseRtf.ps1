#!/usr/bin/env pwsh
# Converts the repository LICENSE (GNU GPL v3) into the RTF document shown by
# the MSI wizard license page (WixUILicenseRtf).
#
# The RTF is regenerated on every installer build so it never drifts from the
# license text stored at the repository root.
#
# Examples:
#   ./installers/windows/New-LicenseRtf.ps1
#   ./installers/windows/New-LicenseRtf.ps1 -OutputPath ./License.rtf

[CmdletBinding()]
param(
	[string]$LicensePath,

	[string]$OutputPath
)

$ErrorActionPreference = 'Stop'

$repoRoot = Split-Path -Parent (Split-Path -Parent $PSScriptRoot)

if (-not $LicensePath) { $LicensePath = Join-Path $repoRoot 'LICENSE' }
if (-not $OutputPath) { $OutputPath = Join-Path $PSScriptRoot 'License.rtf' }

if (-not (Test-Path $LicensePath)) {
	throw "License file not found at '$LicensePath'."
}

function ConvertTo-RtfText {
	param([string]$Text)

	$escaped = $Text -replace '\\', '\\\\' -replace '\{', '\{' -replace '\}', '\}'

	$builder = [System.Text.StringBuilder]::new()
	foreach ($char in $escaped.ToCharArray()) {
		if ([int]$char -gt 127) {
			# Non ASCII characters must be emitted as signed 16 bit escapes.
			[void]$builder.Append('\u' + [int]$char + '?')
		}
		else {
			[void]$builder.Append($char)
		}
	}

	return $builder.ToString()
}

$lines = Get-Content -LiteralPath $LicensePath

$body = [System.Text.StringBuilder]::new()
foreach ($line in $lines) {
	# Leading spaces are meaningful in the GPL layout; keep them as RTF spaces.
	$content = ConvertTo-RtfText ($line.TrimEnd())
	$content = [regex]::Replace($content, '^( +)', { '\~' * $args[0].Groups[1].Length })
	[void]$body.AppendLine($content + '\line')
}

$rtf = @(
	'{\rtf1\ansi\ansicpg1252\deff0\nouicompat{\fonttbl{\f0\fnil\fcharset0 Consolas;}}'
	'\viewkind4\uc1'
	'\pard\f0\fs16'
	$body.ToString().TrimEnd()
	'}'
) -join "`r`n"

Set-Content -LiteralPath $OutputPath -Value $rtf -Encoding ascii -NoNewline

Write-Host "License page generated at: $OutputPath" -ForegroundColor Green
