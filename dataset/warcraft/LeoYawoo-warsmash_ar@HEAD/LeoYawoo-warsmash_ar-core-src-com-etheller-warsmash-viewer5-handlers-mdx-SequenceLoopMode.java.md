---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/SequenceLoopMode.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public enum SequenceLoopMode {
    NEVER_LOOP,
    MODEL_LOOP,
    ALWAYS_LOOP,
    NEVER_LOOP_AND_HIDE_WHEN_DONE, // used by spawned effects
    LOOP_TO_NEXT_ANIMATION // used by the Arthas vs Illidan tech demo
}
