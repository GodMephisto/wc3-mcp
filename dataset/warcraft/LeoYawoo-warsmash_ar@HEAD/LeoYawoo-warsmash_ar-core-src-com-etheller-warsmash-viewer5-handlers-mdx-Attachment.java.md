---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/Attachment.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.hiveworkshop.rms.parsers.mdlx.AnimationMap;
import com.hiveworkshop.rms.parsers.mdlx.MdlxAttachment;

import java.util.Locale;

public class Attachment extends GenericObject {
    protected final String path;
    protected final int attachmentId;
    protected final String name;
    protected MdxModel internalModel;

    public Attachment(final MdxModel model, final MdlxAttachment attachment, final int index) {
        super(model, attachment, index);

        final String path = attachment.getPath().toLowerCase().replace(".mdl", ".mdx");

        this.name = attachment.getName().toLowerCase(Locale.US);
        this.path = path;
        this.attachmentId = attachment.getAttachmentId();
        this.internalModel = null;

        // Second condition is against custom resources using arbitrary paths
        if ((path.contains(".mdx"))) {
            this.internalModel = (MdxModel) model.viewer.load(path, model.pathSolver, model.solverParams);
        }
    }

    @Override
    public void getVisibility(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KATV.getWar3id(), sequence, frame, counter, 1);
    }

    public String getName() {
        return this.name;
    }

    public int getAttachmentId() {
        return this.attachmentId;
    }
}
