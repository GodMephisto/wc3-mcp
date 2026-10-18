---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "src/parser.ts"
ref: HEAD
---

import Model from './parsers/mdlx/model';
import { isMdl, isMdx } from './parsers/mdlx/isformat';
import type { War3ModelFormat } from './types';

const textDecoder = new TextDecoder('utf-8', { fatal: false });

export interface ParsedWar3Model {
  format: War3ModelFormat;
  model: Model;
}

export function detectWar3ModelFormat(
  path: string,
  source: ArrayBuffer | Uint8Array | string,
  mime?: string,
): War3ModelFormat {
  if (isMdx(source)) return 'mdx';
  if (isMdl(source)) return 'mdl';

  const bytes = typeof source === 'string' ? undefined : toUint8Array(source);
  if (bytes && looksLikeTextMdl(bytes)) return 'mdl';

  const normalizedMime = mime?.split(';', 1)[0]?.trim().toLowerCase();
  if (normalizedMime === 'text/plain' || normalizedMime === 'text/x-warcraft3-mdl') {
    return 'mdl';
  }
  if (normalizedMime === 'application/x-warcraft3-mdx') return 'mdx';

  const extension = path.match(/\.([^.\\/]+)$/)?.[1]?.toLowerCase();
  if (extension === 'mdl') return 'mdl';
  if (extension === 'mdx') return 'mdx';

  throw new Error(`Unable to detect Warcraft III model format for "${path}".`);
}

export function parseWar3Model(
  path: string,
  source: ArrayBuffer | Uint8Array | string,
  mime?: string,
): ParsedWar3Model {
  const format = detectWar3ModelFormat(path, source, mime);
  const model = new Model();

  if (format === 'mdl' && typeof source !== 'string') {
    model.load(textDecoder.decode(toUint8Array(source)));
  } else if (format === 'mdx' && typeof source === 'string') {
    model.load(new TextEncoder().encode(source));
  } else {
    model.load(source);
  }

  return { format, model };
}

function looksLikeTextMdl(bytes: Uint8Array): boolean {
  const sample = textDecoder.decode(bytes.subarray(0, Math.min(bytes.length, 4096)));
  const normalized = sample.replace(/^\uFEFF/, '').trimStart();
  if (!normalized || normalized.includes('\0')) return false;
  return (
    /\bFormatVersion\s+\d+/i.test(normalized) ||
    (/^(?:\/\/[^\n]*\n|\s)*Version\s*\{/i.test(normalized) && /\bModel\b/i.test(normalized))
  );
}

function toUint8Array(source: ArrayBuffer | Uint8Array): Uint8Array {
  return source instanceof Uint8Array ? source : new Uint8Array(source);
}
