---
type: vendor-doc
source_repo: "https://github.com/Slayer95/tree-sitter-jass"
source_path: "src/node-types.json"
ref: HEAD
---

[
  {
    "type": "ArrayElement",
    "named": true,
    "fields": {
      "array": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "VariableReference",
            "named": true
          }
        ]
      },
      "index": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "ArrayElement",
            "named": true
          },
          {
            "type": "BinaryExpression",
            "named": true
          },
          {
            "type": "CallExpression",
            "named": true
          },
          {
            "type": "CodeReference",
            "named": true
          },
          {
            "type": "Literal",
            "named": true
          },
          {
            "type": "NegativeExpression",
            "named": true
          },
          {
            "type": "NotExpression",
            "named": true
          },
          {
            "type": "ParenthesizedExpression",
            "named": true
          },
          {
            "type": "PositiveExpression",
            "named": true
          },
          {
            "type": "VariableReference",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "ArrayType",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "Identifier",
          "named": true
        }
      ]
    }
  },
  {
    "type": "AtomicType",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "Identifier",
          "named": true
        }
      ]
    }
  },
  {
    "type": "BinaryExpression",
    "named": true,
    "fields": {
      "lhs": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "ArrayElement",
            "named": true
          },
          {
            "type": "BinaryExpression",
            "named": true
          },
          {
            "type": "CallExpression",
            "named": true
          },
          {
            "type": "CodeReference",
            "named": true
          },
          {
            "type": "Literal",
            "named": true
          },
          {
            "type": "NegativeExpression",
            "named": true
          },
          {
            "type": "NotExpression",
            "named": true
          },
          {
            "type": "ParenthesizedExpression",
            "named": true
          },
          {
            "type": "PositiveExpression",
            "named": true
          },
          {
            "type": "VariableReference",
            "named": true
          }
        ]
      },
      "operator": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Operator",
            "named": true
          }
        ]
      },
      "rhs": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "ArrayElement",
            "named": true
          },
          {
            "type": "BinaryExpression",
            "named": true
          },
          {
            "type": "CallExpression",
            "named": true
          },
          {
            "type": "CodeReference",
            "named": true
          },
          {
            "type": "Literal",
            "named": true
          },
          {
            "type": "NegativeExpression",
            "named": true
          },
          {
            "type": "NotExpression",
            "named": true
          },
          {
            "type": "ParenthesizedExpression",
            "named": true
          },
          {
            "type": "PositiveExpression",
            "named": true
          },
          {
            "type": "VariableReference",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "Boolean",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "False",
          "named": true
        },
        {
          "type": "True",
          "named": true
        }
      ]
    }
  },
  {
    "type": "CallExpression",
    "named": true,
    "fields": {
      "callee": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "FunctionReference",
            "named": true
          }
        ]
      }
    },
    "children": {
      "multiple": false,
      "required": false,
      "types": [
        {
          "type": "FunctionArgumentList",
          "named": true
        }
      ]
    }
  },
  {
    "type": "CallStatement",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "CallExpression",
          "named": true
        }
      ]
    }
  },
  {
    "type": "CodeReference",
    "named": true,
    "fields": {
      "funarg": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "FunctionReference",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "ExitWhenStatement",
    "named": true,
    "fields": {
      "test": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Test",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "False",
    "named": true,
    "fields": {}
  },
  {
    "type": "FunctionArgument",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "ArrayElement",
          "named": true
        },
        {
          "type": "BinaryExpression",
          "named": true
        },
        {
          "type": "CallExpression",
          "named": true
        },
        {
          "type": "CodeReference",
          "named": true
        },
        {
          "type": "Literal",
          "named": true
        },
        {
          "type": "NegativeExpression",
          "named": true
        },
        {
          "type": "NotExpression",
          "named": true
        },
        {
          "type": "ParenthesizedExpression",
          "named": true
        },
        {
          "type": "PositiveExpression",
          "named": true
        },
        {
          "type": "VariableReference",
          "named": true
        }
      ]
    }
  },
  {
    "type": "FunctionArgumentList",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "FunctionArgument",
          "named": true
        }
      ]
    }
  },
  {
    "type": "FunctionBody",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "CallStatement",
          "named": true
        },
        {
          "type": "LocalDeclarationStatement",
          "named": true
        },
        {
          "type": "LoopStatement",
          "named": true
        },
        {
          "type": "NewLine",
          "named": true
        },
        {
          "type": "RIfStatement",
          "named": true
        },
        {
          "type": "ReturnStatement",
          "named": true
        },
        {
          "type": "SetStatement",
          "named": true
        }
      ]
    }
  },
  {
    "type": "FunctionDeclaration",
    "named": true,
    "fields": {
      "signature": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "FunctionSignature",
            "named": true
          }
        ]
      }
    },
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "ConstantAttribute",
          "named": true
        },
        {
          "type": "FunctionBody",
          "named": true
        }
      ]
    }
  },
  {
    "type": "FunctionParameter",
    "named": true,
    "fields": {
      "name": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Identifier",
            "named": true
          }
        ]
      },
      "type": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "TypeReference",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "FunctionParameterList",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "FunctionParameter",
          "named": true
        }
      ]
    }
  },
  {
    "type": "FunctionSignature",
    "named": true,
    "fields": {
      "input": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Empty",
            "named": true
          },
          {
            "type": "FunctionParameterList",
            "named": true
          }
        ]
      },
      "name": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Identifier",
            "named": true
          }
        ]
      },
      "output": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "None",
            "named": true
          },
          {
            "type": "TypeReference",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "GlobalDeclarationStatement",
    "named": true,
    "fields": {
      "name": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Identifier",
            "named": true
          }
        ]
      },
      "type": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "ArrayType",
            "named": true
          },
          {
            "type": "AtomicType",
            "named": true
          }
        ]
      },
      "value": {
        "multiple": false,
        "required": false,
        "types": [
          {
            "type": "Initializer",
            "named": true
          }
        ]
      }
    },
    "children": {
      "multiple": false,
      "required": false,
      "types": [
        {
          "type": "ConstantAttribute",
          "named": true
        }
      ]
    }
  },
  {
    "type": "GlobalsBlock",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": false,
      "types": [
        {
          "type": "GlobalDeclarationStatement",
          "named": true
        },
        {
          "type": "NewLine",
          "named": true
        }
      ]
    }
  },
  {
    "type": "Initializer",
    "named": true,
    "fields": {
      "value": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "ArrayElement",
            "named": true
          },
          {
            "type": "BinaryExpression",
            "named": true
          },
          {
            "type": "CallExpression",
            "named": true
          },
          {
            "type": "CodeReference",
            "named": true
          },
          {
            "type": "Literal",
            "named": true
          },
          {
            "type": "NegativeExpression",
            "named": true
          },
          {
            "type": "NotExpression",
            "named": true
          },
          {
            "type": "ParenthesizedExpression",
            "named": true
          },
          {
            "type": "PositiveExpression",
            "named": true
          },
          {
            "type": "VariableReference",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "LAlternate",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "CallStatement",
          "named": true
        },
        {
          "type": "ExitWhenStatement",
          "named": true
        },
        {
          "type": "LIfStatement",
          "named": true
        },
        {
          "type": "LoopStatement",
          "named": true
        },
        {
          "type": "NewLine",
          "named": true
        },
        {
          "type": "ReturnStatement",
          "named": true
        },
        {
          "type": "SetStatement",
          "named": true
        }
      ]
    }
  },
  {
    "type": "LConsequent",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "CallStatement",
          "named": true
        },
        {
          "type": "ExitWhenStatement",
          "named": true
        },
        {
          "type": "LIfStatement",
          "named": true
        },
        {
          "type": "LoopStatement",
          "named": true
        },
        {
          "type": "NewLine",
          "named": true
        },
        {
          "type": "ReturnStatement",
          "named": true
        },
        {
          "type": "SetStatement",
          "named": true
        }
      ]
    }
  },
  {
    "type": "LElseIfStatement",
    "named": true,
    "fields": {
      "test": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Test",
            "named": true
          }
        ]
      }
    },
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "LAlternate",
          "named": true
        }
      ]
    }
  },
  {
    "type": "LElseStatement",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "LAlternate",
          "named": true
        }
      ]
    }
  },
  {
    "type": "LIfStatement",
    "named": true,
    "fields": {
      "test": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Test",
            "named": true
          }
        ]
      }
    },
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "LConsequent",
          "named": true
        },
        {
          "type": "LElseIfStatement",
          "named": true
        },
        {
          "type": "LElseStatement",
          "named": true
        }
      ]
    }
  },
  {
    "type": "Literal",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "Boolean",
          "named": true
        },
        {
          "type": "DecimalInteger",
          "named": true
        },
        {
          "type": "FourCC",
          "named": true
        },
        {
          "type": "HexInteger",
          "named": true
        },
        {
          "type": "Null",
          "named": true
        },
        {
          "type": "OctalInteger",
          "named": true
        },
        {
          "type": "Real",
          "named": true
        },
        {
          "type": "SignedChar",
          "named": true
        },
        {
          "type": "String",
          "named": true
        }
      ]
    }
  },
  {
    "type": "LocalDeclarationStatement",
    "named": true,
    "fields": {
      "name": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Identifier",
            "named": true
          }
        ]
      },
      "type": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "ArrayType",
            "named": true
          },
          {
            "type": "AtomicType",
            "named": true
          }
        ]
      },
      "value": {
        "multiple": false,
        "required": false,
        "types": [
          {
            "type": "Initializer",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "LoopStatement",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "CallStatement",
          "named": true
        },
        {
          "type": "ExitWhenStatement",
          "named": true
        },
        {
          "type": "LIfStatement",
          "named": true
        },
        {
          "type": "LoopStatement",
          "named": true
        },
        {
          "type": "NewLine",
          "named": true
        },
        {
          "type": "ReturnStatement",
          "named": true
        },
        {
          "type": "SetStatement",
          "named": true
        }
      ]
    }
  },
  {
    "type": "NativeDeclaration",
    "named": true,
    "fields": {
      "signature": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "FunctionSignature",
            "named": true
          }
        ]
      }
    },
    "children": {
      "multiple": false,
      "required": false,
      "types": [
        {
          "type": "ConstantAttribute",
          "named": true
        }
      ]
    }
  },
  {
    "type": "NegativeExpression",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "ArrayElement",
          "named": true
        },
        {
          "type": "BinaryExpression",
          "named": true
        },
        {
          "type": "CallExpression",
          "named": true
        },
        {
          "type": "CodeReference",
          "named": true
        },
        {
          "type": "Literal",
          "named": true
        },
        {
          "type": "NegativeExpression",
          "named": true
        },
        {
          "type": "NotExpression",
          "named": true
        },
        {
          "type": "ParenthesizedExpression",
          "named": true
        },
        {
          "type": "PositiveExpression",
          "named": true
        },
        {
          "type": "VariableReference",
          "named": true
        }
      ]
    }
  },
  {
    "type": "NotExpression",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "ArrayElement",
          "named": true
        },
        {
          "type": "BinaryExpression",
          "named": true
        },
        {
          "type": "CallExpression",
          "named": true
        },
        {
          "type": "CodeReference",
          "named": true
        },
        {
          "type": "Literal",
          "named": true
        },
        {
          "type": "NegativeExpression",
          "named": true
        },
        {
          "type": "NotExpression",
          "named": true
        },
        {
          "type": "ParenthesizedExpression",
          "named": true
        },
        {
          "type": "PositiveExpression",
          "named": true
        },
        {
          "type": "VariableReference",
          "named": true
        }
      ]
    }
  },
  {
    "type": "Null",
    "named": true,
    "fields": {}
  },
  {
    "type": "ParenthesizedExpression",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "ArrayElement",
          "named": true
        },
        {
          "type": "BinaryExpression",
          "named": true
        },
        {
          "type": "CallExpression",
          "named": true
        },
        {
          "type": "CodeReference",
          "named": true
        },
        {
          "type": "Literal",
          "named": true
        },
        {
          "type": "NegativeExpression",
          "named": true
        },
        {
          "type": "NotExpression",
          "named": true
        },
        {
          "type": "ParenthesizedExpression",
          "named": true
        },
        {
          "type": "PositiveExpression",
          "named": true
        },
        {
          "type": "VariableReference",
          "named": true
        }
      ]
    }
  },
  {
    "type": "PositiveExpression",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "ArrayElement",
          "named": true
        },
        {
          "type": "BinaryExpression",
          "named": true
        },
        {
          "type": "CallExpression",
          "named": true
        },
        {
          "type": "CodeReference",
          "named": true
        },
        {
          "type": "Literal",
          "named": true
        },
        {
          "type": "NegativeExpression",
          "named": true
        },
        {
          "type": "NotExpression",
          "named": true
        },
        {
          "type": "ParenthesizedExpression",
          "named": true
        },
        {
          "type": "PositiveExpression",
          "named": true
        },
        {
          "type": "VariableReference",
          "named": true
        }
      ]
    }
  },
  {
    "type": "RAlternate",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "CallStatement",
          "named": true
        },
        {
          "type": "LoopStatement",
          "named": true
        },
        {
          "type": "NewLine",
          "named": true
        },
        {
          "type": "RIfStatement",
          "named": true
        },
        {
          "type": "ReturnStatement",
          "named": true
        },
        {
          "type": "SetStatement",
          "named": true
        }
      ]
    }
  },
  {
    "type": "RConsequent",
    "named": true,
    "fields": {},
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "CallStatement",
          "named": true
        },
        {
          "type": "LoopStatement",
          "named": true
        },
        {
          "type": "NewLine",
          "named": true
        },
        {
          "type": "RIfStatement",
          "named": true
        },
        {
          "type": "ReturnStatement",
          "named": true
        },
        {
          "type": "SetStatement",
          "named": true
        }
      ]
    }
  },
  {
    "type": "RElseIfStatement",
    "named": true,
    "fields": {
      "test": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Test",
            "named": true
          }
        ]
      }
    },
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "RAlternate",
          "named": true
        }
      ]
    }
  },
  {
    "type": "RElseStatement",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "RAlternate",
          "named": true
        }
      ]
    }
  },
  {
    "type": "RIfStatement",
    "named": true,
    "fields": {
      "test": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Test",
            "named": true
          }
        ]
      }
    },
    "children": {
      "multiple": true,
      "required": true,
      "types": [
        {
          "type": "RConsequent",
          "named": true
        },
        {
          "type": "RElseIfStatement",
          "named": true
        },
        {
          "type": "RElseStatement",
          "named": true
        }
      ]
    }
  },
  {
    "type": "ReturnStatement",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": false,
      "types": [
        {
          "type": "ArrayElement",
          "named": true
        },
        {
          "type": "BinaryExpression",
          "named": true
        },
        {
          "type": "CallExpression",
          "named": true
        },
        {
          "type": "CodeReference",
          "named": true
        },
        {
          "type": "Literal",
          "named": true
        },
        {
          "type": "NegativeExpression",
          "named": true
        },
        {
          "type": "NotExpression",
          "named": true
        },
        {
          "type": "ParenthesizedExpression",
          "named": true
        },
        {
          "type": "PositiveExpression",
          "named": true
        },
        {
          "type": "VariableReference",
          "named": true
        }
      ]
    }
  },
  {
    "type": "SetStatement",
    "named": true,
    "fields": {
      "binding": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "ArrayElement",
            "named": true
          },
          {
            "type": "VariableReference",
            "named": true
          }
        ]
      },
      "value": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Initializer",
            "named": true
          }
        ]
      }
    }
  },
  {
    "type": "Test",
    "named": true,
    "fields": {},
    "children": {
      "multiple": false,
      "required": true,
      "types": [
        {
          "type": "ArrayElement",
          "named": true
        },
        {
          "type": "BinaryExpression",
          "named": true
        },
        {
          "type": "CallExpression",
          "named": true
        },
        {
          "type": "CodeReference",
          "named": true
        },
        {
          "type": "Literal",
          "named": true
        },
        {
          "type": "NegativeExpression",
          "named": true
        },
        {
          "type": "NotExpression",
          "named": true
        },
        {
          "type": "ParenthesizedExpression",
          "named": true
        },
        {
          "type": "PositiveExpression",
          "named": true
        },
        {
          "type": "VariableReference",
          "named": true
        }
      ]
    }
  },
  {
    "type": "True",
    "named": true,
    "fields": {}
  },
  {
    "type": "TypeDeclaration",
    "named": true,
    "fields": {
      "name": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "Identifier",
            "named": true
          }
        ]
      },
      "super": {
        "multiple": false,
        "required": true,
        "types": [
          {
            "type": "TypeReference",
            "named": true
          }
        ]
      }
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
          "type": "BOM",
          "named": true
        },
        {
          "type": "FunctionDeclaration",
          "named": true
        },
        {
          "type": "GlobalsBlock",
          "named": true
        },
        {
          "type": "NativeDeclaration",
          "named": true
        },
        {
          "type": "NewLine",
          "named": true
        },
        {
          "type": "TypeDeclaration",
          "named": true
        }
      ]
    }
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
    "type": "+",
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
    "type": "=",
    "named": false
  },
  {
    "type": "BOM",
    "named": true
  },
  {
    "type": "Comment",
    "named": true,
    "extra": true
  },
  {
    "type": "ConstantAttribute",
    "named": true
  },
  {
    "type": "DecimalInteger",
    "named": true
  },
  {
    "type": "Empty",
    "named": true
  },
  {
    "type": "FourCC",
    "named": true
  },
  {
    "type": "FunctionReference",
    "named": true
  },
  {
    "type": "HexInteger",
    "named": true
  },
  {
    "type": "Identifier",
    "named": true
  },
  {
    "type": "NewLine",
    "named": true
  },
  {
    "type": "None",
    "named": true
  },
  {
    "type": "OctalInteger",
    "named": true
  },
  {
    "type": "Operator",
    "named": true
  },
  {
    "type": "Real",
    "named": true
  },
  {
    "type": "SignedChar",
    "named": true
  },
  {
    "type": "String",
    "named": true
  },
  {
    "type": "TypeReference",
    "named": true
  },
  {
    "type": "VariableReference",
    "named": true
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
    "type": "array",
    "named": false
  },
  {
    "type": "call",
    "named": false
  },
  {
    "type": "else",
    "named": false
  },
  {
    "type": "elseif",
    "named": false
  },
  {
    "type": "endfunction",
    "named": false
  },
  {
    "type": "endglobals",
    "named": false
  },
  {
    "type": "endif",
    "named": false
  },
  {
    "type": "endloop",
    "named": false
  },
  {
    "type": "exitwhen",
    "named": false
  },
  {
    "type": "extends",
    "named": false
  },
  {
    "type": "false",
    "named": false
  },
  {
    "type": "function",
    "named": false
  },
  {
    "type": "globals",
    "named": false
  },
  {
    "type": "if",
    "named": false
  },
  {
    "type": "local",
    "named": false
  },
  {
    "type": "loop",
    "named": false
  },
  {
    "type": "native",
    "named": false
  },
  {
    "type": "not",
    "named": false
  },
  {
    "type": "null",
    "named": false
  },
  {
    "type": "return",
    "named": false
  },
  {
    "type": "returns",
    "named": false
  },
  {
    "type": "set",
    "named": false
  },
  {
    "type": "takes",
    "named": false
  },
  {
    "type": "then",
    "named": false
  },
  {
    "type": "true",
    "named": false
  },
  {
    "type": "type",
    "named": false
  }
]