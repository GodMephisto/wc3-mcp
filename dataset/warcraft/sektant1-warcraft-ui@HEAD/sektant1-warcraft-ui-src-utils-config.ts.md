---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/utils/config.ts"
ref: HEAD
---

let _assetsBaseUrl: string | null = null;

export function setAssetsBaseUrl(url: string): void {
  _assetsBaseUrl = url.endsWith('/') ? url.slice(0, -1) : url;
}

export function resolveAssetPath(path: string): string {
  const base = _assetsBaseUrl ?? import.meta.env.BASE_URL;
  return base + path;
}
