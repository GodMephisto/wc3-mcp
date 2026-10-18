---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/GridCell.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

import com.etheller.warsmash.util.RenderMathUtils;

import java.util.ArrayList;
import java.util.List;

public class GridCell {
    public final float left;
    public final float right;
    public final float bottom;
    public final float top;
    public final boolean visible;
    final List<ModelInstance> instances;
    public int plane;

    public GridCell(final float left, final float right, final float bottom, final float top) {
        this.left = left;
        this.right = right;
        this.bottom = bottom;
        this.top = top;
        this.plane = -1;
        this.instances = new ArrayList<>();
        this.visible = false;
    }

    public void add(final ModelInstance instance) {
        this.instances.add(instance);
    }

    public void remove(final ModelInstance instance) {
        this.instances.remove(instance);
    }

    public void clear() {
        this.instances.clear();
    }

    public void isVisible(final Camera camera) {
        this.plane = RenderMathUtils.testCell(camera.planes, this.left, this.right, this.bottom, this.top, this.plane);

    }
}
