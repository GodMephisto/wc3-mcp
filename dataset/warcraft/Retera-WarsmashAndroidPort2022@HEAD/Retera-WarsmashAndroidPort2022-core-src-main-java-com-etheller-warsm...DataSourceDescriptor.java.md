---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/datasources/DataSourceDescriptor.java"
ref: HEAD
---

package com.etheller.warsmash.datasources;

import java.io.Serializable;

public interface DataSourceDescriptor extends Serializable {
	DataSource createDataSource();

	String getDisplayName();
}
