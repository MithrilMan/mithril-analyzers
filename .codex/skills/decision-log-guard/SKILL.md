---
name: decision-log-guard
description: Enforce decision-register discipline for this repository. Use when work introduces significant product, architecture, API, data-model, security, dependency, or workflow decisions, so each decision is logged in the registry rooted at docs/specs/DECISIONS.md and potential conflicts are clarified with the user before writing.
---

# Decision Log Guard

## Workflow

1. Determine whether the current task produced a significant decision.
2. Read `docs/specs/DECISIONS.md`, then read `docs/specs/decisions/INDEX.md`.
3. Check overlap and contradiction with existing accepted decisions in the index and any relevant entry files.
4. Stop and ask user clarification if conflict is possible.
5. Create or update one decision file under `docs/specs/decisions/entries/` if no conflict is found.
6. Update `docs/specs/decisions/INDEX.md` in reverse chronological order (newest first).
7. Mark replaced decisions as superseded in both the old entry file and the index row when direction changes.

## Decision Threshold

Log decisions for:

- architecture boundaries and layering
- API contracts and endpoint behavior
- auth, security, or governance policy
- domain invariants and data-model constraints
- dependency strategy and tooling rules
- cross-cutting workflow/process rules

Skip decision entries for:

- local refactors with no policy/behavior impact
- formatting-only or naming-only cleanup
- temporary exploratory commands without lasting implications

## Entry Requirements

When creating or updating a decision entry, include:

- context
- options considered with tradeoffs
- final decision statement
- consequences
- status
- validation or evidence

Use:

- `docs/specs/decisions/_templates/decision-entry.md` for the entry shape
- `DEC-YYYYMMDD-XX-short-title.md` for the entry filename

Ask the user if conflict confidence is low.
