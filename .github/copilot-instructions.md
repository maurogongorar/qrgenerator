# Instrucciones del repositorio — QRGenerator (`qrgen`)

## Contexto del proyecto

Utilitario de consola .NET 10 que genera codigos QR. El ejecutable se llama
`qrgen` (definido por `AssemblyName` en `QRGenerator/QRGenerator.csproj`).

**Regla clave: el proyecto debe permanecer multiplataforma.** Nunca fijar
`RuntimeIdentifier` ni condicionar codigo/propiedades a Windows en el
`.csproj`. Los RIDs soportados se declaran en `RuntimeIdentifiers`
(`win-x64;win-arm64;linux-x64;linux-arm64;osx-x64;osx-arm64`) y el runtime se
elige al publicar.

## Estructura de distribucion

```
build/Publish-QrGen.ps1      Publicacion portable multiplataforma (PowerShell 7+)
installers/README.md         Guia general y hoja de ruta
installers/windows/          Instalador MSI (WiX Toolset v6)  -> IMPLEMENTADO
installers/macos/            Instalador .pkg                  -> PENDIENTE
installers/linux/            Paquetes .deb / .rpm / tar.gz    -> PENDIENTE
artifacts/<rid>              Salida de la publicacion (ignorado por git)
artifacts/installers/        Instaladores generados (ignorado por git)
```

## Decisiones tomadas (no revertir sin consultar)

- **Instalador tipo wizard, no scripts.** Se descartaron los scripts
  `install.ps1`/`uninstall.ps1` en favor de instaladores nativos con asistente
  grafico en cada plataforma.
- **Windows: WiX Toolset v6 (MSI)**, elegido por compatibilidad con despliegue
  empresarial/GPO.
- **Distribucion:** self-contained single-file por defecto;
  framework-dependent disponible con `-FrameworkDependent`.
- **Comando en terminal:** `qrgen`.
- **Doble alcance con eleccion del usuario:** pagina de seleccion en el wizard
  entre "solo para mi" (sin elevacion) y "todos los usuarios" (UAC / sudo / 
  autenticacion de admin).
- El `.wixproj` NO se agrega a `QRGenerator.slnx` para no afectar la
  compilacion normal de la solucion.

## Estado: Windows (completado)

`installers/windows/Package.wxs` usa `Scope="perUserOrMachine"` y el conjunto
de dialogos `WixUI_Advanced`, que ya aporta la pagina de seleccion de alcance:

| Opcion | Carpeta | PATH | Elevacion |
| --- | --- | --- | --- |
| Solo para mi | `%LOCALAPPDATA%\Programs\qrgen` | PATH de usuario | No |
| Todos los usuarios | `%ProgramFiles%\qrgen` | PATH de sistema | Si (UAC) |

Los archivos publicados se incorporan con `<Files Include="$(var.PublishDir)\**" />`
y el PATH se registra con dos componentes `Environment` condicionados por
`ALLUSERS`, de modo que se limpian solos al desinstalar.

Comando de build: `./installers/windows/Build-Msi.ps1`

### Aprendizajes de WiX v6 (evitar reintentos)

- La condicion de un `Component` es el **atributo** `Condition`, no un elemento
  hijo (error `WIX0005` si se usa como elemento).
- No redefinir `WixUISupportPerUser` / `WixUISupportPerMachine`: ya son
  simbolos virtuales de `WixUI_Advanced` (error `WIX7009`).
- `ICE105` es un falso positivo en paquetes de doble alcance con un componente
  HKLM condicionado; se suprime con `<SuppressIces>ICE105</SuppressIces>`.
- `WixUI_Advanced` requiere `WixUILicenseRtf` (ver `installers/windows/License.rtf`).

## Trabajo pendiente: macOS y Linux

Continuar con los instaladores de las plataformas restantes manteniendo la
misma experiencia de wizard y la eleccion de alcance. Los planes detallados
estan en `installers/macos/README.md` e `installers/linux/README.md`:

1. **macOS** — `.pkg` con `pkgbuild` + `productbuild`; habilitar
   `<domains enable_currentUserHome="true" enable_localSystem="true"/>` en
   `Distribution.xml` para la seleccion de destino; script `postinstall` que
   cree el symlink en `/usr/local/bin/qrgen` o `~/.local/bin/qrgen`; firmar y
   notarizar.
2. **Linux** — `.deb`/`.rpm` (todos los usuarios, `/opt/qrgen` + symlink en
   `/usr/local/bin`) y `.tar.gz` con instalacion por usuario en
   `~/.local/share/qrgen` + `~/.local/bin`. Considerar AppImage/Flatpak para
   una experiencia grafica uniforme.

En ambos casos el ejecutable `qrgen` no tiene extension y debe conservar el
bit de ejecucion (`Publish-QrGen.ps1` ya aplica `chmod +x` en Unix).

## Convenciones

- Los scripts de build usan PowerShell 7+ (`#!/usr/bin/env pwsh`) para poder
  ejecutarse en las tres plataformas.
- Los scripts siguen la convencion `Verbo-Sustantivo.ps1` de PowerShell.
- Comentarios y mensajes de los scripts/instaladores en espanol sin acentos
  (compatibilidad de codificacion en consolas y en el MSI).
