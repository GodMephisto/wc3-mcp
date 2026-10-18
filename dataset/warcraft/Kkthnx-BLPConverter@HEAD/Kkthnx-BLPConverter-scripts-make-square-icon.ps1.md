---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "scripts/make-square-icon.ps1"
ref: HEAD
---

Add-Type -AssemblyName System.Drawing

$src = Join-Path $PSScriptRoot "..\app-icon-source.png"
$out = Join-Path $PSScriptRoot "..\app-icon-square.png"

$img = [System.Drawing.Image]::FromFile($src)
$size = [Math]::Max($img.Width, $img.Height)
$sq = New-Object System.Drawing.Bitmap $size, $size
$g = [System.Drawing.Graphics]::FromImage($sq)
$g.Clear([System.Drawing.Color]::FromArgb(255, 18, 20, 26))
$g.InterpolationMode = [System.Drawing.Drawing2D.InterpolationMode]::HighQualityBicubic
$offX = ($size - $img.Width) / 2
$offY = ($size - $img.Height) / 2
$g.DrawImage($img, $offX, $offY, $img.Width, $img.Height)
$sq.Save($out, [System.Drawing.Imaging.ImageFormat]::Png)
$g.Dispose()
$sq.Dispose()
$img.Dispose()
Write-Host "Saved $out (${size}x${size})"
