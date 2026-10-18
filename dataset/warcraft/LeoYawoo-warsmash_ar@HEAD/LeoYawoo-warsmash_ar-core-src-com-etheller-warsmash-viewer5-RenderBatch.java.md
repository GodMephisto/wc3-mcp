---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/RenderBatch.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

import java.util.ArrayList;
import java.util.List;

/**
 * A render batch.
 */
public abstract class RenderBatch {
    public final Scene scene;
    public final Model<?> model;
    public final TextureMapper textureMapper;
    public final List<ModelInstance> instances = new ArrayList<>();
    public int count = 0;

    public RenderBatch(final Scene scene, final Model<?> model, final TextureMapper textureMapper) {
        this.scene = scene;
        this.model = model;
        this.textureMapper = textureMapper;
    }

    public abstract void render();

    public void clear() {
        this.count = 0;
    }

    public void add(final ModelInstance instance) {
        if (this.count == this.instances.size()) {
            this.instances.add(instance);
        } else if (this.count > this.instances.size()) {
            throw new IllegalStateException("count > size");
        } else {
            this.instances.set(this.count, instance);
        }
        this.count++;
    }
}
