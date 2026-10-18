---
type: vendor-doc
source_repo: "https://github.com/qq200774491/MoyingWar3ModelRuntime"
source_path: "vite.viewer.config.ts"
ref: HEAD
---

import { readdirSync, readFileSync, statSync } from 'node:fs';
import path from 'node:path';
import { defineConfig, type Plugin, type ViteDevServer } from 'vite';

/**
 * 开发用模型查看器配置 / Dev config for the in-browser model viewer.
 *
 *   npm run viewer
 *
 * 提供两个接口：
 *   GET /model-manifest.json      —— 列出模型目录下所有文件（相对路径）与可加载模型
 *   GET /model-assets/<相对路径>   —— 直接读取模型/贴图字节（支持中文文件名）
 */
const MODEL_ROOT = path.resolve('test/模型');

function walk(root: string): string[] {
  const out: string[] = [];
  const rec = (dir: string) => {
    for (const entry of readdirSync(dir)) {
      const abs = path.join(dir, entry);
      if (statSync(abs).isDirectory()) rec(abs);
      else out.push(path.relative(root, abs).replace(/\\/g, '/'));
    }
  };
  rec(root);
  return out;
}

function war3ModelServer(): Plugin {
  return {
    name: 'war3-model-server',
    configureServer(server: ViteDevServer) {
      server.middlewares.use((req, res, next) => {
        const url = (req.url ?? '').split('?')[0];

        if (url === '/model-manifest.json') {
          const files = walk(MODEL_ROOT).sort();
          const models = files.filter((f) => /\.(mdx|mdl)$/i.test(f)).sort();
          res.setHeader('content-type', 'application/json; charset=utf-8');
          res.end(JSON.stringify({ files, models }));
          return;
        }

        if (url.startsWith('/model-assets/')) {
          const rel = decodeURIComponent(url.slice('/model-assets/'.length));
          const abs = path.resolve(MODEL_ROOT, rel);
          if (!abs.startsWith(MODEL_ROOT)) { res.statusCode = 403; res.end('forbidden'); return; }
          try {
            const bytes = readFileSync(abs);
            res.setHeader('content-type', 'application/octet-stream');
            res.setHeader('cache-control', 'no-cache');
            res.end(bytes);
          } catch {
            res.statusCode = 404;
            res.end('not found');
          }
          return;
        }

        next();
      });
    },
  };
}

export default defineConfig({
  root: 'examples/web',
  publicDir: false,
  server: { open: '/index.html', port: 5173 },
  plugins: [war3ModelServer()],
});
