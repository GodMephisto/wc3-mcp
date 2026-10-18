---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/metadata.rs"
ref: HEAD
---

use std::path::{Path, PathBuf};

use image::io::Reader as ImageReader;
use image_blp::parser::load_blp;
use image_blp::types::image::BlpImage;
use walkdir::WalkDir;

use crate::types::{AssetKind, FileMetadata};

pub fn create_asset_id(path: &Path) -> String {
    path.to_string_lossy().replace('\\', "/").to_lowercase()
}

pub fn file_kind_from_path(path: &Path) -> Option<AssetKind> {
    match path
        .extension()
        .and_then(|e| e.to_str())
        .map(|ext| ext.to_lowercase())
        .as_deref()
    {
        Some("png") => Some(AssetKind::Png),
        Some("blp") => Some(AssetKind::Blp),
        _ => None,
    }
}

pub fn extract_metadata(path: &Path) -> FileMetadata {
    let file_name = path
        .file_name()
        .and_then(|n| n.to_str())
        .unwrap_or("unknown")
        .to_string();

    let extension = path
        .extension()
        .and_then(|e| e.to_str())
        .unwrap_or("")
        .to_lowercase();

    let path_string = path.to_string_lossy().to_string();

    let kind = match extension.as_str() {
        "png" => AssetKind::Png,
        "blp" => AssetKind::Blp,
        _ => {
            return FileMetadata {
                id: create_asset_id(path),
                name: file_name,
                path: path_string,
                width: 0,
                height: 0,
                format: "Unknown".into(),
                mipmap_count: 0,
                kind: AssetKind::Blp,
                error: Some("Unsupported file extension".into()),
            };
        }
    };

    let id = create_asset_id(path);

    match kind {
        AssetKind::Blp => match load_blp(path) {
            Ok(blp) => metadata_from_blp(&blp, id, file_name, path_string),
            Err(err) => FileMetadata {
                id,
                name: file_name,
                path: path_string,
                width: 0,
                height: 0,
                format: "Unknown".into(),
                mipmap_count: 0,
                kind,
                error: Some(format!("Failed to parse BLP: {err}")),
            },
        },
        AssetKind::Png => match read_png_dimensions(path) {
            Ok((width, height)) => FileMetadata {
                id,
                name: file_name,
                path: path_string,
                width,
                height,
                format: "PNG".into(),
                mipmap_count: 0,
                kind,
                error: None,
            },
            Err(err) => FileMetadata {
                id,
                name: file_name,
                path: path_string,
                width: 0,
                height: 0,
                format: "PNG".into(),
                mipmap_count: 0,
                kind,
                error: Some(format!("Failed to read PNG: {err}")),
            },
        },
    }
}

fn metadata_from_blp(
    blp: &BlpImage,
    id: String,
    name: String,
    path: String,
) -> FileMetadata {
    let header = &blp.header;
    let format = describe_blp_format(blp);
    let mipmap_count = if header.has_mipmaps() {
        header.mipmaps_count() as u32
    } else {
        1
    };

    FileMetadata {
        id,
        name,
        path,
        width: header.width,
        height: header.height,
        format,
        mipmap_count,
        kind: AssetKind::Blp,
        error: None,
    }
}

pub fn describe_blp_format(blp: &BlpImage) -> String {
    let version = format!("{:?}", blp.header.version);
    let content = format!("{:?}", blp.header.content);
    format!("{version} / {content}")
}

fn read_png_dimensions(path: &Path) -> Result<(u32, u32), String> {
    let reader = ImageReader::open(path).map_err(|e| e.to_string())?;
    let dimensions = reader
        .into_dimensions()
        .map_err(|e| e.to_string())?;
    Ok(dimensions)
}

pub fn collect_supported_files(paths: &[String]) -> (Vec<PathBuf>, Vec<String>) {
    let mut files = Vec::new();
    let mut errors = Vec::new();

    for input in paths {
        let path = PathBuf::from(input);
        if !path.exists() {
            errors.push(format!("Path not found: {}", path.display()));
            continue;
        }

        if path.is_file() {
            if is_supported_extension(&path) {
                files.push(path);
            }
            continue;
        }

        if path.is_dir() {
            for entry in WalkDir::new(&path)
                .follow_links(false)
                .into_iter()
                .filter_map(|e| e.ok())
            {
                let entry_path = entry.path();
                if entry_path.is_file() && is_supported_extension(entry_path) {
                    files.push(entry_path.to_path_buf());
                }
            }
        }
    }

    files.sort_by(|a, b| a.to_string_lossy().cmp(&b.to_string_lossy()));
    files.dedup();

    (files, errors)
}

fn is_supported_extension(path: &Path) -> bool {
    path.extension()
        .and_then(|e| e.to_str())
        .map(|ext| {
            let lower = ext.to_lowercase();
            lower == "blp" || lower == "png"
        })
        .unwrap_or(false)
}
