---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/ResourceHandler.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers;

import com.etheller.warsmash.viewer5.HandlerResource;
import com.etheller.warsmash.viewer5.ModelViewer;

import java.util.List;

public abstract class ResourceHandler {
    public ResourceHandler handler;
    public boolean load;
    public List<String[]> extensions;

    public abstract boolean load(ModelViewer modelViewer);

    public abstract HandlerResource<?> construct(ResourceHandlerConstructionParams params);
}
