---
type: vendor-doc
source_repo: "https://github.com/grolmash/HiveWE-mac"
source_path: "tools/ccache-cxx-modules.sh"
ref: HEAD
---

#!/usr/bin/env bash
set -euo pipefail

if [[ $# -lt 1 ]]; then
	exec ccache "$@"
fi

compiler="$1"
shift

ccache_bin="${CCACHE_BIN:-}"
if [[ -z "$ccache_bin" ]]; then
	if command -v ccache >/dev/null 2>&1; then
		ccache_bin="$(command -v ccache)"
	elif [[ -n "${HOMEBREW_PREFIX:-}" && -x "${HOMEBREW_PREFIX}/bin/ccache" ]]; then
		ccache_bin="${HOMEBREW_PREFIX}/bin/ccache"
	else
		exec "$compiler" "$@"
	fi
fi

extra_files=()
for arg in "$@"; do
	if [[ "$arg" != @*.modmap ]]; then
		continue
	fi

	modmap="${arg#@}"
	if [[ ! -f "$modmap" ]]; then
		continue
	fi

	while IFS= read -r line || [[ -n "$line" ]]; do
		line="${line//$'\r'/}"
		if [[ "$line" != -fmodule-file=* ]]; then
			continue
		fi

		value="${line#-fmodule-file=}"
		value="${value#\"}"
		value="${value%\"}"
		pcm="${value#*=}"
		pcm="${pcm#\"}"
		pcm="${pcm%\"}"

		if [[ "$pcm" != /* ]]; then
			pcm="${PWD}/${pcm}"
		fi

		if [[ -f "$pcm" ]]; then
			extra_files+=("$pcm")
		fi
	done < "$modmap"
done

if [[ ${#extra_files[@]} -gt 0 ]]; then
	joined="$(IFS=:; echo "${extra_files[*]}")"
	if [[ -n "${CCACHE_EXTRAFILES:-}" ]]; then
		export CCACHE_EXTRAFILES="${CCACHE_EXTRAFILES}:${joined}"
	else
		export CCACHE_EXTRAFILES="$joined"
	fi
fi

exec "$ccache_bin" "$compiler" "$@"
