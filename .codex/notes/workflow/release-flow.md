# Release Flow

- Status: active
- Confidence: high
- Last-Validated: 2026-03-30
- Evidence: .github/workflows/release-package.yml; docs/releasing.md; docs/specs/decisions/entries/DEC-20260330-02-release-package-from-tag-on-main.md
- Related: .codex/notes/architecture/project-bootstrap.md
- Supersedes: none
- Superseded-By: none

## Facts

- The repository now releases `Mithril.Analyzers` from semver tags prefixed with `v`.
- The tagged commit must be reachable from `main`, and the tag version must match `<Version>` in `src/Mithril.Analyzers/Mithril.Analyzers.csproj`.
- The release automation builds, tests, packs, and publishes the `.nupkg` as a GitHub Release asset.

## Implications

- Future release tasks should update the project version first, then create an annotated tag that matches it.
- GitHub Releases are the current distribution path; GitHub Packages or NuGet.org would require a follow-up workflow decision.

## Follow-ups

- Revisit feed-based publication only if consumers need installable package registry support beyond GitHub Release assets.
