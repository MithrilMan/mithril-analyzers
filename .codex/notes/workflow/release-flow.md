# Release Flow

- Status: active
- Confidence: medium
- Last-Validated: 2026-03-30
- Evidence: .github/workflows/release-package.yml; docs/releasing.md; README.md; docs/specs/decisions/entries/DEC-20260330-03-publish-package-to-nuget-org.md; dotnet restore Mithril.Analyzers.sln --nologo; dotnet build Mithril.Analyzers.sln --configuration Release --no-restore -p:GeneratePackageOnBuild=false --nologo; dotnet test Mithril.Analyzers.sln --configuration Release --no-build --nologo; dotnet pack src/Mithril.Analyzers/Mithril.Analyzers.csproj --configuration Release --no-build --output artifacts/packages -p:ContinuousIntegrationBuild=true --nologo; dotnet nuget push artifacts/packages/Mithril.Analyzers.0.1.1.nupkg --api-key dummy --source C:\SVILUPPO\repos\MithrilMan\mithril-analyzers\artifacts\local-feed --skip-duplicate; gh release view v0.1.0 --repo MithrilMan/mithril-analyzers; gh run view 23770459998 --repo MithrilMan/mithril-analyzers --job 69260477305 --log
- Related: .codex/notes/architecture/project-bootstrap.md
- Supersedes: none
- Superseded-By: none

## Facts

- The repository now releases `Mithril.Analyzers` from semver tags prefixed with `v`.
- The tagged commit must be reachable from `main`, and the tag version must match `<Version>` in `src/Mithril.Analyzers/Mithril.Analyzers.csproj`.
- The release automation definition now builds, tests, packs, publishes the `.nupkg` to NuGet.org with the `NUGET_API_KEY` GitHub Actions secret, and attaches the same package to the GitHub Release.
- The NuGet push step uses `--skip-duplicate` so re-running the same release tag remains idempotent on the feed side.
- The first real release `v0.1.0` completed successfully on 2026-03-30 as a GitHub Release asset publish before NuGet.org publication was added to the workflow.
- The GitHub Actions run for `v0.1.1` on 2026-03-30 22:18 UTC reached the NuGet publish step and failed because `dotnet nuget push` does not accept `--nologo`.

## Implications

- Future release tasks should update the project version first, then create an annotated tag that matches it.
- The `NUGET_API_KEY` secret is now a release prerequisite in GitHub before pushing a release tag.
- The documented release path now targets both NuGet.org and GitHub Releases from the same tag workflow.
- The published release URL is `https://github.com/MithrilMan/mithril-analyzers/releases/tag/v0.1.0`.
- If the workflow fails with `Tag version 'X.Y.Z' does not match project version 'A.B.C'.`, the fix is to update `src/Mithril.Analyzers/Mithril.Analyzers.csproj`, commit the version bump on `main`, then recreate the release tag so it points to that commit.
- The `dotnet nuget push` invocation must avoid `--nologo`; that switch is accepted by other `dotnet` subcommands in the workflow but not by the NuGet push command.

## Follow-ups

- Run the next semver tag after setting `NUGET_API_KEY` to validate the remote GitHub Actions to NuGet.org publish path end to end.
- When a release tag is created too early, delete and recreate that tag only after the matching version commit is on `main`.
- Re-run the failed `v0.1.1` job after the workflow fix to validate NuGet.org publication end to end.
