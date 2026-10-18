---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/app/docs/[[...slug]]/page.tsx"
ref: HEAD
---

import { DocsBody } from "fumadocs-ui/page";
import { createRelativeLink } from "fumadocs-ui/mdx";
import type { Metadata } from "next";
import { notFound } from "next/navigation";

import { DocsPager } from "@/components/docs/docs-pager";
import { DocsToc, type TocItem } from "@/components/docs/docs-toc";
import { Badge } from "@/components/ui/warcraftcn/badge";
import { getDocLinks } from "@/lib/docs-tree";
import { source } from "@/lib/source";
import { getMDXComponents } from "@/mdx-components";

export default async function Page(props: {
  params: Promise<{ slug?: string[] }>;
}) {
  const params = await props.params;
  const page = source.getPage(params.slug);
  if (!page) {
    notFound();
  }

  const MDXContent = page.data.body;

  const links = getDocLinks();
  const idx = links.findIndex((l) => l.url === page.url);
  const prev = idx > 0 ? links[idx - 1] : null;
  const next = idx >= 0 && idx < links.length - 1 ? links[idx + 1] : null;

  return (
    <div className="flex gap-10">
      <article className="min-w-0 flex-1 py-10">
        <div className="mb-2">
          <Badge size="sm" variant="default">
            Documentation
          </Badge>
        </div>
        <h1 className="fantasy font-bold text-4xl text-amber-200 tracking-tight [text-shadow:0_0_20px_rgba(251,191,36,0.2)]">
          {page.data.title}
        </h1>
        {page.data.description && (
          <p className="mt-2 text-lg text-muted-foreground">
            {page.data.description}
          </p>
        )}

        <div className="my-6 h-px bg-gradient-to-r from-amber-700/60 via-amber-900/30 to-transparent" />

        <DocsBody className="prose prose-neutral max-w-none dark:prose-invert">
          <MDXContent
            components={getMDXComponents({
              a: createRelativeLink(source, page),
            })}
          />
        </DocsBody>

        <DocsPager next={next} prev={prev} />
      </article>

      <DocsToc items={(page.data.toc ?? []) as TocItem[]} />
    </div>
  );
}

export function generateStaticParams() {
  return source.generateParams();
}

export async function generateMetadata(props: {
  params: Promise<{ slug?: string[] }>;
}): Promise<Metadata> {
  const params = await props.params;
  const page = source.getPage(params.slug);
  if (!page) {
    notFound();
  }

  return {
    title: page.data.title,
    description: page.data.description,
  };
}
