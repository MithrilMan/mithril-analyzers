---
name: technical-issue-lookup
description: Consult the repository technical issue knowledge base rooted at docs/specs/TECHNICAL_ISSUES.md when technical problems occur. Use before deep debugging to quickly identify prior matching incidents, validated fixes, and likely root causes.
---

# Technical Issue Lookup

## Workflow

1. Parse the current issue signal (error text, stack trace, failing command, affected file/module).
2. Read `docs/specs/TECHNICAL_ISSUES.md`, then read `docs/specs/technical-issues/INDEX.md`.
3. Search the index for matching entries by:
   - exact error strings
   - subsystem/area
   - similar root-cause pattern
4. Open only the candidate issue files that look relevant from `docs/specs/technical-issues/entries/`.
5. Rank matches: exact > strong pattern > weak pattern.
6. Propose the best known fix first, with confidence and references.
7. If no strong match exists, continue standard troubleshooting and, once solved, invoke `$technical-issue-log`.

## Output Format

When matches exist, return:

```markdown
## Prior Matches
1. [ISSUE-ID] - confidence: high|medium|low
   - Why it matches
   - Suggested fix summary
   - Reference: docs/specs/technical-issues/entries/<file>.md

## Recommended Next Step
- Apply [ISSUE-ID] fix first / proceed with fresh diagnosis
```

When no match exists, return:

```markdown
No prior validated solution found in `docs/specs/technical-issues/INDEX.md`.
Proceed with fresh diagnosis; log final resolution via $technical-issue-log.
```

## Guardrails

- Prefer fixes with explicit validation evidence.
- Do not claim certainty when confidence is low.
- Do not overwrite issue history; use `$technical-issue-log` for new solved incidents.
