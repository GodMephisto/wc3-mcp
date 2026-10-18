---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "src/node-types.json"
ref: HEAD
---

[
  {
    "type": "expr",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": false,
      "types": [
        {
          "type": "expr",
          "named": true
        },
        {
          "type": "float",
          "named": true
        },
        {
          "type": "id",
          "named": true
        },
        {
          "type": "number",
          "named": true
        },
        {
          "type": "string",
          "named": true
        }
      ]
    }
  },
  {
    "type": "globals",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "endglobals_",
          "named": true
        },
        {
          "type": "expr",
          "named": true
        },
        {
          "type": "globals",
          "named": true
        },
        {
          "type": "globals_",
          "named": true
        },
        {
          "type": "loop",
          "named": true
        },
        {
          "type": "struct",
          "named": true
        }
      ]
    }
  },
  {
    "type": "loop",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "endloop_",
          "named": true
        },
        {
          "type": "expr",
          "named": true
        },
        {
          "type": "globals",
          "named": true
        },
        {
          "type": "loop",
          "named": true
        },
        {
          "type": "loop_",
          "named": true
        },
        {
          "type": "struct",
          "named": true
        }
      ]
    }
  },
  {
    "type": "program",
    "named": true,
    "root": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": false,
      "types": [
        {
          "type": "expr",
          "named": true
        },
        {
          "type": "globals",
          "named": true
        },
        {
          "type": "loop",
          "named": true
        },
        {
          "type": "struct",
          "named": true
        }
      ]
    }
  },
  {
    "type": "string",
    "named": true,
    "fields": {
      "content": {
        "multiple": false,
        "required": false,
        "types": [
          {
            "type": "string_content",
            "named": false
          }
        ]
      },
      "end": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "string_end",
            "named": false
          }
        ]
      },
      "start": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "string_start",
            "named": false
          }
        ]
      }
    }
  },
  {
    "type": "struct",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "endstruct_",
          "named": true
        },
        {
          "type": "expr",
          "named": true
        },
        {
          "type": "globals",
          "named": true
        },
        {
          "type": "loop",
          "named": true
        },
        {
          "type": "struct",
          "named": true
        },
        {
          "type": "struct_",
          "named": true
        }
      ]
    }
  },
  {
    "type": "!=",
    "named": false
  },
  {
    "type": "(",
    "named": false
  },
  {
    "type": ")",
    "named": false
  },
  {
    "type": "*",
    "named": false
  },
  {
    "type": "+",
    "named": false
  },
  {
    "type": "++",
    "named": false
  },
  {
    "type": ",",
    "named": false
  },
  {
    "type": "-",
    "named": false
  },
  {
    "type": "--",
    "named": false
  },
  {
    "type": ".",
    "named": false
  },
  {
    "type": "/",
    "named": false
  },
  {
    "type": "<",
    "named": false
  },
  {
    "type": "<=",
    "named": false
  },
  {
    "type": "=",
    "named": false
  },
  {
    "type": "==",
    "named": false
  },
  {
    "type": ">",
    "named": false
  },
  {
    "type": ">=",
    "named": false
  },
  {
    "type": "[",
    "named": false
  },
  {
    "type": "]",
    "named": false
  },
  {
    "type": "and",
    "named": false
  },
  {
    "type": "comment_content",
    "named": false
  },
  {
    "type": "comment_end",
    "named": false
  },
  {
    "type": "comment_start",
    "named": false
  },
  {
    "type": "endglobals_",
    "named": true
  },
  {
    "type": "endloop_",
    "named": true
  },
  {
    "type": "endstruct_",
    "named": true
  },
  {
    "type": "float",
    "named": true
  },
  {
    "type": "globals_",
    "named": true
  },
  {
    "type": "id",
    "named": true
  },
  {
    "type": "loop_",
    "named": true
  },
  {
    "type": "not",
    "named": false
  },
  {
    "type": "number",
    "named": true
  },
  {
    "type": "or",
    "named": false
  },
  {
    "type": "string_content",
    "named": false
  },
  {
    "type": "string_end",
    "named": false
  },
  {
    "type": "string_start",
    "named": false
  },
  {
    "type": "struct_",
    "named": true
  }
]