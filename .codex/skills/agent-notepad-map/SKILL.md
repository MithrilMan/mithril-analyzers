---
name: agent-notepad-map
description: Design and evolve the hierarchy of .codex/notes so operational memory stays organized as a navigable tree. Use when new domains emerge, branches become crowded, or note discovery slows down.
---

# Agent Notepad Map

Maintain the note taxonomy as the project evolves.

## Workflow

1. Inspect `.codex/notes/README.md`, `.codex/notes/INDEX.md`, and branch indexes.
2. Distinguish reserved system folders (`inbox`, `archive`, `_templates`) from project/domain branches.
3. Detect structure pressure:
   - too many unrelated topics in one folder
   - topic files that should share a sub-branch
   - repeated naming ambiguity
4. Propose and apply the smallest structural change:
   - create a new branch folder
   - split a crowded branch into focused sub-branches
   - rename ambiguous files or folders
5. Let the agent own the durable taxonomy. Do not pre-create fixed domain branches for every repository; create new branches only when repeated work or repeated notes make them worthwhile.
6. Move note files without changing their factual content.
7. Update every affected `INDEX.md` link.
8. Add one short migration note under `.codex/notes/archive/` when path changes are non-trivial. Create a `structure-history` note only when that history is needed.

## Branching Heuristics

- Reserve only `inbox`, `archive`, and `_templates` as top-level system folders.
- Choose top-level domain names that reflect durable areas in the current repository, not template assumptions.
- Create a new branch only when it reduces future lookup cost more than it increases navigation overhead.
- Prefer depth over giant flat folders after about 7-10 topic files.
- Group by durable execution context first (for example `architecture/constraints`, `workflow/testing`, `integrations/payments`).
- Keep leaf notes topic-specific; avoid long-lived `misc.md` growth.
- Use kebab-case for file and folder names.

## Guardrails

- Keep restructures incremental; avoid large one-shot reorganizations.
- Do not let examples harden into mandatory taxonomy across unrelated repositories.
- Do not rewrite note facts while reorganizing files.
- If a move breaks discoverability, restore and try a smaller split.
