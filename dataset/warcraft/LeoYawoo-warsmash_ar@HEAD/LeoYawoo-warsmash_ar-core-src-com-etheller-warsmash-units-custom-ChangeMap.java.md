---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/units/custom/ChangeMap.java"
ref: HEAD
---

package com.etheller.warsmash.units.custom;

import com.etheller.warsmash.util.War3ID;

import java.util.*;

public final class ChangeMap implements Iterable<Map.Entry<War3ID, List<Change>>> {
    private final Map<War3ID, List<Change>> idToChanges = new LinkedHashMap<>();

    public void add(final War3ID war3Id, final Change change) {
        List<Change> list = this.idToChanges.computeIfAbsent(war3Id, k -> new ArrayList<>());
        list.add(change);
    }

    public void add(final War3ID war3Id, final List<Change> changes) {
        for (final Change change : changes) {
            add(war3Id, change);
        }
    }

    public List<Change> get(final War3ID war3ID) {
        return this.idToChanges.get(war3ID);
    }

    public void delete(final War3ID war3ID, final Change change) {
        if (this.idToChanges.containsKey(war3ID)) {
            final List<Change> changeList = this.idToChanges.get(war3ID);
            changeList.remove(change);
            if (changeList.isEmpty()) {
                this.idToChanges.remove(war3ID);
            }
        }
    }

    @Override
    public Iterator<Map.Entry<War3ID, List<Change>>> iterator() {
        return this.idToChanges.entrySet().iterator();
    }

    public int size() {
        return this.idToChanges.size();
    }

    public void clear() {
        this.idToChanges.clear();
    }
}
