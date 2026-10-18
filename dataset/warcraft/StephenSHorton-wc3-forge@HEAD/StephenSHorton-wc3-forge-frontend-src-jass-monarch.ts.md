---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/jass-monarch.ts"
ref: HEAD
---

// JASS Monarch tokenizer for Monaco.
//
// Vanilla JASS keyword set + vJASS extensions (matches HiveWE's
// src/trigger_editor/jass_editor.cpp keywords_ table, trimmed to what
// actually appears in shipped WC3 maps + Reforged Lua-adjacent vJASS).
//
// Types list comes from common.j's primitive + handle hierarchy. Any
// identifier matching the types list lights up as a "type" token; the
// rest fall through to the identifier rule.
//
// Reference: Monaco's `monaco.languages.IMonarchLanguage` shape —
// https://microsoft.github.io/monaco-editor/monarch.html
//
// Keep this file small. A real JASS parser is overkill for v1; Monarch
// gives us keyword/string/number/comment colors which covers >95% of
// what the user perceives as "syntax highlighting".

export const jassLanguage = {
  defaultToken: '',
  ignoreCase: false,

  keywords: [
    // Vanilla JASS
    'function', 'endfunction',
    'if', 'then', 'else', 'elseif', 'endif',
    'loop', 'endloop', 'exitwhen',
    'set', 'call', 'return', 'returns',
    'globals', 'endglobals',
    'local', 'constant', 'native',
    'takes', 'nothing',
    'array', 'type', 'extends',
    'true', 'false', 'null',
    'not', 'and', 'or',
    // vJASS / Zinc additions HiveWE recognizes
    'library', 'endlibrary', 'scope', 'endscope',
    'struct', 'endstruct', 'method', 'endmethod',
    'private', 'public', 'static', 'thistype',
    'requires', 'needs', 'uses', 'initializer',
    'module', 'implement', 'optional',
  ],

  typeKeywords: [
    // common.j primitives
    'integer', 'real', 'string', 'boolean', 'code', 'handle',
    // common.j handle subtypes (most-used)
    'unit', 'player', 'force', 'group', 'location', 'rect', 'region',
    'trigger', 'event', 'condition', 'triggeraction', 'timer', 'timerdialog',
    'dialog', 'button', 'item', 'destructable', 'ability', 'buff',
    'effect', 'lightning', 'image', 'ubersplat', 'fogmodifier', 'fogstate',
    'leaderboard', 'multiboard', 'multiboarditem',
    'sound', 'soundtype', 'music', 'quest', 'questitem',
    'defeatcondition', 'gamecache', 'hashtable', 'gamestate',
    'eventid', 'racepreference', 'mapsetting', 'mapdensity', 'mapcontrol',
    'mapflag', 'placement', 'startlocprio', 'gamedifficulty', 'gamespeed',
    'gametype', 'mapvisibility', 'playercolor', 'playerslotstate',
    'playergameresult', 'unitstate', 'aidifficulty', 'eventid',
    'attacktype', 'damagetype', 'weapontype', 'soundtype', 'pathingtype',
    'race', 'alliancetype', 'version', 'volumegroup', 'camerafield',
    'camerasetup', 'texttag', 'unittype', 'itemtype', 'gametype',
  ],

  operators: [
    '=', '>', '<', '!', '~', '?', ':',
    '==', '<=', '>=', '!=',
    '+', '-', '*', '/', '%',
  ],

  symbols: /[=><!~?:&|+\-*\/\^%]+/,

  // C-style numbers + rawcodes ('hfoo' = 4cc)
  tokenizer: {
    root: [
      // Identifiers / keywords / types
      [/[a-zA-Z_]\w*/, {
        cases: {
          '@keywords': 'keyword',
          '@typeKeywords': 'type',
          '@default': 'identifier',
        },
      }],

      // Whitespace
      { include: '@whitespace' },

      // Numbers — int, hex (0x), float, dollar-hex ($ABCD)
      [/0[xX][0-9a-fA-F]+/, 'number.hex'],
      [/\$[0-9a-fA-F]+/, 'number.hex'],
      [/\d*\.\d+([eE][\-+]?\d+)?/, 'number.float'],
      [/\d+/, 'number'],

      // Delimiters / operators
      [/[{}()\[\]]/, '@brackets'],
      [/[,.;]/, 'delimiter'],
      [/@symbols/, {
        cases: {
          '@operators': 'operator',
          '@default': '',
        },
      }],

      // Strings
      [/"/, { token: 'string.quote', bracket: '@open', next: '@string' }],
      // Rawcode (FourCC) — single-quoted 4-char literal. Treat as a string-ish
      // token so it stands out.
      [/'[^']*'/, 'string'],
    ],

    whitespace: [
      [/[ \t\r\n]+/, ''],
      [/\/\*/, 'comment', '@comment'],
      [/\/\/.*$/, 'comment'],
    ],

    comment: [
      [/[^\/*]+/, 'comment'],
      [/\*\//, 'comment', '@pop'],
      [/[\/*]/, 'comment'],
    ],

    string: [
      [/[^\\"]+/, 'string'],
      [/\\./, 'string.escape'],
      [/"/, { token: 'string.quote', bracket: '@close', next: '@pop' }],
    ],
  },
}

// Bracket pairs Monaco uses for matching brackets + auto-close.
export const jassLanguageConfiguration = {
  comments: {
    lineComment: '//',
    blockComment: ['/*', '*/'] as [string, string],
  },
  brackets: [
    ['{', '}'],
    ['[', ']'],
    ['(', ')'],
  ] as [string, string][],
  autoClosingPairs: [
    { open: '{', close: '}' },
    { open: '[', close: ']' },
    { open: '(', close: ')' },
    { open: '"', close: '"' },
    { open: "'", close: "'" },
  ],
  surroundingPairs: [
    { open: '{', close: '}' },
    { open: '[', close: ']' },
    { open: '(', close: ')' },
    { open: '"', close: '"' },
    { open: "'", close: "'" },
  ],
}
