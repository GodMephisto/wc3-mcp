---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/package.json"
ref: HEAD
---

{
  "name": "frontend",
  "private": true,
  "version": "0.3.0",
  "type": "module",
  "scripts": {
    "dev": "vite",
    "build": "vite build",
    "preview": "vite preview",
    "check": "svelte-check --tsconfig ./tsconfig.json"
  },
  "devDependencies": {
    "@internationalized/date": "^3.12.1",
    "@lucide/svelte": "^1.16.0",
    "@sveltejs/vite-plugin-svelte": "^7.1.2",
    "@tailwindcss/vite": "^4.3.0",
    "@tsconfig/svelte": "^5.0.8",
    "bits-ui": "^2.18.1",
    "clsx": "^2.1.1",
    "mode-watcher": "^1.1.0",
    "shadcn-svelte": "^1.2.7",
    "svelte": "^5.55.9",
    "svelte-check": "^4.4.8",
    "svelte-sonner": "^1.1.1",
    "tailwind-merge": "^3.6.0",
    "tailwind-variants": "^3.2.2",
    "tailwindcss": "^4.3.0",
    "tslib": "^2.4.0",
    "tw-animate-css": "^1.4.0",
    "typescript": "^6.0.3",
    "vite": "^8.0.14"
  },
  "dependencies": {
    "@fontsource-variable/nunito": "^5.2.7",
    "events": "^3.3.0",
    "mdx-m3-viewer": "^5.12.0",
    "monaco-editor": "^0.54.0"
  }
}
