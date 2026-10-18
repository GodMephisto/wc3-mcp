---
type: vendor-doc
source_repo: "https://github.com/zengfanfan/war3mdlx"
source_path: "build.sh"
ref: HEAD
---

#!/bin/bash

log() {
    printf '\033[0;32m%s\033[0m\n' "$*"
}

# check

if [ ! -e Cargo.toml ]; then
    echo "This script must be run from the root directory of the project."
    exit 1
fi

# settings

NAME=$(cargo metadata --no-deps --format-version=1 | jq -r '.packages[0].name')
VERSION=$(cargo metadata --no-deps --format-version=1 | jq -r '.packages[0].version')
BIN_NAME=$(cargo metadata --no-deps --format-version=1 | jq -r '.packages[].targets[] | select(.kind[] == "bin") | .name')

WINDOWS_TARGET=x86_64-pc-windows-gnu
LINUX_TARGET=x86_64-unknown-linux-musl

WINDOWS_ZIP=$NAME-$VERSION-win64.zip
LINUX_ZIP=$NAME-$VERSION-linux64.zip

# commands

log "Building for windows ($WINDOWS_TARGET) ..."
cargo build --release --target $WINDOWS_TARGET

log "Building for linux ($LINUX_TARGET) ..."
cargo build --release --target $LINUX_TARGET

log "Making $WINDOWS_ZIP ..."
zip -j $WINDOWS_ZIP LICENSE target/$WINDOWS_TARGET/release/$BIN_NAME.exe

log "Making $LINUX_ZIP ..."
zip -j $LINUX_ZIP LICENSE target/$LINUX_TARGET/release/$BIN_NAME

log "DONE! Generated files: $(ls *.zip | tr '\n' ' ')"
