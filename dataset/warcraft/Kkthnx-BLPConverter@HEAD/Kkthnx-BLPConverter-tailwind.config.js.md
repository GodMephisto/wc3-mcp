---
type: vendor-doc
source_repo: "https://github.com/Kkthnx/BLPConverter"
source_path: "tailwind.config.js"
ref: HEAD
---

/** @type {import('tailwindcss').Config} */
export default {
  content: ["./index.html", "./src/**/*.{js,ts,jsx,tsx}"],
  theme: {
    extend: {
      colors: {
        workspace: {
          bg: "#0a0c10",
          surface: "#10141b",
          panel: "#161b24",
          elevated: "#1d2430",
          border: "#2a3140",
          silver: "#aab3c1",
          "silver-muted": "#7c8798",
          accent: "#3b9ef5",
          "accent-dim": "#2b7ad0",
          "accent-bright": "#6bb6ff",
          "accent-glow": "rgba(59, 158, 245, 0.16)",
        },
      },
      boxShadow: {
        accent: "0 0 20px rgba(59, 158, 245, 0.22)",
        panel: "0 4px 24px rgba(0, 0, 0, 0.45)",
      },
      animation: {
        "pulse-accent": "pulse-accent 2s ease-in-out infinite",
        indeterminate: "indeterminate 1.1s ease-in-out infinite",
      },
      keyframes: {
        "pulse-accent": {
          "0%, 100%": { opacity: "1" },
          "50%": { opacity: "0.55" },
        },
        indeterminate: {
          "0%": { transform: "translateX(-100%)" },
          "100%": { transform: "translateX(400%)" },
        },
      },
    },
  },
  plugins: [],
};
