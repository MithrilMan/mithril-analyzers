# [DEC-20260330-03] Publish the package to NuGet.org from the tag release workflow

- Date: 2026-03-30
- Status: accepted
- Domain: workflow
- Keywords:
  - release
  - github-actions
  - nuget
  - packaging
  - secrets
- Supersedes: DEC-20260330-02
- Superseded-By: none

## Context

- The repository now has a working tag-driven release workflow and a first published GitHub Release asset for `Mithril.Analyzers`.
- Consumers expect `Mithril.Analyzers` to be installable from the standard NuGet.org feed, not only downloadable from GitHub Releases.
- The user explicitly requested real NuGet.org publication and already provisioned a fresh NuGet.org API key for GitHub Actions.

## Options Considered

### Option A

- Keep the current GitHub Release-only workflow.
- Tradeoffs:
  - No new secret management is required.
  - Package consumption remains awkward because the package is missing from the normal NuGet feed.

### Option B

- Replace the GitHub Release asset flow with NuGet.org-only publication.
- Tradeoffs:
  - Simplifies the distribution target to a single package feed.
  - Loses the existing GitHub Release artifact and release notes path that already works for the project.

### Option C

- Keep the existing semver tag workflow on `main`, publish the packed `.nupkg` to NuGet.org, and continue creating the GitHub Release asset for the same tag.
- Tradeoffs:
  - Preserves the current validated release path while adding the expected package-feed endpoint.
  - Requires a maintained GitHub Actions secret and explicit handling for duplicate reruns.

## Decision

- Continue using semver tags prefixed with `v` on `main` as the release trigger.
- Keep the existing version/tag validation and build-test-pack gates.
- Publish the produced `.nupkg` to NuGet.org from the same workflow using the GitHub Actions secret `NUGET_API_KEY`.
- Keep the GitHub Release creation step so every tagged release still has attached artifacts and generated release notes.
- Make reruns idempotent by using duplicate-safe NuGet publication behavior.

## Consequences

- `Mithril.Analyzers` becomes installable from NuGet.org while preserving the existing GitHub Release record.
- Release automation now depends on correct secret management in GitHub, so missing or rotated credentials become a release gate.
- The workflow must tolerate repeat execution for the same tag without failing on an already-published NuGet version.

## Validation / Evidence

- `.github/workflows/release-package.yml`
- `README.md`
- `docs/releasing.md`
- `.codex/skills/release-analyzer-package/SKILL.md`
- `.codex/skills/release-analyzer-package/references/release-checklist.md`
