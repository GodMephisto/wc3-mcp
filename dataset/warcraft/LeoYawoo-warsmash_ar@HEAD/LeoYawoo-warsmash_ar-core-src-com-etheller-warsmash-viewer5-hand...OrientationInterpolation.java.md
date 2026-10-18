---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/w3x/rendersim/OrientationInterpolation.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.w3x.rendersim;

/**
 * We observe this table during gameplay but I haven't found it anywhere in the
 * data yet. So, I'm making my own.
 */
public enum OrientationInterpolation {
    OI0(0.07f, 0.2f, 999f),
    OI1(0.03f, 0.1f, 0.04f),
    OI2(0.015f, 1.0f, 999),
    OI3(0.005f, 0.1f, 0.0043f),
    OI4(0.04f, 0.15f, 0.01f),
    OI5(0.05f, 0.18f, 0.015f),
    OI6(0.1f, 0.3f, 0.1f),
    OI7(0.003f, 0.08f, 0.0027f),
    OI8(0.001f, 0.05f, 0.001f);

    public static final OrientationInterpolation[] VALUES = values();

    private final float startingAcceleration;
    private final float maxVelocity;
    private final float endingNegativeAcceleration;
    private final float endingAccelCutoff;
    private final float startingAccelCutoff;

    OrientationInterpolation(final float startingAcceleration, final float maxVelocity,
                             final float endingNegativeAcceleration) {
        this.startingAcceleration = startingAcceleration;
        this.maxVelocity = maxVelocity;
        this.endingNegativeAcceleration = endingNegativeAcceleration;
        this.endingAccelCutoff = endingAccelCutoff(maxVelocity, endingNegativeAcceleration);
        this.startingAccelCutoff = endingAccelCutoff(maxVelocity, startingAcceleration);
    }

    private static float endingAccelCutoff(final float maxVelocity, final float endingAccel) {
        final float endingAccelFinishingTime = maxVelocity / endingAccel;
        return (maxVelocity * endingAccelFinishingTime)
                - ((endingAccel / 2) * (endingAccelFinishingTime * endingAccelFinishingTime));
    }

    public float getStartingAcceleration() {
        return this.startingAcceleration;
    }

    public float getMaxVelocity() {
        return this.maxVelocity;
    }

    public float getEndingNegativeAcceleration() {
        return this.endingNegativeAcceleration;
    }

    public float getEndingAccelCutoff() {
        return this.endingAccelCutoff;
    }

    public float getStartingAccelCutoff() {
        return this.startingAccelCutoff;
    }
}
