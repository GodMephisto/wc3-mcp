---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/TopHudWebGL/TopHudWebGL.tsx"
ref: HEAD
---

import { useEffect, useRef } from "react";
import type { Race } from "../../utils/types";
import { RACE_PREFIXES } from "../../state/race";

interface Props {
  race: Race;
  leftOnly?: boolean;
}

type TileIndex = 1 | 2 | 3 | 4;

const TILE_INDICES: readonly TileIndex[] = [1, 2, 3, 4];
const TOP_SLICE_V0 = 0;
const TOP_SLICE_V1 = 0.125;
const TILE2_LEFT_U0 = 0;
const TILE2_LEFT_U1 = 0.33984375;
const TILE2_RIGHT_U0 = 0.79296875;
const TILE2_RIGHT_U1 = 1;

const LEFT_TILE1_WIDTH_VW = 32;
const LEFT_TILE2_WIDTH_VW = 10.875;
const RIGHT_TILE2_WIDTH_VW = 6.875;
const RIGHT_TILE3_WIDTH_VW = 32;
const RIGHT_TILE4_WIDTH_VW = 4;
const SEAM_PX = 1;

function topTilePath(race: Race, index: TileIndex): string {
  const tile = RACE_PREFIXES[race].tile;
  return `./console/${race}/${tile}UITile0${index}.png`;
}

function compileShader(
  gl: WebGLRenderingContext,
  type: number,
  src: string,
): WebGLShader | null {
  const shader = gl.createShader(type);
  if (!shader) return null;
  gl.shaderSource(shader, src);
  gl.compileShader(shader);
  if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS)) {
    console.error(
      "Top HUD shader compile failed:",
      gl.getShaderInfoLog(shader),
    );
    gl.deleteShader(shader);
    return null;
  }
  return shader;
}

function createProgram(gl: WebGLRenderingContext): WebGLProgram | null {
  const vertexSource = `
    attribute vec2 a_position;
    attribute vec2 a_uv;
    varying vec2 v_uv;
    void main() {
      gl_Position = vec4(a_position, 0.0, 1.0);
      v_uv = a_uv;
    }
  `;
  const fragmentSource = `
    precision mediump float;
    varying vec2 v_uv;
    uniform sampler2D u_texture;
    void main() {
      vec4 color = texture2D(u_texture, v_uv);
      if (color.a < 0.5) {
        discard;
      }
      gl_FragColor = color;
    }
  `;

  const vertexShader = compileShader(gl, gl.VERTEX_SHADER, vertexSource);
  const fragmentShader = compileShader(gl, gl.FRAGMENT_SHADER, fragmentSource);
  if (!vertexShader || !fragmentShader) return null;

  const program = gl.createProgram();
  if (!program) return null;

  gl.attachShader(program, vertexShader);
  gl.attachShader(program, fragmentShader);
  gl.linkProgram(program);

  gl.deleteShader(vertexShader);
  gl.deleteShader(fragmentShader);

  if (!gl.getProgramParameter(program, gl.LINK_STATUS)) {
    console.error(
      "Top HUD program link failed:",
      gl.getProgramInfoLog(program),
    );
    gl.deleteProgram(program);
    return null;
  }

  return program;
}

interface GLState {
  gl: WebGLRenderingContext | null;
  program: WebGLProgram | null;
  quadBuffer: WebGLBuffer | null;
  positionLoc: number;
  uvLoc: number;
  textureLoc: WebGLUniformLocation | null;
  destroyed: boolean;
  drawQueued: boolean;
  drawRaf: number;
  resizeObserver: ResizeObserver | null;
  textureCache: Map<string, WebGLTexture>;
  textureLoadCache: Map<string, Promise<WebGLTexture | null>>;
  activeTextures: Record<TileIndex, WebGLTexture> | null;
}

export default function TopHudWebGL(props: Props) {
  const canvasRef = useRef<HTMLCanvasElement>(null);
  const propsRef = useRef(props);
  propsRef.current = props;

  const stateRef = useRef<GLState>({
    gl: null,
    program: null,
    quadBuffer: null,
    positionLoc: -1,
    uvLoc: -1,
    textureLoc: null,
    destroyed: false,
    drawQueued: false,
    drawRaf: 0,
    resizeObserver: null,
    textureCache: new Map(),
    textureLoadCache: new Map(),
    activeTextures: null,
  });

  useEffect(() => {
    const s = stateRef.current;
    const canvas = canvasRef.current!;

    const syncCanvasSize = () => {
      if (!s.gl) return;
      const dpr = window.devicePixelRatio || 1;
      const cssWidth = Math.max(1, canvas.clientWidth || 1);
      const cssHeight = Math.max(1, canvas.clientHeight || 1);
      const width = Math.round(cssWidth * dpr);
      const height = Math.round(cssHeight * dpr);
      if (canvas.width !== width || canvas.height !== height) {
        canvas.width = width;
        canvas.height = height;
      }
      s.gl.viewport(0, 0, width, height);
    };

    const drawQuad = (
      texture: WebGLTexture,
      x: number,
      y: number,
      w: number,
      h: number,
      u0: number,
      v0: number,
      u1: number,
      v1: number,
    ) => {
      if (!s.gl || !s.quadBuffer) return;
      const canvasW = canvas.width;
      const canvasH = canvas.height;
      if (canvasW <= 0 || canvasH <= 0 || w <= 0 || h <= 0) return;
      const x0 = (x / canvasW) * 2 - 1;
      const y0 = 1 - (y / canvasH) * 2;
      const x1 = ((x + w) / canvasW) * 2 - 1;
      const y1 = 1 - ((y + h) / canvasH) * 2;
      const vertices = new Float32Array([
        x0,
        y0,
        u0,
        v0,
        x1,
        y0,
        u1,
        v0,
        x0,
        y1,
        u0,
        v1,
        x1,
        y0,
        u1,
        v0,
        x1,
        y1,
        u1,
        v1,
        x0,
        y1,
        u0,
        v1,
      ]);
      s.gl.bindTexture(s.gl.TEXTURE_2D, texture);
      s.gl.bindBuffer(s.gl.ARRAY_BUFFER, s.quadBuffer);
      s.gl.bufferData(s.gl.ARRAY_BUFFER, vertices, s.gl.STREAM_DRAW);
      s.gl.drawArrays(s.gl.TRIANGLES, 0, 6);
    };

    const draw = () => {
      if (!s.gl || !s.program || !s.quadBuffer || !s.activeTextures) return;
      syncCanvasSize();
      const canvasW = canvas.width;
      const canvasH = canvas.height;
      if (canvasW <= 0 || canvasH <= 0) return;
      s.gl.clear(s.gl.COLOR_BUFFER_BIT);
      s.gl.useProgram(s.program);
      s.gl.bindBuffer(s.gl.ARRAY_BUFFER, s.quadBuffer);
      s.gl.enableVertexAttribArray(s.positionLoc);
      s.gl.vertexAttribPointer(s.positionLoc, 2, s.gl.FLOAT, false, 16, 0);
      s.gl.enableVertexAttribArray(s.uvLoc);
      s.gl.vertexAttribPointer(s.uvLoc, 2, s.gl.FLOAT, false, 16, 8);
      s.gl.activeTexture(s.gl.TEXTURE0);
      s.gl.uniform1i(s.textureLoc, 0);

      const dpr = window.devicePixelRatio || 1;
      const seamPx = SEAM_PX * dpr;
      const vw = canvasW / 100;
      const topHudH = canvasH;
      const p = propsRef.current;

      if (p.leftOnly) {
        const leftSpanVw = LEFT_TILE1_WIDTH_VW + LEFT_TILE2_WIDTH_VW;
        const leftTile1W =
          (LEFT_TILE1_WIDTH_VW / leftSpanVw) * canvasW + seamPx;
        const leftTile2X = leftTile1W - seamPx;
        const leftTile2W = canvasW - leftTile2X;
        drawQuad(
          s.activeTextures[1],
          0,
          0,
          leftTile1W,
          topHudH,
          0,
          TOP_SLICE_V0,
          1,
          TOP_SLICE_V1,
        );
        drawQuad(
          s.activeTextures[2],
          leftTile2X,
          0,
          leftTile2W,
          topHudH,
          TILE2_LEFT_U0,
          TOP_SLICE_V0,
          TILE2_LEFT_U1,
          TOP_SLICE_V1,
        );
        return;
      }

      const leftTile1W = LEFT_TILE1_WIDTH_VW * vw + seamPx;
      const leftTile2W = LEFT_TILE2_WIDTH_VW * vw + seamPx;
      const leftTile2X = leftTile1W - seamPx;
      drawQuad(
        s.activeTextures[1],
        0,
        0,
        leftTile1W,
        topHudH,
        0,
        TOP_SLICE_V0,
        1,
        TOP_SLICE_V1,
      );
      drawQuad(
        s.activeTextures[2],
        leftTile2X,
        0,
        leftTile2W,
        topHudH,
        TILE2_LEFT_U0,
        TOP_SLICE_V0,
        TILE2_LEFT_U1,
        TOP_SLICE_V1,
      );

      const rightTile2W = RIGHT_TILE2_WIDTH_VW * vw + seamPx;
      const rightTile3W = RIGHT_TILE3_WIDTH_VW * vw + seamPx;
      const rightTile4W = RIGHT_TILE4_WIDTH_VW * vw + seamPx;
      const rightTile4X = canvasW - rightTile4W;
      const rightTile3X = rightTile4X - (rightTile3W - seamPx);
      const rightTile2X = rightTile3X - (rightTile2W - seamPx);
      drawQuad(
        s.activeTextures[2],
        rightTile2X,
        0,
        rightTile2W,
        topHudH,
        TILE2_RIGHT_U0,
        TOP_SLICE_V0,
        TILE2_RIGHT_U1,
        TOP_SLICE_V1,
      );
      drawQuad(
        s.activeTextures[3],
        rightTile3X,
        0,
        rightTile3W,
        topHudH,
        0,
        TOP_SLICE_V0,
        1,
        TOP_SLICE_V1,
      );
      drawQuad(
        s.activeTextures[4],
        rightTile4X,
        0,
        rightTile4W,
        topHudH,
        0,
        TOP_SLICE_V0,
        1,
        TOP_SLICE_V1,
      );
    };

    const requestDraw = () => {
      if (s.drawQueued || s.destroyed) return;
      s.drawQueued = true;
      s.drawRaf = window.requestAnimationFrame(() => {
        s.drawQueued = false;
        draw();
      });
    };

    const loadTexture = (path: string): Promise<WebGLTexture | null> => {
      if (!s.gl) return Promise.resolve(null);
      const cached = s.textureCache.get(path);
      if (cached) return Promise.resolve(cached);
      const inflight = s.textureLoadCache.get(path);
      if (inflight) return inflight;

      const promise = new Promise<WebGLTexture | null>((resolve) => {
        const image = new Image();
        image.onload = () => {
          if (!s.gl || s.destroyed) {
            s.textureLoadCache.delete(path);
            resolve(null);
            return;
          }
          const texture = s.gl.createTexture();
          if (!texture) {
            s.textureLoadCache.delete(path);
            resolve(null);
            return;
          }
          s.gl.bindTexture(s.gl.TEXTURE_2D, texture);
          s.gl.texImage2D(
            s.gl.TEXTURE_2D,
            0,
            s.gl.RGBA,
            s.gl.RGBA,
            s.gl.UNSIGNED_BYTE,
            image,
          );
          s.gl.texParameteri(
            s.gl.TEXTURE_2D,
            s.gl.TEXTURE_MIN_FILTER,
            s.gl.LINEAR,
          );
          s.gl.texParameteri(
            s.gl.TEXTURE_2D,
            s.gl.TEXTURE_MAG_FILTER,
            s.gl.LINEAR,
          );
          s.gl.texParameteri(
            s.gl.TEXTURE_2D,
            s.gl.TEXTURE_WRAP_S,
            s.gl.CLAMP_TO_EDGE,
          );
          s.gl.texParameteri(
            s.gl.TEXTURE_2D,
            s.gl.TEXTURE_WRAP_T,
            s.gl.CLAMP_TO_EDGE,
          );
          s.textureCache.set(path, texture);
          s.textureLoadCache.delete(path);
          resolve(texture);
        };
        image.onerror = () => {
          console.error(`Failed to load top HUD texture: ${path}`);
          s.textureLoadCache.delete(path);
          resolve(null);
        };
        image.src = path;
      });
      s.textureLoadCache.set(path, promise);
      return promise;
    };

    const loadRaceTextures = async (race: Race) => {
      if (!s.gl || s.destroyed) return;
      const paths = TILE_INDICES.map((index) => topTilePath(race, index));
      const loaded = await Promise.all(paths.map((path) => loadTexture(path)));
      if (s.destroyed || loaded.some((tex) => tex === null)) return;
      s.activeTextures = {
        1: loaded[0]!,
        2: loaded[1]!,
        3: loaded[2]!,
        4: loaded[3]!,
      };
      requestDraw();
    };

    // Store for use in race/leftOnly effects
    (stateRef.current as any).loadRaceTextures = loadRaceTextures;
    (stateRef.current as any).requestDraw = requestDraw;

    s.gl = canvas.getContext("webgl", {
      alpha: true,
      premultipliedAlpha: false,
      antialias: false,
      depth: false,
      stencil: false,
    });
    if (!s.gl) {
      console.error("WebGL is unavailable for top HUD rendering.");
      return;
    }

    s.program = createProgram(s.gl);
    if (!s.program) return;

    s.positionLoc = s.gl.getAttribLocation(s.program, "a_position");
    s.uvLoc = s.gl.getAttribLocation(s.program, "a_uv");
    s.textureLoc = s.gl.getUniformLocation(s.program, "u_texture");
    s.quadBuffer = s.gl.createBuffer();

    if (s.positionLoc < 0 || s.uvLoc < 0 || !s.textureLoc || !s.quadBuffer) {
      console.error("Failed to initialize top HUD WebGL buffers.");
      return;
    }

    s.gl.disable(s.gl.DEPTH_TEST);
    s.gl.disable(s.gl.CULL_FACE);
    s.gl.enable(s.gl.BLEND);
    s.gl.blendFunc(s.gl.SRC_ALPHA, s.gl.ONE_MINUS_SRC_ALPHA);
    s.gl.clearColor(0, 0, 0, 0);

    void loadRaceTextures(propsRef.current.race);
    requestDraw();

    const onResize = () => requestDraw();
    window.addEventListener("resize", onResize);

    if (typeof ResizeObserver !== "undefined") {
      s.resizeObserver = new ResizeObserver(() => requestDraw());
      s.resizeObserver.observe(canvas);
    }

    return () => {
      s.destroyed = true;
      window.removeEventListener("resize", onResize);
      if (s.resizeObserver) {
        s.resizeObserver.disconnect();
        s.resizeObserver = null;
      }
      if (s.drawRaf) window.cancelAnimationFrame(s.drawRaf);
      if (s.gl) {
        for (const texture of s.textureCache.values())
          s.gl.deleteTexture(texture);
        if (s.quadBuffer) s.gl.deleteBuffer(s.quadBuffer);
        if (s.program) s.gl.deleteProgram(s.program);
      }
    };
  }, []);

  // React to race changes
  useEffect(() => {
    const s = stateRef.current;
    if (!s.gl || s.destroyed) return;
    const loadRaceTextures = (s as any).loadRaceTextures as
      | ((race: Race) => Promise<void>)
      | undefined;
    if (loadRaceTextures) void loadRaceTextures(props.race);
  }, [props.race]);

  // React to leftOnly changes
  useEffect(() => {
    const s = stateRef.current;
    const requestDraw = (s as any).requestDraw as (() => void) | undefined;
    if (requestDraw) requestDraw();
  }, [props.leftOnly]);

  return (
    <canvas
      ref={canvasRef}
      aria-hidden="true"
      style={{
        position: "absolute",
        inset: "0",
        width: "100%",
        height: "100%",
        display: "block",
        pointerEvents: "none",
      }}
    />
  );
}
