# Publish

Publishing is the user's decision and never the agent's.

## What grants it

- `permissions.json` beside the server executable carries the switches `publish`, `mirror_export`
  and `npm`. Each one is false unless the user set it.
- The agent cannot write that file and must not try. There is no tool that grants a switch, and a
  request to add one should be refused and explained.
- `WC3_MCP_PERMISSIONS` may name a different file. That is still the user's file.

## What to do instead of publishing

1. Finish the work and show it, with the tests that ran and their counts.
2. Ask the user in plain words. Say what would be published, where it would land, and which switch
   grants it.
3. Wait. A confirmation in the conversation is intent, not permission. Only the file grants it.
4. On a refusal, report the switch and the file the message named. Do not work around it by
   publishing by hand, by another account or by another path.

A publish step that skipped this is a bug to report, not a shortcut to repeat.
