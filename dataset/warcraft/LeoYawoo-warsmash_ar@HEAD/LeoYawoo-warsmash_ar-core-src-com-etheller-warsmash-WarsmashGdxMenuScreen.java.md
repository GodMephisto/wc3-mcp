---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/WarsmashGdxMenuScreen.java"
ref: HEAD
---

package com.etheller.warsmash;

import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.InputProcessor;
import com.badlogic.gdx.Screen;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.graphics.GL30;
import com.badlogic.gdx.graphics.OrthographicCamera;
import com.badlogic.gdx.graphics.g2d.GlyphLayout;
import com.badlogic.gdx.graphics.g2d.SpriteBatch;
import com.badlogic.gdx.math.Matrix4;
import com.badlogic.gdx.math.Quaternion;
import com.badlogic.gdx.math.Rectangle;
import com.badlogic.gdx.math.Vector3;
import com.badlogic.gdx.utils.viewport.ExtendViewport;
import com.badlogic.gdx.utils.viewport.FitViewport;
import com.badlogic.gdx.utils.viewport.Viewport;
import com.etheller.warsmash.datasources.DataSource;
import com.etheller.warsmash.networking.uberserver.GamingNetworkConnectionImpl;
import com.etheller.warsmash.units.DataTable;
import com.etheller.warsmash.units.Element;
import com.etheller.warsmash.util.StringBundle;
import com.etheller.warsmash.viewer5.Camera;
import com.etheller.warsmash.viewer5.*;
import com.etheller.warsmash.viewer5.handlers.ModelHandler;
import com.etheller.warsmash.viewer5.handlers.mdx.*;
import com.etheller.warsmash.viewer5.handlers.w3x.AnimationTokens.PrimaryTag;
import com.etheller.warsmash.viewer5.handlers.w3x.AnimationTokens.SecondaryTag;
import com.etheller.warsmash.viewer5.handlers.w3x.SequenceUtils;
import com.etheller.warsmash.viewer5.handlers.w3x.War3MapViewer;
import com.etheller.warsmash.viewer5.handlers.w3x.ui.MenuUI;

import java.io.IOException;
import java.io.InputStream;
import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.IntBuffer;
import java.util.EnumSet;

public class WarsmashGdxMenuScreen implements InputProcessor, Screen, SingleModelScreen {
    private static final String MAPS_DOWNLOAD_DEFAULT = "Maps/Download";
    private static final boolean ENABLE_AUDIO = true;
    private final Rectangle tempRect = new Rectangle();
    private final DataTable warsmashIni;
    private final WarsmashGdxMultiScreenGame game;
    private final float[] cameraPositionTemp = new float[3];
    private final float[] cameraTargetTemp = new float[3];
    private MdxViewer viewer;
    private CameraManager cameraManager;
    // libGDX stuff
    private OrthographicCamera uiCamera;
    private SpriteBatch batch;
    private Viewport uiViewport;
    private GlyphLayout glyphLayout;
    private Scene uiScene;
    private MenuUI menuUI;
    private boolean hasPlayedStandHack = false;
    private boolean loaded = false;
    private EnumSet<SecondaryTag> tags = SequenceUtils.EMPTY;
    private MdxComplexInstance mainInstance;
    private MdxModel mainModel;
    private com.etheller.warsmash.viewer5.handlers.mdx.Camera modelCamera;
    private Scene scene;

    public WarsmashGdxMenuScreen(final DataTable warsmashIni, final WarsmashGdxMultiScreenGame game) {
        this.warsmashIni = warsmashIni;
        this.game = game;
    }


    @Override
    public void show() {
        if (!this.loaded) {
            this.loaded = true;
            final ByteBuffer tempByteBuffer = ByteBuffer.allocateDirect(4);
            tempByteBuffer.order(ByteOrder.LITTLE_ENDIAN);
            final IntBuffer temp = tempByteBuffer.asIntBuffer();
            Gdx.gl30.glGenVertexArrays(1, temp);
            WarsmashGdxGame.VAO = temp.get(0);

            Gdx.gl30.glBindVertexArray(WarsmashGdxGame.VAO);

            final String renderer = Gdx.gl.glGetString(GL20.GL_RENDERER);
            System.err.println("Renderer: " + renderer);

            DataSource codebase = WarsmashGdxMapScreen.parseDataSources(this.warsmashIni);
            this.viewer = new MdxViewer(codebase, this.game, Vector3.Zero);

            this.viewer.addHandler(new MdxHandler());
            this.viewer.enableAudio();

            this.scene = this.viewer.addSimpleScene();
            this.scene.enableAudio();

            this.uiScene = this.viewer.addSimpleScene();
            this.uiScene.alpha = true;
            if (ENABLE_AUDIO) {
                this.uiScene.enableAudio();
            }
            final int width = Gdx.graphics.getWidth();
            final int height = Gdx.graphics.getHeight();

            this.glyphLayout = new GlyphLayout();

            // Constructs a new OrthographicCamera, using the given viewport width and
            // height
            // Height is multiplied by aspect ratio.
            this.uiCamera = new OrthographicCamera();
            int aspect3By4Width;
            int aspect3By4Height;
            if (width < ((height * 4) / 3)) {
                aspect3By4Width = width;
                aspect3By4Height = (width * 3) / 4;
            } else {
                aspect3By4Width = (height * 4) / 3;
                aspect3By4Height = height;
            }
            this.uiViewport = new FitViewport(aspect3By4Width, aspect3By4Height, this.uiCamera);
            this.uiViewport.update(width, height);

            this.uiCamera.position.set(getMinWorldWidth() / 2, getMinWorldHeight() / 2, 0);
            this.uiCamera.update();

            this.batch = new SpriteBatch();


            this.cameraManager = new CameraManager();
            this.cameraManager.setupCamera(this.scene);

            System.out.println("Loaded");
            Gdx.gl30.glClearColor(0.0f, 0.0f, 0.0f, 1);
            final DataTable musicSLK = new DataTable(StringBundle.EMPTY);
            final String musicSLKPath = "UI\\SoundInfo\\Music.SLK";
            if (this.viewer.dataSource.has(musicSLKPath)) {
                try (InputStream miscDataTxtStream = this.viewer.dataSource.getResourceAsStream(musicSLKPath)) {
                    musicSLK.readSLK(miscDataTxtStream);
                } catch (final IOException e) {
                    e.printStackTrace();
                }
            }

            String server;
            String mapDownloadDir;
            final Element gamingNetworkSettings = this.warsmashIni.get("GamingNetwork");
            if (gamingNetworkSettings != null) {
                server = gamingNetworkSettings.getField("Server");
                mapDownloadDir = gamingNetworkSettings.getField("MapDownloadDir");
                if (mapDownloadDir.isEmpty()) {
                    mapDownloadDir = MAPS_DOWNLOAD_DEFAULT;
                }
            } else {
                server = "localhost";
                mapDownloadDir = MAPS_DOWNLOAD_DEFAULT;
            }
            this.menuUI = new MenuUI(this.viewer.dataSource, this.uiViewport, this.uiScene, this.viewer, this.game, this, this.warsmashIni, rootFrame -> {
                //						WarsmashGdxMapGame.this.viewer.setGameUI(rootFrame);

                singleModelScene(WarsmashGdxMenuScreen.this.scene, rootFrame.getSkinField("GlueSpriteLayerBackground"), "Stand");
                if (!WarsmashGdxMenuScreen.this.mainModel.cameras.isEmpty()) {
                    WarsmashGdxMenuScreen.this.modelCamera = WarsmashGdxMenuScreen.this.mainModel.cameras.get(0);
                } else {
                    WarsmashGdxMenuScreen.this.mainInstance.detach();
                    WarsmashGdxMenuScreen.this.mainInstance.setLocation(0, 0, 0);
                    WarsmashGdxMenuScreen.this.mainInstance.setScene(WarsmashGdxMenuScreen.this.uiScene);
                }
            }, new GamingNetworkConnectionImpl(server), mapDownloadDir);

            final ModelInstance libgdxContentInstance = new LibGDXContentLayerModel(null, this.viewer, "", PathSolver.DEFAULT, "").addInstance();
            libgdxContentInstance.setLocation(0f, 0f, 0.5f);
            libgdxContentInstance.setScene(this.uiScene);
            this.menuUI.main();

            updateUIScene();

            resize(width, height);
        }

        Gdx.input.setInputProcessor(this);
        this.menuUI.show();

    }

    private float getMinWorldWidth() {
        if (this.uiViewport instanceof ExtendViewport) {
            return ((ExtendViewport) this.uiViewport).getMinWorldWidth();
        }
        return this.uiViewport.getWorldWidth();
    }

    private float getMinWorldHeight() {
        if (this.uiViewport instanceof ExtendViewport) {
            return ((ExtendViewport) this.uiViewport).getMinWorldHeight();
        }
        return this.uiViewport.getWorldHeight();
    }

    private void updateUIScene() {
        this.tempRect.x = this.uiViewport.getScreenX();
        this.tempRect.y = this.uiViewport.getScreenY();
        this.tempRect.width = this.uiViewport.getScreenWidth();
        this.tempRect.height = this.uiViewport.getScreenHeight();
        this.uiScene.camera.viewport(this.tempRect);
        final float worldWidth = this.uiViewport.getWorldWidth();
        final float worldHeight = this.uiViewport.getWorldHeight();
        final float xScale = worldWidth / getMinWorldWidth();
        final float yScale = worldHeight / getMinWorldHeight();
        final float uiSceneWidth = 0.8f * xScale;
        final float uiSceneHeight = 0.6f * yScale;
        final float uiSceneX = (0.8f - uiSceneWidth) / 2;
        final float uiSceneY = (0.6f - uiSceneHeight) / 2;
        this.uiScene.camera.ortho(uiSceneX, uiSceneWidth + uiSceneX, uiSceneY, uiSceneHeight + uiSceneY, -1024f, 1024);
    }


    private void singleModelScene(final Scene scene, final String path, final String animName) {
        final MdxModel model2 = War3MapViewer.loadModelMdx(this.viewer.dataSource, viewer, path, (src, solverParams) -> new SolvedPath(src, src.substring(src.lastIndexOf('.')), true), null);

        final MdxComplexInstance instance3 = (MdxComplexInstance) model2.addInstance(0);

        instance3.setScene(scene);

        int animIndex = 0;
        for (final Sequence s : model2.getSequences()) {
            if (s.getName().toLowerCase().startsWith(animName)) {
                animIndex = model2.getSequences().indexOf(s);
                break;
            }
        }
        instance3.setSequence(animIndex);

        instance3.setSequenceLoopMode(SequenceLoopMode.NEVER_LOOP);
        this.mainInstance = instance3;
        this.mainModel = model2;
    }

    @Override
    public void setModel(final String path) {
        if (this.mainInstance != null) {
            this.mainInstance.detach();
        }
        if (path == null) {
            this.modelCamera = null;
            this.mainInstance = null;
            this.mainModel = null;
        } else {
            singleModelScene(this.scene, path, "birth");
            if (!WarsmashGdxMenuScreen.this.mainModel.cameras.isEmpty()) {
                WarsmashGdxMenuScreen.this.modelCamera = WarsmashGdxMenuScreen.this.mainModel.cameras.get(0);
            } else {
                WarsmashGdxMenuScreen.this.mainInstance.detach();
                WarsmashGdxMenuScreen.this.mainInstance.setLocation(0, 0, 1024);
                WarsmashGdxMenuScreen.this.mainInstance.setScene(WarsmashGdxMenuScreen.this.uiScene);
            }
            // this hack is because we only have the queued animation system in RenderWidget
            // which is stupid and back and needs to get moved to the model instance
            // itself... our model instance class is a
            // hacky replica of a model viewer tool with a bunch of irrelevant loop type
            // settings instead of what it should be
            this.hasPlayedStandHack = false;
        }

    }

    @Override
    public void alternateModelToBattlenet() {
        if (this.mainInstance != null) {
            SequenceUtils.randomSequence(this.mainInstance, PrimaryTag.STAND, SequenceUtils.ALTERNATE, true);
            this.tags = SequenceUtils.ALTERNATE;
            this.hasPlayedStandHack = false;
        }
    }

    @Override
    public void unAlternateModelBackToNormal() {
        if (this.mainInstance != null) {
            SequenceUtils.randomSequence(this.mainInstance, PrimaryTag.MORPH, SequenceUtils.ALTERNATE, true);
            this.tags = SequenceUtils.EMPTY;
            this.hasPlayedStandHack = false;
        }
    }


    @Override
    public void render(final float delta) {

        Gdx.gl30.glEnable(GL30.GL_SCISSOR_TEST);
        Gdx.gl30.glBindVertexArray(WarsmashGdxGame.VAO);
        this.cameraManager.updateCamera();
        this.menuUI.update();
        if ((this.mainInstance != null) && this.mainInstance.sequenceEnded && (((this.mainModel.getSequences().get(this.mainInstance.sequence).getFlags() & 0x1) == 0) || !this.hasPlayedStandHack)) {
            SequenceUtils.randomSequence(this.mainInstance, PrimaryTag.STAND, this.tags, true);
            this.hasPlayedStandHack = true;
        }
        this.viewer.updateAndRender();

        Gdx.gl30.glDisable(GL30.GL_SCISSOR_TEST);

        Gdx.gl30.glDisable(GL30.GL_CULL_FACE);

        this.viewer.webGL.useShaderProgram(null);

        Gdx.gl30.glActiveTexture(GL30.GL_TEXTURE0);
    }

    @Override
    public void dispose() {
        this.menuUI.dispose();
    }

    @Override
    public void resize(final int width, final int height) {
        this.tempRect.width = width;
        this.tempRect.height = height;
        final float fourThirdsHeight = (this.tempRect.height * 4) / 3;
        if (fourThirdsHeight < this.tempRect.width) {
            final float dx = this.tempRect.width - fourThirdsHeight;
            this.tempRect.width = fourThirdsHeight;
            this.tempRect.x = dx / 2;
        } else {
            final float threeFourthsWidth = (this.tempRect.width * 3) / 4;
            if (threeFourthsWidth < this.tempRect.height) {
                final float dy = this.tempRect.height - threeFourthsWidth;
                this.tempRect.height = threeFourthsWidth;
                this.tempRect.y = dy;
            }
        }
        this.cameraManager.camera.viewport(this.tempRect);

//		super.resize(width, height);

        this.uiViewport.update(width, height);
        this.uiCamera.position.set(getMinWorldWidth() / 2, getMinWorldHeight() / 2, 0);

        this.menuUI.resize();
        updateUIScene();

    }

    @Override
    public boolean keyDown(final int keycode) {
        return this.menuUI.keyDown(keycode);
    }

    @Override
    public boolean keyUp(final int keycode) {
        return this.menuUI.keyUp(keycode);
    }

    @Override
    public boolean keyTyped(final char character) {
        return this.menuUI.keyTyped(character);
    }

    @Override
    public boolean touchDown(final int screenX, final int screenY, final int pointer, final int button) {
        this.menuUI.touchDown(screenX, screenY, button);
        return false;
    }

    @Override
    public boolean touchUp(final int screenX, final int screenY, final int pointer, final int button) {
        this.menuUI.touchUp(screenX, screenY, button);
        return false;
    }

    @Override
    public boolean touchDragged(final int screenX, final int screenY, final int pointer) {
        this.menuUI.touchDragged(screenX, screenY);
        return false;
    }

    @Override
    public boolean mouseMoved(final int screenX, final int screenY) {
        this.menuUI.mouseMoved(screenX, screenY);
        return false;
    }

    @Override
    public boolean scrolled(final int amount) {
        // TODO Auto-generated method stub
        return false;
    }

    private void renderLibGDXContent() {

        Gdx.gl30.glClear(GL30.GL_DEPTH_BUFFER_BIT);
        Gdx.gl30.glDisable(GL30.GL_SCISSOR_TEST);

        Gdx.gl30.glDisable(GL30.GL_CULL_FACE);

        this.viewer.webGL.useShaderProgram(null);

        Gdx.gl30.glActiveTexture(GL30.GL_TEXTURE0);

        this.uiViewport.apply();
        this.batch.setProjectionMatrix(this.uiCamera.combined);
        this.batch.begin();
        this.menuUI.render(this.batch, this.glyphLayout);
        this.batch.end();

        Gdx.gl30.glEnable(GL30.GL_SCISSOR_TEST);
        Gdx.gl30.glBindVertexArray(WarsmashGdxGame.VAO);
    }

    @Override
    public void hide() {
        this.menuUI.hide();
    }

    @Override
    public void pause() {
    }

    @Override
    public void resume() {
    }

    public void startMap(final String finalFileToLoad) {
        this.menuUI.startMap(finalFileToLoad);
    }

    public void onReturnFromGame() {
        this.menuUI.onReturnFromGame();
    }

    class CameraManager {
        private Camera camera;
        private float horizontalAngle;
        private float verticalAngle;
        private float distance;
        private Vector3 position;
        private Vector3 target;
        private Vector3 worldUp;
        private Quaternion quatHeap;
        private Quaternion quatHeap2;

        // An orbit camera setup example.
        // Left mouse button controls the orbit itself.
        // The right mouse button allows to move the camera and the point it's looking
        // at on the XY plane.
        // Scrolling zooms in and out.
        private void setupCamera(final Scene scene) {
            this.camera = scene.camera;
            this.horizontalAngle = (float) (Math.PI / 2);
            this.verticalAngle = (float) (Math.PI / 4);
            this.distance = 500;
            this.position = new Vector3();
            this.target = new Vector3(0, 0, 50);
            this.worldUp = new Vector3(0, 0, 1);
            this.quatHeap = new Quaternion();
            this.quatHeap2 = new Quaternion();

            updateCamera();

//		cameraUpdate();
        }

        private void updateCamera() {
            // Limit the vertical angle so it doesn't flip.
            // Since the camera uses a quaternion, flips don't matter to it, but this feels
            // better.
            this.verticalAngle = (float) Math.min(Math.max(0.01, this.verticalAngle), Math.PI - 0.01);

            this.quatHeap.idt();
            this.quatHeap.setFromAxisRad(0, 0, 1, this.horizontalAngle);
            this.quatHeap2.idt();
            this.quatHeap2.setFromAxisRad(1, 0, 0, this.verticalAngle);
            this.quatHeap.mul(this.quatHeap2);

            this.position.set(0, 0, 1);
            this.quatHeap.transform(this.position);
            this.position.scl(this.distance);
            this.position = this.position.add(this.target);
            if (WarsmashGdxMenuScreen.this.modelCamera != null) {
                WarsmashGdxMenuScreen.this.modelCamera.getPositionTranslation(WarsmashGdxMenuScreen.this.cameraPositionTemp, WarsmashGdxMenuScreen.this.mainInstance.sequence, WarsmashGdxMenuScreen.this.mainInstance.frame, WarsmashGdxMenuScreen.this.mainInstance.counter);
                WarsmashGdxMenuScreen.this.modelCamera.getTargetTranslation(WarsmashGdxMenuScreen.this.cameraTargetTemp, WarsmashGdxMenuScreen.this.mainInstance.sequence, WarsmashGdxMenuScreen.this.mainInstance.frame, WarsmashGdxMenuScreen.this.mainInstance.counter);

                this.position.set(WarsmashGdxMenuScreen.this.modelCamera.position);
                this.target.set(WarsmashGdxMenuScreen.this.modelCamera.targetPosition);
                this.position.add(WarsmashGdxMenuScreen.this.cameraPositionTemp[0], WarsmashGdxMenuScreen.this.cameraPositionTemp[1], WarsmashGdxMenuScreen.this.cameraPositionTemp[2]);
                this.target.add(WarsmashGdxMenuScreen.this.cameraTargetTemp[0], WarsmashGdxMenuScreen.this.cameraTargetTemp[1], WarsmashGdxMenuScreen.this.cameraTargetTemp[2]);
                this.camera.perspective(WarsmashGdxMenuScreen.this.modelCamera.fieldOfView * 0.6f, this.camera.rect.width / this.camera.rect.height, WarsmashGdxMenuScreen.this.modelCamera.nearClippingPlane, WarsmashGdxMenuScreen.this.modelCamera.farClippingPlane);
            } else {
                this.camera.perspective(70, this.camera.getAspect(), 100, 5000);
            }

            this.camera.moveToAndFace(this.position, this.target, this.worldUp);
        }
    }

    private class LibGDXContentLayerModelInstance extends ModelInstance {

        public LibGDXContentLayerModelInstance(final Model model) {
            super(model);
        }

        @Override
        public void updateAnimations(final float dt) {

        }

        @Override
        public void clearEmittedObjects() {

        }

        @Override
        protected void updateLights(final Scene scene2) {

        }

        @Override
        public void renderOpaque(final Matrix4 mvp) {

        }

        @Override
        public void renderTranslucent() {
            renderLibGDXContent();
        }

        @Override
        public void load() {
        }

        @Override
        protected RenderBatch getBatch(final TextureMapper textureMapper2) {
            throw new UnsupportedOperationException("NOT API");
        }

        @Override
        public void setReplaceableTexture(final int replaceableTextureId, final String replaceableTextureFile) {

        }

        @Override
        public boolean isBatched() {
            return super.isBatched();
        }

        @Override
        protected void removeLights(final Scene scene2) {
            // TODO Auto-generated method stub

        }

        @Override
        public void setReplaceableTextureHD(final int replaceableTextureId, final String replaceableTextureFile) {
            // TODO Auto-generated method stub

        }

    }

    private class LibGDXContentLayerModel extends Model {

        public LibGDXContentLayerModel(final ModelHandler handler, final ModelViewer viewer, final String extension, final PathSolver pathSolver, final String fetchUrl) {
            super(handler, viewer, extension, pathSolver, fetchUrl);
            this.ok = true;
        }

        @Override
        protected ModelInstance createInstance(final int type) {
            return new LibGDXContentLayerModelInstance(this);
        }

        @Override
        protected void lateLoad() {
        }

        @Override
        protected void load(final InputStream src, final Object options) {
        }

        @Override
        protected void error(final Exception e) {
        }

    }
}
