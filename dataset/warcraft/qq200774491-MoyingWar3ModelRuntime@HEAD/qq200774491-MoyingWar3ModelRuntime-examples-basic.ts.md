---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "examples/basic.ts"
ref: HEAD
---

import { createWar3BabylonViewer } from '@moying/war3-model-runtime';

const canvas = document.querySelector<HTMLCanvasElement>('#viewer');
if (!canvas) throw new Error('Missing #viewer canvas');

const viewer = await createWar3BabylonViewer(canvas, {
  resolveResource: async ({ candidatePaths, signal }) => {
    for (const path of candidatePaths) {
      const response = await fetch(`/assets/${encodeURI(path)}`, { signal });
      if (!response.ok) continue;
      return {
        bytes: await response.arrayBuffer(),
        resolvedPath: path,
        mime: response.headers.get('content-type') ?? undefined,
      };
    }
    return undefined;
  },
});

await viewer.loadModel({ path: 'Footman.mdx', autoPlay: true });
window.addEventListener('resize', () => viewer.resize());
window.addEventListener('pagehide', () => viewer.dispose(), { once: true });
