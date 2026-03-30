# Personal Notes Workspace

This folder is the agent's operational memory for this repository.
Use it as a tree of small topic notes, not as a single running journal.

## Structure Model

- Treat durable domain branches as agent-owned and project-specific.
- Reserve only `inbox/`, `archive/`, and `_templates/` as system folders.
- Start from `.codex/notes/INDEX.md` to navigate active branches.
- Keep one narrow topic per leaf file and one `INDEX.md` per branch.

## Entry Points

- Start from `.codex/notes/INDEX.md` to navigate active branches.
- Use branch `INDEX.md` files to locate leaf notes quickly.
- Use templates in `.codex/notes/_templates/` for new notes and indexes.

## Routing Heuristics

- Create the smallest durable branch path that will make future lookup cheaper.
- Examples only: `architecture/constraints`, `workflow/testing`, `integrations/payments`.
- Use `inbox/` only for short-lived captures when branch choice or evidence is still unclear.
- Create a new domain branch only after repeated work or repeated notes show a stable retrieval pattern.

## Lifecycle

1. Lookup before non-trivial work.
2. Capture new learning in the best current branch, using `draft` when confidence or routing is still weak.
3. Promote or downgrade note status as evidence becomes stronger, weaker, or older.
4. Prune contradictions and stale notes at phase boundaries.

## Status Meanings

- `draft` - provisional capture, weak evidence, or unresolved routing. Do not trust for high-impact reuse without re-checking.
- `active` - currently trusted note that guides day-to-day execution.
- `stable` - durable fact or workflow rule revalidated over time.
- `needs-revalidation` - probably still useful, but too stale or environment-sensitive to reuse without a fresh check.
- `superseded` - replaced by a newer note on the same topic. Keep only as a pointer or history artifact.

## Superseded and Archive Policy

- Use `Superseded-By` when one note replaces another on the same topic.
- Keep a short superseded pointer in the original branch when that path is still a common lookup route.
- Move the full historical body to `archive/` only when history still matters.
- Normal lookup should ignore `archive/` and `Status: superseded` unless tracing history.
