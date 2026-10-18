---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/parsers/fdf/frames/ControlFrame.java"
ref: HEAD
---

package com.etheller.warsmash.parsers.fdf.frames;

import com.badlogic.gdx.graphics.g2d.BitmapFont;
import com.badlogic.gdx.graphics.g2d.GlyphLayout;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.utils.viewport.Viewport;
import com.etheller.warsmash.parsers.fdf.GameUI;

public class ControlFrame extends AbstractRenderableFrame {

    private UIFrame controlBackdrop;

    public ControlFrame(final String name, final UIFrame parent) {
        super(name, parent);
    }

    public UIFrame getControlBackdrop() {
        return this.controlBackdrop;
    }

    public void setControlBackdrop(final UIFrame controlBackdrop) {
        this.controlBackdrop = controlBackdrop;
    }

    @Override
    protected void innerPositionBounds(final GameUI gameUI, final Viewport viewport) {
        if (this.controlBackdrop != null) {
            this.controlBackdrop.positionBounds(gameUI, viewport);
        }
    }

    @Override
    protected void internalRender(final SpriteBatch batch, final BitmapFont baseFont, final GlyphLayout glyphLayout) {
        if (this.controlBackdrop != null) {
            this.controlBackdrop.render(batch, baseFont, glyphLayout);
        }
    }

}
