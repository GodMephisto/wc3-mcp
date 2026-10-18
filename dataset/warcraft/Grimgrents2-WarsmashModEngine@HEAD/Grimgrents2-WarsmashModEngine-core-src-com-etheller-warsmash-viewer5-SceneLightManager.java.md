---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/viewer5/SceneLightManager.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

public interface SceneLightManager {
	public void add(final SceneLightInstance lightInstance);

	public void remove(final SceneLightInstance lightInstance);

	public void update();
}
