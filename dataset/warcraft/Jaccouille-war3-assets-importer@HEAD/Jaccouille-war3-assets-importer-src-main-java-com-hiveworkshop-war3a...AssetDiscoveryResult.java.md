---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/war3-assets-importer"
source_path: "src/main/java/com/hiveworkshop/war3assetsimporter/core/model/AssetDiscoveryResult.java"
ref: HEAD
---

package com.hiveworkshop.war3assetsimporter.core.model;

import java.io.File;
import java.util.*;

public final class AssetDiscoveryResult {
    private final List<String> mdxFiles;
    private final List<String> textureFiles;
    private final File rootFolder;
    private final Map<String, Long> fileSizes;
    /**
     * Maps relative MDX path → list of alternate-animation keywords detected in that file
     * (e.g. {@code ["Alternate", "Upgrade First"]}). Only MDX files with at least one
     * such keyword are present as keys.
     */
    private final Map<String, List<String>> mdxAlternateAnims;

    public AssetDiscoveryResult(List<String> mdxFiles, List<String> textureFiles,
                                File rootFolder, Map<String, Long> fileSizes,
                                Map<String, List<String>> mdxAlternateAnims) {
        this.mdxFiles = Collections.unmodifiableList(new ArrayList<>(mdxFiles));
        this.textureFiles = Collections.unmodifiableList(new ArrayList<>(textureFiles));
        this.rootFolder = rootFolder;
        this.fileSizes = Collections.unmodifiableMap(new HashMap<>(fileSizes));
        this.mdxAlternateAnims = Collections.unmodifiableMap(new HashMap<>(mdxAlternateAnims));
    }

    public AssetDiscoveryResult(List<String> mdxFiles, List<String> textureFiles,
                                File rootFolder, Map<String, Long> fileSizes) {
        this(mdxFiles, textureFiles, rootFolder, fileSizes, Collections.emptyMap());
    }

    /**
     * Backward-compatible constructor (CLI path — sizes not needed).
     */
    public AssetDiscoveryResult(List<String> mdxFiles, List<String> textureFiles, File rootFolder) {
        this(mdxFiles, textureFiles, rootFolder, Collections.emptyMap(), Collections.emptyMap());
    }

    public List<String> mdxFiles() {
        return mdxFiles;
    }

    public List<String> textureFiles() {
        return textureFiles;
    }

    public File rootFolder() {
        return rootFolder;
    }

    public Map<String, Long> fileSizes() {
        return fileSizes;
    }

    public Map<String, List<String>> mdxAlternateAnims() {
        return mdxAlternateAnims;
    }

    public int totalFileCount() {
        return mdxFiles.size() + textureFiles.size();
    }
}