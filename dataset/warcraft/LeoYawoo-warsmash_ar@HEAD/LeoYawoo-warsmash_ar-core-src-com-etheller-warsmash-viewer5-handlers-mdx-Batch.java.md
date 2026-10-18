---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/Batch.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public class Batch implements GenericIndexed {
    public final SkinningType skinningType;
    public final boolean hd;
    public final int index;
    public final Geoset geoset;
    public final Layer layer;
    public final Material material;

    public Batch(final int index, final Geoset geoset, final Layer layer, final SkinningType skinningType) {
        this.index = index;
        this.geoset = geoset;
        this.layer = layer;
        this.material = null;
        this.skinningType = skinningType;
        this.hd = false;
    }

    public Batch(final int index, final Geoset geoset, final Material material, final SkinningType skinningType) {
        this.index = index;
        this.geoset = geoset;
        this.material = material;
        this.layer = material.layers.get(0);
        this.skinningType = skinningType;
        this.hd = true;
    }

    @Override
    public int getIndex() {
        return this.index;
    }

}
