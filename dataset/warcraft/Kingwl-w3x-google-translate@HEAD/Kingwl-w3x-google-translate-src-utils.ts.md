---
type: vendor-doc
source_repo: "https://github.com/Kingwl/w3x-google-translate"
source_path: "src/utils.ts"
ref: HEAD
---

export function zip<T, U>(a: T[], b: U[]): [T, U][] {
  if (a.length !== b.length) {
    throw new Error("Length not equals");
  }

  const results: [T, U][] = [];

  for (let i = 0; i < a.length; ++i) {
    results.push([a[i], b[i]]);
  }

  return results;
}

export function log(...args: any[]) {
  const now = new Date();
  const nowText = `${now.toLocaleDateString()} ${now.toLocaleTimeString()}`;
  console.log(nowText, ...args);
}

export function toNumberOrBeUndefined(str: string | undefined) {
  if (!str) {
    return undefined;
  }
  return Number(str);
}
