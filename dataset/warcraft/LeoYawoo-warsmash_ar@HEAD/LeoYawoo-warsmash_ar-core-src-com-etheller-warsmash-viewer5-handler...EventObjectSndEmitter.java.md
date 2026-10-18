---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/EventObjectSndEmitter.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public class EventObjectSndEmitter extends EventObjectEmitter<EventObjectEmitterObject, EventObjectSnd> {

    public EventObjectSndEmitter(final MdxComplexInstance instance, final EventObjectEmitterObject emitterObject) {
        super(instance, emitterObject);
    }

    @Override
    protected EventObjectSnd createObject() {
        return new EventObjectSnd(this);
    }

}
