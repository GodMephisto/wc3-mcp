---
type: vendor-doc
source_repo: "https://github.com/twofold1999-hue/wc3-map-object-editor"
source_path: "src/services/string-resolver.service.ts"
ref: HEAD
---

export class StringResolverService {
  public constructor(private readonly stringTable?: Record<string, string>) {}

  public resolve(value: string): string {
    const match = /^TRIGSTR_(\d+)$/i.exec(value);
    if (!match) {
      return value;
    }

    const numericKey = Number.parseInt(match[1], 10);
    if (Number.isNaN(numericKey)) {
      return value;
    }

    const resolved = this.stringTable?.[String(numericKey)];
    return resolved ?? value;
  }
}
