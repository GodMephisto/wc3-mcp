---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/datachooser/DataSourceDescriptor.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.datachooser;

import java.io.Serializable;

public interface DataSourceDescriptor extends Serializable {
	DataSource createDataSource();

	String getDisplayName();

	DataSourceDescriptor duplicate();
}
