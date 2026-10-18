---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/types.rs"
ref: HEAD
---

use serde::{Deserialize, Serialize};

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct FileMetadata {
    pub id: String,
    pub name: String,
    pub path: String,
    pub width: u32,
    pub height: u32,
    pub format: String,
    pub mipmap_count: u32,
    pub kind: AssetKind,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub error: Option<String>,
}

#[derive(Debug, Clone, Copy, Serialize, Deserialize, PartialEq, Eq)]
#[serde(rename_all = "lowercase")]
pub enum AssetKind {
    Blp,
    Png,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ScanPathsResult {
    pub assets: Vec<FileMetadata>,
    pub errors: Vec<String>,
}

#[derive(Debug, Clone, Copy, Serialize, Deserialize, PartialEq, Eq)]
#[serde(rename_all = "lowercase")]
pub enum QueueStatus {
    Pending,
    Processing,
    Completed,
    Failed,
    Skipped,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct QueueItem {
    pub id: String,
    pub name: String,
    pub path: String,
    pub width: u32,
    pub height: u32,
    pub format: String,
    pub mipmap_count: u32,
    pub kind: AssetKind,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub error: Option<String>,
    pub queue_status: QueueStatus,
    pub progress: u8,
    pub target_format: String,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub output_path: Option<String>,
    #[serde(skip_serializing_if = "Option::is_none")]
    pub error_message: Option<String>,
}

#[derive(Debug, Clone, Copy, Serialize, Deserialize, PartialEq, Eq)]
#[serde(rename_all = "lowercase")]
pub enum CompressionFormat {
    Raw,
    Dxt1,
    Dxt3,
    Dxt5,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct ConversionSettings {
    pub compression: CompressionFormat,
    pub generate_mipmaps: bool,
    pub output_directory: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct BatchConvertResult {
    pub succeeded: u32,
    pub failed: u32,
    pub scan_errors: Vec<String>,
    pub results: Vec<QueueItem>,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct BlpViewStatus {
    pub installed: bool,
    pub dll_path: String,
    pub supported: bool,
    pub message: String,
}

#[derive(Debug, Clone, Serialize, Deserialize)]
#[serde(rename_all = "camelCase")]
pub struct BlpViewActionResult {
    pub success: bool,
    pub message: String,
    pub restart_required: bool,
}
