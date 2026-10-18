---
type: vendor-doc
source_repo: "https://github.com/sektant1/warcraft-ui"
source_path: "src/components/CursorOverlay/CursorOverlay.tsx"
ref: HEAD
---

import { useEffect, useRef } from "react";
import { useCurrentRace } from "../../state/race";
import { loadBlpDataUrl, loadDataUrlImage } from "../../utils/glueButton";
import type { Race } from "../../utils/types";

const cursorFrameDataUrlCache = new Map<Race, Promise<string>>();

async function loadRaceCursorFrameDataUrl(race: Race): Promise<string> {
  const cached = cursorFrameDataUrlCache.get(race);
  if (cached) return cached;

  const promise = (async () => {
    const atlasDataUrl = await loadBlpDataUrl(`./cursor/${race}Cursor.blp`);
    const atlas = await loadDataUrlImage(atlasDataUrl);
    const columns = 8;
    const rows = 4;
    const frameW = Math.max(1, Math.floor(atlas.width / columns));
    const frameH = Math.max(1, Math.floor(atlas.height / rows));
    const canvas = document.createElement("canvas");
    canvas.width = frameW;
    canvas.height = frameH;
    const ctx = canvas.getContext("2d");
    if (!ctx) return atlasDataUrl;
    ctx.imageSmoothingEnabled = false;
    ctx.clearRect(0, 0, frameW, frameH);
    ctx.drawImage(atlas, 0, 0, frameW, frameH, 0, 0, frameW, frameH);
    return canvas.toDataURL("image/png");
  })();

  cursorFrameDataUrlCache.set(race, promise);
  return promise;
}

interface Props {
  race?: Race;
}

export default function CursorOverlay({ race: raceProp }: Props = {}) {
  const ref = useRef<HTMLDivElement>(null);
  const globalRace = useCurrentRace();
  const race = raceProp ?? globalRace;

  useEffect(() => {
    const el = ref.current!;
    const onMove = (e: MouseEvent) => {
      el.style.left = e.clientX + "px";
      el.style.top = e.clientY + "px";
    };
    const onLeave = () => {
      el.style.display = "none";
    };
    const onEnter = () => {
      el.style.display = "block";
    };

    document.addEventListener("mousemove", onMove);
    document.addEventListener("mouseleave", onLeave);
    document.addEventListener("mouseenter", onEnter);

    return () => {
      document.removeEventListener("mousemove", onMove);
      document.removeEventListener("mouseleave", onLeave);
      document.removeEventListener("mouseenter", onEnter);
    };
  }, []);

  useEffect(() => {
    let cancelled = false;

    void loadRaceCursorFrameDataUrl(race)
      .then((cursorFrameUrl) => {
        if (cancelled) return;
        document.documentElement.style.setProperty(
          "--wc3-game-cursor",
          `url("${cursorFrameUrl}") 2 2, auto`,
        );
      })
      .catch((err) => {
        console.error(err);
        document.documentElement.style.setProperty("--wc3-game-cursor", "auto");
      });

    return () => {
      cancelled = true;
      document.documentElement.style.setProperty("--wc3-game-cursor", "auto");
    };
  }, [race]);

  return (
    <div
      ref={ref}
      id="cursorOverlay"
      style={{ backgroundImage: `url('war3/Cursor/${race}Cursor.png')` }}
    />
  );
}
