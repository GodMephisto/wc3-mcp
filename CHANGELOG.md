# Changelog

## 0.5.45 - 2026-11-25

Every image decode runs through one configuration that registers no TIFF and reads no ICC, so the open advisories against the pinned ImageSharp are unreachable. See `changelog/0.5.45.md`.

## 0.5.44 - 2026-11-16

`texture_audit` reports the texture headers the engine mishandles. See `changelog/0.5.44.md`.

## 0.5.43 - 2026-11-16

`game_file_shadows` finds map files that override the installed game's own files, and `repair_game_files` writes a copy without them. See `changelog/0.5.43.md`.

## 0.5.42 - 2026-11-16

`repair_reforged3` repairs a legacy SLK map for Warcraft III 3.0 in code, with no doctor, no game install and no name dictionary, and `reforged3_check` reports the same six counts read-only. See `changelog/0.5.42.md`.

## 0.5.41 - 2026-11-16

Devo's Map Doctor pinned at 2.3.2, which reads the obfuscated names a protected archive hides, and a doctor_schema tool that returns the installed doctor's own JSON contract. See `changelog/0.5.41.md`.

## 0.5.40 - 2026-11-11

The release record reaches main, since the release job checks out the tag and a bare push from a detached HEAD has no branch to send. See `changelog/0.5.40.md`.

## 0.5.39 - 2026-11-10

The release workflow writes `docs/specification/releases/v<version>.json` once the assets exist, holding the build commit and each artifact's name, URL, byte size and sha256. See `changelog/0.5.39.md`.

## 0.5.38 - 2026-11-10

`ToolCancellationTests` cancels from the import's own progress report instead of a one millisecond timer that a fast runner beat, so the expected cancellation lands inside the run. See `changelog/0.5.38.md`.

## 0.5.37 - 2026-11-10

The sync copies the note wc3ctl wrote for the version, and only generates one from its own mirror commits when the source tree has none. See `changelog/0.5.37.md`.

## 0.5.36 - 2026-11-10

The generated note drops the `chore(sync)` bookkeeping lines, so it lists the commits that changed the tool and nothing else. See `changelog/0.5.36.md`.

## 0.5.35 - 2026-11-10

The note generator reads its range from the tag's parent rather than the tag it is about to create, so a note has commits to list. See `changelog/0.5.35.md`.

## 0.5.34 - 2026-11-10

The changelog starts here, with a note per release written, staged and linked from this file by the sync workflow. See `changelog/0.5.34.md`.
