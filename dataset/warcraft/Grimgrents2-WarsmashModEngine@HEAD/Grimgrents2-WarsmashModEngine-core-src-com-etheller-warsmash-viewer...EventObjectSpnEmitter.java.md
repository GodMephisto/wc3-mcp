---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/EventObjectSpnEmitter.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public class EventObjectSpnEmitter extends EventObjectEmitter<EventObjectEmitterObject, EventObjectSpn> {

	public EventObjectSpnEmitter(final MdxComplexInstance instance, final EventObjectEmitterObject emitterObject) {
		super(instance, emitterObject);
	}

	@Override
	protected EventObjectSpn createObject() {
		return new EventObjectSpn(this);
	}

}
