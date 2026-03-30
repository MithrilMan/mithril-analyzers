---
name: agent-notepad-lookup
description: Traverse the personal note tree in .codex/notes to fetch high-signal context before non-trivial work. Use when starting implementation, debugging, refactoring, architecture changes, or risk analysis; map the task to note branches, read relevant leaf notes, validate critical claims against code, and surface contradictions.
---

# Agent Notepad Lookup

Query notes as a hierarchy of small topic files, not as monolithic logs.

## Workflow

1. Open `.codex/notes/README.md` and `.codex/notes/INDEX.md` when branch routing or lifecycle rules matter.
2. Distinguish system folders (`inbox`, `archive`, `_templates`) from project/domain branches.
3. Map the task to 1-3 likely durable branch paths. Do not assume the same top-level branch names across repositories.
4. Read branch `INDEX.md` files first, then only relevant leaf notes.
5. Rank each note by status, relevance, confidence, and last validation date.
6. Treat `draft` and `needs-revalidation` notes as hints that require re-checking. Ignore `Status: superseded` and `archive/` by default unless tracing history. Read `inbox/` only when the current work is still being routed or the branch choice is unclear.
7. Revalidate any high-impact claim against current code before reuse.
8. Flag contradictions or stale notes for `$agent-notepad-prune`.
9. Return reusable actions and unresolved gaps.

## Output Format

When reporting findings, use:

```markdown
## Relevant Notes
1. [status: active|stable|draft|needs-revalidation] [confidence: high|medium|low] <note path>
   - Why relevant
   - Reusable insight
   - Evidence checked

## Apply Now
- <actions to execute immediately>

## Revalidate / Prune
- <note path + reason>
```

## Guardrails

- Prefer leaf files over broad summaries.
- Ignore notes marked `Status: superseded` unless tracing history.
- Ignore `archive/` during normal lookup unless you need path history or replaced note bodies.
- Treat `inbox/` as a temporary staging area, not as a durable knowledge branch.
- Treat `needs-revalidation` and `draft` as lower-trust inputs than `active` or `stable`.
- Stop reading when additional files do not change the plan.
- If no note branch matches, state it and continue with fresh analysis.
