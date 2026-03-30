# Release Checklist

Use this checklist when the user asks to release a new `Mithril.Analyzers` version.

## Canonical Files

- Version source: `src/Mithril.Analyzers/Mithril.Analyzers.csproj`
- Workflow: `.github/workflows/release-package.yml`
- Human checklist: `docs/releasing.md`

## Command Sequence

```powershell
dotnet restore Mithril.Analyzers.sln
dotnet build Mithril.Analyzers.sln --no-restore
dotnet test Mithril.Analyzers.sln --no-build
git tag -a vX.Y.Z -m "Mithril.Analyzers X.Y.Z"
git push origin main
git push origin vX.Y.Z
```

## Verification Gates

- The release commit must be reachable from `main`.
- The tag must begin with `v`.
- The tag version without `v` must match `<Version>` in the analyzer `.csproj`.
- The workflow builds, tests, packs, and attaches the `.nupkg` to the GitHub Release.

## Common Follow-ups

- If validation fails before tagging, fix the repo state and re-run the local checks.
- If the workflow fails on version mismatch, update the `.csproj` or recreate the tag so they match.
- If the workflow fails because the tag is not on `main`, retag the correct commit after pushing `main`.
