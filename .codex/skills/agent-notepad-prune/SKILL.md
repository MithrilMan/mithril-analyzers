---
name: agent-notepad-prune
description: Reconcile contradictions and remove stale noise in the .codex/notes tree. Use at phase boundaries or when conflicts appear to merge duplicates, mark superseded notes, archive outdated entries, and keep indexes accurate.
---

# Agent Notepad Prune

Keep the memory tree trustworthy and fast to query.

## Workflow

1. Start from `.codex/notes/INDEX.md` and scan recently touched branches plus any affected system folders (`inbox`, `archive`).
2. Find low-value candidates:
   - duplicate topic files
   - contradicted statements
   - unresolved temporary notes that are now obsolete
   - notes without evidence or with broken references
3. Resolve each candidate with the lightest action that preserves trust:
   - merge into the strongest note
   - route or delete stale `draft` notes, especially from `inbox/`
   - revalidate or downgrade `needs-revalidation` notes
   - mark replaced notes as `Status: superseded`
   - keep a short superseded pointer in the original branch when that lookup path still matters
   - move full historical note bodies to `.codex/notes/archive/` only when the history still has value
   - delete only when no historical value exists
4. Update `INDEX.md` links after every move or merge.
5. Re-run quick lookup on the touched branch to confirm discoverability.

## Conflict Resolution Rule

When two notes disagree, keep the claim with the strongest current evidence and newest validation date.
Mark the replaced note with:
- `Status: superseded`
- `Superseded-By: <new note path>`

Treat lifecycle states consistently:
- `draft` -> validate, route, or delete
- `needs-revalidation` -> refresh evidence or quarantine from normal reuse
- `superseded` -> keep only as a pointer/history artifact, not as active guidance

## Output Format

```markdown
## Pruned
- <note path> -> merged/archived/deleted

## Reconciled Conflicts
- <old note path> -> <new note path>

## Still Uncertain
- <note path + required validation>
```

## Guardrails

- Preserve active constraints from user instructions.
- Do not let `archive/` become a second active tree.
- Do not leave `draft` notes in `inbox/` indefinitely once routing becomes clear.
- Archive before deleting when unsure.
- Keep branch indexes aligned with the final file layout.
