---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/datasources/CompoundDataSource.java"
ref: HEAD
---

package com.etheller.warsmash.datasources;

import java.io.File;
import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.util.*;

public class CompoundDataSource implements DataSource {
    private final List<DataSource> mpqList = new ArrayList<>();
    final Map<String, File> cache = new HashMap<>();

    public CompoundDataSource(final List<DataSource> dataSources) {
        if (dataSources != null) {
            this.mpqList.addAll(dataSources);
        }
    }

    @Override
    public File getFile(final String filepath) {
        if (this.cache.containsKey(filepath)) {
            return this.cache.get(filepath);
        }
        try {
            for (int i = this.mpqList.size() - 1; i >= 0; i--) {
                final DataSource mpq = this.mpqList.get(i);
                final File tempProduct = mpq.getFile(filepath);
                if (tempProduct != null) {
                    this.cache.put(filepath, tempProduct);
                    return tempProduct;
                }
            }
        } catch (final IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public File getDirectory(String filepath) {
        try {
            for (int i = this.mpqList.size() - 1; i >= 0; i--) {
                final DataSource mpq = this.mpqList.get(i);
                final File tempProduct = mpq.getDirectory(filepath);
                if (tempProduct != null) {
                    return tempProduct;
                }
            }
        } catch (final IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public ByteBuffer read(final String path) {
        try {
            for (int i = this.mpqList.size() - 1; i >= 0; i--) {
                final DataSource mpq = this.mpqList.get(i);
                final ByteBuffer buffer = mpq.read(path);
                if (buffer != null) {
                    return buffer;
                }
            }
        } catch (final IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public InputStream getResourceAsStream(final String filepath) {
        try {
            for (int i = this.mpqList.size() - 1; i >= 0; i--) {
                final DataSource mpq = this.mpqList.get(i);
                final InputStream resourceAsStream = mpq.getResourceAsStream(filepath);
                if (resourceAsStream != null) {
                    return resourceAsStream;
                }
            }
        } catch (final IOException e) {
            // TODO Auto-generated catch block
            e.printStackTrace();
        }
        return null;
    }

    @Override
    public boolean has(final String filepath) {
        if (this.cache.containsKey(filepath)) {
            return true;
        }
        for (int i = this.mpqList.size() - 1; i >= 0; i--) {
            final DataSource mpq = this.mpqList.get(i);
            if (mpq.has(filepath)) {
                return true;
            }
        }
        return false;
    }

    public Set<String> getMergedListfile() {
        final Set<String> listfile = new HashSet<>();
        for (final DataSource mpqGuy : this.mpqList) {
            final Collection<String> dataSourceListfile = mpqGuy.getListfile();
            if (dataSourceListfile != null) {
                listfile.addAll(dataSourceListfile);
            }
        }
        return listfile;
    }

    @Override
    public Collection<String> getListfile() {
        return getMergedListfile();
    }

    @Override
    public void close() throws IOException {
        for (final DataSource mpqGuy : this.mpqList) {
            mpqGuy.close();
        }
    }

}
