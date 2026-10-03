# Contributing

Thanks for helping. Bug reports, map files that break something, and pull
requests are all welcome.

## Reporting a bug

Open an issue with the bug template. The most useful thing you can attach is
the map (or replay) that shows the problem, plus the tool you called and what
it returned. Security problems go through [SECURITY.md](SECURITY.md) instead.

## Building and testing

You need Windows and the .NET 8 SDK.

```powershell
git clone https://github.com/GodMephisto/wc3-mcp
cd wc3-mcp
dotnet build
dotnet test --filter "Category!=Corpus&Category!=GameData"
```

Tests marked `GameData` need a Warcraft III install, and `Corpus` tests need
real maps on disk. CI runs only the hermetic set, so run the others yourself if
your change touches game data or map parsing.

## Pull requests

The code under `src/` and `tests/` is developed in
[wc3ctl](https://github.com/GodMephisto/wc3ctl), the toolkit this server is part
of, and copied here for each release. Send code changes there. A pull request
here that changes it is still welcome. It gets applied in wc3ctl and arrives
here with the next release, so it is closed rather than merged, with a note
saying where it landed. Changes to everything else (the README, the
installer, the workflows, the bundle and registry files) are merged here as
usual. `Directory.Build.props` holds what makes the shared source this product,
its exe name, its version and the name it registers under.

- Keep untouched files byte for byte. A map that is opened and saved without
  edits must come out identical, apart from the MPQ bookkeeping files.
- Add a test that fails without your change.
- Do not bump the pinned dependency versions in the project files. Each pin is
  there for a reason, listed in REQUIREMENTS.md.
- Commit messages follow Conventional Commits, for example
  `fix(cameras): handle a map with no w3c file`.

## License of contributions

By submitting a contribution you agree it is licensed under the Apache License
2.0, the same as the rest of the project (section 5 of the LICENSE).
