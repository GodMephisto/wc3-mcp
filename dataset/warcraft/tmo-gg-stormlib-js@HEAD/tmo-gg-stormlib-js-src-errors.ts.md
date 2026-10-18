---
type: vendor-doc
source_repo: "https://github.com/tmo-gg/stormlib-js"
source_path: "src/errors.ts"
ref: HEAD
---

export class MpqError extends Error {
  constructor(message: string) {
    super(message);
    this.name = 'MpqError';
  }
}

export class MpqNotFoundError extends MpqError {
  constructor(message = 'File not found in archive') {
    super(message);
    this.name = 'MpqNotFoundError';
  }
}

export class MpqCorruptError extends MpqError {
  constructor(message = 'Archive data is corrupt') {
    super(message);
    this.name = 'MpqCorruptError';
  }
}

export class MpqUnsupportedError extends MpqError {
  constructor(message = 'Unsupported feature') {
    super(message);
    this.name = 'MpqUnsupportedError';
  }
}

export class MpqEncryptionError extends MpqError {
  constructor(message = 'Unable to decrypt data') {
    super(message);
    this.name = 'MpqEncryptionError';
  }
}

export class MpqCompressionError extends MpqError {
  constructor(message = 'Decompression failed') {
    super(message);
    this.name = 'MpqCompressionError';
  }
}
