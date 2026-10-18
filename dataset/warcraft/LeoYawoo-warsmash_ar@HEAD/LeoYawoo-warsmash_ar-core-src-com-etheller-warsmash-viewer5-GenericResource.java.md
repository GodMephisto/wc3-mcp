---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/GenericResource.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5;

import com.etheller.warsmash.common.LoadGenericCallback;

import java.io.InputStream;

public final class GenericResource extends Resource {

    // using the data
    private final LoadGenericCallback callback;
    public Object data; // TODO this likely won't work, just brainstorming until I get to the part of

    public GenericResource(final ModelViewer viewer, final String extension, final PathSolver pathSolver,
                           final String fetchUrl, final LoadGenericCallback callback) {
        super(viewer, extension, pathSolver, fetchUrl);
        this.callback = callback;
    }

    @Override
    protected void lateLoad() {

    }

    @Override
    protected void load(final InputStream src, final Object options) {
        this.data = this.callback.call(src);

    }

    @Override
    protected void error(final Exception e) {
        e.printStackTrace();
    }

}
