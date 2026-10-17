---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "RELEASING.md"
ref: HEAD
---

# Releasing BLP Converter

Releases are built automatically by GitHub Actions when you push a **version tag**.

## Create a release

1. Bump the version in all of these (keep them in sync):
   - `package.json`
   - `src-tauri/tauri.conf.json`
   - `src-tauri/Cargo.toml`
   - `src/constants/app.ts`

2. Commit and push to `main`.

3. Create and push a tag (must start with `v`):

```bash
git tag v1.1.0
git push origin v1.1.0
```

4. Open [GitHub Actions](https://github.com/Kkthnx/BLPConverter/actions) and watch the **Release** workflow.

5. When it finishes, open [Releases](https://github.com/Kkthnx/BLPConverter/releases), all platform installers are attached to that release.

## Download artifacts per platform

| Platform | Files |
|----------|-------|
| **Windows** | `*-setup.exe` (NSIS), `*.msi` |
| **Linux** | `*.deb`, `*.AppImage` |
| **macOS** | `*.dmg`, `*.app.tar.gz` (Intel + Apple Silicon) |

BLPView (Explorer thumbnails) is included in the **Windows** build only.

## Repo settings

In **Settings > Actions > General > Workflow permissions**, enable **Read and write permissions** so the workflow can publish releases.
