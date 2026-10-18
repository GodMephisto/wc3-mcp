---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/viewer5/handlers/ResourceHandler.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers;

import java.util.List;

import com.etheller.warsmash.viewer5.ModelViewer;
import com.etheller.warsmash.viewer5.HandlerResource;

public abstract class ResourceHandler {
	public ResourceHandler handler;
	public boolean load;
	public List<String[]> extensions;

	public abstract boolean load(ModelViewer modelViewer);

	public abstract HandlerResource<?> construct(ResourceHandlerConstructionParams params);
}
