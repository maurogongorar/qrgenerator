# Instalador de qrgen para Linux (pendiente)

En Linux la instalacion "para todos los usuarios" es el modelo nativo de los
gestores de paquetes, y la instalacion "solo para mi" se resuelve con un
directorio dentro del home del usuario.

## Formatos previstos

| Formato | Alcance | Notas |
| --- | --- | --- |
| `.deb` / `.rpm` | Todos los usuarios | Requiere `sudo`; instala en `/opt/qrgen` con symlink en `/usr/local/bin/qrgen`. Los centros de software (GNOME Software, Discover) los presentan con asistente grafico. |
| `.tar.gz` + script | Solo el usuario | Instala en `~/.local/share/qrgen` con symlink en `~/.local/bin/qrgen`. |

## Pasos de implementacion

1. Publicar artefactos: `./build/Publish-QrGen.ps1 -Runtime linux-x64`
   (repetir con `linux-arm64`).
2. Generar los paquetes con `dotnet-deb`/`dotnet-rpm`, `fpm` o `nfpm` a partir
   de `artifacts/<rid>`.
3. Incluir scripts `postinst`/`postrm` que creen y eliminen el symlink
   `/usr/local/bin/qrgen`.
4. Para el modo por usuario, agregar `~/.local/bin` al PATH escribiendo en
   `~/.profile` solo si aun no esta presente.
5. Opcional: publicar como AppImage o paquete Flatpak para una experiencia de
   instalacion grafica uniforme entre distribuciones.

## Nota

El ejecutable se llama `qrgen` (sin extension) y debe conservar el bit de
ejecucion; `Publish-QrGen.ps1` ya aplica `chmod +x` al publicar en Unix.
