---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/app/layout.tsx"
ref: HEAD
---

import { RootProvider } from "fumadocs-ui/provider/next";
import { Inter } from "next/font/google";
import type { Metadata } from "next";
import { Toaster } from "sonner";

import "./global.css";
// Load the warcraftcn theme (Cinzel `.fantasy` font, cursors, frame classes) globally.
import "@/components/ui/warcraftcn/styles/warcraft.css";

const inter = Inter({
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: {
    default: "wc3-forge — a Warcraft III map editor for Claude Code",
    template: "%s — wc3-forge",
  },
  description:
    "wc3-forge is a native Warcraft III map editor with an embedded MCP server. Edit terrain, units, objects, and triggers by hand or drive the whole editor with Claude Code — sharing one undo stack.",
};

export default function Layout({ children }: { children: React.ReactNode }) {
  return (
    <html className={inter.className} lang="en" suppressHydrationWarning>
      <body className="flex min-h-screen flex-col">
        {/* Static search is wired separately; disabled until the index route is built. */}
        <RootProvider search={{ enabled: false }}>
          {children}
          <Toaster position="top-right" />
        </RootProvider>
      </body>
    </html>
  );
}
