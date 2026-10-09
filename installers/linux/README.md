# qrgen Linux Installer (pending)

On Linux, "system-wide installation" is the native model used by package
managers, while "per-user installation" is handled by using a directory
within the user's home directory.

## Planned Formats

| Format | Scope | Notes |
| --- | --- | --- |
| `.deb` / `.rpm` | All users | Requires `sudo`; installs to `/opt/qrgen` with a symlink in `/usr/local/bin/qrgen`. Software centers (GNOME Software, Discover) present them with a graphical wizard. |
| `.tar.gz` + script | Current user only | Installs to `~/.local/share/qrgen` with a symlink in `~/.local/bin/qrgen`. |

## Implementation Steps

1. Publish artifacts: `./build/Publish-QrGen.ps1 -Runtime linux-x64`
   (repeat with `linux-arm64`).
2. Generate the packages using `dotnet-deb`/`dotnet-rpm`, `fpm` or `nfpm` from
   `artifacts/<rid>`.
3. Include `postinst`/`postrm` scripts that create and remove the
   `/usr/local/bin/qrgen` symlink.
4. For per-user installation, add `~/.local/bin` to the PATH by writing to
   `~/.profile` only if it is not already present.
5. Optional: publish as an AppImage or Flatpak package for a consistent
   graphical installation experience across distributions.

## Note

The executable is named `qrgen` (without an extension) and must retain its
executable bit; `Publish-QrGen.ps1` already applies `chmod +x` when publishing on
Unix.
