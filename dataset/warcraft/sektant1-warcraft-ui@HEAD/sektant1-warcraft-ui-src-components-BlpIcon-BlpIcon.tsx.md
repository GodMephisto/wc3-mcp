---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/BlpIcon/BlpIcon.tsx"
ref: HEAD
---

import { useEffect, useState } from "react";
import { loadBlpDataUrl } from "../../utils/glueButton";

const FALLBACK_ICON = "./buttons/command/BTNTemp.blp";

interface BlpIconProps {
  /** Full path relative to /public, e.g. "./buttons/command/BTNFootman.blp" */
  path: string;
  size?: number;
  className?: string;
  alt?: string;
  title?: string;
}

export default function BlpIcon({
  path,
  size = 32,
  className = "",
  alt = "",
  title,
}: BlpIconProps) {
  const [url, setUrl] = useState("");

  useEffect(() => {
    let cancelled = false;
    void loadBlpDataUrl(path)
      .catch(() => loadBlpDataUrl(FALLBACK_ICON))
      .then((u) => {
        if (!cancelled) setUrl(u);
      })
      .catch(() => {});
    return () => {
      cancelled = true;
    };
  }, [path]);

  if (!url) {
    return (
      <span
        className={`blp-icon-placeholder ${className}`}
        style={{ width: size, height: size, display: "inline-block" }}
      />
    );
  }

  return (
    <img
      src={url}
      alt={alt}
      title={title}
      className={`blp-icon ${className}`}
      style={{ width: size, height: size }}
      draggable={false}
    />
  );
}
