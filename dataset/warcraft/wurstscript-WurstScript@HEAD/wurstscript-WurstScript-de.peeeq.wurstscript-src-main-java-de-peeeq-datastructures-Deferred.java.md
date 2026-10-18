---
type: vendor-doc
source_repo: "https://github.com/wurstscript/WurstScript"
source_path: "de.peeeq.wurstscript/src/main/java/de/peeeq/datastructures/Deferred.java"
ref: HEAD
---

package de.peeeq.datastructures;

import java.util.function.Function;
import java.util.function.Supplier;

/**
 * Compute something lazily.
 *
 * At the moment, this is used to avoid cyclic dependencies.
 * This should be improved with a better design.
 */
public class Deferred<T> {
    private Supplier<T> supplier;
    private T value;

    public Deferred(Supplier<T> supplier) {
        this.supplier = supplier;
    }
    public Deferred(T value) {
        this.value = value;
    }

    public T get() {
        if (supplier != null) {
            value = supplier.get();
            supplier = null;
        }
        return value;
    }

    public <S> Deferred<S> map(Function<T, S> f) {
        if (supplier == null) {
            S newValue = f.apply(value);
            if (newValue == value) {
                //noinspection unchecked
                return (Deferred<S>) this;
            } else {
                return new Deferred<>(newValue);
            }
        }
        return new Deferred<>(() -> f.apply(get()));
    }

}
