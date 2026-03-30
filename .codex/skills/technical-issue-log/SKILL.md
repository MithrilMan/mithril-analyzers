---
name: technical-issue-log
description: Record solved technical problems for this repository in the registry rooted at docs/specs/TECHNICAL_ISSUES.md. Use when a technical issue has been diagnosed and resolved (build/test failures, runtime errors, integration bugs, environment problems), so future occurrences can be fixed faster.
---

# Technical Issue Log

## Workflow

1. Confirm the issue is solved and reproducible fix steps are known.
2. Read `docs/specs/TECHNICAL_ISSUES.md`, then read `docs/specs/technical-issues/INDEX.md`.
3. Check whether an equivalent issue already exists (same error signature and root cause).
4. If an equivalent issue exists, update that entry file with the new context instead of duplicating it.
5. If not, create a new file under `docs/specs/technical-issues/entries/`.
6. Update `docs/specs/technical-issues/INDEX.md` and keep content concise and operational.

## Logging Rules

- Log only resolved technical issues.
- Include exact symptoms and affected files/commands.
- Include root cause, implemented fix, and validation evidence.
- Include prevention notes when useful.
- Do not log speculative causes as final diagnosis.

## Entry Format

Use this structure when creating or updating an issue entry:

- Template: `docs/specs/technical-issues/_templates/technical-issue-entry.md`
- Filename: `ISSUE-YYYYMMDD-XX-short-title.md`

## Relationship With Lookup Skill

- When a problem is solved, use this skill to persist the solution.
- For new incidents, use `$technical-issue-lookup` first to search existing fixes.
