---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/GlueMenuButton/GlueMenuButton.tsx"
ref: HEAD
---

import { useEffect, useState } from "react";
import { loadBlpDataUrl, useNineSliceButton } from "../../utils/glueButton";
import type { NineSliceUrls } from "../../utils/glueButton";
import "./style.css";

const BUTTON_PATHS = {
  bg: "./buttons/glue/GlueScreen-Button1-BackdropBackground.blp",
  bgDown: "./buttons/glue/GlueScreen-Button1-BackdropBackground-Down.blp",
  bgDisabled:
    "./buttons/glue/GlueScreen-Button1-BackdropBackground-Disabled.blp",
  border: "./buttons/glue/GlueScreen-Button1-BackdropBorder.blp",
  borderDown: "./buttons/glue/GlueScreen-Button1-BackdropBorder-Down.blp",
  borderDisabled:
    "./buttons/glue/GlueScreen-Button1-BackdropBorder-Disabled.blp",
  hover: "./buttons/glue/bnet-button01-highlight-mouse.blp",
};

const FRAME_PATHS = {
  default: "./buttons/glue/GlueScreen-Button1-Border.blp",
  single: "./borders/glue/GlueScreen-Button1-BorderSingle.blp",
};

const pressedOffsetX = -2;
const pressedOffsetY = 3;

interface Props {
  onClick?: () => void;
  disabled?: boolean;
  /** "single" uses the narrower dialog frame */
  variant?: "default" | "single";
  children: React.ReactNode;
}

export default function GlueMenuButton({
  onClick,
  disabled,
  variant = "default",
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
  const [frameUrl, setFrameUrl] = useState("");

  useEffect(() => {
    void Promise.all([
      ...Object.values(BUTTON_PATHS).map((p) => loadBlpDataUrl(p)),
      loadBlpDataUrl(FRAME_PATHS[variant]),
    ]).then(
      ([
        bg,
        bgDown,
        bgDisabled,
        border,
        borderDown,
        borderDisabled,
        hover,
        frame,
      ]) => {
        setUrls({
          bg,
          bgDown,
          bgDisabled,
          border,
          borderDown,
          borderDisabled,
          hover,
        });
        setFrameUrl(frame);
      },
    );
  }, [variant]);

  const { canvasRef, pressed, handlers } = useNineSliceButton(urls, disabled);

  return (
    <div className="menu-button-shell">
      {frameUrl && <img src={frameUrl} alt="" className="menu-button-frame" />}
      <button
        type="button"
        className={`menu-button-inner${variant === "single" ? " menu-button-inner--single" : ""}`}
        disabled={disabled}
        onClick={onClick}
        {...handlers}
      >
        <canvas ref={canvasRef} className="menu-button-canvas" />
        <span
          className="menu-button-label"
          style={{
            transform: pressed
              ? `translate(${pressedOffsetX}px, ${pressedOffsetY}px)`
              : "none",
            color: disabled ? "#808080" : undefined,
          }}
        >
          {children}
        </span>
      </button>
    </div>
  );
}
