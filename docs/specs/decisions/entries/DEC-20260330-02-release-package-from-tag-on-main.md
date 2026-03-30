# [DEC-20260330-02] Release the package from semver tags on main via GitHub Releases

- Date: 2026-03-30
- Status: superseded
- Domain: workflow
- Keywords:
  - release
  - github-actions
  - tagging
  - packaging
- Supersedes: none
- Superseded-By: DEC-20260330-03

## Context

- The repository already produces an analyzer-only NuGet package, but it does not yet have an automated release path.
- The release request is tied to GitHub tags, and the repository currently uses `main` as its default branch.
- The first public release path should stay low-friction, require no extra secrets, and fail loudly when versioning discipline is broken.

## Options Considered

### Option A

- Build and upload release artifacts manually from a contributor machine.
- Tradeoffs:
  - Lowest automation effort.
  - Poor repeatability and no repository-enforced validation gate.

### Option B

- Publish directly to GitHub Packages from a tag workflow.
- Tradeoffs:
  - Gives a package-feed experience.
  - Adds registry-specific behavior and consumer friction before the project has validated its first release path.

### Option C

- Trigger a GitHub Action from a semver tag, validate that the tag matches the project version and belongs to `main`, then create a GitHub Release with the `.nupkg` asset.
- Tradeoffs:
  - Keeps the release path simple and reproducible with built-in GitHub primitives.
  - Does not yet provide a package-feed endpoint.

## Decision

- Use semver-style tags prefixed with `v` as the release trigger.
- Require the tagged commit to be reachable from `main`.
- Require the tag version to match the `<Version>` in `src/Mithril.Analyzers/Mithril.Analyzers.csproj`.
- Build, test, and pack in GitHub Actions, then create a GitHub Release and attach the produced `.nupkg`.

## Consequences

- Releases become reproducible and repository-enforced instead of manual-only.
- Version drift between source and tag becomes a hard failure instead of a silent mismatch.
- This decision established the tag-driven validation and GitHub Release path, but the distribution strategy was later extended by `DEC-20260330-03` to publish the same package to NuGet.org.

## Validation / Evidence

- `.github/workflows/release-package.yml`
- `docs/releasing.md`
- `src/Mithril.Analyzers/Mithril.Analyzers.csproj`
