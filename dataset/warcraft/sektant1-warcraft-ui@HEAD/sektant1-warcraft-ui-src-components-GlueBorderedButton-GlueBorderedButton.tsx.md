---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/GlueBorderedButton/GlueBorderedButton.tsx"
ref: HEAD
---

import { useEffect, useState } from "react";
import { loadBlpDataUrl, useNineSliceButton } from "../../utils/glueButton";
import type { NineSliceUrls } from "../../utils/glueButton";
import "../GlueScreenButton/style.css";

const PATHS = {
  bg: "./buttons/glue/GlueScreen-Button1-BackdropBackground.blp",
  bgDown: "./buttons/glue/GlueScreen-Button1-BackdropBackground-Down.blp",
  bgDisabled:
    "./buttons/glue/GlueScreen-Button1-BackdropBackground-Disabled.blp",
  border: "./buttons/glue/GlueScreen-Button1-BorderedBackdropBorder.blp",
  borderDown:
    "./buttons/glue/GlueScreen-Button1-BorderedBackdropBorder-Down.blp",
  borderDisabled:
    "./buttons/glue/GlueScreen-Button1-BorderedBackdropBorder-Disabled.blp",
  hover: "./buttons/glue/bnet-button01-highlight-mouse.blp",
};

const pressedOffsetX = -2;
const pressedOffsetY = 3;

interface Props {
  onClick?: () => void;
  disabled?: boolean;
  children: React.ReactNode;
}

export default function GlueBorderedButton({
  onClick,
  disabled,
  children,
}: Props) {
  const [urls, setUrls] = useState<NineSliceUrls>({
    bg: "",
    bgDown: "",
    bgDisabled: "",
    border: "",
    borderDown: "",
    borderDisabled: "",
    hover: "",
  });

  useEffect(() => {
    void Promise.all(Object.values(PATHS).map((p) => loadBlpDataUrl(p))).then(
      ([bg, bgDown, bgDisabled, border, borderDown, borderDisabled, hover]) => {
        setUrls({
          bg,
          bgDown,
          bgDisabled,
          border,
          borderDown,
          borderDisabled,
          hover,
        });
      },
    );
  }, []);

  const { canvasRef, pressed, handlers } = useNineSliceButton(urls, disabled);

  return (
    <button
      type="button"
      className="glue-screen-button"
      disabled={disabled}
      onClick={onClick}
      {...handlers}
    >
      <canvas ref={canvasRef} className="glue-screen-canvas" />
      <span
        style={{
          transform: pressed
            ? `translate(${pressedOffsetX}px, ${pressedOffsetY}px)`
            : "none",
        }}
      >
        {children}
      </span>
    </button>
  );
}
