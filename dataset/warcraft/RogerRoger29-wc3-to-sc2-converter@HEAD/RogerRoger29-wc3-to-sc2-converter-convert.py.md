---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "convert.py"
ref: HEAD
---

"""WC3 .mdx -> SC2 .m3 — one-command orchestrator.

Usage:
    python convert.py <model.json>                 # config-driven (recommended; see config.example.json)
    python convert.py <model.mdx> <out_dir>        # quick mode, everything auto-detected
    python convert.py <model.json> --verbose       # detailed debug output
    python convert.py <model.json> --quiet         # only errors and final result

What it does (in system Python — needs numpy + Pillow):
  1. parse the MDX
  2. resolve & convert every file-backed WC3 texture (BLP) to SC2 DDS (DXT5 + mips); particle textures get
     the additive-glow treatment automatically
  3. write a build_config.json
  4. launch Blender headless to run build_m3.py, which builds and exports the .m3 via the m3studio addon

Blender 4.4 with the m3studio addon installed is required for step 4 (see README).
"""
import os, sys, json, subprocess, glob, logging, argparse
from concurrent.futures import ThreadPoolExecutor, as_completed

HERE = os.path.dirname(os.path.abspath(__file__))
sys.path.insert(0, HERE)
import mdx as mdxlib
import textures as tex

log = logging.getLogger("wc3toSC2")


def setup_logging(level):
    """Configure the root logger with a clean format."""
    handler = logging.StreamHandler(sys.stdout)
    handler.setFormatter(logging.Formatter("%(levelname)-7s %(message)s"))
    log.addHandler(handler)
    log.setLevel(level)
    # suppress noisy library loggers
    logging.getLogger("PIL").setLevel(logging.WARNING)


def find_blender(cfg):
    """Locate blender.exe from config, standard install paths, or PATH."""
    if cfg.get("blender") and os.path.exists(cfg["blender"]):
        log.debug("Using Blender from config: %s", cfg["blender"])
        return cfg["blender"]

    cands = []
    if sys.platform == "win32":
        for base in (r"C:\Program Files\Blender Foundation", r"C:\Program Files (x86)\Blender Foundation"):
            cands += sorted(glob.glob(os.path.join(base, "Blender *", "blender.exe")), reverse=True)
    elif sys.platform == "darwin":
        cands += sorted(glob.glob("/Applications/Blender*.app/Contents/MacOS/Blender"), reverse=True)
    else:
        # Linux: check common install locations
        cands += sorted(glob.glob("/usr/local/bin/blender*"), reverse=True)
        cands += sorted(glob.glob("/usr/bin/blender*"), reverse=True)
        cands += sorted(glob.glob(os.path.expanduser("~/blender*/blender")), reverse=True)

    cands += ["blender"]  # PATH fallback (all platforms)

    for c in cands:
        if c == "blender" or os.path.exists(c):
            log.debug("Auto-detected Blender: %s", c)
            return c

    log.critical("Blender not found.")
    log.critical("  • Set \"blender\" in your config JSON to the full path of blender.exe")
    log.critical("  • On Windows this is usually: C:\\Program Files\\Blender Foundation\\Blender 4.4\\blender.exe")
    log.critical("  • On macOS: /Applications/Blender.app/Contents/MacOS/Blender")
    log.critical("  • On Linux: /usr/bin/blender")
    raise SystemExit(1)


def resolve_texture_src(texs_path, search_dirs, override):
    """Find the BLP/image on disk for a WC3 TEXS path, trying an explicit override then search dirs."""
    if override and os.path.exists(override):
        log.debug("  texture override found: %s", override)
        return override
    base = os.path.basename(texs_path.replace("\\", "/"))
    cands = []
    for d in search_dirs:
        cands.append(os.path.join(d, base))                 # by basename
        cands.append(os.path.join(d, texs_path.replace("\\", os.sep)))  # by full relative path
    for c in cands:
        if os.path.exists(c):
            log.debug("  resolved %s -> %s", texs_path, c)
            return c
    return None


def main():
    # ---- argparse with --verbose / --quiet ----
    parser = argparse.ArgumentParser(
        description="WC3 .mdx -> SC2 .m3 model converter",
        usage="%(prog)s <model.json|model.mdx> [out_dir] [--verbose|--quiet]")
    parser.add_argument("input", help="Path to a .json config file or a .mdx model file")
    parser.add_argument("out_dir", nargs="?", default=None,
                        help="Output directory (only used in quick .mdx mode)")
    parser.add_argument("--verbose", "-v", action="store_true", help="Detailed debug output")
    parser.add_argument("--quiet", "-q", action="store_true", help="Only errors and final result")
    parser.add_argument("--dry-run", action="store_true",
                        help="Parse MDX and resolve textures but skip Blender/M3 export")
    parser.add_argument("--no-parallel", action="store_true",
                        help="Disable multi-threaded texture conversion")
    args = parser.parse_args()

    if args.verbose:
        setup_logging(logging.DEBUG)
    elif args.quiet:
        setup_logging(logging.WARNING)
    else:
        setup_logging(logging.INFO)

    # ---- load config ----
    arg = args.input
    if arg.lower().endswith(".json"):
        try:
            cfg = json.load(open(arg, "r", encoding="utf-8"))
        except (json.JSONDecodeError, FileNotFoundError) as e:
            log.critical("Failed to load config '%s': %s", arg, e)
            raise SystemExit(1)
        cfg_dir = os.path.dirname(os.path.abspath(arg))
    else:
        if not os.path.exists(arg):
            log.critical("MDX file not found: %s", arg)
            raise SystemExit(1)
        cfg = {"mdx": arg, "out_dir": args.out_dir or "Converted"}
        cfg_dir = os.getcwd()

    def rel(p):
        return p if os.path.isabs(p) else os.path.normpath(os.path.join(cfg_dir, p))

    mdx_path = rel(cfg["mdx"])
    if not os.path.exists(mdx_path):
        log.critical("MDX file not found: %s", mdx_path)
        raise SystemExit(1)

    out_dir = rel(cfg.get("out_dir", "Converted"))
    os.makedirs(out_dir, exist_ok=True)
    name = cfg.get("model_name") or os.path.splitext(os.path.basename(mdx_path))[0]
    asset_dir = cfg.get("asset_texture_dir", "Assets\\Textures\\")
    search_dirs = [rel(d) for d in cfg.get("texture_search_dirs", [".", "Textures", os.path.dirname(cfg["mdx"]) or "."])]
    tex_cfg = cfg.get("textures", {})

    # ---- parse MDX ----
    log.info("Parsing %s ...", os.path.basename(mdx_path))
    try:
        m = mdxlib.parse(mdx_path)
    except Exception as e:
        log.critical("Failed to parse MDX '%s': %s", mdx_path, e)
        raise SystemExit(1)

    log.info("=== %s: %d textures, %d materials, %d geosets, %d sequences, %d emitters ===",
             name, len(m["textures"]), len(m["materials"]), len(m["geosets"]),
             len(m["sequences"]), len(m["particles"]))

    particle_texids = {e["textureId"] for e in m["particles"]}

    # ---- convert textures ----
    tex_map = {}   # wc3 TEXS index -> output dds basename (for build_m3)
    missing = []

    # Collect conversion jobs first
    jobs = []
    for i, t in enumerate(m["textures"]):
        if t["replaceableId"] in (1, 2):
            log.info("  tex[%d] replaceable team texture — handled as team-colour emissive (no file needed)", i)
            continue
        if not t["path"]:
            continue
        ov = tex_cfg.get(str(i), {})
        src = resolve_texture_src(t["path"], search_dirs, ov.get("src"))
        out_name = ov.get("out") or (os.path.splitext(os.path.basename(t["path"].replace("\\", "/")))[0] + ".dds")
        if not src:
            missing.append((i, t["path"]))
            log.warning("  tex[%d] %-28s MISSING (searched %d dirs)", i, t["path"], len(search_dirs))
            tex_map[i] = out_name
            continue
        glow = ov.get("glow", i in particle_texids)
        alpha_invert = ov.get("alpha_invert", False)
        glow_dim = float(ov.get("glow_dim", 0.7))
        out_path = os.path.join(out_dir, out_name)
        jobs.append((i, src, out_path, out_name, glow, alpha_invert, glow_dim, t["path"]))

    # Convert textures (parallel if more than one and not disabled)
    use_parallel = not args.no_parallel and len(jobs) > 1
    if jobs:
        log.info("Converting %d texture(s)%s...", len(jobs), " in parallel" if use_parallel else "")
    if use_parallel:
        def _convert_one(job):
            i, src, out_path, out_name, glow, alpha_invert, glow_dim, orig_path = job
            try:
                size, mips = tex.convert_texture(
                    src, out_path, alpha_invert=alpha_invert, glow=glow, glow_dim=glow_dim)
                return (i, out_name, src, size, mips, glow, alpha_invert, None)
            except Exception as e:
                return (i, out_name, src, 0, 0, glow, alpha_invert, (orig_path, str(e)))
        with ThreadPoolExecutor(max_workers=min(8, os.cpu_count() or 4)) as ex:
            futures = {ex.submit(_convert_one, j): j for j in jobs}
            for fut in as_completed(futures):
                i, out_name, src, size, mips, glow, alpha_invert, err = fut.result()
                if err:
                    orig_path, exc_msg = err
                    log.error("  tex[%d] %s — FAILED: %s", i, os.path.basename(orig_path), exc_msg)
                    missing.append((i, orig_path))
                else:
                    log.info("  tex[%d] %-28s -> %-22s %s mips=%d%s%s",
                             i, os.path.basename(src), out_name, size, mips,
                             " [glow]" if glow else "", " [alpha-invert]" if alpha_invert else "")
                tex_map[i] = out_name
    else:
        for i, src, out_path, out_name, glow, alpha_invert, glow_dim, orig_path in jobs:
            try:
                size, mips = tex.convert_texture(
                    src, out_path, alpha_invert=alpha_invert, glow=glow, glow_dim=glow_dim)
            except Exception as e:
                log.error("  tex[%d] %s — FAILED: %s", i, os.path.basename(orig_path), e)
                missing.append((i, orig_path))
            else:
                log.info("  tex[%d] %-28s -> %-22s %s mips=%d%s%s",
                         i, os.path.basename(src), out_name, size, mips,
                         " [glow]" if glow else "", " [alpha-invert]" if alpha_invert else "")
            tex_map[i] = out_name

    # --dry-run: stop before Blender
    if args.dry_run:
        log.info("Dry-run complete.  Parsed MDX, processed %d texture jobs.  Skipping Blender/M3 export.", len(jobs))
        if missing:
            log.warning("%d texture(s) not found: %s", len(missing), ", ".join(p for _, p in missing))
        return

    # ---- write build config for Blender ----
    build_cfg = {
        "mdx": mdx_path, "out": os.path.join(out_dir, name + ".m3"),
        "model_name": name, "scale": float(cfg.get("scale", 1.0)),
        "asset_texture_dir": asset_dir, "textures": {str(k): v for k, v in tex_map.items()},
        "anim_names": cfg.get("anim_names", {}), "attachments": cfg.get("attachments"),
        "features": cfg.get("features", {}),
        "particle_rate_scale": float(cfg.get("particle_rate_scale", 1.0)),
        "particle_size_scale": float(cfg.get("particle_size_scale", 1.0)),
        "team_color": bool(cfg.get("team_color", True)),
    }
    build_cfg_path = os.path.join(out_dir, "_build_config.json")
    json.dump(build_cfg, open(build_cfg_path, "w", encoding="utf-8"), indent=1)
    log.debug("Build config written to %s", build_cfg_path)

    # ---- run Blender ----
    blender = find_blender(cfg)
    cmd = [blender, "--background", "--factory-startup", "--python", os.path.join(HERE, "build_m3.py"),
           "--", build_cfg_path]
    log.info("Launching Blender ...")
    log.debug("  %s", " ".join('"%s"' % c if " " in c else c for c in cmd))

    try:
        proc = subprocess.run(cmd, capture_output=True, text=True, timeout=300)
    except subprocess.TimeoutExpired:
        log.critical("Blender timed out after 5 minutes — the conversion may be stuck or the model is too complex.")
        raise SystemExit(1)
    except FileNotFoundError:
        log.critical("Blender executable not found at '%s'. Check your config or Blender installation.", blender)
        raise SystemExit(1)

    # Filter key output lines from Blender
    tail = [ln for ln in proc.stdout.splitlines()
            if any(k in ln for k in ("anim '", "mat[", "particle[", "attachment", "hit-test", "bounds",
                                     "baked", "EXPORT_OK", "EXPORT_FAILED", "BUILD_DONE",
                                     "warn", "Error", "ERROR", "CRITICAL"))]
    for t in tail:
        log.info("  %s", t)

    if "EXPORT_FAILED" in proc.stdout or proc.returncode != 0:
        log.critical("Build FAILED — Blender stderr (last 2000 chars):\n%s", proc.stderr[-2000:])
        raise SystemExit(1)

    log.info("DONE.  Output: %s", build_cfg["out"])
    log.info("Textures: %s%s*.dds  (import into SC2 under %s)", out_dir, os.sep, asset_dir)
    if missing:
        log.warning("%d texture(s) were not found on disk and must be supplied manually: %s",
                    len(missing), ", ".join(p for _, p in missing))


if __name__ == "__main__":
    main()
