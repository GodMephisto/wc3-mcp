# Hero package

A hero package is a folder with one file per role and a manifest that names them, so the installer
can rebind a role without guessing a filename.

## The roles

- `body`, the model the hero wears, an `.mdx`.
- `effect`, the emitter model for the cast, an `.mdx`.
- `icon`, the button art, a `.tga` or `.blp`.
- `sound`, the impact clip, a `.wav`.
- `script`, the generated JASS the dispatcher calls, a `.j`.

Every role is filled either by a supplied file or by a generated one, and the manifest records
which, with the source, the licence note and the digest.

## The compiler gate

- A generated model is parsed back by its own reader and must audit with zero issues.
- A generated script must compile. `doctor check` or `JassScriptCheck` is the gate. A game launch
  is not the gate and does not replace it.
- A package that fails the gate is a refusal. Do not install it, and do not report it as ready.
- `hero generate` refuses an unknown base rawcode before it writes anything, so a failed call
  leaves no half package behind.

State plainly which roles were generated, which were supplied, and which are still empty.
