---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/datasources/MpqDataSourceDescriptor.java"
ref: HEAD
---

package com.etheller.warsmash.datasources;

import mpq.MPQArchive;
import mpq.MPQException;

import java.io.IOException;
import java.nio.channels.SeekableByteChannel;
import java.nio.file.Files;
import java.nio.file.Paths;
import java.nio.file.StandardOpenOption;
import java.util.EnumSet;

public class MpqDataSourceDescriptor implements DataSourceDescriptor {
    /**
     * Generated serial id
     */
    private static final long serialVersionUID = 8424254987711783598L;
    private final String mpqFilePath;

    public MpqDataSourceDescriptor(final String mpqFilePath) {
        this.mpqFilePath = mpqFilePath;
    }

    @Override
    public DataSource createDataSource() {
        try {
            SeekableByteChannel sbc;
            sbc = Files.newByteChannel(Paths.get(this.mpqFilePath), EnumSet.of(StandardOpenOption.READ));
            return new MpqDataSource(new MPQArchive(sbc), sbc);
        } catch (final IOException | MPQException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    public String getDisplayName() {
        return "MPQ Archive: " + this.mpqFilePath;
    }

    @Override
    public int hashCode() {
        final int prime = 31;
        int result = 1;
        result = (prime * result) + ((this.mpqFilePath == null) ? 0 : this.mpqFilePath.hashCode());
        return result;
    }

    @Override
    public boolean equals(final Object obj) {
        if (this == obj) {
            return true;
        }
        if (obj == null) {
            return false;
        }
        if (getClass() != obj.getClass()) {
            return false;
        }
        final MpqDataSourceDescriptor other = (MpqDataSourceDescriptor) obj;
        if (this.mpqFilePath == null) {
            return other.mpqFilePath == null;
        } else return this.mpqFilePath.equals(other.mpqFilePath);
    }

}
