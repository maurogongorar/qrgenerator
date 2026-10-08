<div align="center">
# qrgen — QR Generator

**A free, open-source, cross-platform command-line tool for generating QR codes.**

[![License: GPL v3](https://img.shields.io/badge/License-GPLv3-blue.svg)](LICENSE)
[![.NET](https://img.shields.io/badge/.NET-10.0-512BD4.svg)](https://dotnet.microsoft.com/)
[![Platforms](https://img.shields.io/badge/platforms-Windows%20%7C%20macOS%20%7C%20Linux-lightgrey.svg)](#instalacion)

</div>

---

## Description

`qrgen` generates QR codes directly on your machine, with no cloud services or
intermediaries. Free online generators often shorten or redirect the link you encode,
which means a third party may be able to track, modify, or even disable your QR
code. With `qrgen`, the content you enter is exactly the content that gets encoded.

### Project Principles

- **No intermediaries.** The link or text is encoded exactly as provided and never
  passes through a third-party server.
- **100% offline.** No internet connection or user account is required.
- **Free and open forever.** Non-profit, with no paid plans or telemetry. Published
  under GPL-3.0.
- **Cross-platform.** Windows, macOS, and Linux (x64 and arm64) with the same
  experience.

## Features

- Generate QR codes from text or URLs.
- Dedicated command for WhatsApp links with a pre-filled message.
- Customize dark and light colors (using a name such as `Black` or a hex value
  such as `#FF5733`).
- Optional center icon: built-in icons (Instagram, TikTok, WhatsApp) or a custom
  SVG.
- Output to an image file at the path you specify.
- Single, self-contained executable: no .NET installation required.

## Instalation

### Windows (MSI)

The graphical wizard installer allows you to choose the installation scope:

| Option | Folder | PATH | Elevation |
| --- | --- | --- | --- |
| Just for me | `%LOCALAPPDATA%\Programs\qrgen` | User PATH | No |
| All users | `%ProgramFiles%\qrgen` | System PATH | Yes (UAC) |

To build it locally:

```powershell
./installers/windows/Build-Msi.ps1
```

### macOS and Linux

Native installers are currently under development (see [`installers/README.md`](installers/README.md)).
In the meantime, you can use the portable artifact:

```powershell
./build/Publish-QrGen.ps1 -Runtime osx-arm64   # or linux-x64, linux-arm64, osx-x64
```

The executable is located at `artifacts/<rid>/qrgen`. Copy it to a directory included
in your `PATH`, such as `~/.local/bin`.

## Usage

```bash
# QR code from text
qrgen "Hello Wrold!" -o qr.png

# Explicit subcommand for URLs
qrgen url "https://github.com/maurogongorar/qrgenerator" -o qrgen-repo.png

# WhatsApp QR code with a pre-filled message
qrgen whatsapp --phone-number 573001234567 "Hello, I would like more information" -o wa.png

# Custom colors
qrgen "text" -d "#1B1B1B" -l White -o qr.png

# Center icon
qrgen url "https://www.instagram.com/maurogongora" -i Instagram -o ig.png
qrgen url "https://www.tiktok.com/@mauro727" -i TikTok -o tk.png
qrgen "text" -i Custom --icon-path ./my-logo.svg -o logo.png
```

### Global Options

| Option | Alias | Description |
| --- | --- | --- |
| `--output-file` | `-o` | Path to the generated image file. |
| `--icon` | `-i` | Center icon: `Instagram`, `Tiktok`, `Whatsapp` or `Custom`. |
| `--icon-path` | | Path to the custom SVG when using `--icon Custom`. |
| `--dark-color` | `-d` | Dark color of the QR code (name or hex). |
| `--light-color` | `-l` | Light color / background of the QR code (name or hex). |

You can view the built-in help at any time:

```bash
qrgen --help
qrgen whatsapp --help
```

## Requirements

### To Run

- **Self-contained** artifact (default): None.
- **framework-dependent** artifact: [.NET 10 Runtime](https://dotnet.microsoft.com/download).

### Para compilar

- [.NET SDK 10.0](https://dotnet.microsoft.com/download) or later.
- [PowerShell 7+](https://github.com/PowerShell/PowerShell) for the scripts in `build/` and `installers/`.
- [WiX Toolset v6](https://wixtoolset.org/) only if you intend to build the Windows MSI installer.

## Build and Run from Source

```bash
git clone https://github.com/maurogongorar/qrgenerator.git
cd qrgenerator

dotnet build src/QRGenerator.slnx
dotnet run --project src/Cocosoft.Cli.Tools.QRGenerator -- "https://example.com" -o qr.png
```

Portable publishing for the current runtime:

```powershell
./build/Publish-QrGen.ps1
```

Use `-FrameworkDependent` to create a lightweight artifact that relies on the installed
.NET runtime.

## Repository Structure

```
build/                                   Cross-platform publishing script
installers/                              Native installers by platform
  windows/                               MSI with WiX Toolset v6
  macos/                                 .pkg (pending)
  linux/                                 .deb / .rpm / tar.gz (pending)
src/QRGenerator.slnx                     Solution
src/Cocosoft.Cli.Tools.QRGenerator/      Console application project (executable 'qrgen')
artifacts/                               Publishing output (ignored by Git)
```

## Roadmap

- [x] QR code generation for text and URLs
- [x] WhatsApp command
- [x] PColor and icon customization
- [x] Windows MSI installer
- [ ] Add unit tests
- [ ] macOS `.pkg` installer
- [ ] Linux `.deb` / `.rpm` / `.tar.gz` packages
- [ ] Additional data formats (vCard, Wi-Fi, email, SMS)
- [ ] Support for multiple image formats for the center logo

## How to Contribute

Contributions are welcome. To propose a change:

1. Open an issue describing the bug or enhancement before starting any major changes.
2. Fork the repository and create a descriptive branch: `git checkout -b feature/my-improvement`.
3. Implement the change and verify that it builds: `dotnet build src/QRGenerator.slnx`.
4. Write clear, imperative commit messages (e.g., `Add vCard command`).
5. Open a *Pull Request* targeting `develop` explaining the change and how to test it.

### Project Conventions

- The project must remain **cross-platform**: do not set `RuntimeIdentifier` or
  condition code or properties on a specific operating system.
- Scripts use PowerShell 7+ (`#!/usr/bin/env pwsh`) and follow the
  `Verb-Noun.ps1`.
- Comments and messages in scripts and installers must be in English.
- Follow the existing code style and keep changes as small and focused as
  possible.

## Reporting Issues

Open an [issue](https://github.com/maurogongorar/qrgenerator/issues) including the operating system and architecture, `qrgen` version,
command executed, expected result, and actual result.

## Security and Privacy

`qrgen` does not send data to any server, collect telemetry, or require an internet
connection. All processing takes place locally on your device.

## License

Distributed under the [GNU General Public License v3.0](LICENSE). This ensures that the project
and its derivatives remain free and open source.

## Acknowledgments

- [QRCoder](https://github.com/codebude/QRCoder) — QR code generation
- [Svg.Skia](https://github.com/wieslawsoltes/Svg.Skia) — SVG icon rendering
- [System.CommandLine](https://github.com/dotnet/command-line-api) — command-line interface
- [Serilog](https://serilog.net/) — logging
