---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "README.md"
ref: HEAD
---

# Moying War3 Model Runtime

`@moying/war3-model-runtime` is an ESM TypeScript library for parsing and rendering Warcraft III MDX/MDL models with Babylon.js.

## Install

```sh
npm install @moying/war3-model-runtime @babylonjs/core
```

## Usage

The host owns resource resolution. The library never assumes an API route or storage layout.

```ts
import { createWar3BabylonViewer } from '@moying/war3-model-runtime';

const viewer = await createWar3BabylonViewer(canvas, {
  resolveResource: async ({ requestedPath, signal }) => {
    const response = await fetch(requestedPath, { signal });
    if (!response.ok) throw new Error(`HTTP ${response.status}`);
    return {
      bytes: await response.arrayBuffer(),
      resolvedPath: requestedPath,
      mime: response.headers.get('content-type') ?? undefined,
    };
  },
});

await viewer.loadModel({ path: 'Units/Human/Footman/Footman.mdx' });
```

See [`examples/basic.ts`](examples/basic.ts) for a complete host resolver example.

## Compatibility

- Modern browsers with WebGPU or WebGL2.
- Babylon.js 7.54.3 or later in the 7.x line, supplied by the host.
- MDX binary and MDL text model inputs.

## License

MIT. See `THIRD_PARTY_NOTICES.md` for incorporated and peer dependency notices.
