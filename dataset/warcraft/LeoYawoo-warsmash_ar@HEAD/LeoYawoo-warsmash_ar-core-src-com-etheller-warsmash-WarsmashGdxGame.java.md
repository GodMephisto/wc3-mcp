---
type: vendor-doc
source_repo: "https://github.com/LeoYawoo/warsmash_ar"
source_path: "core/src/com/etheller/warsmash/WarsmashGdxGame.java"
ref: HEAD
---

package com.etheller.warsmash;

import com.badlogic.gdx.ApplicationAdapter;
import com.badlogic.gdx.Gdx;
import com.badlogic.gdx.audio.Music;
import com.badlogic.gdx.graphics.GL20;
import com.badlogic.gdx.math.Quaternion;
import com.badlogic.gdx.math.Rectangle;
import com.badlogic.gdx.math.Vector3;
import com.etheller.warsmash.datasources.DataSource;
import com.etheller.warsmash.units.DataTable;
import com.etheller.warsmash.util.DataSourceFileHandle;
import com.etheller.warsmash.viewer5.*;
import com.etheller.warsmash.viewer5.handlers.mdx.MdxComplexInstance;
import com.etheller.warsmash.viewer5.handlers.mdx.MdxHandler;
import com.etheller.warsmash.viewer5.handlers.mdx.MdxModel;
import com.etheller.warsmash.viewer5.handlers.mdx.MdxViewer;

import java.nio.ByteBuffer;
import java.nio.ByteOrder;
import java.nio.IntBuffer;

public class WarsmashGdxGame extends ApplicationAdapter implements CanvasProvider {
    private static final boolean SPIN = false;
    private static final boolean ADVANCE_ANIMS = true;
    public static int VAO;
    private final Rectangle tempRect = new Rectangle();
    private final DataTable warsmashIni;
    private final float[] cameraPositionTemp = new float[3];
    private final float[] cameraTargetTemp = new float[3];
    private ModelViewer viewer;
    private CameraManager cameraManager;
    private int frame = 0;
    private MdxComplexInstance mainInstance;
    private MdxModel mainModel;
    private com.etheller.warsmash.viewer5.handlers.mdx.Camera modelCamera;
    private boolean firstFrame = true;

    public WarsmashGdxGame(final DataTable warsmashIni) {
        this.warsmashIni = warsmashIni;
    }

    @Override
    public void create() {

        final ByteBuffer tempByteBuffer = ByteBuffer.allocateDirect(4);
        tempByteBuffer.order(ByteOrder.LITTLE_ENDIAN);
        final IntBuffer temp = tempByteBuffer.asIntBuffer();
//
        Gdx.gl30.glGenVertexArrays(1, temp);
        VAO = temp.get(0);

        Gdx.gl30.glBindVertexArray(VAO);

        final String renderer = Gdx.gl.glGetString(GL20.GL_RENDERER);
        System.err.println("Renderer: " + renderer);

        DataSource codebase = WarsmashGdxMapScreen.parseDataSources(this.warsmashIni);
        this.viewer = new MdxViewer(codebase, this, new Vector3(0.3f, 0.3f, -0.25f));

        this.viewer.addHandler(new MdxHandler());
        this.viewer.enableAudio();

        final Scene scene = this.viewer.addSimpleScene();
        scene.enableAudio();

        this.cameraManager = new CameraManager();
        this.cameraManager.setupCamera(scene);


        this.mainModel = (MdxModel) this.viewer.load("Doodads\\Cinematic\\ArthasIllidanFight\\ArthasIllidanFight.mdx",

                (src, solverParams) -> new SolvedPath(src, src.substring(src.lastIndexOf('.')), true), null);


        this.mainInstance = (MdxComplexInstance) this.mainModel.addInstance(0);

        this.mainInstance.setScene(scene);

        final int animIndex = 1;
        this.modelCamera = this.mainModel.cameras.get(animIndex);
        this.mainInstance.setSequence(animIndex);


        System.out.println("Loaded");
        Gdx.gl30.glClearColor(0.5f, 0.5f, 0.5f, 1); // TODO remove white background

    }


    @Override
    public void render() {
        Gdx.gl30.glBindVertexArray(VAO);
        if (SPIN) {
            this.cameraManager.horizontalAngle += 0.0001;
            if (this.cameraManager.horizontalAngle > (2 * Math.PI)) {
                this.cameraManager.horizontalAngle = 0;
            }
        }
        this.modelCamera = this.mainModel.cameras.get(this.mainInstance.sequence);
        this.cameraManager.updateCamera();
        this.viewer.updateAndRender();
        this.frame++;
        if ((this.frame % 1000) == 0) {
            System.out.println(Gdx.graphics.getFramesPerSecond());
        }

        if (ADVANCE_ANIMS && this.mainInstance.sequenceEnded) {
            final int sequence = (this.mainInstance.sequence + 1) % this.mainModel.getSequences().size();
            this.mainInstance.setSequence(sequence);
            this.mainInstance.frame += (int) (Gdx.graphics.getRawDeltaTime() * 1000);
        }
        if (this.firstFrame) {
            final Music music = Gdx.audio.newMusic(new DataSourceFileHandle(this.viewer.dataSource,
                    "Sound\\Ambient\\DoodadEffects\\FinalCinematic.mp3"));
            music.setVolume(0.2f);
            music.setLooping(true);
            music.play();
            this.firstFrame = false;
        }
    }

    @Override
    public void dispose() {
        super.dispose();
    }

    @Override
    public float getWidth() {
        return Gdx.graphics.getWidth();
    }

    @Override
    public float getHeight() {
        return Gdx.graphics.getHeight();
    }

    @Override
    public void resize(final int width, final int height) {
        this.tempRect.width = width;
        this.tempRect.height = height;
        this.cameraManager.camera.viewport(this.tempRect);
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
            if (WarsmashGdxGame.this.modelCamera != null) {
                WarsmashGdxGame.this.modelCamera.getPositionTranslation(WarsmashGdxGame.this.cameraPositionTemp,
                        WarsmashGdxGame.this.mainInstance.sequence, WarsmashGdxGame.this.mainInstance.frame,
                        WarsmashGdxGame.this.mainInstance.counter);
                WarsmashGdxGame.this.modelCamera.getTargetTranslation(WarsmashGdxGame.this.cameraTargetTemp,
                        WarsmashGdxGame.this.mainInstance.sequence, WarsmashGdxGame.this.mainInstance.frame,
                        WarsmashGdxGame.this.mainInstance.counter);

                this.position.set(WarsmashGdxGame.this.modelCamera.position);
                this.target.set(WarsmashGdxGame.this.modelCamera.targetPosition);
                this.position.add(WarsmashGdxGame.this.cameraPositionTemp[0],
                        WarsmashGdxGame.this.cameraPositionTemp[1], WarsmashGdxGame.this.cameraPositionTemp[2]);
                this.target.add(WarsmashGdxGame.this.cameraTargetTemp[0], WarsmashGdxGame.this.cameraTargetTemp[1],
                        WarsmashGdxGame.this.cameraTargetTemp[2]);
                this.camera.perspective(WarsmashGdxGame.this.modelCamera.fieldOfView * 0.75f,
                        Gdx.graphics.getWidth() / (float) Gdx.graphics.getHeight(),
                        WarsmashGdxGame.this.modelCamera.nearClippingPlane,
                        WarsmashGdxGame.this.modelCamera.farClippingPlane);
            } else {
                this.camera.perspective(70, this.camera.getAspect(), 100, 5000);
            }

            this.camera.moveToAndFace(this.position, this.target, this.worldUp);
        }
    }
}
