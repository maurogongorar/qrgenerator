# qrgen Installers

This folder contains the wizard-style installers for `qrgen`, organized by
platform. The .NET project (`QRGenerator/QRGenerator.csproj`) is
cross-platform: it does not specify a fixed `RuntimeIdentifier`, but instead
declares the supported RIDs, and the runtime is selected at publish time.

```
installers/
  windows/   MSI installer (WiX Toolset v6)   -> implemented
  macos/     .pkg installer                   -> pending
  linux/     .deb / .rpm packages             -> pending
```

## 1. Publishing Portable Artifacts

`build/Publish-QrGen.ps1` works on Windows, macOS, and Linux with PowerShell 7+..

```powershell
# Automatically detects the RID of the current machine
./build/Publish-QrGen.ps1

# Explicit RID
./build/Publish-QrGen.ps1 -Runtime win-x64
./build/Publish-QrGen.ps1 -Runtime osx-arm64
./build/Publish-QrGen.ps1 -Runtime linux-x64

# Lightweight variant that requires .NET 10 to be installed
./build/Publish-QrGen.ps1 -Runtime win-x64 -FrameworkDependent
```

Output: artifacts/<rid> (or artifacts/<rid>-fx for framework-dependent publishing).
By default, a single self-contained executable is generated, so the
target machine does not need to have .NET installed.

## 2. Windows: MSI installer

```powershell
./installers/windows/Build-Msi.ps1
./installers/windows/Build-Msi.ps1 -Runtime win-arm64 -ProductVersion 1.2.0
./installers/windows/Build-Msi.ps1 -FrameworkDependent
```

The MSI installer is generated in `artifacts/installers/`.

### Wizard Behavior

The package is declared with `Scope="perUserOrMachine"` and uses the
`WixUI_Advanced` dialog set, so the user is presented with a page where they
can choose:

| Option | Folder | PATH | Elevation |
| --- | --- | --- | --- |
| SJust for me | `%LOCALAPPDATA%\Programs\qrgen` | User PATH | No |
| For all users | `%ProgramFiles%\qrgen` | System PATH | Yes (UAC) |

The PATH entry is automatically removed when uninstalling from Windows
*Installed apps*. PATH changes apply to terminals opened after the
installation.

### Silent Installation

```powershell
msiexec /i qrgen-setup-1.0.0-win-x64.msi /qn                  # per-user
msiexec /i qrgen-setup-1.0.0-win-x64.msi /qn ALLUSERS=1       # per-machine (elevated)
msiexec /x qrgen-setup-1.0.0-win-x64.msi /qn                  # uninstall
```

### Build Requirements

Only the .NET 10 SDK and access to NuGet are required: `WixToolset.Sdk` and
`WixToolset.UI.wixext` are restored as packages. The `.wixproj` is not included
in `QRGenerator.slnx` to avoid affecting the normal solution build.

## 3. Next Platforms

See `installers/macos/README.md` and `installers/linux/README.md`.
