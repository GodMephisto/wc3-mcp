---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/datasources/CompoundDataSourceDescriptor.java"
ref: HEAD
---

package com.etheller.warsmash.datasources;

import java.util.ArrayList;
import java.util.List;

public class CompoundDataSourceDescriptor implements DataSourceDescriptor {
	private final List<DataSourceDescriptor> dataSourceDescriptors;

	public CompoundDataSourceDescriptor(final List<DataSourceDescriptor> dataSourceDescriptors) {
		this.dataSourceDescriptors = dataSourceDescriptors;
	}

	@Override
	public DataSource createDataSource() {
		final List<DataSource> dataSources = new ArrayList<>();
		for (final DataSourceDescriptor descriptor : this.dataSourceDescriptors) {
			dataSources.add(descriptor.createDataSource());
		}
		return new CompoundDataSource(dataSources);
	}

	@Override
	public String getDisplayName() {
		return "CompoundDataSourceDescriptor";
	}

}
