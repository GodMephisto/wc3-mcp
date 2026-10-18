---
type: vendor-doc
source_repo: "https://github.com/Jaccouille/war3-assets-importer"
source_path: "src/main/java/com/hiveworkshop/war3assetsimporter/gui/TreeNodeData.java"
ref: HEAD
---

package com.hiveworkshop.war3assetsimporter.gui;

import com.hiveworkshop.war3assetsimporter.gui.i18n.Messages;

import java.util.Collections;
import java.util.List;

public final class TreeNodeData {
    private final String name;
    private final boolean isFile;
    private final String relativePath;
    private final long sizeInBytes;
    private final int fileCount;
    /**
     * Alternate-animation keywords detected in this MDX file (empty for non-MDX or
     * files without alternate animations). E.g. {@code ["Alternate", "Upgrade First"]}.
     */
    private final List<String> alternateAnims;

    public TreeNodeData(String name, boolean isFile, String relativePath, long sizeInBytes, int fileCount,
                        List<String> alternateAnims) {
        this.name = name;
        this.isFile = isFile;
        this.relativePath = relativePath;
        this.sizeInBytes = sizeInBytes;
        this.fileCount = fileCount;
        this.alternateAnims = alternateAnims != null ? Collections.unmodifiableList(alternateAnims)
                                                     : Collections.emptyList();
    }

    public TreeNodeData(String name, boolean isFile, String relativePath, long sizeInBytes, int fileCount) {
        this(name, isFile, relativePath, sizeInBytes, fileCount, Collections.emptyList());
    }

    private static String formatSize(long size) {
        if (size >= 1 << 20) return String.format("%.1f %s", size / 1024.0 / 1024, Messages.get("tree.size.mb"));
        if (size >= 1 << 10) return String.format("%.1f %s", size / 1024.0, Messages.get("tree.size.kb"));
        return size + " " + Messages.get("tree.size.b");
    }

    public String name() {
        return name;
    }

    public boolean isFile() {
        return isFile;
    }

    public String relativePath() {
        return relativePath;
    }

    public long sizeInBytes() {
        return sizeInBytes;
    }

    public int fileCount() {
        return fileCount;
    }

    public List<String> alternateAnims() {
        return alternateAnims;
    }

    @Override
    public String toString() {
        String sizeStr = formatSize(sizeInBytes);
        if (isFile) {
            if (alternateAnims.isEmpty()) {
                return name + " [" + sizeStr + "]";
            }
            return name + " [" + String.join(", ", alternateAnims) + "] [" + sizeStr + "]";
        } else {
            return name + " (" + fileCount + " " + Messages.get("tree.files") + ", " + sizeStr + ")";
        }
    }
}