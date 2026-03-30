# Notes Index

Use this file as the root navigator for the active note tree.
Durable domain branches are project-specific and agent-owned; only `inbox/`, `archive/`, and `_templates/` are reserved system folders.

## System Folders

- `inbox/INDEX.md` - short-lived captures awaiting routing or stronger validation.
- `archive/INDEX.md` - historical note bodies and structure history excluded from normal lookup.
- `_templates/` - reusable note skeletons.

## Current Domain Branches

- `architecture/INDEX.md` - project shape, package boundaries, and durable structural constraints.
- `rules/INDEX.md` - shipped diagnostic scope and rule-behavior notes.
- `workflow/INDEX.md` - validation, testing, and delivery workflow notes.

## Pinned Leaf Notes

- `architecture/project-bootstrap.md` - bootstrap decisions for package identity, solution layout, and analyzer-only packing.
- `workflow/testing-harness.md` - why tests use a direct Roslyn/xUnit harness in this bootstrap phase.

## Rules

- Create new domain branches only when they reduce future lookup cost.
- Keep one narrow topic per file.
- `Pinned Leaf Notes` is curated, not exhaustive.
- Prefer creating a new child file over growing a mixed note.
- Link every new leaf note in its nearest branch index.
