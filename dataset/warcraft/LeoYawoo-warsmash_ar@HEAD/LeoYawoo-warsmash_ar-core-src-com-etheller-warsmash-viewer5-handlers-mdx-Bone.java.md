---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/Bone.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.hiveworkshop.rms.parsers.mdlx.MdlxBone;

public class Bone extends GenericObject {

    private final GeosetAnimation geosetAnimation;

    public Bone(final MdxModel model, final MdlxBone bone, final int index) {
        super(model, bone, index);

        GeosetAnimation geosetAnimation = null;
        final int geosetId = bone.getGeosetId();
        if (geosetId != -1) {
            final Geoset geoset = model.getGeosets().get(geosetId);
            if (geoset.geosetAnimation != null) {
                geosetAnimation = geoset.geosetAnimation;
            } else {
                final int geosetAnimationId = bone.getGeosetAnimationId();
                if (geosetAnimationId != -1) {
                    geosetAnimation = model.getGeosetAnimations().get(geosetAnimationId);
                }
            }
        }
        this.geosetAnimation = geosetAnimation;
    }

    @Override
    public void getVisibility(final float[] out, final int sequence, final int frame, final int counter) {
        if (this.geosetAnimation != null) {
            this.geosetAnimation.getAlpha(out, sequence, frame, counter);
            return;
        }

        out[0] = 1;

    }

}
