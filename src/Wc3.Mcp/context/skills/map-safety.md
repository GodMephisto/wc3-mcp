# Map safety

Read a map before you change it.

## The order of work

- Read first. Call `map_overview` or `map_info`, then the read tool for the family you are about
  to change, and keep the digest the read reports.
- Never overwrite the input. Name an `out_path`, or let the gate write a sibling `.edited` copy.
- Chain the next edit on the output of the last one by passing it as `map`. A chain of small edits
  is easier to abandon than one large one.
- Pass `expect_input_sha256` with the digest you read, so a file that changed underneath the call
  is refused instead of silently merged.

## The refusals, and what each one means

- `OUT_PATH_IS_INPUT` means the output is the input. Name a different file.
- `OUT_PATH_INSIDE_INPUT` means the output sits inside a folder map. Write beside the folder.
- `OUTPUT_EXISTS` means the destination is already there and `overwrite` was not asked for.
- `PERMISSION_DENIED` means `overwrite` is not granted in the user's `permissions.json`. Ask the
  user, never retry with a different path to get around it.
- `STALE_INPUT` means the map changed after it was read. Read it again and chain on the new one.

A refusal writes nothing. Report the code and the message, and say which file was left untouched.
