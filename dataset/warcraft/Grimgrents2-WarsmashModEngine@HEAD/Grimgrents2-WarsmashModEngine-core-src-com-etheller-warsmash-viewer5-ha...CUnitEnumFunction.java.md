---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/CUnitEnumFunction.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

public interface CUnitEnumFunction {
	/**
	 * Operates on a unit, returning true if we should stop execution.
	 *
	 * @param unit
	 * @return
	 */
	boolean call(CUnit unit);
}
