# Releasing

`Mithril.Analyzers` releases are cut from the repository default branch `main`, not `master`.

## Source of Truth

- Package version: `src/Mithril.Analyzers/Mithril.Analyzers.csproj`
- Release workflow: `.github/workflows/release-package.yml`
- Distribution artifacts: NuGet.org package publication plus `.nupkg` attached to the GitHub Release
- Required secret: `NUGET_API_KEY` GitHub Actions secret with push scope for `Mithril.Analyzers`

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
   - fail if the `NUGET_API_KEY` secret is missing
   - restore, build, test, and pack the solution
   - publish the `.nupkg` to `https://api.nuget.org/v3/index.json`
   - create or update the GitHub Release for that tag and upload the `.nupkg`

## Notes

- Re-running the workflow for the same tag skips duplicate NuGet.org publication and updates the uploaded GitHub Release asset.
- Store the NuGet.org API key only in the repository or organization GitHub Actions secrets; do not commit it or paste it into workflow files.
