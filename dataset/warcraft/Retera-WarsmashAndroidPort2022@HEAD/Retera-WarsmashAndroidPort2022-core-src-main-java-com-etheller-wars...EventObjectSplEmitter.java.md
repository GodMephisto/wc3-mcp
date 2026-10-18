---
type: vendor-doc
source_repo: "https://github.com/Retera/WarsmashAndroidPort2022"
source_path: "core/src/main/java/com/etheller/warsmash/viewer5/handlers/mdx/EventObjectSplEmitter.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

public class EventObjectSplEmitter extends EventObjectEmitter<EventObjectEmitterObject, EventObjectSplUbr> {
	public EventObjectSplEmitter(final MdxComplexInstance instance, final EventObjectEmitterObject emitterObject) {
		super(instance, emitterObject);
	}

	@Override
	protected EventObjectSplUbr createObject() {
		return new EventObjectSplUbr(this);
	}
}
