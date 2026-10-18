---
type: vendor-doc
source_repo: "https://github.com/thegraydot/mpqcli"
source_path: ".gpipe.yml"
ref: HEAD
---

binary: mpqcli

platforms:
  linux_amd64:
    path: ./dist/mpqcli-linux-x86_64
    name: mpqcli-linux-x86_64
  linux_arm64:
    path: ./dist/mpqcli-linux-aarch64
    name: mpqcli-linux-aarch64
  windows_amd64:
    path: ./dist/mpqcli-windows-x86_64.exe
    name: mpqcli-windows-x86_64.exe
