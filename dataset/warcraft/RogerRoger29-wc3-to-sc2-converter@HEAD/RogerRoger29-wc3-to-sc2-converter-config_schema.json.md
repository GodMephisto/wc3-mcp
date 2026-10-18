---
type: vendor-doc
source_repo: "https://github.com/RogerRoger29/wc3-to-sc2-converter"
source_path: "config_schema.json"
ref: HEAD
---

{
  "$schema": "http://json-schema.org/draft-07/schema#",
  "title": "WC3 to SC2 Converter Config",
  "type": "object",
  "required": ["mdx"],
  "properties": {
    "mdx": {
      "type": "string",
      "description": "Path to the .mdx model file (relative to the config file)"
    },
    "out_dir": {
      "type": "string",
      "description": "Output directory for .m3 + .dds files",
      "default": "Converted"
    },
    "model_name": {
      "type": "string",
      "description": "Base name for .m3 and internal names (default: mdx filename)"
    },
    "scale": {
      "type": "number",
      "description": "World scale (WC3 units are large; ~0.04-0.06 is typical)",
      "default": 1.0,
      "minimum": 0.001,
      "maximum": 100.0
    },
    "blender": {
      "type": "string",
      "description": "Path to blender.exe (auto-detected if empty or omitted)"
    },
    "asset_texture_dir": {
      "type": "string",
      "description": "SC2 asset path for texture references in .m3",
      "default": "Assets\\Textures\\"
    },
    "texture_search_dirs": {
      "type": "array",
      "description": "Directories to search for .blp texture files",
      "items": {"type": "string"}
    },
    "textures": {
      "type": "object",
      "description": "Per-texture overrides keyed by TEXS index (as string)",
      "additionalProperties": {
        "type": "object",
        "properties": {
          "out": {"type": "string", "description": "Output .dds filename"},
          "src": {"type": "string", "description": "Explicit source file path"},
          "glow": {"type": "boolean", "description": "Apply particle glow treatment"},
          "alpha_invert": {"type": "boolean", "description": "Invert alpha channel"},
          "glow_dim": {"type": "number", "description": "Glow brightness multiplier (0-1)", "minimum": 0.0, "maximum": 1.0}
        }
      }
    },
    "anim_names": {
      "type": "object",
      "description": "Override WC3 sequence name -> SC2 animation token",
      "additionalProperties": {"type": "string"}
    },
    "attachments": {
      "description": "null = auto (Ref_Origin/Ref_Center), or list of {name, bone}",
      "oneOf": [
        {"type": "null"},
        {
          "type": "array",
          "items": {
            "type": "object",
            "required": ["name"],
            "properties": {
              "name": {"type": "string"},
              "bone": {"type": "string"}
            }
          }
        }
      ]
    },
    "features": {
      "type": "object",
      "description": "Enable/disable specific conversion features",
      "properties": {
        "animations": {"type": "boolean", "default": true},
        "attachments": {"type": "boolean", "default": true},
        "particles": {"type": "boolean", "default": true},
        "hittest": {"type": "boolean", "default": true},
        "camera": {"type": "boolean", "default": true}
      }
    },
    "particle_rate_scale": {
      "type": "number",
      "description": "Multiply all emission rates",
      "default": 1.0,
      "minimum": 0.0
    },
    "particle_size_scale": {
      "type": "number",
      "description": "Multiply all particle sizes",
      "default": 1.0,
      "minimum": 0.0
    },
    "team_color": {
      "type": "boolean",
      "description": "Enable team colour emissive layer",
      "default": true
    }
  }
}
