---
type: vendor-doc
source_repo: "https://github.com/dimonp/assets_mpq_importer"
source_path: "scripts/mpq_import.sh"
ref: HEAD
---

#!/bin/bash

if [ "$#" -lt 2 ]; then
    echo "Usage: $0 <path to input mpq archive> <path to output folder>"
    echo "Example: $0 ""path/to/war3.mpq"" ""otput/folder"""
    echo ""
    echo "This script processes files."

    exit 1
fi

find_executable() {
    local executable_name="$1"
    local path=""

    # 1. Check if the executable is in the system's PATH
    if command -v "$executable_name" &> /dev/null; then
        path=$(command -v "$executable_name")
    fi

    # 2. If not found in PATH, perform a recursive search in the current directory and subdirectories
    if [ -z "$path" ]; then
        # Look for a file with the given name (-type f) that is executable (-executable)
        # -print -quit ensures we return only the first match and stop immediately
        path=$(find . -type f -name "$executable_name" -executable -print -quit 2>/dev/null)

        # Note: If your 'find' version doesn't support -executable, use -perm /111 instead:
        # path=$(find . -type f -name "$executable_name" -perm /111 -print -quit 2>/dev/null)
    fi

    echo "$path"
}

importer_path=$(find_executable "importer")
merger_path=$(find_executable "merger")

if [[ -z "$importer_path" ]]; then
    echo "Error. Importer executable not found." >&2
    exit 1
fi

if [[ -z "$importer_path" || -z "$merger_path" ]]; then
    echo "Error. Merger executable not found." >&2
    exit 1
fi

compression="BC3"

echo ${importer_path}

# Extract W3E map files
"${importer_path}" --filter="*.w3m" --input="$1" --output="$2"
"${importer_path}" --filter="*.w3x" --input="$1" --output="$2"

# Import terrain textures to DDS wth mipmaps
${importer_path} --dds --compression=${compression} --filter="*TerrainArt*.blp" --input="$1" --output="$2"
${importer_path} --dds --compression=${compression} --filter="*ReplaceableTextures*Cliff*.blp" --input="$1" --output="$2"

# Import terrain MDX meshes
${importer_path} --filter="*Doodads*Terrain*Cliffs*.mdx" --input="$1" --output="$2"
${importer_path} --filter="*Doodads*Terrain*CliffTrans*.mdx" --input="$1" --output="$2"

mkdir -p $2/Geosets

# Merge ground cliffs and ramps
${merger_path} --name="ground" --pattern="Cliffs([a-zA-Z0-9]{5})\.obj" --input="$2/Doodads/Terrain/Cliffs" --output="$2/Geosets"
${merger_path} --ramp --append --name="ground" --pattern="CliffTrans([a-zA-Z0-9]{5})\.obj" --input="$2/Doodads/Terrain/CliffTrans" --output="$2/Geosets"

# Merge city cliffs and ramps
${merger_path} --name="city" --pattern="CityCliffs([a-zA-Z0-9]{5})\.obj" --input="$2/Doodads/Terrain/CityCliffs" --output="$2/Geosets"
${merger_path} --ramp --append --name="city" --pattern="CityCliffTrans([a-zA-Z0-9]{5})\.obj" --input="$2/Doodads/Terrain/CityCliffTrans" --output="$2/Geosets"

rm -r $2/Doodads
rm $2/Geosets/*.mtl

echo Done.