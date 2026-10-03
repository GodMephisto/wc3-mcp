# Security policy

## Reporting a vulnerability

Please do not open a public issue for a security problem. Report it privately
through GitHub instead, on the
[Security tab](https://github.com/GodMephisto/wc3-mcp/security/advisories/new)
of this repository ("Report a vulnerability"). Only the maintainer can see it.

Include what you found, how to reproduce it (a small map or replay helps), and
which version you ran (`wc3-mcp version`).

You should get a reply within a week. Once a fix is released, the advisory is
published and you are credited unless you ask not to be.

## Supported versions

Only the latest release gets fixes. Update with the install one-liner in the
README, or download the newest zip from the Releases page.

## What counts

wc3-mcp reads and writes map files you point it at, reads your local Warcraft
III install, and edits your AI app configs and user PATH during setup. Things
worth reporting include

- a map, model or replay file that makes wc3-mcp write outside the path it was
  given, run code, or crash in a way that could be exploited
- setup changing a config file or PATH entry it should not touch
- a release zip whose checksum does not match its `.sha256`

A map that fails to load or a wrong value in a tool's output is a normal bug,
so please open an issue for those.
