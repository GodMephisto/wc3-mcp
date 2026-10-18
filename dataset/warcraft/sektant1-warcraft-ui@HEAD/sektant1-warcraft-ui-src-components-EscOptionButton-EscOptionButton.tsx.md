---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/EscOptionButton/EscOptionButton.tsx"
ref: HEAD
---

import { useEffect, useState } from "react";
import { loadBlpDataUrl, useNineSliceButton } from "../../utils/glueButton";
import type { NineSliceUrls } from "../../utils/glueButton";
import { useCurrentRace } from "../../state/race";
import type { Race } from "../../utils/types";
import "./style.css";

// Press offset from FDF ButtonPushedTextOffset X/Y 0.002f
const pressedOffsetX = 2;
const pressedOffsetY = 2;

interface EscSkin {
  bg: string;
  bgDown: string;
  bgDisabled: string;
  border: string;
  borderDown: string;
  borderDisabled: string;
  hover: string;
}

const HUMAN_BORDER = "./buttons/esc/human/human-options-button-border-up.blp";
const HUMAN_BORDER_DOWN =
  "./buttons/esc/human/human-options-button-border-down.blp";
const HUMAN_DISABLED_BG =
  "./buttons/esc/human/human-options-button-background-disabled.blp";

function getEscSkin(race: Race): EscSkin {
  if (race === "Human") {
    return {
      bg: "./buttons/esc/human/human-options-menu-background.blp",
      bgDown: "./buttons/esc/human/human-options-menu-background.blp",
      bgDisabled: HUMAN_DISABLED_BG,
      border: HUMAN_BORDER,
      borderDown: HUMAN_BORDER_DOWN,
      borderDisabled: HUMAN_BORDER,
      hover: "./buttons/esc/human/human-options-button-highlight.blp",
    };
  }
  if (race === "Orc") {
    return {
      bg: "./buttons/esc/orc/orc-options-button-background.blp",
      bgDown: "./buttons/esc/orc/orc-options-button-background-down.blp",
      bgDisabled: HUMAN_DISABLED_BG,
      border: HUMAN_BORDER,
      borderDown: HUMAN_BORDER_DOWN,
      borderDisabled: HUMAN_BORDER,
      hover: "./buttons/esc/orc/orc-options-button-highlight.blp",
    };
  }
  if (race === "NightElf") {
    return {
      bg: "./buttons/esc/nightelf/nightelf-options-button-background.blp",
      bgDown:
        "./buttons/esc/nightelf/nightelf-options-button-background-down.blp",
      bgDisabled: HUMAN_DISABLED_BG,
      border: HUMAN_BORDER,
      borderDown: HUMAN_BORDER_DOWN,
      borderDisabled: HUMAN_BORDER,
      hover: "./buttons/esc/nightelf/nightelf-options-button-highlight.blp",
    };
  }
  // Undead
  return {
    bg: "./buttons/esc/undead/undead-options-button-background.blp",
    bgDown: "./buttons/esc/undead/undead-options-button-background-down.blp",
    bgDisabled:
      "./buttons/esc/undead/undead-options-button-background-disabled.blp",
    border: HUMAN_BORDER,
    borderDown: HUMAN_BORDER_DOWN,
    borderDisabled: HUMAN_BORDER,
    hover: "./buttons/esc/undead/undead-options-button-highlight.blp",
  };
}

interface Props {
  onClick?: () => void;
  disabled?: boolean;
  children: React.ReactNode;
  race?: Race;
}

export default function EscOptionButton({
  onClick,
  disabled,
  children,
  race: raceProp,
}: Props) {
  const globalRace = useCurrentRace() ?? "Human";
  const race = raceProp ?? globalRace;
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
    const skin = getEscSkin(race);
    void Promise.all(Object.values(skin).map((p) => loadBlpDataUrl(p))).then(
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
  }, [race]);

  const { canvasRef, pressed, handlers } = useNineSliceButton(urls, disabled);

  return (
    <button
      type="button"
      className="esc-option-preview"
      disabled={disabled}
      onClick={onClick}
      {...handlers}
    >
      <canvas ref={canvasRef} className="esc-option-canvas" />
      <span
        className={`esc-option-label${disabled ? " esc-option-label-disabled" : ""}`}
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
