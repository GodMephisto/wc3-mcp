---
type: vendor-doc
source_repo: "https://github.com/TrayHard/casc-modern"
source_path: "tailwind.config.js"
ref: HEAD
---

/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{ts,tsx}"],
  theme: { extend: {} },
  corePlugins: { preflight: false }, // antd has its own resets
  plugins: [],
};
