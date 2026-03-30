# DECISIONS

Project-level decisions only.
This file is the canonical entrypoint for the decisions registry.
Use it as a router for agents, not as a rolling journal.

## Registry Layout

- `docs/specs/decisions/INDEX.md` - searchable catalog of decisions, newest first.
- `docs/specs/decisions/entries/` - one Markdown file per decision.
- `docs/specs/decisions/_templates/decision-entry.md` - template for new decision entries.

## Workflow

1. Read this file, then read `docs/specs/decisions/INDEX.md`.
2. Search the index for overlapping or conflicting accepted decisions.
3. Open only the candidate decision files you need from `docs/specs/decisions/entries/`.
4. Create or update exactly one entry file per durable project decision.
5. Update the index row and keep entries in reverse chronological order.
6. When a decision changes direction, update the old entry to `superseded` and link the replacement entry.

## Naming Rule

- Use `DEC-YYYYMMDD-XX-short-title.md` for entry files.

## Current Catalog

- Open `docs/specs/decisions/INDEX.md`.
