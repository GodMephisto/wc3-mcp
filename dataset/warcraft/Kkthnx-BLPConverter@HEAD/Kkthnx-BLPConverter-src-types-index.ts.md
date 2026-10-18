---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src/types/index.ts"
ref: HEAD
---

export type AssetKind = "blp" | "png";

export type CompressionFormat = "raw" | "dxt1" | "dxt3" | "dxt5";

export type ConvertDirection = "to-png" | "to-blp";

export interface FileMetadata {
  id: string;
  name: string;
  path: string;
  width: number;
  height: number;
  format: string;
  mipmapCount: number;
  kind: AssetKind;
  error?: string;
}

export type QueueStatus =
  | "pending"
  | "processing"
  | "completed"
  | "failed"
  | "skipped";

export interface QueueItem extends FileMetadata {
  queueStatus: QueueStatus;
  progress: number;
  targetFormat: string;
  outputPath?: string;
  errorMessage?: string;
}

export interface ConversionSettings {
  compression: CompressionFormat;
  generateMipmaps: boolean;
  outputDirectory: string;
}

export interface ScanPathsResult {
  assets: FileMetadata[];
  errors: string[];
}

export interface BatchConvertResult {
  succeeded: number;
  failed: number;
  scanErrors: string[];
  results: QueueItem[];
}

export interface BlpViewStatus {
  installed: boolean;
  dllPath: string;
  supported: boolean;
  message: string;
}

export interface BlpViewActionResult {
  success: boolean;
  message: string;
  restartRequired: boolean;
}

export type LogLevel = "info" | "success" | "error";

export interface LogEntry {
  id: string;
  time: string;
  message: string;
  level: LogLevel;
}
