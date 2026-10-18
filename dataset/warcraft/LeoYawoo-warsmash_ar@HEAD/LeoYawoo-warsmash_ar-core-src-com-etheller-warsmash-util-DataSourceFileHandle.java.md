---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/util/DataSourceFileHandle.java"
ref: HEAD
---

package com.etheller.warsmash.util;

import com.badlogic.gdx.files.FileHandle;
import com.etheller.warsmash.datasources.DataSource;

import java.io.IOException;
import java.io.InputStream;

public class DataSourceFileHandle extends FileHandle {
    private final DataSource dataSource;

    public DataSourceFileHandle(final DataSource dataSource, final String path) {
        super(fixPath(dataSource, path));
        this.dataSource = dataSource;
    }

    private static String fixPath(final DataSource dataSource, String path) {
        if (!dataSource.has(path) && (path.toLowerCase().endsWith(".wav") || path.toLowerCase().endsWith(".mp3"))) {
            final String otherPossiblePath = path.substring(0, path.lastIndexOf('.')) + ".flac";
            if (dataSource.has(otherPossiblePath)) {
                path = otherPossiblePath;
            }
        }
        return path;
    }

    @Override
    public String path() {
        return file().getPath();
    }

    @Override
    public InputStream read() {
        try {
            return this.dataSource.getResourceAsStream(path());
        } catch (final IOException e) {
            throw new RuntimeException("Failed to load FileHandle from DataSource: " + path());
        }
    }
}
