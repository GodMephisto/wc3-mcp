---
type: vendor-doc
source_repo: "https://github.com/StephenSHorton/wc3-forge"
source_path: "frontend/src/wc3-intellisense.ts"
ref: HEAD
---

// Monaco completion + hover providers driven by TriggerData.txt's vocabulary.
//
// We register one set of providers for each editor language we care about
// (lua + jass). Source-of-truth is Session.GetTriggerFunctionsMeta() which
// reads TriggerData.txt at map-open time; the result is cached for the
// page lifetime — every Monaco instance shares the same vocab.
//
// Both providers are gated by a module-level "registered" flag so multiple
// editor instances mounting concurrently don't double-register (Monaco
// silently dedups by ID but emits duplicate completions in some versions).

import type * as Monaco from 'monaco-editor'
import { GetTriggerFunctionsMeta, type TriggerFunctionMeta } from './trigger-editor-bindings'

let registered = false
let cachedMeta: TriggerFunctionMeta[] | null = null
let cachedByName: Map<string, TriggerFunctionMeta> = new Map()
let metaLoadPromise: Promise<void> | null = null

// Languages the IntelliSense provider targets. JavaScript / plaintext are
// not in the list — they don't get WC3 completions.
const TARGET_LANGUAGES = ['lua', 'jass'] as const

async function ensureMeta(): Promise<void> {
  if (cachedMeta) return
  if (metaLoadPromise) return metaLoadPromise
  metaLoadPromise = (async () => {
    try {
      const res = await GetTriggerFunctionsMeta()
      const fns = res?.functions ?? []
      cachedMeta = fns
      const m = new Map<string, TriggerFunctionMeta>()
      for (const f of fns) m.set(f.name, f)
      cachedByName = m
    } catch (e) {
      console.warn('IntelliSense: GetTriggerFunctionsMeta failed', e)
      cachedMeta = []
      cachedByName = new Map()
    }
  })()
  return metaLoadPromise
}

// Eagerly start fetching as soon as the module loads — the Wails RPC is
// fast and we want completions ready by the time the user hits Ctrl+Space.
void ensureMeta()

function buildSnippetInsert(fn: TriggerFunctionMeta): string {
  const args = fn.arg_types ?? []
  if (args.length === 0) {
    return `${fn.name}()`
  }
  const parts: string[] = []
  for (let i = 0; i < args.length; i++) {
    const t = args[i] || 'arg'
    parts.push(`\${${i + 1}:${t}${i + 1}}`)
  }
  return `${fn.name}(${parts.join(', ')})`
}

function buildDetail(fn: TriggerFunctionMeta): string {
  const args = (fn.arg_types ?? []).join(', ')
  const ret = fn.return_type ? ` : ${fn.return_type}` : ''
  return `(${args})${ret}`
}

function buildDocs(fn: TriggerFunctionMeta): Monaco.IMarkdownString {
  const parts: string[] = []
  if (fn.display_name && fn.display_name !== fn.name) {
    parts.push(`**${fn.display_name}**`)
  }
  if (fn.hint) parts.push(fn.hint)
  if (fn.section) parts.push(`_${fn.section}_`)
  if (fn.category) parts.push(`Category: ${fn.category}`)
  return { value: parts.join('\n\n'), isTrusted: false }
}

export function registerWC3Intellisense(monaco: typeof Monaco): void {
  if (registered) return
  registered = true

  for (const lang of TARGET_LANGUAGES) {
    monaco.languages.registerCompletionItemProvider(lang, {
      // Trigger on identifier chars; Monaco also re-invokes on Ctrl+Space.
      triggerCharacters: ['.', '(', ' '],
      provideCompletionItems: async (model, position) => {
        await ensureMeta()
        const fns = cachedMeta ?? []
        const word = model.getWordUntilPosition(position)
        const range: Monaco.IRange = {
          startLineNumber: position.lineNumber,
          endLineNumber: position.lineNumber,
          startColumn: word.startColumn,
          endColumn: word.endColumn,
        }
        const suggestions: Monaco.languages.CompletionItem[] = fns.map((fn) => ({
          label: fn.name,
          kind: monaco.languages.CompletionItemKind.Function,
          detail: buildDetail(fn),
          documentation: buildDocs(fn),
          insertText: buildSnippetInsert(fn),
          insertTextRules: monaco.languages.CompletionItemInsertTextRule.InsertAsSnippet,
          range,
        }))
        return { suggestions }
      },
    })

    monaco.languages.registerHoverProvider(lang, {
      provideHover: async (model, position) => {
        await ensureMeta()
        const word = model.getWordAtPosition(position)
        if (!word) return null
        const fn = cachedByName.get(word.word)
        if (!fn) return null
        const contents: Monaco.IMarkdownString[] = [
          { value: `\`\`\`\n${fn.name}${buildDetail(fn)}\n\`\`\`` },
        ]
        const docs = buildDocs(fn)
        if (docs.value) contents.push(docs)
        return {
          range: {
            startLineNumber: position.lineNumber,
            endLineNumber: position.lineNumber,
            startColumn: word.startColumn,
            endColumn: word.endColumn,
          },
          contents,
        }
      },
    })
  }
}
