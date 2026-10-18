---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/Heading/Heading.tsx"
ref: HEAD
---

import type { ReactNode } from "react";
import "./style.css";

type HeadingLevel = 1 | 2 | 3 | 4;

interface HeadingProps {
  level?: HeadingLevel;
  icon?: ReactNode;
  children: ReactNode;
  className?: string;
}

export default function Heading({
  level = 2,
  icon,
  children,
  className,
}: HeadingProps) {
  const Tag = `h${level}` as const;

  return (
    <Tag
      className={`wc3-heading wc3-heading--${level}${className ? ` ${className}` : ""}`}
    >
      {icon && <span className="wc3-heading__icon">{icon}</span>}
      {children}
    </Tag>
  );
}
