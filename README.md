# Mithril.Analyzers

`Mithril.Analyzers` is an MIT-licensed Roslyn analyzer package focused on backend maintainability guardrails and repository-informed architecture checks.

The first delivery wave stays intentionally small and conservative:

- V1 covers `BGA001` to `BGA005`
- configuration lives in `.editorconfig`
- generated code is ignored by default
- the package is packed as analyzer-only content so consumers get no runtime dependency

## Package Status

- Status: bootstrap in progress
- Current scope: V1 maintainability parity for oversized files, methods, accessors, async cancellation exposure, and `ContainsKey` double lookups
- Deferred to later phases: `BGA100+` repository-informed architecture diagnostics

## Rule Table

| ID | Title | Default severity | Primary config | Docs |
| --- | --- | --- | --- | --- |
| `BGA001` | File Too Long | `warning` | `mithril_analyzers.max_file_lines` | [BGA001](docs/rules/BGA001-file-too-long.md) |
| `BGA002` | Method Too Long | `warning` | `mithril_analyzers.max_method_lines` | [BGA002](docs/rules/BGA002-method-too-long.md) |
| `BGA003` | Accessor Too Long | `warning` | `mithril_analyzers.max_accessor_lines` | [BGA003](docs/rules/BGA003-accessor-too-long.md) |
| `BGA004` | Async Method Should Expose Cancellation | `suggestion` | `mithril_analyzers.allow_missing_cancellation_for_tests` | [BGA004](docs/rules/BGA004-async-method-should-expose-cancellation.md) |
| `BGA005` | Prefer `TryGetValue` | `suggestion` | n/a | [BGA005](docs/rules/BGA005-prefer-try-get-value.md) |

## Install

Use the package as a build-only analyzer dependency:

```xml
<ItemGroup>
  <PackageReference Include="Mithril.Analyzers" Version="0.1.0" PrivateAssets="all" />
</ItemGroup>
```

## `.editorconfig` Example

```ini
[*.cs]
dotnet_diagnostic.BGA001.severity = warning
dotnet_diagnostic.BGA002.severity = warning
dotnet_diagnostic.BGA003.severity = warning
dotnet_diagnostic.BGA004.severity = suggestion
dotnet_diagnostic.BGA005.severity = suggestion

mithril_analyzers.max_accessor_lines = 60
mithril_analyzers.allow_missing_cancellation_for_tests = true

[src/**/*.cs]
mithril_analyzers.max_file_lines = 800
mithril_analyzers.max_method_lines = 100

[tests/**/*.cs]
mithril_analyzers.max_file_lines = 1000
mithril_analyzers.max_method_lines = 140

[tools/**/*.cs]
mithril_analyzers.max_file_lines = 800
mithril_analyzers.max_method_lines = 100
```

## Suppression Examples

Use the normal Roslyn suppression surfaces when a diagnostic is intentionally not actionable:

```csharp
#pragma warning disable BGA004
public Task<string> ExecuteAsync()
{
    return Task.FromResult("intentionally non-cancellable");
}
#pragma warning restore BGA004
```

```csharp
[System.Diagnostics.CodeAnalysis.SuppressMessage(
    "Maintainability",
    "BGA002:Method Too Long",
    Justification = "Legacy orchestration remains until the module split ships.")]
```

## Repository Layout

- [src/Mithril.Analyzers](src/Mithril.Analyzers) contains production analyzers and shared helpers
- [tests/Mithril.Analyzers.Tests](tests/Mithril.Analyzers.Tests) contains Roslyn analyzer tests
- [docs/architecture.md](docs/architecture.md) describes the project structure
- [docs/migration-from-menees.md](docs/migration-from-menees.md) outlines the migration path away from `Menees.Analyzers`

## Development

```powershell
dotnet restore Mithril.Analyzers.sln
dotnet build Mithril.Analyzers.sln --no-restore
dotnet test Mithril.Analyzers.sln --no-build
```
