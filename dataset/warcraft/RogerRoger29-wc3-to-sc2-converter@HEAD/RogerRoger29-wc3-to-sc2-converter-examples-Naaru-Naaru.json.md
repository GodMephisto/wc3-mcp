---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "examples/Naaru/Naaru.json"
ref: HEAD
---

{
  "mdx": "Naaru.mdx",
  "out_dir": "out",
  "model_name": "Naaru",
  "scale": 0.04,
  "blender": "",

  "asset_texture_dir": "Assets\\Textures\\",
  "texture_search_dirs": ["."],

  "textures": {
    "0": { "out": "Naaru.dds",       "alpha_invert": false },
    "3": { "out": "Naaru_star.dds",  "glow": true },
    "4": { "out": "Naaru_mana.dds",  "glow": true },
    "5": { "out": "Naaru_shock.dds", "glow": true }
  },

  "anim_names": {
    "Stand - 1": "Stand",
    "Stand - 2": "Stand 02",
    "Stand - 3": "Stand 03",
    "Dissipate": "Death Disintegrate"
  },

  "features": { "animations": true, "attachments": true, "particles": true, "hittest": true, "camera": true },
  "particle_rate_scale": 1.0,
  "team_color": true
}
