---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/CWidgetFilterFunction.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

public interface CWidgetFilterFunction {
	boolean call(CWidget unit);

	CWidgetFilterFunction ACCEPT_ALL = new CWidgetFilterFunction() {
		@Override
		public boolean call(final CWidget unit) {
			return true;
		}
	};

	CWidgetFilterFunction ACCEPT_ALL_LIVING = new CWidgetFilterFunction() {
		@Override
		public boolean call(final CWidget unit) {
			return !unit.isDead();
		}
	};
}
