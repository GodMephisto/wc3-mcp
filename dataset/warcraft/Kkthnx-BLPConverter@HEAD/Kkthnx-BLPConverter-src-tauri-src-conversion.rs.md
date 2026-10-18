---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "src-tauri/src/conversion.rs"
ref: HEAD
---

use std::collections::HashSet;
use std::fs;
use std::path::{Path, PathBuf};

use image::io::Reader as ImageReader;
use image::DynamicImage;
use image_blp::convert::{
    blp_to_image, image_to_blp, Blp2Format, BlpTarget, DxtAlgorithm, FilterType,
};
use image_blp::encode::save_blp;
use image_blp::parser::load_blp;
use rayon::prelude::*;

use crate::metadata::{collect_supported_files, create_asset_id, describe_blp_format, file_kind_from_path};
use crate::types::{AssetKind, BatchConvertResult, CompressionFormat, ConversionSettings, QueueItem, QueueStatus};

struct ConvertOutput {
    output_path: PathBuf,
    width: u32,
    height: u32,
    format: String,
    mipmap_count: u32,
}

fn convert_blp_to_png(
    source: &Path,
    output_dir: &Path,
    settings: &ConversionSettings,
) -> Result<ConvertOutput, String> {
    let blp = load_blp(source).map_err(|e| e.to_string())?;
    let header = &blp.header;
    let format = describe_blp_format(&blp);
    let mipmap_count = if header.has_mipmaps() {
        header.mipmaps_count() as u32
    } else {
        1
    };

    let image = blp_to_image(&blp, 0).map_err(|e| e.to_string())?;

    let output_path = build_output_path(source, output_dir, settings, "png")?;
    image
        .save_with_format(&output_path, image::ImageFormat::Png)
        .map_err(|e| e.to_string())?;

    Ok(ConvertOutput {
        output_path,
        width: header.width,
        height: header.height,
        format,
        mipmap_count,
    })
}

fn convert_png_to_blp(
    source: &Path,
    output_dir: &Path,
    settings: &ConversionSettings,
) -> Result<ConvertOutput, String> {
    let reader = ImageReader::open(source).map_err(|e| e.to_string())?;
    let image = reader.decode().map_err(|e| e.to_string())?;

    let width = image.width();
    let height = image.height();
    validate_texture_dimensions(width, height)?;

    let target = compression_to_blp_target(settings.compression, &image);
    let blp = image_to_blp(
        image,
        settings.generate_mipmaps,
        target,
        FilterType::Lanczos3,
    )
    .map_err(|e| e.to_string())?;

    let mipmap_count = if settings.generate_mipmaps {
        mip_level_count(width, height)
    } else {
        1
    };

    let output_path = build_output_path(source, output_dir, settings, "blp")?;
    save_blp(&blp, &output_path).map_err(|e| e.to_string())?;

    Ok(ConvertOutput {
        output_path,
        width,
        height,
        format: target_format_for(AssetKind::Png, settings.compression),
        mipmap_count,
    })
}

pub fn compression_to_blp_target(compression: CompressionFormat, image: &DynamicImage) -> BlpTarget {
    let dxt_algo = DxtAlgorithm::ClusterFit;
    let has_alpha = image_has_meaningful_alpha(image);

    match compression {
        CompressionFormat::Raw => BlpTarget::Blp2(Blp2Format::Raw3),
        CompressionFormat::Dxt1 => BlpTarget::Blp2(Blp2Format::Dxt1 {
            has_alpha,
            compress_algorithm: dxt_algo,
        }),
        CompressionFormat::Dxt3 => BlpTarget::Blp2(Blp2Format::Dxt3 {
            has_alpha,
            compress_algorithm: dxt_algo,
        }),
        CompressionFormat::Dxt5 => BlpTarget::Blp2(Blp2Format::Dxt5 {
            has_alpha,
            compress_algorithm: dxt_algo,
        }),
    }
}

fn image_has_meaningful_alpha(image: &DynamicImage) -> bool {
    if let Some(rgba) = image.as_rgba8() {
        return rgba.pixels().any(|pixel| pixel[3] < 255);
    }

    image.color().has_alpha()
}

fn validate_texture_dimensions(width: u32, height: u32) -> Result<(), String> {
    if width == 0 || height == 0 {
        return Err("Texture dimensions cannot be zero".into());
    }

    if !width.is_power_of_two() || !height.is_power_of_two() {
        return Err(format!(
            "Texture dimensions must be powers of two (got {width}x{height}). Resize the image before converting."
        ));
    }

    Ok(())
}

fn mip_level_count(width: u32, height: u32) -> u32 {
    let max_dim = width.max(height);
    (max_dim as f64).log2().floor() as u32 + 1
}

pub fn convert_paths_batch(
    paths: &[String],
    kind: AssetKind,
    settings: &ConversionSettings,
) -> BatchConvertResult {
    let (files, scan_errors) = collect_supported_files(paths);
    let sources: Vec<PathBuf> = files
        .into_iter()
        .filter(|path| file_kind_from_path(path) == Some(kind))
        .collect();

    if sources.is_empty() {
        return BatchConvertResult {
            succeeded: 0,
            failed: 0,
            scan_errors,
            results: Vec::new(),
        };
    }

    if let Err(err) = ensure_output_dirs(&sources, settings) {
        return BatchConvertResult {
            succeeded: 0,
            failed: 0,
            scan_errors: {
                let mut errors = scan_errors;
                errors.push(err);
                errors
            },
            results: Vec::new(),
        };
    }

    let results: Vec<QueueItem> = sources
        .par_iter()
        .map(|source| convert_path(source, kind, settings))
        .collect();

    let succeeded = results
        .iter()
        .filter(|r| r.queue_status == QueueStatus::Completed)
        .count() as u32;
    let failed = results
        .iter()
        .filter(|r| r.queue_status == QueueStatus::Failed)
        .count() as u32;

    BatchConvertResult {
        succeeded,
        failed,
        scan_errors,
        results,
    }
}

fn ensure_output_dirs(sources: &[PathBuf], settings: &ConversionSettings) -> Result<(), String> {
    let dirs: HashSet<PathBuf> = sources
        .iter()
        .filter_map(|source| resolve_output_dir(source, settings).ok())
        .collect();

    for dir in dirs {
        fs::create_dir_all(&dir).map_err(|e| {
            format!(
                "Cannot create output folder {}: {e}",
                dir.display()
            )
        })?;
    }

    Ok(())
}

fn convert_path(source: &Path, kind: AssetKind, settings: &ConversionSettings) -> QueueItem {
    let name = source
        .file_name()
        .and_then(|n| n.to_str())
        .unwrap_or("unknown")
        .to_string();
    let path_string = source.to_string_lossy().to_string();
    let target_format = target_format_for(kind, settings.compression);

    let mut item = QueueItem {
        id: create_asset_id(source),
        name: name.clone(),
        path: path_string,
        width: 0,
        height: 0,
        format: String::new(),
        mipmap_count: 0,
        kind,
        error: None,
        queue_status: QueueStatus::Processing,
        progress: 0,
        target_format,
        output_path: None,
        error_message: None,
    };

    let output_dir = match resolve_output_dir(source, settings) {
        Ok(dir) => dir,
        Err(err) => {
            item.queue_status = QueueStatus::Failed;
            item.error_message = Some(err);
            return item;
        }
    };

    let result = match kind {
        AssetKind::Blp => convert_blp_to_png(source, &output_dir, settings),
        AssetKind::Png => convert_png_to_blp(source, &output_dir, settings),
    };

    match result {
        Ok(output) => {
            item.queue_status = QueueStatus::Completed;
            item.progress = 100;
            item.width = output.width;
            item.height = output.height;
            item.format = output.format;
            item.mipmap_count = output.mipmap_count;
            item.output_path = Some(output.output_path.to_string_lossy().to_string());
        }
        Err(err) => {
            item.queue_status = QueueStatus::Failed;
            item.progress = 100;
            item.error_message = Some(err);
        }
    }

    item
}

fn target_format_for(kind: AssetKind, compression: CompressionFormat) -> String {
    match kind {
        AssetKind::Blp => "PNG".into(),
        AssetKind::Png => match compression {
            CompressionFormat::Raw => "BLP (RAW)".into(),
            CompressionFormat::Dxt1 => "BLP (DXT1)".into(),
            CompressionFormat::Dxt3 => "BLP (DXT3)".into(),
            CompressionFormat::Dxt5 => "BLP (DXT5)".into(),
        },
    }
}

fn resolve_output_dir(source: &Path, settings: &ConversionSettings) -> Result<PathBuf, String> {
    if settings.output_directory.is_empty() {
        source
            .parent()
            .map(Path::to_path_buf)
            .ok_or_else(|| "Cannot determine source folder".to_string())
    } else {
        Ok(PathBuf::from(&settings.output_directory))
    }
}

fn build_output_path(
    source: &Path,
    output_dir: &Path,
    settings: &ConversionSettings,
    extension: &str,
) -> Result<PathBuf, String> {
    let filename = output_filename(source, settings, extension)?;
    Ok(output_dir.join(filename))
}

fn output_filename(
    source: &Path,
    settings: &ConversionSettings,
    extension: &str,
) -> Result<String, String> {
    let stem = source
        .file_stem()
        .and_then(|s| s.to_str())
        .ok_or_else(|| "Invalid source filename".to_string())?;

    if settings.output_directory.is_empty() {
        return Ok(format!("{stem}.{extension}"));
    }

    let parent_label = source
        .parent()
        .and_then(|p| p.file_name())
        .and_then(|n| n.to_str())
        .map(sanitize_filename_component)
        .filter(|label| !label.is_empty())
        .unwrap_or_else(|| "file".to_string());

    Ok(format!("{parent_label}_{stem}.{extension}"))
}

fn sanitize_filename_component(name: &str) -> String {
    name.chars()
        .map(|c| {
            if c.is_ascii_alphanumeric() || c == '-' || c == '_' {
                c
            } else {
                '_'
            }
        })
        .collect()
}

#[cfg(test)]
mod tests {
    use super::*;

    #[test]
    fn compression_targets_are_constructible() {
        let image = DynamicImage::new_rgba8(64, 64);
        let _ = compression_to_blp_target(CompressionFormat::Raw, &image);
        let _ = compression_to_blp_target(CompressionFormat::Dxt1, &image);
        let _ = compression_to_blp_target(CompressionFormat::Dxt3, &image);
        let _ = compression_to_blp_target(CompressionFormat::Dxt5, &image);
    }

    #[test]
    fn empty_output_dir_uses_source_parent() {
        let source = PathBuf::from("textures").join("icon.blp");
        let settings = ConversionSettings {
            compression: CompressionFormat::Dxt5,
            generate_mipmaps: true,
            output_directory: String::new(),
        };

        let dir = resolve_output_dir(&source, &settings).unwrap();
        assert_eq!(dir, PathBuf::from("textures"));
    }

    #[test]
    fn rejects_non_power_of_two_dimensions() {
        assert!(validate_texture_dimensions(100, 64).is_err());
        assert!(validate_texture_dimensions(64, 64).is_ok());
    }

    #[test]
    fn detects_meaningful_alpha() {
        let mut opaque = DynamicImage::new_rgba8(4, 4);
        if let Some(rgba) = opaque.as_mut_rgba8() {
            for pixel in rgba.pixels_mut() {
                pixel[3] = 255;
            }
        }
        assert!(!image_has_meaningful_alpha(&opaque));

        let mut transparent = DynamicImage::new_rgba8(4, 4);
        if let Some(rgba) = transparent.as_mut_rgba8() {
            for pixel in rgba.pixels_mut() {
                pixel[3] = 255;
            }
            rgba.pixels_mut().next().unwrap()[3] = 128;
        }
        assert!(image_has_meaningful_alpha(&transparent));
    }

    #[test]
    fn mip_level_count_matches_wow_chain() {
        assert_eq!(mip_level_count(256, 256), 9);
        assert_eq!(mip_level_count(512, 128), 10);
    }

    #[test]
    fn custom_output_dir_prefixes_parent_folder() {
        let source = PathBuf::from("textures").join("icons").join("spell.blp");
        let settings = ConversionSettings {
            compression: CompressionFormat::Dxt5,
            generate_mipmaps: true,
            output_directory: "output".into(),
        };

        let name = output_filename(&source, &settings, "png").unwrap();
        assert_eq!(name, "icons_spell.png");
    }

    fn temp_workdir(tag: &str) -> PathBuf {
        let unique = format!(
            "blpconv_{tag}_{}_{:?}",
            std::process::id(),
            std::time::SystemTime::now()
                .duration_since(std::time::UNIX_EPOCH)
                .unwrap()
                .as_nanos()
        );
        let dir = std::env::temp_dir().join(unique);
        fs::create_dir_all(&dir).unwrap();
        dir
    }

    #[test]
    fn png_to_blp_and_back_round_trips() {
        let dir = temp_workdir("roundtrip");

        // A 8x8 RGBA image with a partially transparent pixel.
        let mut source = DynamicImage::new_rgba8(8, 8);
        if let Some(rgba) = source.as_mut_rgba8() {
            for pixel in rgba.pixels_mut() {
                *pixel = image::Rgba([200, 100, 50, 255]);
            }
            rgba.get_pixel_mut(0, 0)[3] = 64;
        }
        let png_path = dir.join("sample.png");
        source
            .save_with_format(&png_path, image::ImageFormat::Png)
            .unwrap();

        let settings = ConversionSettings {
            compression: CompressionFormat::Dxt5,
            generate_mipmaps: true,
            output_directory: String::new(),
        };

        let encoded = convert_png_to_blp(&png_path, &dir, &settings).unwrap();
        assert!(encoded.output_path.exists());
        assert_eq!(encoded.width, 8);
        assert_eq!(encoded.height, 8);
        assert_eq!(encoded.mipmap_count, mip_level_count(8, 8));

        let decoded = convert_blp_to_png(&encoded.output_path, &dir, &settings).unwrap();
        assert!(decoded.output_path.exists());
        assert_eq!(decoded.width, 8);
        assert_eq!(decoded.height, 8);

        fs::remove_dir_all(&dir).ok();
    }

    #[test]
    fn same_folder_output_uses_stem_only() {
        let source = PathBuf::from("textures").join("icons").join("spell.blp");
        let settings = ConversionSettings {
            compression: CompressionFormat::Dxt5,
            generate_mipmaps: true,
            output_directory: String::new(),
        };

        let name = output_filename(&source, &settings, "png").unwrap();
        assert_eq!(name, "spell.png");
    }
}
