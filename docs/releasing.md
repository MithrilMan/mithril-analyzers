# Releasing

`Mithril.Analyzers` releases are cut from the repository default branch `main`, not `master`.

## Source of Truth

- Package version: `src/Mithril.Analyzers/Mithril.Analyzers.csproj`
- Release workflow: `.github/workflows/release-package.yml`
- Distribution artifact: `.nupkg` attached to the GitHub Release

## Release Tag Format

- Stable release: `vX.Y.Z`
- Pre-release: `vX.Y.Z-suffix`

The workflow strips the leading `v`, verifies the remaining string matches the project `<Version>`, and marks the GitHub Release as a pre-release when the version contains a suffix such as `-rc.1`.

## Release Steps

1. Update `<Version>` in `src/Mithril.Analyzers/Mithril.Analyzers.csproj`.
2. Update any release-relevant docs or analyzer release notes that changed with the version.
3. Run the local validation loop:

   ```powershell
   dotnet restore Mithril.Analyzers.sln
   dotnet build Mithril.Analyzers.sln --no-restore
   dotnet test Mithril.Analyzers.sln --no-build
   ```

4. Commit the release-ready changes on `main`.
5. Create an annotated tag that matches the package version:

   ```powershell
   git tag -a vX.Y.Z -m "Mithril.Analyzers X.Y.Z"
   ```

6. Push the branch and the tag:

   ```powershell
   git push origin main
   git push origin vX.Y.Z
   ```

7. Wait for the `Release Package` workflow to finish. It will:
   - fail if the tagged commit is not reachable from `origin/main`
   - fail if the tag version and project version diverge
   - restore, build, test, and pack the solution
   - create or update the GitHub Release for that tag and upload the `.nupkg`

## Notes

- Re-running the workflow for the same tag updates the uploaded package asset instead of creating a second release.
- This flow intentionally publishes a GitHub Release asset rather than GitHub Packages, keeping distribution simple for the first OSS release path. A registry publish job can be added later if package-feed consumption becomes a priority.
