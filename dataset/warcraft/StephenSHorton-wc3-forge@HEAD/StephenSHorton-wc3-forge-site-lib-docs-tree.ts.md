---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/lib/docs-tree.ts"
ref: HEAD
---

import { source } from "@/lib/source";

export interface DocLink {
  title: string;
  url: string;
}

type TreeNode = {
  type: string;
  name?: React.ReactNode;
  url?: string;
  index?: { name?: React.ReactNode; url: string };
  children?: TreeNode[];
};

/**
 * Flatten the Fumadocs page tree into an ordered list of { title, url } in the
 * order declared by content/docs/meta.json. Titles come from each page's
 * frontmatter so they're plain strings (safe for the client-side filter).
 */
export function getDocLinks(): DocLink[] {
  const titleByUrl = new Map(
    source.getPages().map((p) => [p.url, p.data.title])
  );

  const out: DocLink[] = [];
  const walk = (nodes: TreeNode[] = []) => {
    for (const node of nodes) {
      if (node.type === "page" && node.url) {
        out.push({
          url: node.url,
          title: titleByUrl.get(node.url) ?? String(node.name ?? node.url),
        });
      } else if (node.type === "folder") {
        if (node.index?.url) {
          out.push({
            url: node.index.url,
            title:
              titleByUrl.get(node.index.url) ??
              String(node.index.name ?? node.index.url),
          });
        }
        walk(node.children);
      }
    }
  };

  walk(source.pageTree.children as TreeNode[]);
  return out;
}
