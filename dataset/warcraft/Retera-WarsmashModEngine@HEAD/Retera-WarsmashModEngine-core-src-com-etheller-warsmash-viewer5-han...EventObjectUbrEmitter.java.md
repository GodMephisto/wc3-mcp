---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/EventObjectUbrEmitter.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public class EventObjectUbrEmitter extends EventObjectEmitter<EventObjectEmitterObject, EventObjectUbr> {
	public EventObjectUbrEmitter(final MdxComplexInstance instance, final EventObjectEmitterObject emitterObject) {
		super(instance, emitterObject);
	}

	@Override
	protected EventObjectUbr createObject() {
		return new EventObjectUbr(this);
	}

}
