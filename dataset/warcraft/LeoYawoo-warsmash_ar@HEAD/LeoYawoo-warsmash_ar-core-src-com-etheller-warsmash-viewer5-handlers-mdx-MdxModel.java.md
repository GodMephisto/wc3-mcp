---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/viewer5/handlers/mdx/MdxModel.java"
ref: HEAD
---

package com.etheller.warsmash.viewer5.handlers.mdx;

import com.badlogic.gdx.graphics.GL20;
import com.etheller.warsmash.viewer5.ModelInstance;
import com.etheller.warsmash.viewer5.ModelViewer;
import com.etheller.warsmash.viewer5.PathSolver;
import com.etheller.warsmash.viewer5.Texture;
import com.hiveworkshop.rms.parsers.mdlx.*;
import com.hiveworkshop.rms.parsers.mdlx.MdlxTexture.WrapMode;
import org.apache.commons.compress.utils.IOUtils;

import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.util.ArrayList;
import java.util.List;

public class MdxModel extends com.etheller.warsmash.viewer5.Model<MdxHandler> {
    public boolean reforged = false;
    public boolean hd = false;
    public final SolverParams solverParams = new SolverParams();
    public String name = "";
    public long blendTime;
    public final List<Sequence> sequences = new ArrayList<>();
    public final List<Long> globalSequences = new ArrayList<>();
    public final List<Material> materials = new ArrayList<>();
    public final List<Layer> layers = new ArrayList<>();
    public final List<Integer> replaceables = new ArrayList<>();
    public final List<Texture> textures = new ArrayList<>();
    public final List<TextureAnimation> textureAnimations = new ArrayList<>();
    public final List<Geoset> geosets = new ArrayList<>();
    public final List<GeosetAnimation> geosetAnimations = new ArrayList<>();
    public final List<Bone> bones = new ArrayList<>();
    public final List<Light> lights = new ArrayList<>();
    public final List<Helper> helpers = new ArrayList<>();
    public final List<Attachment> attachments = new ArrayList<>();
    public List<float[]> pivotPoints = new ArrayList<>();
    public final List<ParticleEmitterObject> particleEmitters = new ArrayList<>();
    public final List<ParticleEmitter2Object> particleEmitters2 = new ArrayList<>();
    public final List<RibbonEmitterObject> ribbonEmitters = new ArrayList<>();
    public final List<Camera> cameras = new ArrayList<>();
    public final List<EventObjectEmitterObject> eventObjects = new ArrayList<>();
    public final List<CollisionShape> collisionShapes = new ArrayList<>();
    public boolean hasLayerAnims = false;
    public boolean hasGeosetAnims = false;
    public final List<Batch> batches = new ArrayList<>();
    public final List<GenericObject> genericObjects = new ArrayList<>();
    public final List<GenericObject> sortedGenericObjects = new ArrayList<>();
    public final List<Integer> hierarchy = new ArrayList<>();
    public final List<GenericGroup> opaqueGroups = new ArrayList<>();
    public final List<GenericGroup> translucentGroups = new ArrayList<>();
    public final List<GenericGroup> simpleGroups = new ArrayList<>();
    public int arrayBuffer;
    public int elementBuffer;

    public MdxModel(final MdxHandler handler, final ModelViewer viewer, final String extension,
                    final PathSolver pathSolver, final String fetchUrl) {
        super(handler, viewer, extension, pathSolver, fetchUrl);
    }

    @Override
    public ModelInstance createInstance(final int type) {
        if (type == 1) {
            return new MdxSimpleInstance(this);
        } else {
            return new MdxComplexInstance(this);
        }
    }

    public void load(final Object bufferOrParser) throws IOException {
        MdlxModel parser;

        if (bufferOrParser instanceof MdlxModel) {
            parser = (MdlxModel) bufferOrParser;
        } else {
            System.err.println("Wasting memory with conversion from InputStream to buffer in MdxModel");
            parser = new MdlxModel(ByteBuffer.wrap(IOUtils.toByteArray((InputStream) bufferOrParser)));
        }

        final ModelViewer viewer = this.viewer;
        final PathSolver pathSolver = this.pathSolver;
        final SolverParams solverParams = this.solverParams;
        final boolean reforged = parser.getVersion() > 800;
        final String texturesExt = reforged ? ".dds" : ".blp";

        this.reforged = reforged;
        this.name = parser.getName();
        this.blendTime = parser.getBlendTime();

        // Initialize the bounds.
        final MdlxExtent extent = parser.getExtent();
        final float[] min = extent.getMin();
        final float[] max = extent.getMax();
        for (int i = 0; i < 3; i++) {
            if (min[i] > max[i]) {
                min[i] = max[i] = 0;
            }
        }
        this.bounds.fromExtents(min, max, extent.getBoundsRadius());

        // Sequences
        for (final MdlxSequence sequence : parser.getSequences()) {
            this.sequences.add(new Sequence(sequence));
        }

        // Global sequences
        this.globalSequences.addAll(parser.getGlobalSequences());

        // Texture animations
        for (final MdlxTextureAnimation textureAnimation : parser.getTextureAnimations()) {
            this.textureAnimations.add(new TextureAnimation(this, textureAnimation));
        }

        // Materials
        int layerId = 0;
        for (final MdlxMaterial material : parser.getMaterials()) {
            final List<Layer> layers = new ArrayList<>();

            for (final MdlxLayer layer : material.getLayers()) {
                final Layer vLayer = new Layer(this, layer, layerId++, material.getPriorityPlane());

                layers.add(vLayer);

                this.layers.add(vLayer);
            }

            this.materials.add(new Material(this, material.shader, layers));

            if (!"".equals(material.shader)) {
                this.hd = true;
            }
        }

        if (reforged) {
            solverParams.reforged = true;
        }

        if (this.hd) {
            solverParams.hd = true;
        }

        final GL20 gl = viewer.gl;

        // Textures.
        for (final MdlxTexture texture : parser.getTextures()) {
            String path = texture.getPath();
            final int replaceableId = texture.getReplaceableId();
            final WrapMode wrapMode = texture.getWrapMode();

            if (replaceableId != 0) {
                // TODO This uses dumb, stupid, terrible, no-good hardcoded replaceable IDs
                // instead of the real system, because currently MdxSimpleInstance is not
                // supporting it correctly.
                final String idString = ((replaceableId == 1) || (replaceableId == 2)) ? ReplaceableIds.getIdString(0)
                        : "";
                path = "ReplaceableTextures\\" + ReplaceableIds.getPathString(replaceableId) + idString + ".blp";
            }

            if (reforged && !path.endsWith(".dds")) {
                path = path.substring(0, path.length() - 4) + ".dds";
            } else if ("".equals(path)) {
                path = "Textures\\white.blp";
            }

            final Texture viewerTexture = (Texture) viewer.load(path, pathSolver, solverParams);

            // When the texture will load, it will apply its wrap modes.
            if (wrapMode.isWrapWidth()) {
                viewerTexture.setWrapS(true);
            }

            if (wrapMode.isWrapHeight()) {
                viewerTexture.setWrapT(true);
            }

            this.replaceables.add(replaceableId);
            this.textures.add(viewerTexture);
        }

        // Geoset animations
        for (final MdlxGeosetAnimation geosetAnimation : parser.getGeosetAnimations()) {
            this.geosetAnimations.add(new GeosetAnimation(this, geosetAnimation));
        }

        // Geosets
        SetupGeosets.setupGeosets(this, parser.getGeosets(), parser.getBones().size() >= 256);

        this.pivotPoints = parser.getPivotPoints();

        // Tracks the IDs of all generic objects
        int objectId = 0;

        // Bones
        for (final MdlxBone bone : parser.getBones()) {
            this.bones.add(new Bone(this, bone, objectId++));
        }

        // Lights
        for (final MdlxLight light : parser.getLights()) {
            this.lights.add(new Light(this, light, objectId++));
        }

        // Helpers
        for (final MdlxHelper helper : parser.getHelpers()) {
            this.helpers.add(new Helper(this, helper, objectId++));
        }

        // Attachments
        for (final MdlxAttachment attachment : parser.getAttachments()) {
            this.attachments.add(new Attachment(this, attachment, objectId++));
        }

        // Particle Emitters
        for (final MdlxParticleEmitter particleEmitter : parser.getParticleEmitters()) {
            this.particleEmitters.add(new ParticleEmitterObject(this, particleEmitter, objectId++));
        }

        // Particle Emitters 2
        for (final MdlxParticleEmitter2 particleEmitter2 : parser.getParticleEmitters2()) {
            this.particleEmitters2.add(new ParticleEmitter2Object(this, particleEmitter2, objectId++));
        }

        // Ribbon emitters
        for (final MdlxRibbonEmitter ribbonEmitter : parser.getRibbonEmitters()) {
            this.ribbonEmitters.add(new RibbonEmitterObject(this, ribbonEmitter, objectId++));
        }

        // Camera
        for (final MdlxCamera camera : parser.getCameras()) {
            this.cameras.add(new Camera(this, camera));
        }

        // Event objects
        for (final MdlxEventObject eventObject : parser.getEventObjects()) {
            this.eventObjects.add(new EventObjectEmitterObject(this, eventObject, objectId++));
        }

        // Collision shapes
        for (final MdlxCollisionShape collisionShape : parser.getCollisionShapes()) {
            this.collisionShapes.add(new CollisionShape(this, collisionShape, objectId++));
        }

        // One array for all generic objects.
        this.genericObjects.addAll(this.bones);
        this.genericObjects.addAll(this.lights);
        this.genericObjects.addAll(this.helpers);
        this.genericObjects.addAll(this.attachments);
        this.genericObjects.addAll(this.particleEmitters);
        this.genericObjects.addAll(this.particleEmitters2);
        this.genericObjects.addAll(this.ribbonEmitters);
        this.genericObjects.addAll(this.eventObjects);
        this.genericObjects.addAll(this.collisionShapes);

        // Render groups.
        SetupGroups.setupGroups(this);

        // SimpleInstance render group.
        SetupSimpleGroups.setupSimpleGroups(this);

        // Creates the sorted indices array of the generic objects
        try {
            this.setupHierarchy(-1);
        } catch (final StackOverflowError e) {
            System.out.println("bah");
        }

        // Keep a sorted array.
        for (int i = 0, l = this.genericObjects.size(); i < l; i++) {
            this.sortedGenericObjects.add(this.genericObjects.get(this.hierarchy.get(i)));
        }

    }

    private void setupHierarchy(final int parent) {
        for (int i = 0, l = this.genericObjects.size(); i < l; i++) {
            final GenericObject object = this.genericObjects.get(i);

            if (object.parentId == parent) {
                this.hierarchy.add(i);

                this.setupHierarchy(object.objectId);
            }
        }
    }

    @Override
    protected void lateLoad() {
    }

    @Override
    protected void load(final InputStream src, final Object options) {
        try {
            this.load(src);
        } catch (final IOException e) {
            throw new RuntimeException(e);
        }
    }

    @Override
    protected void error(final Exception e) {
        e.printStackTrace();
    }

    // TODO typing
    public List<Long> getGlobalSequences() {
        return this.globalSequences;
    }

    public List<Sequence> getSequences() {
        return this.sequences;
    }

    public List<float[]> getPivotPoints() {
        return this.pivotPoints;
    }

    public List<GeosetAnimation> getGeosetAnimations() {
        return this.geosetAnimations;
    }

    public List<Texture> getTextures() {
        return this.textures;
    }

    public List<Material> getMaterials() {
        return this.materials;
    }

    public List<TextureAnimation> getTextureAnimations() {
        return this.textureAnimations;
    }

    public List<Geoset> getGeosets() {
        return this.geosets;
    }

    public List<Camera> getCameras() {
        return this.cameras;
    }

    public List<EventObjectEmitterObject> getEventObjects() {
        return this.eventObjects;
    }

    public List<Bone> getBones() {
        return this.bones;
    }

    private static final class SolverParams {
        public boolean reforged;
        public boolean hd;

    }

}
