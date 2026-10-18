---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/datasources/GdxDataSourceDescriptor.java"
ref: HEAD
---

package com.etheller.warsmash.datasources;

public class GdxDataSourceDescriptor implements DataSourceDescriptor {

    @Override
    public DataSource createDataSource() {
        return new GdxDataSource();
    }

    @Override
    public String getDisplayName() {
        return "GDX";
    }
}
