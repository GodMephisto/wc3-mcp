---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/hiveworkshop/wc3/gui/datachooser/JavaJarDataSourceDescriptor.java"
ref: HEAD
---

package com.hiveworkshop.wc3.gui.datachooser;

public class JavaJarDataSourceDescriptor implements DataSourceDescriptor {
	@Override
	public DataSource createDataSource() {
		return new JavaJarDataSource();
	}

	@Override
	public String getDisplayName() {
		return "JAR";
	}

	@Override
	public DataSourceDescriptor duplicate() {
		return this;
	}
}