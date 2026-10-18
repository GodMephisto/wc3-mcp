---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "site/components/download-button.tsx"
ref: HEAD
---

"use client";

import { useEffect, useState } from "react";

import { Badge } from "@/components/ui/warcraftcn/badge";
import { Button } from "@/components/ui/warcraftcn/button";
import { Spinner } from "@/components/ui/warcraftcn/spinner";
import {
  Tooltip,
  TooltipBody,
  TooltipContent,
  TooltipTitle,
  TooltipTrigger,
} from "@/components/ui/warcraftcn/tooltip";

const OWNER = "StephenSHorton";
const REPO = "wc3-forge";
// GitHub redirects /releases/latest/download/<asset> to the newest release's
// matching asset, so this link is always current with no build step.
const INSTALLER_URL = `https://github.com/${OWNER}/${REPO}/releases/latest/download/wc3-forge-amd64-installer.exe`;
const RELEASES_URL = `https://github.com/${OWNER}/${REPO}/releases/latest`;

export function DownloadButton() {
  const [version, setVersion] = useState<string | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    let cancelled = false;
    fetch(`https://api.github.com/repos/${OWNER}/${REPO}/releases/latest`)
      .then((res) => (res.ok ? res.json() : null))
      .then((data) => {
        if (!cancelled && data?.tag_name) {
          setVersion(data.tag_name as string);
        }
      })
      .catch(() => {
        // Offline / rate-limited — the button still works without a version.
      })
      .finally(() => {
        if (!cancelled) {
          setLoading(false);
        }
      });
    return () => {
      cancelled = true;
    };
  }, []);

  return (
    <div className="flex flex-col items-center gap-2">
      <Tooltip>
        <TooltipTrigger asChild>
          <a href={INSTALLER_URL} rel="noopener" download>
            <Button className="gap-2 px-10 text-xl">
              Download for Windows
              {loading ? (
                <Spinner className="size-5" />
              ) : (
                version && (
                  <Badge size="sm" variant="secondary">
                    {version}
                  </Badge>
                )
              )}
            </Button>
          </a>
        </TooltipTrigger>
        <TooltipContent variant="legendary">
          <TooltipTitle>wc3-forge-amd64-installer.exe</TooltipTitle>
          <TooltipBody>
            Windows installer, bundles the CASC libraries. Always the latest
            release.
          </TooltipBody>
        </TooltipContent>
      </Tooltip>
      <a
        className="text-muted-foreground text-xs underline-offset-4 hover:underline"
        href={RELEASES_URL}
        rel="noopener noreferrer"
        target="_blank"
      >
        All releases &amp; changelog
      </a>
    </div>
  );
}
