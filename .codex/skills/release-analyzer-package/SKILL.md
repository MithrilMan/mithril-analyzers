---
name: release-analyzer-package
description: Prepare and publish a new `Mithril.Analyzers` package release. Use when the user wants to cut a new package version, bump the analyzer package version, create the matching Git tag, verify the release workflow, or follow the repository release checklist on `main`.
---

# Release Analyzer Package

Release `Mithril.Analyzers` through the repository workflow instead of inventing one-off commands. Keep the source version, tag, validation evidence, and GitHub Release aligned.

## Workflow

1. Read `docs/releasing.md` first, then open `.github/workflows/release-package.yml` if you need to confirm the current automation gates.
2. Confirm the target version. Use the user's requested version when given; otherwise infer it from the release request and ask only if bump semantics are ambiguous.
3. Update `src/Mithril.Analyzers/Mithril.Analyzers.csproj` so `<Version>` exactly matches the intended release version without the leading `v`.
4. Check whether the release also needs documentation or analyzer release-note updates. Touch `README.md`, `docs/rules/`, and `src/Mithril.Analyzers/AnalyzerReleases.*.md` only when the shipped behavior changed.
5. Run the repository validation loop before tagging:
   - `dotnet restore Mithril.Analyzers.sln`
   - `dotnet build Mithril.Analyzers.sln --no-restore`
   - `dotnet test Mithril.Analyzers.sln --no-build`
6. Commit the release-ready phase with `phase-commit-workflow`. Do not mix unrelated files into the release commit.
7. Ensure the release commit is on `main`, then create an annotated tag in the form `vX.Y.Z` or `vX.Y.Z-suffix`.
8. Push `main` and the tag. The GitHub Action will reject tags that do not match the project version or are not reachable from `main`.
9. Report the release result, including any failed gate, the created tag, and the GitHub Release/package asset outcome.

## Guardrails

- Treat `main` as the release branch for this repository. Do not use `master` unless the repository branch strategy changes.
- Keep the tag and `<Version>` identical apart from the leading `v`.
- Prefer annotated tags over lightweight tags.
- Use pre-release suffixes such as `-rc.1` when the user asks for a non-final release; the workflow marks those GitHub Releases as pre-releases automatically.
- If the user asks for package-feed publication, explain that the current repository automation publishes a GitHub Release asset, not GitHub Packages or NuGet.org.

## References

- Read `references/release-checklist.md` for the exact command sequence and verification expectations.
