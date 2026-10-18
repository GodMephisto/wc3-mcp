---
type: vendor-doc
source_repo: "https://github.com/ClaudixoPG/DSI_PCG_War3MapGeneratorAutoencoder"
source_path: "Proyecto/Decompresser.ps1"
ref: HEAD
---

'''# Set the directory where your w3x files are stored
$W3X_DIR = "C:\Users\claud\Desktop\DSI\2023-S2\PCG\Proyecto\maps"

# Set the extraction destination directory
$DEST_DIR = "C:\Users\claud\Desktop\DSI\2023-S2\PCG\Proyecto\ExtractedMaps"

# Ensure MPQEditor is in your PATH or specify its full path here
$MPQEDITOR_PATH = "C:\Users\claud\Desktop\DSI\2023-S2\PCG\Proyecto\mpqeditor_en_v3.6.0.868\Win32\MPQEditor.exe"

# Get all w3x files in the directory
$w3xFiles = Get-ChildItem -Path $W3X_DIR -Filter *.w3x

# Loop through each w3x file
foreach ($file in $w3xFiles) {
    $destPath = Join-Path $DEST_DIR $file.BaseName
    # Extract all files from the w3x file to the destination directory
    & $MPQEDITOR_PATH /extract $file.FullName * $destPath /fp
}'''

'''# Set the base directory where your subfolders with w3x files are stored
$BASE_DIR = "D:\DSI_PCG\War3MapsDatabase\Maps"

# Set the extraction destination base directory
$DEST_BASE_DIR = "D:\DSI_PCG\ExtractedMaps"

# Ensure MPQEditor is in your PATH or specify its full path here
$MPQEDITOR_PATH = "C:\Users\claud\Desktop\DSI\2023-S2\PCG\Proyecto\mpqeditor_en_v3.6.0.868\Win32\MPQEditor.exe"

# Get all subdirectories in the base directory
$subDirs = Get-ChildItem -Path $BASE_DIR -Directory

# Loop through each subdirectory
foreach ($dir in $subDirs) {
    # Get all w3x files in the current subdirectory
    $w3xFiles = Get-ChildItem -Path $dir.FullName -Filter *.w3x
    
    # Loop through each w3x file in the current subdirectory
    foreach ($file in $w3xFiles) {
        $destPath = Join-Path $DEST_BASE_DIR $dir.Name
        # Ensure the destination directory exists
        if (-not (Test-Path $destPath)) {
            New-Item -ItemType Directory -Path $destPath | Out-Null
        }
        
        # Extract all files from the w3x file to the destination directory
        & $MPQEDITOR_PATH /extract $file.FullName * $destPath /fp
    }
}'''

# Set the base directory where your subfolders with w3x files are stored
$BASE_DIR = "D:\DSI_PCG\War3MapsDatabase\Maps"

# Set the extraction destination base directory
$DEST_BASE_DIR = "D:\DSI_PCG\ExtractedMaps"

# Ensure MPQEditor is in your PATH or specify its full path here
$MPQEDITOR_PATH = "C:\Users\claud\Desktop\DSI\2023-S2\PCG\Proyecto\mpqeditor_en_v3.6.0.868\Win32\MPQEditor.exe"

# Get all subdirectories in the base directory
$subDirs = Get-ChildItem -Path $BASE_DIR -Directory

# Loop through each subdirectory
foreach ($dir in $subDirs) {
    # Get all w3x files in the current subdirectory
    $w3xFiles = Get-ChildItem -Path $dir.FullName -Filter *.w3x
    
    # Loop through each w3x file in the current subdirectory
    foreach ($file in $w3xFiles) {
        $destPath = Join-Path $DEST_BASE_DIR ($dir.Name + '_' + $file.BaseName)
        # Ensure the destination directory exists
        if (-not (Test-Path $destPath)) {
            New-Item -ItemType Directory -Path $destPath | Out-Null
        }
        
        # Extract all files from the w3x file to the destination directory
        & $MPQEDITOR_PATH /extract $file.FullName * $destPath /fp

        # Close MPQEditor.exe process after each file extraction
        Get-Process | Where-Object {$_.Path -eq $MPQEDITOR_PATH} | Stop-Process
    }
}

