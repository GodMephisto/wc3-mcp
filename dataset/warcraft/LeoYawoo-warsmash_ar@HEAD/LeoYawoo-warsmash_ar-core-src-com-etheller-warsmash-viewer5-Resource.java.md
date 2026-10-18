---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/Resource.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

import java.io.InputStream;

public abstract class Resource {
    public final ModelViewer viewer;
    public final String extension;
    public final String fetchUrl;
    public final PathSolver pathSolver;
    public final Object solverParams = null;
    public boolean ok;
    public boolean loaded;

    public Resource(final ModelViewer viewer, final String extension, final PathSolver pathSolver,
                    final String fetchUrl) {
        this.viewer = viewer;
        this.extension = extension;
        this.pathSolver = pathSolver;
        this.fetchUrl = fetchUrl;
        this.ok = false;
        this.loaded = false;
    }

    public void loadData(final InputStream src, final Object options) {
        this.loaded = true;

        try {
            this.load(src, options);
            this.ok = true;
            this.lateLoad();
        } catch (final Exception e) {
            this.error(e);
        }
    }

    protected abstract void lateLoad();

    protected abstract void load(InputStream src, Object options);

    protected abstract void error(Exception e);
}
