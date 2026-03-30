# Testing Harness

- Status: active
- Confidence: medium
- Last-Validated: 2026-03-30
- Evidence: tests/Mithril.Analyzers.Tests/AnalyzerVerifier.cs; dotnet test Mithril.Analyzers.sln --nologo
- Related: .codex/notes/architecture/project-bootstrap.md
- Supersedes: none
- Superseded-By: none

## Facts

- The bootstrap validates analyzers with xUnit plus a small direct Roslyn harness rather than `Microsoft.CodeAnalysis.CSharp.Analyzer.Testing`.
- The package version available during bootstrap drags Roslyn 1.0.1 compile assets, which conflicts with analyzers built on Roslyn 5.3.0.
- The custom harness injects `.editorconfig` values through a test `AnalyzerConfigOptionsProvider` and verifies IDs plus source locations from inline markup.

## Implications

- Future contributors can extend tests without reintroducing the old package conflict.
- If the official analyzer-testing package aligns with current Roslyn versions later, the harness can be reconsidered.

## Follow-ups

- Re-evaluate the official Roslyn testing package in a future dependency refresh phase.
