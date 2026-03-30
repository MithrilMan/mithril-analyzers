# Release Flow

- Status: active
- Confidence: medium
- Last-Validated: 2026-03-30
- Evidence: .github/workflows/release-package.yml; docs/releasing.md; README.md; docs/specs/decisions/entries/DEC-20260330-03-publish-package-to-nuget-org.md; dotnet restore Mithril.Analyzers.sln --nologo; dotnet build Mithril.Analyzers.sln --configuration Release --no-restore -p:GeneratePackageOnBuild=false --nologo; dotnet test Mithril.Analyzers.sln --configuration Release --no-build --nologo; dotnet pack src/Mithril.Analyzers/Mithril.Analyzers.csproj --configuration Release --no-build --output artifacts/packages -p:ContinuousIntegrationBuild=true --nologo; gh release view v0.1.0 --repo MithrilMan/mithril-analyzers
- Related: .codex/notes/architecture/project-bootstrap.md
- Supersedes: none
- Superseded-By: none

## Facts

- The repository now releases `Mithril.Analyzers` from semver tags prefixed with `v`.
- The tagged commit must be reachable from `main`, and the tag version must match `<Version>` in `src/Mithril.Analyzers/Mithril.Analyzers.csproj`.
- The release automation definition now builds, tests, packs, publishes the `.nupkg` to NuGet.org with the `NUGET_API_KEY` GitHub Actions secret, and attaches the same package to the GitHub Release.
- The NuGet push step uses `--skip-duplicate` so re-running the same release tag remains idempotent on the feed side.
- The first real release `v0.1.0` completed successfully on 2026-03-30 as a GitHub Release asset publish before NuGet.org publication was added to the workflow.

## Implications

- Future release tasks should update the project version first, then create an annotated tag that matches it.
- The `NUGET_API_KEY` secret is now a release prerequisite in GitHub before pushing a release tag.
- The documented release path now targets both NuGet.org and GitHub Releases from the same tag workflow.
- The published release URL is `https://github.com/MithrilMan/mithril-analyzers/releases/tag/v0.1.0`.

## Follow-ups

- Run the next semver tag after setting `NUGET_API_KEY` to validate the remote GitHub Actions to NuGet.org publish path end to end.
