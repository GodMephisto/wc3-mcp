---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/war3-assets-importer"
source_path: "README.md"
ref: HEAD
---

# War3AssetsImporter

A desktop tool for batch import custom 3D models (MDX) and textures into Warcraft 3 map files (`.w3x` / `.w3m`).

Built with Java Swing. Work in progress.

---

## Features

- Browse and select MDX and texture assets from a folder tree
- Preview textures before importing
- Automatically generates unit definitions and places them on the map
- Draws a placement preview so you can see where units will appear

See more by clicking on the Help button

## Usage

1. Click **Open Map** and select a `.w3x` or `.w3m` file
2. Click **Open Assets Folder** and point to your MDX/textures directory
3. Check the assets you want to import in the tree panel
4. Configure unit naming and placement options in the **Import Configuration** tab
5. Click **Process & Save**

## Video


https://github.com/user-attachments/assets/fbe70dc2-c9b8-428f-a1bf-4e5dca47ef6f

## Screenshots

### Assets Panel

![Assets Panel](docs/images/asset_panel.jpg)

### Import Configuration Panel

![Import Configuration Panel](docs/images/import_configuration_panel.jpg)

## Building from Source

Requires JDK 17+ (the project uses a Java 17 toolchain).
The Gradle wrapper is included, so no global Gradle installation is required.

```bash
# Clone the repo
git clone https://github.com/YOUR_USERNAME/War3AssetsImporter.git
cd War3AssetsImporter

# Build and run tests
./gradlew clean build

# Run the app from source
./gradlew run

# Build the standalone JAR
./gradlew shadowJar
# Output: build/libs/War3AssetsImporter.jar
```

On Windows, use `gradlew.bat` instead of `./gradlew`.

### Build Windows `.exe` (portable app-image)

```powershell
.\gradlew.bat clean jpackageTask
```

Output:
- `build\dist\War3AssetsImporter\War3AssetsImporter.exe`

Notes:
- `jpackageTask` depends on `shadowJar` and `jlinkTask`, so it builds everything needed.
- This produces a portable app-image directory (not an MSI installer).

## Deploying a New Version

This repository has a release workflow in `.github/workflows/release.yml` that runs when a tag like `v1.2.3` is pushed.

### Quick Release Checklist

| Step | Command / Action |
| --- | --- |
| 1. Bump version | Update `version 'x.y.z'` in `build.gradle` |
| 2. Commit + push | `git add build.gradle README.md && git commit -m "Release vX.Y.Z" && git push` |
| 3. Tag + push | `git tag vX.Y.Z && git push origin vX.Y.Z` |
| 4. Verify workflow | Check GitHub Actions `Release` workflow status |
| 5. Verify assets | Confirm release contains Windows/macOS/Linux zip files |

1. Update the version in `build.gradle`:
```groovy
version 'x.y.z'
```
2. Commit and push your changes:
```bash
git add build.gradle README.md
git commit -m "Release vX.Y.Z"
git push
```
3. Create and push the release tag:
```bash
git tag vX.Y.Z
git push origin vX.Y.Z
```
4. Wait for the GitHub Actions `Release` workflow to finish.
5. Verify the GitHub Release was created with the attached platform zip files:
- `War3AssetsImporter-windows.zip`
- `War3AssetsImporter-macos.zip`
- `War3AssetsImporter-linux.zip`

## Credits

[wc3libs](https://github.com/inwc3/wc3libs) - Java library for reading and modifying Warcraft III maps and game files
[blp-iio-plugin](https://github.com/DrSuperGood/blp-iio-plugin/tree/master) - Java Image IO plugins for BLP texture files

## License

[MIT](LICENSE)
