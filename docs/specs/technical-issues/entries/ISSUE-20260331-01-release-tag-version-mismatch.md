# [ISSUE-20260331-01] Release tag version mismatch in GitHub Actions

- Date: 2026-03-31
- Area: infra
- Services / Modules:
  - .github/workflows/release-package.yml
  - src/Mithril.Analyzers/Mithril.Analyzers.csproj
  - git tags
- Keywords:
  - release
  - github-actions
  - tags
  - versioning
  - nuget

## Symptoms

- The `Release Package` workflow fails at the version gate with an error like:
  - `Tag version '0.1.1' does not match project version '0.1.0'.`

## Root Cause

- A semver release tag was created and pushed before the matching `<Version>` bump was committed to `src/Mithril.Analyzers/Mithril.Analyzers.csproj`.
- The workflow intentionally rejects this state to prevent publishing a package whose tag and source version diverge.

## Fix

- Update `src/Mithril.Analyzers/Mithril.Analyzers.csproj` so `<Version>` matches the intended tag without the leading `v`.
- Commit that version bump on `main`.
- Delete the incorrect local and remote tag, recreate the annotated tag on the new commit, then push `main` and the recreated tag.

## Validation

- `git log --oneline --decorate -n 8`
- `git ls-remote --tags origin`
- `dotnet restore Mithril.Analyzers.sln --nologo`
- `dotnet build Mithril.Analyzers.sln --configuration Release --no-restore -p:GeneratePackageOnBuild=false --nologo`
- `dotnet test Mithril.Analyzers.sln --configuration Release --no-build --nologo`

## Prevention

- Always commit the version bump before creating the semver tag.
- Treat the release tag as the last step before `git push origin vX.Y.Z`, not as a placeholder to move later.
