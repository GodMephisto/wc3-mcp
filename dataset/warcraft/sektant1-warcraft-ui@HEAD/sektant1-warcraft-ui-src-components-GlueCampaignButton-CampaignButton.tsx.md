---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/GlueCampaignButton/CampaignButton.tsx"
ref: HEAD
---

import { useEffect, useState } from "react";
import { loadBlpDataUrl, useNineSliceButton } from "../../utils/glueButton";
import type { NineSliceUrls } from "../../utils/glueButton";
import "./style.css";

const PATHS = {
  bg: "./buttons/campaign/GlueScreen-CampaignButton-BackdropBackground.blp",
  bgDown:
    "./buttons/campaign/GlueScreen-CampaignButton-BackdropBackground-Down.blp",
  bgDisabled:
    "./buttons/campaign/GlueScreen-CampaignButton-BackdropBackground-Disabled.blp",
  border: "./buttons/campaign/GlueScreen-CampaignButton-BackdropBorder.blp",
  borderDown:
    "./buttons/campaign/GlueScreen-CampaignButton-BackdropBorder-Down.blp",
  borderDisabled:
    "./buttons/campaign/GlueScreen-CampaignButton-BackdropBorder-Disabled.blp",
  hover: "./buttons/glue/bnet-button01-highlight-mouse.blp",
};

interface Props {
  onClick?: () => void;
  disabled?: boolean;
  children: React.ReactNode;
}

export default function CampaignButton({ onClick, disabled, children }: Props) {
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
      className="campaign-button"
      disabled={disabled}
      onClick={onClick}
      {...handlers}
    >
      <canvas ref={canvasRef} className="glue-screen-canvas" />
      <span
        style={{
          transform: pressed ? "translate(-1px, 1px)" : "none",
        }}
      >
        {children}
      </span>
    </button>
  );
}
