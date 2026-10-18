---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "src/grammar.json"
ref: HEAD
---

{
  "$schema": "https://tree-sitter.github.io/tree-sitter/assets/schemas/grammar.schema.json",
  "name": "jass",
  "word": "Identifier",
  "rules": {
    "program": {
      "type": "SEQ",
      "members": [
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "BOM"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "TopLevel"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "TopLevel"
            },
            {
              "type": "BLANK"
            }
          ]
        }
      ]
    },
    "NewLine": {
      "type": "PATTERN",
      "value": "\\r?\\n|\\r"
    },
    "Identifier": {
      "type": "PATTERN",
      "value": "[a-zA-Z]([a-zA-Z0-9_]*)"
    },
    "AtomicType": {
      "type": "SYMBOL",
      "name": "Identifier"
    },
    "ArrayType": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "Identifier"
        },
        {
          "type": "STRING",
          "value": "array"
        }
      ]
    },
    "ArrayElement": {
      "type": "SEQ",
      "members": [
        {
          "type": "FIELD",
          "name": "array",
          "content": {
            "type": "ALIAS",
            "content": {
              "type": "SYMBOL",
              "name": "Identifier"
            },
            "named": true,
            "value": "VariableReference"
          }
        },
        {
          "type": "STRING",
          "value": "["
        },
        {
          "type": "FIELD",
          "name": "index",
          "content": {
            "type": "SYMBOL",
            "name": "Expression"
          }
        },
        {
          "type": "STRING",
          "value": "]"
        }
      ]
    },
    "BOM": {
      "type": "TOKEN",
      "content": {
        "type": "STRING",
        "value": "ï»¿"
      }
    },
    "Comment": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "STRING",
            "value": "//"
          },
          {
            "type": "PATTERN",
            "value": "[^\\n\\x00\\xFF]*"
          }
        ]
      }
    },
    "Null": {
      "type": "PREC",
      "value": 2,
      "content": {
        "type": "STRING",
        "value": "null"
      }
    },
    "True": {
      "type": "PREC",
      "value": 2,
      "content": {
        "type": "STRING",
        "value": "true"
      }
    },
    "False": {
      "type": "PREC",
      "value": 2,
      "content": {
        "type": "STRING",
        "value": "false"
      }
    },
    "Boolean": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "True"
        },
        {
          "type": "SYMBOL",
          "name": "False"
        }
      ]
    },
    "String": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "STRING",
            "value": "\""
          },
          {
            "type": "REPEAT",
            "content": {
              "type": "CHOICE",
              "members": [
                {
                  "type": "PATTERN",
                  "value": "[^\"\\\\\\x00\\xFF]"
                },
                {
                  "type": "PATTERN",
                  "value": "\\\\(\\\\|n|r|t|f|b|\"|')"
                }
              ]
            }
          },
          {
            "type": "STRING",
            "value": "\""
          }
        ]
      }
    },
    "DecimalInteger": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "STRING",
                "value": "-"
              },
              {
                "type": "BLANK"
              }
            ]
          },
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "STRING",
                "value": "0"
              },
              {
                "type": "PATTERN",
                "value": "[1-9]([0-9]*)"
              }
            ]
          }
        ]
      }
    },
    "OctalInteger": {
      "type": "PATTERN",
      "value": "-?0([0-7]+)"
    },
    "HexInteger": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "STRING",
                "value": "-"
              },
              {
                "type": "BLANK"
              }
            ]
          },
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "STRING",
                "value": "0x"
              },
              {
                "type": "STRING",
                "value": "0X"
              },
              {
                "type": "STRING",
                "value": "$"
              }
            ]
          },
          {
            "type": "PATTERN",
            "value": "[0-9a-fA-F]+"
          }
        ]
      }
    },
    "Integer": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "DecimalInteger"
        },
        {
          "type": "SYMBOL",
          "name": "OctalInteger"
        },
        {
          "type": "SYMBOL",
          "name": "HexInteger"
        }
      ]
    },
    "Real": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "STRING",
                "value": "-"
              },
              {
                "type": "BLANK"
              }
            ]
          },
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "PATTERN",
                "value": "\\.[0-9]+"
              },
              {
                "type": "PATTERN",
                "value": "[0-9]+\\.[0-9]*"
              }
            ]
          }
        ]
      }
    },
    "SignedChar": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "STRING",
            "value": "'"
          },
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "PATTERN",
                "value": "[^'\\\\\\x00\\xFF]"
              },
              {
                "type": "PATTERN",
                "value": "\\\\(\\\\|n|r|t|f|b|\"|')"
              }
            ]
          },
          {
            "type": "STRING",
            "value": "'"
          }
        ]
      }
    },
    "FourCC": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "STRING",
            "value": "'"
          },
          {
            "type": "PATTERN",
            "value": "[^'\\\\\\x00\\xFF]"
          },
          {
            "type": "PATTERN",
            "value": "[^'\\\\\\x00\\xFF]"
          },
          {
            "type": "PATTERN",
            "value": "[^'\\\\\\x00\\xFF]"
          },
          {
            "type": "PATTERN",
            "value": "[^'\\\\\\x00\\xFF]"
          },
          {
            "type": "STRING",
            "value": "'"
          }
        ]
      }
    },
    "Literal": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "Null"
        },
        {
          "type": "SYMBOL",
          "name": "Boolean"
        },
        {
          "type": "SYMBOL",
          "name": "String"
        },
        {
          "type": "SYMBOL",
          "name": "Integer"
        },
        {
          "type": "SYMBOL",
          "name": "Real"
        },
        {
          "type": "SYMBOL",
          "name": "FourCC"
        },
        {
          "type": "SYMBOL",
          "name": "SignedChar"
        }
      ]
    },
    "TopLevel": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "TypeDeclaration"
        },
        {
          "type": "SYMBOL",
          "name": "NativeDeclaration"
        },
        {
          "type": "SYMBOL",
          "name": "FunctionDeclaration"
        },
        {
          "type": "SYMBOL",
          "name": "GlobalsBlock"
        }
      ]
    },
    "TypeDeclaration": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "type"
        },
        {
          "type": "FIELD",
          "name": "name",
          "content": {
            "type": "SYMBOL",
            "name": "Identifier"
          }
        },
        {
          "type": "STRING",
          "value": "extends"
        },
        {
          "type": "FIELD",
          "name": "super",
          "content": {
            "type": "ALIAS",
            "content": {
              "type": "SYMBOL",
              "name": "Identifier"
            },
            "named": true,
            "value": "TypeReference"
          }
        }
      ]
    },
    "GlobalsBlock": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "globals"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "GlobalDeclarationStatement"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        },
        {
          "type": "STRING",
          "value": "endglobals"
        }
      ]
    },
    "GlobalDeclarationStatement": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SEQ",
          "members": [
            {
              "type": "CHOICE",
              "members": [
                {
                  "type": "SYMBOL",
                  "name": "DeclareAttributes"
                },
                {
                  "type": "BLANK"
                }
              ]
            },
            {
              "type": "FIELD",
              "name": "type",
              "content": {
                "type": "SYMBOL",
                "name": "AtomicType"
              }
            },
            {
              "type": "FIELD",
              "name": "name",
              "content": {
                "type": "SYMBOL",
                "name": "Identifier"
              }
            },
            {
              "type": "CHOICE",
              "members": [
                {
                  "type": "FIELD",
                  "name": "value",
                  "content": {
                    "type": "SYMBOL",
                    "name": "Initializer"
                  }
                },
                {
                  "type": "BLANK"
                }
              ]
            }
          ]
        },
        {
          "type": "SEQ",
          "members": [
            {
              "type": "FIELD",
              "name": "type",
              "content": {
                "type": "SYMBOL",
                "name": "ArrayType"
              }
            },
            {
              "type": "FIELD",
              "name": "name",
              "content": {
                "type": "SYMBOL",
                "name": "Identifier"
              }
            }
          ]
        }
      ]
    },
    "FunctionSignature": {
      "type": "SEQ",
      "members": [
        {
          "type": "FIELD",
          "name": "name",
          "content": {
            "type": "SYMBOL",
            "name": "Identifier"
          }
        },
        {
          "type": "STRING",
          "value": "takes"
        },
        {
          "type": "FIELD",
          "name": "input",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "ALIAS",
                "content": {
                  "type": "STRING",
                  "value": "nothing"
                },
                "named": true,
                "value": "Empty"
              },
              {
                "type": "SYMBOL",
                "name": "FunctionParameterList"
              }
            ]
          }
        },
        {
          "type": "STRING",
          "value": "returns"
        },
        {
          "type": "FIELD",
          "name": "output",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "ALIAS",
                "content": {
                  "type": "STRING",
                  "value": "nothing"
                },
                "named": true,
                "value": "None"
              },
              {
                "type": "ALIAS",
                "content": {
                  "type": "SYMBOL",
                  "name": "Identifier"
                },
                "named": true,
                "value": "TypeReference"
              }
            ]
          }
        }
      ]
    },
    "FunctionParameterList": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "FunctionParameter"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "STRING",
                "value": ","
              },
              {
                "type": "SYMBOL",
                "name": "FunctionParameter"
              }
            ]
          }
        }
      ]
    },
    "FunctionParameter": {
      "type": "SEQ",
      "members": [
        {
          "type": "FIELD",
          "name": "type",
          "content": {
            "type": "ALIAS",
            "content": {
              "type": "SYMBOL",
              "name": "Identifier"
            },
            "named": true,
            "value": "TypeReference"
          }
        },
        {
          "type": "FIELD",
          "name": "name",
          "content": {
            "type": "SYMBOL",
            "name": "Identifier"
          }
        }
      ]
    },
    "NativeDeclaration": {
      "type": "SEQ",
      "members": [
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "NativeAttributes"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "STRING",
          "value": "native"
        },
        {
          "type": "FIELD",
          "name": "signature",
          "content": {
            "type": "SYMBOL",
            "name": "FunctionSignature"
          }
        }
      ]
    },
    "NativeAttributes": {
      "type": "SYMBOL",
      "name": "ConstantAttribute"
    },
    "FunctionDeclaration": {
      "type": "SEQ",
      "members": [
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "FunctionAttributes"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "STRING",
          "value": "function"
        },
        {
          "type": "FIELD",
          "name": "signature",
          "content": {
            "type": "SYMBOL",
            "name": "FunctionSignature"
          }
        },
        {
          "type": "SYMBOL",
          "name": "FunctionBody"
        },
        {
          "type": "STRING",
          "value": "endfunction"
        }
      ]
    },
    "FunctionAttributes": {
      "type": "SYMBOL",
      "name": "ConstantAttribute"
    },
    "FunctionBody": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "NewLine"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "LocalDeclarationStatement"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "RStatement"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        }
      ]
    },
    "LocalDeclarationStatement": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SEQ",
          "members": [
            {
              "type": "STRING",
              "value": "local"
            },
            {
              "type": "FIELD",
              "name": "type",
              "content": {
                "type": "SYMBOL",
                "name": "AtomicType"
              }
            },
            {
              "type": "FIELD",
              "name": "name",
              "content": {
                "type": "SYMBOL",
                "name": "Identifier"
              }
            },
            {
              "type": "CHOICE",
              "members": [
                {
                  "type": "FIELD",
                  "name": "value",
                  "content": {
                    "type": "SYMBOL",
                    "name": "Initializer"
                  }
                },
                {
                  "type": "BLANK"
                }
              ]
            }
          ]
        },
        {
          "type": "SEQ",
          "members": [
            {
              "type": "STRING",
              "value": "local"
            },
            {
              "type": "FIELD",
              "name": "type",
              "content": {
                "type": "SYMBOL",
                "name": "ArrayType"
              }
            },
            {
              "type": "FIELD",
              "name": "name",
              "content": {
                "type": "SYMBOL",
                "name": "Identifier"
              }
            }
          ]
        }
      ]
    },
    "DeclareAttributes": {
      "type": "SYMBOL",
      "name": "ConstantAttribute"
    },
    "ConstantAttribute": {
      "type": "STRING",
      "value": "constant"
    },
    "Initializer": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "="
        },
        {
          "type": "FIELD",
          "name": "value",
          "content": {
            "type": "SYMBOL",
            "name": "Expression"
          }
        }
      ]
    },
    "Test": {
      "type": "SYMBOL",
      "name": "Expression"
    },
    "RStatement": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "RIfStatement"
        },
        {
          "type": "SYMBOL",
          "name": "LoopStatement"
        },
        {
          "type": "SYMBOL",
          "name": "ReturnStatement"
        },
        {
          "type": "SYMBOL",
          "name": "SetStatement"
        },
        {
          "type": "SYMBOL",
          "name": "CallStatement"
        }
      ]
    },
    "LStatement": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "LIfStatement"
        },
        {
          "type": "SYMBOL",
          "name": "LoopStatement"
        },
        {
          "type": "SYMBOL",
          "name": "ReturnStatement"
        },
        {
          "type": "SYMBOL",
          "name": "SetStatement"
        },
        {
          "type": "SYMBOL",
          "name": "CallStatement"
        },
        {
          "type": "SYMBOL",
          "name": "ExitWhenStatement"
        }
      ]
    },
    "RIfStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "if"
        },
        {
          "type": "FIELD",
          "name": "test",
          "content": {
            "type": "SYMBOL",
            "name": "Test"
          }
        },
        {
          "type": "STRING",
          "value": "then"
        },
        {
          "type": "SYMBOL",
          "name": "RConsequent"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "SYMBOL",
            "name": "RElseIfStatement"
          }
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "RElseStatement"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "STRING",
          "value": "endif"
        }
      ]
    },
    "LIfStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "if"
        },
        {
          "type": "FIELD",
          "name": "test",
          "content": {
            "type": "SYMBOL",
            "name": "Test"
          }
        },
        {
          "type": "STRING",
          "value": "then"
        },
        {
          "type": "SYMBOL",
          "name": "LConsequent"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "SYMBOL",
            "name": "LElseIfStatement"
          }
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "LElseStatement"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "STRING",
          "value": "endif"
        }
      ]
    },
    "RElseIfStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "elseif"
        },
        {
          "type": "FIELD",
          "name": "test",
          "content": {
            "type": "SYMBOL",
            "name": "Test"
          }
        },
        {
          "type": "STRING",
          "value": "then"
        },
        {
          "type": "SYMBOL",
          "name": "RAlternate"
        }
      ]
    },
    "LElseIfStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "elseif"
        },
        {
          "type": "FIELD",
          "name": "test",
          "content": {
            "type": "SYMBOL",
            "name": "Test"
          }
        },
        {
          "type": "STRING",
          "value": "then"
        },
        {
          "type": "SYMBOL",
          "name": "LAlternate"
        }
      ]
    },
    "RElseStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "else"
        },
        {
          "type": "SYMBOL",
          "name": "RAlternate"
        }
      ]
    },
    "LElseStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "else"
        },
        {
          "type": "SYMBOL",
          "name": "LAlternate"
        }
      ]
    },
    "RConsequent": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "NewLine"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "RStatement"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        }
      ]
    },
    "LConsequent": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "NewLine"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "LStatement"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        }
      ]
    },
    "RAlternate": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "NewLine"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "RStatement"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        }
      ]
    },
    "LAlternate": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "NewLine"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "LStatement"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        }
      ]
    },
    "LoopStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "loop"
        },
        {
          "type": "SYMBOL",
          "name": "NewLine"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "LStatement"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "NewLine"
                  }
                ]
              },
              {
                "type": "SYMBOL",
                "name": "NewLine"
              }
            ]
          }
        },
        {
          "type": "STRING",
          "value": "endloop"
        }
      ]
    },
    "ExitWhenStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "exitwhen"
        },
        {
          "type": "FIELD",
          "name": "test",
          "content": {
            "type": "SYMBOL",
            "name": "Test"
          }
        }
      ]
    },
    "ReturnStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "return"
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "Expression"
            },
            {
              "type": "BLANK"
            }
          ]
        }
      ]
    },
    "SetStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "set"
        },
        {
          "type": "FIELD",
          "name": "binding",
          "content": {
            "type": "SYMBOL",
            "name": "Binding"
          }
        },
        {
          "type": "FIELD",
          "name": "value",
          "content": {
            "type": "SYMBOL",
            "name": "Initializer"
          }
        }
      ]
    },
    "Binding": {
      "type": "CHOICE",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "SYMBOL",
            "name": "Identifier"
          },
          "named": true,
          "value": "VariableReference"
        },
        {
          "type": "SYMBOL",
          "name": "ArrayElement"
        }
      ]
    },
    "CallStatement": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "call"
        },
        {
          "type": "SYMBOL",
          "name": "CallExpression"
        }
      ]
    },
    "CallExpression": {
      "type": "SEQ",
      "members": [
        {
          "type": "FIELD",
          "name": "callee",
          "content": {
            "type": "ALIAS",
            "content": {
              "type": "SYMBOL",
              "name": "Identifier"
            },
            "named": true,
            "value": "FunctionReference"
          }
        },
        {
          "type": "STRING",
          "value": "("
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "FunctionArgumentList"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "STRING",
          "value": ")"
        }
      ]
    },
    "FunctionArgumentList": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "FunctionArgument"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "STRING",
                "value": ","
              },
              {
                "type": "SYMBOL",
                "name": "FunctionArgument"
              }
            ]
          }
        }
      ]
    },
    "Expression": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "PrimaryExpression"
        },
        {
          "type": "SYMBOL",
          "name": "UnaryExpression"
        },
        {
          "type": "SYMBOL",
          "name": "BinaryExpression"
        }
      ]
    },
    "PrimaryExpression": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "Literal"
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "SYMBOL",
            "name": "Identifier"
          },
          "named": true,
          "value": "VariableReference"
        },
        {
          "type": "SYMBOL",
          "name": "CodeReference"
        },
        {
          "type": "SYMBOL",
          "name": "ArrayElement"
        },
        {
          "type": "SYMBOL",
          "name": "CallExpression"
        },
        {
          "type": "SYMBOL",
          "name": "ParenthesizedExpression"
        }
      ]
    },
    "UnaryExpression": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "NotExpression"
        },
        {
          "type": "SYMBOL",
          "name": "NegativeExpression"
        },
        {
          "type": "SYMBOL",
          "name": "PositiveExpression"
        }
      ]
    },
    "NotExpression": {
      "type": "PREC_RIGHT",
      "value": 30,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "STRING",
            "value": "not"
          },
          {
            "type": "SYMBOL",
            "name": "Expression"
          }
        ]
      }
    },
    "NegativeExpression": {
      "type": "PREC_RIGHT",
      "value": 30,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "STRING",
            "value": "-"
          },
          {
            "type": "SYMBOL",
            "name": "Expression"
          }
        ]
      }
    },
    "PositiveExpression": {
      "type": "PREC_RIGHT",
      "value": 30,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "STRING",
            "value": "+"
          },
          {
            "type": "SYMBOL",
            "name": "Expression"
          }
        ]
      }
    },
    "BinaryExpression": {
      "type": "CHOICE",
      "members": [
        {
          "type": "PREC_LEFT",
          "value": 1,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "and"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 2,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "or"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 10,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "=="
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 10,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "!="
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 15,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "<"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 15,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": ">"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 15,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "<="
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 15,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": ">="
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 20,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "+"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 20,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "-"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 21,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "*"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 21,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "/"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 21,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "FIELD",
                "name": "lhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              },
              {
                "type": "FIELD",
                "name": "operator",
                "content": {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "%"
                  },
                  "named": true,
                  "value": "Operator"
                }
              },
              {
                "type": "FIELD",
                "name": "rhs",
                "content": {
                  "type": "SYMBOL",
                  "name": "Expression"
                }
              }
            ]
          }
        }
      ]
    },
    "ParenthesizedExpression": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "("
        },
        {
          "type": "SYMBOL",
          "name": "Expression"
        },
        {
          "type": "STRING",
          "value": ")"
        }
      ]
    },
    "FunctionArgument": {
      "type": "SYMBOL",
      "name": "Expression"
    },
    "CodeReference": {
      "type": "SEQ",
      "members": [
        {
          "type": "STRING",
          "value": "function"
        },
        {
          "type": "FIELD",
          "name": "funarg",
          "content": {
            "type": "ALIAS",
            "content": {
              "type": "SYMBOL",
              "name": "Identifier"
            },
            "named": true,
            "value": "FunctionReference"
          }
        }
      ]
    }
  },
  "extras": [
    {
      "type": "PATTERN",
      "value": " |\\t"
    },
    {
      "type": "SYMBOL",
      "name": "Comment"
    }
  ],
  "conflicts": [
    [
      "FunctionBody"
    ]
  ],
  "precedences": [],
  "externals": [],
  "inline": [
    "TopLevel",
    "RStatement",
    "LStatement",
    "Binding",
    "Expression",
    "PrimaryExpression",
    "UnaryExpression",
    "Integer",
    "DeclareAttributes",
    "FunctionAttributes",
    "NativeAttributes"
  ],
  "supertypes": [],
  "reserved": {}
}