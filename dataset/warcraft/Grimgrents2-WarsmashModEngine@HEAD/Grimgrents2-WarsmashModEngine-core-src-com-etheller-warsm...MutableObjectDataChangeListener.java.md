---
type: vendor-doc
source_repo: "https://github.com/Grimgrents2/WarsmashModEngine"
source_path: "core/src/com/etheller/warsmash/units/manager/MutableObjectDataChangeListener.java"
ref: HEAD
---

package com.etheller.warsmash.units.manager;

import com.etheller.warsmash.util.War3ID;

public interface MutableObjectDataChangeListener {
	void textChanged(War3ID changedObject);

	void iconsChanged(War3ID changedObject);

	void categoriesChanged(War3ID changedObject);

	void fieldsChanged(War3ID changedObject);

	void modelChanged(War3ID changedObject);

	void objectCreated(War3ID newObject);

	void objectsCreated(War3ID[] newObject);

	void objectRemoved(War3ID removedObject);

	void objectsRemoved(War3ID[] removedObject);
}
