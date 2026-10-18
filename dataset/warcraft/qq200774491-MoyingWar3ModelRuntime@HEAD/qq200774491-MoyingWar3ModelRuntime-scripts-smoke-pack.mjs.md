---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "scripts/smoke-pack.mjs"
ref: HEAD
---

import { execFileSync } from 'node:child_process';
import {
  copyFileSync,
  existsSync,
  mkdirSync,
  readFileSync,
  rmSync,
  writeFileSync,
} from 'node:fs';
import { dirname, join, resolve } from 'node:path';
import { fileURLToPath } from 'node:url';

const root = resolve(dirname(fileURLToPath(import.meta.url)), '..');
const smoke = join(root, '.smoke-consumer');
const npmCli = process.env.npm_execpath;
if (!npmCli) throw new Error('npm_execpath is not available. Run this script through npm.');
const runNpm = (args, options = {}) => execFileSync(process.execPath, [npmCli, ...args], options);

rmSync(smoke, { recursive: true, force: true });
mkdirSync(join(smoke, 'src'), { recursive: true });

const packOutput = runNpm(['pack', '--json', '--ignore-scripts'], {
  cwd: root,
  encoding: 'utf8',
});
const pack = JSON.parse(packOutput).at(0);
if (!pack?.filename) throw new Error('npm pack did not report an output file.');
copyFileSync(join(root, pack.filename), join(smoke, 'runtime.tgz'));

writeFileSync(join(smoke, 'package.json'), JSON.stringify({
  private: true,
  type: 'module',
  scripts: { build: 'tsc --noEmit && vite build' },
  dependencies: {
    '@babylonjs/core': '7.54.3',
    '@moying/war3-model-runtime': 'file:./runtime.tgz',
  },
  devDependencies: {
    typescript: '5.9.3',
    vite: '5.4.21',
  },
}, null, 2));
writeFileSync(join(smoke, 'tsconfig.json'), JSON.stringify({
  compilerOptions: {
    target: 'ES2022',
    module: 'ESNext',
    moduleResolution: 'Bundler',
    lib: ['ES2022', 'DOM'],
    strict: true,
    noEmit: true,
    skipLibCheck: true,
  },
  include: ['src'],
}, null, 2));
writeFileSync(join(smoke, 'index.html'), '<div id="app"></div><script type="module" src="/src/main.ts"></script>\n');
writeFileSync(join(smoke, 'src', 'main.ts'), `
import { createWar3BabylonViewer } from '@moying/war3-model-runtime';
import type { War3ViewerOptions } from '@moying/war3-model-runtime';

const options: War3ViewerOptions = {
  resolveResource: async () => undefined,
};
(globalThis as typeof globalThis & { __runtimeSmoke?: unknown }).__runtimeSmoke = {
  createWar3BabylonViewer,
  options,
};
`);

runNpm(['install', '--ignore-scripts', '--prefer-offline'], { cwd: smoke, stdio: 'inherit' });
runNpm(['run', 'build'], { cwd: smoke, stdio: 'inherit' });
const output = join(smoke, 'dist', 'index.html');
if (!existsSync(output) || !readFileSync(output, 'utf8').includes('<script')) {
  throw new Error('Consumer smoke build did not produce a usable index.html.');
}

rmSync(smoke, { recursive: true, force: true });
console.log(`Packed and consumed ${pack.filename}`);
