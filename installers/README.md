# Instaladores de qrgen

Esta carpeta contiene los instaladores tipo *wizard* de `qrgen`, organizados por
plataforma. El proyecto .NET (`QRGenerator/QRGenerator.csproj`) es
multiplataforma: no fija un `RuntimeIdentifier`, sino que declara los RIDs
soportados y el runtime se elige al publicar.

```
installers/
  windows/   Instalador MSI (WiX Toolset v6)   -> implementado
  macos/     Instalador .pkg                   -> pendiente
  linux/     Paquetes .deb / .rpm              -> pendiente
```

## 1. Publicar los artefactos portables

`build/Publish-QrGen.ps1` funciona en Windows, macOS y Linux con PowerShell 7+.

```powershell
# Detecta automaticamente el RID de la maquina actual
./build/Publish-QrGen.ps1

# RID explicito
./build/Publish-QrGen.ps1 -Runtime win-x64
./build/Publish-QrGen.ps1 -Runtime osx-arm64
./build/Publish-QrGen.ps1 -Runtime linux-x64

# Variante liviana que requiere el runtime .NET 10 instalado
./build/Publish-QrGen.ps1 -Runtime win-x64 -FrameworkDependent
```

Salida: `artifacts/<rid>` (o `artifacts/<rid>-fx` para framework-dependent).
Por defecto se genera un ejecutable unico **self-contained**, por lo que la
maquina destino no necesita tener .NET instalado.

## 2. Windows: instalador MSI

```powershell
./installers/windows/Build-Msi.ps1
./installers/windows/Build-Msi.ps1 -Runtime win-arm64 -ProductVersion 1.2.0
./installers/windows/Build-Msi.ps1 -FrameworkDependent
```

El MSI queda en `artifacts/installers/`.

### Comportamiento del wizard

El paquete se declara con `Scope="perUserOrMachine"` y usa el conjunto de
dialogos `WixUI_Advanced`, por lo que el usuario ve una pagina donde elige:

| Opcion | Carpeta | PATH | Elevacion |
| --- | --- | --- | --- |
| Solo para mi | `%LOCALAPPDATA%\Programs\qrgen` | PATH de usuario | No |
| Para todos los usuarios | `%ProgramFiles%\qrgen` | PATH de sistema | Si (UAC) |

La entrada de PATH se elimina automaticamente al desinstalar desde
*Aplicaciones instaladas* de Windows. Los cambios de PATH aplican a terminales
abiertas despues de la instalacion.

### Instalacion silenciosa

```powershell
msiexec /i qrgen-setup-1.0.0-win-x64.msi /qn                  # por usuario
msiexec /i qrgen-setup-1.0.0-win-x64.msi /qn ALLUSERS=1       # todos (elevado)
msiexec /x qrgen-setup-1.0.0-win-x64.msi /qn                  # desinstalar
```

### Requisitos de compilacion

Solo el SDK de .NET 10 y acceso a NuGet: `WixToolset.Sdk` y
`WixToolset.UI.wixext` se restauran como paquetes. El `.wixproj` no forma parte
de `QRGenerator.slnx` para no afectar la compilacion normal de la solucion.

## 3. Siguientes plataformas

Ver `installers/macos/README.md` e `installers/linux/README.md`.
