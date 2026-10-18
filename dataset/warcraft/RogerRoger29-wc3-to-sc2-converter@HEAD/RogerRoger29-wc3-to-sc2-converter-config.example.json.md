---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "config.example.json"
ref: HEAD
---

{
  "mdx": "MyModel.mdx",
  "out_dir": "out",
  "model_name": "MyModel",
  "scale": 0.05,
  "blender": "",

  "asset_texture_dir": "Assets\\Textures\\",
  "texture_search_dirs": [".", "Textures"],

  "textures": {
    "0": { "out": "MyModel_body.dds", "alpha_invert": false },
    "2": { "out": "MyModel_fx.dds", "glow": true }
  },

  "anim_names": {
    "Stand - 1": "Stand",
    "Stand - 2": "Stand 02",
    "Dissipate": "Death Disintegrate"
  },

  "attachments": null,

  "features": { "animations": true, "attachments": true, "particles": true, "hittest": true, "camera": true },
  "particle_rate_scale": 1.0,
  "particle_size_scale": 1.0,
  "team_color": true
}
