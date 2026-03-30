# Migration From `Menees.Analyzers`

This repository is being bootstrapped to replace the temporary `Menees.Analyzers` dependency with `Mithril.Analyzers`.

## Target End State

- `Mithril.Analyzers` referenced with `PrivateAssets="all"`
- thresholds expressed in `.editorconfig`
- `Menees.Analyzers.Settings.xml` removed
- `Menees.Analyzers` package removed once diagnostic parity is acceptable

## Suggested Migration Steps

1. Add `Mithril.Analyzers` beside the temporary dependency.
2. Mirror the current file and method thresholds into `.editorconfig`.
3. Run `dotnet build` and compare the warning set.
4. Tune any false positives conservatively.
5. Remove `Menees.Analyzers` and its XML settings once visibility is good enough.

## Threshold Mapping

The bootstrap uses these repository defaults:

- `src/**/*.cs`
  - `mithril_analyzers.max_file_lines = 800`
  - `mithril_analyzers.max_method_lines = 100`
- `tests/**/*.cs`
  - `mithril_analyzers.max_file_lines = 1000`
  - `mithril_analyzers.max_method_lines = 140`
- `tools/**/*.cs`
  - `mithril_analyzers.max_file_lines = 800`
  - `mithril_analyzers.max_method_lines = 100`
- all backend `.cs`
  - `mithril_analyzers.max_accessor_lines = 60`

## Notes

- `BGA004` and `BGA005` are intentionally `suggestion` severity by default.
- Generated code stays ignored without requiring extra configuration.
- The repository-specific architecture rules remain planned follow-up work after V1 parity is stable.
