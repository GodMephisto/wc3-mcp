---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/simulation/HandleIdAllocator.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.simulation;

/**
 * This class is not similar to how WC3 allocates handle IDs in any way.
 * Changing this would probably be necessary to support TimerUtils madness,
 * because I forget how it works but I know it uses subtraction on handle IDs.
 */
public class HandleIdAllocator {
	private int next = 3412532; // bogus number

	public int createId() {
		return this.next++;
	}
}
