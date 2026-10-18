---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/datasources/DataSourceDescriptor.java"
ref: HEAD
---

package com.etheller.warsmash.datasources;

import java.io.Serializable;

public interface DataSourceDescriptor extends Serializable {
	DataSource createDataSource();

	String getDisplayName();
}
