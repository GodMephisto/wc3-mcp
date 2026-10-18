---
type: vendor-doc
source_repo: "https://github.com/WarRaft/tree-sitter-vjass"
source_path: "src/grammar.json"
ref: HEAD
---

{
  "$schema": "https://tree-sitter.github.io/tree-sitter/assets/schemas/grammar.schema.json",
  "name": "vjass",
  "rules": {
    "program": {
      "type": "REPEAT",
      "content": {
        "type": "SYMBOL",
        "name": "_block"
      }
    },
    "id": {
      "type": "TOKEN",
      "content": {
        "type": "PREC",
        "value": -1,
        "content": {
          "type": "PATTERN",
          "value": "[a-zA-Z_][a-zA-Z0-9_]*"
        }
      }
    },
    "keywords": {
      "type": "CHOICE",
      "members": [
        {
          "type": "STRING",
          "value": "struct"
        },
        {
          "type": "STRING",
          "value": "endstruct"
        },
        {
          "type": "STRING",
          "value": "globals"
        },
        {
          "type": "STRING",
          "value": "endglobals"
        },
        {
          "type": "STRING",
          "value": "loop"
        },
        {
          "type": "STRING",
          "value": "endloop"
        }
      ]
    },
    "_block": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "globals"
        },
        {
          "type": "SYMBOL",
          "name": "struct"
        },
        {
          "type": "SYMBOL",
          "name": "loop"
        },
        {
          "type": "SYMBOL",
          "name": "expr"
        }
      ]
    },
    "struct": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "TOKEN",
            "content": {
              "type": "STRING",
              "value": "struct"
            }
          },
          "named": true,
          "value": "struct_"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "SYMBOL",
            "name": "_block"
          }
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "TOKEN",
            "content": {
              "type": "STRING",
              "value": "endstruct"
            }
          },
          "named": true,
          "value": "endstruct_"
        }
      ]
    },
    "globals": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "TOKEN",
            "content": {
              "type": "STRING",
              "value": "globals"
            }
          },
          "named": true,
          "value": "globals_"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "SYMBOL",
            "name": "_block"
          }
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "TOKEN",
            "content": {
              "type": "STRING",
              "value": "endglobals"
            }
          },
          "named": true,
          "value": "endglobals_"
        }
      ]
    },
    "loop": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "TOKEN",
            "content": {
              "type": "STRING",
              "value": "loop"
            }
          },
          "named": true,
          "value": "loop_"
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "SYMBOL",
            "name": "_block"
          }
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "TOKEN",
            "content": {
              "type": "STRING",
              "value": "endloop"
            }
          },
          "named": true,
          "value": "endloop_"
        }
      ]
    },
    "expr": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "id"
        },
        {
          "type": "SYMBOL",
          "name": "number"
        },
        {
          "type": "SYMBOL",
          "name": "float"
        },
        {
          "type": "SYMBOL",
          "name": "string"
        },
        {
          "type": "PREC_LEFT",
          "value": 2,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "."
              },
              {
                "type": "SYMBOL",
                "name": "expr"
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
                "type": "STRING",
                "value": "["
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "]"
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
                "type": "STRING",
                "value": "("
              },
              {
                "type": "REPEAT",
                "content": {
                  "type": "SYMBOL",
                  "name": "expr"
                }
              },
              {
                "type": "STRING",
                "value": ")"
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
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "++"
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
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "++"
              }
            ]
          }
        },
        {
          "type": "PREC_RIGHT",
          "value": 3,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "STRING",
                "value": "++"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_RIGHT",
          "value": 3,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "STRING",
                "value": "--"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_RIGHT",
          "value": 3,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "STRING",
                "value": "not"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_RIGHT",
          "value": 3,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "STRING",
                "value": "-"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_RIGHT",
          "value": 3,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "STRING",
                "value": "+"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 5,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "*"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 5,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "/"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 6,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "+"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 6,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "-"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 8,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "<"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 8,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": ">"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 8,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "<="
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 8,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": ">="
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 9,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "=="
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 9,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "!="
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 14,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "and"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 13,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "or"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_RIGHT",
          "value": 15,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "="
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        },
        {
          "type": "PREC_LEFT",
          "value": 16,
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": ","
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              }
            ]
          }
        }
      ]
    },
    "number": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "REPEAT1",
                    "content": {
                      "type": "PATTERN",
                      "value": "[0-9]+"
                    }
                  },
                  {
                    "type": "REPEAT",
                    "content": {
                      "type": "SEQ",
                      "members": [
                        {
                          "type": "STRING",
                          "value": "_"
                        },
                        {
                          "type": "REPEAT1",
                          "content": {
                            "type": "PATTERN",
                            "value": "[0-9]+"
                          }
                        }
                      ]
                    }
                  }
                ]
              },
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "PATTERN",
                    "value": "0[xX]"
                  },
                  {
                    "type": "SEQ",
                    "members": [
                      {
                        "type": "REPEAT1",
                        "content": {
                          "type": "PATTERN",
                          "value": "[0-9a-fA-F]"
                        }
                      },
                      {
                        "type": "REPEAT",
                        "content": {
                          "type": "SEQ",
                          "members": [
                            {
                              "type": "STRING",
                              "value": "_"
                            },
                            {
                              "type": "REPEAT1",
                              "content": {
                                "type": "PATTERN",
                                "value": "[0-9a-fA-F]"
                              }
                            }
                          ]
                        }
                      }
                    ]
                  }
                ]
              },
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "PATTERN",
                    "value": "0[bB]"
                  },
                  {
                    "type": "SEQ",
                    "members": [
                      {
                        "type": "REPEAT1",
                        "content": {
                          "type": "PATTERN",
                          "value": "[01]"
                        }
                      },
                      {
                        "type": "REPEAT",
                        "content": {
                          "type": "SEQ",
                          "members": [
                            {
                              "type": "STRING",
                              "value": "_"
                            },
                            {
                              "type": "REPEAT1",
                              "content": {
                                "type": "PATTERN",
                                "value": "[01]"
                              }
                            }
                          ]
                        }
                      }
                    ]
                  }
                ]
              }
            ]
          },
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "PATTERN",
                "value": "([lL]|[uU][lL]?)"
              },
              {
                "type": "BLANK"
              }
            ]
          }
        ]
      }
    },
    "float": {
      "type": "TOKEN",
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SEQ",
                    "members": [
                      {
                        "type": "REPEAT1",
                        "content": {
                          "type": "PATTERN",
                          "value": "[0-9]+"
                        }
                      },
                      {
                        "type": "REPEAT",
                        "content": {
                          "type": "SEQ",
                          "members": [
                            {
                              "type": "STRING",
                              "value": "_"
                            },
                            {
                              "type": "REPEAT1",
                              "content": {
                                "type": "PATTERN",
                                "value": "[0-9]+"
                              }
                            }
                          ]
                        }
                      }
                    ]
                  },
                  {
                    "type": "PATTERN",
                    "value": "[eE][+-]?[0-9]+"
                  },
                  {
                    "type": "CHOICE",
                    "members": [
                      {
                        "type": "PATTERN",
                        "value": "[fF]"
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
                    "type": "CHOICE",
                    "members": [
                      {
                        "type": "SEQ",
                        "members": [
                          {
                            "type": "REPEAT1",
                            "content": {
                              "type": "PATTERN",
                              "value": "[0-9]+"
                            }
                          },
                          {
                            "type": "REPEAT",
                            "content": {
                              "type": "SEQ",
                              "members": [
                                {
                                  "type": "STRING",
                                  "value": "_"
                                },
                                {
                                  "type": "REPEAT1",
                                  "content": {
                                    "type": "PATTERN",
                                    "value": "[0-9]+"
                                  }
                                }
                              ]
                            }
                          }
                        ]
                      },
                      {
                        "type": "BLANK"
                      }
                    ]
                  },
                  {
                    "type": "STRING",
                    "value": "."
                  },
                  {
                    "type": "REPEAT1",
                    "content": {
                      "type": "SEQ",
                      "members": [
                        {
                          "type": "REPEAT1",
                          "content": {
                            "type": "PATTERN",
                            "value": "[0-9]+"
                          }
                        },
                        {
                          "type": "REPEAT",
                          "content": {
                            "type": "SEQ",
                            "members": [
                              {
                                "type": "STRING",
                                "value": "_"
                              },
                              {
                                "type": "REPEAT1",
                                "content": {
                                  "type": "PATTERN",
                                  "value": "[0-9]+"
                                }
                              }
                            ]
                          }
                        }
                      ]
                    }
                  },
                  {
                    "type": "CHOICE",
                    "members": [
                      {
                        "type": "PATTERN",
                        "value": "[eE][+-]?[0-9]+"
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
                        "value": "[fF]"
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
                    "type": "SEQ",
                    "members": [
                      {
                        "type": "REPEAT1",
                        "content": {
                          "type": "PATTERN",
                          "value": "[0-9]+"
                        }
                      },
                      {
                        "type": "REPEAT",
                        "content": {
                          "type": "SEQ",
                          "members": [
                            {
                              "type": "STRING",
                              "value": "_"
                            },
                            {
                              "type": "REPEAT1",
                              "content": {
                                "type": "PATTERN",
                                "value": "[0-9]+"
                              }
                            }
                          ]
                        }
                      }
                    ]
                  },
                  {
                    "type": "PATTERN",
                    "value": "[fF]"
                  }
                ]
              }
            ]
          }
        ]
      }
    },
    "var_stmt": {
      "type": "PREC_RIGHT",
      "value": 1,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "REPEAT",
            "content": {
              "type": "CHOICE",
              "members": [
                {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "local"
                  },
                  "named": true,
                  "value": "local"
                },
                {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "constant"
                  },
                  "named": true,
                  "value": "constant"
                },
                {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "array"
                  },
                  "named": true,
                  "value": "array"
                }
              ]
            }
          },
          {
            "type": "FIELD",
            "name": "type",
            "content": {
              "type": "SYMBOL",
              "name": "id"
            }
          },
          {
            "type": "SYMBOL",
            "name": "var_decl"
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
                  "name": "var_decl"
                }
              ]
            }
          }
        ]
      }
    },
    "var_decl": {
      "type": "SEQ",
      "members": [
        {
          "type": "FIELD",
          "name": "name",
          "content": {
            "type": "SYMBOL",
            "name": "id"
          }
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SEQ",
              "members": [
                {
                  "type": "STRING",
                  "value": "="
                },
                {
                  "type": "SYMBOL",
                  "name": "expr"
                }
              ]
            },
            {
              "type": "BLANK"
            }
          ]
        }
      ]
    },
    "_var": {
      "type": "PREC",
      "value": 2,
      "content": {
        "type": "CHOICE",
        "members": [
          {
            "type": "SYMBOL",
            "name": "id"
          },
          {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "prefix_exp"
              },
              {
                "type": "STRING",
                "value": "["
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "STRING",
                "value": "]"
              }
            ]
          },
          {
            "type": "SEQ",
            "members": [
              {
                "type": "SYMBOL",
                "name": "prefix_exp"
              },
              {
                "type": "STRING",
                "value": "."
              },
              {
                "type": "SYMBOL",
                "name": "id"
              }
            ]
          }
        ]
      }
    },
    "var_list": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "_var"
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
                "name": "_var"
              }
            ]
          }
        }
      ]
    },
    "string": {
      "type": "SEQ",
      "members": [
        {
          "type": "FIELD",
          "name": "start",
          "content": {
            "type": "ALIAS",
            "content": {
              "type": "SYMBOL",
              "name": "_string_start"
            },
            "named": false,
            "value": "string_start"
          }
        },
        {
          "type": "FIELD",
          "name": "content",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "ALIAS",
                "content": {
                  "type": "SYMBOL",
                  "name": "_string_content"
                },
                "named": false,
                "value": "string_content"
              },
              {
                "type": "BLANK"
              }
            ]
          }
        },
        {
          "type": "FIELD",
          "name": "end",
          "content": {
            "type": "ALIAS",
            "content": {
              "type": "SYMBOL",
              "name": "_string_end"
            },
            "named": false,
            "value": "string_end"
          }
        }
      ]
    },
    "_statement": {
      "type": "PREC_RIGHT",
      "value": 15,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "SYMBOL",
                "name": "var_stmt"
              },
              {
                "type": "SYMBOL",
                "name": "function_call"
              },
              {
                "type": "SYMBOL",
                "name": "do_statement"
              },
              {
                "type": "SYMBOL",
                "name": "while_statement"
              },
              {
                "type": "SYMBOL",
                "name": "repeat_statement"
              },
              {
                "type": "SYMBOL",
                "name": "if_statement"
              },
              {
                "type": "SYMBOL",
                "name": "function_statement"
              }
            ]
          }
        ]
      }
    },
    "function_name": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "id"
        }
      ]
    },
    "function": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "function_start"
        },
        {
          "type": "SYMBOL",
          "name": "function_impl"
        }
      ]
    },
    "function_impl": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "("
          },
          "named": true,
          "value": "function_body_paren"
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "parameter_list"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": ")"
          },
          "named": true,
          "value": "function_body_paren"
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SYMBOL",
                "name": "_block"
              },
              {
                "type": "BLANK"
              }
            ]
          },
          "named": true,
          "value": "function_body"
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "end"
          },
          "named": true,
          "value": "function_end"
        }
      ]
    },
    "parameter_list": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SEQ",
          "members": [
            {
              "type": "PREC_LEFT",
              "value": -1,
              "content": {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "id"
                  },
                  {
                    "type": "REPEAT",
                    "content": {
                      "type": "SEQ",
                      "members": [
                        {
                          "type": "PATTERN",
                          "value": ",\\s*"
                        },
                        {
                          "type": "SYMBOL",
                          "name": "id"
                        }
                      ]
                    }
                  }
                ]
              }
            }
          ]
        }
      ]
    },
    "unary_operation": {
      "type": "PREC_LEFT",
      "value": 13,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "STRING",
                "value": "not"
              },
              {
                "type": "STRING",
                "value": "#"
              },
              {
                "type": "STRING",
                "value": "-"
              },
              {
                "type": "STRING",
                "value": "~"
              }
            ]
          },
          {
            "type": "SYMBOL",
            "name": "expr"
          }
        ]
      }
    },
    "_id_list": {
      "type": "PREC_RIGHT",
      "value": -1,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "SYMBOL",
            "name": "id"
          },
          {
            "type": "REPEAT",
            "content": {
              "type": "SEQ",
              "members": [
                {
                  "type": "PATTERN",
                  "value": ",\\s*"
                },
                {
                  "type": "SYMBOL",
                  "name": "id"
                }
              ]
            }
          }
        ]
      }
    },
    "return_statement": {
      "type": "PREC",
      "value": 2,
      "content": {
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
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "expr"
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
                          "name": "expr"
                        }
                      ]
                    }
                  }
                ]
              },
              {
                "type": "BLANK"
              }
            ]
          }
        ]
      }
    },
    "break_statement": {
      "type": "STRING",
      "value": "break"
    },
    "do_statement": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "do"
          },
          "named": true,
          "value": "do_start"
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "_block"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "end"
          },
          "named": true,
          "value": "do_end"
        }
      ]
    },
    "while_statement": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "while"
          },
          "named": true,
          "value": "while_start"
        },
        {
          "type": "SYMBOL",
          "name": "expr"
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "do"
          },
          "named": true,
          "value": "while_do"
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "_block"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "end"
          },
          "named": true,
          "value": "while_end"
        }
      ]
    },
    "repeat_statement": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "repeat"
          },
          "named": true,
          "value": "repeat_start"
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "_block"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "until"
          },
          "named": true,
          "value": "repeat_until"
        },
        {
          "type": "SYMBOL",
          "name": "expr"
        }
      ]
    },
    "if_statement": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "if"
          },
          "named": true,
          "value": "if_start"
        },
        {
          "type": "SYMBOL",
          "name": "expr"
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "then"
          },
          "named": true,
          "value": "if_then"
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SYMBOL",
              "name": "_block"
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "REPEAT",
          "content": {
            "type": "SEQ",
            "members": [
              {
                "type": "ALIAS",
                "content": {
                  "type": "STRING",
                  "value": "elseif"
                },
                "named": true,
                "value": "if_elseif"
              },
              {
                "type": "SYMBOL",
                "name": "expr"
              },
              {
                "type": "ALIAS",
                "content": {
                  "type": "STRING",
                  "value": "then"
                },
                "named": true,
                "value": "if_then"
              },
              {
                "type": "CHOICE",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "_block"
                  },
                  {
                    "type": "BLANK"
                  }
                ]
              }
            ]
          }
        },
        {
          "type": "CHOICE",
          "members": [
            {
              "type": "SEQ",
              "members": [
                {
                  "type": "ALIAS",
                  "content": {
                    "type": "STRING",
                    "value": "else"
                  },
                  "named": true,
                  "value": "if_else"
                },
                {
                  "type": "CHOICE",
                  "members": [
                    {
                      "type": "SYMBOL",
                      "name": "_block"
                    },
                    {
                      "type": "BLANK"
                    }
                  ]
                }
              ]
            },
            {
              "type": "BLANK"
            }
          ]
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "end"
          },
          "named": true,
          "value": "if_end"
        }
      ]
    },
    "function_start": {
      "type": "STRING",
      "value": "function"
    },
    "function_statement": {
      "type": "PREC_RIGHT",
      "value": 1,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "ALIAS",
                    "content": {
                      "type": "STRING",
                      "value": "local"
                    },
                    "named": true,
                    "value": "local"
                  },
                  {
                    "type": "SYMBOL",
                    "name": "function_start"
                  },
                  {
                    "type": "FIELD",
                    "name": "name",
                    "content": {
                      "type": "SYMBOL",
                      "name": "id"
                    }
                  }
                ]
              },
              {
                "type": "SEQ",
                "members": [
                  {
                    "type": "SYMBOL",
                    "name": "function_start"
                  },
                  {
                    "type": "PATTERN",
                    "value": "\\s*"
                  },
                  {
                    "type": "FIELD",
                    "name": "name",
                    "content": {
                      "type": "SYMBOL",
                      "name": "function_name"
                    }
                  }
                ]
              }
            ]
          },
          {
            "type": "SYMBOL",
            "name": "function_impl"
          }
        ]
      }
    },
    "_prefix_exp": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "_var"
        },
        {
          "type": "SYMBOL",
          "name": "function_call"
        },
        {
          "type": "SEQ",
          "members": [
            {
              "type": "STRING",
              "value": "("
            },
            {
              "type": "SYMBOL",
              "name": "expr"
            },
            {
              "type": "STRING",
              "value": ")"
            }
          ]
        }
      ]
    },
    "prefix_exp": {
      "type": "SYMBOL",
      "name": "_prefix_exp"
    },
    "function_call": {
      "type": "PREC_RIGHT",
      "value": 1,
      "content": {
        "type": "SEQ",
        "members": [
          {
            "type": "FIELD",
            "name": "prefix",
            "content": {
              "type": "SYMBOL",
              "name": "prefix_exp"
            }
          },
          {
            "type": "CHOICE",
            "members": [
              {
                "type": "SYMBOL",
                "name": "_args"
              },
              {
                "type": "SYMBOL",
                "name": "_self_call"
              }
            ]
          }
        ]
      }
    },
    "_args": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SYMBOL",
          "name": "_parentheses_call"
        },
        {
          "type": "SYMBOL",
          "name": "_string_call"
        }
      ]
    },
    "_parentheses_call": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": "("
          },
          "named": true,
          "value": "function_call_paren"
        },
        {
          "type": "FIELD",
          "name": "args",
          "content": {
            "type": "CHOICE",
            "members": [
              {
                "type": "SYMBOL",
                "name": "function_arguments"
              },
              {
                "type": "BLANK"
              }
            ]
          }
        },
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": ")"
          },
          "named": true,
          "value": "function_call_paren"
        }
      ]
    },
    "_string_call": {
      "type": "FIELD",
      "name": "args",
      "content": {
        "type": "ALIAS",
        "content": {
          "type": "SYMBOL",
          "name": "string"
        },
        "named": true,
        "value": "string_argument"
      }
    },
    "_self_call": {
      "type": "SEQ",
      "members": [
        {
          "type": "ALIAS",
          "content": {
            "type": "STRING",
            "value": ":"
          },
          "named": true,
          "value": "self_call_colon"
        },
        {
          "type": "SYMBOL",
          "name": "id"
        },
        {
          "type": "SYMBOL",
          "name": "_args"
        }
      ]
    },
    "function_arguments": {
      "type": "SEQ",
      "members": [
        {
          "type": "SYMBOL",
          "name": "expr"
        },
        {
          "type": "CHOICE",
          "members": [
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
                    "name": "expr"
                  }
                ]
              }
            },
            {
              "type": "BLANK"
            }
          ]
        }
      ]
    },
    "comment": {
      "type": "CHOICE",
      "members": [
        {
          "type": "SEQ",
          "members": [
            {
              "type": "FIELD",
              "name": "start",
              "content": {
                "type": "ALIAS",
                "content": {
                  "type": "STRING",
                  "value": "//"
                },
                "named": false,
                "value": "comment_start"
              }
            },
            {
              "type": "FIELD",
              "name": "content",
              "content": {
                "type": "ALIAS",
                "content": {
                  "type": "PATTERN",
                  "value": "[^\\r\\n]*"
                },
                "named": false,
                "value": "comment_content"
              }
            }
          ]
        },
        {
          "type": "SEQ",
          "members": [
            {
              "type": "FIELD",
              "name": "start",
              "content": {
                "type": "ALIAS",
                "content": {
                  "type": "SYMBOL",
                  "name": "_block_comment_start"
                },
                "named": false,
                "value": "comment_start"
              }
            },
            {
              "type": "FIELD",
              "name": "content",
              "content": {
                "type": "CHOICE",
                "members": [
                  {
                    "type": "ALIAS",
                    "content": {
                      "type": "SYMBOL",
                      "name": "_block_comment_content"
                    },
                    "named": false,
                    "value": "comment_content"
                  },
                  {
                    "type": "BLANK"
                  }
                ]
              }
            },
            {
              "type": "FIELD",
              "name": "end",
              "content": {
                "type": "ALIAS",
                "content": {
                  "type": "SYMBOL",
                  "name": "_block_comment_end"
                },
                "named": false,
                "value": "comment_end"
              }
            }
          ]
        }
      ]
    }
  },
  "extras": [
    {
      "type": "PATTERN",
      "value": "\\n"
    },
    {
      "type": "PATTERN",
      "value": "\\s"
    },
    {
      "type": "SYMBOL",
      "name": "comment"
    }
  ],
  "conflicts": [
    [
      "id",
      "keywords"
    ]
  ],
  "precedences": [],
  "externals": [
    {
      "type": "SYMBOL",
      "name": "_block_comment_start"
    },
    {
      "type": "SYMBOL",
      "name": "_block_comment_content"
    },
    {
      "type": "SYMBOL",
      "name": "_block_comment_end"
    },
    {
      "type": "SYMBOL",
      "name": "_string_start"
    },
    {
      "type": "SYMBOL",
      "name": "_string_content"
    },
    {
      "type": "SYMBOL",
      "name": "_string_end"
    }
  ],
  "inline": [
    "comment"
  ],
  "supertypes": [],
  "reserved": {}
}