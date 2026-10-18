---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/MdxHandler.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.badlogic.gdx.graphics.glutils.ShaderProgram;
import com.etheller.warsmash.viewer5.HandlerResource;
import com.etheller.warsmash.viewer5.ModelViewer;
import com.etheller.warsmash.viewer5.handlers.ModelHandler;
import com.etheller.warsmash.viewer5.handlers.ResourceHandlerConstructionParams;
import com.etheller.warsmash.viewer5.handlers.blp.BlpHandler;
import com.etheller.warsmash.viewer5.handlers.blp.DdsHandler;
import com.etheller.warsmash.viewer5.handlers.tga.TgaHandler;

import java.util.ArrayList;

public class MdxHandler extends ModelHandler {
    public static ShaderEnvironmentType CURRENT_SHADER_TYPE;
    public final Shaders shaders = new Shaders();

    public MdxHandler() {
        this.extensions = new ArrayList<>();
        this.extensions.add(new String[]{".mdx", "arrayBuffer"});
        this.extensions.add(new String[]{".mdl", "text"});
        this.load = true;
    }

    @Override
    public boolean load(final ModelViewer viewer) {
        viewer.addHandler(new BlpHandler());
        viewer.addHandler(new DdsHandler());
        viewer.addHandler(new TgaHandler());

        this.shaders.complex = viewer.webGL.createShaderProgram(MdxShaders.vsComplex(), MdxShaders.fsComplex);
        this.shaders.extended = viewer.webGL.createShaderProgram("#define EXTENDED_BONES\r\n" + MdxShaders.vsComplex(),
                MdxShaders.fsComplex);
        this.shaders.complexShadowMap = viewer.webGL.createShaderProgram(MdxShaders.vsComplex(),
                MdxShaders.fsComplexShadowMap);
        this.shaders.extendedShadowMap = viewer.webGL.createShaderProgram(
                "#define EXTENDED_BONES\r\n" + MdxShaders.vsComplex(), MdxShaders.fsComplexShadowMap);
        this.shaders.particles = viewer.webGL.createShaderProgram(MdxShaders.vsParticles(), MdxShaders.fsParticles);
        this.shaders.hd = viewer.webGL.createShaderProgram(MdxShaders.vsHd, MdxShaders.fsHd());
        // TODO HD reforged

        // If a shader failed to compile, don't allow the handler to be registered, and
        // send an error instead.
        return this.shaders.complex.isCompiled() && this.shaders.extended.isCompiled()
                && this.shaders.particles.isCompiled()
                /* && Shaders.simple.isCompiled() && Shaders.hd.isCompiled() */;
    }

    @Override
    public HandlerResource<?> construct(final ResourceHandlerConstructionParams params) {
        return new MdxModel((MdxHandler) params.getHandler(), params.getViewer(), params.getExtension(),
                params.getPathSolver(), params.getFetchUrl());
    }

    public enum ShaderEnvironmentType {
        MENU,
        GAME
    }

    public static final class Shaders {
        public ShaderProgram complex;
        public ShaderProgram complexShadowMap;
        public ShaderProgram extended;
        public ShaderProgram extendedShadowMap;
        public ShaderProgram simple;
        public ShaderProgram particles;
        public ShaderProgram hd;

        private Shaders() {

        }
    }
}
