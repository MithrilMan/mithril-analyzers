# TECHNICAL ISSUES

Resolved technical incidents only.
This file is the canonical entrypoint for the technical-issue registry.
Use it as a router for agents, not as a rolling journal.

## Registry Layout

- `docs/specs/technical-issues/INDEX.md` - searchable catalog of resolved issues, newest first.
- `docs/specs/technical-issues/entries/` - one Markdown file per resolved issue.
- `docs/specs/technical-issues/_templates/technical-issue-entry.md` - template for new issue entries.

## Workflow

1. Read this file, then read `docs/specs/technical-issues/INDEX.md`.
2. Search the index for matching symptoms, areas, and root-cause patterns.
3. Open only the candidate issue files you need from `docs/specs/technical-issues/entries/`.
4. Update an existing issue file when the signature and root cause match; otherwise create a new entry file.
5. Update the index row and keep entries in reverse chronological order.
6. Log only validated fixes for resolved incidents.

## Naming Rule

- Use `ISSUE-YYYYMMDD-XX-short-title.md` for entry files.

## Current Catalog

- Open `docs/specs/technical-issues/INDEX.md`.
