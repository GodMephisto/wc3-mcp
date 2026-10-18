---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/EventObjectUbrEmitter.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public class EventObjectUbrEmitter extends EventObjectEmitter<EventObjectEmitterObject, EventObjectSplUbr> {
    public EventObjectUbrEmitter(final MdxComplexInstance instance, final EventObjectEmitterObject emitterObject) {
        super(instance, emitterObject);
    }

    @Override
    protected EventObjectSplUbr createObject() {
        return new EventObjectSplUbr(this);
    }
}
