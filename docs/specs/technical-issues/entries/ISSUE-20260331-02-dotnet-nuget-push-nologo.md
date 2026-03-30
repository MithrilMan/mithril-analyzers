# [ISSUE-20260331-02] `dotnet nuget push` rejects `--nologo` in the release workflow

- Date: 2026-03-31
- Area: infra
- Services / Modules:
  - .github/workflows/release-package.yml
  - dotnet nuget CLI
- Keywords:
  - release
  - github-actions
  - nuget
  - cli
  - options

## Symptoms

- The `Release Package` workflow reaches the `Publish to NuGet.org` step and fails with:
  - `error: Unrecognized option '--nologo'`

## Root Cause

- `--nologo` is accepted by commands like `dotnet restore`, `dotnet build`, `dotnet test`, and `dotnet pack`, but not by `dotnet nuget push`.
- The workflow reused the flag pattern from the build commands, which made the release fail only at the final publish step.

## Fix

- Remove `--nologo` from the `dotnet nuget push` invocation in `.github/workflows/release-package.yml`.
- Keep the other publish arguments unchanged: `--api-key`, `--source`, and `--skip-duplicate`.

## Validation

- `gh run view 23770459998 --repo MithrilMan/mithril-analyzers --job 69260477305 --log`
- `dotnet restore Mithril.Analyzers.sln --nologo`
- `dotnet build Mithril.Analyzers.sln --configuration Release --no-restore -p:GeneratePackageOnBuild=false --nologo`
- `dotnet test Mithril.Analyzers.sln --configuration Release --no-build --nologo`
- `dotnet pack src/Mithril.Analyzers/Mithril.Analyzers.csproj --configuration Release --no-build --output artifacts/packages -p:ContinuousIntegrationBuild=true --nologo`
- `dotnet nuget push artifacts/packages/Mithril.Analyzers.0.1.1.nupkg --api-key dummy --source C:\SVILUPPO\repos\MithrilMan\mithril-analyzers\artifacts\local-feed --skip-duplicate`

## Prevention

- Do not assume switches supported by `dotnet` build/test commands also apply to `dotnet nuget` subcommands.
- When extending release automation with new CLI calls, confirm the exact command-specific options before tagging a release.
