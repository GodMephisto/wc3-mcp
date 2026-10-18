---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/config/nav-items.ts"
ref: HEAD
---

interface NavMainItem {
  title: string;
  items: { title: string; url: string }[];
}

export const navItems = {
  header: [
    {
      label: "Docs",
      href: "/docs",
    },
    {
      label: "Components",
      href: "/docs/components",
    },
  ],
  navMain: [] as NavMainItem[],
};

export const TOP_LEVEL_SECTIONS = [{ name: "Get Started", href: "/docs" }];
