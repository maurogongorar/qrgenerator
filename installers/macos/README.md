# qrgen macOS Installer (pending)

Planned approach to provide the same wizard-style installation experience as on Windows.

## Format

`.pkg` installer (Distribution package) generated with `pkgbuild` + `productbuild`. The macOS graphical
installer (`Installer.app`) natively provides destination selection: *Install for me only* versus *Install for all users of the computer*,
which is the equivalent of the MSI scope selection page and requests administrator authentication when required.

## Implementation Steps

1. Publish artifacts: `./build/Publish-QrGen.ps1 -Runtime osx-arm64` (repeat
   with `osx-x64` and combine them into a universal binary with `lipo` if desired).
2. `pkgbuild --root artifacts/osx-arm64 --identifier com.qrgenerator.qrgen --version 1.0.0 --install-location /usr/local/qrgen qrgen-component.pkg`
3. `productbuild --distribution Distribution.xml --package-path . qrgen.pkg`
   with `<domains enable_anywhere="true" enable_currentUserHome="true" enable_localSystem="true"/>`
   to enable scope selection.
4. Add a `postinstall` script to create the command symlink:
   - All users: `/usr/local/bin/qrgen` (already included in the system PATH).
   - Current user only: `~/.local/bin/qrgen` and add that path to
	 `~/.zprofile` if it is not already in the PATH.
5. Sign and notarize with `productsign` + `notarytool` to avoid Gatekeeper warnings.

## Note

The executable is named `qrgen` (without an extension) and must retain its executable bit;
`Publish-QrGen.ps1` already applies `chmod +x` when publishing on Unix.
