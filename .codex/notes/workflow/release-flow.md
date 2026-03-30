# Release Flow

- Status: active
- Confidence: high
- Last-Validated: 2026-03-30
- Evidence: .github/workflows/release-package.yml; gh run view 23759151908 --repo MithrilMan/mithril-analyzers; gh run view 23759227298 --repo MithrilMan/mithril-analyzers; gh run view 23760107334 --repo MithrilMan/mithril-analyzers; gh release view v0.1.0 --repo MithrilMan/mithril-analyzers
- Related: .codex/notes/architecture/project-bootstrap.md
- Supersedes: none
- Superseded-By: none

## Facts

- The repository now releases `Mithril.Analyzers` from semver tags prefixed with `v`.
- The tagged commit must be reachable from `main`, and the tag version must match `<Version>` in `src/Mithril.Analyzers/Mithril.Analyzers.csproj`.
- The release automation builds, tests, packs, and publishes the `.nupkg` as a GitHub Release asset.
- Smoke-test tags on 2026-03-30 confirmed that GitHub triggers the workflow remotely and blocks mismatched versions before package publication.
- The first real release `v0.1.0` completed successfully on 2026-03-30 and published `Mithril.Analyzers.0.1.0.nupkg`.

## Implications

- Future release tasks should update the project version first, then create an annotated tag that matches it.
- GitHub Releases are the current distribution path; GitHub Packages or NuGet.org would require a follow-up workflow decision.
- The published release URL is `https://github.com/MithrilMan/mithril-analyzers/releases/tag/v0.1.0`.

## Follow-ups

- Revisit feed-based publication only if consumers need installable package registry support beyond GitHub Release assets.
