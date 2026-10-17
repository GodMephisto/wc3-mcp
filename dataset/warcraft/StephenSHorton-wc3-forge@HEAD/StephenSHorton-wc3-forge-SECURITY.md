---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "SECURITY.md"
ref: HEAD
---

# Security Policy

## Supported versions

wc3-forge is pre-1.0 and ships from a single moving release line. Security fixes
land in the **latest release** only; please update before reporting an issue you
hit on an older build.

## Reporting a vulnerability

Please report security issues **privately** — do not open a public issue for an
unfixed vulnerability.

- Preferred: GitHub's private vulnerability reporting — the repository's
  **Security** tab → **Report a vulnerability**.
- Or email **4stephenhorton@gmail.com** with `wc3-forge security` in the subject.

Include what you found, how to reproduce it, and the affected version
(Help → About, or the binary's `--version`). I aim to acknowledge within a few
days and to coordinate a fix + disclosure timeline with you.

## What the editor touches

- **Local files.** wc3-forge reads and writes Warcraft III map files (`.w3x` and
  extracted folders) and reads your Warcraft III install via CASC. It does not
  upload your maps anywhere.
- **MCP bridge.** The optional MCP bridge listens on `127.0.0.1` (loopback only)
  on an ephemeral port, and every request must carry a per-process token written
  to `~/.wc3-forge/mcp/<pid>.lock`. It is not exposed to the network.
- **Update check.** The in-app updater queries the GitHub Releases API for this
  repository and, when you choose to update, downloads the installer from a
  GitHub release.

## Binary integrity

The published Windows binaries are **not code-signed** yet (so Windows
SmartScreen may warn on first run — that is expected for an unsigned app).

To compensate, the in-app updater verifies integrity before it elevates and runs
anything:

1. The installer is only ever downloaded from a `github.com` /
   `*.githubusercontent.com` URL over HTTPS — a tampered update-check response
   can't redirect the download to an arbitrary host.
2. Each release publishes a **`SHA256SUMS`** asset. After downloading, the
   updater computes the installer's SHA-256 and **refuses to launch it on a
   mismatch** (the temp file is deleted).

You can verify a download yourself: grab `SHA256SUMS` from the
[release page](https://github.com/StephenSHorton/wc3-forge/releases) and compare,
e.g. on Windows:

```powershell
(Get-FileHash -Algorithm SHA256 .\wc3-forge-amd64-installer.exe).Hash
```

Releases from **v0.9.0** onward include `SHA256SUMS`; earlier releases predate it
(the updater logs and proceeds when the file is absent, since the GitHub-over-TLS
download is still origin-checked). Code signing is tracked as future work.

## Dependencies

The Go module graph is pinned in `go.mod`/`go.sum`; the frontend is compiled and
embedded into the binary at build time (no `node_modules` ships in the release).
CascLib is loaded at runtime via `purego` (no cgo) — see `CREDITS.md`.
