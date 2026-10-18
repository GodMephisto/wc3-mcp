---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/rendersim/ability/EffectAttachmentUI.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.rendersim.ability;

import java.util.List;

public class EffectAttachmentUI {
    private final String modelPath;
    private final List<String> attachmentPoint;

    public EffectAttachmentUI(final String modelPath, final List<String> attachmentPoint) {
        this.modelPath = modelPath;
        this.attachmentPoint = attachmentPoint;
    }

    public String getModelPath() {
        return this.modelPath;
    }

    public List<String> getAttachmentPoint() {
        return this.attachmentPoint;
    }
}
