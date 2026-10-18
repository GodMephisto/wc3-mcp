---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/Light.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.hiveworkshop.rms.parsers.mdlx.AnimationMap;
import com.hiveworkshop.rms.parsers.mdlx.MdlxLight;

public class Light extends GenericObject {

    private final Type type;
    private final float[] attenuation;
    private final float[] color;
    private final float intensity;
    private final float[] ambientColor;
    private final float ambientIntensity;

    public Light(final MdxModel model, final MdlxLight light, final int index) {
        super(model, light, index);

        switch (light.getType()) {
            case OMNIDIRECTIONAL:
                this.type = Type.OMNIDIRECTIONAL;
                break;
            case AMBIENT:
                this.type = Type.AMBIENT;
                break;
            case DIRECTIONAL:
            default:
                this.type = Type.DIRECTIONAL;
                break;
        }
        this.attenuation = light.getAttenuation();
        this.color = light.getColor();
        this.intensity = light.getIntensity();
        this.ambientColor = light.getAmbientColor();
        this.ambientIntensity = light.getAmbientIntensity();
    }

    public Type getType() {
        return this.type;
    }

    public void getAttenuationStart(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KLAS.getWar3id(), sequence, frame, counter, this.attenuation[0]);
    }

    public void getAttenuationEnd(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KLAE.getWar3id(), sequence, frame, counter, this.attenuation[1]);
    }

    public void getIntensity(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KLAI.getWar3id(), sequence, frame, counter, this.intensity);
    }

    public void getColor(final float[] out, final int sequence, final int frame, final int counter) {
        this.getVectorValue(out, AnimationMap.KLAC.getWar3id(), sequence, frame, counter, this.color);
    }

    public void getAmbientIntensity(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KLBI.getWar3id(), sequence, frame, counter, this.ambientIntensity);
    }

    public void getAmbientColor(final float[] out, final int sequence, final int frame, final int counter) {
        this.getVectorValue(out, AnimationMap.KLBC.getWar3id(), sequence, frame, counter, this.ambientColor);
    }

    @Override
    public void getVisibility(final float[] out, final int sequence, final int frame, final int counter) {
        this.getScalarValue(out, AnimationMap.KLAV.getWar3id(), sequence, frame, counter, 1);
    }

    public enum Type {
        // Omnidirectional light used for in-game sun
        OMNIDIRECTIONAL,
        // Directional light used for torches in the game world, and similar objects
        // that "glow"
        DIRECTIONAL,
        // Directional ambient light used for torches in the game world, and similar
        // objects that "glow"
        AMBIENT
    }
}
