# Instalador de qrgen para macOS (pendiente)

Plan previsto para mantener la misma experiencia de wizard que en Windows.

## Formato

Instalador `.pkg` (Distribution package) generado con `pkgbuild` +
`productbuild`. El asistente grafico de macOS (`Installer.app`) ofrece de forma
nativa la seleccion de destino: *Instalar solo para mi* frente a *Instalar para
todos los usuarios del equipo*, que es el equivalente de la pagina de alcance
del MSI y solicita autenticacion de administrador cuando corresponde.

## Pasos de implementacion

1. Publicar artefactos: `./build/Publish-QrGen.ps1 -Runtime osx-arm64` (repetir
   con `osx-x64` y combinar en un binario universal con `lipo` si se desea).
2. `pkgbuild --root artifacts/osx-arm64 --identifier com.qrgenerator.qrgen
   --version 1.0.0 --install-location /usr/local/qrgen qrgen-component.pkg`
3. `productbuild --distribution Distribution.xml --package-path . qrgen.pkg`
   con `<domains enable_anywhere="true" enable_currentUserHome="true"
   enable_localSystem="true"/>` para habilitar la seleccion de alcance.
4. Script `postinstall` que cree el symlink del comando:
   - Todos los usuarios: `/usr/local/bin/qrgen` (ya esta en el PATH del sistema).
   - Solo el usuario: `~/.local/bin/qrgen` y agregar esa ruta a
	 `~/.zprofile` si aun no esta en el PATH.
5. Firmar y notarizar con `productsign` + `notarytool` para evitar Gatekeeper.

## Nota

El ejecutable se llama `qrgen` (sin extension) y debe conservar el bit de
ejecucion; `Publish-QrGen.ps1` ya aplica `chmod +x` al publicar en Unix.
