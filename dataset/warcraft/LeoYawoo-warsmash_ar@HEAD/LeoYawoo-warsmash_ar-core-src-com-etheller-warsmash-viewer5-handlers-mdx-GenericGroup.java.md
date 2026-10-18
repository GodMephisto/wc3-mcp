---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/GenericGroup.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.badlogic.gdx.math.Matrix4;

import java.util.ArrayList;
import java.util.List;

public abstract class GenericGroup {
    public final List<Integer> objects;

    public GenericGroup() {
        this.objects = new ArrayList<>(); // TODO IntArrayList
    }

    public abstract void render(MdxComplexInstance instance, Matrix4 mvp);

}
