---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/content/docs/installation.mdx"
ref: HEAD
---

# Installation

---
title: Installation
description: Download a release on Windows, or build wc3-forge from source (Windows or macOS).
---

## Download (Windows)

The fastest way to get started is the installer from the latest release. It
bundles everything wc3-forge needs (including the vendored CASC libraries).

> **macOS:** there is no prebuilt macOS binary — build from source
> ([below](#build-from-source)). The in-app updater is Windows-only too.

<Cards>
  <Card
    href="https://github.com/StephenSHorton/wc3-forge/releases/latest/download/wc3-forge-amd64-installer.exe"
    title="Download the installer"
    description="wc3-forge-amd64-installer.exe — always the newest release."
  />
  <Card
    href="https://github.com/StephenSHorton/wc3-forge/releases/latest"
    title="All releases"
    description="Portable .zip bundles and full changelogs."
  />
</Cards>

The portable build is published as `wc3-forge-v<version>-windows-amd64.zip` if you
prefer not to run an installer.

## Build from source

You'll need [Go](https://go.dev), [Node.js](https://nodejs.org), and the
[Wails CLI](https://wails.io/docs/gettingstarted/installation).

### Windows

```powershell
git clone https://github.com/StephenSHorton/wc3-forge
cd wc3-forge
wails build
```

The binary lands at `build/bin/wc3-forge.exe`.

### macOS (build from source — no prebuilt binary)

macOS has no published release, so building from source is the only way to run
it there. Reforged Warcraft III on macOS typically runs through
[Mead](https://github.com/StephenSHorton/mead) (a Wine-bottle manager); see
"Pointing at your Warcraft III install" below. macOS needs one extra step to
build CascLib:

```bash
git clone https://github.com/StephenSHorton/wc3-forge
cd wc3-forge
./scripts/build-casclib-macos.sh   # one-time: compiles libcasc.dylib
wails build
```

The bundle lands at `build/bin/wc3-forge.app`. CascLib is `dlopen`'d at runtime,
so the `.dylib` must sit alongside the binary inside the `.app` bundle — the
`darwin/*` postBuildHook in `wails.json` copies it in for you. The macOS build is
arm64 by default; cross-build with `wails build -platform darwin/amd64` after
running the casclib script on a matching architecture.

## Pointing at your Warcraft III install

wc3-forge resolves assets from the loaded map first, then from the CASC mount on
your WC3 install. It looks in the OS-conventional location by default
(`C:\Program Files (x86)\Warcraft III` on Windows, `/Applications/Warcraft III`
on macOS). Override it with the `WC3FORGE_WC3_PATH` environment variable if your
install lives elsewhere.

Next: [set up Claude Code](/docs/claude-code).
