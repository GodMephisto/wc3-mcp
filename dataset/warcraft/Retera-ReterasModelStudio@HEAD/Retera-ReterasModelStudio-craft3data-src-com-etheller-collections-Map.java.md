---
type: vendor-doc
source_repo: "https://github.com/Retera/ReterasModelStudio"
source_path: "craft3data/src/com/etheller/collections/Map.java"
ref: HEAD
---

package com.etheller.collections;

public interface Map<KEY, VALUE> extends MapView<KEY, VALUE> {
	void clear();

	VALUE remove(KEY key);

	VALUE put(KEY key, VALUE value);

	@Override
	Set<MapView.Entry<KEY, VALUE>> entrySet();
}
