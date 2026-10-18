---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/MdxNodeDescriptor.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.etheller.warsmash.util.Descriptor;

public class MdxNodeDescriptor implements Descriptor<MdxNode> {
    public static final MdxNodeDescriptor INSTANCE = new MdxNodeDescriptor();

    @Override
    public MdxNode create() {
        return new MdxNode();
    }

}
