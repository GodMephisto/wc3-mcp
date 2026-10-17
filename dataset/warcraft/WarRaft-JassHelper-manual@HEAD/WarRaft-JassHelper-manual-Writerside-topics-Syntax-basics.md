---
type: vendor-doc
source_repo: "https://github.com/WarRaft/JassHelper-manual"
source_path: "Writerside/topics/Syntax-basics.md"
ref: HEAD
---

# Syntax basics

In Zinc, every statement/command will either start a block of code or finish with a single `;` character.

`;` Means that you are ending what you are saying. It works to separate different statements of code. Other languages,
like jass, would often need whitespace to make this differentation, for example, require a line break after each
command.

Code blocks are groups of code inside two bracket delimiters, `{` marks the beginning of a code block, and `}` marks the
end of a code block. All syntax constructs that require code blocks use this same syntax.

Since most things ignore whitespace, you can pretty much do plenty of things. You can have multiple declarations in the
same line, and you can also split a declaration that is very long into multiple lines. Notice that this means that you
will need some self control, and also setting up some code style standards for your code would not hurt.

There are two comment constructs: `//` and `/* .. */`. `//` Makes the compiler ignore all the text between `//` and the
next
line break. While `/* */` can make it ignore all the text between the `/*` and the `*/`. `/* */` comments can be nested.
